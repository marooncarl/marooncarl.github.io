/// <summary>
/// Spawn additional objects
/// 
/// Programmer: Carl Childers
/// Date: 7/11/2015
/// 
/// In case an object needs to spawn one or more additional objects, either immediately or by sending a message
/// </summary>

using UnityEngine;
using System.Collections;

public class SpawnAdditionalObjects : MonoBehaviour {

	public bool SpawnImmediately = true;
	public Transform[] Prefabs;


	void Start() {
		if (SpawnImmediately && Prefabs.Length > 0) {
			for (int i = 0; i < Prefabs.Length; ++i) {
				Instantiate(Prefabs[i], transform.position, transform.rotation);
			}
		}
	}


	// Messages

	void SpawnObjects() {
		if (Prefabs.Length > 0) {
			for (int i = 0; i < Prefabs.Length; ++i) {
				Instantiate(Prefabs[i], transform.position, transform.rotation);
			}
		}
	}
}
