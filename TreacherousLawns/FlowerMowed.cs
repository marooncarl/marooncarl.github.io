/// <summary>
/// Flower mowed
/// 
/// Programmer: Carl Childers
/// Date: 6/21/2015
/// 
/// Specific handling for mowing a flower tile.
/// </summary>


using UnityEngine;
using System.Collections;

public class FlowerMowed : MonoBehaviour {

	public AudioClip MowedSound;
	public Transform DeathEffect;

	// Messages
	void Mowed() {
		SendMessage("ChangeTexture");

		if (MowedSound != null) {
			AudioSource.PlayClipAtPoint(MowedSound, transform.position);
		}
		if (DeathEffect != null) {
			Instantiate(DeathEffect, transform.position, Quaternion.identity);
		}
	}
}
