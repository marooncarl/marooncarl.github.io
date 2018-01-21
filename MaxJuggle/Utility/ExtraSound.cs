// Programmer: Carl Childers
// Date: 12/18/2017
//
// Helper class for ExtraSoundPlayer.  Disables its audio object after a short time and adds it as an inactive object.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExtraSound : MonoBehaviour {

	//ExtraSoundPlayer myPlayer;
	AudioSource myAudioSource;
	float timeLeft;


	/*
	public void Initialize(ExtraSoundPlayer inPlayer)
	{
		//myPlayer = inPlayer;
		myAudioSource = gameObject.AddComponent<AudioSource>();
	}
	*/

	public void Play(AudioClip inClip, float inVolume, AudioMixerGroup inMixerGroup, float duration)
	{
		if (myAudioSource == null)
		{
			myAudioSource = gameObject.AddComponent<AudioSource>();
		}

		gameObject.SetActive(true);
		myAudioSource.outputAudioMixerGroup = inMixerGroup;
		myAudioSource.PlayOneShot(inClip, inVolume);
		timeLeft = duration;
	}

	void Update()
	{
		timeLeft -= Time.unscaledDeltaTime;
		if (timeLeft <= 0)
		{
			//myPlayer.AddInactiveSoundObj(this);
			//gameObject.SetActive(false);
			Destroy(gameObject);
		}
	}
}
