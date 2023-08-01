using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using EasyModbus;

namespace gcp_Wpf.commClass
{
    internal class modbusRtuClass
    {
        private readonly SerialPort _serialPort;
        private readonly ModbusClient _modbusClient;
        private readonly Thread _modbusThread;
        private bool _isRunning;
        int srmNum;
        string port;
        int baudRate;
        int parity;
        int dataBits;
        int stopBits;

        //Singletone
        singletonClass gClass;
        public modbusRtuClass(int srmNum, string port, int baudRate, int parity, int dataBits, int stopBits)
        {
            gClass = singletonClass.Instance;
            this.srmNum = srmNum;
            this.port = port;
            this.baudRate = baudRate;
            this.parity = parity;
            this.dataBits = dataBits;
            this.stopBits = stopBits;

            try
            {
                // test
                _serialPort = new SerialPort("COM8", 9600, Parity.None, 8, StopBits.One);
                //_serialPort = new SerialPort(port, baudRate, parity, dataBits, stopBits);
                //_serialPort.Open();

                if (!_serialPort.IsOpen)
                {
                    //System.Console.WriteLine("Modbus RTU Comm Open - " + port + "is Error");
                    //throw new Exception("Failed to open serial port");
                }
                else
                {
                    //System.Console.WriteLine("Modbus RTU Comm Opened - " + port);

                }
                // Initialize ModbusClient
                _modbusClient = new ModbusClient(_serialPort.PortName);
                //_modbusClient = new ModbusClient(port);
                //_modbusClient.SerialPort = "COM8";
                _modbusClient.Baudrate = 9600;
                _modbusClient.Parity = Parity.None;
                _modbusClient.StopBits = StopBits.One;
                _modbusClient.UnitIdentifier = 1;
                _modbusClient.ConnectionTimeout = 10000;

                // Initialize Modbus thread
                _modbusThread = new Thread(ModbusThreadFunction);
                _isRunning = true;
                _modbusThread.Start();

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Modbus Comm Error: " + ex.Message);
                // Handle the exception, e.g. display an error message
            }
            finally
            {
                // Disconnect from the Modbus device
                if (_modbusThread.IsAlive)
                {
                    //_modbusThread.Abort();
                    //_serialPort.Close();
                }
            }
        }

        private void ModbusThreadFunction()
        {
            while (_isRunning)
            {
                try
                {
                    // Connect to the Modbus device
                    _modbusClient.Connect();

                    // Read holding registers starting at address 0x0000-------------------------------------------
                    var startAddress = 0x0000;
                    var numRegisters = cConstDefine.IOCOUNT;
                    //int[] values = _modbusClient.ReadHoldingRegisters(startAddress, numRegisters);

                    // // Write the data to the specified registers
                    int[] writeData = new int[numRegisters];
                    for (int i = 0; i < numRegisters; i++)
                    {
                        writeData[i] = gClass.str.DioPacket[srmNum].DO[i]; // Replace with your desired values to be written
                    }
                    //_modbusClient.WriteMultipleRegisters(startAddress, writeData);

                    // Read/Write Multiple register -------------------------------------------
                    int[] values = _modbusClient.ReadWriteMultipleRegisters(startAddress, numRegisters, startAddress+cConstDefine.IOCOUNT, writeData);


                    // Convert int[] to ushort[]
                    ushort[] ushortValues = new ushort[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        if(i < cConstDefine.IOCOUNT)
                        {
                            gClass.str.DioPacket[srmNum].DI[i] = (ushort)values[i];
                        }
                        ushortValues[i] = (ushort)values[i];
                    }

                    // Do something with the registers, e.g. update UI
                    //Dispatcher.Invoke(() =>
                    //{
                    //    // Update UI here
                    //    // Example: update a TextBlock with the value of the first register
                    //    //TextBlockRegisterValue.Text = values[0].ToString();
                    //});

                    // Disconnect from the Modbus device
                    _modbusClient.Disconnect();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Modbus Comm Error: " + ex.Message + " " + srmNum);
                    // Handle the exception, e.g. display an error message
                    // ...
                }

                // Wait for 1 second before reading again
                Thread.Sleep(2000);
            }

            Console.WriteLine("Modbus Thread Finished " + srmNum);
        }

        public void Close()
        {
            // Stop Modbus thread and clean up resources
            _isRunning = false;
            _modbusThread.Join();

            if(_modbusClient != null)
            {
                if (_modbusClient.Connected)
                {
                    _modbusClient.Disconnect();
                }
            }
            //if (_serialPort != null && _serialPort.IsOpen)
            //{
            //    _serialPort.Close();
            //}
        }
    }
}
