// Programmer: Carl Childers
// Date: 9/7/2017
//
// Player for a juggling game.  Controls input during gameplay, and keeps track of stats,
// such as number of objects juggled and remaining lives.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juggler : MonoBehaviour {

	public string ClickButton = "Fire1";
	public string JuggleLayer = "Jugglable";
	public string HurtLayer = "Hurtful";
	public string LifeMeterName = "LifeMeter";
	public string PostGameSceneName;
	public string LifeReplenishAnim = "Replenish";
	public string LifeDropAnim = "Drop";
	public string LifeHurtAnim = "Hurt";
	public string BGGameStartTrigger = "JuggleStart";
	public float ClickDuration = 0.1f;
	public float ClickStartRadius = 0.01f;
	public float ClickEndRadius = 0.1f;
	public float JuggleForce = 6;
	public float FullJuggleForce = 30;

	// Delay for the life meter updating icon positions
	public float LifeAddMoveDelay = 0;
	public float LifeDropMoveDelay = 0.25f;
	public float LifeHurtMoveDelay = 0.75f;

	public int FullJuggleThreshold = 10;				// How many times an object needs to be clicked to fully juggle it
	public int StartingLives = 3;
	public int MaxLives = 5;
	public ParticleSystem ClickEffect;
	public ParticleSystem JuggleEffect;
	public ParticleSystem FullJuggleEffect;
	public ParticleSystem HurtEffect;
	public RecycledUIEffect FullTextEffect;

	public AudioClip JuggleSound;
	[Range(0, 2)]
	public float JuggleSoundVolume = 1;

	public AudioClip FullJuggleSound;
	[Range(0, 2)]
	public float FullJuggleSoundVolume = 1;

	public AudioClip FullJuggleJingle;
	[Range(0, 2)]
	public float FullJuggleJingleVolume = 1;

	public AudioClip HurtSound;
	[Range(0, 2)]
	public float HurtSoundVolume = 1;

	public AudioClip ExtraLifeSound;
	[Range(0, 2)]
	public float ExtraLifeSoundVolume = 1;

	#if UNITY_EDITOR
	public string GodModeEndButton = "EndGameGodMode";
	#endif

	GameTosser theTosser;
	public GameTosser TheTosser {
		get { return theTosser; }
	}

	int currentLives;
	public int CurrentLives {
		get { return currentLives; }
	}

	LinkedList<JuggleClick> inactiveClicks;
	public LinkedList<JuggleClick> InactiveClicks {
		get { return inactiveClicks; }
	}

	RecycledParticleEffect hurtEffectInstance;
	RecycledParticleEffect juggleEffectInstance;
	RecycledParticleEffect fullJuggleEffectInstance;
	RecycledUIEffect fullTextEffectInstance;

	AnimIconMeter myLifeMeter;

	bool inGameOver;
	public bool InGameOver {
		get { return inGameOver; }
	}

	bool beganPostGame;

	bool exitingGame;

	AudioSource myAudioSource;


	void Awake()
	{
		theTosser = GameObject.FindObjectOfType<GameTosser>();
		myAudioSource = GetComponent<AudioSource>();
	}

	void Start()
	{
		Restart();
	}

	public void Restart()
	{
		currentLives = StartingLives;
		enabled = true;
		inGameOver = false;
		beganPostGame = false;
		Pauser.GetPauser().PausingEnabled = true;

		if (myLifeMeter == null)
		{
			myLifeMeter = GameObject.Find(LifeMeterName).GetComponent<AnimIconMeter>();
			if (myLifeMeter.ActiveIconCount < currentLives)
			{
				myLifeMeter.AddIcon(LifeReplenishAnim, currentLives - myLifeMeter.ActiveIconCount);
			}
			else if (myLifeMeter.ActiveIconCount > currentLives)
			{
				myLifeMeter.RemoveIcon(LifeDropAnim, myLifeMeter.ActiveIconCount - currentLives, LifeDropMoveDelay);
			}
		}

		JuggleStatKeeper.GetStatKeeper().Restart();

		//print("Starting transition to juggling scene");
		//GameControl.GetGameControl().PlayBackgroundAnimation(BGGameStartTrigger);
	}

	void Update()
	{
		if (!inGameOver)
		{
			if (!Pauser.GetPauser().IsPaused && Pauser.GetPauser().GetTimeTillUnpause() <= 0)
			{
				CheckForClicks();
			}
		}

		#if UNITY_EDITOR
		if (theTosser.GodMode && Input.GetButtonDown(GodModeEndButton))
		{
			EndGame();
		}
		#endif
	}

	void CheckForClicks()
	{
		if (Input.GetButtonDown(ClickButton))
		{
			//print("Clicked on screen position: (" + Input.mousePosition.x + ", " + Input.mousePosition.y + ")");
			Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			clickPos.z = 0;
			Clicked(clickPos);
		}
	}
	
	// The player has clicked on the screen
	void Clicked(Vector3 clickPosition)
	{
		if (inactiveClicks == null)
		{
			inactiveClicks = new LinkedList<JuggleClick>();
		}

		if (inactiveClicks.Count > 0)
		{
			// Reuse an inactive click
			JuggleClick firstClick = inactiveClicks.First.Value;
			firstClick.Restart(clickPosition);
			inactiveClicks.RemoveFirst();
		}
		else
		{
			// Create a new click object
			GameObject clickObj = new GameObject("Juggle Click");
			JuggleClick newClick = clickObj.AddComponent<JuggleClick>();
			newClick.Initialize(this);
			newClick.Restart(clickPosition);
		}

		/*
		// Test: print screen point of click
		Vector3 screenPos = Camera.main.WorldToScreenPoint(clickPosition);
		screenPos -= new Vector3(Screen.width / 2, Screen.height / 2);
		print("Clicked: " + screenPos);
		*/
	}

	public void HurtByTouch(JuggleObject hurtingObj, Vector3 touchPosition)
	{
		if (!hurtingObj.Failed)
		{
			hurtingObj.Failed = true;
			hurtingObj.RemoveFromPlay();
			hurtingObj.BecomeImmune();
			RemoveLife(LifeHurtAnim, LifeHurtMoveDelay);
			hurtingObj.ForcefulDrop(this, new Vector2(touchPosition.x, touchPosition.y));
		}

		if (hurtEffectInstance == null)
		{
			hurtEffectInstance = new RecycledParticleEffect(HurtEffect);
		}
		hurtEffectInstance.Restart(touchPosition);

		if (HurtSound != null)
		{
			myAudioSource.PlayOneShot(HurtSound, HurtSoundVolume);
		}
	}

	// Passes a general type to the tosser (eg: water balloon) and a specific one to the stat keeper (eg: red water balloon)
	public void FullyJuggledObject(JuggleObjectType generalJuggledType, JuggleObjectType specificJuggledType)
	{
		theTosser.ObjectWasFullyJuggled(generalJuggledType);
		JuggleStatKeeper.GetStatKeeper().AddJuggledObject(specificJuggledType);

		// Refill lives to the starting value
		AddExtraLife(StartingLives - currentLives);
	}

	public void AddExtraLife(int amt = 1)
	{
		if (amt <= 0)
		{
			return;
		}

		currentLives = Mathf.Min(currentLives + amt, MaxLives);
		myLifeMeter.AddIcon(LifeReplenishAnim, currentLives - myLifeMeter.ActiveIconCount, LifeAddMoveDelay);

		if (ExtraLifeSound != null)
		{
			myAudioSource.PlayOneShot(ExtraLifeSound, ExtraLifeSoundVolume);
		}
	}

	public void PlayJuggleEffect(Vector3 effectPosition)
	{
		if (juggleEffectInstance == null)
		{
			juggleEffectInstance = new RecycledParticleEffect(JuggleEffect);
		}
		juggleEffectInstance.Restart(effectPosition);

		if (JuggleSound != null)
		{
			myAudioSource.PlayOneShot(JuggleSound, JuggleSoundVolume);
		}
	}

	public void PlayFullJuggleEffect(Vector3 effectPosition)
	{
		if (fullJuggleEffectInstance == null)
		{
			fullJuggleEffectInstance = new RecycledParticleEffect(FullJuggleEffect);
		}
		fullJuggleEffectInstance.Restart(effectPosition);

		if (FullTextEffect != null)
		{
			if (fullTextEffectInstance == null)
			{
				fullTextEffectInstance = Instantiate(FullTextEffect, effectPosition, Quaternion.identity);
			}
			fullTextEffectInstance.Restart(effectPosition);
		}

		if (FullJuggleSound != null)
		{
			myAudioSource.PlayOneShot(FullJuggleSound, FullJuggleSoundVolume);
		}
		if (FullJuggleJingle != null)
		{
			myAudioSource.PlayOneShot(FullJuggleJingle, FullJuggleJingleVolume);
		}
	}

	public void ObjectDropped(JuggleObject droppedObject)
	{
		RemoveLife(LifeDropAnim, LifeDropMoveDelay);
	}

	void RemoveLife(string anim, float meterMoveDelay = 0)
	{
		#if UNITY_EDITOR
		if (!theTosser.GodMode)
		{
		#endif

			currentLives--;

			myLifeMeter.RemoveIcon(anim, myLifeMeter.ActiveIconCount - currentLives, meterMoveDelay);

			if (currentLives <= 0)
			{
				EndGame();
			}

		#if UNITY_EDITOR
		}
		#endif
	}

	public void EndGame()
	{
		if (!inGameOver)
		{
			inGameOver = true;
			theTosser.GameEnded();
			Pauser.GetPauser().PausingEnabled = false;
		}
	}

	public void MoveToPostGame()
	{
		if (beganPostGame)
			return;

		beganPostGame = true;
		GameControl.GetGameControl().LoadScene(PostGameSceneName);
	}

	void OnApplicationQuit()
	{
		exitingGame = true;
	}

	void OnDestroy()
	{
		if (exitingGame)
			return;

		//print("Juggler was destroy");

		// Clean up click objects and effects

		if (inactiveClicks != null)
		{
			foreach (JuggleClick click in inactiveClicks)
			{
				Destroy(click.gameObject);
			}
			inactiveClicks.Clear();
		}

		JuggleClick[] remainingClicks = GameObject.FindObjectsOfType<JuggleClick>();
		foreach (JuggleClick click in remainingClicks)
		{
			Destroy(click.gameObject);
		}

		foreach (RecycledParticleEffect effect in new RecycledParticleEffect[] { hurtEffectInstance, juggleEffectInstance, fullJuggleEffectInstance })
		{
			if (effect != null)
			{
				Destroy(effect.MyInstance.gameObject);
			}
		}
	}

	public void RecreateFromBookmark(Bookmark inBookmark)
	{
		currentLives = inBookmark.GameState.LivesLeft;

		if (myLifeMeter.ActiveIconCount < currentLives)
		{
			myLifeMeter.AddIcon(LifeReplenishAnim, currentLives - myLifeMeter.ActiveIconCount);
		}
		else if (myLifeMeter.ActiveIconCount > currentLives)
		{
			myLifeMeter.RemoveIcon(LifeDropAnim, myLifeMeter.ActiveIconCount - currentLives, LifeDropMoveDelay, true);
		}

		// Make camera jump to target position immediately
		DragCameraToTarget camScript = GetComponent<DragCameraToTarget>();
		if (camScript != null)
		{
			camScript.DragImmediate();
		}
	}
}
