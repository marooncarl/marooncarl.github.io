/// <summary>
/// Cloud movement
/// 
/// Programmer: Carl Childers
/// Date: 7/25/2015
/// 
/// Cloud movement.  Destroys self when moved far enough.
/// </summary>

using UnityEngine;
using System.Collections;

public class CloudMovement : MonoBehaviour {

	CloudGenerator MyGenerator;
	float AxisValue; // cloud's progress on its forward axis


	void Update() {
		if (MyGenerator != null) {
			Vector3 moveDelta = new Vector3(MyGenerator.MoveDelta.x, MyGenerator.MoveDelta.y, 0) * Time.deltaTime;
			transform.position += moveDelta;
			AxisValue += moveDelta.magnitude;
			if (AxisValue > MyGenerator.ForwardRadius) {
				Destroy(this.gameObject);
			}
		}
	}

	public void SetGenerator(CloudGenerator inGen) {
		MyGenerator = inGen;
	}

	public void SetAxisValue(float inValue) {
		AxisValue = inValue;
	}
}
