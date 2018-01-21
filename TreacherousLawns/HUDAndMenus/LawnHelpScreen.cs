/// <summary>
/// Lawn help screen
/// 
/// Programmer: Carl Childers
/// Date: 8/23/2015
/// 
/// Help Screen buttons.
/// </summary>

using UnityEngine;
using System.Collections;

public class LawnHelpScreen : MonoBehaviour {

	public GUISkin MySkin;
	public GUISkin LowResSkin;

	public string PreviousSceneName;
	public string TitleSceneName;
	public string NextSceneName;

	public string TitleButtonText = "Title";

	public float PrevButtonXPerc = 0.35f;
	public float TitleButtonXPerc = 0.5f;
	public float NextButtonXPerc = 0.65f;
	public float ButtonYPerc = 0.85f;
	public float PrevButtonWPerc = 0.06f;
	public float TitleButtonWPerc = 0.2f;
	public float ButtonHPerc = 0.1f;

	public bool ShouldShowPrevious = true;
	public bool ShouldShowNext = true;
	public bool AllowControlTryOut = false;

	public string TryOutControlsText;
	public string StopTryOutText;
	public float TryOutTextXPerc = 0.75f;
	public float TryOutTextYPerc = 0.75f;
	public Vector3 PlayerSpawnPos;
	public Transform TestPlayerPrefab;
	public Transform PlayerSpawnEffect, PlayerDespawnEffect;

	bool PressedCurrentButton;
	bool TryingOutControls;
	Transform SpawnedPlayer;


	void Update() {

		if (!TryingOutControls)
		{
			if (Input.GetButtonDown("PressMenuButton"))
			{
				PressedCurrentButton = true;
			}
		}

		if (AllowControlTryOut)
		{
			if (Input.GetButtonDown("Pause"))
			{
				TryingOutControls = !TryingOutControls;
				if (TryingOutControls)
				{
					// Spawn a test player
					if (TestPlayerPrefab != null)
					{
						SpawnedPlayer = (Transform)Instantiate(TestPlayerPrefab, PlayerSpawnPos, TestPlayerPrefab.rotation);

						WeaponChoice theWeaponChoice = FindObjectOfType<WeaponChoice>();
						WeaponEquip playerWeaponEquip = FindObjectOfType<WeaponEquip>();
						if (theWeaponChoice != null && playerWeaponEquip != null)
						{
							theWeaponChoice.WeaponParent = playerWeaponEquip.transform;
							theWeaponChoice.InitWeapons();
						}

						if (PlayerSpawnEffect != null)
							Instantiate(PlayerSpawnEffect, PlayerSpawnPos, Quaternion.identity);
					}
				}
				else
				{
					if (SpawnedPlayer != null)
					{
						if (PlayerDespawnEffect != null)
							Instantiate(PlayerDespawnEffect, SpawnedPlayer.position, Quaternion.identity);

						GameObject.Destroy(SpawnedPlayer.gameObject);
					}
				}
			}
		}
	}

	void MenuLeft()
	{
		if (!TryingOutControls)
		{
			if (ShouldShowPrevious)
			{
				LawnGameControl.PlayButtonPressSound();
				Application.LoadLevel(PreviousSceneName);
			}
		}
	}

	void MenuRight()
	{
		if (!TryingOutControls)
		{
			if (ShouldShowNext)
			{
				LawnGameControl.PlayButtonPressSound();
				Application.LoadLevel(NextSceneName);
			}
		}
	}

	void OnGUI() {
		if (Screen.height <= 600) {
			GUI.skin = LowResSkin;
		} else {
			GUI.skin = MySkin;
		}

		if (!TryingOutControls)
		{
			float HighlightBorderSize = 5.0f;

			if (ShouldShowPrevious) {
				if (GUI.Button(new Rect(Screen.width * 0.5f + Screen.height * (PrevButtonXPerc - 0.5f - PrevButtonWPerc * 0.5f),
				               Screen.height * (ButtonYPerc - ButtonHPerc * 0.5f),
				               Screen.height * PrevButtonWPerc, Screen.height * ButtonHPerc), "", "LeftArrow")) {
					LawnGameControl.PlayButtonPressSound();
					Application.LoadLevel(PreviousSceneName);
				}
			}

			// Highlight rect for title button
			Rect HighlightBoxRect = new Rect(Screen.width * 0.5f + Screen.height * (TitleButtonXPerc - 0.5f - TitleButtonWPerc * 0.5f),
			                                 Screen.height * (ButtonYPerc - ButtonHPerc * 0.5f),
			                                 Screen.height * TitleButtonWPerc, Screen.height * ButtonHPerc);
			
			HighlightBoxRect.xMin -= HighlightBorderSize;
			HighlightBoxRect.xMax += HighlightBorderSize;
			HighlightBoxRect.yMin -= HighlightBorderSize;
			HighlightBoxRect.yMax += HighlightBorderSize;
			
			GUI.Box( HighlightBoxRect, "", "SelectedButton" );

			// Title button
			if (GUI.Button(new Rect(Screen.width * 0.5f + Screen.height * (TitleButtonXPerc - 0.5f - TitleButtonWPerc * 0.5f),
			                        Screen.height * (ButtonYPerc - ButtonHPerc * 0.5f),
			                        Screen.height * TitleButtonWPerc, Screen.height * ButtonHPerc), TitleButtonText)
			    				|| (PressedCurrentButton)) {
				LawnGameControl.PlayButtonPressSound();
				Application.LoadLevel(TitleSceneName);
			}

			if (ShouldShowNext) {
				if (GUI.Button(new Rect(Screen.width * 0.5f + Screen.height * (NextButtonXPerc - 0.5f - PrevButtonWPerc * 0.5f),
				                        Screen.height * (ButtonYPerc - ButtonHPerc * 0.5f),
				                        Screen.height * PrevButtonWPerc, Screen.height * ButtonHPerc), "", "RightArrow")) {
					LawnGameControl.PlayButtonPressSound();
					Application.LoadLevel(NextSceneName);
				}
			}
		}

		if (AllowControlTryOut)
		{
			string currentText = "";
			if (!TryingOutControls)
				currentText = TryOutControlsText;
			else
				currentText = StopTryOutText;

			GUI.Label(GUIUtilityFunctions.GetRectFromScreenPerc(TryOutTextXPerc, TryOutTextYPerc, 0.5f, 0.5f), currentText, "OptionName");
		}

		PressedCurrentButton = false;
	}
}
