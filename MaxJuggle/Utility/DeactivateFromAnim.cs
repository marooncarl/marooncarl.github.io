// Programmer: Carl Childers
// Date: 9/18/2017
//
// Allows an animation to deactivate the game object when finished with an animation.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateFromAnim : MonoBehaviour {

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}
}
