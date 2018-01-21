/// <summary>
/// Simple move 2D
/// 
/// Programmer: Carl Childers
/// Date: 6/20/2015
/// 
/// Simply moves a delta amount in 2D.  The delta can be changed with a SetMoveDelta message.
/// </summary>


using UnityEngine;
using System.Collections;

public class SimpleMove2D : MonoBehaviour {

	public Vector2 MoveDelta; // in units per second

	// Update is called once per frame
	void Update () {
		Vector3 newPos = transform.position;
		newPos.x += MoveDelta.x * Time.deltaTime;
		newPos.y += MoveDelta.y * Time.deltaTime;
		transform.position = newPos;
	}

	// Messages

	void SetMoveDelta(Vector2 inMoveDelta) {
		MoveDelta = inMoveDelta;
	}
}
