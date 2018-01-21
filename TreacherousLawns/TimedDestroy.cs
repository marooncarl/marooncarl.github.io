using UnityEngine;
using System.Collections;

public class TimedDestroy : MonoBehaviour
{
	public float DestroyTime = 1.0f;
	
	// Use this for initialization
	void Start ()
	{
		CancelInvoke();
		Invoke("SelfDestruct", DestroyTime);
	}
	
	void SelfDestruct()
	{
		Destroy(this.gameObject);
	}

	// Messages

	// Throws out the current SelfDestruct timer and starts new one
	void SetDestroyTime(float inTime) {
		CancelInvoke();
		DestroyTime = inTime;
		Invoke("SelfDestruct", inTime);
	}
}
