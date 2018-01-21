/// <summary>
/// Generic death
/// 
/// Programmer: Carl Childers
/// Date: 5/24/2015
/// 
/// Creates an effect prefab and destroys the game object.
/// </summary>

using UnityEngine;
using System.Collections;

public class GenericDeath : MonoBehaviour {

	public Transform DeathEffect;
	public AudioClip DeathSound;
	public bool UseCurrentRotation = false;


	// Messages

	void Died(Defense.EDamageType inDamageType) {
		if (DeathEffect != null) {
			Instantiate(DeathEffect, transform.position, (UseCurrentRotation == true ? transform.rotation : DeathEffect.rotation));
		}
		if (DeathSound != null) {
			AudioSource.PlayClipAtPoint(DeathSound, transform.position);
		}

		Destroy(this.gameObject);
	}
}
