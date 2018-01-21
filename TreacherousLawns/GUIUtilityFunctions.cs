/// <summary>
/// GUI utility functions
/// 
/// Programmer: Carl Childers
/// Date: 8/23/2015
/// 
/// A collection of functions useful for displaying a GUI
/// </summary>

using UnityEngine;
using System.Collections;

public class GUIUtilityFunctions : MonoBehaviour {

	// Returns a rect to use for drawing a screen element, taking in screen percents for center x, center y, width, and height
	// Percents are multiplied by screen height
	// x positions are centered at the screen center, then offset by (X Perc - 0.5) * Screen Height
	public static Rect GetRectFromScreenPerc(float inXPerc, float inYPerc, float inWPerc, float inHPerc) {
		Rect rValue = new Rect();
		
		rValue.width = inWPerc * Screen.height;
		rValue.height = inHPerc * Screen.height;
		
		rValue.x = (inXPerc * Screen.height) - (Screen.height / 2) + (Screen.width / 2) - (rValue.width / 2);
		rValue.y = (inYPerc * Screen.height) - (rValue.height / 2);
		
		return rValue;
	}

	// Scales a rect if the aspect ratio is not widescreen.
	public static Rect ModifyRectByResRatio(Rect theRect, float inNonWideScale) {
		if (Screen.height <= 0) {
			return theRect;
		}
		float aspectRatio = (float)Screen.width / Screen.height;

		Rect newRect = theRect;
		if (aspectRatio < 1.5f) {
			Vector2 oldCenter = newRect.center;
			newRect.width *= inNonWideScale;
			newRect.height *= inNonWideScale;
			newRect.center = new Vector2((Screen.width / 2.0f) + (oldCenter.x - Screen.width / 2.0f) * inNonWideScale,
			                             (Screen.height / 2.0f) + (oldCenter.y - Screen.height / 2.0f) * inNonWideScale);
		}
		return newRect;
	}
}
