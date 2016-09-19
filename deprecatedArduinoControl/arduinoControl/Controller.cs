using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace arduinoControl
{

    class Controller
    {

        private SerialPort currentPort;
        private bool connectedToPort;
        private int cnt = 0;

        public void run()
        {
            getComPort();
            if(connectedToPort == false)
            {
                Console.WriteLine("Could not locate arduino port");
                return;
            }
            else
            {
                Console.WriteLine("Arduino port is " + currentPort.PortName);
                controlArduino();
            }
        }


        private void controlArduino()
        {
            bool running = true;
            currentPort.Open();

            while (running)
            {
                string entered = Console.ReadLine();
                int num;
                if(Int32.TryParse(entered, out num) == false)
                {
                    Console.WriteLine("could not parse entered text"); ;
                    running = false;
                }
                //if(entered== "1")
                //{
                //    turnLedOn();
                //}
                //else if(entered == "0")
                //{
                //    turnLedOff();
                //}
                if(num > 0 && num < 180)
                {
                    byte[] instructionBuffer = new byte[1];
                    instructionBuffer[0] = Convert.ToByte(num);
                    currentPort.Write(instructionBuffer, 0, 1);
                }
                else if(num == 0)
                {
                    testFullMotion(1);
                }
                else {
                    running = false;
                    currentPort.Close();
                }
            }
        }

        private void testFullMotion(int startingServo)
        {
            byte[] instructionBuffer = new byte[2];
            instructionBuffer[0] = Convert.ToByte(startingServo);

            for (int i = 6; i < 13; i++ )
            {
                int angle = i * 10;
                instructionBuffer[1] = Convert.ToByte(angle);
                currentPort.Write(instructionBuffer, 0, 2);
                Console.WriteLine("servo " + startingServo + " receiving angle " + angle);
                Thread.Sleep(1000);
                cnt++;

                if(startingServo < 4)
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

                while(numReturningBytes > 0 )
                {
                    returnedASCIIint = currentPort.ReadByte();
                    returnedASCIIchar = Convert.ToChar(returnedASCIIint);
                    sb.Append(returnedASCIIchar);
                    numReturningBytes--;
                }

                currentPort.Close();
                string returnedMessage = sb.ToString();

                if(returnedMessage.Contains("HELLO FROM ARDUINO"))
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
