using UnityEngine;
using System.Collections;

public class CameraFollow2D : MonoBehaviour {

	public Transform Target;

	void Awake() {
		if (Target == null || camera == null) {
			enabled = false;
			return;
		}

		if (enabled) {
			camera.transform.position = new Vector3(Target.position.x, Target.position.y, Camera.main.transform.position.z);
		}
	}
	
	// Update is called once per frame
	void LateUpdate() {
		camera.transform.position = new Vector3(Target.position.x, Target.position.y, Camera.main.transform.position.z);
	}
}
