/// <summary>
/// Camera zoom 2D
/// 
/// Programmer: Carl Childers
/// Date: 1/18/2015
/// 
/// Zooms a 2D camera using the mouse wheel.
/// </summary>

using UnityEngine;
using System.Collections;

public class CameraZoom2D : MonoBehaviour {

	public float MinCameraSize = 1, MaxCameraSize = 20, SizeInterval = 1;
	public string InputAxisName = "Mouse ScrollWheel";

	
	void Awake() {
		if (camera == null) {
			enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
		float inScroll = Input.GetAxis (InputAxisName);
		if (inScroll > 0) {
			camera.orthographicSize = Mathf.Max (camera.orthographicSize - SizeInterval, MinCameraSize);
		} else if (inScroll < 0) {
			camera.orthographicSize = Mathf.Min (camera.orthographicSize + SizeInterval, MaxCameraSize);
		}
	}
}
