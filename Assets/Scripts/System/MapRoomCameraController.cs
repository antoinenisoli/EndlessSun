using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class MapRoomCameraController : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    [SerializeField] int sensitivity = 200;
    [SerializeField] Vector2Int bounds;
    [SerializeField] RectTransform minimapMarker;
    [SerializeField] PixelPerfectCamera pixelCamera;
    [SerializeField] Transform cinemachineCamera;

    void Zoom()
    {
        float scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scrollWheel != 0)
        {
            int scroll = Mathf.RoundToInt(scrollWheel * 10f);
            pixelCamera.assetsPPU += scroll * (2 * sensitivity);
        }

        int x = bounds.x;
        if (x % 2 != 0)
            x++;
        int y = bounds.y;
        if (y % 2 != 0)
            y++;

        pixelCamera.assetsPPU = Mathf.Clamp(pixelCamera.assetsPPU, x, y);
    }

    void Moving()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        Vector3 move = (Vector3.right * xInput) + (Vector3.up * yInput);
        cinemachineCamera.transform.position += move.normalized * speed * Time.deltaTime;
        if (minimapMarker)
            minimapMarker.position += move.normalized * (speed * 2) * Time.deltaTime;
    }

    private void Update()
    {
        if (!pixelCamera)
            return;

        Moving();
        Zoom();

        if (Input.GetKeyDown(KeyCode.Space))
            GridManager.Instance.GenerateMap();
        else if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
