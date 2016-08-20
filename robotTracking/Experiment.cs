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
        private RobotControl controller;
        private RigidBodyData robotBase, robotTip;
        private CalibrationData testData = new CalibrationData();
        private int calibrationStep = 0;
        private int[] motorAngles = new int[] { 90, 90, 90, 90 };
        private int numCalibrationSteps = 6;
        private int maxAngle = 140;
        private float[] relativeTipAngles = new float[] { 0.0f, 0.0f, 0.0f };
        private float[] relativeTipPos = new float[] { 0.0f, 0.0f, 0.0f };
        private FrameOfMocapData currentFrame;
        private object syncLock;
        private bool calibrating = false;
        private CalibrationData activeCalibrationData;

        private Hashtable htRigidBodiesNameToBody = new Hashtable();
        private double distanceBetween;

        private XmlSerializer ser = new XmlSerializer(typeof(CalibrationData));
        private string filename = "calibrationDataNew.xml";

        private NatNetClientML m_NatNet;



        //private string[] bodyNames = new string[] { "robotBase", "robotTip" };
        // need to store the name of the rigid bodies mapped against their ID
        private Hashtable rigidBodiesIDtoName = new Hashtable();
        // private List<RigidBody> blocks;

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
        }

        // perhaps put a lock on this in case the currentFrame object is changed mid-read???????
        public void update(NatNetML.FrameOfMocapData newCurrentFrame)
        {
            // get the correct rigid body data objects from the frame
            this.currentFrame = newCurrentFrame;
            getCurrentData();

            calculateDistanceBetween();
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


        public void calibrate()
        {
            int startingServoIndex = 0;
            controller.shareMotorAngles(motorAngles);
            calibrating = true;
            calibrate(startingServoIndex);
            Console.WriteLine("calibration finished");

            saveDataXML();
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
                Console.WriteLine("Could not open " + filename);
                return false;
            }
            activeCalibrationData = (CalibrationData)reader.Deserialize(file);
            file.Close();
            for (int i = 0; i < activeCalibrationData.Count; i++)
            {
                Console.WriteLine("for data point number " + i);
                Console.WriteLine("motor angles:");
                for (int j = 0; j < activeCalibrationData[i].motorAngles.Length; j++)
                {
                    Console.Write(activeCalibrationData[i].motorAngles[j] + "   ");
                }
                Console.WriteLine();
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


        public void calibrate(int motorIndex) {
            int activeRange = (maxAngle - 90) * 2;
            int step = activeRange / numCalibrationSteps;
            int shiftAngle = 90 - (step * (numCalibrationSteps / 2));
            
            for(int i = 0; i < numCalibrationSteps; i++)
            {
                int angle = 90 + (i * step);
                angle = angle % maxAngle;
                if (angle < 90) angle += shiftAngle;

                motorAngles[motorIndex] = angle;

                if (!calibrating)
                {
                    saveDataXML();
                    return;
                }
                Console.WriteLine("motor angles are {0} {1} {2} {3}", motorAngles[0], motorAngles[1], motorAngles[2], motorAngles[3]);
                if(motorIndex < 3)
                {
                    calibrate(motorIndex + 1);
                }
                else if (motorIndex == 3)
                {
                    // move the motors to new positions
                    // sleep for about a second
                    controller.setMotorAngles();
                    Thread.Sleep(1200);
                    // log the new data to the list, it will be already updated every time update is called from another thread
                    logCalibrationData();
                }
            }
            
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
    }
}
