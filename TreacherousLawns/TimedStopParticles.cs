/// <summary>
/// Timed stop particles
/// 
/// Programmer: Carl Childers
/// Date: 6/27/2015
/// 
/// Stops particle systems in this game object and its children after a certain amount of time.  May precede a TimedDestroy
/// so that the particle system is given time to dissipate before the object is destroyed.
/// </summary>

using UnityEngine;
using System.Collections;

public class TimedStopParticles : MonoBehaviour {

	public float StopTime = 1.0f;


	// Use this for initialization
	void Start () {
		Invoke("StopParticles", StopTime);
	}
	
	void StopParticles() {
		if (particleSystem != null) {
			particleSystem.Stop();
		}
		for (int i = 0; i < transform.childCount; ++i) {
			StopChildParticles(transform.GetChild(i));
		}
	}

	// Recursive; goes through children and stops particle effects
	void StopChildParticles(Transform inChild) {
		if (inChild.particleSystem != null) {
			inChild.particleSystem.Stop();
		}
		
		for (int i = 0; i < inChild.childCount; ++i) {
			StopChildParticles(inChild.GetChild(i));
		}
	}
}
