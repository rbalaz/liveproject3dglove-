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

public class Finger
{
    public GameObject obj;
    public List<InteractibleObject> nearObjects;
    public Slider slider;
    public string nameShort;
    public float minDistance;
    public Collider collider;

    private float touchDistance;


    public Finger(GameObject f, Sliders sl, float td)
    {
        obj = f;
        collider = obj.GetComponent<Collider>();
        minDistance = float.PositiveInfinity;
        touchDistance = td;
        nearObjects = new List<InteractibleObject>();
        FindSlider(sl);
    }

    // Assign slider to finger
    private void FindSlider(Sliders sl)
    {
        if (obj.name.Contains("Thumb"))
        {
            nameShort = "T";
            slider = sl.sliderThumb;
        }
        if (obj.name.Contains("Index"))
        {
            nameShort = "I";
            slider = sl.sliderIndex;
        }
        if (obj.name.Contains("Middle"))
        {
            nameShort = "M";
            slider = sl.sliderMiddle;
        }
        if (obj.name.Contains("Ring"))
        {
            nameShort = "R";
            slider = sl.sliderRing;
        }
        if (obj.name.Contains("Pinky"))
        {
            nameShort = "P";
            slider = sl.sliderPinky;
        }
    }
}

public class InteractibleObject
{
    public GameObject gameObject;
    public Collider collider; // Collider of interactible objects (stored for performance reasons)
    public InteractionBehaviour interaction;

    public InteractibleObject(CollisionTouch manager, InteractionBehaviour ib)
    {
        if (manager != null && ib != null)
        {
            interaction = ib;
            // Register events
            ib.OnHoverEnd += (() => manager.UpdateOnHoverEnd(this));
            ib.OnHoverBegin += (() => manager.UpdateOnHoverBegin(this));
            // Store gameObject and collider
            gameObject = ib.gameObject;
            collider = (ib.gameObject.GetComponent<Collider>());
        }
    }
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

    // List of interactible objects
    private List<InteractibleObject> ilList;

    // Lists of left and right hand fingers
    private List<Finger> leftHandFingers;
    private List<Finger> rightHandFingers;

    // Constants used for finding fingers in the scene
    private int BONES_COUNT = 5;
    private string BASE_PATH_LEFT = "/LMHeadMountedRig/Interaction Manager/Left Interaction Hand Contact Bones/Contact Fingerbone ";
    private string BASE_PATH_RIGHT = "/LMHeadMountedRig/Interaction Manager/Right Interaction Hand Contact Bones/Contact Fingerbone ";
    private string[] BONE_NAMES = new string[] { "(Thumb-Distal)", "(Index-Distal)", "(Middle-Distal)", "(Ring-Distal)", "(Pinky-Distal)" };

    private SocketServer server;

    void Start()
    {
        leftHandFingers = new List<Finger>();
        rightHandFingers = new List<Finger>();

        // Find all object at stage with InteractionBehavior script
        List<InteractionBehaviour> interactibleObjects = new List<InteractionBehaviour>();
        Transform[] allStageChildren = stage.GetComponentsInChildren<Transform>();
        foreach (Transform child in allStageChildren)
        {
            InteractionBehaviour childIb = child.GetComponent<InteractionBehaviour>();
            if (childIb != null) interactibleObjects.Add(childIb);
        }

        // Fill list of interactible objects
        ilList = new List<InteractibleObject>();
        foreach (InteractionBehaviour ib in interactibleObjects)
        {
            //Debug.Log(ib);
            ilList.Add(new InteractibleObject(this, ib));
        }

        server = new SocketServer("127.0.0.1", 7999, leftHandFingers, rightHandFingers); // DAT ADRESU, NIE 127.0.0.1
        server.Run();
    }

    private void Update() // Try to find fingers in scene or update sliders
    {
        if (leftHandFingers.Count != BONES_COUNT) TryFindFingers(BASE_PATH_LEFT);
        else UpdateSliders(leftHandFingers);
        if (rightHandFingers.Count != BONES_COUNT) TryFindFingers(BASE_PATH_RIGHT);
        else UpdateSliders(rightHandFingers);
    }

    public void UpdateOnHoverBegin(InteractibleObject caller) // Called during HoverStay() and HoverEnd() events by InteractionBehavior script
    {
        foreach (Finger finger in leftHandFingers)
        {
            if (!finger.nearObjects.Contains(caller)) finger.nearObjects.Add(caller);
        }
        foreach (Finger finger in rightHandFingers)
        {
            if (!finger.nearObjects.Contains(caller)) finger.nearObjects.Add(caller);
        }
    }

    public void UpdateOnHoverEnd(InteractibleObject caller) // Called during HoverStay() and HoverEnd() events by InteractionBehavior script
    {
        foreach (Finger finger in leftHandFingers)
        {
            if (finger.nearObjects.Contains(caller)) finger.nearObjects.Remove(caller);
        }
        foreach (Finger finger in rightHandFingers)
        {
            if (finger.nearObjects.Contains(caller)) finger.nearObjects.Remove(caller);
        }
    }
    private void UpdateSliders(List<Finger> fingers)
    {
        // If the list is empty, do nothing (hand was not yet initialized)
        if (fingers.Count != BONES_COUNT) return;

        foreach (Finger finger in fingers)
        {
            finger.minDistance = float.PositiveInfinity;
            foreach (InteractibleObject nearObj in finger.nearObjects)
            {
                // Cast ray from finger to the center of the object
                RaycastHit[] hits;
                Vector3 origin = finger.obj.transform.position;
                Vector3 destination = nearObj.gameObject.transform.position;
                //if (finger.slider == rightHandSliders.sliderIndex) Debug.Log(Vector3.Distance(origin, destination));
                hits = Physics.RaycastAll(origin, destination - origin, 100.0F);

                bool colliderHit = false;
                foreach (RaycastHit hitInfo in hits)
                {
                    // Debug - draw line in editor
                    if (finger.slider == rightHandSliders.sliderIndex) Debug.DrawRay(origin, destination - origin, Color.red);

                    // If the ray hit the object's collider, compare distance with threshold and update sliders
                    if (hitInfo.collider == nearObj.collider)
                    {
                        colliderHit = true;
                        if (hitInfo.distance < finger.minDistance)
                            finger.minDistance = hitInfo.distance;
                    }
                }
                if (colliderHit == false) // Inside of the object
                    finger.minDistance = 0;
            }
            if (finger.minDistance < touchDistance)
                finger.slider.value = ((touchDistance - finger.minDistance) * 170) / touchDistance;
            else
                finger.slider.value = 0;
        }
    }

    /*
    private void UpdateSliders(List<Finger> fingers, InteractibleObject caller)
    {
        // If the list is empty, do nothing (hand was not yet initialized)
        if (fingers.Count != BONES_COUNT) return;


        foreach (Finger finger in fingers)
        {
            // Cast ray from finger to the center of the object
            RaycastHit[] hits;
            Vector3 origin = finger.obj.transform.position;
            Vector3 destination = caller.gameObject.transform.position;
            hits = Physics.RaycastAll(origin, destination - origin, 100.0F);

            foreach (RaycastHit hitInfo in hits)
            {
                // Debug - draw line in editor
                if (finger.slider == rightHandSliders.sliderIndex) Debug.DrawRay(origin, destination - origin, Color.red);

                // If the ray hit the object's collider, compare distance with threshold and update sliders
                if (hitInfo.collider == caller.collider)
                {
                    if (hitInfo.distance < touchDistance)
                        finger.slider.value = ((touchDistance - hitInfo.distance) * 120) / touchDistance;
                    else
                        finger.slider.value = 0;
                }
            }
        }
    }
    */

    private void TryFindFingers(string basePath)
    {
        // Try to find distal bone of each finger
        foreach (string bone in BONE_NAMES)
        {
            GameObject tmp = GameObject.Find(basePath + bone);
            // If object was found, store it in the list
            if (tmp != null)
            {
                if (basePath == BASE_PATH_LEFT)
                    leftHandFingers.Add(new Finger(tmp, leftHandSliders, touchDistance));
                else
                    rightHandFingers.Add(new Finger(tmp, rightHandSliders, touchDistance));
            }
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

    private List<Finger> leftHandFingers;
    private List<Finger> rightHandFingers;

    public bool Running { get; set; }

    public SocketServer(string ip, int in_port, List<Finger> lFingers, List<Finger> rFingers)
    {
        localIP = IPAddress.Parse(ip);
        port = in_port;
        server = null;
        Running = false;
        leftHandFingers = lFingers;
        rightHandFingers = rFingers;
        listenThread = null;
    }

    public void Run()
    {
        // Start server
        server = new TcpListener(localIP, port);
        server.Start();

        // Start listening for clients
        listenThread = new Thread(new ThreadStart(Listen));
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
            Stop();
        }
    }

    private void HandleClient(TcpClient client)
    {
        // Get a stream object
        NetworkStream stream = client.GetStream();

        try
        {
            while (true)
            {
                // Create control string
                StringBuilder control_str = new StringBuilder();
                /*
                foreach (Finger f in leftHandFingers)
                {
                    control_str.Append("L" + f.nameShort + f.slider.value + ",");
                }
                foreach (Finger f in rightHandFingers)
                {
                    control_str.Append("R" + f.nameShort + f.slider.value + ",");
                }
                */
                if (rightHandFingers.Count > 0)
                {
                    foreach (Finger f in rightHandFingers)
                    {
                        control_str.Append(f.slider.value + ",");
                    }
                    control_str[control_str.Length - 1] = 'x';
                    control_str.Append('y');
                }
                

                byte[] msg = Encoding.ASCII.GetBytes(control_str.ToString());

                // Send data
                stream.Write(msg, 0, msg.Length);
                Debug.Log(control_str.ToString());

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
}