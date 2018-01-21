/// <summary>
/// Simple attach
/// 
/// Programmer: Carl Childers
/// Date: 5/24/2015
/// 
/// Attaches a child game object without parenting.
/// In other words, the two hierarchies are separate, but the child inherits position, rotation, and scale.
/// This script's relative position, rotation, and scale are applied to the child.
/// </summary>

using UnityEngine;
using System.Collections;

public class SimpleAttach : MonoBehaviour {

	public Transform Child;
	public Vector3 RelPosition;
	public Vector3 RelRotation;
	public Vector3 RelScale;

	Quaternion RealRotation;

	void Awake() {
		RealRotation = Quaternion.Euler(RelRotation);
		UpdateChildTransform();
	}
	
	// Update is called once per frame
	void Update () {
		RealRotation = Quaternion.Euler(RelRotation);
		UpdateChildTransform();
	}

	void UpdateChildTransform() {
		if (Child != null) {
			Child.position = transform.position + transform.rotation * RelPosition;
			Child.rotation = transform.rotation * RealRotation;
			
			Vector3 newScale = transform.localScale;
			newScale.x *= RelScale.x;
			newScale.y *= RelScale.y;
			newScale.z *= RelScale.z;
			
			Child.localScale = newScale;
		}
	}

	// Messages

	void AttachChild(Transform inChild) {
		Child = inChild;
	}

	void DetachChild(Transform inChild) {
		Child = null;
	}
}
