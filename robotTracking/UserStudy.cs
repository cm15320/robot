using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robotTracking
{
    class UserStudy
    {
        private UserStudyType type;
        private float[] basePosition;
        private float[] absoluteTargetPosition;
        private float[] relativeTargetPosition;
        private bool triggerPress = false;
        private int numTriggerPresses = 0;
        private float[][] targetPositions;
        private bool completed = false;


        public UserStudy(UserStudyType type)
        {
            this.type = type;
        }


        public bool update(float[] basePosition, bool triggerPress)
        {
            this.basePosition = basePosition;
            updateTriggerPress(triggerPress);
            updateTarget();

            return completed;
        }


        public float[] getRelativeTargetPosition()
        {
            return relativeTargetPosition;
        }


        private void updateTriggerPress(bool triggerPress)
        {
            bool oldTriggerPress = this.triggerPress;
            this.triggerPress = triggerPress;

            // If trigger has been released, increase number of trigger presses
            if(oldTriggerPress == true && triggerPress == false)
            {
                numTriggerPresses++;
            }
        }


        private void updateTarget()
        {
            if(numTriggerPresses > targetPositions.Length)
            {
                completed = true;
                return;
            }
            absoluteTargetPosition = targetPositions[numTriggerPresses];

            calculateRelativeTarget();
        }


        private void calculateRelativeTarget()
        {
            for(int i = 0; i < absoluteTargetPosition.Length; i++)
            {
                relativeTargetPosition[i] = absoluteTargetPosition[i] - basePosition[i];
            }
        }


    }
}
