using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robotTracking
{
    [Serializable]
    public class BodeDataPoint
    {
        private float timeStamp;
        private float[] relativeTargetPosition = new float[3];
        private float[] relativeTipPosition = new float[3];

        public BodeDataPoint(float timeStamp, float[] relativeTargetPosition, float[] relativeTipPosition)
        {
            this.timeStamp = timeStamp;
            setTargetPos(relativeTargetPosition);
            setTipPos(relativeTipPosition);
        }


        private void setTargetPos(float[] relativeTargetPosition)
        {
            this.relativeTargetPosition[0] = relativeTargetPosition[0];
            this.relativeTargetPosition[1] = relativeTargetPosition[1];
            this.relativeTargetPosition[2] = relativeTargetPosition[2];

        }


        private void setTipPos(float[] relativeTipPosition)
        {
            this.relativeTipPosition[0] = relativeTipPosition[0];
            this.relativeTipPosition[1] = relativeTipPosition[1];
            this.relativeTipPosition[2] = relativeTipPosition[2];
        }


    }
}
