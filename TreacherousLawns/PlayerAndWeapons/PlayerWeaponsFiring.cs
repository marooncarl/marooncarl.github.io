/// <summary>
/// Player weapons firing
/// 
/// Programmer: Carl Childers
/// Date: 5/24/2015
/// 
/// Sends firing messages to children based on player input.
/// </summary>

using UnityEngine;
using System.Collections;

public class PlayerWeaponsFiring : MonoBehaviour {

	public string InputName = "Fire1";

	//bool WeaponsLocked;


	/*
	void Awake() {
		WeaponsLocked = false;
	}
	*/

	// Update is called once per frame
	void Update () {
		if (/*WeaponsLocked ||*/ Time.timeScale == 0) {
			return;
		}

		if (Input.GetButtonDown(InputName)) {
			BroadcastMessage("StartFire", SendMessageOptions.DontRequireReceiver);
		} else if (Input.GetButtonUp(InputName)) {
			BroadcastMessage("StopFire", SendMessageOptions.DontRequireReceiver);
		}
	}

	/*
	public void LockWeapons(float inDuration) {
		WeaponsLocked = true;
		Invoke("UnlockWeapons", inDuration);
	}

	void UnlockWeapons() {
		WeaponsLocked = false;
	}
	*/
}
