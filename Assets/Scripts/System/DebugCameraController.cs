using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera movingCamera;
    [SerializeField] float speed = 20f;

    private void Update()
    {
        if (!movingCamera)
            return;

        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        float scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel") * 10f;
        Vector3 move = (Vector3.right * xInput) + (Vector3.up * yInput);
        movingCamera.m_Lens.OrthographicSize += -scrollWheel * speed * Time.deltaTime;
        movingCamera.transform.position += move.normalized * speed * Time.deltaTime;
        if (movingCamera.m_Lens.OrthographicSize <= 1)
            movingCamera.m_Lens.OrthographicSize = 1;
    }
}
