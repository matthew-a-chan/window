using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class testCPP : MonoBehaviour
{

    [DllImport ("TestPlugin")]
    private static extern int test();


    // Start is called before the first frame update
    void Start()
    {
      Debug.Log(test());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
