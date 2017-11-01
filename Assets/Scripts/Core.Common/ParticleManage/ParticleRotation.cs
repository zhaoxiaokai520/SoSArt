using UnityEngine;
using System.Collections;

public class ParticleRotation : MonoBehaviour
{

    public ParticleSystem[] m_ParticleSystem;
    int angle = 0;
    public void OnEnable()
    {
        if (m_ParticleSystem == null || m_ParticleSystem.Length == 0) return;
        angle =(int) transform.rotation.eulerAngles.y;
        for (int i = 0; i < m_ParticleSystem.Length; i++)
        {
            m_ParticleSystem[i].startRotation3D = new Vector3(0, angle * Mathf.Deg2Rad, 0);
        }
    }
}
