/// <summary>
/// Ignite
/// 
/// Programmer: Carl Childers
/// Date: 12/8/2015
/// 
/// Creates a flame prefab when something with a specfic tag runs into it
/// </summary>

using UnityEngine;
using System.Collections;

public class Ignite : MonoBehaviour {

	public Transform FirePrefab;
	public string IgniteTag;
	public float ActiveTime = 5.0f;


	void Awake() {
		if (ActiveTime > 0) {
			Invoke("NoLongerActive", ActiveTime);
		}
	}

	void NoLongerActive() {
		enabled = false;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (enabled && other.tag == IgniteTag) {
			Instantiate(FirePrefab, transform.position, Quaternion.identity);
			SendMessage("Died", Defense.EDamageType.DT_Normal);
			enabled = false;
		}
	}
}
