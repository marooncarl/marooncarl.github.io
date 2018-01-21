/// <summary>
/// Wasp behaviour
/// 
/// Programmer: Carl Childers
/// Date: 10/24/2015
/// 
/// Controls when a wasp moves and attacks
/// </summary>

using UnityEngine;
using System.Collections;

public class WaspBehaviour : MonoBehaviour {

	public string PlayerTag = "Player";
	public float AttackRange = 0.2f;

	bool IsAttacking;
	WaspMove MoveScript;


	void Awake() {
		IsAttacking = false;
		MoveScript = GetComponent<WaspMove>();
	}

	void Update () {

		if (!IsAttacking) {
			GameObject[] plys = GameObject.FindGameObjectsWithTag(PlayerTag);
			foreach (GameObject p in plys) {
				float distSq = (p.transform.position - transform.position).sqrMagnitude;
				if (distSq < AttackRange * AttackRange) {
					IsAttacking = true;
					SendMessage("TargetSighted", p.transform);
					break;
				}
			}
		}
	}

	// Messages
	void FinishedAttack() {
		MoveScript.enabled = true;
		MoveScript.HoverAtCurrentPosition();
	}
}
