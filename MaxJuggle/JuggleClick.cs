// Programmer: Carl Childers
// Date: 9/7/2017
//
// Created or reused by the Juggler (player) when the player clicks somewhere.
// Can last more than one frame.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleClick : MonoBehaviour {

	Juggler myJuggler;
	ParticleSystem myEffect;

	Vector2 position2D;

	float lifetime;
	int hitLayerMask;

	Collider2D[] hits;

	bool juggledSomething = false;


	public void Initialize(Juggler inJuggler)
	{
		myJuggler = inJuggler;
		hitLayerMask = LayerMask.GetMask( new string[] { myJuggler.JuggleLayer, myJuggler.HurtLayer } );
		hits = new Collider2D[inJuggler.TheTosser.MaxObjectsInPlay];
		myEffect = Instantiate(inJuggler.ClickEffect, transform.position, inJuggler.ClickEffect.transform.rotation) as ParticleSystem;
		myEffect.transform.SetParent(transform);
	}

	public void Restart(Vector3 clickPos)
	{
		lifetime = myJuggler.ClickDuration;
		enabled = true;
		juggledSomething = false;
		transform.position = clickPos;
		position2D = new Vector2(clickPos.x, clickPos.y);

		for (int i = 0; i < hits.Length; ++i)
		{
			hits[i] = null;
		}

		if (myEffect.isPlaying)
		{
			myEffect.Stop();
		}
		myEffect.Play();
	}

	void Update()
	{
		CheckForHits();

		lifetime -= Time.deltaTime;
		if (lifetime <= 0)
		{
			ClickEnded();
		}
	}

	void CheckForHits()
	{
		float clickRadius = GetClickRadius();

		if (Physics2D.OverlapCircleNonAlloc(position2D, clickRadius, hits, hitLayerMask) > 0)
		{
			JuggleObject hurtObj = null;

			foreach (Collider2D hit in hits)
			{
				if (hit == null)
					continue;

				JuggleObject juggleObj = hit.transform.root.gameObject.GetComponent<JuggleObject>();
				if (juggleObj == null || juggleObj.IsImmune() || juggleObj.Failed)
				{
					continue;
				}

				if (hit.gameObject.layer == LayerMask.NameToLayer(myJuggler.HurtLayer))
				{
					if (hurtObj == null)
					{
						hurtObj = juggleObj;
					}
				}
				else if (hit.gameObject.layer == LayerMask.NameToLayer(myJuggler.JuggleLayer))
				{
					// Juggle the object
					Vector2 hitPoint = new Vector2(transform.position.x, transform.position.y);
					juggleObj.WasJuggled(myJuggler, hitPoint);
					juggledSomething = true;
				}
			}

			// Only damage the player if didn't juggle something with the same click, and only one object per click
			if (!juggledSomething && hurtObj != null)
			{
				myJuggler.HurtByTouch(hurtObj, transform.position);
				ClickEnded();
			}

			// Clear the hits array
			for (int i = 0; i < hits.Length; ++i)
			{
				hits[i] = null;
			}
		}
	}

	void ClickEnded()
	{
		enabled = false;
		myJuggler.InactiveClicks.AddLast(this);
	}

	float GetClickRadius()
	{
		float alpha = 0;
		if (myJuggler.ClickDuration > 0)
		{
			alpha = 1f - (lifetime / myJuggler.ClickDuration);
		}

		return Mathf.Lerp(myJuggler.ClickStartRadius, myJuggler.ClickEndRadius, alpha);
	}
}
