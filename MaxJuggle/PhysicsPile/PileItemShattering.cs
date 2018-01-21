// Programmer: Carl Childers
// Date: 10/25/2017
//
// Physics Pile Item that breaks and changes collision when hitting the ground / another item.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileItemShattering : PhysicsPileItem {

	public Sprite ShatteredSprite;
	public string ShatteredCollision;
	public ParticleSystem ShatterEffect;
	public AudioClip ShatterSound;
	[Range(0, 1)]
	public float ShatterSoundVolume = 1;

	bool shattered = false;


	protected override void Collided(Collision2D coll)
	{
		if (!shattered)
		{
			shattered = true;

			if (ShatterSound != null)
			{
				myAudioSource.PlayOneShot(ShatterSound, ShatterSoundVolume);
			}

			if (ShatterEffect != null)
			{
				ParticleSystem shatterEffectInstance = Instantiate(ShatterEffect, transform.position, transform.rotation) as ParticleSystem;
				shatterEffectInstance.transform.SetParent(transform.parent);
				if (!shatterEffectInstance.isPlaying)
				{
					shatterEffectInstance.Play();
				}
			}

			Collider2D[] myColliders = GetComponentsInChildren<Collider2D>();
			foreach (Collider2D collider in myColliders)
			{
				collider.enabled = false;
			}
			Transform newColliderTransform = transform.Find(ShatteredCollision);
			if (newColliderTransform != null)
			{
				newColliderTransform.GetComponent<Collider2D>().enabled = true;
			}

			SpriteRenderer mySprite = GetComponentInChildren<SpriteRenderer>();
			if (mySprite != null)
			{
				mySprite.sprite = ShatteredSprite;
			}

			RefreshDepthPairs();
		}
		else
		{
			base.Collided(coll);
		}
	}
}
