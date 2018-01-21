// Programmer: Carl Childers
// Date: 9/23/2017
//
// An item in a physics pile.  Stops rigid body physics when still, and adjusts its sprite's sorting order so higher objects are further back.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsPileItem : MonoBehaviour {

	public string SettledLayer = "PileSettled";
	public float SettleTime = 0.05f;
	public float FallDelay = 0f;

	Rigidbody2D myRigidbody;
	public Rigidbody2D MyRigidbody {
		get { return myRigidbody; }
	}

	JuggleObjectType myType;
	public JuggleObjectType MyType {
		get { return myType; }
		set { myType = value; }
	}

	float stoppedCounter;
	float wakeCounter;
	Vector2 wakeVelocity;

	const float speedThreshold = 0.0025f;

	protected AudioSource myAudioSource;
	public AudioSource MyAudioSource {
		get { return myAudioSource; }
	}

	List<ObjectDepthPair> depthPairs;


	public virtual void Initialize()
	{
		myRigidbody = GetComponent<Rigidbody2D>();
		myAudioSource = GetComponent<AudioSource>();

		if (FallDelay > 0)
		{
			FlameSpreader spreader = GetComponentInChildren<FlameSpreader>();
			if (spreader != null)
			{
				spreader.enabled = false;
			}
		}

		if (myType.UseItemColor)
		{
			SpriteRenderer[] mySprites = GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spr in mySprites)
			{
				spr.color = myType.ItemColor;
			}
		}
	}

	public virtual void PostInit()
	{
		if (FallDelay > 0)
		{
			wakeVelocity = myRigidbody.velocity;
			myRigidbody.bodyType = RigidbodyType2D.Static;
			wakeCounter = FallDelay;
		}

		RefreshDepthPairs();
	}

	void Update()
	{
		UpdateDepths();

		UpdateWakeDelay();
		UpdatePileItem();
	}

	void UpdateWakeDelay()
	{
		if (wakeCounter > 0)
		{
			wakeCounter -= Time.deltaTime;
			if (wakeCounter <= 0)
			{
				// Start falling
				myRigidbody.bodyType = RigidbodyType2D.Dynamic;
				myRigidbody.velocity = wakeVelocity;

				FlameSpreader spreader = GetComponentInChildren<FlameSpreader>();
				if (spreader != null)
				{
					spreader.enabled = true;
				}
			}
		}
	}

	protected virtual void UpdatePileItem()
	{
		// Become static when no longer moving much
		// Has to be stopped for a short time, to try and avoid floating situations
		if (myRigidbody.velocity.sqrMagnitude < speedThreshold)
		{
			stoppedCounter += Time.deltaTime;
			if (stoppedCounter >= SettleTime)
			{
				myRigidbody.bodyType = RigidbodyType2D.Static;
				gameObject.layer = LayerMask.NameToLayer(SettledLayer);
				enabled = false;
			}
		}
		else
		{
			stoppedCounter = 0;
		}
	}

	public virtual void SetInitialRotation(JuggleObjectType inType)
	{
		transform.rotation = Quaternion.Euler(0, 0, Random.Range(inType.PiledMinRotation, inType.PiledMaxRotation));
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		Collided(coll);
	}

	protected virtual void Collided(Collision2D coll)
	{
		// Play impact sound
		if (myType.ImpactSound != null)
		{
			float volumeMult = Mathf.Clamp(coll.relativeVelocity.magnitude / Mathf.Max(myType.ImpactMaxSpeed, 0.1f), 0, 1);
			myAudioSource.PlayOneShot(myType.ImpactSound, myType.ImpactSoundVolume * volumeMult);
		}
	}

	// Should be called when a new sprite or particle system is added, to set them up with the depth pairs.
	public void RefreshDepthPairs()
	{
		if (depthPairs == null)
		{
			depthPairs = new List<ObjectDepthPair>();
		}

		SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spr in sprites)
		{
			if (depthPairs.Find( i => i is SpriteDepthPair && ((SpriteDepthPair)i).Sprite == spr) == null)
			{
				SpriteDepthPair newPair = new SpriteDepthPair();
				newPair.Sprite = spr;
				newPair.Depth = spr.sortingOrder;
				depthPairs.Add(newPair);
			}
		}

		ParticleSystemRenderer[] particleRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
		foreach (ParticleSystemRenderer psr in particleRenderers)
		{
			if (depthPairs.Find( i => i is ParticleDepthPair && ((ParticleDepthPair)i).Particles == psr) == null)
			{
				ParticleDepthPair newPair = new ParticleDepthPair();
				newPair.Particles = psr;
				newPair.Depth = psr.sortingOrder;
				depthPairs.Add(newPair);
			}
		}
	}

	void UpdateDepths()
	{
		foreach (ObjectDepthPair p in depthPairs)
		{
			p.AdjustDepth( Mathf.FloorToInt(-transform.position.y * 10) );
		}
	}
}

// The following classes associate a sprite or particle system with a sorting order/ depth, so they can all be updated easily.

public class ObjectDepthPair {

	public int Depth;

	public virtual void AdjustDepth(int inBaseDepth)
	{
	}
}

public class SpriteDepthPair : ObjectDepthPair {

	public SpriteRenderer Sprite;

	public override void AdjustDepth(int inBaseDepth)
	{
		if (Sprite != null)
		{
			Sprite.sortingOrder = inBaseDepth + Depth;
		}
	}
}

public class ParticleDepthPair : ObjectDepthPair {
	
	public ParticleSystemRenderer Particles;

	public override void AdjustDepth(int inBaseDepth)
	{
		if (Particles != null)
		{
			Particles.sortingOrder = inBaseDepth + Depth;
		}
	}
}