using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class DebugCameraController : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    [SerializeField] float sensitivity = 200f;
    [SerializeField] RectTransform minimapMarker;
    [SerializeField] PixelPerfectCamera pixelCamera;

    private void Update()
    {
        if (!pixelCamera)
            return;

        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        float scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel") * 10f;
        Vector3 move = (Vector3.right * xInput) + (Vector3.up * yInput);
        pixelCamera.assetsPPU += (int)(-scrollWheel * sensitivity * Time.deltaTime);
        pixelCamera.transform.position += move.normalized * speed * Time.deltaTime;
        if (pixelCamera.assetsPPU <= 1)
            pixelCamera.assetsPPU = 1;

        if (minimapMarker)
            minimapMarker.position += move.normalized * (speed * 2) * Time.deltaTime;
    }
}
