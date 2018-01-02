using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/*
 * static class - zaznamena zmenu dotyku a vytvori event
 * server samostatny skript
 * slidre nezavisle na zbytku
 * 
 * 
 */


[System.Serializable]
public class Sliders
{
    public Slider sliderThumb;
    public Slider sliderIndex;
    public Slider sliderMiddle;
    public Slider sliderRing;
    public Slider sliderPinky;
}

[System.Serializable]
public class ServerInfo
{
    public GameObject IPAddressText;
}

public class CollisionTouch : MonoBehaviour
{
    [Tooltip("Sliders for left hand fingers")]
    public Sliders leftHandSliders;
    [Tooltip("Sliders for right hand fingers")]
    public Sliders rightHandSliders;
    [Tooltip("Container object where objects with InteractionBehaviour script will be placed")]
    public GameObject stage;
    [Tooltip("GUI elements where information about server will be shown")]
    public ServerInfo serverInfo;


    void Start()
    {
        SocketServer.Stop();
        Thread.Sleep(100);
        SocketServer.Initialize(SocketServer.GetLocalIP(), 7999, leftHandSliders, rightHandSliders, serverInfo);
        SocketServer.Run();
    }

    public void UpdateFinger(ContactBone.FingerType fingerType, bool isLeftHand, bool touching)
    {
        var sliders = isLeftHand ? leftHandSliders : rightHandSliders;
        int value = touching ? 100 : 0;
        switch (fingerType)
        {
            case (ContactBone.FingerType.ThumbDistal):
                sliders.sliderThumb.value = value;
                break;
            case (ContactBone.FingerType.IndexDistal):
                sliders.sliderIndex.value = value;
                break;
            case (ContactBone.FingerType.MidleDistal):
                sliders.sliderMiddle.value = value;
                break;
            case (ContactBone.FingerType.RingDistal):
                sliders.sliderRing.value = value;
                break;
            case (ContactBone.FingerType.PinkyDistal):
                sliders.sliderPinky.value = value;
                break;
            default:
                Debug.LogError("UpdateFinger: fingerType=" + fingerType + ", isLeftHand=" + isLeftHand + ", touching=" + touching);
                break;
        }
    }

}

/////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////

public class SocketServer
{
    private static IPAddress localIP;
    private static int port;
    private static TcpListener server;
    private static Thread serverThread;
    private static Thread listenThread;

    private static Sliders leftHandFingers;
    private static Sliders rightHandFingers;

    private static ServerInfo serverInfo;

    private static bool _running;
    public static bool Running { get
        {
            return _running;
        }
    }


    public static void Initialize(string ip, int in_port, Sliders lFingers, Sliders rFingers, ServerInfo sInfo)
    {
        Stop();
        localIP = IPAddress.Parse(ip);
        port = in_port;
        server = null;
        _running = false;
        leftHandFingers = lFingers;
        rightHandFingers = rFingers;
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
        Dispatcher.RunOnMainThread(() => serverInfo.IPAddressText.GetComponent<TextMesh>().text = (localIP.ToString() + ":" + port.ToString()));

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
            Debug.Log(ex.Message);
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
                handler.Start();
            }
        }
        catch (Exception ex)
        {
            // SocketException
            // Usually throws exception if thread is aborted when waiting for client
            Debug.Log(ex.Message);
            Restart();
        }
    }

    private static void HandleClient(TcpClient client)
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

                control_str.Append(leftHandFingers.sliderThumb.value + ",");
                control_str.Append(leftHandFingers.sliderIndex.value + ",");
                control_str.Append(leftHandFingers.sliderMiddle.value + ",");
                control_str.Append(leftHandFingers.sliderRing.value + ",");
                control_str.Append(leftHandFingers.sliderPinky.value);
                control_str.Append("xy");

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
            }
        }
        catch (Exception ex)
        {
            // System.IO.IOException
            // Usually throws exception when client disconnects during transmit
            Debug.Log(ex.Message);
        }

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

}