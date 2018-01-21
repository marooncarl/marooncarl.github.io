/// <summary>
/// Player hit effects
/// 
/// Programmer: Carl Childers
/// Date: 9/8/2015
/// 
/// Plays damage effects for the player character.
/// </summary>

using UnityEngine;
using System.Collections;

public class PlayerHitEffects : MonoBehaviour {

	public Transform NormalHitEffect;
	public Transform FireHitEffect;
	public float BaseShakeMagnitude = 0.05f;
	public float MaxShakeMagnitude = 0.1f;
	public AudioClip[] VoiceClips; // Picks a random voice clip
	public AudioClip ImpactSound;
	public float TimeBetweenVoiceClips = 1.0f;
	public float FireEffectDuration = 2.0f;

	float LastVoiceClipTime;
	ParticleSystem FireHitComponent;


	void StopFireEffects() {
		if (FireHitComponent != null) {
			FireHitComponent.Stop();
		}
	}

	// damage effects message
	void PlayDamageEffects(Damage inDmg)
	{
		if (NormalHitEffect != null) {
			Instantiate(NormalHitEffect, transform.position, NormalHitEffect.rotation);
		}

		if (inDmg.Type == Defense.EDamageType.DT_Fire) {
			if (FireHitComponent == null && FireHitEffect != null) {
				Transform fireTransform = (Transform)Instantiate(FireHitEffect, transform.position, transform.rotation);
				fireTransform.parent = transform;
				FireHitComponent = fireTransform.GetComponent<ParticleSystem>();
			}

			if (FireHitComponent != null) {
				FireHitComponent.Play();
			}

			Invoke("StopFireEffects", FireEffectDuration);
		}

		float shakeMag = Mathf.Min(BaseShakeMagnitude *  inDmg.Amount, MaxShakeMagnitude);
		Camera.main.SendMessage("SetShakeMagnitude", shakeMag, SendMessageOptions.DontRequireReceiver);

		if (VoiceClips.Length > 0 && Time.time - LastVoiceClipTime > TimeBetweenVoiceClips) {
			AudioSource.PlayClipAtPoint(VoiceClips[ Random.Range(0, VoiceClips.Length) ], transform.position);
			LastVoiceClipTime = Time.time;
		}

		if (ImpactSound != null) {
			AudioSource.PlayClipAtPoint(ImpactSound, transform.position);
		}
	}
}
