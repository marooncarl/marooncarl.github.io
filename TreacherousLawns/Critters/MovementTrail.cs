/// <summary>
/// Movement trail
/// 
/// Programmer: Carl Childers
/// Date: 12/8/2015
/// 
/// Spawns a trail of objects while moving.
/// </summary>

using UnityEngine;
using System.Collections;

public class MovementTrail : MonoBehaviour {

	public Transform TrailPrefab;
	public float DistanceThreshold = 0.1f; // Must move this far before spawning another object
	public float SpawnMaxOffset = 0.2f; // Randomizes spawn position of trail objects

	float DistanceTraveled;
	Vector3 LastPosition;

	void Awake () {
		if (TrailPrefab == null) {
			enabled = false;
			return;
		}

		DistanceTraveled = 0;
		LastPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		DistanceTraveled += (transform.position - LastPosition).magnitude;
		if (DistanceTraveled >= DistanceThreshold) {
			Vector3 spawnPosition = transform.position;
			Vector2 randPos = Random.insideUnitCircle * SpawnMaxOffset;
			spawnPosition.x += randPos.x;
			spawnPosition.y += randPos.y;

			Instantiate(TrailPrefab, spawnPosition, Quaternion.identity);
			DistanceTraveled -= DistanceThreshold;
		}
		LastPosition = transform.position;
	}
}
