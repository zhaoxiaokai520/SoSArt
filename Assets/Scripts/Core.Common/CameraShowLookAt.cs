using UnityEngine;
using System.Collections;


public class CameraShowLookAt : MonoBehaviour {
	public Transform mTarget;
	public Animation mAnim;
	public Transform _mPos ;

	public bool mbPlay = false;

	void Awake()
	{
		if(mAnim == null)
			mAnim = GetComponent<Animation> ();
	}

	public void Play()
	{
		mbPlay = true;
		if(mAnim != null)
			mAnim.Play ();
	}

	void LateUpdate()
	{
		if (mbPlay && mTarget != null && mAnim != null ) {
			transform.position = _mPos.position;
			transform.LookAt(mTarget.position);
		}
	}
}