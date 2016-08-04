using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NatNetML;

namespace robotTracking
{
    class Experiment
    {
        private RobotControl controller;
        private RigidBody robotBase, robotTip;
        private List<RigidBody> blocks;

        public Experiment(RobotControl controller)
        {
            this.controller = controller;
        }

        public void update(NatNetML.FrameOfMocapData currentFrame)
        {

        }
    }
}
