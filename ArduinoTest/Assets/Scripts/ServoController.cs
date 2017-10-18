using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

public class ServoController : MonoBehaviour {

    public GameObject top;
    public float rotationMultiplier;
    private Rigidbody rb;

    //private Thread writeThread;

    SerialPort serial;
    int baud = 115200;

    void Start () {
        rb = top.GetComponent<Rigidbody>();

        var portnames = SerialPort.GetPortNames();
        serial = new SerialPort(portnames[0], baud); // DORIESIT - UKAZOVAT ZOZNAM PORTOV NA VYBER
        serial.Open();
    }

    public void ValueUpdated(float floatVal)
    {
        if (serial == null || !serial.IsOpen)
            return;

        int value = Mathf.RoundToInt(floatVal);

        rb.transform.localRotation = Quaternion.AngleAxis(-floatVal*rotationMultiplier, Vector3.up);
        //Debug.Log("S" + value.ToString());
        //SerialWrite(value);
        serial.Write(value.ToString()+"#");
        //Debug.Log("R" + serial.ReadLine());

    }
    /*
    async Task SerialWrite(int value)
    {
        await Task.Run(() =>
        {
            serial.Write(value.ToString()+"#");
        });
    }*/
    /*
    private void StartThread(int value)
    {
      //  if (writeThread == null)
      //  {
            writeThread = new Thread(() => ThreadJob(value));
            writeThread.IsBackground = true;
            writeThread.Start();
      //  }
    }

    private void ThreadJob(int value)
    {
        serial.Write(value.ToString());
    }
    */


    void Update () {
       
    }

    void OnApplicationQuit()
    {
        serial.Close();
    }
}
