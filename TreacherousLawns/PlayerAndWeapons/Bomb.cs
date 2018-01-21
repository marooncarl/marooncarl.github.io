/// <summary>
/// Bomb
/// 
/// Programmer: Carl Childers
/// Date: 5/30/2015
/// 
/// Explodes after some time, and advances an animation parameter so it can blink faster or something
/// </summary>


using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {

	public float DetonateTime = 5.0f;
	public string AnimParamName = "BombState";
	public Transform ExplodePrefab;
	public bool CanAnchor = true;
	public string AnchorTag = "Bombable";
	public float AnchorThreshold = 4.0f; // velocity threshold; must be slower than this to anchor

	float DetonateCounter;
	Animator MyAnimator;

	Transform AnchorObject; // something the bomb has latched on to


	void Awake() {
		MyAnimator = GetComponent<Animator>();

		DetonateCounter = 0;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimParamName, GetBombState());
		}
	}

	void Update() {
		DetonateCounter += Time.deltaTime;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimParamName, GetBombState());
		}

		if (AnchorObject != null) {
			float dist = (AnchorObject.position - transform.position).magnitude;
			if (dist > 0) {
				if (dist <= 0.02f) {
					transform.position = AnchorObject.position;
				} else {
					transform.position = AnchorObject.position + (transform.position - AnchorObject.position).normalized * (dist / 1.125f);
				}
			}
		}

		if (DetonateCounter >= DetonateTime) {
			// Boom!

			if (AnchorObject != null) {
				AnchorObject.BroadcastMessage("Bombed", SendMessageOptions.DontRequireReceiver);
			}

			if (ExplodePrefab != null) {
				Instantiate(ExplodePrefab, transform.position, transform.rotation);
			}
			Destroy(this.gameObject);
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (CanAnchor && AnchorObject == null && other.tag == AnchorTag) {
			if (rigidbody2D != null && rigidbody2D.velocity.magnitude < AnchorThreshold) {
				AnchorObject = other.transform.root;
				if (rigidbody2D != null) {
					rigidbody2D.isKinematic = true;
					rigidbody2D.velocity = Vector2.zero;
				}
				//transform.position = AnchorObject.position;
				AnchorObject.BroadcastMessage("BombAnchored", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	int GetBombState() {
		if (DetonateCounter >= DetonateTime * 0.85f) {
			return 3;
		}
		if (DetonateCounter >= DetonateTime * 0.5f) {
			return 2;
		}
		return 1;
	}
}
