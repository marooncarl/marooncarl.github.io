// Programmer: Carl Childers
// Date: 9/22/2017
//
// Creates or reuses a particle effect object when restarted.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycledParticleEffect {

	ParticleSystem myPrefab;
	public ParticleSystem MyPrefab {
		get { return myPrefab; }
	}

	ParticleSystem myInstance;
	public ParticleSystem MyInstance {
		get { return myInstance; }
	}

	// Constructor - assigns a particle effect prefab
	public RecycledParticleEffect(ParticleSystem inParticleSystem)
	{
		myPrefab = inParticleSystem;
	}

	// Creates or restarts the particle effect instance
	public void Restart(Vector3 startPosition)
	{
		if (myInstance == null)
		{
			myInstance = MonoBehaviour.Instantiate(myPrefab, startPosition, myPrefab.transform.rotation);
			if (!myInstance.isPlaying)
			{
				myInstance.Play();
			}
		}
		else
		{
			myInstance.transform.position = startPosition;
			if (myInstance.isPlaying)
			{
				myInstance.Stop();
			}
			myInstance.Play();
		}
	}
}
