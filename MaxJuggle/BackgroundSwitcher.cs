// Programmer: Carl Childers
// Date: 11/10/2017
//
// Allows a background object to switch to a sub-location, enabling one background child object and disabling the others.
// This is done by calling SwitchBackground() with the appropriate sub-index.
// SwitchBackground() triggers a transition effect, which will keep playing until the game control is finished loading or unloading.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSwitcher : MonoBehaviour {

	public float TransitionWarmupTime = 0.4f;
	public float TransitionCooldownTime = 0.4f;

	// These should be set to child objects within the background.
	public GameObject[] SubBackgrounds;

	// This should be set to a child particle system within the background.
	public ParticleSystem TransitionEffect;

	int currentBGIndex = 0;
	public int CurrentBGIndex {
		get { return currentBGIndex; }
	}

	int pendingBGIndex = 0;
	float transitionCounter = 0;
	bool inTransition = false;


	void Update()
	{
		if (transitionCounter > 0)
		{
			transitionCounter -= Time.unscaledDeltaTime;
			if (transitionCounter <= 0)
			{
				currentBGIndex = pendingBGIndex;
				ChangeBackground();
			}
		}
		else
		{
			// Stop the transition when finished with transition warm up and finished loading / unloading scenes
			if (inTransition && !GameControl.GetGameControl().IsLoadingOrUnloading())
			{
				FinishTransition();
			}
		}
	}

	// Begins background switch.  Backgrounds will actually be switched once the transition effect has covered the screen.
	public void SwitchBackground(int bgIndex)
	{
		if (bgIndex != currentBGIndex && bgIndex >= 0 && bgIndex < SubBackgrounds.Length)
		{
			//print("Beginning background switch");
			if (inTransition)
			{
				if (transitionCounter > 0)
				{
					// Still warming up the transition effect, so don't switch backgrounds yet
					pendingBGIndex = bgIndex;
				}
				else
				{
					// Transition is already covering the screen, so can immediately change background
					currentBGIndex = bgIndex;
					pendingBGIndex = bgIndex;
					ChangeBackground();
				}
			}
			else
			{
				// Begin transition, and switch backgrounds when the screen is covered
				TransitionEffect.gameObject.SetActive(true);
				TransitionEffect.Play();
				inTransition = true;
				transitionCounter = TransitionWarmupTime;
				currentBGIndex = bgIndex;
				pendingBGIndex = bgIndex;
			}
		}
	}

	void ChangeBackground()
	{
		for (int i = 0; i < SubBackgrounds.Length; ++i)
		{
			if (i == currentBGIndex)
			{
				SubBackgrounds[i].SetActive(true);
			}
			else
			{
				SubBackgrounds[i].SetActive(false);
			}
		}
	}

	void FinishTransition()
	{
		//print("Background finished transition");
		inTransition = false;
		if (TransitionEffect.isPlaying)
		{
			TransitionEffect.Stop();
			Invoke("HideTransition", TransitionCooldownTime);
		}
	}

	void HideTransition()
	{
		TransitionEffect.gameObject.SetActive(false);
	}
}
