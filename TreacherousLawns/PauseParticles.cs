/// <summary>
/// Pause particles
/// 
/// Programmer: Carl Childers
/// Date: 12/22/2015
/// 
/// Pauses attached particle systems
/// </summary>

using UnityEngine;
using System.Collections;

public class PauseParticles : MonoBehaviour {

	public bool PauseChildren = true;


	void Awake() {
		if (particleSystem != null) {
			particleSystem.Pause(PauseChildren);
		}
	}
}
