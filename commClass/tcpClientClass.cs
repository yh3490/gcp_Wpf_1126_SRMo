using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

namespace gcp_Wpf.commClass
{
    internal class tcpClientClass
    {
        private TcpClient client;
        private string ipAddress;
        private int port;

        bool bConnected = false;

        bool connecting = false;
        private bool Connected { get => client == null ? false : client.Connected && bConnected; }

        NetworkStream clientStream;

        System.Timers.Timer connTimer = new System.Timers.Timer();
        public tcpClientClass(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;

            connTimer.Interval = 1000; // 1 second
            connTimer.AutoReset = true; // Repeat the timer
            connTimer.Elapsed += connTimer_Elapsed;
            connTimer.Start();

        }

        private void connTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Try Client Connect");
            if(client != null && bConnected)
            {
                // MC Example
                //byte[] buffer = { 0x50, 0x00, 0x00, 0x00, 0x0C, 0x00, 0xFF, 0xFF, 0x03, 0x01, 0x0A, 0x01 };

                byte[] buffer = System.Text.Encoding.ASCII.GetBytes("test");
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }
            else
            {
                if (connecting) return;
                client_Connect();
            }
        }

        private void client_Connect()
        {
            if (client != null && Connected /*mTcpClient.Connected*/) return; //연결이 되어 있는데 또 연결 방지용, 서버쪽에 연결된 노드가 늘어남
            try
            {
                connecting = true;
                Console.WriteLine("client_Connect start");
                client = new TcpClient();
                client.BeginConnect(ipAddress, port, onCompleteConnect, client);
            }
            catch (Exception exc)
            {
                bConnected = false;
                Console.WriteLine("client_Connect " + exc.Message);
            }
        }

        void onCompleteConnect(IAsyncResult iar)
        {
            try
            {
                //iar.AsyncWaitHandle.WaitOne(1000, false);
                //tcpc = (TcpClient)iar.AsyncState;
                Console.WriteLine("end Connect");
                client.EndConnect(iar);
                Console.WriteLine("success to connect ");
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
                bConnected = true; //연결 성공
                //mRx = new byte[512];
                //tcpc.GetStream().BeginRead(mRx, 0, mRx.Length, onCompleteReadFromServerStream, tcpc);
            }
            catch (Exception exc)
            {
                connecting = false;
                Console.WriteLine("onCompleteConnect " + exc.Message);
                bConnected = false; //연결 실패

            }
        }


        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    // Blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    // A socket error has occurred
                    break;
                }

                if (bytesRead == 0)
                {
                    // The client has disconnected from the server
                    break;
                }

                // Convert the message bytes to a string and display it.
                string data = System.Text.Encoding.ASCII.GetString(message, 0, bytesRead);
                Console.WriteLine(string.Format("Received: {0} - {1}", data, port));

                // Echo the message back to the client.
                //byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
                //clientStream.Write(buffer, 0, buffer.Length);
                //clientStream.Flush();
            }
            connecting = false;
            tcpClient.Close();
            bConnected = false;
            Console.WriteLine("Client disconnected.");
        }

        public void Close()
        {
            Console.WriteLine("TcpClient Class Close");
        }

    }
}
