using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMirrorFlipper : MonoBehaviour
{
    public Camera m_camera;
    private void Start()
    {
        Matrix4x4 mat = m_camera.projectionMatrix;
        mat *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
        m_camera.projectionMatrix = mat;
    }
}
