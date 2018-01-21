/// <summary>
/// Lawn title screen
/// 
/// Programmer: Carl Childers
/// Date: 7/25/2015
/// 
/// Title screen - contains start button.  Goes to the next level when start button is pressed.
/// </summary>


using UnityEngine;
using System.Collections;

public class LawnTitleScreen : MonoBehaviour {

	public GUISkin MySkin;
	public GUISkin LowResSkin;

	public float ButtonWPerc = 0.25f;
	public float ButtonHPerc = 0.08f;
	public float ButtonXPerc = 0.5f;
	public float ButtonYPerc = 0.5f;
	public float ButtonSeperatePerc = 0.02f;

	public string[] ButtonText;
	public string[] SceneNames;

	public string QuitText;

	int CurrentButtonIndex, NumButtons;
	bool PressedCurrentButton;


	void Awake() {
		NumButtons = ButtonText.Length + 1;
	}

	void Update() {

		if (Input.GetButtonDown("PressMenuButton"))
		{
			PressedCurrentButton = true;
		}
	}

	// Keyboard input messages

	void MenuUp()
	{
		CurrentButtonIndex--;
		if (CurrentButtonIndex < 0)
			CurrentButtonIndex = NumButtons - 1;
	}

	void MenuDown()
	{
		CurrentButtonIndex++;
		if (CurrentButtonIndex >= NumButtons)
			CurrentButtonIndex = 0;
	}

	void OnGUI() {
		if (Screen.height <= 600) {
			GUI.skin = LowResSkin;
		} else {
			GUI.skin = MySkin;
		}

		float HighlightBorderSize = 5.0f;

		for (int i = 0; i < ButtonText.Length; ++i)
		{
			if (CurrentButtonIndex == i)
			{
				Rect HighlightBoxRect = new Rect(Screen.width * 0.5f +  Screen.height * (ButtonXPerc - 0.5f - ButtonWPerc * 0.5f),
				                                 Screen.height * (ButtonYPerc + ButtonHPerc * (-0.5f + i) + i * ButtonSeperatePerc),
				                                 Screen.height * ButtonWPerc, Screen.height * ButtonHPerc);

				HighlightBoxRect.xMin -= HighlightBorderSize;
				HighlightBoxRect.xMax += HighlightBorderSize;
				HighlightBoxRect.yMin -= HighlightBorderSize;
				HighlightBoxRect.yMax += HighlightBorderSize;

				GUI.Box( HighlightBoxRect, "", "SelectedButton" );
			}

			if (GUI.Button( new Rect(Screen.width * 0.5f +  Screen.height * (ButtonXPerc - 0.5f - ButtonWPerc * 0.5f),
			                         Screen.height * (ButtonYPerc + ButtonHPerc * (-0.5f + i) + i * ButtonSeperatePerc),
			                         Screen.height * ButtonWPerc, Screen.height * ButtonHPerc), ButtonText[i] )
			    			|| (CurrentButtonIndex == i && PressedCurrentButton)) {
				if (SceneNames.Length > i) {
					LawnGameControl.PlayButtonPressSound();
					Application.LoadLevel(SceneNames[i]);
				}
			}
		}

		// Quit button
		if (CurrentButtonIndex == NumButtons - 1)
		{
			Rect HighlightBoxRect = GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, ButtonYPerc + ButtonHPerc * ButtonText.Length + ButtonText.Length * ButtonSeperatePerc,
			                                                                  ButtonWPerc, ButtonHPerc);
			
			HighlightBoxRect.xMin -= HighlightBorderSize;
			HighlightBoxRect.xMax += HighlightBorderSize;
			HighlightBoxRect.yMin -= HighlightBorderSize;
			HighlightBoxRect.yMax += HighlightBorderSize;
			
			GUI.Box( HighlightBoxRect, "", "SelectedButton" );
		}

		if (GUI.Button( GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, ButtonYPerc + ButtonHPerc * ButtonText.Length + ButtonText.Length * ButtonSeperatePerc,
		                                                          ButtonWPerc, ButtonHPerc), QuitText)
		    			|| (CurrentButtonIndex == (NumButtons - 1) && PressedCurrentButton)) {
			LawnGameControl.PlayButtonPressSound();
			Application.Quit();
		}

		PressedCurrentButton = false;
	}
}
