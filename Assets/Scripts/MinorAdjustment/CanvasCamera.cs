using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCamera : MonoBehaviour
{
    private Camera canvasCamera;

    public static CanvasCamera instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        SetCanvasRenderCamera();
    }
    public void SetCanvasRenderCamera()
    {
        canvasCamera = GameObject.Find("UI Camera").GetComponent<Camera>();
        gameObject.GetComponent<Canvas>().worldCamera = canvasCamera;
    }
}
