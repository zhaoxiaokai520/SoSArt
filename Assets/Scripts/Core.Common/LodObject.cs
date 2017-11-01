using UnityEngine;
using System.Collections;

public class LodObject : MonoBehaviour
{
    public enum E_LOD_TYPE
    {
        E_LOD_HIGH,
        E_LOD_MIDDLE,
        E_LOD_LOW,
        E_LOD_MAX,
    };

    public E_LOD_TYPE meLodType = E_LOD_TYPE.E_LOD_LOW;
	
}
