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
        // will use this eventually just testing with old calibration data
        //public float[] combinedTipPosOri = new float[6];

        public void setMotorAngles(int[] motorAngles)
        {
            this.motorAngles[0] = motorAngles[0];
            this.motorAngles[1] = motorAngles[1];
            this.motorAngles[2] = motorAngles[2];
            this.motorAngles[3] = motorAngles[3];

        }

        public void setTipPos(float[] relativeTipPosition)
        {
            this.relativeTipPosition[0] = /*combinedTipPosOri[0] = */relativeTipPosition[0];
            this.relativeTipPosition[1] = /*combinedTipPosOri[1] = */relativeTipPosition[1];
            this.relativeTipPosition[2] = /*combinedTipPosOri[2] = */relativeTipPosition[2];
        }

        public void setTipOrientation(float[] relativeTipOrientation)
        {
            this.relativeTipOrientation[0] = /*combinedTipPosOri[3] = */relativeTipOrientation[0];
            this.relativeTipOrientation[1] = /*combinedTipPosOri[4] = */relativeTipOrientation[1];
            this.relativeTipOrientation[2] = /*combinedTipPosOri[5] = */relativeTipOrientation[2];
        }


    }

}
