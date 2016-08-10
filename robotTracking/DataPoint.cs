using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robotTracking
{
    [Serializable]
    public class DataPoint
    {
        public int[] motorAngles = new int[4];
        public float[] relativeTipPosition = new float[3];
        public float[] relativeTipOrientation = new float[3];

        public void setMotorAngles(int[] motorAngles)
        {
            this.motorAngles[0] = motorAngles[0];
            this.motorAngles[1] = motorAngles[1];
            this.motorAngles[2] = motorAngles[2];
            this.motorAngles[3] = motorAngles[3];

        }
        public void setTipPos(float[] relativeTipPosition)
        {
            this.relativeTipPosition = relativeTipPosition;
        }
        public void setTipOrientation(float[] relativeTipOrientation)
        {
            this.relativeTipOrientation = relativeTipOrientation;
        }


    }

}
