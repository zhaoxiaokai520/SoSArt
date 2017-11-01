using UnityEngine;
using System.Collections.Generic;

public class StaticBatchScene : MonoBehaviour
{
    public List<GameObject> mBatchObjs = new List<GameObject>();
    private static bool mbABMode = true;
    public List<Transform> mRotateTrms = new List<Transform>(); 
	// Use this for initialization
	void Awake () {
	    if (mbABMode)
	    {
	        for (int i = 0; i < mBatchObjs.Count; i++)
	        {
                if (mBatchObjs[i] != null)
                {
                    StaticBatchingUtility.Combine(mBatchObjs[i]);
                }	            
	        }
	    }        
	}
}
