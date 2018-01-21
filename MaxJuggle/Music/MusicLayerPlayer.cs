// Programmer: Carl Childers
// Date: 10/20/2017
//
// Plays multiple song parts at once, silencing those that are in a layer above the current layer index.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicLayerPlayer : MonoBehaviour {

	public AudioClip[] MusicLayers;
	public int StartingLayer = 0;
	public float StartDelay = 1;
	public float FadeDuration = 1;
	public AudioMixerGroup MusicGroup;

	int currentMusicLayer = 0;
	public int CurrentMusicLayer {
		get { return currentMusicLayer; }
		set { currentMusicLayer = value; }
	}

	bool isPlaying;
	public bool IsPlaying {
		get { return isPlaying; }
	}

	bool paused;
	public bool Paused {
		get { return paused; }
	}

	bool fadingOut;

	AudioSource[] childMusicPlayers;
	float delayCounter;


	void Start()
	{
		delayCounter = StartDelay;
		if (delayCounter <= 0)
		{
			isPlaying = true;
		}
		currentMusicLayer = StartingLayer;
		InitMusicPlayers();
	}

	void Update()
	{
		if (delayCounter > 0)
		{
			delayCounter -= Time.deltaTime;
			if (delayCounter <= 0)
			{
				Play();
			}
		}
		UpdateLayerVolumes();
	}

	void InitMusicPlayers()
	{
		childMusicPlayers = new AudioSource[MusicLayers.Length];
		int i = 0;
		foreach (AudioClip lay in MusicLayers)
		{
			GameObject musicObj = new GameObject("Music Layer");
			AudioSource newPlayer = musicObj.AddComponent<AudioSource>();
			musicObj.transform.SetParent(transform);
			newPlayer.clip = lay;
			newPlayer.outputAudioMixerGroup = MusicGroup;
			newPlayer.loop = true;
			newPlayer.volume = (i <= currentMusicLayer ? 1 : 0);
			childMusicPlayers[i] = newPlayer;

			if (isPlaying && !paused)
			{
				if (!newPlayer.isPlaying)
				{
					newPlayer.Play();
				}
			}
			else
			{
				if (newPlayer.isPlaying)
				{
					newPlayer.Stop();
				}
			}

			++i;
		}
	}

	public void Play(bool skipFade = true)
	{
		isPlaying = true;
		paused = false;
		foreach (AudioSource ply in childMusicPlayers)
		{
			ply.Play();
		}

		if (skipFade)
		{
			for (int i = 0; i < childMusicPlayers.Length; ++i)
			{
				if (i <= currentMusicLayer)
				{
					childMusicPlayers[i].volume = 1;
				}
				else
				{
					childMusicPlayers[i].volume = 0;
				}
			}
		}
	}

	public void Stop()
	{
		isPlaying = false;
		paused = false;
		foreach (AudioSource ply in childMusicPlayers)
		{
			ply.Stop();
		}
	}

	public void Pause()
	{
		if (isPlaying)
		{
			paused = true;
			foreach (AudioSource ply in childMusicPlayers)
			{
				ply.Play();
			}
		}
	}

	// Fades layers in and out, depending on whether each layer is less than or equal to the current music layer.
	// If the music player is paused or stopped, then fading is skipped and the volume is set to the target amount immediately.
	// Fading is also skipped when fade duration is <= 0
	void UpdateLayerVolumes()
	{
		int numSilentLayers = 0;

		for (int i = 0; i < childMusicPlayers.Length; ++i)
		{
			if (i <= currentMusicLayer && !fadingOut)
			{
				// Volume up
				if (FadeDuration > 0 && isPlaying && !paused)
				{
					childMusicPlayers[i].volume = Mathf.Min(childMusicPlayers[i].volume + (Time.unscaledDeltaTime / FadeDuration), 1f);
				}
				else
				{
					childMusicPlayers[i].volume = 1;
				}
			}
			else
			{
				// Go silent
				if (FadeDuration > 0 && isPlaying && !paused)
				{
					childMusicPlayers[i].volume = Mathf.Max(childMusicPlayers[i].volume - (Time.unscaledDeltaTime / FadeDuration), 0);
				}
				else
				{
					childMusicPlayers[i].volume = 0;
				}

				if (childMusicPlayers[i].volume <= 0)
				{
					numSilentLayers++;
				}
			}
		}

		//print("Num silent layers: " + numSilentLayers);
		if (fadingOut && numSilentLayers >= childMusicPlayers.Length)
		{
			//print("Finished fading out");
			Stop();
			fadingOut = false;
		}
	}

	public void FadeOut()
	{
		if (!isPlaying)
			return;

		//print("Started fading out");
		fadingOut = true;
	}
}
