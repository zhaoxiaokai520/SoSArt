using UnityEngine;
using System.Collections;

public class CameraLookAtY : MonoBehaviour
{
    public Camera mCameraToLookAt;
    Transform mTrm;

    void Start()
    {
        mCameraToLookAt = Camera.main;
        mTrm = transform;
    }

    void LateUpdate()
    {
        Vector3 v = mCameraToLookAt.transform.position - mTrm.position;
        v.x = v.z = 0.0f;
        mTrm.LookAt(mCameraToLookAt.transform.position - v);
    }
}
