using UnityEngine;

public class CBinaryObject : ScriptableObject
{
	public byte[] m_data;

    public void Destroy()
    {
        m_data = null;
        GameObject.Destroy(this);
    }
}
