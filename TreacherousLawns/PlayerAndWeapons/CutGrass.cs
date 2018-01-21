/// <summary>
/// Cut grass
/// 
/// Programmer: Carl Childers
/// Date: 4/18/2015
/// 
/// Destroys grass objects when overlapping them just right.  Checks if the center of the
/// grass tile is close enough to the mower's center, with some optional offset for the mower's center.
/// </summary>

using UnityEngine;
using System.Collections;

public class CutGrass : MonoBehaviour {

	public Vector3 CenterOffset;
	public float CheckDistance = 0.1f;
	public string GrassTag = "Grass";
	public string FlowerTag = "Flowers";
	public string CutTag = "CutGrass";
	public Transform CutEffect;

	Transform MyScoreKeeper;

	void Start() {
		LawnScoreKeeper keeperScript = FindObjectOfType<LawnScoreKeeper>();
		if (keeperScript != null) {
			MyScoreKeeper = keeperScript.transform;
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == GrassTag || other.tag == FlowerTag) {
			Vector3 myCenter = transform.position + (transform.rotation * CenterOffset);
			Vector3 delta = other.transform.position - myCenter;
			//if (Vector3.SqrMagnitude(delta) < CheckDistance * CheckDistance) {
			if (Mathf.Abs(delta.x) < CheckDistance && Mathf.Abs(delta.y) < CheckDistance) {
				HandleCutTile (other.tag);
				other.gameObject.tag = CutTag;
				other.transform.SendMessage ("Mowed");
				if (CutEffect != null) {
					Instantiate (CutEffect, transform.position, CutEffect.rotation);
				}
				BroadcastMessage("CueAudio", SendMessageOptions.DontRequireReceiver);
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
