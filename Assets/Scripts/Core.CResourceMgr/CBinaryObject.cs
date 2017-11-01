using System;
using UnityEngine;
using UnityEngine.Networking.Match;

public class CBinaryObject : ScriptableObject
{
	public byte[] m_data;

    public void Destroy()
    {
        m_data = null;
        GameObject.Destroy(this);
    }
}
