using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace gcp_Wpf.commClass
{
    internal class tcpServerClass
    {
        private TcpListener tcpListener;
        private CancellationTokenSource cts;

        private CancellationTokenSource cReads;
        private Thread clientThread = null;
        private Thread listenThread = null;
        private int port;
        private int srmNum;
        public bool isRunning;
        private int connectCnt = 0;

        //Singletone
        singletonClass gClass;

        public tcpServerClass(int srmNum, int port)
        {
            gClass = singletonClass.Instance;
            this.srmNum = srmNum;
            this.port = port;
            StartServer(port);

        }
        private void StartServer(int port)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                listenThread = new Thread(new ThreadStart(ListenForClients));
                isRunning = true;
                listenThread.Start();
                Console.WriteLine("Server started.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private async void ListenForClients()
        {
            Console.WriteLine("Listening for clients...");
            cts = new CancellationTokenSource();

            try
            {
                tcpListener.Start();
                Console.WriteLine($"Listening on port {port}...");

                while (!cts.IsCancellationRequested)
                {
                    Console.WriteLine("Listening for Wait...");
                    var client = await tcpListener.AcceptTcpClientAsync().WithCancellation(cts.Token);
                    //TcpClient client = tcpListener.AcceptTcpClient();
                    Console.WriteLine("Client connected.");
                    if (clientThread == null)
                    {
                        clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                        clientThread.Start(client);
                    }
                    else
                    {
                        Console.WriteLine("Client Already connected.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Listening canceled." + srmNum);
            }
            finally
            {
                Console.WriteLine("Listening finally." + srmNum);
                tcpListener.Stop();
            }
        }

        private async void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            cReads = new CancellationTokenSource();
            byte[] message = new byte[4096];
            int bytesRead;
            try
            {

                while (!cReads.IsCancellationRequested)
                {
                    bytesRead = 0;
                    // Blocks until a client sends a message
                    bytesRead = await clientStream.ReadAsync(message, 0, 4096).WithCancellation(cts.Token);
                    // bytesRead = clientStream.Read(message, 0, 4096);

                    if (bytesRead == 0)
                    {
                        // The client has disconnected from the server
                        break;
                    }

                    // Convert the message bytes to a string and display it.
                    string data = System.Text.Encoding.ASCII.GetString(message, 0, bytesRead);
                    Console.WriteLine(string.Format("Received: {0} - {1}", data, port));

                    // Echo the message back to the client.
                    byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                }

                tcpClient.Close();
                Console.WriteLine("Client disconnected." + srmNum);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Read Waitfor canceled." + srmNum);
            }
            finally
            {
                Console.WriteLine("Read finally." + srmNum );
                //tcpListener.Stop();
                //cReads.Dispose();
            }
        }


        //private void WriteLog(string message)
        //{
        //    // Ensure thread-safe access to the control
        //    if (listBox1.InvokeRequired)
        //    {
        //        listBox1.Invoke(new Action<string>(WriteLog), message);
        //    }
        //    else
        //    {
        //        listBox1.Items.Add(message);
        //    }
        //}

        public void Close()
        {
            Console.WriteLine("TcpServer Class Close  " + srmNum);
            if (clientThread != null)
            {
                cReads.Cancel();
                Console.WriteLine("TcpServer thread Read Close " + srmNum);
                clientThread.Join();
            }
            if (listenThread != null)
            {

                cts.Cancel();
                Console.WriteLine("TcpServer thread Listen Close " + listenThread + " " + srmNum);
                listenThread.Join();
                Console.WriteLine("TcpServer thread Listen Close after join " + listenThread + " " + srmNum);
            }
        }
    }
    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => tcs.TrySetResult(true)))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }
            return await task;
        }
    }

}
