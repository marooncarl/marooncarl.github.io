/// <summary>
/// Lawn game control
/// 
/// Programmer: Carl Childers
/// Date: 8/3/2015
/// 
/// Keeps track of high scores between levels, and saves and loads them.
/// </summary>


using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LawnGameControl : MonoBehaviour {

	static LawnGameControl TheGameControl;

	public LevelSelectData[] AllLevelSelectData; // set in the editor; every level that can be played and ranked should be here

	public Transform ButtonPressTemplate; // to have the sound play during level transitions,
										// an object w/ audio source is created and set not to destroy on load.
										// This object should destroy itself after a time.

	[System.NonSerialized]
	public int LastLevelPage; // last page the level select was on


	int NumUnlockedLevels;
	LevelScores[] AllLevelScores;
	int PlayedLevelIndex; // index of the level currently being played.  Set by level select so game control knows where to write scores.


	void Awake() {
		// Make sure there is only one
		if (TheGameControl == null) {
			TheGameControl = this;
		} else {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(this.gameObject);

		if (!LoadLevelData()) {
			InitLevelScores();
		}

		PlayedLevelIndex = 0;
	}

	public static LawnGameControl GetGameControl() {
		return TheGameControl;
	}

	// Plays a button press sound
	// Done by the game control because it persists between levels, so the sound isn't cut off when changing scenes
	public static void PlayButtonPressSound() {
		LawnGameControl theGameControl = GetGameControl();
		if (theGameControl != null) {
			if (theGameControl.ButtonPressTemplate != null && Camera.main != null) {
				Transform newSoundObj = (Transform)Instantiate(theGameControl.ButtonPressTemplate, Camera.main.transform.position, Quaternion.identity);
				DontDestroyOnLoad(newSoundObj.gameObject);
			}
		}
	}

	public void DeleteSaveData() {
		InitLevelScores();
		SaveLevelData();
	}

	// This creates or resets all level score data, leaving a blank slate
	void InitLevelScores() {
		AllLevelScores = new LevelScores[AllLevelSelectData.Length];
		for (int i = 0; i < AllLevelScores.Length; ++i) {
			AllLevelScores[i] = new LevelScores();
			AllLevelScores[i].LevelName = AllLevelSelectData[i].LevelName;
		}

		// For now, just unlock all levels
		NumUnlockedLevels = AllLevelSelectData.Length;
	}

	// Records rank, high score, and best time for the current level, specified by PlayedLevelIndex
	// Also marks the level as complete
	public void RecordLevelScores(LawnScoreKeeper.EScoreRank inRank, int inScore, int inTimeSeconds) {

		if (PlayedLevelIndex < 0 || PlayedLevelIndex >= AllLevelScores.Length)
			return;

		LevelScores scr = AllLevelScores[PlayedLevelIndex];

		/*
		// Search for the first level in level scores with the name of the current level
		string currentLevelName = AllLevelSelectData[PlayedLevelIndex].LevelName;
		LevelScores scr = null;
		for (int i = 0; i < AllLevelScores.Length; ++i)
		{
			if (AllLevelScores[i].LevelName == currentLevelName)
			{
				scr = AllLevelScores[i];
				break;
			}
		}
		// If the level name wasn't found, create a new LevelScores object
		if (scr == null)
		{
			scr = new LevelScores();
			AllLevelScores.Add(scr); // error: can't add to a static array
		}
		*/

		if (scr != null)
		{
			scr.IsComplete = true;
			scr.HighestRank = (LawnScoreKeeper.EScoreRank)Mathf.Max((int)inRank, (int)scr.HighestRank);
			scr.HighScore = Mathf.Max(inScore, scr.HighScore);
			scr.BestClearTimeSeconds = Mathf.Min(inTimeSeconds, scr.BestClearTimeSeconds);

			SaveLevelData();
		}
	}

	public void SetLevelIndex(int inIndex) {
		PlayedLevelIndex = Mathf.Clamp(inIndex, 0, AllLevelScores.Length - 1);
	}

	public int GetHighScore(int inLevelIndex) {
		if (inLevelIndex < 0 || inLevelIndex >= AllLevelScores.Length) {
			return -1;
		}

		return AllLevelScores[inLevelIndex].HighScore;
	}

	public int GetBestTime(int inLevelIndex) {
		if (inLevelIndex < 0 || inLevelIndex >= AllLevelScores.Length) {
			return -1;
		}

		return AllLevelScores[inLevelIndex].BestClearTimeSeconds;
	}

	public bool GetLevelComplete(int inLevelIndex) {
		if (inLevelIndex < 0 || inLevelIndex >= AllLevelScores.Length) {
			return false;
		}

		return AllLevelScores[inLevelIndex].IsComplete;
	}

	public LawnScoreKeeper.EScoreRank GetLevelRank(int inLevelIndex) {
		if (inLevelIndex < 0 || inLevelIndex >= AllLevelScores.Length) {
			return LawnScoreKeeper.EScoreRank.SR_D;
		}

		return AllLevelScores[inLevelIndex].HighestRank;
	}

	public int GetLevelIndex() {
		return PlayedLevelIndex;
	}

	// Saves level data to a file, including scores and completion status for each level, and how many levels are unlocked.
	public void SaveLevelData() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/levelData.dat");

		LevelSaveData saveData = new LevelSaveData(NumUnlockedLevels, AllLevelScores);
		bf.Serialize(file, saveData);

		file.Close();
	}

	// Loads level data from a file
	// Returns whether load was successful.  If not, then new save data should be created from scratch.
	public bool LoadLevelData() {
		if (File.Exists(Application.persistentDataPath + "/levelData.dat")) {

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/levelData.dat", FileMode.Open);
			LevelSaveData saveData = (LevelSaveData)bf.Deserialize(file);
			file.Close();

			NumUnlockedLevels = saveData.NumUnlockedLevels;
			LevelScores[] LoadedLevelScores = saveData.AllLevelScores;

			// Check that the data isn't obviously corrupt
			if (LoadedLevelScores.Length == 0) {
				return false;
			}

			// Make sure the level names match up between level select data and loaded level data
			bool bLoadIsGood = true;
			if (LoadedLevelScores.Length != AllLevelSelectData.Length)
				bLoadIsGood = false;

			if (bLoadIsGood)
			{
				for (int i = 0; i < AllLevelSelectData.Length; ++i)
				{
					if (AllLevelSelectData[i].LevelName != LoadedLevelScores[i].LevelName)
					{
						bLoadIsGood = false;
						break;
					}
				}
			}

			if (bLoadIsGood)
				AllLevelScores = LoadedLevelScores;
			else
			{
				// If there is a mismatch between level select data and level scores,
				// then re-initialize level scores and fill in data from the loaded level scores.
				AllLevelScores = new LevelScores[AllLevelSelectData.Length];
				for (int i = 0; i < AllLevelScores.Length; ++i) {
					AllLevelScores[i] = new LevelScores();
					AllLevelScores[i].LevelName = AllLevelSelectData[i].LevelName;

					string lvlName = AllLevelSelectData[i].LevelName;
					LevelScores scr = null;
					for (int j = 0; j < LoadedLevelScores.Length; ++j)
					{
						if (LoadedLevelScores[j].LevelName == lvlName)
						{
							scr = LoadedLevelScores[j];
							break;
						}
					}
					if (scr != null)
					{
						// Found a level in loaded data with the same name, so fill in info
						// from the loaded level data
						AllLevelScores[i].IsComplete = scr.IsComplete;
						AllLevelScores[i].HighestRank = scr.HighestRank;
						AllLevelScores[i].HighScore = scr.HighScore;
						AllLevelScores[i].BestClearTimeSeconds = scr.BestClearTimeSeconds;
					}
				}
			}

			// Prevent negative high scores or best clear times
			for (int i = 0; i < AllLevelScores.Length; ++i) {
				if (AllLevelScores[i].HighScore < 0) {
					AllLevelScores[i].HighScore = 0;
				}
				if (AllLevelScores[i].BestClearTimeSeconds < 0) {
					AllLevelScores[i].BestClearTimeSeconds = 0;
				}
			}

			return true;
		}

		return false;
	}
}

[System.Serializable]
public class LevelSelectData {
	public string LevelName;
	public Texture2D PreviewTexture;
}

[System.Serializable]
public class LevelScores {

	public string LevelName;
	public bool IsComplete;
	public LawnScoreKeeper.EScoreRank HighestRank;
	public int HighScore;
	public int BestClearTimeSeconds;


	public LevelScores() {
		LevelName = "";
		IsComplete = false;
		HighestRank = LawnScoreKeeper.EScoreRank.SR_D;
		HighScore = 0;
		BestClearTimeSeconds = 300;
	}
}

[System.Serializable]
class LevelSaveData {

	public int NumUnlockedLevels;
	public LevelScores[] AllLevelScores;

	public LevelSaveData(int inUnlockedLevels, LevelScores[] inScoreData) {
		NumUnlockedLevels = inUnlockedLevels;
		AllLevelScores = inScoreData;
	}
}