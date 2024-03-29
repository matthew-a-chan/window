﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class OpenCVFaceDetection : MonoBehaviour
{
    public static List<Vector2> NormalizedFacePositions { get; private set; }
    public static Vector2 CameraResolution;

    /// <summary>
    /// Downscale factor to speed up detection.
    /// </summary>
    private const int DetectionDownScale = 1;

    private bool _ready;
    private int _maxFaceDetectCount = 5;
    private CvCircle[] _faces;

    void Start()
    {
        int camWidth = 0, camHeight = 0;
        int result = OpenCVInterop.Init(ref camWidth, ref camHeight);
        Debug.Log(result);
        if (result == 1)
          _ready = true;
        else
          _ready = false;


        CameraResolution = new Vector2(camWidth, camHeight);
        _faces = new CvCircle[_maxFaceDetectCount];
        NormalizedFacePositions = new List<Vector2>();
        OpenCVInterop.SetScale(DetectionDownScale);
        _ready = true;
    }

    void OnApplicationQuit()
    {

        if(_ready)
        {
            OpenCVInterop.Close();
        }

    }
/*
    void Update()
    {
        if (!_ready)
            return;

        int detectedFaceCount = 0;
        unsafe
        {
            fixed(CvCircle* outFaces = _faces)
            {
                OpenCVInterop.Detect(outFaces, _maxFaceDetectCount, ref detectedFaceCount);
            }
        }

        NormalizedFacePositions.Clear();
        for(int i = 0; i < detectedFaceCount; i++)
        {
            NormalizedFacePositions.Add(new Vector2((_faces[i].X * DetectionDownScale) / CameraResolution.x, 1f - ((_faces[i].Y * DetectionDownScale) / CameraResolution.y)));
        }
    }
    */
}


internal static class OpenCVInterop
{
    [DllImport("TestPlugin")]
    internal static extern int Init(ref int outCameraWidth, ref int outCameraHeight);

    [DllImport("TestPlugin")]
    internal static extern int Close();

    [DllImport("TestPlugin")]
    internal static extern int SetScale(int downscale);

    [DllImport("TestPlugin")]
    internal unsafe static extern void Detect(CvCircle* outFaces, int maxOutFacesCount, ref int outDetectedFacesCount);
}


// Define the structure to be sequential and with the correct byte size (3 ints = 4 bytes * 3 = 12 bytes)
[StructLayout(LayoutKind.Sequential, Size = 12)]
public struct CvCircle
{
    public int X, Y, Radius;
}
