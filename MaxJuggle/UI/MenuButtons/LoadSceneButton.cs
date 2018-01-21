// Programmer: Carl Childers
// Date: 9/8/2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneButton : MonoBehaviour {

	public string SceneName;
	public float LoadSceneDelay = 0.5f;
	public Animator AnimatedObject;						// Menu that needs to play an exit animation when this button is clicked
	public string AnimObjectName;						// In case the animated object is not available and needs to be found by name.
	public string ExitAnimTrigger = "Exit";
	public string AudioObjectName = "MenuSoundPlayer";

	public AudioClip ButtonPressSound;
	[Range(0, 2)]
	public float ButtonPressSoundVolume = 1;

	float loadCounter;
	protected float LoadCounter {
		get { return loadCounter; }
	}

	bool beganLoadingScene;
	protected bool BeganLoadingScene {
		get { return beganLoadingScene; }
	}


	void Start()
	{
		if (AnimatedObject == null && AnimObjectName != "")
		{
			GameObject gameObj = GameObject.Find(AnimObjectName);
			if (gameObj != null)
			{
				AnimatedObject = gameObject.GetComponent<Animator>();
			}
		}
	}

	void Update()
	{
		UpdateButton();
	}

	protected virtual void UpdateButton()
	{
		if (loadCounter > 0)
		{
			loadCounter -= Time.deltaTime;
			if (loadCounter <= 0)
			{
				LoadScene();
			}
		}
	}

	public virtual void BeginLoadingScene()
	{
		if (beganLoadingScene)
			return;

		if (ButtonPressSound != null)
		{
			// Use an object in the main scene to play the sound so it will continue across scenes
			GameObject soundObj = GameObject.Find(AudioObjectName);
			if (soundObj != null)
			{
				AudioSource audio = soundObj.GetComponent<AudioSource>();
				if (audio != null)
				{
					audio.PlayOneShot(ButtonPressSound, ButtonPressSoundVolume);
				}
			}
		}

		beganLoadingScene = true;
		if (LoadSceneDelay > 0)
		{
			loadCounter = LoadSceneDelay;
			if (AnimatedObject != null)
			{
				AnimatedObject.SetTrigger(ExitAnimTrigger);
				AnimatedObject.SendMessage("Exit", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			LoadScene();
		}
	}

	void LoadScene()
	{
		GameControl.GetGameControl().LoadScene(SceneName);
	}
}
