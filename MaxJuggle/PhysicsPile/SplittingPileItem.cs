// Programmer: Carl Childers
// Date: 11/8/2017
//
// Pile item that splits into parts on impact.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplittingPileItem : PhysicsPileItem {

	public float ImpactThreshold = 10;
	public float OutwardForce = 10;
	public float ImpactForceMultiplier = 10;
	public SplitChildPileItem[] Parts;
	public ParticleSystem SplitEffect;
	public AudioClip SplitSound;
	[Range(0, 1)]
	public float SplitSoundVolume = 1f;

	const float outForceOffsetMult = 0.5f;
	const float outForceDownNudge = 0.5f;

	bool hasSplit = false;


	protected override void Collided(Collision2D coll)
	{
		if (!hasSplit && coll.relativeVelocity.magnitude >= ImpactThreshold)
		{
			// Create parts, and destroy the original

			foreach (SplitChildPileItem part in Parts)
			{
				Vector3 offset = part.SplitOffset;
				offset = transform.rotation * offset;
				SplitChildPileItem partInstance = Instantiate(part, transform.position + offset, transform.rotation);
				partInstance.transform.SetParent(transform.parent);
				partInstance.MyType = MyType;
				partInstance.Initialize();

				Vector2 force = offset.normalized * OutwardForce;
				force += coll.contacts[0].normal * coll.relativeVelocity.magnitude * ImpactForceMultiplier;
				partInstance.MyRigidbody.AddForceAtPosition(force, transform.position + offset * outForceOffsetMult + Vector3.down * outForceDownNudge);

				partInstance.RefreshDepthPairs();
			}

			if (SplitEffect != null)
			{
				ParticleSystem splitEffectInstance = Instantiate(SplitEffect, transform.position, SplitEffect.transform.rotation * transform.rotation);
				splitEffectInstance.transform.SetParent(transform.parent);
			}

			if (SplitSound != null)
			{
				ExtraSoundPlayer.GetSoundPlayer().Play(SplitSound, transform.position, SplitSoundVolume, MyAudioSource.outputAudioMixerGroup);
			}

			hasSplit = true;
			Destroy(gameObject);
		}
	}
}
