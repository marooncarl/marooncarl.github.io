// Programmer: Carl Childers
// Date: 10/16/2017
//
// Enforce calm music in case it was frenetic earlier.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGameMusicControl : MonoBehaviour {

	public int MusicLayer = 0;

	void Start()
	{
		MusicLayerPlayer theMusicPlayer = FindObjectOfType<MusicLayerPlayer>();
		if (theMusicPlayer != null)
		{
			theMusicPlayer.CurrentMusicLayer = MusicLayer;
		}
	}
}
