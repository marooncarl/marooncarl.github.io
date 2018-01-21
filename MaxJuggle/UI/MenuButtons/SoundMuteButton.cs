// Programmer: Carl Childers
// Date: 9/4/2017
//
// Mutes or unmutes sound.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMuteButton : ToggleButton {

	public AudioMixer AffectedMixer;
	public string AffectedProperty;
	public float OnVolume = 0;
	public float OffVolume = -80f;


	protected override void SetInitialState()
	{
		if (AffectedMixer != null)
		{
			float volumeSetting = PlayerPrefs.GetFloat(AffectedProperty, OnVolume);
			AffectedMixer.SetFloat(AffectedProperty, volumeSetting);
			isOn = (volumeSetting > OffVolume);
		}
	}

	protected override void PerformAction(bool turnedOn)
	{
		if (AffectedMixer != null)
		{
			float newVolume = (turnedOn ? OnVolume : OffVolume);
			PlayerPrefs.SetFloat(AffectedProperty, newVolume);
			AffectedMixer.SetFloat(AffectedProperty, newVolume);
		}
	}
}
