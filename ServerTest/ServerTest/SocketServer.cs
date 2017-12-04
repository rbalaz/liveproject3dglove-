using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ServerTest
{
    public class SocketServer
    {
        private IPAddress localIP;
        private int port;
        private TcpListener server;
        private Thread listenThread;

        private MainWindow window;

        public bool Running { get; set; }

        public SocketServer(string ip, int in_port, MainWindow w)
        {
            localIP = IPAddress.Parse(ip);
            port = in_port;
            window = w;
            server = null;
            Running = false;
            listenThread = null;
        }

        public void Run()
        {
            Debug.WriteLine("Server: Starting at " + localIP + ":" + port);
            // Start server
            server = new TcpListener(localIP, port);
            server.Start();

            Debug.WriteLine("Server: Waiting for client to connect...");
            // Start listening for clients
            listenThread = new Thread(new ThreadStart(Listen));
            listenThread.IsBackground = true;
            listenThread.Start();

            Thread.Sleep(100);
            Running = true;
        }

        private void Listen()
        {
            try
            {
                // Listen for clients
                while (Thread.CurrentThread.IsAlive)
                {
                    // Wait for new client to connect (blocking function)
                    TcpClient client = server.AcceptTcpClient();

                    Debug.WriteLine("Server: Client connected with IP " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                    // Handle client in new thread
                    Thread handler = new Thread(() => HandleClient(client));
                    handler.Start();
                }
            }
            catch (Exception ex)
            {
                // SocketException
                // Usually throws exception if thread is aborted when waiting for client
                Debug.WriteLine(ex.Message);
                Stop();
            }
        }

        private void HandleClient(TcpClient client)
        {
            // Get a stream object
            NetworkStream stream = client.GetStream();

            try
            {
                string lastData = "";
                while (true)
                {
                    // Create control string
                    StringBuilder control_str = new StringBuilder();

                    for (int i = 0; i < 5; i++)
                    {
                        string val = null;
                        window.Dispatcher.Invoke(new Action(() => val = window.rightHandFingers[i].Text));
                        control_str.Append(val + ",");
                    }

                    control_str[control_str.Length - 1] = 'x';
                    control_str.Append('y');

                    string newData = control_str.ToString();
                    
                    //if (newData != lastData)
                    //{
                    // Send data
                    byte[] msg = Encoding.ASCII.GetBytes(newData);
                    stream.Write(msg, 0, msg.Length);
                    Debug.WriteLine(newData);
                    // Save
                    lastData = newData;
                    //}

                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                // System.IO.IOException
                // Usually throws exception when client disconnects during transmit
                Debug.WriteLine(ex.Message);
            }

            // End connection
            stream.Close();
            client.Close();
        }

        public void Stop()
        {
            if (Running)
            {
                if (server != null)
                {
                    Running = false;
                    server.Stop(); // Stop listening
                }
            }
        }

        public static string GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            Debug.WriteLine("Server: Cannot find IP address");
            return null;
        }

    }
}
