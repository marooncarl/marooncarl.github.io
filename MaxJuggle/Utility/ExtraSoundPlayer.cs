// Programmer: Carl Childers
// Date: 12/18/2017
//
// Plays extra sounds as needed when it is inconvenient for an existing audio source to play them.
// Similar to using PlayClipAtPoint(), but can use an audio mixer group.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExtraSoundPlayer {

	static ExtraSoundPlayer theSoundPlayer;

	//LinkedList<ExtraSound> inactiveSoundObjects;


	public static ExtraSoundPlayer GetSoundPlayer()
	{
		if (theSoundPlayer == null)
		{
			theSoundPlayer = new ExtraSoundPlayer();
		}

		return theSoundPlayer;
	}

	public void Play(AudioClip inClip, Vector3 inLocation, float inVolume, AudioMixerGroup inMixerGroup)
	{
		/*
		ExtraSound snd;
		if (inactiveSoundObjects != null && inactiveSoundObjects.Count > 0)
		{
			snd = inactiveSoundObjects.First.Value;
		}
		else
		{
			// Create new sound object
			GameObject soundObj = new GameObject("Extra Sound");
			snd = soundObj.AddComponent<ExtraSound>();
			//snd.Initialize(this);
		}
		*/

		GameObject soundObj = new GameObject("Extra Sound");
		ExtraSound snd = soundObj.AddComponent<ExtraSound>();

		snd.transform.position = inLocation;
		snd.Play(inClip, inVolume, inMixerGroup, inClip.length + 0.1f);
	}

	/*
	public void AddInactiveSoundObj(ExtraSound inSoundObj)
	{
		if (inactiveSoundObjects == null)
		{
			inactiveSoundObjects = new LinkedList<ExtraSound>();
		}
		inactiveSoundObjects.AddLast(inSoundObj);
	}
	*/
}
