using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFollowDevice : MonoBehaviour
{
#if UNITY_EDITOR
    public static Action OnSolutionChanged;
#endif

    public float MaxAspect = 2, MinAspect = 1;
    public float MaxCamSize = 5, MinCamSize = 5;
    private CanvasScaler _canvasScaler;
    private Camera _cam;

    private void Awake()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
        _cam = Camera.main;
        FixCamSizeFollowScreen();

#if UNITY_EDITOR
        OnSolutionChanged += Update;
#endif
    }


#if UNITY_EDITOR
    public void Update()
    {
        FixCamSizeFollowScreen();
    }
#endif

    [ContextMenu("Fix cam zide follow screen")]
    private void FixCamSizeFollowScreen()
    {
        float aspect;
        if (Screen.height > Screen.width)
            aspect = Screen.height / (float) Screen.width;
        else
            aspect = Screen.width / (float) Screen.height;

        _canvasScaler.matchWidthOrHeight = 1 - (aspect - MinAspect) / (MaxAspect - MinAspect);

        _cam.orthographicSize = MinCamSize + (aspect - MinAspect) / (MaxAspect - MinAspect) * (MaxCamSize - MinCamSize);
    }
}