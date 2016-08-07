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
        private RigidBodyData robotBase, robotTip;
        // need to store the name of the rigid bodies mapped against their ID
        private Hashtable rigidBodiesNameToID = new Hashtable();
        // private List<RigidBody> blocks;

        public Experiment(RobotControl controller, List<RigidBody> mRigidBodies)
        {
            this.controller = controller;
            if(mRigidBodies.Count == 0)
            {
                Console.WriteLine("Error, no rigid body info stored");
            }
        }

        public void update(NatNetML.FrameOfMocapData currentFrame)
        {
            
        }

        public void calibrate()
        {

        }
    }
}
