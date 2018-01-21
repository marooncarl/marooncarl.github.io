/// <summary>
/// Random audio delay
/// 
/// Programmer: Carl Childers
/// Date: 12/17/2015
/// 
/// If an audio source is also attached to this transform, this will cause it to play after
/// a randomized delay.  Used to try and prevent two of the same sound from playing at the
/// exact same time.
/// </summary>

using UnityEngine;
using System.Collections;

public class RandomAudioDelay : MonoBehaviour {

	public float MaxDelay = 1.0f;


	void Awake() {
		Invoke("StartSound", Random.value * MaxDelay);
	}

	void StartSound() {
		AudioSource myAudio = GetComponent<AudioSource>();
		if (myAudio != null) {
			myAudio.Play();
		}
	}
}
