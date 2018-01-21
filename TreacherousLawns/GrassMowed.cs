/// <summary>
/// Grass mowed
/// 
/// Programmer: Carl Childers
/// Date: 6/21/2015
/// 
/// Specific handling for mowing a grass tile.
/// </summary>

using UnityEngine;
using System.Collections;

public class GrassMowed : MonoBehaviour {

	// Messages

	void Mowed() {
		Destroy(this.gameObject);
	}
}
