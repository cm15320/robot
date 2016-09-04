using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NatNetML;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace robotTracking
{
    class Experiment
    {
        private bool dummyExperiment = false;

        public float inverseSqrt2Pi = 1f / (float)Math.Sqrt(2 * Math.PI);
        private RobotControl controller;
        private RigidBodyData robotBase, robotTip;
        private CalibrationData newCalibrationData = new CalibrationData();
        private CalibrationData storedCalibrationData, storedTestData;
        //private int calibrationStep = 0;
        private int[] motorAngles = new int[] { 90, 90, 90, 90 };
        private int numIterationPoints = 5;
        private int numTestPoints = 3;
        private int numPoints;
        private int maxAngle = 120;
        private float[] relativeTipAngles = new float[] { 0.0f, 0.0f, 0.0f };
        private float[] relativeTipPos = new float[] { 0.0f, 0.0f, 0.0f };
        private FrameOfMocapData currentFrame;
        private object syncLock;
        private bool calibrating = false;
        private bool pausedCalibration = false; // this can indicate the pause of the calibration and the test stage
        private const float startingAlpha = 0.0008f;
        private StringBuilder csv = new StringBuilder();
        private const int motorScaler = 10000;

        private Hashtable htRigidBodiesNameToBody = new Hashtable();
        private double distanceBetween;

        private XmlSerializer ser = new XmlSerializer(typeof(CalibrationData));
        private string calibrationFilename = "calibrationData.xml";
        private string testDataFilename = "testPoints.xml";
        private bool currentlyTracked;

        private NatNetClientML m_NatNet;



        //private string[] bodyNames = new string[] { "robotBase", "robotTip" };
        // need to store the name of the rigid bodies mapped against their ID
        private Hashtable rigidBodiesIDtoName = new Hashtable();
        // private List<RigidBody> blocks;

        public Experiment(bool realExperiment)
        {
            if(realExperiment == false)
            {
                dummyExperiment = true;
                controller = new RobotControl();
                syncLock = new object();
                m_NatNet = new NatNetClientML();
            }
        }

        // can just have one RobotControl controller in the future, in this class only, not needed from RobotTracker
        public Experiment(RobotControl controller, List<RigidBody> mRigidBodies, object syncLock, NatNetML.NatNetClientML m_NatNet)
        {
            this.syncLock = syncLock;
            this.controller = controller;
            this.m_NatNet = m_NatNet;

            //for(int i = 0; i < currentFrame.nRigidBodies; i++ )
            //{
            //    RigidBodyData rb = currentFrame.RigidBodies[i];
            //    if(rb != null)
            //    {
            //        if (robotBase == null) robotBase = rb;
            //        else if (robotTip != null) robotTip = rb;
            //    }
            //}
            htRigidBodiesNameToBody.Add("robotBase", new RigidBodyData());
            htRigidBodiesNameToBody.Add("robotTip", new RigidBodyData());

            //string[] requiredNames = new string[] { "robotBase", "robotTip" };
            if(mRigidBodies.Count == 0)
            {
                Console.WriteLine("Error, no rigid body info stored");
            }
            foreach(RigidBody rb in mRigidBodies)
            {
                // if bodies to id ht contatins key (rb. name) then add it to ID to Name ht
                if(htRigidBodiesNameToBody.ContainsKey(rb.Name))
                {
                    int key = rb.ID.GetHashCode();
                    rigidBodiesIDtoName.Add(key, rb.Name);
                }
            }
            if(rigidBodiesIDtoName.Count == 0)
            {
                Console.WriteLine("Error, not added necessary rigid bodies to the hash table");
            }

            initialiseMotors();
        }

        // perhaps put a lock on this in case the currentFrame object is changed mid-read???????
        public void update(NatNetML.FrameOfMocapData newCurrentFrame)
        {
            // get the correct rigid body data objects from the frame
            this.currentFrame = newCurrentFrame;
            getCurrentData();

            calculateDistanceBetween();
        }

        private void initialiseMotors()
        {
            if(controller.isConnected())
            {
                controller.zeroMotors();
            }
        }


        
        public void stopCalibration()
        {
            calibrating = false;
        }

        private void calculateDistanceBetween()
        {
            // If this doesn't work then get hash code from the string and use that instead
            robotBase = (RigidBodyData)htRigidBodiesNameToBody["robotBase"];
            robotTip = (RigidBodyData)htRigidBodiesNameToBody["robotTip"];

            float x1 = robotBase.x;
            float y1 = robotBase.y;
            float z1 = robotBase.z;

            float x2 = robotTip.x;
            float y2 = robotTip.y;
            float z2 = robotTip.z;

            double distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2) + Math.Pow(z1 - z2, 2));
            distanceBetween = distance;
        }

        public double getDistanceBetween()
        {
            return distanceBetween;
        }

        public double[] testRegression(float[] inputVectorTarget, RegressionInput inputType, float bandwidth = startingAlpha)
        {
            double[] output = NWRegression(inputVectorTarget, inputType, bandwidth);
            //if(controller.isConnected())
            //{
            //    controller.setMotorAnglesTest(output);
            //}

            return output;
        }

        private void convertMotors(float[] origMotorAngles, bool reduce)
        {
            for(int i = 0; i < origMotorAngles.Length; i++)
            {
                if (reduce) origMotorAngles[i] = origMotorAngles[i] / motorScaler;
                else origMotorAngles[i] = origMotorAngles[i] * motorScaler;
            }
        }

        private void convertOutputMotors(double[] origMotorAngles, bool reduce)
        {
            for (int i = 0; i < origMotorAngles.Length; i++)
            {
                if (reduce) origMotorAngles[i] = origMotorAngles[i] / motorScaler;
                else origMotorAngles[i] = origMotorAngles[i] * motorScaler;
            }

        }

        private float[] getCopyVector(float[] inputVectorTarget)
        {
            float[] copyVector = new float[inputVectorTarget.Length];
            for (int i = 0; i < copyVector.Length; i++)
            {
                copyVector[i] = inputVectorTarget[i];
            }

            return copyVector;
        }

        private double[] NWRegression(float[] inputVectorTarget, RegressionInput inputType, float alpha = startingAlpha)
        {
            int numOutputDimensions = getNumOutputDimensions(inputType);
            double[] outputVector = new double[numOutputDimensions];
            double[] sumNumerator = new double[numOutputDimensions];
            double sumDenominator = 0;

            float[] newInputVector = getCopyVector(inputVectorTarget);


            if(inputType == RegressionInput.MOTORS)
            {
                convertMotors(newInputVector, true);
            }

            // If not using KD tree, loop through entire set of data
            for(int i = 0; i < storedCalibrationData.Count; i++)
            {
                float kernelInput = getKernelInput(i, inputType, alpha, newInputVector);
                float[] currentYValue = getcurrentYValue(i, inputType);
                //Console.WriteLine("current vector output is  :");
                //for (int n = 0; n < currentYValue.Length; n++)
                //{
                //    Console.Write(currentYValue[n] + "   ");
                //}
                //Console.WriteLine();

                // work out the values for the numerator and denominator to be added to sum
                //Console.WriteLine("output of kernel function is " + kernelFunction(kernelInput));
                for (int k = 0; k < currentYValue.Length; k++)
                {
                    sumNumerator[k] += currentYValue[k] * kernelFunction(kernelInput);
                }
                sumDenominator += kernelFunction(kernelInput);
                //Console.WriteLine("sum denominator is " + sumDenominator);
            }
            
            for(int k = 0; k < numOutputDimensions; k++)
            {
                outputVector[k] = sumNumerator[k] / sumDenominator;
            }

            if (inputType != RegressionInput.MOTORS)
            {
                convertOutputMotors(outputVector, false);
            }


            return outputVector;

        }


        private double kernelFunction(float kernelInput)
        {
            double exponent = -0.5 * Math.Pow(kernelInput, 2);
            //Console.WriteLine("exponent is " + exponent);
            double output = inverseSqrt2Pi * Math.Exp(exponent);
            //Console.WriteLine("e to the exponent is " + Math.Exp(exponent));
            //Console.WriteLine("output is " + output);

            return output;
        }


        private float[] getcurrentYValue(int i, RegressionInput inputType)
        {
            DataPoint currentDataPoint = storedCalibrationData[i];
            float[] currentVector;
            if(inputType == RegressionInput.MOTORS)
            {
                // If want to find a given config that yields certain motor angles, could use tip position, orientation or both
                // just use tip position for now (will very rarely want to know what position matches to motor angles anyway, will be the other way round)
                currentVector = currentDataPoint.relativeTipPosition;
            }
            else
            {
                // Otherwise of course the output will need to be the motor angle vector so this will be the yi value

                // again have to do this temporary conversion but will change this of course!!!!!!!!!!!!!
                currentVector = new float[4];
                currentVector[0] = currentDataPoint.motorAngles[0];
                currentVector[1] = currentDataPoint.motorAngles[1];
                currentVector[2] = currentDataPoint.motorAngles[2];
                currentVector[3] = currentDataPoint.motorAngles[3];
                convertMotors(currentVector, true);
                //currentVector = currentDataPoint.motorAngles;

            }

            return currentVector;
        }

        
        private float getKernelInput(int i, RegressionInput inputType, float alpha, float[] inputVectorTarget)
        {
            DataPoint currentDataPoint = storedCalibrationData[i];
            float[] currentVector;
            float difference = 0, kernelInput;


            // this will be a lot shorter!!!!!!!!!!!!!!!!!!!!!!
            if (inputType == RegressionInput.MOTORS)
            {
                // Need to use motor angles if they are not the input used to find what matches them

                // convert to float here (don't need to do this in the future as can just save the motor angles as float array anyway)
                currentVector = new float[4];
                currentVector[0] = currentDataPoint.motorAngles[0];
                currentVector[1] = currentDataPoint.motorAngles[1];
                currentVector[2] = currentDataPoint.motorAngles[2];
                currentVector[3] = currentDataPoint.motorAngles[3];


                convertMotors(currentVector, true);
            }
            else if (inputType == RegressionInput.POSITION)
            {
                // Need to use position and orientation vector to get what matches the motors

                // will eventually just use the combined array that will be in the DataPoint, but doing testing
                currentVector = currentDataPoint.relativeTipPosition;
                //currentVector = new float[3];
                //currentVector[0] = currentDataPoint.relativeTipPosition[0];
                //currentVector[1] = currentDataPoint.relativeTipPosition[1];
                //currentVector[2] = currentDataPoint.relativeTipPosition[2];
                //currentVector[3] = currentDataPoint.relativeTipOrientation[0];
                //currentVector[4] = currentDataPoint.relativeTipOrientation[1];
                //currentVector[5] = currentDataPoint.relativeTipOrientation[2];
            }
            else if (inputType == RegressionInput.ORIENTATION)
            {
                currentVector = currentDataPoint.relativeTipOrientation;
            }
            // temporary while testing, should be the combination of both
            else currentVector = currentDataPoint.relativeTipPosition;
            
            // Work out the difference between the vectors
            for (int j = 0; j < currentVector.Length; j++)
            {
                difference += (float)Math.Pow((inputVectorTarget[j] - currentVector[j]), 2);
            }
            //Console.WriteLine("difference is " + difference);

            difference = (float)Math.Sqrt(difference);

            //Console.WriteLine("difference is " + difference);
            //Console.WriteLine("alpha is " + alpha);

            kernelInput = difference / alpha;

            //Console.WriteLine("the kernel parameters are:");
            //for (int m = 0; m < inputVectorTarget.Length; m++)
            //{
            //    Console.WriteLine(inputVectorTarget[m] + " : " + currentVector[m]);
            //}

            //Console.WriteLine("and the kernel input was   " + kernelInput);

            return kernelInput;
            
        }


        // can just make simpler by only needing the input type really
        private int getNumOutputDimensions(RegressionInput inputType)
        {
            if (inputType == RegressionInput.MOTORS)
            {
                // If given motor angles as target, return 3 dimensions (for position corresponding to this, in reality this will be rare as will be given something else for target)
                return 3;
            }
            else 
            {
                // If given position or orientation vectors or both, return 4 dimensions for the motor
                return 4;
            }
        }


        public void calibrate(bool testPoints)
        {
            int startingServoIndex = 0;
            string filename;
            if (controller.isConnected()) controller.shareMotorAngles(motorAngles);
            calibrating = true;

            if (testPoints) filename = "testPoints.xml";
            else filename = "calibrationData.xml";

            calibrate(startingServoIndex, true, testPoints);
            
            Console.WriteLine("finished");

            saveDataXML(filename);


        }

        // calibrate function and this one can be the same thing with different parameters passed in or
        // something to indicate whether it is calibrating or testing

        private void calibrate(int motorIndex, bool rising, bool testPoints)
        {
            int numPoints, numSteps, localMaxAngle, activeRange, step, cnt = 1;
            int angle, startingAngle;
            string filename;
            bool newRising;

            if (testPoints) filename = "testPoints.xml";
            else filename = "calibrationData.xml";
            // similar to calibrate funtion, but instead does a much smaller subset of points and aims to deliberately 'miss'
            // points covered by the calibration function
            if (testPoints) localMaxAngle = maxAngle - 10; // smaller than the real max angle as that will be covered by calibrate
            else localMaxAngle = maxAngle;

            activeRange = (localMaxAngle - 90) * 2; // active range will thus be 10 smaller on each side if testPoints

            if (testPoints) numPoints = numTestPoints;
            else numPoints = numIterationPoints;

            numSteps = numPoints - 1;
            step = activeRange / numSteps;
            startingAngle = 90 - (localMaxAngle - 90);
            if(testPoints) startingAngle -= 5; // offset it by 5 degrees to the left to cover angles not covered by calibration alone


            // if should be descending then just start at the extreme and come down
            if (!rising)    startingAngle += numSteps * step;

            for (int i = 0; i < numPoints; i++)
            {
                if (rising) angle = startingAngle + (i * step); // rise up
                else angle = startingAngle - (i * step); // come down

                cnt++;
                if (cnt % 2 == 0)   newRising = true;
                else    newRising = false;
                motorAngles[motorIndex] = angle;

                if (!calibrating)
                {
                    saveDataXML(filename);
                    return;
                }
                if (motorIndex < 3)     calibrate(motorIndex + 1, newRising, testPoints);
                else if (motorIndex == 3)
                {
                    Console.WriteLine("motor angles are {0} {1} {2} {3}", motorAngles[0], motorAngles[1], motorAngles[2], motorAngles[3]);
                    // move the motors to new positions
                    // sleep for about a second

                    //Console.WriteLine("setting motor angles");
                    setMotorAngles();
                    Thread.Sleep(1000);
                    // log the new data to the list, it will be already updated every time update is called from another thread
                    logCalibrationData();

                    checkForPause();
                }
            }

        }

        private void setMotorAngles()
        {
            if(controller.isConnected())
            {
                controller.setMotorAngles();
            } 
        }


        //public void calibrate()
        //{
        //    int startingServoIndex = 0;
        //    if(controller.isConnected())    controller.shareMotorAngles(motorAngles);
        //    calibrating = true;
        //    calibrate(startingServoIndex, true);
        //    Console.WriteLine("calibration finished");

        //    if(!dummyExperiment)    saveDataXML("calibrationData.xml");
        //}

        // make this a LOT neater
        //private void calibrate(int motorIndex, bool rising)
        //{
        //    int localMaxAngle = maxAngle;
        //    // move greater angles if it is one of the DOF at the tip???
        //    //if(motorIndex > 1)
        //    //{
        //    //    localMaxAngle += 20;
        //    //}
        //    int activeRange = (localMaxAngle - 90) * 2;
        //    int numCalibrationSteps = numIterationPoints - 1;
        //    int step = activeRange / numCalibrationSteps;
        //    //int shiftAngle = 90 - (step * (numCalibrationSteps / 2));
        //    int angle;
        //    int startingAngle = 90 - (localMaxAngle - 90);
        //    int cnt = 1; // could make this start at 2 if want angles to begin opposed (may make more smooth to start)

        //    if (!rising)
        //    {
        //        startingAngle += numCalibrationSteps * step;
        //    }

        //    for (int i = 0; i < numIterationPoints; i++)
        //    {
        //        cnt++;
        //        //int angle = 90 + (i * step);
        //        //angle = angle % maxAngle;
        //        //if (angle < 90) angle += shiftAngle;
        //        if (rising) angle = startingAngle + (i * step);
        //        else angle = startingAngle - (i * step);

        //        motorAngles[motorIndex] = angle;

        //        if (!calibrating)
        //        {
        //            if (!dummyExperiment) saveDataXML("calibrationData.xml");
        //            return;
        //        }
        //        if (motorIndex < 3)
        //        {
        //            bool newRising;
        //            if (cnt % 2 == 0) newRising = true;
        //            else newRising = false;
        //            calibrate(motorIndex + 1, newRising);
        //        }
        //        else if (motorIndex == 3)
        //        {
        //            Console.WriteLine("motor angles are {0} {1} {2} {3}", motorAngles[0], motorAngles[1], motorAngles[2], motorAngles[3]);
        //            // move the motors to new positions
        //            // sleep for about a second
        //            if (controller.isConnected())
        //            {
        //                //Console.WriteLine("setting motor angles");
        //                controller.setMotorAngles();
        //            }
        //            Thread.Sleep(1000);
        //            // log the new data to the list, it will be already updated every time update is called from another thread
        //            if (!dummyExperiment) logCalibrationData();

        //            checkForPause();

        //        }
        //    }

        //}




        public void makeDummy()
        {
            dummyExperiment = true;
        }


        private bool getData(string filename)
        {
            XmlSerializer reader = new XmlSerializer(typeof(CalibrationData));
            StreamReader file;
            try
            {
                file = new StreamReader(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not open " + filename + " message: " + ex.Message);
                return false;
            }
            CalibrationData dataReadIn = (CalibrationData)reader.Deserialize(file);
            if (filename.Equals(calibrationFilename))
            {
                storedCalibrationData = dataReadIn;
            }
            else if (filename.Equals(testDataFilename))
            {
                storedTestData = dataReadIn;
            }
            else return false;
            file.Close();
            //for (int i = 0; i < storedCalibrationData.Count; i++)
            //{
                //Console.WriteLine("for data point number " + i);
                //Console.WriteLine("motor angles:");
                //for (int j = 0; j < storedCalibrationData[i].motorAngles.Length; j++)
                //{
                //    Console.Write(storedCalibrationData[i].motorAngles[j] + "   ");
                //}
                //Console.WriteLine("tip position:");
                //for (int j = 0; j < storedCalibrationData[i].relativeTipPosition.Length; j++)
                //{
                //    Console.WriteLine(storedCalibrationData[i].relativeTipPosition[j] + "   ");
                //}
                //Console.WriteLine();
            //}

            if (dataReadIn.Count <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        //public bool getAllData()
        //{
        //    bool gotCalibrationData = getCalibrationData();
        //    bool gotTestData = getTestData();

        //    if(gotCalibrationData && gotTestData)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public bool getTestData()
        {
            return getData(testDataFilename);
        }

        public bool getCalibrationData()
        {

            return getData(calibrationFilename);


            //XmlSerializer reader = new XmlSerializer(typeof(CalibrationData));
            //StreamReader file;
            //try
            //{
            //    file = new StreamReader(calibrationFilename);
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine("Could not open " + calibrationFilename + " message: " + ex.Message);
            //    return false;
            //}
            //storedCalibrationData = (CalibrationData)reader.Deserialize(file);
            //file.Close();
            //for (int i = 0; i < storedCalibrationData.Count; i++)
            //{
            //    //Console.WriteLine("for data point number " + i);
            //    //Console.WriteLine("motor angles:");
            //    //for (int j = 0; j < storedCalibrationData[i].motorAngles.Length; j++)
            //    //{
            //    //    Console.Write(storedCalibrationData[i].motorAngles[j] + "   ");
            //    //}
            //    //Console.WriteLine("tip position:");
            //    //for (int j = 0; j < storedCalibrationData[i].relativeTipPosition.Length; j++)
            //    //{
            //    //    Console.WriteLine(storedCalibrationData[i].relativeTipPosition[j] + "   ");
            //    //}
            //    //Console.WriteLine();
            //}

            //if(storedCalibrationData.Count <= 0)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}

        }

        private void getCurrentData()
        {
            lock(syncLock)
            {
                // loop through RigidBody data
                for (int i = 0; i < currentFrame.nRigidBodies; i++)
                {
                    NatNetML.RigidBodyData rb = currentFrame.RigidBodies[i];
                    currentlyTracked = rb.Tracked;
                    // get the hashcode of the id for later displaying in grid form
                    int keyID = rb.ID.GetHashCode();
                    if (rigidBodiesIDtoName.ContainsKey(keyID))
                    {
                        string name = (string)rigidBodiesIDtoName[keyID];
                        htRigidBodiesNameToBody[name] = rb;
                        // THIS MAY NOT BE NEEDED NOW AS JUST UPDATING THE HT WITH THE STRING EQUAL TO THE NAME
                        if (name.Equals("robotBase")) robotBase = rb;
                        else if (name.Equals("robotTip")) robotTip = rb;

                    }
                }
                getRelativeTipInfo();

            }
        }

        public void getBandwidthErrorPlot()
        {
            float alpha = 0.000f;
            for (int i = 0; i < 50; i++)
            {
                alpha += 0.0001f;
                testAlphaValueFindPositions(alpha);
                //testAlphaValueFindMotors(alpha);
            }

            File.WriteAllText("bandwidthResults.csv", csv.ToString());
        }

        private void testAlphaValueFindPositions(float alpha)
        {
            float totalDiff = 0f;
            float meanDiff;
            foreach (DataPoint currentDataPoint in storedTestData)
            {
                float[] currentMotorAngles = currentDataPoint.motorAngles;
                float[] realPositionMatch = currentDataPoint.relativeTipPosition;


                double[] regressionPositionMatch = NWRegression(currentMotorAngles, RegressionInput.MOTORS, alpha);
                //Console.WriteLine("real position match starts with " + realPositionMatch[0]);
                //Console.WriteLine("regression position match starts with " + regressionPositionMatch[0]);

                double xDiff = regressionPositionMatch[0] - realPositionMatch[0];
                double yDiff = regressionPositionMatch[1] - realPositionMatch[1];
                double zDiff = regressionPositionMatch[2] - realPositionMatch[2];

                float diff = (float)(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2) + Math.Pow(zDiff, 2));
                diff = (float)Math.Sqrt(diff);

                totalDiff += diff;

            }
            meanDiff = totalDiff / storedTestData.Count;
            string newLine = String.Format("{0},{1}", alpha, meanDiff);
            csv.AppendLine(newLine);
        }


        private void testAlphaValueFindMotors(float alpha)
        {
            float totalDiff = 0f;
            float meanDiff;
            foreach (DataPoint currentDataPoint in storedTestData)
            {
                float[] currentPosition = currentDataPoint.relativeTipPosition;
                float[] realMotorsMatch = currentDataPoint.motorAngles;


                double[] regressionMotorsMatch = NWRegression(currentPosition, RegressionInput.POSITION, alpha);
                //Console.WriteLine("regression motor match starts with " + regressionPositionMatch[0]);

                double m1Diff = regressionMotorsMatch[0] - realMotorsMatch[0];
                double m2Diff = regressionMotorsMatch[1] - realMotorsMatch[1];
                double m3Diff = regressionMotorsMatch[2] - realMotorsMatch[2];
                double m4Diff = regressionMotorsMatch[3] - realMotorsMatch[3];


                float diff = (float)(Math.Pow(m1Diff, 2) + Math.Pow(m2Diff, 2) + Math.Pow(m3Diff, 2) + Math.Pow(m4Diff, 2));
                diff = (float)Math.Sqrt(diff);

                totalDiff += diff;

            }
            meanDiff = totalDiff / storedTestData.Count;
            string newLine = String.Format("{0},{1}", alpha, meanDiff);
            csv.AppendLine(newLine);
        }




        private void getRelativeTipInfo()
        {
            float xDiff = robotTip.x - robotBase.x;
            float yDiff = robotTip.y - robotBase.y;
            float zDiff = robotTip.z - robotBase.z;

            // Convert quaternion to eulers.  Motive coordinate conventions: X(Pitch), Y(Yaw), Z(Roll), Relative, RHS
            float[] quatRobotTip = new float[4] { robotTip.qx, robotTip.qy, robotTip.qz, robotTip.qw };
            float[] quatRobotBase = new float[4] { robotBase.qx, robotBase.qy, robotBase.qz, robotBase.qw };
            float[] eulersRobotTip = new float[3];
            float[] eulersRobotBase = new float[3];
            eulersRobotTip = m_NatNet.QuatToEuler(quatRobotTip, (int)NATEulerOrder.NAT_XYZr);
            eulersRobotBase = m_NatNet.QuatToEuler(quatRobotBase, (int)NATEulerOrder.NAT_XYZr);

            float xRDiff = (float)RobotTracker.RadiansToDegrees(eulersRobotTip[0] - eulersRobotBase[0]);     // convert to degrees
            float yRDiff = (float)RobotTracker.RadiansToDegrees(eulersRobotTip[1] - eulersRobotBase[1]);
            float zRDiff = (float)RobotTracker.RadiansToDegrees(eulersRobotTip[2] - eulersRobotBase[2]);

            relativeTipPos[0] = xDiff;
            relativeTipPos[1] = yDiff;
            relativeTipPos[2] = zDiff;

            relativeTipAngles[0] = xRDiff;
            relativeTipAngles[1] = yRDiff;
            relativeTipAngles[2] = zRDiff;
        }




        // This method checks to see if the calibration should be paused, whether from user or if batteries run out
        private void checkForPause()
        {
            // If not moving then should pause
            if( !pausedCalibration && notMoving())
            {
                pausedCalibration = true;
                // trim the last value from the calibration data as it shouldn't count
                newCalibrationData.RemoveAt(newCalibrationData.Count - 1);
                Console.WriteLine("Calibration has been paused due to lack of motion");
            }

            while(pausedCalibration && calibrating)
            {
                Thread.Sleep(5000);
            }

            return;
        }


        // Check if not moving by taking the last two data points and checking if the difference is minimal
        private bool notMoving()
        {
            if (newCalibrationData.Count < 2) return false;

            DataPoint currentDataPoint = newCalibrationData[newCalibrationData.Count - 1];
            DataPoint lastDataPoint = newCalibrationData[newCalibrationData.Count - 2];

            float xdiff = Math.Abs(currentDataPoint.relativeTipPosition[0] - lastDataPoint.relativeTipPosition[0]);
            float ydiff = Math.Abs(currentDataPoint.relativeTipPosition[1] - lastDataPoint.relativeTipPosition[1]);
            float zdiff = Math.Abs(currentDataPoint.relativeTipPosition[2] - lastDataPoint.relativeTipPosition[2]);

            if(xdiff < 0.0001 && ydiff < 0.0001 && zdiff < 0.0001)
            {
                return true;
            }

            return false;
        }


        public void pauseCalibration()
        {
            pausedCalibration = true;
        }

        public void resumeCalibration()
        {
            pausedCalibration = false;
        }



        //private void testFullMotion(int startingServo)
        //{
        //    byte[] instructionBuffer = new byte[2];
        //    instructionBuffer[0] = Convert.ToByte(startingServo);

        //    for (int i = 6; i < 13; i++)
        //    {
        //        int angle = i * 10;
        //        instructionBuffer[1] = Convert.ToByte(angle);
        //        currentPort.Write(instructionBuffer, 0, 2);
        //        Console.WriteLine("servo " + startingServo + " receiving angle " + angle);
        //        Thread.Sleep(1000);
        //        cnt++;

        //        if (startingServo < 4)
        //        {
        //            testFullMotion(startingServo + 1);
        //        }

        //        Console.WriteLine("count is " + cnt);
        //    }
        //}


        private void logCalibrationData() {

            if (dummyExperiment) return;

            DataPoint newDataPoint = new DataPoint();
            // lock so it doesn't change as adding it to the list of data points
            lock(syncLock)
            {
                if (!currentlyTracked)
                {
                    Console.WriteLine("not logged a data point due to untracked rigid body");
                    return;
                }
                newDataPoint.setMotorAngles(motorAngles);
                newDataPoint.setTipOrientation(relativeTipAngles);
                newDataPoint.setTipPos(relativeTipPos);

                newCalibrationData.Add(newDataPoint);
            }
        }

        private void saveDataXML(string filename)
        {
            if(dummyExperiment)
            {
                return;
            }

            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, newCalibrationData);
            writer.Close();
            Console.WriteLine("should have written to " + filename);
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public enum RegressionInput
        {
            MOTORS,
            POSITION,
            ORIENTATION,
            BOTH
        }
    }
}
