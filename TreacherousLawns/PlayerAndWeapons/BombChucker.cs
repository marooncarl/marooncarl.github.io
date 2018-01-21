/// <summary>
/// Bomb chucker
/// 
/// Programmer: Carl Childers
/// Date: 6/6/2015
/// 
/// Fires bombs.  Can only have one bomb out at a time.
/// </summary>

using UnityEngine;
using System.Collections;

public class BombChucker : MonoBehaviour {

	public Transform BombPrefab;
	public float LaunchForce = 100;
	public AudioClip ThrowSound;

	Transform CurrentBomb;
	SpriteRenderer MySprite;


	void Awake() {
		MySprite = GetComponent<SpriteRenderer>();
	}

	void Update() {
		if (MySprite != null) {
			if (CurrentBomb == null && !MySprite.enabled) {
				MySprite.enabled = true;
			} else if (CurrentBomb != null && MySprite.enabled) {
				MySprite.enabled = false;
			}
		}
	}

	// Messages

	void StartFire() {
		if (!enabled) {
			return;
		}

		if (CurrentBomb != null) {
			return;
		}

		CurrentBomb = (Transform)Instantiate(BombPrefab, transform.position, transform.rotation);
		if (CurrentBomb.rigidbody2D != null) {
			Vector3 force3D = transform.rotation * (Vector3.up * LaunchForce);
			CurrentBomb.rigidbody2D.AddForce( new Vector2(force3D.x, force3D.y) );
		}
		if (ThrowSound != null) {
			AudioSource.PlayClipAtPoint(ThrowSound, CurrentBomb.position);
		}
	}

	void Disable() {
		enabled = false;
	}
}
