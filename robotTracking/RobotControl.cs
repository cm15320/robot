﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace robotTracking
{
    class RobotControl
    {
        private SerialPort currentPort;
        private bool connectedToPort;
        private int cnt = 0;
        private bool running;
        private int setMotorDelay = 4;
        private int triggerCode = 7;
        private int[] targetMotorAngles, currentMotorAngles;
        private int m1Offset = 0, m3Offset = 0;
        private bool usingOffset = false;
        public static string offsetFilename = "offset.csv";

        public object motorAngleLock;
        public object arduinoLock = new object();

        public RobotControl(object motorAngleLock)
        {
            this.motorAngleLock = motorAngleLock;
        }

        public bool initialise()
        {
            getComPort();
            if (connectedToPort == false)
            {
                Console.WriteLine("Could not locate arduino port");
                return false;
            }
            else
            {
                Console.WriteLine("Arduino port is " + currentPort.PortName);
                currentPort.Open();
                running = true;
                return true;
            }
        }

        
        public bool isConnected()
        {
            return connectedToPort;
        }


        public void uninitialise()
        {
            currentPort.Close();
        }

        private void zeroTargetMotors()
        {
            if(targetMotorAngles == null)
            {
                targetMotorAngles = new int[] { 90, 90, 90, 90 };
                return;
            }
            for(int i = 0; i < targetMotorAngles.Length; i++)
            {
                targetMotorAngles[i] = 90;
            }
        }

        public bool zeroMotors()
        {
            if (running)
            {
                if(currentMotorAngles != null)
                {
                    // If the motors are already at a position
                    zeroTargetMotors();
                    setMotorAngles();
                    return true;
                }
                byte[] instructionBuffer = new byte[2];
                //instructionBuffer[0] = Convert.ToByte(90);
                //instructionBuffer[1] = Convert.ToByte(90);
                //instructionBuffer[2] = Convert.ToByte(90);
                //instructionBuffer[3] = Convert.ToByte(90);

                //currentPort.Write(instructionBuffer, 0, 4);
                for (int i = 0; i < 4; i++)
                {
                    instructionBuffer[0] = Convert.ToByte(i + 1);
                    instructionBuffer[1] = Convert.ToByte(90);

                    lock(arduinoLock)
                    {
                        currentPort.Write(instructionBuffer, 0, 2);
                    }
                    Thread.Sleep(setMotorDelay);

                }
                currentMotorAngles = new int[] { 90, 90, 90, 90 };
                return true;

            }
            else
            {
                return false;
            }
        }


        public bool getTrigger()
        {
       
            
            int returnedInt = 2;
            int code = 7;
            //try
            //{
            //    // Byte pattern to get a response from arduino
            //    byte[] buffer = new byte[3];
            //    buffer[0] = Convert.ToByte(7);
            //    buffer[1] = Convert.ToByte(7);
            //    buffer[2] = Convert.ToByte(7);

            //    currentPort.Write(buffer, 0, 3);
            //    Thread.Sleep(1000); // Pause for 10ms to allow arduino to process the sent bytes

            //    int numReturningBytes = currentPort.BytesToRead;

            //    while (numReturningBytes > 0)
            //    {
            //        returnedInt = currentPort.ReadByte();
            //        numReturningBytes--;
            //    }

            //    Console.WriteLine(returnedInt);

            //    if (returnedInt == 1)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }

            //}
            //catch
            //{
            //    return false;
            //}
            try
            {
                byte[] buffer = new byte[2];
                buffer[0] = Convert.ToByte(triggerCode);
                buffer[1] = Convert.ToByte(code);
                int numReturningBytes;


                // could put a lock here with a different object (arduinoLock) to write safely
                // shared with the set motor angles method
                lock(arduinoLock)
                {
                    currentPort.Write(buffer, 0, 2);
                    numReturningBytes = currentPort.BytesToRead;
                }

                while (numReturningBytes > 0)
                {
                    returnedInt = currentPort.ReadByte();
                    numReturningBytes--;
                }

                returnedInt = (int)Char.GetNumericValue((char)returnedInt);

                if (returnedInt == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }

        }


        public void activateMagnet(bool on)
        {
            int code;
            if (on) code = 1;
            else code = 0;
            try
            {
                byte[] buffer = new byte[2];
                buffer[0] = Convert.ToByte(triggerCode);
                buffer[1] = Convert.ToByte(code);
                lock(arduinoLock)
                {
                    currentPort.Write(buffer, 0, 2);
                }
                //Thread.Sleep(3); // sleep after writing 
            }
            catch
            {
                Console.WriteLine("Not able to change magnet");
            }
        }


        public void setMotorAnglesTest(float[] testMotorAngles)
        {
            targetMotorAngles = new int[4];
            targetMotorAngles[0] = (int)testMotorAngles[0];
            targetMotorAngles[1] = (int)testMotorAngles[1];
            targetMotorAngles[2] = (int)testMotorAngles[2];
            targetMotorAngles[3] = (int)testMotorAngles[3];

            setMotorAngles();

        }

        private bool controlArduino(int servoNumber, int angle)
        {

            if (running)
            {

                if (angle >= 20 && angle <= 165 && servoNumber > 0 && servoNumber < 5)
                {
                    byte[] instructionBuffer = new byte[2];
                    instructionBuffer[0] = Convert.ToByte(servoNumber);
                    instructionBuffer[1] = Convert.ToByte(angle);
                    currentPort.Write(instructionBuffer, 0, 2);
                    return true;
                }
                else
                {

                    running = false;
                    currentPort.Close();
                    Console.WriteLine("Bad instruction, closing port");
                    return false;
                }   
            }

            return false;
        }

        public void test()
        {
            testPartMotion();
            // int startingServo = 1;
            //testFullMotion(startingServo);
        }

        public void shareMotorAngles(int[] targetMotorAngles)
        {
            //zeroMotors();
            this.targetMotorAngles = targetMotorAngles;
        }


        // this isn't really needed as can just change the live value in the array instead of setting an integer
        // then setting the value in the array to that integer
        private void updateCurrentMotorAngles(int i, int angle)
        {
            currentMotorAngles[i] = angle;
            //currentMotorAngles[1] = targetMotorAngles[1];
            //currentMotorAngles[2] = targetMotorAngles[2];
            //currentMotorAngles[3] = targetMotorAngles[3];

        }


        // Sends the appropriate signals to the Arduino to set the motor angles
        public void setMotorAngles()
        {
            // Already eliminated extreme angles in the Experiment class and no NaN values should have been added
            byte[] instructionBuffer = new byte[2];
            int oldAngle, newAngle, toWrite;
            while (anglesNotEqual())
            {
                for (int i = 0; i < 4; i++)
                {
                    instructionBuffer[0] = Convert.ToByte(i + 1);
                    oldAngle = currentMotorAngles[i];
                    newAngle = targetMotorAngles[i];

                    if (oldAngle < newAngle) oldAngle++;
                    else if (oldAngle > newAngle) oldAngle--;
                    else continue;

                    toWrite = oldAngle;

                    if (usingOffset && i == 0) toWrite += m1Offset;
                    if (usingOffset && i == 2) toWrite += m3Offset;

                    instructionBuffer[1] = Convert.ToByte(toWrite);

                    lock(arduinoLock) // So other threads don't simultaneously write to Arduino and cause incorrect serial transmission
                    {
                        currentPort.Write(instructionBuffer, 0, 2);
                    }

                    Thread.Sleep(setMotorDelay);
                    updateCurrentMotorAngles(i, oldAngle);
                }
            }
        }

        private bool anglesNotEqual()
        {
            int cnt = 0;
            lock(motorAngleLock)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (targetMotorAngles[i] == currentMotorAngles[i]) cnt++;
                }
            }
            if (cnt == 4) return false;
            else return true;
        }

        private void testPartMotion()
        {
            byte[] instructionBuffer = new byte[2];
            int[] startingPosition = new int[] { 90, 90, 90, 90 };
            shareMotorAngles(startingPosition);
            targetMotorAngles = new int[] { 110, 110, 110, 110 };
            setMotorAngles();

            //for (int i = 0; i < 4; i++)
            //{
            //    instructionBuffer[0] = Convert.ToByte(i + 1);
            //    instructionBuffer[1] = Convert.ToByte(110);

            //    currentPort.Write(instructionBuffer, 0, 2);

            //    Thread.Sleep(25);

            //}

            Thread.Sleep(1000);
            targetMotorAngles = new int[] { 70, 70, 70, 70 };
            setMotorAngles();


            Thread.Sleep(500);

            zeroMotors();

            //targetMotorAngles = new int[] { 120, 90, 60, 90 };
            //setMotorAngles();

            //for (int i = 0; i < 4; i++)
            //{
            //    instructionBuffer[0] = Convert.ToByte(i + 1);
            //    instructionBuffer[1] = Convert.ToByte(80);

            //    currentPort.Write(instructionBuffer, 0, 2);

            //    Thread.Sleep(25);

            //}


            //Thread.Sleep(20);

            //instructionBuffer[0] = Convert.ToByte(70);
            //instructionBuffer[1] = Convert.ToByte(70);
            //instructionBuffer[2] = Convert.ToByte(70);
            //instructionBuffer[3] = Convert.ToByte(70);

            //currentPort.Write(instructionBuffer, 0, 4);


        }

        private void testFullMotion(int startingServo)
        {
            byte[] instructionBuffer = new byte[2];
            instructionBuffer[0] = Convert.ToByte(startingServo);

            for (int i = 6; i < 13; i++)
            {
                int angle = i * 10;
                instructionBuffer[1] = Convert.ToByte(angle);
                currentPort.Write(instructionBuffer, 0, 2);
                Console.WriteLine("servo " + startingServo + " receiving angle " + angle);
                Thread.Sleep(1000);
                cnt++;

                if (startingServo < 4)
                {
                    testFullMotion(startingServo + 1);
                }

                Console.WriteLine("count is " + cnt);
            }
        }



        private void turnLedOn()
        {
            currentPort.Write("a");
        }

        private void turnLedOff()
        {
            currentPort.Write("b");
        }

        private void getComPort()
        {
            try
            {
                string[] availablePorts = SerialPort.GetPortNames();
                foreach (string port in availablePorts)
                {
                    currentPort = new SerialPort(port, 57600);
                    if (detectArduinoPort())
                    {
                        connectedToPort = true;
                        return;
                    }
                    else

                    {
                        connectedToPort = false;
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        private bool detectArduinoPort()
        {
            int returnedASCIIint = 0;
            char returnedASCIIchar = (char)returnedASCIIint;

            try
            {
                // Byte pattern to get a 'hello' handshake from arduino
                byte[] buffer = new byte[5];
                buffer[0] = Convert.ToByte(16);
                buffer[1] = Convert.ToByte(128);
                buffer[2] = Convert.ToByte(0);
                buffer[3] = Convert.ToByte(0);
                buffer[4] = Convert.ToByte(4);

                currentPort.DtrEnable = true;
                currentPort.Open();
                Thread.Sleep(1000);
                currentPort.DtrEnable = false;
                Thread.Sleep(1000);

                currentPort.Write(buffer, 0, 5);
                Thread.Sleep(1000); // Pause for a second to allow arduino to process the sent bytes

                int numReturningBytes = currentPort.BytesToRead;
                StringBuilder sb = new StringBuilder();

                while (numReturningBytes > 0)
                {
                    returnedASCIIint = currentPort.ReadByte();
                    returnedASCIIchar = Convert.ToChar(returnedASCIIint);
                    sb.Append(returnedASCIIchar);
                    numReturningBytes--;
                }

                currentPort.Close();
                string returnedMessage = sb.ToString();

                if (returnedMessage.Contains("HELLO FROM ARDUINO"))
                {
                    Console.WriteLine(returnedMessage);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }
        }


        public void readOffset()
        {
            StreamReader reader;
            string line;
            int m1OffsetStored, m3OffsetStored;

            try
            {
                reader = new StreamReader(File.OpenRead(offsetFilename));
                line = reader.ReadLine();
            }
            catch(Exception)
            {
                Console.WriteLine("Failed to read in offsets properly");
                return;
            }

            string[] stringValues = line.Split(';');
            if (stringValues.Length != 2)
            {
                Console.WriteLine("Failed to read in offsets properly");
                return;
            }

            for (int j = 0; j < stringValues.Length; j++)
            {
                stringValues[j] = stringValues[j].Trim();
            }

            try
            {
                m1OffsetStored = Convert.ToInt32(stringValues[0]);
                m3OffsetStored = Convert.ToInt32(stringValues[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error in converting string to float, mssage : " + ex.Message);
                return;
            }

            m1Offset = m1OffsetStored;
            m3Offset = m3OffsetStored;

            Console.WriteLine("read in offsets of: {0} and {1}", m1Offset, m3Offset);

            usingOffset = true;
        }


    }

}

