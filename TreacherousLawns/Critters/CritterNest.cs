/// <summary>
/// Critter nest
/// 
/// Programmer: Carl Childers
/// Date: 5/16/2015
/// 
/// Nest that spawns critters at intervals, and in bunches when an enemy approaches or collides with the nest
/// </summary>

using UnityEngine;
using System.Collections;

public class CritterNest : MonoBehaviour {

	public Transform CritterType;
	public int CritterCount = 10;
	public float SpawnInterval = 2.0f;
	public int DefendSpawnNumber = 5;
	public float DefendSpawnCooldown = 10.0f;
	public float DamageReactTime = 0.5f;
	public Transform DeathPrefab;
	public AudioClip DeathSound;
	public Transform DeathEffect;
	public string[] HostileTags;

	int CrittersLeft;
	float DefendSpawnCounter;
	bool IsBlocked; // can't spawn critters if blocked


	void Awake() {
		if (CritterType == null) {
			enabled = false;
			return;
		}

		CrittersLeft = CritterCount;
		DefendSpawnCounter = 0;
		IsBlocked = false;
	}

	// Use this for initialization
	void Start () {
		InvokeRepeating("SpawnCritter", SpawnInterval, SpawnInterval);
	}
	
	// Update is called once per frame
	void Update () {
		if (DefendSpawnCounter > 0) {
			DefendSpawnCounter -= Time.deltaTime;
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (DefendSpawnCounter <= 0 && CrittersLeft > 0 && !IsBlocked) {

			// check if the other object is hostile
			bool IsHostile = false;
			foreach (string t in HostileTags) {
				//print("other.tag == " + other.tag);
				if (other.tag == t) {
					IsHostile = true;
					break;
				}
			}
			if (!IsHostile) {
				return;
			}

			DefendSpawnCounter = DefendSpawnCooldown;

			// look for a player object in the collider's hierarchy
			Transform enemy = other.transform;
			Transform[] childTransforms = enemy.GetComponentsInChildren<Transform>();
			foreach (Transform t in childTransforms) {
				if (t.tag == "Player") {
					enemy = t;
					//print("Found player transform");
					break;
				}
			}

			for (int i = 0; i < DefendSpawnNumber; ++i) {
				SpawnCritterWithTarget(enemy);
			}
		}
	}

	void SpawnCritter() {
		if (CrittersLeft <= 0 || IsBlocked) {
			return;
		}

		Transform critter = (Transform)Instantiate(CritterType, transform.position, CritterType.rotation);
		critter.SendMessage("MoveInDirection", Random.Range(0, 360), SendMessageOptions.DontRequireReceiver);
		critter.SendMessage("SetNest", transform, SendMessageOptions.DontRequireReceiver);
		CrittersLeft--;
	}

	void SpawnCritterWithTarget(Transform inTarget) {
		if (CrittersLeft <= 0 || IsBlocked) {
			return;
		}

		Transform critter = (Transform)Instantiate(CritterType, transform.position, CritterType.rotation);
		critter.SendMessage("SetTarget", inTarget, SendMessageOptions.DontRequireReceiver);
		critter.SendMessage("SetNest", transform, SendMessageOptions.DontRequireReceiver);
		critter.SendMessage("SetRandomAttackDirection", SendMessageOptions.DontRequireReceiver);
		CrittersLeft--;
	}

	void SendCrittersAtPlayer() {
		if (DefendSpawnCounter <= 0 && CrittersLeft > 0 && !IsBlocked) {
			DefendSpawnCounter = DefendSpawnCooldown;

			GameObject playerObject = GameObject.FindWithTag("Player");
			if (playerObject != null) {
				for (int i = 0; i < DefendSpawnNumber; ++i) {
					SpawnCritterWithTarget(playerObject.transform);
				}
			}
		}
	}

	// Get functions

	public bool GetIsBlocked() {
		return IsBlocked;
	}

	// Messages

	void CritterReturned() {
		CrittersLeft++;
	}

	void BombAnchored() {
		IsBlocked = true;
	}

	void Bombed() {
		if (DeathPrefab != null) {
			Instantiate(DeathPrefab, transform.position, transform.rotation);
		}
		if (DeathEffect != null) {
			Instantiate(DeathEffect, transform.position, transform.rotation);
		}
		if (DeathSound != null) {
			AudioSource.PlayClipAtPoint(DeathSound, transform.position);
		}
		Destroy(this.gameObject);
	}

	void TakeDamage(Damage inDmg) {
		if (IsBlocked) {
			return;
		}
		Invoke("SendCrittersAtPlayer", DamageReactTime);
	}
}
