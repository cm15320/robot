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
        public float[] motorAngles = new float[4];
        public float[] relativeTipPosition = new float[3];
        public float[] relativeTipOrientation = new float[3];

        public void setMotorAngles(float[] motorAngles)
        {
            this.motorAngles = motorAngles;
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
