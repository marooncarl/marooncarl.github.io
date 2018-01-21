// Programmer: Carl Childers
// Date: 11/29/2017
//
// Clears bookmark, and brings up the normal play menu.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseBookmarkButton : MonoBehaviour {

	public AudioClip ButtonPressSound;
	[Range(0, 1)]
	public float ButtonPressVolume = 1;
	public string AudioObjectName = "MenuSoundPlayer";


	public void Erase()
	{
		Bookmark.ClearBookmark();

		if (ButtonPressSound != null)
		{
			// Use an object in the main scene to play the sound so it will continue across scenes
			GameObject soundObj = GameObject.Find(AudioObjectName);
			if (soundObj != null)
			{
				AudioSource audio = soundObj.GetComponent<AudioSource>();
				if (audio != null)
				{
					audio.PlayOneShot(ButtonPressSound, ButtonPressVolume);
				}
			}
		}

		TitleScreen title = FindObjectOfType<TitleScreen>();
		if (title != null)
		{
			title.Exit();
			title.Invoke("SwitchToNormalMenu", title.ExitDuration);
		}
	}
}
