// Programmer: Carl Childers
// Date: 10/16/2017
//
// Changes the music based on the number of juggle items present.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleMusicControl : MonoBehaviour {

	public int CalmMusicLayer = 0;
	public int FreneticMusicLayer = 1;
	public int PausedMusicLayer = 0;
	public int ItemThreshold = 3;
	public float CalmDownTime = 3.0f;
	public float GetFreneticTime = 0.5f;
	public float FirstTimeFreneticTime = 1.25f;

	MusicLayerPlayer myMusicPlayer;
	GameTosser theTosser;

	float calmDownCounter;
	float freneticCounter;
	bool haveUsedFreneticMusic;


	void Start()
	{
		myMusicPlayer = FindObjectOfType<MusicLayerPlayer>();
		theTosser = FindObjectOfType<GameTosser>();
		freneticCounter = GetFreneticTime;
		haveUsedFreneticMusic = false;
	}

	void Update()
	{
		if (Time.timeScale <= 0)
		{
			myMusicPlayer.CurrentMusicLayer = PausedMusicLayer;
		}
		else if (theTosser != null)
		{
			CheckNumObjectsInPlay();
		}
		else
		{
			myMusicPlayer.CurrentMusicLayer = CalmMusicLayer;
		}
	}

	void CheckNumObjectsInPlay()
	{
		if (theTosser.DesiredObjectsInPlay >= ItemThreshold)
		{
			if (!haveUsedFreneticMusic)
			{
				// Stop and go back to the beginning the first time the music comes on.
				myMusicPlayer.FadeOut();
				haveUsedFreneticMusic = true;
				freneticCounter = FirstTimeFreneticTime;
				calmDownCounter = CalmDownTime;
			}
			else
			{
				freneticCounter -= Time.deltaTime;
				calmDownCounter = CalmDownTime;
				if (freneticCounter <= 0)
				{
					myMusicPlayer.CurrentMusicLayer = FreneticMusicLayer;
					if (!myMusicPlayer.IsPlaying)
					{
						myMusicPlayer.Play();
					}
				}
			}
		}
		else
		{
			calmDownCounter -= Time.deltaTime;
			freneticCounter = GetFreneticTime;
			if (calmDownCounter <= 0)
			{
				myMusicPlayer.CurrentMusicLayer = CalmMusicLayer;
				if (!myMusicPlayer.IsPlaying)
				{
					myMusicPlayer.Play();
				}
			}
		}
	}
}
