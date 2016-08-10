using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NatNetML;
using System.Collections;

namespace robotTracking
{
    class Experiment
    {
        private RobotControl controller;
        private RigidBodyData robotBase = new RigidBodyData();
        private RigidBodyData robotTip = new RigidBodyData();

        private Hashtable htRigidBodiesNameToBody;
        private double distanceBetween;
        
        
        //private string[] bodyNames = new string[] { "robotBase", "robotTip" };
        // need to store the name of the rigid bodies mapped against their ID
        private Hashtable rigidBodiesIDtoName = new Hashtable();
        // private List<RigidBody> blocks;


        public Experiment(RobotControl controller, List<RigidBody> mRigidBodies)
        {
            
            this.controller = controller;
            string[] requiredNames = new string[] { "robotBase", "robotTip" };
            if(mRigidBodies.Count == 0)
            {
                Console.WriteLine("Error, no rigid body info stored");
            }
            foreach(RigidBody rb in mRigidBodies)
            {
                // if bodies to id ht contatins key (rb. name) then add it to ID to Name ht
                if(requiredNames.Contains(rb.Name))
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
                    //htRigidBodiesNameToBody[name] = rb;
                    if (name.Equals("robotBase")) robotBase = rb;
                    else if (name.Equals("robotTip")) robotTip = rb;
                }
            }

            calculateDistanceBetween();
        }

        private void calculateDistanceBetween()
        {
            // If this doesn't work then get hash code from the string and use that instead
            //RigidBodyData robotBase = (RigidBodyData)htRigidBodiesNameToBody["robotBase"];
            //RigidBodyData robotTip = (RigidBodyData)htRigidBodiesNameToBody["robotTip"];

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

        }
    }
}
