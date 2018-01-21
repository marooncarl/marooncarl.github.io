/// <summary>
/// Cloud generator
/// 
/// Programmer: Carl Childers
/// Date: 6/20/2015
/// 
/// Generates clouds that move in an area.  The area is defined by a forward and side axis, and
/// can be rotated any way in 2D.  Objects created should have a CloudMovement script.
/// </summary>

using UnityEngine;
using System.Collections;

public class CloudGenerator : MonoBehaviour {

	public Transform[] CloudPrefabs;
	
	public float MinScale = 0.5f, MaxScale = 1.0f;
	public Vector2 MoveDelta = new Vector2(0.2f, 0.0f);
	public bool UseRandomMoveDelta = false;
	public float MinMoveSpeed = 0.2f, MaxMoveSpeed = 0.5f; // move speed if using random delta.  All clouds move the same speed.
	public float ForwardRadius = 60, SideRadius = 60;
	public float ForwardCellSize = 5, SideCellSize = 5;
	public float ExtraRadius = 1.0f;
	public float CloudChance = 1.0f;
	public bool ShouldPreWarm = true;

	Vector2 CloudOrigin;
	float CreateCounter;
	float MoveSpeed;
	Vector3 ForwardAxis, SideAxis;

	void Start() {
		int cloudSetting = PlayerPrefs.GetInt("CloudShadows", 1);
		if (cloudSetting == 0) {
			// player wants to disable cloud shadows
			enabled = false;
			return;
		}

		if (UseRandomMoveDelta) {
			MoveSpeed = Random.Range(MinMoveSpeed, MaxMoveSpeed);
			float randDir = Random.value * 360.0f * Mathf.Deg2Rad;
			MoveDelta = new Vector2(Mathf.Cos(randDir) * MoveSpeed, Mathf.Sin(randDir) * MoveSpeed);
		} else {
			MoveSpeed = MoveDelta.magnitude;
		}

		CloudOrigin = new Vector2(transform.position.x, transform.position.y) - MoveDelta.normalized * ForwardRadius;

		ForwardAxis = MoveDelta.normalized;
		SideAxis = new Vector3(-ForwardAxis.y, ForwardAxis.x, 0);

		// Make sure forward and side cell size are not <= 0, otherwise there will be an infinite loop
		ForwardCellSize = Mathf.Max(0.1f, ForwardCellSize);
		SideCellSize = Mathf.Max(0.1f, SideCellSize);

		if (ShouldPreWarm) {
			PreWarm();
		}
	}

	void Update() {
		CreateCounter -= MoveSpeed * Time.deltaTime;
		if (CreateCounter <= 0) {

			float extraSide = Random.Range(-ExtraRadius * 8, ExtraRadius * 8);
			for (float i = -SideRadius; i <= SideRadius; i += SideCellSize) {
				if (Random.value <= CloudChance) {
					Vector3 cloudPos = new Vector3(CloudOrigin.x, CloudOrigin.y) + (SideAxis * (i + extraSide));
					CreateCloud(cloudPos, -ForwardRadius);
				}
			}

			CreateCounter += ForwardCellSize;
		}
	}

	void PreWarm() {
		for (float j = 0; j <= ForwardRadius * 2; j += ForwardCellSize) {
			float extraSide = Random.Range(-ExtraRadius * 8, ExtraRadius * 8);
			for (float i = -SideRadius; i <= SideRadius; i += SideCellSize) {
				if (Random.value <= CloudChance) {
					Vector3 cloudPos = new Vector3(CloudOrigin.x, CloudOrigin.y) + (ForwardAxis * j) + (SideAxis * (i + extraSide));
					CreateCloud(cloudPos, -ForwardRadius + j);
				}
			}
		}

		CreateCounter = ForwardCellSize;
	}

	void CreateCloud(Vector3 inPos, float inAxisValue) {
		if (CloudPrefabs.Length == 0) {
			return;
		}

		int randIndex = (int)(Random.value * CloudPrefabs.Length);
		randIndex = Mathf.Clamp(randIndex, 0, CloudPrefabs.Length - 1);
		Transform newCloud = (Transform)Instantiate(CloudPrefabs[randIndex], inPos, Quaternion.Euler(0, 0, Random.value * 360.0f));

		float randScale = Random.Range(MinScale, MaxScale);
		newCloud.localScale = new Vector3(randScale, randScale, randScale);

		if (ExtraRadius > 0) {
			Vector2 randVect = Random.insideUnitCircle * Random.Range(0, ExtraRadius);
			newCloud.position = new Vector3(newCloud.position.x + randVect.x, newCloud.position.y + randVect.y, newCloud.position.z);
		}

		CloudMovement moveScript = newCloud.GetComponent<CloudMovement>();
		if (moveScript != null) {
			moveScript.SetGenerator(this);
			moveScript.SetAxisValue(inAxisValue);
		}
	}
}
