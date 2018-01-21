/// <summary>
/// Drop bomb
/// 
/// Programmer: Carl Childers
/// Date: 7/11/2015
/// 
/// Drops a bomb or other item periodically.
/// </summary>

using UnityEngine;
using System.Collections;

public class DropBomb : MonoBehaviour {

	public Transform DropPrefab;
	public AudioClip DropSound;
	public float DropPeriod;
	public Vector3 DropOffset;
	public bool TurnedOn = true;
	public bool StartWithTimer = false;

	float DropCounter;


	void Awake() {
		if (StartWithTimer) {
			DropCounter = DropPeriod;
		} else {
			DropCounter = 0;
		}
	}

	void Update () {
		DropCounter = Mathf.Max(DropCounter - Time.deltaTime, 0);
		if (DropCounter == 0 && TurnedOn && DropPrefab != null) {
			ReleaseBomb();
		}
	}

	void ReleaseBomb() {
		Vector3 dropPosition = transform.position + transform.right * DropOffset.x + transform.up * DropOffset.y + transform.forward * DropOffset.z;
		Instantiate(DropPrefab, dropPosition, DropPrefab.rotation);
		DropCounter = DropPeriod;
		if (DropSound != null) {
			AudioSource.PlayClipAtPoint(DropSound, dropPosition);
		}
	}

	// Messages
	// This script can turn on or off, but the Drop Counter will still update so
	// when it is turned on again it will be ready to drop another item

	void TurnOn() {
		TurnedOn = true;
	}

	void TurnOff() {
		TurnedOn = false;
	}
}
