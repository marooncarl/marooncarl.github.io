/// <summary>
/// Cut grass audio
/// 
/// Programmer: Carl Childers
/// Date: 6/7/2015
/// 
/// Cues audio for cutting grass when needed.  Should come with a looping audio component.
/// </summary>

using UnityEngine;
using System.Collections;

public class CutGrassAudio : MonoBehaviour {

	public float Duration = 1.0f;
	public float FadeInTime = 0.1f;
	public float FadeOutTime = 0.1f;
	public float MaxVolume = 1.0f;

	bool IsAudioOn;
	float ShutOffCounter;


	void Awake() {
		IsAudioOn = false;

		if (audio == null) {
			enabled = false;
			return;
		}
	}

	void Update() {
		if (ShutOffCounter > 0) {
			ShutOffCounter -= Time.deltaTime;
			if (ShutOffCounter <= 0) {
				IsAudioOn = false;
			}
		}

		if (IsAudioOn) {
			if (!audio.isPlaying) {
				audio.Play();
			}

			if (FadeInTime > 0) {
				audio.volume = Mathf.Min(audio.volume + (Time.deltaTime / FadeInTime), Mathf.Min(MaxVolume, 1.0f));
			} else {
				audio.volume = Mathf.Min(MaxVolume, 1.0f);
			}
		} else {
			if (FadeOutTime > 0) {
				audio.volume = Mathf.Max(audio.volume - (Time.deltaTime / FadeOutTime), 0.0f);
			} else {
				audio.volume = 0;
			}

			if (audio.volume == 0 && audio.isPlaying) {
				audio.Stop();
			}
		}
	}
	
	void CueAudio() {
		//print("Cued");

		if (!enabled) {
			return;
		}

		IsAudioOn = true;
		ShutOffCounter = Duration;
	}
}
