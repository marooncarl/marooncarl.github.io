/// <summary>
/// Weapon equip
/// 
/// Programmer: Carl Childers
/// Date: 6/6/2015
/// 
/// Handles player weapon equipment.  Receives messages from the Weapon Choice HUD script.
/// </summary>

using UnityEngine;
using System.Collections;

public class WeaponEquip : MonoBehaviour {

	//public string[] WeaponQuickSelectInput;

	Transform[] WeaponTypes; // set by the Weapon Choice script before equipping a weapon

	Transform[] AllWeapons;
	int CurrentWeapIndex;
	Transform CurrentWeapon;


	void Awake() {
		CurrentWeapIndex = 0;
	}

	void InitWeaponTypes(Transform[] inPrefabs) {
		WeaponTypes = inPrefabs;
		AllWeapons = new Transform[ WeaponTypes.Length ];

		for (int i = 0; i < WeaponTypes.Length; ++i) {
			if (WeaponTypes[i] != null) {
				EquipWeapon(i);
				break;
			}
		}
	}

	// Messages
	void EquipWeapon(int inIndex) {
		if (AllWeapons == null || WeaponTypes == null) {
			return;
		}

		if (inIndex == CurrentWeapIndex && CurrentWeapon != null) {
			return;
		}

		if (inIndex < 0 || inIndex >= WeaponTypes.Length) {
			return;
		}

		if (CurrentWeapon != null) {
			CurrentWeapon.BroadcastMessage("TurnOff", SendMessageOptions.DontRequireReceiver);
			CurrentWeapon.parent = null;
			CurrentWeapon.gameObject.SetActive(false);
		}

		CurrentWeapIndex = inIndex;
		if (AllWeapons[inIndex] == null && WeaponTypes[inIndex] != null) {
			AllWeapons[inIndex] = (Transform)Instantiate(WeaponTypes[inIndex], transform.position, transform.rotation);
		}
		if (AllWeapons[inIndex] != null) {
			CurrentWeapon = AllWeapons[inIndex];
			CurrentWeapon.gameObject.SetActive(true);
			CurrentWeapon.position = transform.position;
			CurrentWeapon.rotation = transform.rotation;
			CurrentWeapon.parent = transform;
		}
	}
}
