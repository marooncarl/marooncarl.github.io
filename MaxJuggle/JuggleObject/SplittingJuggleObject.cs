// Programmer: Carl Childers
// Date: 10/21/2017
//
// Juggle item that splits into multiple smaller juggle objects when clicked enough times.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplittingJuggleObject : JuggleObject {

	public float OutwardForce = 10;
	public float UpwardForce = 10;
	public Sprite CrackedSprite;
	public ParticleSystem CrackEffect;
	public AudioClip CrackSound;
	[Range(0, 1)]
	public float CrackSoundVolume = 1;
	public AudioClip PreCrackSound;					// Played when it will crack the next time it is clicked.
	[Range(0, 1)]
	public float PreCrackSoundVolume = 1f;
	public SplitChildItem[] ChildItemTypes;

	SpriteRenderer mySprite;
	Collider2D myCollider;

	Sprite startingSprite;

	// Crack effect is not parented to this object, since this object will go inactive when the effect is needed.
	// Instead, the effect is cached, reused after the first time, and destroyed when this item is destroyed.
	ParticleSystem crackEffectInstance;

	const float outForceOffsetMult = 0.5f;
	const float outForceDownNudge = 0.5f;


	public override void Initialize(Tosser inTosser, JuggleObjectType inType)
	{
		base.Initialize(inTosser, inType);

		mySprite = GetComponent<SpriteRenderer>();
		myCollider = GetComponent<Collider2D>();
		startingSprite = mySprite.sprite;
	}

	public override void Restart(Vector2 startPos, bool wasTossed = true)
	{
		base.Restart(startPos, wasTossed);

		mySprite.enabled = true;
		myCollider.enabled = true;
		MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
		mySprite.sprite = startingSprite;
		if (myArrow != null)
		{
			myArrow.ChangeSprite(mySprite.sprite);
		}
	}

	public override void Destroyed()
	{
		if (crackEffectInstance != null)
		{
			Destroy(crackEffectInstance.gameObject);
		}
	}

	public override void WasJuggled(Juggler inJuggler, Vector2 hitPoint)
	{
		base.WasJuggled(inJuggler, hitPoint);

		if (JuggleCount >= GetJuggleLimit(inJuggler) - 2)
		{
			Split(inJuggler);
		}
		else if (JuggleCount >= GetJuggleLimit(inJuggler) - 3)
		{
			if (CrackedSprite != null)
			{
				mySprite.sprite = CrackedSprite;
			}

			if (myArrow != null)
			{
				myArrow.ChangeSprite(mySprite.sprite);
			}
			if (PreCrackSound != null)
			{
				MyAudioSource.PlayOneShot(PreCrackSound, PreCrackSoundVolume);
			}
		}
	}

	void Split(Juggler inJuggler)
	{
		LinkedList<SplitChildItem> childItems = new LinkedList<SplitChildItem>();

		bool playedCrackSound = false;
		foreach (SplitChildItem childType in ChildItemTypes)
		{
			SplitChildItem newChildItem = MyTosser.Toss(childType.ItemType, false) as SplitChildItem;
			if (newChildItem != null)
			{
				childItems.AddLast(newChildItem);

				Vector3 offset = (transform.rotation * new Vector3(newChildItem.SplitPosition.x, newChildItem.SplitPosition.y));
				newChildItem.transform.position = transform.position + offset;
				newChildItem.transform.rotation = transform.rotation;

				Vector2 force = offset.normalized * OutwardForce;
				force += Vector2.up * UpwardForce;
				newChildItem.ApplyForce( new Vector2(transform.position.x, transform.position.y) + -force.normalized, force.magnitude);

				if (!playedCrackSound)
				{
					// Since the parent item is going to deactivate, make the first child item play the crack sound
					newChildItem.MyAudioSource.PlayOneShot(CrackSound, CrackSoundVolume);
					playedCrackSound = true;
				}
			}
		}

		// Give each child object a reference to the other child objects
		foreach (SplitChildItem child1 in childItems)
		{
			child1.Siblings = new LinkedList<SplitChildItem>();

			foreach (SplitChildItem child2 in childItems)
			{
				if (child1 != child2)
				{
					child1.Siblings.AddLast(child2);
				}
			}
		}

		PlayCrackEffect();
		RemoveFromPlay();
		Disable();
	}

	public override Sprite GetStartingOffscreenSprite()
	{
		return mySprite.sprite;
	}

	void PlayCrackEffect()
	{
		if (CrackEffect != null)
		{
			if (crackEffectInstance == null)
			{
				// Create crack effect
				crackEffectInstance = Instantiate(CrackEffect, transform.position, transform.rotation * CrackEffect.transform.rotation) as ParticleSystem;
				if (!crackEffectInstance.isPlaying)
				{
					crackEffectInstance.Play();
				}
			}
			else
			{
				// Reuse the crack effect
				crackEffectInstance.transform.position = transform.position;
				crackEffectInstance.transform.rotation = transform.rotation * CrackEffect.transform.rotation;
				if (crackEffectInstance.isPlaying)
				{
					crackEffectInstance.Stop();
				}
				crackEffectInstance.Play();
			}
		}
	}

	public override void RecreateFromSaveData(ItemStateData inStateData, Juggler inJuggler)
	{
		base.RecreateFromSaveData(inStateData, inJuggler);

		if (JuggleCount >= GetJuggleLimit(inJuggler) - 3)
		{
			if (CrackedSprite != null)
			{
				mySprite.sprite = CrackedSprite;
			}
		}
	}
}
