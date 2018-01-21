/// <summary>
/// Wall impact sound
/// 
/// Programmer: Carl Childers
/// Date: 12/15/2015
/// 
/// Allows different wall pieces to play a different impact sound.
/// </summary>

using UnityEngine;
using System.Collections;

public class WallImpactSound : MonoBehaviour {

	public AudioClip ImpactSound;
	public float SoundVolume = 1.0f;


	void Awake() {
		if (ImpactSound == null || SoundVolume <= 0) {
			enabled = false;
			return;
		}
	}

	// Impact sound message
	void PlayImpactEffects(Vector3 inLocation) {
		AudioSource.PlayClipAtPoint(ImpactSound, inLocation, SoundVolume);
	}
}
