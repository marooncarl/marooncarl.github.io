// Programmer: Carl Childers
// Date: 9/26/2017
//
// Causes Flammable objects to catch fire.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSpreader : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!enabled)
			return;

		Flammable flameReceiver = other.GetComponent<Flammable>();
		if (flameReceiver != null)
		{
			flameReceiver.CatchFire();
		}
	}
}
