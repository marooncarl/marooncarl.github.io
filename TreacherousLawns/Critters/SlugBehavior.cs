/// <summary>
/// Slug behavior
/// 
/// Programmer: Carl Childers
/// Date: 7/11/2015
/// 
/// Controls other scripts for slug behaviour.
/// </summary>

using UnityEngine;
using System.Collections;

public class SlugBehavior : MonoBehaviour {

	//public string PlayerTag = "Player";
	public string VehicleTag = "Vehicle";
	//public string RestingParameter = "IsResting";
	//public float PanicDistance = 2.0f;
	//public float SafeDistance = 6.0f;
	public float SneezeChance = 0.5f;
	public float SneezeStartup = 1.0f;
	public float SneezeDuration = 0.5f;

	//Animator animator;

	//Wander2D WanderScript;
	//Retreat2D RetreatScript;
	//DamageArea DamageScript;
	//AudioSource MyAudio;
	//bool IsRetreating;


	/*
	void Awake() {
		//WanderScript = GetComponent<Wander2D>();
		//RetreatScript = GetComponent<Retreat2D>();
		//DamageScript = GetComponentInChildren<DamageArea>();
		//MyAudio = GetComponent<AudioSource>();
		//animator = GetComponent<Animator>();
		//IsRetreating = false;
	}
	*/

	/*
	// Update is called once per frame
	void Update () {
		if (!IsRetreating) {
			GameObject[] vehs = GameObject.FindGameObjectsWithTag(VehicleTag);
			foreach (GameObject v in vehs) {
				float distSq = (v.transform.position - transform.position).sqrMagnitude;
				if (distSq < PanicDistance * PanicDistance) {
					// Start retreating
					IsRetreating = true;
					if (RetreatScript != null) {
						RetreatScript.enabled = true;
						RetreatScript.Target = v.transform;
					}
					if (WanderScript != null) {
						WanderScript.enabled = false;
					}
					// Turn on attacking, if available
					SendMessage("TurnOn", SendMessageOptions.DontRequireReceiver);
					if (animator != null) {
						animator.SetBool(RestingParameter, false);
					}
					break;
				}
			}
		} else {
			GameObject[] vehs = GameObject.FindGameObjectsWithTag(VehicleTag);
			foreach (GameObject v in vehs) {
				float distSq = (v.transform.position - transform.position).sqrMagnitude;
				if (distSq > SafeDistance * SafeDistance) {
					// No need to retreat
					IsRetreating = false;
					if (RetreatScript != null) {
						RetreatScript.enabled = false;
					}
					if (WanderScript != null) {
						WanderScript.enabled = true;
						WanderScript.StartMoving();
					}
					// Turn off attacking
					SendMessage("TurnOff", SendMessageOptions.DontRequireReceiver);
					break;
				}
			}
		}
	}
	*/

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.transform != null && coll.transform.tag == VehicleTag) {
			Damage dmg = new Damage(20, Defense.EDamageType.DT_Normal, false);
			SendMessage("TakeDamage", dmg);
		}
	}

	void Sneeze() {
		/*
		if (DamageScript != null) {
			DamageScript.enabled = true;
		}

		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem p in ps) {
			p.Play();
		}

		if (MyAudio != null) {
			MyAudio.Play();
		}
		*/
		SendMessage("StartFire");

		Invoke("StopSneeze", SneezeDuration);
	}

	void StopSneeze() {
		/*
		if (DamageScript != null) {
			DamageScript.enabled = true;
		}

		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem p in ps) {
			p.Stop();
		}

		if (MyAudio != null) {
			MyAudio.Stop();
		}
		*/
		SendMessage("StopFire");
	}

	// Messages

	void StoppedMoving() {
		//print("Stopped Moving");
		if (Random.value < SneezeChance) {
			//print("Preparing Sneeze");
			Invoke("Sneeze", SneezeStartup);
		}
	}
}
