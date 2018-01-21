/// <summary>
/// Lawn credits
/// 
/// Programmer: Carl Childers
/// Date: 10/29/2015
/// 
/// Displays exit button for the credits screen
/// </summary>


using UnityEngine;
using System.Collections;

public class LawnCredits : MonoBehaviour {

	public GUISkin MySkin;
	public GUISkin LowResSkin;

	public float TitleButtonXPerc = 0.5f;
	public float TitleButtonYPerc = 0.85f;
	public float TitleButtonWPerc = 0.2f;
	public float TitleButtonHPerc = 0.1f;

	public string TitleButtonText = "Title";
	public string TitleSceneName;


	void Update() {
		// Press the one button with the keyboard
		if (Input.GetButtonDown("PressMenuButton"))
		{
			LawnGameControl.PlayButtonPressSound();
			Application.LoadLevel(TitleSceneName);
		}
	}

	void OnGUI() {
		if (Screen.height <= 600) {
			GUI.skin = LowResSkin;
		} else {
			GUI.skin = MySkin;
		}

		// Highlight border around title button
		float HighlightBorderSize = 5.0f;

		Rect HighlightBoxRect = GUIUtilityFunctions.GetRectFromScreenPerc(TitleButtonXPerc, TitleButtonYPerc,
		                                                                  TitleButtonWPerc, TitleButtonHPerc);
		
		HighlightBoxRect.xMin -= HighlightBorderSize;
		HighlightBoxRect.xMax += HighlightBorderSize;
		HighlightBoxRect.yMin -= HighlightBorderSize;
		HighlightBoxRect.yMax += HighlightBorderSize;
		
		GUI.Box( HighlightBoxRect, "", "SelectedButton" );

		// Title button
		Rect btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(TitleButtonXPerc, TitleButtonYPerc, TitleButtonWPerc, TitleButtonHPerc);
		if (GUI.Button(btnRect, TitleButtonText)) {
			LawnGameControl.PlayButtonPressSound();
			Application.LoadLevel(TitleSceneName);
		}
	}
}
