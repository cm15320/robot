using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private float[] relativeTipAngles = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
        private float[] relativeTipPos = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };


        private Hashtable htRigidBodiesNameToBody = new Hashtable();
        private double distanceBetween;

        private XmlSerializer ser = new XmlSerializer(typeof(CalibrationData));
        private string filename = "calibrationDataNew.xml";
        private TextWriter writer;



        //private string[] bodyNames = new string[] { "robotBase", "robotTip" };
        // need to store the name of the rigid bodies mapped against their ID
        private Hashtable rigidBodiesIDtoName = new Hashtable();
        // private List<RigidBody> blocks;


        public Experiment(RobotControl controller, List<RigidBody> mRigidBodies, FrameOfMocapData currentFrame)
        {
            
            this.controller = controller;
            writer = new StreamWriter(filename);

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

        public void update(NatNetML.FrameOfMocapData currentFrame)
        {
            // get the correct rigid body data objects from the frame
            // loop through RigidBody data
            for (int i = 0; i < currentFrame.nRigidBodies; i++)
            {
                NatNetML.RigidBodyData rb = currentFrame.RigidBodies[i];
                // get the hashcode of the id for later displaying in grid form
                int keyID = rb.ID.GetHashCode();
                if(rigidBodiesIDtoName.ContainsKey(keyID))
                {
                    string name = (string)rigidBodiesIDtoName[keyID];
                    htRigidBodiesNameToBody[name] = rb;
                    //if (name.Equals("robotBase")) robotBase = rb;
                    //else if (name.Equals("robotTip")) robotTip = rb;
                }
            }

            calculateDistanceBetween();
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
            calibrate(startingServoIndex);
            Console.WriteLine("calibration finished");

            saveDataXML();
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

                Console.WriteLine("motor angles are {0} {1} {2} {3}", motorAngles[0], motorAngles[1], motorAngles[2], motorAngles[3]);
                
                if(motorIndex < 3)
                {
                    calibrate(motorIndex + 1);
                }
                else if (motorIndex == 3)
                {
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

            newDataPoint.setMotorAngles(motorAngles);
            newDataPoint.setTipOrientation(relativeTipAngles);
            newDataPoint.setTipPos(relativeTipPos);

            testData.Add(newDataPoint);
        }

        private void saveDataXML()
        {
            ser.Serialize(writer, testData);
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
