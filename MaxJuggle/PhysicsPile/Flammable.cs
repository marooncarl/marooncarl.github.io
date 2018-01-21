// Programmer: Carl Childers
// Date: 9/26/2017
//
// Catches fire when touched by a flame spreader.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour {

	public ParticleSystem FireParticles;
	public Sprite BurntSprite;
	public float FlameRadius = 1;
	public float FireDuration = 5;
	public float BurntDelay = 2f;
	public string FlameSpreadLayer = "FlameSpreading";

	bool onFire = false;
	public bool OnFire {
		get { return onFire; }
	}

	GameObject flameSpreaderObj;
	ParticleSystem fireParticleInstance;

	float burnOutCounter;


	public void CatchFire()
	{
		if (onFire)
		{
			return;
		}

		onFire = true;
		burnOutCounter = FireDuration;

		if (BurntDelay > 0)
		{
			Invoke("BecomeBurnt", BurntDelay);
		}
		else
		{
			BecomeBurnt();
		}

		if (FireParticles != null)
		{
			fireParticleInstance = Instantiate(FireParticles, transform.position, FireParticles.transform.rotation);
			fireParticleInstance.transform.SetParent(transform);
			if (!fireParticleInstance.isPlaying)
			{
				fireParticleInstance.Play();
			}

			// Particle effects were added, so the physics pile behaviour needs to add it to adjust its sorting order
			PhysicsPileItem pileItemBehaviour = GetComponent<PhysicsPileItem>();
			if (pileItemBehaviour != null)
			{
				pileItemBehaviour.RefreshDepthPairs();
			}
		}

		// Create a new flame spreader for even more fire
		flameSpreaderObj = new GameObject("FlameSpreader");
		flameSpreaderObj.transform.SetParent(transform);
		flameSpreaderObj.transform.localPosition = Vector3.zero;
		flameSpreaderObj.transform.localRotation = Quaternion.identity;
		flameSpreaderObj.transform.localScale = Vector3.one;
		flameSpreaderObj.layer = LayerMask.NameToLayer(FlameSpreadLayer);
		CircleCollider2D flameCollider = flameSpreaderObj.AddComponent<CircleCollider2D>();
		flameCollider.radius = FlameRadius;
		flameCollider.isTrigger = true;
		flameSpreaderObj.AddComponent<FlameSpreader>();
	}

	void Update()
	{
		if (onFire && burnOutCounter > 0)
		{
			burnOutCounter -= Time.deltaTime;
			if (burnOutCounter <= 0)
			{
				// finishing burning; flames go out and can no longer spread fire
				if (fireParticleInstance != null)
				{
					Destroy(fireParticleInstance.gameObject);
				}
				if (flameSpreaderObj != null)
				{
					Destroy(flameSpreaderObj);
				}
				enabled = false;
			}
		}
	}

	void BecomeBurnt()
	{
		if (BurntSprite == null)
			return;
		
		SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
		if (mySprite != null)
		{
			mySprite.sprite = BurntSprite;
		}
	}
}
