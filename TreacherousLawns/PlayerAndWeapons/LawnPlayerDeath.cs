/// <summary>
/// Lawn player death
/// 
/// Programmer: Carl Childers
/// Date: 5/30/2015
/// 
/// Handles player death, and alerts the score keeper that the player died.
/// </summary>


using UnityEngine;
using System.Collections;

public class LawnPlayerDeath : MonoBehaviour {

	void Died() {
		Transform myScoreKeeper = FindObjectOfType<LawnScoreKeeper>().transform;
		if (myScoreKeeper != null) {
			myScoreKeeper.SendMessage("PlayerDied");
		}

		// Stop vehicle movement
		VehicleMovement2D vehicleScript = transform.root.GetComponentInChildren<VehicleMovement2D>();
		if (vehicleScript != null) {
			vehicleScript.enabled = false;
		}

		SendMessageUpwards("StartTurningOff", SendMessageOptions.DontRequireReceiver);

		// Stop weapon firing
		transform.root.BroadcastMessage("TurnOff", SendMessageOptions.DontRequireReceiver);
		transform.root.BroadcastMessage("Disable", SendMessageOptions.DontRequireReceiver);
	}
}
