/// <summary>
/// Lawn score keeper
/// 
/// Programmer: Carl Childers
/// Date: 4/25/2015
/// 
/// Keeps track of how many tiles have been mowed, and other scoring needs.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LawnScoreKeeper : MonoBehaviour {

	public GUISkin MySkin;
	public string GrassTag = "Grass";
	public string ScorchTag = "Scorch";
	public Transform StageCompletePrefab;
	public Transform FailurePrefab;
	public string LevelSelectScene;
	public int SThreshold = 0; // probably temp, as this will be gotten from somewhere else
	public int ParSeconds = 60; // Displayed in clear screen
	public int AllPestBonus = 100;
	public int PenaltyPerLostRank = 500; // lose a rank every x points
	public float RankXPerc = 0.5f;
	public float RankYPerc = 0.3333f;
	public float RankWPerc = 0.25f;
	public float RankHPerc = 0.25f;
	public float LevelBtnXPerc = 0.5f;
	public float LevelBtnYPerc = 0.875f;
	public float LevelBtnWPerc = 0.25f;
	public float LevelBtnHPerc = 0.0833f;
	public float RetryBtnYPerc = 0.8333f;
	public float FailedLevelBtnYPerc = 0.9306f;
	public float ScoreXPerc = 0.5f;
	public float ScoreYPerc = 0.5f;
	public float ScoreIncrementYPerc = 0.0556f;
	public float ScoreWPerc = 0.5556f;
	public float ScoreHPerc = 0.0556f;
	public float BackBoxXPerc = 0.5f;
	public float BackBoxYPerc = 0.75f;
	public float BackBoxWPerc = 0.5f;
	public float BackBoxHPerc = 0.5f;
	public float PenaltyBoxXPerc = 0.8f;
	public float PenaltyBoxYPerc = 0.75f;
	public float PenaltyBoxWPerc = 0.2f;
	public float PenaltyBoxHPerc = 0.4f;
	public string BackBoxStyle = "BackBox";
	public float FinishScreenDelay = 2.0f;
	public string NextLevelString = "Next Level";
	public string LevelSelectString = "Level Select";
	public string RetryString = "Retry Level";
	public string ParText = "Par";
	public float ParXPerc = 0.75f, ParWPerc = 0.25f;
	public string FlowerPenaltyTag = "Flowers";
	public string DamagePenaltyTag = "Injury";
	public string FirePenaltyTag = "Scorches";

	public GUIStyle ScoreStyle, PenaltyStyle;
	//public GUIStyle ButtonStyle;
	
	public Texture2D[] RankTextures;

	public AudioClip FinishSound;
	public AudioClip FailureSound;

	int TotalGrassTiles, GrassTilesMowed, FlowerTilesMowed;
	bool IsStageComplete;
	bool IsStageFailed;

	int TimeBonus = 0, PestBonus = 0, Penalty = 0, TotalScore = 0;
	int PestThreshold = 0; // contributed to by pests.

	string ParString;
	List<string> PenaltyTags;

	bool ShowingNextLevelButton;


	public enum EScoreRank {
		SR_D = 0,
		SR_C = 1,
		SR_B = 2,
		SR_A = 3,
		SR_S = 4
	};

	EScoreRank LevelRank;
	//string RankString; // will probably use a texture later

	int FinishTime; // time the player had when finishing

	// For menus that appear on stage completion
	bool PressedCurrentButton;
	int CurrentButtonIndex;
	const int FailedMenuNumButtons = 2;

	// Delay finish screen
	float FinishScreenTimer = 0;


	void Start() {
		GameObject[] allTiles = GameObject.FindGameObjectsWithTag(GrassTag);
		TotalGrassTiles = allTiles.Length;

		ShowingNextLevelButton = false;
	}
	
	// Message sent from lawn mower object when a grass tile is mowed
	void GrassMowed() {
		GrassTilesMowed++;
		//print ("Grass Tiles Mowed: " + GrassTilesMowed);
		if (!IsStageComplete && !IsStageFailed && GrassTilesMowed >= TotalGrassTiles) {
			OnStageComplete();
		}
	}

	// Message sent during the start of the level by enemies
	void AddToPestThreshold(int inAmt)
	{
		PestThreshold += inAmt;
	}

	// Message sent when an enemy is killed
	void AddToPestBonus(int inAmt)
	{
		PestBonus += inAmt;
	}

	// Message sent from lawn mower player when killed
	void PlayerDied() {
		if (IsStageComplete)
		{
			return;
		}

		IsStageFailed = true;
		if (FailurePrefab != null) {
			Instantiate(FailurePrefab, FailurePrefab.position, FailurePrefab.rotation);
		}
		if (FailureSound != null) {
			AudioSource.PlayClipAtPoint(FailureSound, Camera.main.transform.position);
		}
		Camera.main.transform.BroadcastMessage("StopMusic", SendMessageOptions.DontRequireReceiver);

		LevelTimer theTimer = GetComponentInChildren<LevelTimer>();
		if (theTimer != null) {
			theTimer.SendMessage("StopTimer");
		}

		DisableWeaponSelect();

		Screen.showCursor = true;
	}

	void OnStageComplete() {
		IsStageComplete = true;
		if (StageCompletePrefab != null) {
			Instantiate (StageCompletePrefab, StageCompletePrefab.position, StageCompletePrefab.rotation);
		}
		if (FinishSound != null) {
			AudioSource.PlayClipAtPoint(FinishSound, Camera.main.transform.position);
		}
		Camera.main.transform.BroadcastMessage("StopMusic", SendMessageOptions.DontRequireReceiver);

		Transform theLawnMower;
		theLawnMower = FindObjectOfType<CutGrass>().transform;
		if (theLawnMower != null) {
			// disable lawn mower movement
			VehicleMovement2D vehicleScript = theLawnMower.GetComponent<VehicleMovement2D>();
			if (vehicleScript != null) {
				vehicleScript.enabled = false;
			}
			// disable weapons firing
			theLawnMower.BroadcastMessage("TurnOff", SendMessageOptions.DontRequireReceiver);
			theLawnMower.BroadcastMessage("Disable", SendMessageOptions.DontRequireReceiver);

			theLawnMower.SendMessage("StartTurningOff");

			theLawnMower.BroadcastMessage("LevelCompleted");

			DisableWeaponSelect();

			// raise lawn mower's mass so he doesn't get pushed
			theLawnMower.rigidbody2D.mass += 100000;
		}

		LevelTimer theTimer = GetComponentInChildren<LevelTimer>();
		if (theTimer != null) {
			theTimer.SendMessage("StopTimer");

			TimeBonus = GetTimeBonus( theTimer.GetTime() );
		}

		PenaltyTags = new List<string>();

		// Determine penalty
		Penalty = 100 * FlowerTilesMowed;
		if (FlowerTilesMowed > 0)
			PenaltyTags.Add(FlowerPenaltyTag);

		int numScorches = GameObject.FindGameObjectsWithTag(ScorchTag).Length;
		Penalty += 20 * numScorches;
		if (numScorches > 0)
			PenaltyTags.Add(FirePenaltyTag);

		if (theLawnMower != null) {
			Health healthScript = theLawnMower.GetComponentInChildren<Health>();
			if (healthScript != null) {
				float healthLost = healthScript.MaxHealth - healthScript.GetHealth();
				if (healthScript.MaxHealth / 4.0f > 0) {
					int healthPenalty = (int)(healthLost / (healthScript.MaxHealth / 4.0f));
					Penalty += healthPenalty * 50;

					if (healthPenalty > 0)
						PenaltyTags.Add(DamagePenaltyTag);
				}
			}
		}

		TotalScore = TimeBonus + PestBonus - Penalty;
		if (PestBonus > 0 && PestBonus >= PestThreshold) {
			TotalScore += AllPestBonus;
		}

		// determine level rank
		EScoreRank MaxRank = EScoreRank.SR_S;
		if (Penalty > 0 && PenaltyPerLostRank > 0) {
			MaxRank = (EScoreRank)Mathf.Max(0, 3 - (Penalty / PenaltyPerLostRank));
		}

		if (SThreshold <= 0) {
			// just give max rank
			LevelRank = MaxRank;
		} else {
			float divisor = SThreshold / 4.0f;
			float result = TotalScore / divisor;

			if (result < 1.0f) {
				LevelRank = EScoreRank.SR_D;
			} else if (result < 2.0f) {
				LevelRank = EScoreRank.SR_C;
			} else if (result < 3.0f) {
				LevelRank = EScoreRank.SR_B;
			} else if (result < 4.0f) {
				LevelRank = EScoreRank.SR_A;
			} else {
				// just give max rank
				LevelRank = MaxRank;
			}
		}

		LevelRank = (EScoreRank)Mathf.Min((int)LevelRank, (int)MaxRank);

		//RankString = GetRankString();

		FinishTime = theTimer.GetTime();
		Screen.showCursor = true;
		FinishScreenTimer = FinishScreenDelay;
		ParString = GetTimeString(ParSeconds);

		LawnGameControl gc = LawnGameControl.GetGameControl();
		if (gc != null && CanGoToNextLevel(gc)) {
			ShowingNextLevelButton = true;
		}
	}


	int GetTimeBonus(int timeSeconds) {
		if (timeSeconds <= 30) {
			return 4500;
		}
		if (timeSeconds <= 60) {
			return (4500 - (timeSeconds - 30) * 50);
		}
		if (timeSeconds <= 120) {
			return (3000 - (timeSeconds - 60) * 20);
		}
		if (timeSeconds <= 300) {
			return (1800 - (timeSeconds - 120) * 10);
		}
		return 0;
	}

	// Message sent from lawn mower object when a flower tile is mowed
	// This is a bad thing, and lowers the player's rank
	void FlowerMowed() {
		FlowerTilesMowed++;
	}

	void Update()
	{
		if ((IsStageComplete || IsStageFailed) && Input.GetButtonDown("PressMenuButton"))
		{
			PressedCurrentButton = true;
		}

		if (FinishScreenTimer > 0)
		{
			FinishScreenTimer = Mathf.Max(FinishScreenTimer - Time.deltaTime, 0);
			if (PressedCurrentButton)
			{
				FinishScreenTimer = 0;
				PressedCurrentButton = false; // Eat the input so you can still see the score sheet
			}
		}
	}

	void MenuUp()
	{
		if (IsStageFailed)
		{
			CurrentButtonIndex--;
			if (CurrentButtonIndex < 0)
				CurrentButtonIndex = FailedMenuNumButtons - 1;
		}
		else if (ShowingNextLevelButton)
		{
			CurrentButtonIndex--;
			if (CurrentButtonIndex < 0)
				CurrentButtonIndex = 1;
		}
	}

	void MenuDown()
	{
		if (IsStageFailed)
		{
			CurrentButtonIndex++;
			if (CurrentButtonIndex >= FailedMenuNumButtons)
				CurrentButtonIndex = 0;
		}
		else if (ShowingNextLevelButton)
		{
			CurrentButtonIndex++;
			if (CurrentButtonIndex >= 2)
				CurrentButtonIndex = 0;
		}
	}

	void GotoNextLevel()
	{
		LawnGameControl gc = LawnGameControl.GetGameControl();
		if (gc != null && CanGoToNextLevel(gc)) {
			gc.SetLevelIndex(gc.GetLevelIndex() + 1);
			Application.LoadLevel (gc.AllLevelSelectData[gc.GetLevelIndex()].LevelName);
		}
	}

	bool CanGoToNextLevel(LawnGameControl inGameControl) {
		if (inGameControl.GetLevelIndex() + 1 < inGameControl.AllLevelSelectData.Length
		    		&& inGameControl.GetLevelIndex() + 1 >= 0) {
			return true;
		}
		return false;
	}

	void OnGUI() {
		GUI.skin = MySkin;

		int HighlightBorderSize = 5;

		// Post-clear GUI
		if (IsStageComplete && FinishScreenTimer <= 0) {
			// Back box
			GUI.Box(GUIUtilityFunctions.GetRectFromScreenPerc(BackBoxXPerc, BackBoxYPerc, BackBoxWPerc, BackBoxHPerc), "", BackBoxStyle);
			GUI.Box(GUIUtilityFunctions.GetRectFromScreenPerc(PenaltyBoxXPerc, PenaltyBoxYPerc, PenaltyBoxWPerc, PenaltyBoxHPerc), "", BackBoxStyle);

			// Display score
			Rect btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(ScoreXPerc, ScoreYPerc, ScoreWPerc, ScoreHPerc);
			GUI.Label(btnRect, "Time Bonus: " + TimeBonus.ToString(), ScoreStyle);
			btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(ScoreXPerc, ScoreYPerc + ScoreIncrementYPerc, ScoreWPerc, ScoreHPerc);
			GUI.Label(btnRect, "Penalty: " + Penalty.ToString(), (Penalty > 0 ? PenaltyStyle : ScoreStyle));
			btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(ScoreXPerc, ScoreYPerc + 2 * ScoreIncrementYPerc, ScoreWPerc, ScoreHPerc);
			GUI.Label(btnRect, "Pest Bonus: " + PestBonus.ToString(), ScoreStyle);
			if (PestBonus > 0 && PestBonus >= PestThreshold) {
				btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(ScoreXPerc, ScoreYPerc + 3 * ScoreIncrementYPerc, ScoreWPerc, ScoreHPerc);
				GUI.Label(btnRect, "All Pest Bonus: " + AllPestBonus.ToString(), ScoreStyle);
			}
			btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(ScoreXPerc, ScoreYPerc + 4 * ScoreIncrementYPerc, ScoreWPerc, ScoreHPerc);
			GUI.Label(btnRect, "Total Score: " + TotalScore.ToString(), ScoreStyle);

			int rankIndex = (int)LevelRank;
			if (rankIndex >= 0 && rankIndex < RankTextures.Length) {
				Rect rankRect = GUIUtilityFunctions.GetRectFromScreenPerc(RankXPerc, RankYPerc, RankWPerc, RankHPerc);
				GUI.DrawTexture(rankRect, RankTextures[rankIndex]);
			}

			// Par and Penalties
			btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(ParXPerc, ScoreYPerc, ParWPerc, ScoreHPerc);
			GUI.Label(btnRect, ParText + ": " + ParString, ScoreStyle);

			if (PenaltyTags != null)
			{
				for (int i = 0; i < PenaltyTags.Count; ++i)
				{
					btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(ParXPerc, ScoreYPerc + ScoreIncrementYPerc * (i + 1), ParWPerc, ScoreHPerc);
					GUI.Label(btnRect, PenaltyTags[i], PenaltyStyle);
				}
			}

			// If there is a next level, show next level button
			if (ShowingNextLevelButton)
			{
				btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(LevelBtnXPerc, LevelBtnYPerc, LevelBtnWPerc, LevelBtnHPerc);

				// Next level highlight
				if (CurrentButtonIndex == 0)
				{
					Rect HighlightBoxRect = btnRect;
					
					HighlightBoxRect.xMin -= HighlightBorderSize;
					HighlightBoxRect.xMax += HighlightBorderSize;
					HighlightBoxRect.yMin -= HighlightBorderSize;
					HighlightBoxRect.yMax += HighlightBorderSize;
					
					GUI.Box( HighlightBoxRect, "", "SelectedButton" );
				}

				// Next level button

				if (GUI.Button(btnRect, NextLevelString) || (PressedCurrentButton && CurrentButtonIndex == 0))
				{
					GotoNextLevel();
					LawnGameControl.PlayButtonPressSound();
				}
			}

			float LvlSelectBtnY = LevelBtnYPerc;
			if (ShowingNextLevelButton)
				LvlSelectBtnY += 0.1f;

			btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(LevelBtnXPerc, LvlSelectBtnY, LevelBtnWPerc, LevelBtnHPerc);

			// Level select highlight
			if (!ShowingNextLevelButton || CurrentButtonIndex == 1)
			{
				Rect HighlightBoxRect = btnRect;
				
				HighlightBoxRect.xMin -= HighlightBorderSize;
				HighlightBoxRect.xMax += HighlightBorderSize;
				HighlightBoxRect.yMin -= HighlightBorderSize;
				HighlightBoxRect.yMax += HighlightBorderSize;
				
				GUI.Box( HighlightBoxRect, "", "SelectedButton" );
			}

			// Back to level select button

			if (GUI.Button (btnRect, LevelSelectString) || (PressedCurrentButton && (!ShowingNextLevelButton || CurrentButtonIndex == 1))) {

				// Record level scores
				LawnGameControl gControl = LawnGameControl.GetGameControl();
				gControl.RecordLevelScores(LevelRank, TotalScore, FinishTime);

				LawnGameControl.PlayButtonPressSound();
				Application.LoadLevel (LevelSelectScene);
			}
		} else if (IsStageFailed) {

			// Retry highlight
			Rect btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(LevelBtnXPerc, RetryBtnYPerc, LevelBtnWPerc, LevelBtnHPerc);

			if (CurrentButtonIndex == 0)
			{
				Rect HighlightBoxRect = btnRect;
				
				HighlightBoxRect.xMin -= HighlightBorderSize;
				HighlightBoxRect.xMax += HighlightBorderSize;
				HighlightBoxRect.yMin -= HighlightBorderSize;
				HighlightBoxRect.yMax += HighlightBorderSize;
				
				GUI.Box( HighlightBoxRect, "", "SelectedButton" );
			}

			// Retry button

			if (GUI.Button (btnRect, RetryString) || (PressedCurrentButton && CurrentButtonIndex == 0)) {
				LawnGameControl.PlayButtonPressSound();
				Application.LoadLevel(Application.loadedLevel);
			}

			// Level select highlight
			btnRect = GUIUtilityFunctions.GetRectFromScreenPerc(LevelBtnXPerc, FailedLevelBtnYPerc, LevelBtnWPerc, LevelBtnHPerc);

			if (CurrentButtonIndex == 1)
			{
				Rect HighlightBoxRect = btnRect;
				
				HighlightBoxRect.xMin -= HighlightBorderSize;
				HighlightBoxRect.xMax += HighlightBorderSize;
				HighlightBoxRect.yMin -= HighlightBorderSize;
				HighlightBoxRect.yMax += HighlightBorderSize;
				
				GUI.Box( HighlightBoxRect, "", "SelectedButton" );
			}

			// Back to level select button
			
			if (GUI.Button (btnRect, LevelSelectString) || (PressedCurrentButton && CurrentButtonIndex == 1)) {
				LawnGameControl.PlayButtonPressSound();
				Application.LoadLevel(LevelSelectScene);
			}
		}

		PressedCurrentButton = false;
	}

	void DisableWeaponSelect() {
		WeaponChoice weapChoiceScript = FindObjectOfType(typeof(WeaponChoice)) as WeaponChoice;
		if (weapChoiceScript != null) {
			weapChoiceScript.SendMessage("Disable");
		}
	}

	// Returns a minutes : seconds string from seconds
	string GetTimeString(int inSeconds)
	{
		string timeString, secondsString;
		int mins, secs;

		mins = inSeconds / 60;
		secs = inSeconds - mins * 60;

		secondsString = secs.ToString();
		if (secs < 10) {
			secondsString = "0" + secondsString;
		}
		timeString = mins.ToString() + ":" + secondsString;
		return timeString;
	}
}
