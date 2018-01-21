/// <summary>
/// Sound volume
/// 
/// Programmer: Carl Childers
/// Date: 12/22/2015
/// 
/// Controls sound volume
/// </summary>

using UnityEngine;
using System.Collections;

public class SoundVolume : MonoBehaviour {

	void Awake() {
		AudioListener.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
	}
}
