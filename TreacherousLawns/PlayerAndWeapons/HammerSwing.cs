using UnityEngine;
using System.Collections;

public class HammerSwing : MonoBehaviour {

	public string HammerDownProp = "IsHammerDown";
	public float HammerDownTime = 1.0f;
	public float SwingDamage = 10;
	public Defense.EDamageType SwingDamageType = Defense.EDamageType.DT_Normal;
	public AudioClip HitSound;
	public Transform HitEffect;
	public Vector2 HitEffectOffset;
	public string FlowerTag = "Flowers";
	public string CutTag = "CutGrass";
	public Transform CutEffect;
	public float FlowerDistanceThreshold = 0.8f;

	Animator MyAnimator;
	bool IsHammerDown;


	void Awake() {
		MyAnimator = GetComponent<Animator>();
		IsHammerDown = false;
	}

	void RaiseHammer() {
		IsHammerDown = false;
		if (MyAnimator != null) {
			MyAnimator.SetBool(HammerDownProp, IsHammerDown);
		}
		SendMessage("RefreshAttack");
	}

	// Messages
	void StartFire() {
		if (IsHammerDown || !enabled) {
			return;
		}

		IsHammerDown = true;
		if (MyAnimator != null) {
			MyAnimator.SetBool(HammerDownProp, IsHammerDown);
		}
		Invoke("RaiseHammer", HammerDownTime);
	}

	void TurnOff() {
		CancelInvoke("RaiseHammer");
		RaiseHammer();
	}

	void Disable() {
		enabled = false;
	}

	void HitTarget(Transform inTarget) {
		Damage dmg = new Damage(SwingDamage, SwingDamageType, false);
		inTarget.BroadcastMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);


		if (inTarget.gameObject.tag == FlowerTag && (transform.position - inTarget.position).magnitude < FlowerDistanceThreshold)
		{
			//print("Hit flowers with hammer");
			//print("Hammer - flower distance: " + (transform.position - inTarget.position).magnitude);

			LawnScoreKeeper keeperScript = FindObjectOfType<LawnScoreKeeper>();
			if (keeperScript != null) {
				keeperScript.transform.SendMessage("FlowerMowed");
			}

			inTarget.gameObject.tag = CutTag;
			inTarget.SendMessage("Mowed");
			if (CutEffect != null) {
				Instantiate(CutEffect, inTarget.position, CutEffect.rotation);
			}
		}

	}

	// Animation Events

	void HammerLand() {
		//print("Hammer Land");
		Vector3 hitLoc = transform.position;
		float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
		
		hitLoc.x += Mathf.Cos(angle) * HitEffectOffset.x - Mathf.Sin(angle) * HitEffectOffset.y;
		hitLoc.y += Mathf.Sin(angle) * HitEffectOffset.x + Mathf.Cos(angle) * HitEffectOffset.y;
		
		if (HitEffect != null) {
			Instantiate(HitEffect, hitLoc, Quaternion.identity);
		}
		if (HitSound != null) {
			AudioSource.PlayClipAtPoint(HitSound, hitLoc);
		}
	}
}
