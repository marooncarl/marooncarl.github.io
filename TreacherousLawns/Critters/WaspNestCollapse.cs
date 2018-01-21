/// <summary>
/// Wasp nest collapse
/// 
/// Programmer: Carl Childers
/// Date: 12/2/2015
/// 
/// Sets up the moving rocks for the nest collapse animation.
/// Rock prefabs should have a "Move To Position" script
/// </summary>

using UnityEngine;
using System.Collections;

public class WaspNestCollapse : MonoBehaviour {

	public int NumRocks = 4;
	public float RockRadius = 0.2f;
	public float RockClosedRadius = 0.1f;
	public Transform[] RockPrefabs;


	// Use this for initialization
	void Start () {
		if (RockPrefabs.Length == 0 || NumRocks <= 0) {
			enabled = false;
			return;
		}

		float startingRot = Random.Range(-45, 0);
		for (int i = 0; i < NumRocks; ++i) {
			float currentRadians = (startingRot + (360.0f / NumRocks) * i) * Mathf.Deg2Rad;

			Vector3 currentPos = new Vector3(transform.position.x + RockRadius * Mathf.Cos(currentRadians),
			                                 transform.position.y + RockRadius * Mathf.Sin(currentRadians),
			                                 transform.position.z);
			Quaternion currentRot = Quaternion.Euler(0, 0, Random.value * 360.0f);
			Transform newRock = (Transform)Instantiate(RockPrefabs[ Random.Range(0, RockPrefabs.Length) ],
			                                           currentPos, currentRot);

			// Set properties on rock's Move To Position script
			MoveToPosition moveScript = newRock.GetComponent<MoveToPosition>();
			if (moveScript != null) {
				moveScript.RelativePosition = (transform.position - newRock.position).normalized * (RockRadius - RockClosedRadius);
				moveScript.SetDelay(1.0f + 0.3f * i);
			}
		}
	}
}
