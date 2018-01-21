/// <summary>
/// Bomb explosion hit
/// 
/// Programmer: Carl Childers
/// Date: 6/7/2015
/// 
/// Lasts for only one frame and damages anything it touches.  Disables instead of destroying so there
/// can be a particle effect attached, so another script needs to destroy this game object.
/// 
/// Also cuts grass with the explosion.
/// </summary>

using UnityEngine;
using System.Collections;

public class BombExplosionHit : MonoBehaviour {

	public float DamageAmount;
	public Defense.EDamageType DamageType = Defense.EDamageType.DT_Normal;

	// grass cutting properties
	public bool CutsGrass = true;
	public int[] GrassLayers;
	public float CheckDistance = 0.1f;
	public string GrassTag = "Grass";
	public string FlowerTag = "Flowers";
	public string CutTag = "CutGrass";
	public Transform CutEffect;

	int disableCounter;

	Transform MyScoreKeeper;


	void Start() {
		disableCounter = 2;

		if (CutsGrass) {
			LawnScoreKeeper keeperScript = FindObjectOfType<LawnScoreKeeper>();
			if (keeperScript != null) {
				MyScoreKeeper = keeperScript.transform;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (CutsGrass) {
			SearchForGrass();
		}

		disableCounter--;
		if (disableCounter <= 0) {
			enabled = false;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (!enabled) {
			return;
		}
		//print("Bomb hit - other is " + other.gameObject);
		Damage dmg = new Damage(DamageAmount, DamageType, false);
		other.BroadcastMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);
	}

	void SearchForGrass() {
		if (GrassLayers.Length == 0) {
			return;
		}

		int layerMask = 0;
		foreach (int i in GrassLayers) {
			layerMask = layerMask | (1 << i);
		}

		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), CheckDistance,
		                                                       layerMask);
		foreach (Collider2D c in hitColliders) {
			if (c.transform.tag == GrassTag || c.transform.tag == FlowerTag) {
				HandleCutTile(c.transform.tag);
				c.transform.gameObject.tag = CutTag;
				c.transform.SendMessage ("Mowed");
				if (CutEffect != null) {
					Instantiate (CutEffect, c.transform.position, CutEffect.rotation);
				}
			}
		}
	}

	void HandleCutTile(string inTag) {
		if (inTag == GrassTag) {
			// increase cut tile count
			if (MyScoreKeeper != null) {
				MyScoreKeeper.SendMessage ("GrassMowed");
			}
		} else if (inTag == FlowerTag) {
			// give penalty
			if (MyScoreKeeper != null) {
				MyScoreKeeper.SendMessage ("FlowerMowed");
			}
		}
	}
}
