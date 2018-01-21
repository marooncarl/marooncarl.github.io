/// <summary>
/// Vehicle sound
/// 
/// Programmer: Carl Childers
/// Date: 6/27/2015
/// 
/// Allows a vehicle to raise its volume when turning on or lower it when turning off.  Expects an audio component
/// </summary>


using UnityEngine;
using System.Collections;

public class VehicleSound : MonoBehaviour {

	public bool IsOn = true;
	public float TargetVolume = 1.0f;
	public float TargetPitch = 1.0f;
	public float TurningOnEaseFactor = 4f;
	public float TurningOffEaseFactor = 4f;

	AudioSource MyAudio;


	void Awake () {
		MyAudio = GetComponent<AudioSource>();
		if (MyAudio == null) {
			enabled = false;
		}
	}

	void Update () {
		UpdateAudio();
	}

	void UpdateAudio() {
		if (IsOn) {
			if (TurningOnEaseFactor > 0) {
				if (MyAudio.volume < TargetVolume) {
					MyAudio.volume += (TargetVolume - MyAudio.volume) / TurningOnEaseFactor;
					if (TargetVolume - MyAudio.volume < 0.01f) {
						MyAudio.volume = TargetVolume;
					}
				}
				if (MyAudio.pitch < TargetPitch) {
					MyAudio.pitch += (TargetPitch - MyAudio.pitch) / TurningOnEaseFactor;
					if (TargetPitch - MyAudio.pitch < 0.01f) {
						MyAudio.pitch = TargetPitch;
					}
				}
			}
		} else {
			if (TurningOffEaseFactor > 0) {
				if (MyAudio.volume > 0) {
					MyAudio.volume -= (MyAudio.volume / TurningOffEaseFactor);
					if (MyAudio.volume < 0.01f) {
						MyAudio.volume = 0;
					}
				}
				if (MyAudio.pitch > 0) {
					MyAudio.pitch -= (MyAudio.pitch / TurningOffEaseFactor);
					if (MyAudio.pitch < 0.01f) {
						MyAudio.pitch = 0;
					}
				}
			}
		}
	}

	// Messages

	void StartTurningOn() {
		IsOn = true;
	}

	void StartTurningOff() {
		IsOn = false;
	}
}
