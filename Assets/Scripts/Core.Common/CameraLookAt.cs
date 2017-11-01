using UnityEngine;
using System.Collections;

public class CameraLookAt : MonoBehaviour {

    public Camera m_Camera;
    Transform mTrm;

    void Start()
    {
        m_Camera = Camera.main;
        mTrm = transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.back,
            m_Camera.transform.rotation * Vector3.up);
    }
}
