/// <summary>
/// Pauser
/// 
/// Programmer: Carl Childers
/// Date: 8/23/2015
/// 
/// Pauses the game, with a menu allowing the player to resume or quit the current stage.
/// The pauser also hides the mouse cursor at the start of a level, and shows it again during the pause menu.
/// </summary>

using UnityEngine;
using System.Collections;

public class Pauser : MonoBehaviour {

	public GUISkin MySkin;
	public GUISkin LowResSkin;
	public string PauseStyle;

	public string PauseInputName = "Pause";
	public string LevelSelectSceneName;

	public float ButtonXPerc = 0.5f;
	public float ButtonYPerc = 0.5f;
	public float ButtonWPerc = 0.2f;
	public float ButtonHPerc = 0.08f;
	public float ButtonSeperationPerc = 0.02f;

	public float PauseLabelYPerc = 0.3f;
	public float PauseLabelWPerc = 0.4f;
	public float PauseLabelHPerc = 0.2f;

	public string PausedText = "Paused";
	public string ResumeText = "Resume";
	public string ControlsText = "Controls";
	public string BackText = "Back";
	public string LevelSelectText = "Level Select";

	public float LockWeaponDuration = 0.1f;

	public Transform ControlSheetPrefab;

	bool IsPaused;

	int CurrentButtonIndex;
	const int MaxButtonIndex = 3;
	bool PressedCurrentButton;

	Transform myControlSheet;
	bool ShowingControlSheet;
	//bool PressedUp, PressedDown;


	void Awake() {
		IsPaused = false;
		ShowingControlSheet = false;
		Screen.showCursor = false; // Don't show the mouse cursor during gameplay
	}

	void Update() {
		if (Input.GetButtonDown(PauseInputName)) {
			if (!IsPaused) {
				Pause();
			} else {
				UnPause();
			}
		}

		if (IsPaused)
		{
			if (Input.GetButtonDown("PressMenuButton"))
			{
				PressedCurrentButton = true;
			}
		}
	}

	// Keyboard input messages
	void MenuUp()
	{
		CurrentButtonIndex--;
		if (CurrentButtonIndex < 0)
			CurrentButtonIndex = MaxButtonIndex - 1;
	}

	void MenuDown()
	{
		CurrentButtonIndex++;
		if (CurrentButtonIndex >= MaxButtonIndex)
			CurrentButtonIndex = 0;
	}

	/*
	void UpdateMenuInput()
	{
		// Seperate script doesn't work with the keyboard while paused, so do keyboard input here
		if (IsPaused && !ShowingControlSheet)
		{
			float VertAxis = Input.GetAxis("Vertical");
			if (VertAxis > 0)
			{
				if (!PressedUp)
				{
					PressedUp = true;
					
					CurrentButtonIndex--;
					if (CurrentButtonIndex < 0)
						CurrentButtonIndex = MaxButtonIndex - 1;
				}
			}
			else
				PressedUp = false;
			
			if (VertAxis < 0)
			{
				if (!PressedDown)
				{
					PressedDown = true;
					
					CurrentButtonIndex++;
					if (CurrentButtonIndex >= MaxButtonIndex)
						CurrentButtonIndex = 0;
				}
			}
			else
				PressedDown = false;
		}
	}
	*/

	void OnGUI() {

		//UpdateMenuInput();

		if (Screen.height <= 600) {
			GUI.skin = LowResSkin;
		} else {
			GUI.skin = MySkin;
		}

		int HighlightBorderSize = 5;

		if (IsPaused) {

			if (!ShowingControlSheet)
			{
				GUI.Label(GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, PauseLabelYPerc, PauseLabelWPerc, PauseLabelHPerc), PausedText, PauseStyle);

				// Resume highlight
				if (CurrentButtonIndex == 0)
				{
					Rect HighlightBoxRect = GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, ButtonYPerc, ButtonWPerc, ButtonHPerc);
					
					HighlightBoxRect.xMin -= HighlightBorderSize;
					HighlightBoxRect.xMax += HighlightBorderSize;
					HighlightBoxRect.yMin -= HighlightBorderSize;
					HighlightBoxRect.yMax += HighlightBorderSize;
					
					GUI.Box( HighlightBoxRect, "", "SelectedButton" );
				}

				// Resume button
				if (GUI.Button(GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, ButtonYPerc, ButtonWPerc, ButtonHPerc), ResumeText)
				    		|| (PressedCurrentButton && CurrentButtonIndex == 0)) {

					LawnGameControl.PlayButtonPressSound();
					UnPause();
				}

				// Controls highlight
				if (CurrentButtonIndex == 1)
				{
					Rect HighlightBoxRect = GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, ButtonYPerc + (ButtonHPerc + ButtonSeperationPerc),
					                                                             ButtonWPerc, ButtonHPerc);
					
					HighlightBoxRect.xMin -= HighlightBorderSize;
					HighlightBoxRect.xMax += HighlightBorderSize;
					HighlightBoxRect.yMin -= HighlightBorderSize;
					HighlightBoxRect.yMax += HighlightBorderSize;
					
					GUI.Box( HighlightBoxRect, "", "SelectedButton" );
				}

				// Controls button
				if (GUI.Button(GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, ButtonYPerc + (ButtonHPerc + ButtonSeperationPerc),
				                                                         ButtonWPerc, ButtonHPerc), ControlsText)
				    || (PressedCurrentButton && CurrentButtonIndex == 1)) {

					// Show controls, and a back button
					if (ControlSheetPrefab != null)
					{
						myControlSheet = (Transform)Instantiate(ControlSheetPrefab, Vector3.zero, Quaternion.identity);
						ShowingControlSheet = true;
						LawnGameControl.PlayButtonPressSound();
					}
				}

				// Level select highlight
				if (CurrentButtonIndex == 2)
				{
					Rect HighlightBoxRect = GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, ButtonYPerc + (ButtonHPerc + ButtonSeperationPerc) * 2,
					                                                             ButtonWPerc, ButtonHPerc);
					
					HighlightBoxRect.xMin -= HighlightBorderSize;
					HighlightBoxRect.xMax += HighlightBorderSize;
					HighlightBoxRect.yMin -= HighlightBorderSize;
					HighlightBoxRect.yMax += HighlightBorderSize;
					
					GUI.Box( HighlightBoxRect, "", "SelectedButton" );
				}

				// Level select button
				if (GUI.Button(GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, ButtonYPerc + (ButtonHPerc + ButtonSeperationPerc) * 2,
				                                                         ButtonWPerc, ButtonHPerc), LevelSelectText)
				    		|| (PressedCurrentButton && CurrentButtonIndex == 2)) {

					LawnGameControl.PlayButtonPressSound();
					UnPause();
					Screen.showCursor = true;
					Application.LoadLevel(LevelSelectSceneName);
				}
			}
			else
			{
				// Showing control sheet

				// Back highlight
				Rect btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(ButtonXPerc, ButtonYPerc + (ButtonHPerc + ButtonSeperationPerc) * 3,
				                                                         ButtonWPerc, ButtonHPerc);

				Rect HighlightBoxRect = btnRect;
				
				HighlightBoxRect.xMin -= HighlightBorderSize;
				HighlightBoxRect.xMax += HighlightBorderSize;
				HighlightBoxRect.yMin -= HighlightBorderSize;
				HighlightBoxRect.yMax += HighlightBorderSize;
				
				GUI.Box( HighlightBoxRect, "", "SelectedButton" );
				
				// Back button
				if (GUI.Button(btnRect, BackText)
				    || PressedCurrentButton) {
					
					LawnGameControl.PlayButtonPressSound();
					ShowingControlSheet = false;
					if (myControlSheet != null) {
						GameObject.Destroy(myControlSheet.gameObject);
					}
				}
			}
		}

		PressedCurrentButton = false;
	}

	void Pause() {
		if (IsPaused) {
			return;
		}

		IsPaused = true;
		Time.timeScale = 0f;
		AudioListener.pause = true;
		CurrentButtonIndex = 0;
		Screen.showCursor = true;
	}

	void UnPause() {
		if (!IsPaused) {
			return;
		}

		IsPaused = false;
		Time.timeScale = 1.0f;
		AudioListener.pause = false;
		Screen.showCursor = false;

		ShowingControlSheet = false;
		if (myControlSheet != null) {
			GameObject.Destroy(myControlSheet.gameObject);
		}

		/*
		PlayerWeaponsFiring fireScript = FindObjectOfType<PlayerWeaponsFiring>();
		if (fireScript != null) {
			fireScript.LockWeapons(LockWeaponDuration);
		}
		*/
	}
}
