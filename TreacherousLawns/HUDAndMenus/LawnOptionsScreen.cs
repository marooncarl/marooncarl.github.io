/// <summary>
/// Lawn options screen
/// 
/// Programmer: Carl Childers
/// Date: 9/26/2015
/// 
/// Options screen for lawn mowing game.
/// </summary>

using UnityEngine;
using System.Collections;

public class LawnOptionsScreen : MonoBehaviour {

	public GUISkin MySkin;
	public GUISkin LowResSkin;

	public string TitleSceneName;
	public string TitleButtonText;
	public string OptionNameStyle;

	public float TitleButtonXPerc = 0.5f;
	public float TitleButtonYPerc = 0.85f;
	public float TitleButtonWPerc = 0.2f;
	public float TitleButtonHPerc = 0.1f;

	public float OptionLabelXPerc = 0.3f;
	public float OptionLabelWPerc = 0.2f;

	public float OptionBarXPerc = 0.5f;
	public float OptionBarYPerc = 0.15f;
	public float OptionButtonWPerc = 0.1f; // width of one button
	public float OptionButtonHPerc = 0.05f;
	public float OptionSliderWPerc = 0.25f;
	public float OptionSliderYPercAdd = 0.025f; // amount to add for a slider so it is centered vertically
	public float OptionButtonIncrementYPerc = 0.075f; // space between each button y value

	public string CloudOptionName = "Cloud Shadows";
	string[] CloudOptionChoices = {"Off", "On"};
	int CloudOptionSetting;

	public string SoundVolumeName = "Sound Volume";
	float SoundVolumeSetting;

	public string MusicVolumeName = "Music Volume";
	float MusicVolumeSetting;

	public string DeleteSaveDataString = "Delete Save Data";

	public string DeletedString = "Save data has been exterminated";
	public float DeletedXPerc = 0.5f;
	public float DeletedYPerc = 0.75f;
	public float DeletedWPerc = 0.4f;
	public float DeletedHPerc = 0.05f;

	int CurrentButtonIndex;
	bool PressedCurrentButton;
	int NumItems; // all selectable menu items, including the exit button

	bool ShowingDeletedMessage;
	float DeleteCountdown;
	bool HoldingDeleteCountdownMouse;
	bool HoldingDeleteCountdownKey;
	bool DeletedDataThisPress;

	AudioSource musicAudioSource;


	void Awake() {
		CloudOptionSetting = PlayerPrefs.GetInt("CloudShadows", 1);
		SoundVolumeSetting = PlayerPrefs.GetFloat("SoundVolume", 1f);
		MusicVolumeSetting = PlayerPrefs.GetFloat("MusicVolume", 1f);

		NumItems = 5;

		MenuMusic mus = MenuMusic.GetTheMenuMusic();
		if (mus != null) {
			musicAudioSource = mus.GetComponentInChildren<AudioSource>();
		}

		DeleteCountdown = 3.0f;
	}

	void Update() {

		if (Input.GetButtonDown("PressMenuButton"))
		{
			PressedCurrentButton = true;
		}

		// Adjust music volume on the fly
		if (musicAudioSource != null)
			musicAudioSource.volume = MusicVolumeSetting;

		if (HoldingDeleteCountdownKey && Input.GetButtonUp("PressMenuButton"))
		{
			HoldingDeleteCountdownKey = false;
		}

		if (HoldingDeleteCountdownMouse || HoldingDeleteCountdownKey)
		{
			if (!DeletedDataThisPress)
			{
				DeleteCountdown -= Time.deltaTime;
				if (DeleteCountdown <= 0)
				{
					HoldingDeleteCountdownMouse = false;
					HoldingDeleteCountdownKey = false;
					DeletedDataThisPress = true;

					LawnGameControl theGameControl = LawnGameControl.GetGameControl();
					if (theGameControl != null) {
						theGameControl.DeleteSaveData();
						ShowingDeletedMessage = true;
						CancelInvoke("HideDeletedMessage");
						Invoke("HideDeletedMessage", 5.0f);
					}

					StopCameraShake();
				}
				else if (DeleteCountdown <= 1)
				{
					StartCameraShake();
				}
			}
		}
		else
		{
			if (DeleteCountdown <= 1) {
				StopCameraShake();
			}

			DeleteCountdown = 3;
			DeletedDataThisPress = false;
		}
	}

	void StartCameraShake()
	{
		ShakeCamera shakeScript = Camera.main.GetComponent<ShakeCamera>();
		if (shakeScript != null) {
			shakeScript.SetShakeMagnitude(0.02f);
		}
	}

	void StopCameraShake()
	{
		ShakeCamera shakeScript = Camera.main.GetComponent<ShakeCamera>();
		if (shakeScript != null) {
			shakeScript.SetShakeMagnitude(0f);
		}
		Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);
	}

	// Keyboard Input Delegates

	void MenuUp()
	{
		CurrentButtonIndex--;
		if (CurrentButtonIndex < 0)
			CurrentButtonIndex = NumItems - 1;
	}

	void MenuDown()
	{
		CurrentButtonIndex++;
		if (CurrentButtonIndex >= NumItems)
			CurrentButtonIndex = 0;
	}

	void MenuLeft()
	{
		switch (CurrentButtonIndex)
		{
		case 0:
			CloudOptionSetting = Mathf.Max(CloudOptionSetting - 1, 0);
			break;
		case 1:
			SoundVolumeSetting = Mathf.Max(SoundVolumeSetting - 0.05f, 0);
			break;
		case 2:
			MusicVolumeSetting = Mathf.Max(MusicVolumeSetting - 0.05f, 0);
			break;
		}
	}

	void MenuRight()
	{
		switch (CurrentButtonIndex)
		{
		case 0:
			CloudOptionSetting = Mathf.Min(CloudOptionSetting + 1, CloudOptionChoices.Length - 1);
			break;
		case 1:
			SoundVolumeSetting = Mathf.Min(SoundVolumeSetting + 0.05f, 1f);
			break;
		case 2:
			MusicVolumeSetting = Mathf.Min(MusicVolumeSetting + 0.05f, 1f);
			break;
		}
	}

	void HideDeletedMessage() {
		ShowingDeletedMessage = false;
	}

	void OnGUI() {

		if (Screen.height <= 600) {
			GUI.skin = LowResSkin;
		} else {
			GUI.skin = MySkin;
		}

		int HighlightBorderSize = 5;

		// Cloud shadow highlight
		if (CurrentButtonIndex == 0)
		{
			Rect HighlightBoxRect = GUIUtilityFunctions.GetRectFromScreenPerc(OptionBarXPerc, OptionBarYPerc, OptionButtonWPerc * CloudOptionChoices.Length, OptionButtonHPerc);
			
			HighlightBoxRect.xMin -= HighlightBorderSize;
			HighlightBoxRect.xMax += HighlightBorderSize;
			HighlightBoxRect.yMin -= HighlightBorderSize;
			HighlightBoxRect.yMax += HighlightBorderSize;
			
			GUI.Box( HighlightBoxRect, "", "SelectedButton" );
		}

		// Cloud Shadow Option
		GUI.Label(GUIUtilityFunctions.GetRectFromScreenPerc(OptionLabelXPerc, OptionBarYPerc, OptionLabelWPerc, OptionButtonHPerc),
		          CloudOptionName, OptionNameStyle);
		CloudOptionSetting = GUI.Toolbar(GUIUtilityFunctions.GetRectFromScreenPerc(OptionBarXPerc, OptionBarYPerc, OptionButtonWPerc * CloudOptionChoices.Length, OptionButtonHPerc),
		                                 CloudOptionSetting, CloudOptionChoices);

		// Sound volume highlight
		if (CurrentButtonIndex == 1)
		{
			Rect HighlightBoxRect = GUIUtilityFunctions.GetRectFromScreenPerc(OptionBarXPerc, OptionBarYPerc + OptionButtonIncrementYPerc + OptionSliderYPercAdd,
			                                                                  OptionSliderWPerc, OptionButtonHPerc);

			HighlightBoxRect.xMin -= HighlightBorderSize;
			HighlightBoxRect.xMax += HighlightBorderSize;
			HighlightBoxRect.yMin -= HighlightBorderSize;
			HighlightBoxRect.yMax += HighlightBorderSize;
			
			GUI.Box( HighlightBoxRect, "", "SelectedButton" );
		}

		// Sound Volume Slider
		GUI.Label(GUIUtilityFunctions.GetRectFromScreenPerc(OptionLabelXPerc, OptionBarYPerc + OptionButtonIncrementYPerc,
		                                                    OptionLabelWPerc, OptionButtonHPerc), SoundVolumeName, OptionNameStyle);
		SoundVolumeSetting = GUI.HorizontalSlider(GUIUtilityFunctions.GetRectFromScreenPerc(OptionBarXPerc, OptionBarYPerc + OptionButtonIncrementYPerc + OptionSliderYPercAdd,
																							OptionSliderWPerc, OptionButtonHPerc),
		                                          SoundVolumeSetting, 0f, 1f);

		// Music volume highlight
		if (CurrentButtonIndex == 2)
		{
			Rect HighlightBoxRect = GUIUtilityFunctions.GetRectFromScreenPerc(OptionBarXPerc, OptionBarYPerc + 2 * OptionButtonIncrementYPerc + OptionSliderYPercAdd,
			                                                                  OptionSliderWPerc, OptionButtonHPerc);

			HighlightBoxRect.xMin -= HighlightBorderSize;
			HighlightBoxRect.xMax += HighlightBorderSize;
			HighlightBoxRect.yMin -= HighlightBorderSize;
			HighlightBoxRect.yMax += HighlightBorderSize;
			
			GUI.Box( HighlightBoxRect, "", "SelectedButton" );
		}

		// Music Volume Slider
		GUI.Label(GUIUtilityFunctions.GetRectFromScreenPerc(OptionLabelXPerc, OptionBarYPerc + 2 * OptionButtonIncrementYPerc,
		                                                    OptionLabelWPerc, OptionButtonHPerc), MusicVolumeName, OptionNameStyle);
		MusicVolumeSetting = GUI.HorizontalSlider(GUIUtilityFunctions.GetRectFromScreenPerc(OptionBarXPerc, OptionBarYPerc + 2 * OptionButtonIncrementYPerc + OptionSliderYPercAdd,
		                                                                                    OptionSliderWPerc, OptionButtonHPerc),
		                                          MusicVolumeSetting, 0f, 1f);

		// Delete save data highlight
		if (CurrentButtonIndex == 3)
		{
			Rect HighlightBoxRect = GUIUtilityFunctions.GetRectFromScreenPerc(TitleButtonXPerc, OptionBarYPerc + 3 * OptionButtonIncrementYPerc + OptionSliderYPercAdd,
			                                                                  OptionSliderWPerc * 1.2f, OptionButtonHPerc * 1.2f);
			
			HighlightBoxRect.xMin -= HighlightBorderSize;
			HighlightBoxRect.xMax += HighlightBorderSize;
			HighlightBoxRect.yMin -= HighlightBorderSize;
			HighlightBoxRect.yMax += HighlightBorderSize;
			
			GUI.Box( HighlightBoxRect, "", "SelectedButton" );
		}

		// Delete save data button
		if (GUI.RepeatButton(GUIUtilityFunctions.GetRectFromScreenPerc(TitleButtonXPerc, OptionBarYPerc + 3 * OptionButtonIncrementYPerc + OptionSliderYPercAdd,
		                                                                                  OptionSliderWPerc * 1.2f, OptionButtonHPerc * 1.2f), DeleteSaveDataString))
		{
			HoldingDeleteCountdownMouse = true;
		}
		else
		{
			HoldingDeleteCountdownMouse = false;
		}

		if (PressedCurrentButton && CurrentButtonIndex == 3)
		{

			HoldingDeleteCountdownKey = true;
		}

		// Delete save data countdown
		if ((HoldingDeleteCountdownMouse || HoldingDeleteCountdownKey) && !DeletedDataThisPress)
		{
			GUI.Label(GUIUtilityFunctions.GetRectFromScreenPerc(TitleButtonXPerc + 0.5f * OptionSliderWPerc * 1.2f + 0.1f,
		                                                    	OptionBarYPerc + 3 * OptionButtonIncrementYPerc + OptionSliderYPercAdd - 0.025f,
		                                                    	0.1f, 0.1f), Mathf.Ceil(DeleteCountdown).ToString(), "LevelTitle");
		}

		// Title screen highlight
		if (CurrentButtonIndex == NumItems - 1)
		{
			Rect HighlightBoxRect = new Rect(Screen.width * 0.5f + Screen.height * (TitleButtonXPerc - 0.5f - TitleButtonWPerc * 0.5f),
			                                 Screen.height * (TitleButtonYPerc - TitleButtonHPerc * 0.5f),
			                                 Screen.height * TitleButtonWPerc, Screen.height * TitleButtonHPerc);

			HighlightBoxRect.xMin -= HighlightBorderSize;
			HighlightBoxRect.xMax += HighlightBorderSize;
			HighlightBoxRect.yMin -= HighlightBorderSize;
			HighlightBoxRect.yMax += HighlightBorderSize;
			
			GUI.Box( HighlightBoxRect, "", "SelectedButton" );
		}

		// Title screen button
		// Saves player prefs before returning to title screen
		if (GUI.Button(new Rect(Screen.width * 0.5f + Screen.height * (TitleButtonXPerc - 0.5f - TitleButtonWPerc * 0.5f),
		                        Screen.height * (TitleButtonYPerc - TitleButtonHPerc * 0.5f),
		                        Screen.height * TitleButtonWPerc, Screen.height * TitleButtonHPerc), TitleButtonText)
		    		|| (PressedCurrentButton && CurrentButtonIndex == NumItems - 1)) {
			LawnGameControl.PlayButtonPressSound();
			UpdatePlayerPrefs();
			PlayerPrefs.Save();
			Application.LoadLevel(TitleSceneName);
		}

		if (ShowingDeletedMessage) {
			GUI.Label(GUIUtilityFunctions.GetRectFromScreenPerc(DeletedXPerc, DeletedYPerc, DeletedWPerc, DeletedHPerc), DeletedString, "LevelTitle");
		}

		PressedCurrentButton = false;
	}

	void UpdatePlayerPrefs() {
		PlayerPrefs.SetInt("CloudShadows", CloudOptionSetting);
		PlayerPrefs.SetFloat("SoundVolume", SoundVolumeSetting);
		PlayerPrefs.SetFloat("MusicVolume", MusicVolumeSetting);
	}
}
