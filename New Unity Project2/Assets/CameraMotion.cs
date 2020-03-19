using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;


public class CameraMotion : MonoBehaviour
{

  Thread receiveThread; //1
  UdpClient client; //2
  int port; //3
  int terminated = 0;

  //input cam coords
  float xin;
  float yin;
  float zin;

  float x;
  float y;
  float z;

    [SerializeField]
    private GameObject target1;
    [SerializeField]
    private GameObject target2;

    private Camera myCam;

    float initialFOV;

    // Start is called before the first frame update
    void Start()
    {
        myCam = this.GetComponent<Camera>();
        initialFOV = myCam.fieldOfView;

        port = 5005;
        InitUDP();

        xin = 0;
        yin = 2;
        zin = -12;

        //float windowaspect = (float)Screen.width / (float)Screen.height;
    }

    void OnDisable()
    {
        terminated = 1;
    }

    private void InitUDP()
    {


      receiveThread = new Thread (new ThreadStart(ReceiveData)); //1
      receiveThread.IsBackground = true; //2
      receiveThread.Start(); //3
      Debug.Log("UDP Initialized");
    }

    private void ReceiveData()
    {
      client = new UdpClient (port); //1

      Regex rx = new Regex(@"\((-?\d*\.\d*), (-?\d*\.\d*), (-?\d*\.\d*)\)$", RegexOptions.Compiled);

      while (terminated == 0) //2
      {
        try
        {

          IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //3
          byte[] data = client.Receive(ref anyIP); //4

          string text = Encoding.UTF8.GetString(data); //5
          //print (">> " + text);

          //print(text);
          //Debug.Log(text);
          //Debug.Log(data);
          MatchCollection matches = rx.Matches(text);
          Debug.Log (matches.Count);
          GroupCollection groups = matches[0].Groups;
          Debug.Log ("-----------");
          Debug.Log(groups[1]);
          Debug.Log(groups[2]);
          Debug.Log(groups[3]);

          x = float.Parse (groups[1].Value);
          y = float.Parse (groups[2].Value);
          z = float.Parse (groups[3].Value);

        }
        catch(Exception e)
        {
          print (e.ToString()); //7
        }
      }
      Debug.Log ("Ending thread");
    }

    void resetFOV()
    {
        myCam.fieldOfView = initialFOV;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        gameObject.transform.position = new Vector3 (x*1f + xin, y*1f + yin, z + zin);

        Vector3 vector1 = target1.transform.position - gameObject.transform.position;
        Vector3 vector2 = target2.transform.position - gameObject.transform.position;

        Vector3 middle = Vector3.Slerp (vector1, vector2, .5f);
        gameObject.transform.forward = middle;



        float anglex = Vector2.Angle (flatteny(vector1), flatteny(vector2))/2;
        float angley = Vector2.Angle (flattenx(vector1), flattenx(vector2))/2;



//        Debug.Log (anglex);
//        Debug.Log (angley);

        myCam.fieldOfView = anglex;
        myCam.aspect = anglex/angley;
    }

    Vector2 flatteny (Vector3 vector3)
    {
        return new Vector2 (vector3.x, vector3.z);
    }

    Vector2 flattenx (Vector3 vector3)
    {
        return new Vector2 (vector3.y, vector3.z);
    }
}






/*
        float width = target1.transform.position.x - target2.transform.position.x;
        float height = target1.transform.position.y - target2.transform.position.y;
//        float A = Mathf.A;
//        float B = ;
//        float C = ;
        Vector3 avg = ( target1.transform.position + target2.transform.position ) / 2;

        float x = gameObject.transform.position.x - avg.x;
        float y = gameObject.transform.position.y - avg.y;
        float z = gameObject.transform.position.z - avg.z;

        float FOVX =
        Mathf.Asin (2 / ( Mathf.Sqrt (x * x / z / z + 1) * width * Mathf.Sqrt ( (-width / 2 - x) * (-width / 2 - x) + z * z) ) )
        - Mathf.Asin (2 / ( Mathf.Sqrt (x * x / z / z + 1) * width * Mathf.Sqrt ( (width / 2 - x) * (-width / 2 - x) + z * z) ) );

        float FOVY =
        Mathf.Asin (2 / ( Mathf.Sqrt (y * y / z / z + 1) * height * Mathf.Sqrt ( (-height / 2 - y) * (-height / 2 - y) + z * z) ) )
        - Mathf.Asin (2 / ( Mathf.Sqrt (y * y / z / z + 1) * height * Mathf.Sqrt ( (height / 2 - y) * (-height / 2 - y) + z * z) ) );

        Debug.Log (FOVX);
*/
