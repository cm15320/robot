using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
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
        private RigidBodyData bodyToFollow;
        private CalibrationData newCalibrationData = new CalibrationData();
        private CalibrationData storedCalibrationData, storedTestData;
        private BodePlot bodePlot;
        //private int calibrationStep = 0;
        private int[] motorAngles = new int[] { 90, 90, 90, 90 };
        private int numIterationPoints = 5;
        private int numTestPoints = 3;
        //private int numPoints;
        private int maxAngle = 120;
        private float[] basePosition = new float[3];
        private float[] relativeTipAngles = new float[] { 0.0f, 0.0f, 0.0f };
        private float[] relativeTipPos = new float[] { 0.0f, 0.0f, 0.0f };
        private float[] relativeBodyFollowPos = new float[] { 0.0f, 0.0f, 0.0f };
        private FrameOfMocapData currentFrame;
        private object syncLock;
        private bool calibrating = false;
        private bool pausedCalibration = false; // this can indicate the pause of the calibration and the test stage
        private const float startingAlpha = 0.0008f;
        private StringBuilder csv = new StringBuilder();
        private StringBuilder bodeCsv = new StringBuilder();
        private int motorScaler = 100;

        private int newTargetDelay = 50;

        private Hashtable htRigidBodiesNameToBody = new Hashtable();
        private double distanceBetween;

        private XmlSerializer ser = new XmlSerializer(typeof(CalibrationData));
        private XmlSerializer bodeSer = new XmlSerializer(typeof(BodePlot));
        private string calibrationFilename = "calibrationData.xml";
        private string testDataFilename = "testPoints.xml";
        private string bodeFileName = "bodePlot.xml";
        private bool currentlyTracked;
        private bool experimentLive = false;
        private float[] eulersRobotBase;
        private float[] maxRelativePositionValues;
        private float[] minRelativePositionValues;
        private float tipToBaseSphereRadius;
        private float shiftFactor = 0.001f;

        private UserStudy activeStudy;
        private bool runningStudy = false;

        private NatNetClientML m_NatNet;



        //private string[] bodyNames = new string[] { "robotBase", "robotTip" };
        // need to store the name of the rigid bodies mapped against their ID
        private Hashtable rigidBodiesIDtoName = new Hashtable();
        // private List<RigidBody> blocks;

        public Experiment(bool realExperiment)
        {
            if (realExperiment == false)
            {
                dummyExperiment = true;
                controller = new RobotControl(syncLock);
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
            htRigidBodiesNameToBody.Add("bodyToFollow", new RigidBodyData());

            //string[] requiredNames = new string[] { "robotBase", "robotTip" };
            if (mRigidBodies.Count == 0)
            {
                Console.WriteLine("Error, no rigid body info stored");
            }
            foreach (RigidBody rb in mRigidBodies)
            {
                // if bodies to id ht contatins key (rb. name) then add it to ID to Name ht
                if (htRigidBodiesNameToBody.ContainsKey(rb.Name))
                {
                    int key = rb.ID.GetHashCode();
                    rigidBodiesIDtoName.Add(key, rb.Name);
                }
            }
            if (rigidBodiesIDtoName.Count == 0)
            {
                Console.WriteLine("Error, not added necessary rigid bodies to the hash table");
            }

            initialiseMotors();
        }



        public bool bodyFollowRequirements()
        {
            // check if there is actually a 
            if ( rigidBodiesIDtoName.ContainsValue("bodyToFollow"))
            {
                return true;
            }
            return false;
        }


        private float[] convertTargetOrientation(float[] targetPos, float[] baseRads)
        {

            float[][] rotationMatrix = buildInverseRotationMatrix(baseRads);
            //float[][] rotationMatrix = buildRotationMatrix(baseRads);
            float[] convertedPoint = multiplyByRotationMatrix(rotationMatrix, targetPos);

            roundToMil(convertedPoint);
            return convertedPoint;
        }

        private float[] convertTargetOrientation(float[] targetPos)
        {
            float[] baseRads = eulersRobotBase;

            //Console.WriteLine("relative target point was:");
            //for (int i = 0; i < 3; i++)
            //{
            //    Console.WriteLine(targetPos[i]);
            //}
            //Console.WriteLine();
            //float[] baseRads = eulersRobotBase;
            //Console.WriteLine("base radians were");
            //for (int i = 0; i < 3; i++)
            //{
            //    Console.WriteLine(baseRads[i]);
            //}

            float[] convertedTargetPoint = convertTargetOrientation(targetPos, baseRads);

            //Console.WriteLine("converted target point is:");
            //for (int i = 0; i < 3; i++)
            //{
            //    Console.WriteLine(convertedTargetPoint[i]);
            //}

            return convertedTargetPoint;

        }

        private void roundToMil(float[] point)
        {
            for (int i = 0; i < point.Length; i++)
            {
                point[i] = (float)Math.Round(point[i], 3);
            }
        }

        private void featureScaling()
        {
            int numMotors = 4;
            float[] maxAngles = new float[] { 0, 0, 0, 0 };
            float[] minAngles = new float[] { 180, 180, 180, 180 };
            float maxDifference = 0;

            for (int i = 0; i < numMotors; i++)
            {
                foreach (DataPoint currentDataPoint in storedCalibrationData)
                {
                    if (currentDataPoint.motorAngles[i] > maxAngles[i])
                    {
                        maxAngles[i] = currentDataPoint.motorAngles[i];
                    }
                    if (currentDataPoint.motorAngles[i] < minAngles[i])
                    {
                        minAngles[i] = currentDataPoint.motorAngles[i];
                    }
                }
            }

            for (int i = 0; i < maxAngles.Length; i++)
            {
                float difference = maxAngles[i] - minAngles[i];
                if (difference > maxDifference)
                {
                    maxDifference = difference;
                }
            }

            motorScaler = (int)Math.Round(maxDifference);
        }

        private float[][] buildInverseRotationMatrix(float[] baseRotation)
        {
            // this goes in reverse order (z rot then y rot then x rot) to move the 
            // target to a new position relative to the base to account for the rotation of the
            // base, giving a suitable point for the robot to move to

            float alpha = -1 * baseRotation[0];
            float beta = -1 * baseRotation[1];
            float gamma = -1 * baseRotation[2];

            float[] firstLineRotMatrix = new float[]
            {
                (float)(Math.Cos(beta) * Math.Cos(gamma)),
                (float)(-1 * Math.Cos(beta) * Math.Sin(gamma)),
                (float)(Math.Sin(beta))
            };

            float[] secondLineRotMatrix = new float[]
            {
                (float)((Math.Cos(alpha) * Math.Sin(gamma)) + (Math.Sin(alpha) * Math.Sin(beta) * Math.Cos(gamma))),
                (float)((Math.Cos(alpha) * Math.Cos(gamma)) - (Math.Sin(alpha) * Math.Sin(beta) * Math.Sin(gamma))),
                (float)(-1 * Math.Sin(alpha) * Math.Cos(beta))
            };

            float[] thirdLineRotMatrix = new float[]
            {
                (float)((Math.Sin(alpha) * Math.Sin(gamma)) - (Math.Cos(alpha) * Math.Sin(beta) * Math.Cos(gamma))),
                (float)((Math.Sin(alpha) * Math.Cos(gamma)) + (Math.Cos(alpha) * Math.Sin(beta) * Math.Sin(gamma))),
                (float)(Math.Cos(alpha) * Math.Cos(beta))
            };

            float[][] rotationMatrix = new float[][] { firstLineRotMatrix, secondLineRotMatrix, thirdLineRotMatrix };

            return rotationMatrix;
        }



        private float[][] buildRotationMatrix(float[] baseRotation)
        {
            float alpha = baseRotation[0];
            float beta = baseRotation[1];
            float gamma = baseRotation[2];

            float[] firstLineRotMatrix = new float[]
            {
                (float)(Math.Cos(beta) * Math.Cos(gamma)),
                (float)((Math.Cos(gamma) * Math.Sin(alpha) * Math.Sin(beta)) - (Math.Cos(alpha) * Math.Sin(gamma))),
                (float)((Math.Cos(alpha) * Math.Cos(gamma) * Math.Sin(beta)) + (Math.Sin(alpha) * Math.Sin(gamma)))
            };

            float[] secondLineRotMatrix = new float[]
            {
                (float)(Math.Cos(beta) * Math.Sin(gamma)),
                (float)((Math.Cos(alpha) * Math.Cos(gamma)) + (Math.Sin(alpha) * Math.Sin(beta) * Math.Sin(gamma))),
                (float)((-1 * Math.Cos(gamma) * Math.Sin(alpha)) + (Math.Cos(alpha) * Math.Sin(beta) * Math.Sin(gamma)))
            };

            float[] thirdLineRotMatrix = new float[]
            {
                (float)(-1 * Math.Sin(beta)),
                (float)(Math.Cos(beta) * Math.Sin(alpha)),
                (float)(Math.Cos(alpha) * Math.Cos(beta))
            };

            float[][] rotationMatrix = new float[][] { firstLineRotMatrix, secondLineRotMatrix, thirdLineRotMatrix };

            return rotationMatrix;
        }


        public void testMultiplyMatrix()
        {
            float[] vector = new float[] { 2, 1, 3 };
            float[][] matrix = new float[][] { new float[] { 1, 2, 3 }, new float[] { 4, 5, 6 }, new float[] { 7, 8, 9 } };

            float[] solution = multiplyByRotationMatrix(matrix, vector);


            Console.WriteLine("solution is:");
            for (int i = 0; i < solution.Length; i++)
            {
                Console.WriteLine(solution[i]);
            }
        }


        public void testRotation()
        {
            float[] targetPos = new float[] { -30, 1, 5 };
            RotationFromStartPoint baseRotation = new RotationFromStartPoint(90, -15, 0);
            float[] baseRads = baseRotation.getRads();


            float[] solution = convertTargetOrientation(targetPos, baseRads);

            Console.WriteLine("solution is:");
            for (int i = 0; i < solution.Length; i++)
            {
                Console.WriteLine(solution[i]);
            }
        }


        private float[] multiplyByRotationMatrix(float[][] rotationMatrix, float[] targetPos)
        {
            float[] solution = new float[targetPos.Length];

            for (int i = 0; i < rotationMatrix.Length; i++)
            {
                for (int j = 0; j < rotationMatrix[0].Length; j++)
                {
                    solution[i] = solution[i] + (rotationMatrix[i][j] * targetPos[j]);
                }
            }

            return solution;
        }


        // perhaps put a lock on this in case the currentFrame object is changed mid-read???????
        public void update(NatNetML.FrameOfMocapData newCurrentFrame)
        {
            // get the correct rigid body data objects from the frame
            this.currentFrame = newCurrentFrame;
            getCurrentTrackingData();

            calculateDistanceBetween();
        }

        private void initialiseMotors()
        {
            if (controller.isConnected())
            {
                controller.zeroMotors();
                controller.shareMotorAngles(motorAngles);
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


        private double[] getSolutionFromSphereIntersection(float[] inputVectorTarget, float bandwidth = startingAlpha)
        {
            double[] output;
            if (outOfRecordedRange(inputVectorTarget))
            {
                //Console.WriteLine("out of recorded range, using closest point to intersection");
                output = getClosestCalibrationSolution(inputVectorTarget);
                return output;
            }
            else
            {
                //Console.WriteLine("in range, attempting direct solution");
                output = NWRegression(inputVectorTarget, RegressionInput.POSITION, bandwidth);
                if (!motorAnglesNaN(output))
                {
                    //Console.WriteLine("solution found directly");
                    return output;
                }
                else
                {
                    //Console.WriteLine("solution not found, finding closest target point");
                    output = getClosestCalibrationSolution(inputVectorTarget);
                    return output;
                }
            }

        }



        public double[] testRegression(float[] inputVectorTarget, RegressionInput inputType, float bandwidth = startingAlpha)
        {
            // assume that the base is in its zero orientation, so just convert the point to the sphere 
            // then deal with it from there, don't perform any orientation conversions

            double[] output;

            if (inputType == RegressionInput.POSITION)
            {
                sphereIntersectionConvert(inputVectorTarget);
                output = getSolutionFromSphereIntersection(inputVectorTarget, bandwidth);
            }
            else
            {
                output = NWRegression(inputVectorTarget, inputType, bandwidth);
            }

            //if(controller.isConnected())
            //{
            //    controller.setMotorAnglesTest(output);
            //}

            return output;
        }

        // could also actually subtract the value of the min from the value coming in before dividing by the scale value, for true min-max normalization
        private void convertMotors(float[] origMotorAngles, bool reduce)
        {
            for (int i = 0; i < origMotorAngles.Length; i++)
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
            // put a lock as the array copying from may be changed by another process
            lock(syncLock)
            {
                inputVectorTarget.CopyTo(copyVector, 0);
                //for (int i = 0; i < copyVector.Length; i++)
                //{
                //    copyVector[i] = inputVectorTarget[i];
                //}
            }

            return copyVector;
        }


        // This is for points that have already been identified as out of possible range,
        // therefore just simply finds the closest point in the calibration data to the 
        // target point, and uses the corresponding motor angles
        private double[] getClosestCalibrationSolution(float[] inputVectorTarget)
        {

            DataPoint currentDataPoint;
            int indexOfClosestPoint = 0;
            float minAbsoluteDifference = 100000f;
            float[] currentDifference = new float[3];
            for (int i = 0; i < storedCalibrationData.Count; i++)
            {
                currentDataPoint = storedCalibrationData[i];
                for (int j = 0; j < currentDifference.Length; j++)
                {
                    currentDifference[j] = inputVectorTarget[j] - currentDataPoint.relativeTipPosition[j];
                }
                float absoluteDifference = getAbsoluteValue(currentDifference);
                if (absoluteDifference < minAbsoluteDifference)
                {
                    indexOfClosestPoint = i;
                    minAbsoluteDifference = absoluteDifference;
                }
            }
            //Console.WriteLine("index of closest point is " + indexOfClosestPoint);
            //Console.WriteLine("at which the values are ");
            for (int j = 0; j < inputVectorTarget.Length; j++)
            {
                //Console.WriteLine(storedCalibrationData[indexOfClosestPoint].relativeTipPosition[j]);
                inputVectorTarget[j] = storedCalibrationData[indexOfClosestPoint].relativeTipPosition[j];
            }

            //Console.WriteLine("corresponding solution of:");
            //for (int j = 0; j < 4; j++)
            //{
            //    Console.WriteLine(storedCalibrationData[indexOfClosestPoint].motorAngles[j]);
            //}


            //inputVectorTarget = getCopyVector(storedCalibrationData[indexOfClosestPoint].relativeTipPosition);

            float[] correspondingMotors = storedCalibrationData[indexOfClosestPoint].motorAngles;
            double[] solution = floatToDouble(correspondingMotors);

            return solution;

        }

        private double[] floatToDouble(float[] input)
        {
            double[] output = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (double)input[i];
            }
            return output;
        }

        private double[] NWRegression(float[] inputVectorTarget, RegressionInput inputType, float alpha = startingAlpha)
        {
            int numOutputDimensions = getNumOutputDimensions(inputType);
            double[] outputVector = new double[numOutputDimensions];
            double[] sumNumerator = new double[numOutputDimensions];
            double sumDenominator = 0;

            //// could be in its own function as it's not really part of the 'regression'
            //if(inputType == RegressionInput.POSITION)
            //{
            //    sphereIntersectionConvert(inputVectorTarget);
            //    getClosestCalibrationSolution(inputVectorTarget);
            //}

            float[] newInputVector = getCopyVector(inputVectorTarget);
            //Console.WriteLine("vector to have regression performed is:");
            //for (int i = 0; i < newInputVector.Length; i++)
            //{
            //    Console.WriteLine(newInputVector[i]);
            //}


            if (inputType == RegressionInput.MOTORS)
            {
                convertMotors(newInputVector, true);
            }

            // If not using KD tree, loop through entire set of data
            for (int i = 0; i < storedCalibrationData.Count; i++)
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

            for (int k = 0; k < numOutputDimensions; k++)
            {
                outputVector[k] = sumNumerator[k] / sumDenominator;
            }

            if (inputType != RegressionInput.MOTORS)
            {
                convertOutputMotors(outputVector, false);
            }

            //Console.WriteLine("Vector output being returned is:");
            //for (int i = 0; i < outputVector.Length; i++)
            //{
            //    Console.WriteLine(outputVector[i]);
            //}

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


        private void sphereIntersectionConvert(float[] relativeTargetPoint)
        {
            float absoluteTargetDistance = getAbsoluteValue(relativeTargetPoint);
            if (absoluteTargetDistance > tipToBaseSphereRadius)
            {
                float vectorFactor = tipToBaseSphereRadius / absoluteTargetDistance;
                //Console.WriteLine("absolute distance is: " + absoluteTargetDistance);
                //Console.WriteLine("sphere radius is :" + tipToBaseSphereRadius);
                //Console.WriteLine("so vector factor is: " + vectorFactor);
                //Console.WriteLine("converted target point to sphere intersection from:  x = {0}, y = {1}, z = {2}", relativeTargetPoint[0], relativeTargetPoint[1], relativeTargetPoint[2]);
                for (int i = 0; i < relativeTargetPoint.Length; i++)
                {
                    relativeTargetPoint[i] *= vectorFactor;
                }
                //Console.WriteLine("To:  x = {0}, y = {1}, z = {2}", relativeTargetPoint[0], relativeTargetPoint[1], relativeTargetPoint[2]);
            }
            // also put in code for if it is less than the minimum radius experienced 
            // so that it can shift on to this minimum sphere before finding closest position
        }


        private float[] getcurrentYValue(int i, RegressionInput inputType)
        {
            DataPoint currentDataPoint = storedCalibrationData[i];
            float[] currentVector;
            if (inputType == RegressionInput.MOTORS)
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
            if (testPoints) startingAngle -= 5; // offset it by 5 degrees to the left to cover angles not covered by calibration alone


            // if should be descending then just start at the extreme and come down
            if (!rising) startingAngle += numSteps * step;

            for (int i = 0; i < numPoints; i++)
            {
                if (rising) angle = startingAngle + (i * step); // rise up
                else angle = startingAngle - (i * step); // come down

                cnt++;
                if (cnt % 2 == 0) newRising = true;
                else newRising = false;
                motorAngles[motorIndex] = angle;

                if (!calibrating)
                {
                    saveDataXML(filename);
                    return;
                }
                if (motorIndex < 3) calibrate(motorIndex + 1, newRising, testPoints);
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
            if (controller.isConnected())
            {
                eliminateExtremeAngles();
                lock(syncLock)
                {
                    controller.setMotorAngles();
                }
            }
        }

        private void eliminateExtremeAngles()
        {
            for (int i = 0; i < motorAngles.Length; i++)
            {
                if (motorAngles[i] > maxAngle)
                {
                    motorAngles[i] = maxAngle;
                    //Console.WriteLine("Had to remove  exreme high angle");
                }
                if (motorAngles[i] < 180 - maxAngle)
                {
                    motorAngles[i] = 180 - maxAngle;
                    //Console.WriteLine("Had to remove extreme low angle");
                }
            }
        }


        public void startLiveBodyFollowing()
        {
            experimentLive = true;
            // start the thread to constantly set the motor angles
            new Task(liveExperimentThreadLoop).Start();
            float[] relativeTargetPoint;
            while (experimentLive)
            {
                // in future would have to pass in a literal point then get the relative point 
                // based on the position of the base as well
                relativeTargetPoint = relativeBodyFollowPos;
                getMotorAnglesForTargetPoint(relativeTargetPoint);

                Thread.Sleep(newTargetDelay);
            }


        }

        public void startLivePointFollowing(float[] relativeTargetPoint)
        {
            experimentLive = true;
            new Task(liveExperimentThreadLoop).Start();
            while (experimentLive)
            {
                // in future would have to pass in a literal point then get the relative point 
                // based on the position of the base as well
                getMotorAnglesForTargetPoint(relativeTargetPoint);

                Thread.Sleep(newTargetDelay);
            }
        }

        private void getMotorAnglesForTargetPoint(float[] relativeTargetPoint)
        {
            double[] motorAngleSolution = getRegressionMotorSolution(relativeTargetPoint);
            //double[] motorAngleSolution = NWRegression(convertedTargetPoint, RegressionInput.POSITION);
            //Console.WriteLine("motor solution is");
            //for (int i = 0; i < 4; i++)
            //{
            //    Console.WriteLine(motorAngleSolution[i]);
            //}

            updateNewMotorAngles(motorAngleSolution);
            //Console.WriteLine("updated new motor angles");

        }


        private bool outOfRecordedRange(float[] vectorPosition)
        {
            for (int i = 0; i < vectorPosition.Length; i++)
            {
                if (vectorPosition[i] > maxRelativePositionValues[i])
                {
                    return true;
                }
                if (vectorPosition[i] < minRelativePositionValues[i])
                {
                    return true;
                }
            }

            return false;
        }

        // Gets the regression solution for an input position returning the motor angles
        private double[] getRegressionMotorSolution(float[] targetPoint, float alpha = startingAlpha)
        {
            double[] motorAngleSolution;
            // First convert the target orientation, not this gets a copy vector and doesn't change
            // the original so don't have to get a copy before passing in
            float[] convertedTargetPoint = convertTargetOrientation(targetPoint);

            // then get the sphere intersection
            sphereIntersectionConvert(convertedTargetPoint);

            // then eliminate extreme positions to get a valid point that can be reached
            motorAngleSolution = getSolutionFromSphereIntersection(convertedTargetPoint);

            return motorAngleSolution;
        }


        public void stopAllLive()
        {
            experimentLive = false;
            runningStudy = false;
        }

        public void moveToRelTargetPoint(float[] relTargetPoint)
        {
            if (controller.isConnected()) controller.shareMotorAngles(motorAngles);
            getMotorAnglesForTargetPoint(relTargetPoint);
            setMotorAngles();
        }

        
        // Tests the movement towards a tracked rigid body
        public void moveToTargetBody()
        {
            float[] relTargetPoint = getCopyVector(relativeBodyFollowPos);
            getMotorAnglesForTargetPoint(relTargetPoint);
            setMotorAngles();
        }
        

        private void updateNewMotorAngles(double[] newMotorAngles)
        {
            lock (syncLock)
            {
                // don't update any angles if they are NaN
                if (motorAnglesNaN(newMotorAngles))
                {
                    Console.WriteLine("Not updated due to NaN values");
                    return;
                }

                for (int i = 0; i < motorAngles.Length; i++)
                {
                    motorAngles[i] = (int)Math.Round(newMotorAngles[i]);
                }
            }
        }


        public void startStudy(UserStudyType type)
        {
            activeStudy = new UserStudy(type);
            experimentLive = true;
            new Task(liveExperimentThreadLoop).Start();

            runUserStudy();

            experimentLive = false;
            controller.zeroMotors();
            Console.WriteLine("finished study");

        }


        public void testReadInTargetPositions()
        {
            activeStudy = new UserStudy(UserStudyType.GESTURING);

            activeStudy.testTargetPositions();
        }


        private void runUserStudy()
        {
            bool triggerPress;
            float[] relativeTargetPosition; 
            if (!activeStudy.isInitialised())
            {
                // Check that it has been initialise before attempting to run
                Console.WriteLine("unable to initialise user study");
                return;
            }

            relativeTargetPosition = activeStudy.getRelativeTargetPosition();

            runningStudy = true;
            //activeStudy.update(basePosition, false); // update once to know the relative position
            while(runningStudy)
            {
                triggerPress = getTrigger();
                runningStudy = activeStudy.update(basePosition, triggerPress);

                if(triggerPress)
                {
                    zeroMotorAngles();
                }
                else if (runningStudy)
                {
                    getMotorAnglesForTargetPoint(relativeTargetPosition);
                }
                Thread.Sleep(40);
            }
        }


        private bool getTrigger()
        {
            bool trigger;
            lock(syncLock)
            {
               trigger = controller.getTrigger();
            }
            return trigger;
        }


        private void zeroMotorAngles()
        {
            for(int i = 0; i < motorAngles.Length; i++)
            {
                motorAngles[i] = 90;
            }
        }


        private bool motorAnglesNaN(double[] newMotorAngles)
        {
            for (int i = 0; i < newMotorAngles.Length; i++)
            {
                if (Double.IsNaN(newMotorAngles[i]))
                {
                    return true;
                }
            }
            return false;
        }


        private void liveExperimentThreadLoop()
        {
            while (experimentLive)
            {
                setMotorAngles();
                Thread.Sleep(20);
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

        private bool readInBodeData()
        {
            XmlSerializer reader = new XmlSerializer(typeof(BodePlot));
            StreamReader file;
            try
            {
                file = new StreamReader(bodeFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not open " + bodeFileName + " message: " + ex.Message);
                return false;
            }
            BodePlot dataReadIn = (BodePlot)reader.Deserialize(file);
            bodePlot = dataReadIn;

            file.Close();
            if(bodePlot.Count > 0)
            {
                return true;
            }
            else return false;

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

        private void getMaxAndMinValues()
        {
            maxRelativePositionValues = new float[] { -10, -10, -10 };
            minRelativePositionValues = new float[] { 10, 10, 10 };

            foreach (DataPoint currentDataPoint in storedCalibrationData)
            {
                float relX = currentDataPoint.relativeTipPosition[0];
                float rely = currentDataPoint.relativeTipPosition[1];
                float relz = currentDataPoint.relativeTipPosition[2];

                float[] relPosition = currentDataPoint.relativeTipPosition;

                for (int i = 0; i < relPosition.Length; i++)
                {
                    if (relPosition[i] > maxRelativePositionValues[i])
                    {
                        maxRelativePositionValues[i] = relPosition[i];
                    }
                    else if (relPosition[i] < minRelativePositionValues[i])
                    {
                        minRelativePositionValues[i] = relPosition[i];
                    }
                }

            }
            Console.WriteLine("max relative Position values are: x = {0}, y = {1}, z = {2}", maxRelativePositionValues[0], maxRelativePositionValues[1], maxRelativePositionValues[2]);
            Console.WriteLine("min relative Position values are: x = {0}, y = {1}, z = {2}", minRelativePositionValues[0], minRelativePositionValues[1], minRelativePositionValues[2]);
        }


        private bool getTipToBaseSphereRadius()
        {
            float[] zeroPosition;
            foreach (DataPoint currentDataPoint in storedCalibrationData)
            {
                int cnt = 0;
                for (int i = 0; i < 4; i++)
                {
                    if ((int)Math.Round(currentDataPoint.motorAngles[i]) == 90)
                    {
                        cnt++;
                    }
                }
                if (cnt == 4)
                {
                    tipToBaseSphereRadius = getAbsoluteValue(currentDataPoint.relativeTipPosition);
                    Console.WriteLine("sphere radius is " + tipToBaseSphereRadius);
                    return true;
                }
            }
            Console.WriteLine("ERROR, NO ZERO POSITION RECORDED TO GIVE SPHERE RADIUS");
            return false;
        }


        private float getAbsoluteValue(float[] relativePosition)
        {
            float absoluteValue, difference = 0f;
            for (int i = 0; i < relativePosition.Length; i++)
            {
                difference += (float)Math.Pow(relativePosition[i], 2);
            }
            absoluteValue = (float)Math.Sqrt(difference);
            return absoluteValue;
        }

        public bool getBodeData()
        {
            bool success = readInBodeData();
            if (!success)
            {
                Console.WriteLine("not able to read bode data");
            }
                return success;
        }


        public void testTrigger()
        {
            bool triggerPress;

            runningStudy = true;
            while (runningStudy)
            {
                triggerPress = controller.getTrigger();

                if (triggerPress)
                {
                    Console.WriteLine("trigger is pressed down");
                }
                Thread.Sleep(40);
            }
        }

        public bool getCalibrationData()
        {
            bool success = getData(calibrationFilename);
            if (success)
            {
                featureScaling();
                Console.WriteLine("feature scaling result is " + motorScaler);
                getMaxAndMinValues();
                getTipToBaseSphereRadius();
                //testMultiplyMatrix();
                //testRotation();
                //testBode();
            }
            return success;


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


        public void startBodePlot()
        {
            experimentLive = true;
            Stopwatch sw = new Stopwatch();
            bodePlot = new BodePlot();
                        
            // start the thread to constantly set the motor angles
            new Task(liveExperimentThreadLoop).Start();
            float[] relativeTargetPoint;

            // START TIMER
            sw.Start();

            while (experimentLive)
            {
                relativeTargetPoint = relativeBodyFollowPos;
                getMotorAnglesForTargetPoint(relativeTargetPoint);

                float timestamp = sw.ElapsedMilliseconds;
                logBodeData(timestamp);

                Thread.Sleep(newTargetDelay);
            }

            sw.Stop();

            saveBodeXML(bodeFileName);

            // STOP TIMER

        }

        private void logBodeData(float timestamp)
        {
            BodeDataPoint newBodePoint = new BodeDataPoint();
            if (!currentlyTracked)
            {
                Console.WriteLine("not logged a data point due to untracked rigid body");
                return;
            }

            lock (syncLock)
            {
                newBodePoint.setTimeStamp(timestamp);
                newBodePoint.setTargetPos(relativeBodyFollowPos);
                newBodePoint.setTipPos(relativeTipPos);
            }
            // Add the bode point to the existing bode plot data
            bodePlot.Add(newBodePoint);
            //Console.WriteLine("added a new body point at a time at " + timestamp);
        }

        private void saveBodeXML(string filename)
        {
            //if (dummyExperiment)
            //{
            //    return;
            //}

            TextWriter writer = new StreamWriter(filename);
            bodeSer.Serialize(writer, bodePlot);
            writer.Close();
            Console.WriteLine("should have written to " + filename);
        }


        private void testBode()
        {
            bodePlot = new BodePlot();
            BodeDataPoint testPoint = new BodeDataPoint();
            testPoint.setTimeStamp(2f);
            testPoint.setTipPos(new float[] { 4f, 5f, 6f });
            testPoint.setTargetPos(new float[] { 1f, 2f, 3f });
            bodePlot.Add(testPoint);
            bodePlot.Add(testPoint);

            saveBodeXML(bodeFileName);
        }



        private void getCurrentTrackingData()
        {
            lock (syncLock)
            {
                int trackedBodies = 0;
                // loop through RigidBody data
                for (int i = 0; i < currentFrame.nRigidBodies; i++)
                {
                    NatNetML.RigidBodyData rb = currentFrame.RigidBodies[i];
                    if (rb.Tracked)
                    {
                        trackedBodies++;
                    }
                
                    // get the hashcode of the id for later displaying in grid form
                    int keyID = rb.ID.GetHashCode();
                    if (rigidBodiesIDtoName.ContainsKey(keyID))
                    {
                        string name = (string)rigidBodiesIDtoName[keyID];
                        htRigidBodiesNameToBody[name] = rb;
                        // THIS MAY NOT BE NEEDED NOW AS JUST UPDATING THE HT WITH THE STRING EQUAL TO THE NAME
                        if (name.Equals("robotBase")) robotBase = rb;
                        else if (name.Equals("robotTip")) robotTip = rb;
                        else if (name.Equals("bodyToFollow")) bodyToFollow = rb;

                    }
                }
                if(trackedBodies != currentFrame.nRigidBodies)
                {
                    currentlyTracked = false;
                }
                else currentlyTracked = true;
               
                getRelativeTipInfo();

            }
        }

        public void generateBodePlot()
        {
            string firstLine = String.Format("{0},{1},{2}", "Time (ms)", "y target (mm)", "y tip (mm)");
            bodeCsv.AppendLine(firstLine);
            //Console.WriteLine("appeneded first line to bode plot");
            foreach(BodeDataPoint currentPoint in bodePlot)
            {
                float timestamp = currentPoint.timeStamp;
                float yPosTarget = currentPoint.relativeTargetPosition[1];
                float yPosTip = currentPoint.relativeTipPosition[1];
                string newLine = String.Format("{0},{1},{2}", timestamp, yPosTarget , yPosTip);
                bodeCsv.AppendLine(newLine);
                //Console.WriteLine("appened line to bode plot with timestamp of " + timestamp);
            }

            File.WriteAllText("bodePlot.csv", bodeCsv.ToString());

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
            eulersRobotTip = m_NatNet.QuatToEuler(quatRobotTip, (int)NATEulerOrder.NAT_XYZr);
            eulersRobotBase = m_NatNet.QuatToEuler(quatRobotBase, (int)NATEulerOrder.NAT_XYZr);

            float xRDiff = (float)RobotTracker.RadiansToDegrees(eulersRobotTip[0] - eulersRobotBase[0]);     // convert to degrees
            float yRDiff = (float)RobotTracker.RadiansToDegrees(eulersRobotTip[1] - eulersRobotBase[1]);
            float zRDiff = (float)RobotTracker.RadiansToDegrees(eulersRobotTip[2] - eulersRobotBase[2]);

            basePosition[0] = robotBase.x;
            basePosition[1] = robotBase.y;
            basePosition[2] = robotBase.z;


            relativeTipPos[0] = xDiff;
            relativeTipPos[1] = yDiff;
            relativeTipPos[2] = zDiff;

            relativeTipAngles[0] = xRDiff;
            relativeTipAngles[1] = yRDiff;
            relativeTipAngles[2] = zRDiff;

            if (rigidBodiesIDtoName.ContainsValue("bodyToFollow"))
            {
                relativeBodyFollowPos[0] = (float)(bodyToFollow.x - robotBase.x);
                relativeBodyFollowPos[1] = (float)(bodyToFollow.y - robotBase.y);
                relativeBodyFollowPos[2] = (float)(bodyToFollow.z - robotBase.z);
            }
        }




        // This method checks to see if the calibration should be paused, whether from user or if batteries run out
        private void checkForPause()
        {
            // If not moving then should pause
            if (!pausedCalibration && notMoving())
            {
                pausedCalibration = true;
                // trim the last value from the calibration data as it shouldn't count
                newCalibrationData.RemoveAt(newCalibrationData.Count - 1);
                Console.WriteLine("Calibration has been paused due to lack of motion");
            }

            while (pausedCalibration && calibrating)
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

            if (xdiff < 0.0001 && ydiff < 0.0001 && zdiff < 0.0001)
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


        private void logCalibrationData()
        {

            if (dummyExperiment) return;

            DataPoint newDataPoint = new DataPoint();
            // lock so it doesn't change as adding it to the list of data points
            lock (syncLock)
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
            if (dummyExperiment)
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

        public class RotationFromStartPoint
        {
            private float xRotDeg;
            private float yRotDeg;
            private float zRotDeg;

            private float[] rotationRads = new float[3];

            public RotationFromStartPoint(float xRot, float yRot, float zRot)
            {
                this.xRotDeg = xRot;
                this.yRotDeg = yRot;
                this.zRotDeg = zRot;

                convertRads();
            }

            private void convertRads()
            {
                rotationRads[0] = xRotDeg * ((float)Math.PI / 180);
                rotationRads[1] = yRotDeg * ((float)Math.PI / 180);
                rotationRads[2] = zRotDeg * ((float)Math.PI / 180);
            }

            public float[] getRads()
            {
                return rotationRads;
            }

        }
    }
}
