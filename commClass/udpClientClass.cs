using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace gcp_Wpf.commClass
{
    internal class udpClientClass
    {
        private byte[] sendBuffer = new byte[1024];     
        private string sendStr;
        private uint oldNum;                             // 이전 통신 시퀀스 번호
        private uint seqNum;                             // 현재 통신 시퀀스 번호
        private int srmNum;
        private bool isRunning;

        private string ipAddress;
        private int port;
        private UdpClient udpClient;
        IPEndPoint remoteEndPoint;
        IPEndPoint localEndPoint;
        private List<byte[]> txDataList = new List<byte[]>();
        private Thread receiveThread = null;
        System.Timers.Timer sendTimer = new System.Timers.Timer();

        static System.Threading.Mutex mutex = new System.Threading.Mutex();

        string pathString;

        //Singletone
        singletonClass gClass;
        //public delegate void UdpCallbackDelegate(string message, IPEndPoint remoteEndpoint);

        public udpClientClass(int srmNum)
        {
            gClass = singletonClass.Instance;
            this.srmNum = srmNum;

            // type 이 달라질 경우 대비  -------------------- 230614 현재는 미사용
            int type = 1;

            switch (type)
            {
                case 1:             // SRM 로그 저장을 위함
                    pathString = System.IO.Path.Combine(Environment.CurrentDirectory, "SRM" + srmNum, cConstDefine.PATH_LOG, cConstDefine.PATH_SRMLOG);
                    break;
                case 2:
                    //pathString = System.IO.Path.Combine(Environment.CurrentDirectory, "SRM" + srmNum, cConstDefine.PATH_LOG, cConstDefine.PATH_SRMLOG);
                    break;
                case 3:
                    //pathString = System.IO.Path.Combine(Environment.CurrentDirectory, "SRM" + srmNum, cConstDefine.PATH_LOG, cConstDefine.PATH_DIOLOG);
                    break;
            }
        }

        public void connect(string ipAddress, int port)
        {
            // 시퀀스 번호 초기화
            seqNum = 1;
            gClass.str.SrmPacket[gClass.srmNum].recvStr.seqNum = 0;

            this.ipAddress = ipAddress;
            this.port = port;
            udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 3000;
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            localEndPoint = new IPEndPoint(IPAddress.Any, 0);
            
            udpClient.Client.Bind(localEndPoint);
            //client.BeginSend(data, data.Length, hostname, port, callback, client);
           // udpClient.Connect(remoteEndPoint);
            //udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);

            receiveThread = new Thread(new ThreadStart(ReceiveData));                   // 230517 Receive 콜백함수
            isRunning = true;
            receiveThread.Start();

            sendTimer.Interval = 1000; // 1 second
            sendTimer.AutoReset = true; // Repeat the timer
            sendTimer.Elapsed += sendTimer_Elapsed;

            sendTimer.Start();

            // Test
            //Tx_SendCmd(0x00, 0x50);
        }

        private async void SaveLogFile(string text)
        {
            await Task.Run(() =>
            {
                mutex.WaitOne();

                if (!Directory.Exists(pathString))
                {
                    Directory.CreateDirectory(pathString);
                    Console.WriteLine("Folder created at: " + pathString);
                }


                string filePath = System.IO.Path.Combine(pathString, "SRMLOG_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

                if (!File.Exists(filePath))
                {
                    using (StreamWriter writer = File.CreateText(filePath))
                    {
                        writer.WriteLine("File created on " + DateTime.Now.ToString());
                    }
                }

                // Write the text to the file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(text);
                    //Console.WriteLine(text + "  " + srmNum);                  // 로그 콘솔화면 출력
                }

                mutex.ReleaseMutex();
            });


            // SRM Log Dialog SendMessage
            IntPtr WindowToFind = cConstDefine.FindWindow(null, "WindowSrmLog" + (srmNum + 1));
            //IntPtr WindowToFind = win_SrmLog.GetHandle();
            if (WindowToFind != IntPtr.Zero)
            {
                //Console.WriteLine("Send Udp Message " + WindowToFind);
                IntPtr hwnd = WindowToFind;
                var copyData = new cConstDefine.COPYDATASTRUCT();
                copyData.dwData = IntPtr.Zero;
                copyData.lpData = text;
                copyData.cbData = Encoding.Unicode.GetBytes(text).Length + 1; // add 1 for null-terminator
                cConstDefine.SendMessage(WindowToFind, cConstDefine.WM_USER, IntPtr.Zero, ref copyData);               // Send - Post 차이 비교 필요
                //PostMessage(WindowToFind, cConstDefine.WM_USER, IntPtr.Zero, ref copyData);

            }
            else
            {
                Console.WriteLine("Find Srm Window Fail " + WindowToFind);
            }
        }


        private ushort crc16_ccitt(byte[] bufBytes)
        {
            ushort crc = 0;

            for (int i = 0; i < bufBytes.Length; i++)
            {
                crc = (ushort)((crc << 8) ^ cConstDefine.crc16tab[((crc >> 8) ^ bufBytes[i]) & 0x00FF]);
            }
            return crc;
        }

        private byte bcc_check(byte[] bufBytes)
        {
            byte bcc = 0;
            foreach (byte b in bufBytes)
            {
                bcc ^= b;
            }
            return bcc;
        }

        private void sendTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // for test
            //byte[] myBytes = Encoding.ASCII.GetBytes(sendStr);
            //string asciiString = Encoding.ASCII.GetString(bytes);
         

            //IntPtr WindowToFind = cConstDefine.FindWindow(null, "WindowSrmLog"+(srmNum + 1));
            ////IntPtr WindowToFind = win_SrmLog.GetHandle();
            //if (WindowToFind != IntPtr.Zero)
            //{
            //    string message = "SEND: " + sendStr;
            //    IntPtr hwnd = WindowToFind;
            //    var copyData = new cConstDefine.COPYDATASTRUCT();
            //    copyData.dwData = IntPtr.Zero;
            //    copyData.lpData = message;
            //    copyData.cbData = Encoding.Unicode.GetBytes(message).Length + 1; // add 1 for null-terminator
            //    cConstDefine.SendMessage(WindowToFind, cConstDefine.WM_USER, IntPtr.Zero, ref copyData);               // Send - Post 차이 비교 필요
            //    //PostMessage(WindowToFind, cConstDefine.WM_USER, IntPtr.Zero, ref copyData);

            //}
            //else
            //{
            //    Console.WriteLine("Find Srm Window Fail " + WindowToFind);
            //}
        }

//---------------------------미사용-----------------------------------------------------
        //private void ReceiveCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        //IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        //        byte[] bytes = udpClient.EndReceive(ar, ref remoteEndPoint);

        //        string message = Encoding.ASCII.GetString(bytes);
        //        // process the received message
        //        Console.WriteLine($"Received message from {remoteEndPoint}: {message}");

        //        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //}
//------------------------------------------------------------------------------------------

        public void Send(byte[] data)
        {
            //byte[] data = Encoding.ASCII.GetBytes(message);
            //udpClient = new UdpClient();
            int sendByte = udpClient.Send(data, data.Length, remoteEndPoint);

            if(sendByte != 0)
            {
                // Send Data Parse --------------------------------------------------
                gClass.str.SrmPacket[srmNum].sendStr.srcType = data[4];
                gClass.str.SrmPacket[srmNum].sendStr.srcID = data[5];
                gClass.str.SrmPacket[srmNum].sendStr.dstType = data[6];
                gClass.str.SrmPacket[srmNum].sendStr.dstID = data[7];
                gClass.str.SrmPacket[srmNum].sendStr.seqNum = data[8];
                gClass.str.SrmPacket[srmNum].sendStr.byPass1 = data[9];
                gClass.str.SrmPacket[srmNum].sendStr.byPass2 = data[10];
                gClass.str.SrmPacket[srmNum].sendStr.cmd1 = data[11];
                gClass.str.SrmPacket[srmNum].sendStr.len = BitConverter.ToUInt16(data, 12);
                gClass.str.SrmPacket[srmNum].sendStr.cmd2 = data[14];

                string dataString = BitConverter.ToString(data).Replace("-", string.Empty);
                //string asciiString = Encoding.ASCII.GetString(data);
                SaveLogFile("SEND: " + dataString);
            }

            //byte[] responseData = udpClient.Receive(ref remoteEndPoint);
            //string responseMessage = Encoding.UTF8.GetString(responseData);
            //Console.WriteLine($"Received response from server: {responseMessage}");
        }

        private void ReceiveData()
        {
            while (isRunning)
            {
                try
                {
                    Thread.Sleep(1000);
                    Send_Command();

                    //Console.WriteLine("Receive Wait UDP data: " + ipAddress + " " + port);
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    //remoteEndPoint = localEndPoint;
                    //string message = Encoding.ASCII.GetString(data);
                    string message = BitConverter.ToString(data).Replace("-", string.Empty);
                    // Handle the incoming data here, e.g. raise an event or call a callback

                    // 전송받은 데이터 파싱
                    bool result = Rx_CommandParse(data);

                    if (result)         // 리시브 헤더, CRC 정상확인
                    {
                        result = Rx_DataParse(gClass.str.SrmPacket[srmNum].recvStr.cmd1, gClass.str.SrmPacket[srmNum].recvStr.cmd2);
                        if (result)         // 리시브 데이터 정상 확인
                        {
                            seqNum++;
                            if (seqNum > 255) seqNum = 0;   // 수신 데이터 파싱 정상 시 seqNum 증가
                        }
                        else
                        {
                            //Tx_SendCmd(0x00, 0x30);
                        }
                    }
                    else
                    {
                        //Tx_SendCmd(0x00, 0x50);
                    }
                    //Send(message);
                    //230517-----------------------------------------------------------------

                    OnUdpDataReceived(message);
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("Thread Abort Exception");
                    // Thread was aborted, stop receiving
                    break;
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions here
                    int errorCode = Marshal.GetLastWin32Error();
                    //Console.WriteLine(ipAddress +" "+ port + " : Receive Timeout - WD Count " + MainWindow.watchDogCount + ex.Message);

                    //if(errorCode == 0)
                    
                    //Console.WriteLine("Error code: " + errorCode);
                    //Console.WriteLine("Error receiving UDP data: " + ex.Message + ipAddress + " " + port);
                }
            }
            Console.WriteLine("UdpClient Thread Finished " + srmNum);
        }

        private void OnUdpDataReceived(string message)
        {
            //byte[] data = Encoding.ASCII.GetBytes(message);
            //Console.WriteLine($"Received response from server: {message}");
            //udpClient.Send(data, data.Length, remoteEndPoint);
            // Parse Recieve From SRM Packet
            //SaveLogFile("SEND: " + dataString);
            SaveLogFile("RECV: " + message);
        }

        public void Close()
        {
            Console.WriteLine("UdpClient Class Close " + srmNum);
            udpClient.Close();
            sendTimer.Stop();

            if (receiveThread != null)
            {
                isRunning = false;
                receiveThread.Join();       // 순차적 종료 대기를 위함
                Console.WriteLine("UdpClient thread Finished Close " + srmNum);
                //receiveThread.Abort();
            }
        }

        //------------------------------------------------RECEIVE DATA PARSE FUNC------------------------------------------------------------
        private bool Rx_CommandParse(byte[] recv)
        {
            bool result = false;
            if(recv.Length < 18)
            {
                SaveLogFile("PacketError - Receive Length Failed");           // 패킷 길이가 맞지않음
                return false;
            }
            // Check Header----SYN---0x16 X 4 Check-------------------
            for (int i=0; i<4; i++)
            {
                if(recv[i] == 0x16) result = true;
                else result = false;
            }

            if (!result)
            {
                SaveLogFile("PacketError - Start Header Failed");             // SYN 헤더 형식이 맞지않음
                return false;
            }

            // Check ETX ------------------------------------------------------
            if(recv[recv.Length-1] != 0xF5)
            {
                SaveLogFile("PacketError - ETX Failed");                        // ETX 를 찾지못함
                return false;
            }

            // Check SRC Type ID ----------------------------------------------
            if ((recv[4] != 0x60) || (recv[5] != 0x01)) 
            {
                SaveLogFile("PacketError - SRC Device Type Failed");             // 장치 타입이 맞지않음 (지상반 통신 SRM TYPE 0x0060)
                return false;
            }

            // Check DST Type ID ----------------------------------------------
            if ((recv[6] != 0x00) || (recv[7] != 0x00))
            {
                SaveLogFile("PacketError - DST Device Type Failed");             // 장치 타입이 맞지않음 (지상반 통신 GCP TYPE 0x0000)
                return false;
            }

            // Check SeqNum ---------------------------------------------------
            if((int)recv[8] != seqNum)
            {
                SaveLogFile("PacketError - SeqNum Failed");             // 시퀀스 번호 에러
                return false;
            }

            // Check Receive Command ---------------------------------------------------
            if ((int)recv[14] != (gClass.str.SrmPacket[srmNum].sendStr.cmd2))    // 전송한 cmd2 가 맞는지 확인
            {
                SaveLogFile("PacketError - Receive Command2 Failed");             // 리시브 커맨드 에러
                return false;
            }

            // Check Receive Command ---------------------------------------------------
            if ((int)recv[11] != (gClass.str.SrmPacket[srmNum].sendStr.cmd1 + 0x80))    // 전송한 cmd1 + 0x80 이 맞는지 확인
            {
                SaveLogFile("PacketError - Receive Command1 Failed");             // 리시브 커맨드 에러
                return false;
            }

            // Check CRC ------------------------------------------------------
            byte[] packetData = new byte[recv.Length-7];                        // Receive Length - SYN, CRC, ETX
            Array.Copy(recv, 4, packetData, 0, packetData.Length);              // SYN CRC ETX 를 제외한 Recv 총 길이 = 계산길이
            ushort recvCrc = (ushort)((recv[recv.Length-3] << 8) | recv[recv.Length - 2]);      // CRC BigEndian   
            ushort calcCrc = crc16_ccitt(packetData);         // 수신데이터 작성데이터 crc16 계산

            if(recvCrc != calcCrc)
            {
                SaveLogFile("PacketError - CRC Data Failed");             // CRC 체크 에러
                return false;
            }

            // Get Parse Command 1 --------------------------------------------------
            gClass.str.SrmPacket[srmNum].recvStr.srcType = recv[4];
            gClass.str.SrmPacket[srmNum].recvStr.srcID = recv[5];
            gClass.str.SrmPacket[srmNum].recvStr.dstType = recv[6];
            gClass.str.SrmPacket[srmNum].recvStr.seqNum = recv[8];
            gClass.str.SrmPacket[srmNum].recvStr.byPass1 = recv[9];
            gClass.str.SrmPacket[srmNum].recvStr.byPass2 = recv[10];
            gClass.str.SrmPacket[srmNum].recvStr.cmd1 = recv[11];
            gClass.str.SrmPacket[srmNum].recvStr.len = BitConverter.ToUInt16(recv, 12);
            gClass.str.SrmPacket[srmNum].recvStr.cmd2 = recv[14];

            Console.WriteLine("recvLen : " + gClass.str.SrmPacket[srmNum].recvStr.len + " recvBuf " + recv[12] + " " + recv[13]);

            if (gClass.str.SrmPacket[srmNum].recvStr.len > 1)
            {
                gClass.str.SrmPacket[srmNum].recvStr.data = Enumerable.Repeat<byte>(0, gClass.str.SrmPacket[srmNum].recvStr.len - 1).ToArray<byte>();     // byte Array 메모리 할당
                Array.Copy(recv, 15, gClass.str.SrmPacket[srmNum].recvStr.data, 0, gClass.str.SrmPacket[srmNum].recvStr.len - 1);     // Data 바이트 파싱  len = Command2 ~ Data 까지 길이
            }

            // TX 보낼 때 보낼지.. to do   비동기식 처리 고민중
            //oldNum = (int)recv[8];
            //seqNum += 1;
            //if (seqNum > 255) seqNum = 0;

            //sendStr = cConstDefine.STX + sendStr + Convert.ToChar(crc);     // 보내는 데이터 
            //string asciiString = Encoding.ASCII.GetString(bytes);
            return true;
        }

        private bool Rx_DataParse(byte cmd1, byte cmd2)
        {
            bool result = false;
            switch (cmd2)
            {
                case 0x30:                      // 상태조회 0x30 
                    Console.WriteLine("상태조회 : " + gClass.srmNum);
                    result = Rx_RequestState();
                    break;
                case 0x41:                      // 반송지령 0x41
                    Console.WriteLine("반송지령 : " + gClass.srmNum);
                    result = Rx_RequestTransfer();
                    break;
                case 0x80:                      // 수동조작 0x80

                    break;
            }
            return result;
        }

        unsafe private bool Rx_RequestTransfer()          //  0x0040 반송지령
        {
            return true;
        }

        unsafe private bool Rx_RequestState()          //  0x0030 상태조회
        {

            if (gClass.str.SrmPacket[srmNum].recvStr.len >= cConstDefine.DATACOUNT_0X30)
            {
                //byte[] dataArray = new byte[gClass.str.SrmPacket[gClass.srmNum].recvStr.data.Length];
                // Array.Copy(gClass.str.SrmPacket[gClass.srmNum].recvStr.data
                ref byte [] dataArray = ref gClass.str.SrmPacket[srmNum].recvStr.data;    // 코드 가독성을 위해 ref 사용
                ref Srm_State refState = ref gClass.str.SrmState[srmNum];

                fixed (Srm_State* fixedDataPtr = &refState)
                {
                    refState.protocolVer = dataArray[2];        // Protocol Version

                    refState.firmwareVer = dataArray[3];        // Firmware Version - Ver
                    refState.buildYear = dataArray[4];          // Firmware Version - Year
                    refState.buildMonth = dataArray[5];         // Firmware Version - Month
                    refState.buildDay = dataArray[6];           // Firmware Version - Day

                    refState.utcTime = BitConverter.ToUInt32(dataArray, 7);     // System DateTime
                    Marshal.Copy(dataArray, 11, (IntPtr)fixedDataPtr, 6);       // Project No
                    refState.groupNo = dataArray[17];                           // Group No
                    refState.srmNo = BitConverter.ToUInt16(dataArray, 18);      // Srm No
                    refState.srmType = BitConverter.ToUInt16(dataArray, 20);    // Srm Type
                    refState.gcpRecvState.gcpMode = dataArray[22];              // Gcp Mode - Return
                    refState.gcpRecvState.safetyPlug = (dataArray[23] & 0x08) != 0;     // Gcp SafetyPlug    true:해제
                    refState.gcpRecvState.faultReset = (dataArray[23] & 0x02) != 0;     // Gcp FaultReset    true:눌림
                    refState.gcpRecvState.emStop = (dataArray[23] & 0x01) != 0;         // Gcp EmStop        true:눌림

                    for(int i = 0; i<8; i++)
                    {
                        refState.CVOK[i] = dataArray[24+i];                             // CVOK
                        refState.CVNO[i] = dataArray[32+i];                             // CVNO
                    }

                    refState.devMode = dataArray[40];                                   // Device Mode
                    refState.devState1 = dataArray[41];                                 // Dev State 1
                    refState.devState2 = dataArray[42];                                 // Dev State 2

                    refState.operCode = dataArray[43];                                  // Operation Code
                    refState.errcodeH = dataArray[44];                                  // ErrCode H
                    refState.errcodeM = dataArray[45];                                  // ErrCode M
                    refState.errcodeL = BitConverter.ToUInt16(dataArray, 46);           // ErrCode L

                    // Fork 1 Position Info
                    refState.fork1.curStation = dataArray[48];
                    refState.fork1.curBay = BitConverter.ToUInt16(dataArray, 49);
                    refState.fork1.curLev = dataArray[51];
                    refState.fork1.curPosNum = (sbyte)dataArray[52];
                    refState.fork1.curPos1 = dataArray[53];                             // 포크1 기준 주행/승강 정위치
                    refState.fork1.curPos2 = dataArray[54];

                    // Fork 2 Position Info
                    refState.fork2.curStation = dataArray[57];
                    refState.fork2.curBay = BitConverter.ToUInt16(dataArray, 58);
                    refState.fork2.curLev = dataArray[60];
                    refState.fork2.curPosNum = (sbyte)dataArray[61];
                    refState.fork2.curPos1 = dataArray[62];                             // 포크1 기준 주행/승강 정위치
                    refState.fork2.curPos2 = dataArray[63];

                    refState.fork1.targetStation = dataArray[66];
                    refState.fork1.targetRow = dataArray[67];
                    refState.fork1.targetLev = dataArray[68];
                    refState.fork1.targetBay = BitConverter.ToUInt16(dataArray, 69);       // 2

                    //reserved 2    71-72

                    refState.fork2.targetStation = dataArray[73];
                    refState.fork2.targetRow = dataArray[74];
                    refState.fork2.targetLev = dataArray[75];       
                    refState.fork2.targetBay = BitConverter.ToUInt16(dataArray, 76);      // 2

                    // reserved 2   77-78
                    // reserved 11  79-89
                    refState.trav.state1 = dataArray[90];
                    refState.trav.state2 = dataArray[91];
                    refState.trav.fwDecNo = dataArray[92];
                    refState.trav.bwDecNo = dataArray[93];
                    refState.trav.curPos = BitConverter.ToInt32(dataArray, 94); // 4
                    refState.trav.curSpd = BitConverter.ToInt16(dataArray, 98); // 2
                    refState.trav.targetPos = BitConverter.ToInt32(dataArray, 100); // 4

                    // reserved 2   104-105
                    refState.lift.state1 = dataArray[106];
                    refState.lift.state2 = dataArray[107];
                    refState.lift.upDecNo = dataArray[108];
                    refState.lift.dnDecNo = dataArray[109];
                    refState.lift.curPos = BitConverter.ToInt32(dataArray, 110); // 4
                    refState.lift.curSpd = BitConverter.ToInt16(dataArray, 114); // 2
                    refState.lift.targetPos = BitConverter.ToInt32(dataArray, 116); // 4

                    // reserved 2   120-121

                    refState.fork1.state1 = dataArray[122];
                    refState.fork1.state2 = dataArray[123];
                    refState.fork1.loadType = dataArray[124];

                    // reserved 1   125
                    refState.fork1.curPos = BitConverter.ToInt32(dataArray, 126); // 4
                    refState.fork1.curSpd = BitConverter.ToInt16(dataArray, 130); // 2
                    refState.fork1.targetPos = BitConverter.ToInt32(dataArray, 132); // 4

                    // reserved 2   136-137

                    refState.fork1.state1 = dataArray[138];
                    refState.fork1.state2 = dataArray[139];
                    refState.fork1.loadType = dataArray[140];

                    // reserved 1   141
                    refState.fork1.curPos = BitConverter.ToInt32(dataArray, 142); // 4
                    refState.fork1.curSpd = BitConverter.ToInt16(dataArray, 146); // 2
                    refState.fork1.targetPos = BitConverter.ToInt32(dataArray, 148); // 4

                    // reserved 2   152-153

                    // FORK1 작업정보 - 반송명령
                    refState.fork1.jobNo = BitConverter.ToUInt32(dataArray, 154); // 4
                    refState.fork1.taskIdx = dataArray[158];
                    refState.fork1.fromStation = dataArray[159];
                    refState.fork1.fromRow = dataArray[160];
                    refState.fork1.fromBay = BitConverter.ToUInt16(dataArray, 161); // 2
                    refState.fork1.fromLev = dataArray[163];
                    refState.fork1.toStation = dataArray[164];
                    refState.fork1.toRow = dataArray[165];
                    refState.fork1.toBay = BitConverter.ToUInt16(dataArray, 166); // 2
                    refState.fork1.toLev = dataArray[168];

                    refState.fork1.cmdCode = dataArray[169];
                    refState.fork1.procState = dataArray[170];
                    refState.fork1.procStep = dataArray[171];

                    // 작업정보 - 이동명령
                    refState.fork1.mvJobNo = BitConverter.ToUInt32(dataArray, 172); // 4
                    refState.fork1.mvToStation = dataArray[176];
                    refState.fork1.mvToRow = dataArray[177];
                    refState.fork1.mvToBay = BitConverter.ToUInt16(dataArray, 178); // 2
                    refState.fork1.mvToLev = dataArray[180];

                    refState.fork1.mvProcState = dataArray[181];
                    refState.fork1.mvProcStep = dataArray[182];


                    // FORK2 작업정보 - 반송명령
                    refState.fork2.jobNo = BitConverter.ToUInt32(dataArray, 183); // 4
                    refState.fork2.taskIdx = dataArray[187];
                    refState.fork2.fromStation = dataArray[188];
                    refState.fork2.fromRow = dataArray[189];
                    refState.fork2.fromBay = BitConverter.ToUInt16(dataArray, 190); // 2
                    refState.fork2.fromLev = dataArray[192];
                    refState.fork2.toStation = dataArray[193];
                    refState.fork2.toRow = dataArray[194];
                    refState.fork2.toBay = BitConverter.ToUInt16(dataArray, 195); // 2
                    refState.fork2.toLev = dataArray[197];

                    refState.fork2.cmdCode = dataArray[198];
                    refState.fork2.procState = dataArray[199];
                    refState.fork2.procStep = dataArray[200];

                    // 작업정보 - 이동명령
                    refState.fork2.mvJobNo = BitConverter.ToUInt32(dataArray, 201); // 4
                    refState.fork2.mvToStation = dataArray[205];
                    refState.fork2.mvToRow = dataArray[206];
                    refState.fork2.mvToBay = BitConverter.ToUInt16(dataArray, 207); // 2
                    refState.fork2.mvToLev = dataArray[209];

                    refState.fork2.mvProcState = dataArray[210];
                    refState.fork2.mvProcStep = dataArray[211];

                }

                //long utcTimeInSeconds = 1623586500; // Example UTC time in seconds
                //DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(utcTimeInSeconds);
                //DateTime dateTimeUtc = dateTimeOffset.UtcDateTime;
                //Console.WriteLine(dateTimeUtc);  // Output: 2021-06-13 10:55:00 AM
                return true;
            }
            else
            {
                SaveLogFile("PacketError - Receive Data Length Failed");             // 리시브 데이터 길이 에러
                return false;
            }
        }

        //------------------------------------------------SEND COMMAND FUNC--------------------------------------------------------------------------------------------------------


        // Send Command Main Func
        private void Send_Command()
        {
            //Console.WriteLine(ipAddress + " CurSeq : " + seqNum + "  RecvSeq : " + gClass.str.SrmPacket[gClass.srmNum].recvStr.seqNum);

            if (seqNum == gClass.str.SrmPacket[srmNum].recvStr.seqNum)           // 시퀀스 번호 동일할 경우 작업커맨드 완료처리 NO
            {
                // 시퀀스 번호 미 변경 시 최근 명령 재전송 할지  to do 
                Tx_SendCmd(0x00, 0x30);
                //Console.WriteLine("Tx Retry Command - " + ipAddress + " " + port);
            }
            else
            {
                if (gClass.str.SrmPacket[srmNum].sendStr.cmd2 != 0x30)           // 상태조회 명령이 아닐경우 응답코드 확인 후 완료 처리
                {
                    // 상태조회 명령 전송
                    Tx_SendCmd(0x00, 0x30);
                }
                else
                {
                    switch (gClass.str.SrmState[srmNum].devMode)
                    {
                        case 64:    //  기상반 수동
                            break;
                        case 128:   //  기상반 자동
                            break;
                        case 32:    //  기상반 강제모드
                            break;
                        case 16:    //  기상반 셋업모드
                            break;
                        default:
                            break;
                    }

                    // to do
                    if (gClass.str.SrmPacket[srmNum].manClicked)
                    {
                        Tx_SendCmd(0x00, 0x80);
                    }
                    else
                    {
                        Tx_SendCmd(0x00, 0x30);
                    }


                    // 수동 - 작업 명령 등 기타 커맨드 요청 상태에 따라 없으면 상태조회
                }
            }
        }


        private byte[] Tx_SetHeader(byte cmd1)
        {
            byte[] test = new byte[12];
            test[0] = 0x16;
            test[1] = 0x16;
            test[2] = 0x16;
            test[3] = 0x16;

            test[4] = 0x00;     // SRC ID - TYPE    00:지상반
            test[5] = 0x00;     // SRC ID - INDEX
            test[6] = 0x60;     // DST ID - TYPE    60:SRM    
            test[7] = 0x01;     // DST ID - INDEX   00:SRM 내 INDEX ex)호기번호

            test[8] = Convert.ToByte(seqNum);     // Sequence Num 
            //test[8] = 78;     // Sequence Num 

            test[9] = 0x00;     // Bypass1
            test[10] = 0x00;    // Bypass2
            test[11] = cmd1;    // CMD1

            return test;
        }

        private void Tx_SendCmd(byte cmd1, byte cmd2)      // 명령 전송
        {
            List<byte[]> txData = new List<byte[]>();
            txData.Clear();
            txData.Add(Tx_SetHeader(cmd1));                                         // Header 설정
            if (Tx_SetData(ref txData, cmd2))                                       // Data 설정
            {
                byte[] byteArray = txData.SelectMany(bytes => bytes).ToArray();     // Byte Lists Merge 1 Byte List
                //Console.WriteLine("Print TxData : " + byteArray.ToString());


                // Append CRC ------------------------------------------------------
                byte[] packetData = new byte[byteArray.Length - 4];                        // Total Length - SYN
                Array.Copy(byteArray, 4, packetData, 0, packetData.Length);             // SYN CRC ETX 를 제외한 Recv 총 길이 = 계산길이 
                ushort calcCrc = crc16_ccitt(packetData);                               // 송신데이터 crc16 계산

                Array.Resize(ref byteArray, byteArray.Length + 3);                      // Append CRC + ETX
                // CRC BigEndian
                byteArray[byteArray.Length - 3] = (byte)(calcCrc >> 8);
                byteArray[byteArray.Length - 2] = (byte)calcCrc;
                byteArray[byteArray.Length - 1] = 0xF5;

                // Send To SRM
                Send(byteArray);
            }
            txData.Clear();
        }

        private bool Tx_SetData(ref List<byte[]> txData, byte cmd2)
        {
            ushort length;
            byte[] data = {};
            byte[] len = new byte[3];

            switch (cmd2)
            {
                case 0x30:
                    CMD_Req_State(ref data);        //  0x0030  SRM 상태조회
                    break;
                case 0x31:
                    CMD_Req_DriveInfo(ref data);    //  0x0031  운행정보 요구
                    break;
                case 0x32:
                    CMD_Req_InvStateInfo(ref data); //  0x0032  인버터정보 요구
                    break;
                case 0x34:
                    CMD_Req_AlarmLog(ref data);     //  0x0034  알람로그
                    break;
                case 0x50:                          //  0x0050  시작
                    Array.Resize(ref data, 1);
                    data[0] = 0x00;     //  시작 On/Off   0:Off, 1:On
                    break;
                case 0x53:                          //  작업삭제
                    Array.Resize(ref data, 1);
                    data[0] = 0x00;     //  삭제Flag  Bit5~7 대기작업모두삭제,Fork2작업삭제, Fork1작업삭제
                    break;
                case 0x51:                          //  0x0051  홈복귀
                case 0x52:                          //  0x0052  이상리셋
                case 0x54:                          //  0x0054  정지
                case 0x55:                          //  0x0055  비상정지
                case 0x56:                          //  0x0056  일시정지
                case 0x57:                          //  0x0057  복구
                case 0x59:                          //  0x0059  보수위치 이동
                    break;
                case 0x80:                          //  0x0080  수동 조작명령
                    CMD_Req_ManualOperation(ref data);
                    break;
            }

            //Console.WriteLine("Tx_SetData data byteSize : " + data.Length);
            length = (ushort)data.Length;
            length += 1;    // CMD2 Length Add
            Buffer.BlockCopy(BitConverter.GetBytes(length), 0, len, 0, sizeof(ushort));     // Length - Command2 ~ Data
            len[2] = cmd2;

            txData.Add(len);
            txData.Add(data);

            return true;
        }

        #region 0x30~0x34 상태정보 ~ 로그정보 요구

        private void CMD_Req_State(ref byte[] data)      // 0x0030  상태조회
        {
            Array.Resize(ref data, data.Length + 28);
            data[0] = 0x01;     //  지상반 정보 유효/무효 - 0x01 : 유효
            //----------------------UTC Time Setup------------------------------
            DateTimeOffset utcTime = DateTimeOffset.UtcNow; // get the current UTC time offset
            int utcSeconds = (int)utcTime.ToUnixTimeSeconds(); // convert to seconds since Unix epoch
            byte[] bytes = BitConverter.GetBytes(utcSeconds);
            Buffer.BlockCopy(bytes, 0, data, 1, bytes.Length);     // 지상반 PC 시간 = UTC Time 4Byte

            data[5] = 0x01;     //  지상반 모드  1:수동, 2:반자동, 3:자동
            data[6] = 0x00;     //  지상반 상태  8Bit to do

            data[7] = 0x00;     //  인터록(CV->장비) to do
            data[8] = 0x00;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x00;

            data[15] = 0x00;     //  인터록(장비->CV) to do
            data[16] = 0x00;
            data[17] = 0x00;
            data[18] = 0x00;
            data[19] = 0x00;
            data[20] = 0x00;
            data[21] = 0x00;
            data[22] = 0x00;

            data[23] = 0x00;     //  reserved
            data[24] = 0x00;
            data[25] = 0x00;
            data[26] = 0x00;
            data[27] = 0x00;

            // if (BitConverter.IsLittleEndian)
            //    Array.Reverse(byteArray);
        }

        private void CMD_Req_DriveInfo(ref byte[] data)      // 0x0031  운행정보 요구
        {
            Array.Resize(ref data, data.Length + 20);
            data[0] = 0x00;     //  reserved
            data[1] = 0x00;
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x00;
            data[6] = 0x00;
            data[7] = 0x00;
            data[8] = 0x00;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x00;
            data[15] = 0x00;
            data[16] = 0x00;
            data[17] = 0x00;
            data[18] = 0x00;
            data[19] = 0x00;
        }

        private void CMD_Req_InvStateInfo(ref byte[] data)      // 0x0032  인버터 상태 요구
        {
            Array.Resize(ref data, data.Length + 20);
            data[0] = 0x00;     //  reserved
            data[1] = 0x00;
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x00;
            data[6] = 0x00;
            data[7] = 0x00;
            data[8] = 0x00;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x00;
            data[15] = 0x00;
            data[16] = 0x00;
            data[17] = 0x00;
            data[18] = 0x00;
            data[19] = 0x00;
        }

        private void CMD_Req_AlarmLog(ref byte[] data)      // 0x0034  알람로그 조회
        {
            Array.Resize(ref data, data.Length + 12);
            data[0] = 0x00;     //  로그종류    0:알람로그, 1:이벤트로그
            data[1] = 0x00;     //  로그요청 Type

            ushort logCount = 0;
            byte[] bytes = BitConverter.GetBytes(logCount);
            Buffer.BlockCopy(bytes, 0, data, 2, bytes.Length);     // 지상반 PC 시간 = UTC Time 4Byte

            
            data[4] = 0x00;     //  로그요청 시작시간
            data[5] = 0x00;     //  로그요청 종료시간
            data[6] = 0x00;
            data[7] = 0x00;
            data[8] = 0x00;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
        }

        #endregion
        private void CMD_Req_Operation(ref byte[] data)      //
        {
            Array.Resize(ref data, data.Length + 1);
            data[0] = 0x00;     //  로그종류    0:알람로그, 1:이벤트로그
            data[1] = 0x00;     //  로그요청 Type
        }


        #region 0x80 수동 조작
        private void CMD_Req_ManualOperation(ref byte[] data)      //
        {
            Array.Resize(ref data, data.Length + 18);

            data[0] = (byte)gClass.str.SrmPacket[srmNum].manAxis;     //  제어 flag 2
            data[1] = 0x00;     //  제어 flag 2
            data[2] = gClass.str.SrmPacket[srmNum].manTrav;     //  주행
            data[3] = gClass.str.SrmPacket[srmNum].manLift;     //  승강
            data[4] = gClass.str.SrmPacket[srmNum].manFork1;     //  포크1
            data[5] = gClass.str.SrmPacket[srmNum].manFork2;     //  포크2
            data[6] = 0x00;     //  reserved 1
            data[7] = 0x00;     //  reserved 2
            data[8] = 0x00;     //  reserved 3
            data[9] = 0x00;     //  reserved 4
            data[10] = 0x00;    //  reserved 5
            data[11] = 0x00;    //  reserved 6
            data[12] = 0x00;    //  reserved 7
            data[13] = 0x00;    //  reserved 8
            data[14] = 0x00;    //  reserved 9
            data[15] = 0x00;    //  reserved 10
            data[16] = (byte)gClass.str.SrmInfo[srmNum].forkType;    //  포크 정위치 기준
            data[17] = 0x00;    //  저속 정위치 기준     1:포크1좌  2:포크1우  3:포크2좌  4:포크2우 
        }

        #endregion
    }
}
