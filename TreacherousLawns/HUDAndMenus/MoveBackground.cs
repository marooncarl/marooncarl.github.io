/// <summary>
/// Move background
/// 
/// Programmer: Carl Childers
/// Date: 1/21/2015
/// 
/// Moves background object to keep up with a 2d camera.  Assumes object is a quad with
/// a tiled material.
/// </summary>


using UnityEngine;
using System.Collections;

public class MoveBackground : MonoBehaviour {

	Vector2 tileSize;

	void Awake () {
		Vector2 tilingScale = renderer.material.mainTextureScale;
		if (tilingScale.x > 0 && tilingScale.y > 0) {
			tileSize.x = transform.localScale.x / tilingScale.x;
			tileSize.y = transform.localScale.y / tilingScale.y;
		}
		else
			tileSize = new Vector2(1, 1);
		updatePosition();
	}

	void LateUpdate () {
		updatePosition();
	}

	void updatePosition() {
		if (tileSize.x <= 0 || tileSize.y <= 0) {
			return;
		}

		Vector3 topLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
		topLeft.z = transform.position.z;
		Vector3 newPos = topLeft + new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, 0);

		newPos.x /= tileSize.x;
		newPos.x = Mathf.Floor (newPos.x);
		newPos.x *= tileSize.x;

		newPos.y /= tileSize.y;
		newPos.y = Mathf.Floor (newPos.y);
		newPos.y *= tileSize.y;

		transform.position = newPos;
	}
}
