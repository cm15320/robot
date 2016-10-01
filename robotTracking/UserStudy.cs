using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        private float[] relativeTargetPosition = new float[3];
        private bool oldestTriggerPress = false, oldTriggerPress = false;
        private bool triggerPress = false;
        private int numTriggerPresses = 0;
        private float[][] targetPositions;
        private bool running, initialised;
        private bool justReleased = false;
        public static string gesturingFilename = "gesturingPositions.csv";
        public static string robotColourFilename = "robotColourPositions.csv";
        public static string userColourFilename = "userColourPositions.csv";
        private bool randomisedOrder = false;
        private int[] newOrder;
        private bool toRandom = false; // Set to true if want to randomise the order of the target positions


        public UserStudy(UserStudyType type)
        {
            this.type = type;
            running = true;

            if (populateTargetPositions())
            {
                initialised = true;
            }
            else
            {
                initialised = false;
                Console.WriteLine("Not able to read in positions from file correctly");
            }
        }

        public bool isInitialised()
        {
            return initialised;
        }


        public bool update(float[] basePosition, bool triggerPress)
        {
            this.basePosition = basePosition;
            updateTriggerPress(triggerPress);
            updateTarget();

            return running;
        }


        private bool populateTargetPositions()
        {

            bool success;

            switch(type)
            {
                case UserStudyType.GESTURING:
                    success = readPositionsFromFile(gesturingFilename);
                    break;
                case UserStudyType.ROBOTCOLOUR:
                    success = readPositionsFromFile(robotColourFilename);
                    break;
                case UserStudyType.USERCOLOUR:
                    success = readPositionsFromFile(userColourFilename);
                    break;
                default:
                    return false;
            }

            if(success && type == UserStudyType.GESTURING && toRandom)
            {
                randomiseOrder(5, targetPositions);
            }

            return success;
        }


        // Get the positions of the targets from the stored csv files
        private bool readPositionsFromFile(string filename)
        {
            int numLines;
            StreamReader reader = new StreamReader(File.OpenRead(filename));
            List<string> listOfPositions = new List<string>();
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                listOfPositions.Add(line);
            }
            numLines = listOfPositions.Count;
            if (numLines == 0) return false;
            targetPositions = new float[numLines][];


            for (int i = 0; i < listOfPositions.Count; i++)
            {
                string line = listOfPositions[i];
                string[] stringValues = line.Split(';');
                if (stringValues.Length != 3) return false;

                for(int j = 0; j < stringValues.Length; j++)
                {
                    stringValues[j] = stringValues[j].Trim();
                }

                float[] position = getPositionFromString(stringValues);
                if (position == null) return false;

                targetPositions[i] = position;
            }

            return true;

        }


        // Reads an array of string values and returns it as an array of float values
        private float[] getPositionFromString(string[] stringValues)
        {
            float[] position = new float[3];
            for (int i = 0; i < stringValues.Length; i++)
            {
                try
                {
                    float value = float.Parse(stringValues[i], CultureInfo.InvariantCulture.NumberFormat);
                    position[i] = value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error in converting string to float, mssage : " + ex.Message);
                    return null;
                }
            }

            return position;

        }


        public float[] getRelativeTargetPosition()
        {
            return relativeTargetPosition;
        }


        public bool isJustReleased()
        {
            return justReleased;
        }

        private void updateTriggerPress(bool newTriggerPress)
        {
            oldestTriggerPress = oldTriggerPress;
            oldTriggerPress = triggerPress;
            triggerPress = newTriggerPress;

            justReleased = false;

            // If trigger has been released, increase number of trigger presses
            if(oldestTriggerPress == true && oldTriggerPress == false && triggerPress == false)
            {
                justReleased = true;
                numTriggerPresses++;
                Console.WriteLine("num trigger presses is " + numTriggerPresses);
            }
        }

        
        private void updateTarget()
        {
            if(numTriggerPresses >= targetPositions.Length)
            {
                //Console.WriteLine("finished study");
                return;
            }
            absoluteTargetPosition = targetPositions[numTriggerPresses];

            calculateRelativeTarget();
        }


        private void calculateRelativeTarget()
        {
            //Console.WriteLine("new rel target is:");
            for(int i = 0; i < absoluteTargetPosition.Length; i++)
            {
                relativeTargetPosition[i] = absoluteTargetPosition[i] - basePosition[i];
                //Console.Write(relativeTargetPosition[i]);
            }
        }

        public void testTargetPositions()
        {
            if(!initialised)
            {
                Console.WriteLine("failed to read in positions correctly");
                return;
            }

            Console.WriteLine("target positions are:");

            for(int i = 0; i < targetPositions.Length; i++)
            {
                for(int j = 0; j < targetPositions[0].Length; j++)
                {
                    Console.Write(targetPositions[i][j] + "   ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("randomising....");
            randomiseOrder(targetPositions.Length, targetPositions);
            printRandomisedOrder();
            
        }


        public void undoTarget()
        {
            if(numTriggerPresses > 0)
            {
                numTriggerPresses--;
                Console.WriteLine("num trigger presses is now: " + numTriggerPresses);
            }
        }


        private void randomiseOrder(int newArraySize, float[][] positions)
        {
            Random rnd = new Random();
            if(newArraySize > positions.Length)
            {
                Console.WriteLine("Cannot have a new array size greater than the number of positions");
                return;
            }

            newOrder = new int[newArraySize];
            float[][] newPositions = new float[newArraySize][];
            initialiseArrayTo(newOrder, -1); // set it to -1 so the array contains no numbers that will come up randomly

            for (int i = 0; i < newArraySize; i++)
            {
                int newLocationIndex = rnd.Next(0, positions.Length);
                while(newOrder.Contains(newLocationIndex))
                {
                    newLocationIndex = rnd.Next(positions.Length);
                }
                newOrder[i] = newLocationIndex;
                newPositions[i] = positions[newLocationIndex];
            }

            randomisedOrder = true;
            targetPositions = newPositions;
        }


        private void initialiseArrayTo(int[] array, int value)
        {
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }


        public void printRandomisedOrder()
        {
            if (!randomisedOrder) return;

            Console.WriteLine("new order was");
            for(int i = 0; i < newOrder.Length; i++)
            {
                Console.Write(newOrder[i] + "  ");
            }
            Console.WriteLine();
            Console.WriteLine("with actual values of:" );
            for (int i = 0; i < targetPositions.Length; i++)
            {
                for (int j = 0; j < targetPositions[0].Length; j++)
                {
                    Console.Write(targetPositions[i][j] + "   ");
                }
                Console.WriteLine();
            }
        }


    }
}
