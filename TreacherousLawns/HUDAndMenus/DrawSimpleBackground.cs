// Programmer: Carl Childers
// Date: 7/22/2016
//
// Draws a GUI box with specified style and screen percentages.

using UnityEngine;
using System.Collections;

public class DrawSimpleBackground : MonoBehaviour {

	public float XPerc = 0.5f;
	public float YPerc = 0.5f;
	public float WPerc = 0.25f;
	public float HPerc = 0.25f;

	public GUISkin Skin;
	public string Style;


	void OnGUI()
	{
		GUI.skin = Skin;

		Rect r = GUIUtilityFunctions.GetRectFromScreenPerc(XPerc, YPerc, WPerc, HPerc);
		GUI.Box(r, "", Style);
	}
}
