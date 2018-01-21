/// <summary>
/// Font size by res
/// 
/// Programmer: Carl Childers
/// Date: 12/22/2015
/// 
/// Causes attached GUI Text component to have a different font size for low resolutions.
/// Added option to add to the x position if the screen is in a wider aspect ratio.
/// </summary>


using UnityEngine;
using System.Collections;

public class FontSizeByRes : MonoBehaviour {

	public int FontSizeNormal = 18, FontSizeSmall = 10;
	public bool AddXForWidescreen = false;

	float defaultTextX;


	void Awake() {
		if (guiText == null) {
			enabled = false;
			return;
		}

		defaultTextX = guiText.transform.position.x;
	}

	// Update is called once per frame
	void Update () {
		if (Screen.height <= 600) {
			guiText.fontSize = FontSizeSmall;
		} else {
			guiText.fontSize = FontSizeNormal;
		}

		if (AddXForWidescreen && Screen.height > 0)
		{
			Vector3 newTextPos = guiText.transform.position;
			newTextPos.x = defaultTextX + ((float)Screen.width / Screen.height - 1.25f) / 10.0f;
			guiText.transform.position = newTextPos;
		}
	}
}
