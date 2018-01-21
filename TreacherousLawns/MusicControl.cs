/// <summary>
/// Music control
/// 
/// Programmer: Carl Childers
/// Date: 12/17/2015
/// 
/// Can delay the start of the music, and receive messages to stop or start it.
/// Assumes the same transform has an audio source providing the music.
/// </summary>

using UnityEngine;
using System.Collections;

public class MusicControl : MonoBehaviour {

	public float MusicDelay = 0.0f;
	public AudioClip MusicIntro;
	public AudioClip MusicClip;
	public float IntroDuration = 0.0f;
	public float MusicDuration = 0.0f;
	public Transform ChildPrefab;


	void Awake() {
		if (ChildPrefab == null) {
			enabled = false;
			return;
		}

		/*
		if (audio != null) {
			audio.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
			audio.ignoreListenerVolume = true;
			audio.ignoreListenerPause = true;
		}
		*/

		if (MusicDelay > 0) {
			Invoke("StartMusic", MusicDelay);
		} else {
			StartMusic();
		}
	}

	void StartMusic() {
		/*
		if (audio != null) {
			if (MusicIntro != null) {
				audio.clip = MusicIntro;
				Invoke("StartMain", IntroDuration);
			} else if (MusicClip != null) {
				audio.clip = MusicClip;
				Invoke("StartMain", MusicDuration);
			}
			audio.Play();
		}
		*/
		
		Transform newChild = (Transform)Instantiate(ChildPrefab, transform.position, Quaternion.identity);
		newChild.parent = transform;
		if (newChild.audio != null) {
			if (MusicIntro != null) {
				newChild.audio.clip = MusicIntro;
				Invoke("StartMain", IntroDuration);
				newChild.audio.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
				newChild.audio.ignoreListenerVolume = true;
				//newChild.audio.ignoreListenerPause = true;
				newChild.audio.Play();
				newChild.SendMessage("SetDestroyTime", IntroDuration + 1);

			} else if (MusicClip != null) {
				newChild.audio.clip = MusicClip;
				Invoke("StartMain", MusicDuration);
				newChild.audio.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
				newChild.audio.ignoreListenerVolume = true;
				//newChild.audio.ignoreListenerPause = true;
				newChild.audio.Play();
				newChild.SendMessage("SetDestroyTime", MusicDuration + 1);

			}
		}
	}

	void StartMain() {
		/*
		if (audio != null) {
			if (MusicClip != null) {
				audio.clip = MusicClip;
				Invoke("StartMain", MusicDuration);
			}
			audio.Play();
		}
		*/

		Transform newChild = (Transform)Instantiate(ChildPrefab, transform.position, Quaternion.identity);
		newChild.parent = transform;
		if (newChild.audio != null) {
			if (MusicClip != null) {
				newChild.audio.clip = MusicClip;
				Invoke("StartMain", MusicDuration);
				newChild.audio.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
				newChild.audio.ignoreListenerVolume = true;
				//newChild.audio.ignoreListenerPause = true;
				newChild.audio.Play();
				newChild.SendMessage("SetDestroyTime", MusicDuration + 1);
			}
		}
	}

	void StopMusic() {
		//print("Stop Music");
		CancelInvoke("StartMain");
		CancelInvoke("StartMusic");

		/*
		if (audio != null) {
			audio.Stop();
		}
		*/
		for (int i = 0; i < transform.childCount; ++i) {
			Transform currChild = transform.GetChild(i);
			if (currChild.audio != null) {
				currChild.audio.Stop();
			}
			Destroy(currChild.gameObject);
		}
	}
}
