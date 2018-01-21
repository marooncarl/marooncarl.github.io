/// <summary>
/// Lawn level menu
/// 
/// Programmer: Carl Childers
/// Date: 4/26/2015
/// 
/// Level menu script for the lawn mowing game.
/// </summary>

using UnityEngine;
using System.Collections;

public class LawnLevelMenu : MonoBehaviour {

	public GUISkin MySkin;
	public GUISkin LowResSkin;

	public string TitleScreenName;

	public string LevelStartTag = "Lawn_"; // this starting tag will be removed when displaying the level name

	// width and height percents (WPerc and HPerc) are percents of the screen height

	public float LeftColumnXPerc = 0.5f;
	public float RightColumnXPerc = 0.75f;
	public float[] LevelRowYPerc = {0.25f, 0.5f, 0.75f};
	public float LevelBtnWPerc = 0.4f;
	public float LevelBtnHPerc = 0.2f;
	public float PageChangeYPerc = 0.92f;
	public float PageChangeWPerc = 0.1f;
	public float PageChangeHPerc = 0.1f;
	public int ButtonsPerPage = 6;

	public float ButtonRankWPerc = 0.1f;
	public float ButtonRankHPerc = 0.1f;

	public float PreviewXPerc = 0.0f;
	public float PreviewYPerc = 0.5f;
	public float PreviewWPerc = 0.5f;
	public float PreviewHPerc = 0.5f;

	public float PreviewTextureWPerc = 0.45f;
	public float PreviewTextureHPerc = 0.45f;

	public float PreviewRankYPerc = 0.75f;
	public float PreviewRankWPerc = 0.07f;
	public float PreviewRankHPerc = 0.07f;

	public float ScoreXPerc = 0.0f;
	public float ScoreYPerc = 0.75f;
	public float ScoreWPerc = 0.4f;
	public float ScoreHPerc = 0.2f;

	public float TitleBtnXPerc = 0.625f;
	public float TitleBtnWPerc = 0.3f;

	public float NotWideScreenScale = 0.9f;

	public string LevelTitleStyle;

	public string StrTitle = "Title";
	public string StrHighScore = "High Score";
	public string StrBestTime = "Best Time";

	public Texture2D DefaultPreviewTexture;
	public Texture2D[] RankTextures;
	public Texture2D[] SmallRankTextures;

	public GUIText TitleText; // GUI Text showing the screen title; will have page number added to it

	int CurrentPage;
	int MaxPage;
	int DisplayLevelIndex;
	int MaxLevelIndex;
	Rect[] LevelButtonRects; // saved for checking if mouse overlaps one

	string TitleOriginalText;

	Vector2 PrevMousePos;

	// Keyboard support
	bool PressedCurrentButton;
	int CurrentRow, CurrentColumn, NumRows, NumColumns;
	bool SelectingTitleButton;


	void Awake() {
		CurrentPage = 0;
		DisplayLevelIndex = 0;

		LawnGameControl gControl = LawnGameControl.GetGameControl();

		if (ButtonsPerPage > 0 && gControl != null) {
			MaxPage = (int)Mathf.Ceil((float)gControl.AllLevelSelectData.Length / (float)ButtonsPerPage) - 1;
		} else {
			MaxPage = 0;
		}

		if (gControl != null) {
			MaxLevelIndex = gControl.AllLevelSelectData.Length;
			CurrentPage = gControl.LastLevelPage;
		}

		if (ButtonsPerPage > 0) {
			LevelButtonRects = new Rect[6];
		}

		if (TitleText != null) {
			TitleOriginalText = TitleText.text;
			UpdatePageNumber(CurrentPage);
		}

		NumRows = 3;
		NumColumns = 2;
	}

	void Update() {
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		mousePos.y = Screen.height - mousePos.y;

		if ((mousePos - PrevMousePos).magnitude > 2)
		{
			for (int i = 0; i < LevelButtonRects.Length; ++i) {
				if (LevelButtonRects[i].Contains(mousePos)) {
					if (CurrentPage * ButtonsPerPage + i >= MaxLevelIndex) {
						break;
					}

					DisplayLevelIndex = CurrentPage * ButtonsPerPage + i;
					break;
				}
			}
		}

		PrevMousePos = mousePos;
		
		if (Input.GetButtonDown("PressMenuButton"))
		{
			PressedCurrentButton = true;
		}
	}

	// Keyboard input messages

	void MenuUp()
	{
		if (!SelectingTitleButton)
		{
			CurrentRow--;
			if (CurrentRow < 0)
				CurrentRow = 0;
			
			UpdateDisplayLevelIndex();
		}
		else
		{
			SelectingTitleButton = false;
			CurrentColumn = 0;
			CurrentRow = NumRows - 1;

			while (CurrentRow > 0 && !IsRowColumnValid(CurrentRow, CurrentColumn))
			{
				CurrentRow--;
			}

			UpdateDisplayLevelIndex();
		}
	}

	void MenuDown()
	{
		if (!SelectingTitleButton)
		{
			CurrentRow++;
			if (CurrentRow >= NumRows || !IsRowColumnValid(CurrentRow, CurrentColumn))
			{
				CurrentRow = NumRows - 1;
				SelectingTitleButton = true;
			}
			
			UpdateDisplayLevelIndex();
		}
	}

	void MenuRight()
	{
		if (!SelectingTitleButton)
		{
			CurrentColumn++;
			if (CurrentColumn >= NumColumns)
			{
				// Go to next page, if not already on the last page
				if (CurrentPage < MaxPage) {
					LawnGameControl.PlayButtonPressSound();
					CurrentPage++;
					UpdatePageNumber(CurrentPage);
					CurrentColumn = 0;
				}
				else
					CurrentColumn = NumColumns - 1;
			}

			while (CurrentColumn > 0 && !IsRowColumnValid(CurrentRow, CurrentColumn))
			{
				CurrentColumn--;
			}
			
			UpdateDisplayLevelIndex();
		}
		else
		{
			// Go to next page, if not already on the last page
			if (CurrentPage < MaxPage) {
				LawnGameControl.PlayButtonPressSound();
				CurrentPage++;
				UpdatePageNumber(CurrentPage);
			}
		}
	}

	void MenuLeft()
	{
		if (!SelectingTitleButton)
		{
			CurrentColumn--;
			if (CurrentColumn < 0)
			{
				// Go to previous page, if not already on the first page
				if (CurrentPage > 0) {
					LawnGameControl.PlayButtonPressSound();
					CurrentPage--;
					UpdatePageNumber(CurrentPage);
					CurrentColumn = NumColumns - 1;
				}
				else
					CurrentColumn = 0;
			}
			
			UpdateDisplayLevelIndex();
		}
		else
		{
			// Go to previous page, if not already on the first page
			if (CurrentPage > 0) {
				LawnGameControl.PlayButtonPressSound();
				CurrentPage--;
				UpdatePageNumber(CurrentPage);
			}
		}
	}

	Rect PrepareRect(float inXPerc, float inYPerc, float inWPerc, float inHPerc) {
		Rect newRect = GUIUtilityFunctions.GetRectFromScreenPerc(inXPerc, inYPerc, inWPerc, inHPerc);
		newRect = GUIUtilityFunctions.ModifyRectByResRatio(newRect, NotWideScreenScale);
		return newRect;
	}

	void OnGUI() {
		if (Screen.height <= 600) {
			GUI.skin = LowResSkin;
		} else {
			GUI.skin = MySkin;
		}

		int HighlightBorderSize = 5;

		LawnGameControl gControl = LawnGameControl.GetGameControl();

		// Level preview window
		Rect nextRect = PrepareRect(PreviewXPerc, PreviewYPerc, PreviewWPerc, PreviewHPerc);
		GUI.Box(nextRect, "");

		// Selected level title
		nextRect = PrepareRect(PreviewXPerc, PreviewYPerc - PreviewHPerc / 2.0f - LevelBtnHPerc / 2.0f, PreviewWPerc, LevelBtnHPerc);
		GUI.Label(nextRect, GetDisplayName(gControl.AllLevelSelectData[DisplayLevelIndex].LevelName), LevelTitleStyle);

		Texture2D prevTexture;
		if (gControl.AllLevelSelectData[DisplayLevelIndex].PreviewTexture != null) {
			prevTexture = gControl.AllLevelSelectData[DisplayLevelIndex].PreviewTexture;
		} else {
			prevTexture = DefaultPreviewTexture;
		}

		nextRect = PrepareRect(PreviewXPerc, PreviewYPerc, PreviewTextureWPerc, PreviewTextureHPerc);
		GUI.DrawTexture(nextRect, prevTexture);

		// Level score window
		nextRect = PrepareRect(ScoreXPerc, ScoreYPerc, ScoreWPerc, ScoreHPerc);
		GUI.Box(nextRect, "");

		nextRect = PrepareRect(ScoreXPerc + ScoreWPerc * 0.1f, ScoreYPerc - ScoreHPerc / 8, ScoreWPerc * 0.8f, ScoreHPerc / 4);
		GUI.Label(nextRect, StrHighScore + ": " + gControl.GetHighScore(DisplayLevelIndex));

		nextRect = PrepareRect(ScoreXPerc + ScoreWPerc * 0.1f, ScoreYPerc + ScoreHPerc / 8, ScoreWPerc * 0.8f, ScoreHPerc / 4);
		GUI.Label(nextRect, StrBestTime + ": " + GetTimeString(gControl.GetBestTime(DisplayLevelIndex)));

		// Rank for the currently selected level, if any
		if (gControl.GetLevelComplete(DisplayLevelIndex)) {
			int rankIndex = (int)gControl.GetLevelRank(DisplayLevelIndex);
			if (rankIndex >= 0 && rankIndex < RankTextures.Length) {
				nextRect = PrepareRect(PreviewXPerc, PreviewRankYPerc, PreviewRankWPerc, PreviewRankHPerc);
				GUI.DrawTexture(nextRect, RankTextures[rankIndex]);
			}
		}

		// Level Select Buttons
		int maxIndex = Mathf.Min(MaxLevelIndex, (CurrentPage + 1) * ButtonsPerPage);
		int savedBtnIndex = 0;
		for (int i = CurrentPage * ButtonsPerPage; i < maxIndex; ++i) {
			string displayName = GetDisplayName(gControl.AllLevelSelectData[i].LevelName);
			displayName = (i + 1) + ". " + displayName;

			float btnXPerc;
			bool IsInCurrentColumn = false;
			if ((i - (CurrentPage * ButtonsPerPage)) % 2 == 0) {
				btnXPerc = LeftColumnXPerc;
				if (CurrentColumn == 0)
					IsInCurrentColumn = true;
			} else {
				btnXPerc = RightColumnXPerc;
				if (CurrentColumn == 1)
					IsInCurrentColumn = true;
			}

			int rowIndex = ( i - (CurrentPage * ButtonsPerPage) ) / 2;
			rowIndex = Mathf.Clamp(rowIndex, 0, LevelRowYPerc.Length - 1);

			Rect btnRect = PrepareRect(btnXPerc, LevelRowYPerc[rowIndex], LevelBtnWPerc, LevelBtnHPerc);

			// If this is the currently selected button, then show the highlight rectangle
			if (IsInCurrentColumn && rowIndex == CurrentRow && !SelectingTitleButton)
			{
				Rect HighlightBoxRect = btnRect;
				
				HighlightBoxRect.xMin -= HighlightBorderSize;
				HighlightBoxRect.xMax += HighlightBorderSize;
				HighlightBoxRect.yMin -= HighlightBorderSize;
				HighlightBoxRect.yMax += HighlightBorderSize;
				
				GUI.Box( HighlightBoxRect, "", "SelectedButton" );
			}

			if (GUI.Button(btnRect, displayName)
			    		|| ((IsInCurrentColumn && rowIndex == CurrentRow && !SelectingTitleButton) && PressedCurrentButton)) {
				LawnGameControl.PlayButtonPressSound();
				// Make sure game control knows what level is being played
				gControl.SetLevelIndex(i);

				// Stop menu music
				MenuMusic mus = MenuMusic.GetTheMenuMusic();
				if (mus != null) {
					Destroy(mus.gameObject);
				}

				// Load the selected level!
				Application.LoadLevel (gControl.AllLevelSelectData[i].LevelName);
			}

			// If level is completed, show rank for that level
			if (gControl.GetLevelComplete(i))
			{
				int rankIndex = (int)gControl.GetLevelRank(i);
				if (rankIndex >= 0 && rankIndex < SmallRankTextures.Length) {
					nextRect = PrepareRect(btnXPerc + LevelBtnWPerc * 0.5f, LevelRowYPerc[rowIndex] - LevelBtnHPerc * 0.5f,
					                                                    ButtonRankWPerc, ButtonRankHPerc);
					GUI.DrawTexture(nextRect, SmallRankTextures[rankIndex]);
				}
			}

			if (savedBtnIndex < LevelButtonRects.Length) {
				LevelButtonRects[savedBtnIndex] = btnRect;
			}
			savedBtnIndex++;
		}

		// Page Change Buttons
		if (CurrentPage > 0)
		{
			nextRect = PrepareRect(LeftColumnXPerc, PageChangeYPerc, PageChangeWPerc, PageChangeHPerc);
			if (GUI.Button(nextRect, "", "LeftArrow")) {
				// Go to previous page, if not already on the first page

				LawnGameControl.PlayButtonPressSound();
				CurrentPage--;
				UpdatePageNumber(CurrentPage);
			}
		}

		if (CurrentPage < MaxPage)
		{
			nextRect = PrepareRect(RightColumnXPerc, PageChangeYPerc, PageChangeWPerc, PageChangeHPerc);
			if (GUI.Button(nextRect, "", "RightArrow")) {
				// Go to next page, if not already on the last page

				LawnGameControl.PlayButtonPressSound();
				CurrentPage++;
				UpdatePageNumber(CurrentPage);
			}
		}

		// Title screen button highlight
		if (SelectingTitleButton)
		{
			Rect HighlightBoxRect = PrepareRect(TitleBtnXPerc, PageChangeYPerc, TitleBtnWPerc, PageChangeHPerc);
			
			HighlightBoxRect.xMin -= HighlightBorderSize;
			HighlightBoxRect.xMax += HighlightBorderSize;
			HighlightBoxRect.yMin -= HighlightBorderSize;
			HighlightBoxRect.yMax += HighlightBorderSize;
			
			GUI.Box( HighlightBoxRect, "", "SelectedButton" );
		}

		// To Title Screen
		nextRect = PrepareRect(TitleBtnXPerc, PageChangeYPerc, TitleBtnWPerc, PageChangeHPerc);
		if (GUI.Button(nextRect, StrTitle)
		    		|| (SelectingTitleButton && PressedCurrentButton)) {
			LawnGameControl.PlayButtonPressSound();
			Application.LoadLevel(TitleScreenName);
		}

		PressedCurrentButton = false;
	}

	// Returns a level name modified to remove a prefix and underscores that are part of the filename
	string GetDisplayName(string inLevelName) {
		string rValue = inLevelName;
		if (rValue.StartsWith(LevelStartTag)) {
			rValue = rValue.Remove(0, LevelStartTag.Length);
		}
		rValue = rValue.Replace("_", " ");
		return rValue;
	}

	// Returns a minutes : seconds string
	string GetTimeString(int inSeconds) {
		int mins = inSeconds / 60;
		int secs = inSeconds - (mins * 60);
		string secString = secs.ToString();
		if (secs < 10) {
			secString = "0" + secString;
		}
		return mins.ToString() + " : " + secString;
	}

	void UpdatePageNumber(int inPageNumber) {

		if (TitleText != null) {
			TitleText.text = TitleOriginalText + " (" + (inPageNumber + 1) + " / " + (MaxPage + 1) + ")";
		}

		LawnGameControl gControl = LawnGameControl.GetGameControl();
		if (gControl != null) {
			gControl.LastLevelPage = inPageNumber;
		}
	}

	void UpdateDisplayLevelIndex() {
		int newIndex = (CurrentPage * ButtonsPerPage) + (CurrentRow * NumColumns + CurrentColumn);
		if (newIndex >= 0 && newIndex < MaxLevelIndex)
			DisplayLevelIndex = newIndex;
	}

	bool IsRowColumnValid(int inRow, int inColumn)
	{
		int lvlIndex = (CurrentPage * ButtonsPerPage) + (inRow * NumColumns + inColumn);
		if (lvlIndex >= 0 && lvlIndex < MaxLevelIndex)
			return true;
		return false;
	}
}
