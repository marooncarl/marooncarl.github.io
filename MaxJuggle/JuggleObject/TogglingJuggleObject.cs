// Programmer: Carl Childers
// Date: 10/18/2017
//
// Juggle item that toggles one of its colliders between safe and not safe to touch.
// Also changes its sprite to reflect this.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglingJuggleObject : JuggleObject {

	public float StartSafeDuration = 1.5f;
	public float SafeDuration = 3.0f;
	public float UnsafeDuration = 0.5f;
	public float WarningDuration = 0.5f;
	public bool StartSafe = true;
	public Sprite WarningSprite;
	public Sprite UnsafeSprite;
	public string ToggleColliderName;
	public string SafeLayer = "Jugglable";
	public string UnsafeLayer = "Hurtful";
	public string SafeTrigger;
	public string WarningTrigger;
	public string UnsafeTrigger;
	public string StartUnsafeTrigger;
	public string RestartTrigger;
	public AudioClip TurnOnSound;
	[Range(0, 1)]
	public float TurnOnVolume = 1f;
	public float TurnOnSoundBeforeTime = 0.1f;				// Time before the turn on animation for the sound to play.
	public AudioClip WarningSound;
	[Range(0, 1)]
	public float WarningSoundVolume = 1f;

	SpriteRenderer mySprite;
	Collider2D toggleCollider;
	Animator myAnimator;

	Sprite safeSprite;
	bool isSafe;
	bool warningOn;
	bool playedTurnOnSound;

	float toggleCounter;


	public override void Initialize(Tosser inTosser, JuggleObjectType inType)
	{
		base.Initialize(inTosser, inType);

		mySprite = GetComponentInChildren<SpriteRenderer>();
		myAnimator = GetComponent<Animator>();
		safeSprite = mySprite.sprite;
		toggleCollider = transform.Find(ToggleColliderName).GetComponent<Collider2D>();
	}

	public override void Restart(Vector2 startPos, bool wasTossed = true)
	{
		base.Restart(startPos, wasTossed);

		SetSafe(StartSafe, StartSafe);
		warningOn = false;
		playedTurnOnSound = false;

		myAnimator.SetTrigger(RestartTrigger);
	}

	protected override void UpdateJuggleObject()
	{
		if ((fullyJuggled || Failed) && isSafe && !warningOn)
		{
			return;
		}

		if (Time.timeScale > 0)
		{
			toggleCounter -= Time.deltaTime;
			if (toggleCounter <= 0)
			{
				SetSafe(!isSafe);
				if (!playedTurnOnSound && TurnOnSoundBeforeTime == 0)
				{
					if (TurnOnSound != null)
					{
						MyAudioSource.PlayOneShot(TurnOnSound, TurnOnVolume);
					}
				}
			}
			else if (isSafe && toggleCounter <= WarningDuration && !warningOn)
			{
				SetWarning();
			}
			if (isSafe && !playedTurnOnSound && toggleCounter <= TurnOnSoundBeforeTime)
			{
				playedTurnOnSound = true;
				if (TurnOnSound != null)
				{
					MyAudioSource.PlayOneShot(TurnOnSound, TurnOnVolume);
				}
			}
		}
	}

	void SetSafe(bool isNowSafe, bool startingSafe = false)
	{
		isSafe = isNowSafe;
		warningOn = false;
		myAnimator.SetTrigger( (isSafe ? SafeTrigger : UnsafeTrigger) );
		if (myArrow != null)
		{
			myArrow.ChangeSprite( (isSafe ? safeSprite : UnsafeSprite) );
		}

		string newLayerName = (isSafe ? SafeLayer : UnsafeLayer);
		toggleCollider.gameObject.layer = LayerMask.NameToLayer(newLayerName);

		if (!startingSafe || !isNowSafe)
		{
			toggleCounter = (isSafe ? SafeDuration : UnsafeDuration);
		}
		else
		{
			toggleCounter = StartSafeDuration;
		}

		if (isNowSafe)
		{
			// Refresh turn on sound
			playedTurnOnSound = false;
		}
	}

	void SetWarning(bool silent = false)
	{
		warningOn = true;
		myAnimator.SetTrigger(WarningTrigger);
		if (myArrow != null)
		{
			myArrow.ChangeSprite(WarningSprite);
		}

		if (!silent && WarningSound != null)
		{
			MyAudioSource.PlayOneShot(WarningSound, WarningSoundVolume);
		}
	}

	protected override Sprite GetTrailSprite()
	{
		return mySprite.sprite;
	}

	public override Sprite GetStartingOffscreenSprite()
	{
		if (warningOn)
		{
			return WarningSprite;
		}
		else
		{
			return (isSafe ? safeSprite : UnsafeSprite);
		}
	}

	public override ItemStateData GetStateData()
	{
		ItemStateData myState = base.GetStateData();

		myState.CustomProps = new Dictionary<string, float>(2);

		myState.CustomProps.Add("togCtr", toggleCounter);
		myState.CustomProps.Add("sf", (isSafe ? 1 : 0));

		return myState;
	}

	public override void RecreateFromSaveData(ItemStateData inStateData, Juggler inJuggler)
	{
		base.RecreateFromSaveData(inStateData, inJuggler);

		SetSafe( (inStateData.CustomProps["sf"] > 0) );
		toggleCounter = inStateData.CustomProps["togCtr"];
		if (isSafe && toggleCounter <= WarningDuration)
		{
			myAnimator.ResetTrigger(SafeTrigger);
			SetWarning(true);
		}

		if (!isSafe)
		{
			myAnimator.ResetTrigger(UnsafeTrigger);
			myAnimator.SetTrigger(StartUnsafeTrigger);
		}
	}
}
