/// <summary>
/// Randomize child rotations
/// 
/// Programmer: Carl Childers
/// Date: 6/20/2015
/// 
/// Randomize child rotations for a 2D setting, optionally using a set seed.
/// Useful for randomizing the appearance of decorative tiles.
/// </summary>

using UnityEngine;
using System;
using System.Collections;

public class RandomizeChildRotations : MonoBehaviour {
	
	public int Seed = 0; // Seed to use for random numbers
	public bool UseSeed = true;
	public bool UsePosition = true; // If true, use position to get a random number, so object load order doesn't matter.
	public bool AllowFreeRotation = false; // If true, rotation in any direction.  Otherwise, only allow four directions.

	System.Random MyRandom;
	int[] StoredNumbers;
	const int STORED_NUMBER_COUNT = 1000;


	void Awake() {
		if (UseSeed || UsePosition) {
			MyRandom = new System.Random(Seed);
		}

		if (UsePosition) {
			StoredNumbers = new int[STORED_NUMBER_COUNT];
			for (int i = 0; i < STORED_NUMBER_COUNT; ++i) {
				if (AllowFreeRotation) {
					StoredNumbers[i] = MyRandom.Next(360);
				} else {
					StoredNumbers[i] = MyRandom.Next(4);
				}
			}
		}

		for (int i = 0; i < transform.childCount; ++i) {
			RotateChild( transform.GetChild(i) );
		}

		if (MyRandom != null) {
			MyRandom = null;
		}
		if (StoredNumbers != null) {
			StoredNumbers = null;
		}
		enabled = false; // disable at the end, since it doesn't need to do anything else
	}

	// Recursive; rotates the given child and calls this function of the child's children if there are any
	void RotateChild(Transform inChild) {
		//print("rotating " + inChild.gameObject.name);

		int rand, rotDegrees;
		if (!UsePosition)
		{

			if (MyRandom != null) {
				rand = MyRandom.Next(360);
			} else {
				rand = (int)(UnityEngine.Random.value * 360.0f);
			}

			rotDegrees = rand;
			if (!AllowFreeRotation) {
				rotDegrees = (rotDegrees / 90) * 90;
			}

		} else {

			int index = (int)Mathf.Abs(inChild.position.x + (inChild.position.y * 25)) % STORED_NUMBER_COUNT;
			if (StoredNumbers != null) {
				rand = StoredNumbers[index];
			} else {
				rand = 0;
			}

			rotDegrees = rand;
			if (!AllowFreeRotation) {
				rotDegrees *= 90;
			}

		}



		inChild.rotation = Quaternion.Euler(0, 0, rotDegrees);

		for (int i = 0; i < inChild.childCount; ++i) {
			RotateChild( inChild.GetChild(i) );
		}
	}
}
