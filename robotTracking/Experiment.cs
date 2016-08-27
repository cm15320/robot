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
        private CalibrationData testData = new CalibrationData();
        //private int calibrationStep = 0;
        private int[] motorAngles = new int[] { 90, 90, 90, 90 };
        private int numIterationPoints = 5;
        private int numCalibrationSteps;
        private int maxAngle = 120;
        private float[] relativeTipAngles = new float[] { 0.0f, 0.0f, 0.0f };
        private float[] relativeTipPos = new float[] { 0.0f, 0.0f, 0.0f };
        private FrameOfMocapData currentFrame;
        private object syncLock;
        private bool calibrating = false;
        private CalibrationData activeCalibrationData;
        private bool pausedCalibration = false;
        private const float startingAlpha = 0.005f;

        private Hashtable htRigidBodiesNameToBody = new Hashtable();
        private double distanceBetween;

        private XmlSerializer ser = new XmlSerializer(typeof(CalibrationData));
        private string filename = "calibrationDataNew.xml";

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

        public float[] testRegression(float[] inputVectorTarget, RegressionInput inputType)
        {
            float[] output = NWRegression(inputVectorTarget, inputType);
            if(controller.isConnected())
            {
                controller.setMotorAnglesTest(output);
            }

            return output;
        }

        private float[] NWRegression(float[] inputVectorTarget, RegressionInput inputType, float alpha = startingAlpha)
        {
            int numOutputDimensions = getNumOutputDimensions(inputType);
            float[] sumNumerator = new float[numOutputDimensions];
            float[] outputVector = new float[numOutputDimensions];
            float sumDenominator = 0;

            // If not using KD tree, loop through entire set of data
            for(int i = 0; i < activeCalibrationData.Count; i++)
            {
                float kernelInput = getKernelInput(i, inputType, alpha, inputVectorTarget);
                float[] currentVectorOutput = getCurrentVectorOutput(i, inputType);
                //Console.WriteLine("current vector output is  :");
                //for(int n = 0; n< currentVectorOutput.Length; n++)
                //{
                //    Console.Write(currentVectorOutput[n] + "   ");
                //}
                //Console.WriteLine();

                // work out the values for the numerator and denominator to be added to sum
                //Console.WriteLine("output of kernel function is " + kernelFunction(kernelInput));
                for (int k = 0; k < currentVectorOutput.Length; k++)
                {
                    sumNumerator[k] += currentVectorOutput[k] * kernelFunction(kernelInput);
                }
                sumDenominator += kernelFunction(kernelInput);
            }
            
            for(int k = 0; k < numOutputDimensions; k++)
            {
                outputVector[k] = sumNumerator[k] / sumDenominator;
            }

            return outputVector;

        }


        private float kernelFunction(float kernelInput)
        {
            float exponenet = -0.5f * (float)Math.Pow(kernelInput, 2);
            float output = inverseSqrt2Pi * (float)Math.Exp(exponenet);

            return output;
        }


        private float[] getCurrentVectorOutput(int i, RegressionInput inputType)
        {
            DataPoint currentDataPoint = activeCalibrationData[i];
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
                //currentVector = new float[4];
                //currentVector[0] = currentDataPoint.motorAngles[0];
                //currentVector[1] = currentDataPoint.motorAngles[1];
                //currentVector[2] = currentDataPoint.motorAngles[2];
                //currentVector[3] = currentDataPoint.motorAngles[3];
                currentVector = currentDataPoint.motorAngles;
            }

            return currentVector;
        }

        
        private float getKernelInput(int i, RegressionInput inputType, float alpha, float[] inputVectorTarget)
        {
            DataPoint currentDataPoint = activeCalibrationData[i];
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
            difference = (float)Math.Sqrt(difference);

            kernelInput = difference / alpha;

            //Console.WriteLine("the kernel parameters are:");
            //for(int m = 0; m < inputVectorTarget.Length; m++)
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


        public void calibrate()
        {
            int startingServoIndex = 0;
            if(controller.isConnected())    controller.shareMotorAngles(motorAngles);
            calibrating = true;
            calibrate(startingServoIndex, true);
            Console.WriteLine("calibration finished");

            if(!dummyExperiment)    saveDataXML();
        }

        public void makeDummy()
        {
            dummyExperiment = true;
        }

        public bool getCalibrationData()
        {
            XmlSerializer reader = new XmlSerializer(typeof(CalibrationData));
            StreamReader file;
            try
            {
                file = new StreamReader(filename);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Could not open " + filename + " message: " + ex.Message);
                return false;
            }
            activeCalibrationData = (CalibrationData)reader.Deserialize(file);
            file.Close();
            for (int i = 0; i < activeCalibrationData.Count; i++)
            {
                //Console.WriteLine("for data point number " + i);
                //Console.WriteLine("motor angles:");
                //for (int j = 0; j < activeCalibrationData[i].motorAngles.Length; j++)
                //{
                //    Console.Write(activeCalibrationData[i].motorAngles[j] + "   ");
                //}
                //Console.WriteLine("tip position:");
                //for (int j = 0; j < activeCalibrationData[i].relativeTipPosition.Length; j++)
                //{
                //    Console.WriteLine(activeCalibrationData[i].relativeTipPosition[j] + "   ");
                //}
                //Console.WriteLine();
            }

            if(activeCalibrationData.Count <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        private void getCurrentData()
        {
            lock(syncLock)
            {
                // loop through RigidBody data
                for (int i = 0; i < currentFrame.nRigidBodies; i++)
                {
                    NatNetML.RigidBodyData rb = currentFrame.RigidBodies[i];
                    // get the hashcode of the id for later displaying in grid form
                    int keyID = rb.ID.GetHashCode();
                    if (rigidBodiesIDtoName.ContainsKey(keyID))
                    {
                        string name = (string)rigidBodiesIDtoName[keyID];
                        htRigidBodiesNameToBody[name] = rb;
                        if (name.Equals("robotBase")) robotBase = rb;
                        else if (name.Equals("robotTip")) robotTip = rb;

                    }
                }
                getRelativeTipInfo();

            }
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


        private void calibrate(int motorIndex, bool rising) {
            int localMaxAngle = maxAngle;
            // move greater angles if it is one of the DOF at the tip???
            //if(motorIndex > 1)
            //{
            //    localMaxAngle += 20;
            //}
            int activeRange = (localMaxAngle - 90) * 2;
            numCalibrationSteps = numIterationPoints - 1;
            int step = activeRange / numCalibrationSteps;
            //int shiftAngle = 90 - (step * (numCalibrationSteps / 2));
            int angle;
            int startingAngle = 90 - (maxAngle - 90);
            int cnt = 1; // could make this start at 2 if want angles to begin opposed (may make more smooth to start)

            if (!rising)
            {
                startingAngle += numCalibrationSteps * step;
            }

            for (int i = 0; i < numIterationPoints; i++)
            {
                cnt++;
                //int angle = 90 + (i * step);
                //angle = angle % maxAngle;
                //if (angle < 90) angle += shiftAngle;
                if (rising) angle = startingAngle + (i * step);
                else angle = startingAngle - (i * step);

                motorAngles[motorIndex] = angle;

                if (!calibrating)
                {
                    if(!dummyExperiment)    saveDataXML();
                    return;
                }
                if(motorIndex < 3)
                {
                    bool newRising;
                    if (cnt % 2 == 0) newRising = true;
                    else newRising = false;
                    calibrate(motorIndex + 1, newRising);
                }
                else if (motorIndex == 3)
                {
                    Console.WriteLine("motor angles are {0} {1} {2} {3}", motorAngles[0], motorAngles[1], motorAngles[2], motorAngles[3]);
                    // move the motors to new positions
                    // sleep for about a second
                    if (controller.isConnected())
                    {
                        //Console.WriteLine("setting motor angles");
                        controller.setMotorAngles();
                    }
                    Thread.Sleep(1000);
                    // log the new data to the list, it will be already updated every time update is called from another thread
                    if (!dummyExperiment) logCalibrationData();

                    checkForPause();
                    
                }
            }
            
        }


        // This method checks to see if the calibration should be paused, whether from user or if batteries run out
        private void checkForPause()
        {
            // If not moving then should pause
            if( !pausedCalibration && notMoving())
            {
                pausedCalibration = true;
                // trim the last value from the calibration data as it shouldn't count
                testData.RemoveAt(testData.Count - 1);
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
            if (testData.Count < 2) return false;

            DataPoint currentDataPoint = testData[testData.Count - 1];
            DataPoint lastDataPoint = testData[testData.Count - 2];

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

            DataPoint newDataPoint = new DataPoint();
            // lock so it doesn't change as adding it to the list of data points
            lock(syncLock)
            {
                newDataPoint.setMotorAngles(motorAngles);
                newDataPoint.setTipOrientation(relativeTipAngles);
                newDataPoint.setTipPos(relativeTipPos);

                testData.Add(newDataPoint);
            }
        }

        private void saveDataXML()
        {
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, testData);
            Console.WriteLine("should have written data.xml");

            writer.Close();
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
