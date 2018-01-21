// Programmer: Carl Childers
// Date: 7/25/2016
//
// Unparents all of the children of this object.  Useful for grouping things in the editor without them
// all having the same root in game.

using UnityEngine;
using System.Collections;

public class UnparentChildren : MonoBehaviour {

	public bool DestroySelf = true;


	void Awake()
	{
		transform.DetachChildren();
		if (DestroySelf)
			GameObject.Destroy(gameObject);

		enabled = false;
	}
}
