using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace robotTracking
{
    class RobotControl
    {
        private SerialPort currentPort;
        private bool connectedToPort;
        private int cnt = 0;
        private bool running;
        private int[] motorAngles, oldMotorAngles;

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

        public bool zeroMotors()
        {
            if (running)
            {
                if(oldMotorAngles != null)
                {
                    // If the motors are already at a position
                    motorAngles = new int[] { 90, 90, 90, 90 };
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

                    currentPort.Write(instructionBuffer, 0, 2);
                    Thread.Sleep(25);

                }
                oldMotorAngles = new int[] { 90, 90, 90, 90 };
                return true;

            }
            else
            {
                return false;
            }
        }


        public void setMotorAnglesTest(float[] testMotorAngles)
        {
            motorAngles = new int[4];
            motorAngles[0] = (int)testMotorAngles[0];
            motorAngles[1] = (int)testMotorAngles[1];
            motorAngles[2] = (int)testMotorAngles[2];
            motorAngles[3] = (int)testMotorAngles[3];

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

        public void shareMotorAngles(int[] motorAngles)
        {
            zeroMotors();
            this.motorAngles = motorAngles;
        }

        private void updateOldMotorAngles(int i, int angle)
        {
            oldMotorAngles[i] = angle;
            //oldMotorAngles[1] = motorAngles[1];
            //oldMotorAngles[2] = motorAngles[2];
            //oldMotorAngles[3] = motorAngles[3];

        }

        public void setMotorAngles()
        {
            byte[] instructionBuffer = new byte[2];

            while (anglesNotEqual())
            {
                for (int i = 0; i < 4; i++)
                {
                    instructionBuffer[0] = Convert.ToByte(i + 1);
                    int oldAngle = oldMotorAngles[i];
                    int newAngle = motorAngles[i];

                    if (oldAngle < newAngle) oldAngle++;
                    else if (oldAngle > newAngle) oldAngle--;
                    else continue;

                    instructionBuffer[1] = Convert.ToByte(oldAngle);

                    currentPort.Write(instructionBuffer, 0, 2);
                    //Console.WriteLine("writing motor angles");

                    Thread.Sleep(6);
                    updateOldMotorAngles(i, oldAngle);

                }
            }
            //Console.WriteLine("finished writing");

        }

        private bool anglesNotEqual()
        {
            int cnt = 0;
            for (int i = 0; i < 4; i++)
            {
                if (motorAngles[i] == oldMotorAngles[i]) cnt++;
            }
            if (cnt == 4) return false;
            else return true;
        }

        private void testPartMotion()
        {
            byte[] instructionBuffer = new byte[2];
            int[] startingPosition = new int[] { 90, 90, 90, 90 };
            shareMotorAngles(startingPosition);
            motorAngles = new int[] { 110, 110, 110, 110 };
            setMotorAngles();

            //for (int i = 0; i < 4; i++)
            //{
            //    instructionBuffer[0] = Convert.ToByte(i + 1);
            //    instructionBuffer[1] = Convert.ToByte(110);

            //    currentPort.Write(instructionBuffer, 0, 2);

            //    Thread.Sleep(25);

            //}

            Thread.Sleep(2000);
            motorAngles = new int[] { 80, 80, 80, 80 };
            setMotorAngles();


            Thread.Sleep(500);

            zeroMotors();

            //motorAngles = new int[] { 120, 90, 60, 90 };
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
    }
}
