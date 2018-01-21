// Programmer: Carl Childers
// Date: 7/22/2016
//
// Draws text for the control screen.

using UnityEngine;
using System.Collections;

public class DrawControlText : MonoBehaviour {

	public GUISkin MySkin, LowResSkin;
	public string TitleText, ControlsText;

	public float TitleXPerc, TitleYPerc, TitleWPerc, TitleHPerc;
	public float ControlsXPerc, ControlsYPerc, ControlsWPerc, ControlsHPerc;

	public string TitleStyle, ControlsStyle;


	void OnGUI()
	{
		if (Screen.height <= 600) {
			GUI.skin = LowResSkin;
		} else {
			GUI.skin = MySkin;
		}

		Rect r = GUIUtilityFunctions.GetRectFromScreenPerc(TitleXPerc, TitleYPerc, TitleWPerc, TitleHPerc);
		GUI.Label(r, TitleText, TitleStyle);

		r = GUIUtilityFunctions.GetRectFromScreenPerc(ControlsXPerc, ControlsYPerc, ControlsWPerc, ControlsHPerc);
		GUI.Label(r, ControlsText, ControlsStyle);
	}
}
