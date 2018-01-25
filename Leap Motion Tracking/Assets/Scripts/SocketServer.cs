using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class ServerInfo
{
    public GameObject IPAddressText;
    public GameObject leftHandConnectionColor;
    public GameObject rightHandConnectionColor;
    public GameObject leftHandConnectionText;
    public GameObject rightHandConnectionText;
    public Material connectedMaterial;
    public Material disconnectedMaterial;
}

public class SocketServer : MonoBehaviour {
    public ServerInfo serverInfo;

    void Start () {
        if (!ServerClass.Running)
        {
            ServerClass.Stop();
            Thread.Sleep(100);
            ServerClass.Initialize(ServerClass.GetLocalIP(), 7999, serverInfo);
            ServerClass.Run();
        }
        else
        {
            ServerClass.serverInfo = serverInfo;
            ServerClass.UpdateStatus();
        }
        
    }

    void OnApplicationQuit()
    {
        ServerClass.Stop();
    }

}

public class ServerClass : MonoBehaviour
{
    private static IPAddress localIP;
    private static int port;
    private static TcpListener server;
    private static Thread serverThread;
    private static Thread listenThread;

    public static ServerInfo serverInfo;

    private static bool rightHandConnected;
    private static bool leftHandConnected;
    private static bool _running;
    public static bool Running
    {
        get
        {
            return _running;
        }
    }

    private static ServerClass _instance;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = new GameObject("ServerClass").AddComponent<ServerClass>();
            DontDestroyOnLoad(_instance.gameObject);
        }
    }

    public static void Initialize(string ip, int in_port, ServerInfo sInfo)
    {
        Stop();
        localIP = IPAddress.Parse(ip);
        port = in_port;
        server = null;
        _running = false;
        listenThread = null;
        serverThread = null;
        serverInfo = sInfo;
    }

    public static void Run()
    {
        Stop();
        serverThread = new Thread(new ThreadStart(RunServerThread));
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    private static void RunServerThread()
    {
        Debug.Log("Server: Starting at " + localIP + ":" + port);
        UpdateStatus();

        try
        {
            // Start server
            server = new TcpListener(localIP, port);
            server.Start();

            Debug.Log("Server: Waiting for client to connect...");
            // Start listening for clients
            listenThread = new Thread(new ThreadStart(Listen));
            listenThread.IsBackground = true;
            listenThread.Start();

            Thread.Sleep(100);
            _running = true;

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Restart();
        }
    }

    private static void Listen()
    {
        try
        {
            // Listen for clients
            while (Thread.CurrentThread.IsAlive)
            {
                // Wait for new client to connect (blocking function)
                TcpClient client = server.AcceptTcpClient();

                Debug.Log("Server: Client connected with IP " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                // Handle client in new thread
                Thread handler = new Thread(() => HandleClient(client));
                handler.IsBackground = true;
                handler.Start();
            }
        }
        catch (Exception ex)
        {
            // SocketException
            // Usually throws exception if thread is aborted when waiting for client
            Debug.LogException(ex);
            Restart();
        }
    }

    private static void HandleClient(TcpClient client)
    {
        // Get a stream object
        NetworkStream stream = client.GetStream();
        TouchFinger[] fingerData = null;
        bool isLeftHand = false;

        try
        {
            byte[] incMsg = new byte[1024];
            int length;
            if ((length = stream.Read(incMsg, 0, incMsg.Length)) == 0) ; // Wait for message
            string incoming = Encoding.ASCII.GetString(incMsg, 0, length);
            if (incoming == "l" || ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString() == "192.168.0.101")
            {
                Debug.Log("Sending LEFT hand data to client " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                
                isLeftHand = true;
                leftHandConnected = true;
            }
            else
            {
                Debug.Log("Sending RIGHT hand data to client " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                
                isLeftHand = false;
                rightHandConnected = true;
            }
            UpdateStatus();

            string lastData = "";
            while (true)
            {
                if (isLeftHand)
                {
                    fingerData = TouchDetector.touchFingersLeft;
                }
                else
                {
                    fingerData = TouchDetector.touchFingersRight;
                }

                // Create control string
                StringBuilder control_str = new StringBuilder();

                for (int f = 0; f < fingerData.Length; f++)
                {
                    var force = fingerData[f].Force; 
                    control_str.Append(Math.Floor(180 - (force * (120 - 60) / (100 - 0) + 60))); // <0-100> => <120-60>

                    if (f == fingerData.Length-1)
                        control_str.Append("xy");
                    else
                        control_str.Append(",");
                }
                Debug.Log(control_str);

                string newData = control_str.ToString();
                if (newData != lastData)
                {
                    // Send data
                    byte[] msg = Encoding.ASCII.GetBytes(newData);
                    stream.Write(msg, 0, msg.Length);
                    //Debug.Log(newData);

                    // Save
                    lastData = newData;
                }

                Thread.Sleep(50);

                if (isLeftHand)
                    leftHandConnected = true;
                else
                    rightHandConnected = true;

                UpdateStatus();
            }
        }
        catch (Exception ex)
        {
            // System.IO.IOException
            // Usually throws exception when client disconnects during transmit
            Debug.LogException(ex);
            if (isLeftHand)
                leftHandConnected = false;
            else
                rightHandConnected = false;

            UpdateStatus();
        }
        if (isLeftHand)
            leftHandConnected = false;
        else
            rightHandConnected = false;

        UpdateStatus();

        // End connection
        stream.Close();
        client.Close();
    }

    public static void Stop()
    {
        if (Running)
        {
            if (server != null)
            {
                _running = false;
                server.Stop(); // Stop listening
            }
        }
        if (serverThread != null && serverThread.IsAlive)
        {
            serverThread.Abort();
        }
        if (listenThread != null && listenThread.IsAlive)
        {
            listenThread.Abort();
        }
    }

    public static void Restart()
    {
        Stop();
        Thread.Sleep(1000);
        Run();
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
        Debug.LogError("Server: Cannot find IP address");
        return null;
    }

    public static void UpdateStatus()
    {
        // IP display
        Dispatcher.RunOnMainThread(() => serverInfo.IPAddressText.GetComponent<TextMesh>().text = (localIP.ToString() + ":" + port.ToString()));

        // Get correct strings and materials
        string leftMessage = leftHandConnected ? "Connected" : "Not connected";
        Material leftMat = leftHandConnected ? serverInfo.connectedMaterial : serverInfo.disconnectedMaterial;
        string rightMessage = rightHandConnected ? "Connected" : "Not connected";
        Material rightMat = rightHandConnected ? serverInfo.connectedMaterial : serverInfo.disconnectedMaterial;

        // Update display for both hands
        Dispatcher.RunOnMainThread(() => serverInfo.leftHandConnectionText.GetComponent<TextMesh>().text = leftMessage);
        Dispatcher.RunOnMainThread(() => serverInfo.leftHandConnectionColor.GetComponent<Renderer>().material.color = leftMat.color);

        Dispatcher.RunOnMainThread(() => serverInfo.rightHandConnectionText.GetComponent<TextMesh>().text = rightMessage);
        Dispatcher.RunOnMainThread(() => serverInfo.rightHandConnectionColor.GetComponent<Renderer>().material.color = rightMat.color);
    }

}
