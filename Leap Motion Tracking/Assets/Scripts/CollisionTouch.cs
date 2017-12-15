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
    [Tooltip("Distance threshold to decide whether finger is touching the object")]
    public float touchDistance;
    [Tooltip("Sliders for left hand fingers")]
    public Sliders leftHandSliders;
    [Tooltip("Sliders for right hand fingers")]
    public Sliders rightHandSliders;
    [Tooltip("Container object where objects with InteractionBehaviour script will be placed")]
    public GameObject stage;
    [Tooltip("GUI elements where information about server will be shown")]
    public ServerInfo serverInfo;

    private SocketServer server;

    void Start()
    {
        // !! PRI RESTARTE SCENY JE CHYBA S PORTOM
        server = new SocketServer(SocketServer.GetLocalIP(), 7999, leftHandSliders, rightHandSliders, serverInfo);
		
		Debug.Log("Server: Starting at " + SocketServer.GetLocalIP() + ":7999");
        serverInfo.IPAddressText.GetComponent<TextMesh>().text = (SocketServer.GetLocalIP().ToString() + ":7999");
		
		Thread serverThread = new Thread(new ThreadStart(server.Run));
		serverThread.IsBackground = true;
		serverThread.Start();
		
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
    private IPAddress localIP;
    private int port;
    private TcpListener server;
    private Thread listenThread;

    private Sliders leftHandFingers;
    private Sliders rightHandFingers;

    private ServerInfo serverInfo;

    public bool Running { get; set; }

    public SocketServer(string ip, int in_port, Sliders lFingers, Sliders rFingers, ServerInfo sInfo)
    {
        localIP = IPAddress.Parse(ip);
        port = in_port;
        server = null;
        Running = false;
        leftHandFingers = lFingers;
        rightHandFingers = rFingers;
        listenThread = null;
        serverInfo = sInfo;
    }

    public void Run()
    {
        Debug.Log("Server: Starting at " + localIP + ":" + port);
        serverInfo.IPAddressText.GetComponent<TextMesh>().text = (localIP.ToString() + ":" + port.ToString());
        try{
			// Start server
			server = new TcpListener(localIP, port);
			server.Start();
		
			Debug.Log("Server: Waiting for client to connect...");
			// Start listening for clients
			listenThread = new Thread(new ThreadStart(Listen));
			listenThread.IsBackground = true;
			listenThread.Start();

			Thread.Sleep(100);
			Running = true;
		
		}
		catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Restart();
        }
		
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
	
	public void Restart()
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