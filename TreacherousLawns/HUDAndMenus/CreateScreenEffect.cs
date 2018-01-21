/// <summary>
/// Create screen effect
/// 
/// Programmer: Carl Childers
/// Date: 4/25/2015
/// 
/// For a 2D game, creates a prefab wherever the camera is for a screen effect.
/// </summary>

using UnityEngine;
using System.Collections;

public class CreateScreenEffect : MonoBehaviour {

	public Transform ScreenEffect;

	void Start() {
		if (ScreenEffect != null) {
			Instantiate (ScreenEffect, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), ScreenEffect.rotation);
		}
		enabled = false; // script no longer needed
	}
}
