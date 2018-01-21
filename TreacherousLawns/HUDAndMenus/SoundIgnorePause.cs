/// <summary>
/// Sound ignore pause
/// 
/// Programmer: Carl Childers
/// Date: 12/22/2015
/// 
/// Causes an attached audio source to ignore the Audio Listener's pause
/// </summary>

using UnityEngine;
using System.Collections;

public class SoundIgnorePause : MonoBehaviour {

	void Awake() {
		if (audio != null) {
			audio.ignoreListenerPause = true;
		}
	}
}
