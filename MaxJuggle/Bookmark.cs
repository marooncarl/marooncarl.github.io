// Programmer: Carl Childers
// Date: 11/27/2017
// 
// Saves the game state for the next play session.
// Creating a new bookmark object loads data from file.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

public class Bookmark {

	GameStateData gameState;
	public GameStateData GameState {
		get { return gameState; }
	}

	const string Filename = "bookmark.txt";


	[DllImport("__Internal")]
	public static extern void SyncFiles();

	public Bookmark()
	{
		if (File.Exists(Application.persistentDataPath + "/" + Filename))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fStream = File.Open(Application.persistentDataPath + "/" + Filename, FileMode.Open);
			gameState = (GameStateData)bf.Deserialize(fStream);
			fStream.Close();

			//MonoBehaviour.print("Loaded bookmark");
		}
	}

	public static void Save()
	{
		GameTosser theTosser = GameObject.FindObjectOfType<GameTosser>();
		Juggler theJuggler = GameObject.FindObjectOfType<Juggler>();
		JuggleStatKeeper statKeeper = JuggleStatKeeper.GetStatKeeper();

		if (theTosser != null && theJuggler != null && statKeeper != null)
		{
			ItemStateData[] itemStates = new ItemStateData[theTosser.ObjectsInPlay.Count];
			int itemIndex = 0;
			foreach (JuggleObject item in theTosser.ObjectsInPlay)
			{
				itemStates[itemIndex] = item.GetStateData();
				itemIndex++;
			}

			GameStateData savedGameState = new GameStateData(itemStates, statKeeper.FullyJuggledObjects, (byte)theJuggler.CurrentLives,
				theTosser.CurrentStage, theTosser.AllAvailableTypes, GameControl.GetGameControl().GetPossibleTypeList());
			savedGameState.Usable = true;
			theTosser.SaveToGameState(savedGameState);

			// Save game state data to file
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fStream = File.Create(Application.persistentDataPath + "/" + Filename);
			bf.Serialize(fStream, savedGameState);
			fStream.Close();
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				SyncFiles();
			}

			GameControl.GetGameControl().RefreshBookmark();

			//MonoBehaviour.print("Saved bookmark");
		}
	}

	public static void ClearBookmark()
	{
		GameStateData emptyGameState = new GameStateData();

		// Save cleared game state to file
		BinaryFormatter bf = new BinaryFormatter();
		FileStream fStream = File.Create(Application.persistentDataPath + "/" + Filename);
		bf.Serialize(fStream, emptyGameState);
		fStream.Close();
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			SyncFiles();
		}

		GameControl.GetGameControl().ClearBookmark();
	}

	public static JuggleObjectType GetItemTypeFromLabel(string inLabel)
	{
		Dictionary<string, JuggleObjectType> labelsToTypes = GameControl.GetGameControl().GetLabelToTypeDictionary();
		if (labelsToTypes != null && labelsToTypes.ContainsKey(inLabel))
		{
			return labelsToTypes[inLabel];
		}
		else
		{
			return null;
		}
	}

	/*
	public void DisplaySaveData()
	{
		GUI.color = Color.black;

		GUI.Label(new Rect(0, 0, 1000, 24), "Current Stage: " + gameState.CurrentStage);
		GUI.Label(new Rect(0, 24, 1000, 24), "Lives Left: " + gameState.LivesLeft);

		string displayString = "";
		if (gameState.ItemsInPlay != null)
		{
			foreach (ItemStateData item in gameState.ItemsInPlay)
			{
				if (displayString != "")
				{
					displayString += ", ";
				}

				displayString += GetItemTypeFromLabel(item.TypeLabel).SingularName;
			}
			GUI.Label(new Rect(0, 48, 1000, 24), "Items in play: " + displayString);
		}

		if (gameState.JuggledItems != null)
		{
			displayString = "";
			foreach (string typeLabel in gameState.JuggledItems.Keys)
			{
				if (displayString != "")
				{
					displayString += ", ";
				}

				displayString += gameState.JuggledItems[typeLabel] + " " + GetItemTypeFromLabel(typeLabel).PluralName;
			}
			GUI.Label(new Rect(0, 72, 1000, 24), "Items Juggled: " + displayString);
		}
	}
	*/
}

// Data for the game in progress
[System.Serializable]
public class GameStateData {

	public ItemStateData[] ItemsInPlay;
	public Dictionary<string, int> JuggledItems;		// The key is a juggle object type's bookmark label
	public byte LivesLeft;
	public int CurrentStage;
	public int TypesIntroduced;			// Bitmask telling which types have been introduced in-game.  Stores up to 32 types.
	public bool Usable;

	// Additional game tosser properties
	public float TossCounter;
	public string IntroducedType = "";
	public string PendingIntroducedType = "";
	public string LastIntroducedType = "";
	public byte JugglesToNextStage;
	public byte CountdownToNextIntro;
	public bool UsedMinStagesBetweenLast;
	public int StageToIntroduce;
	public byte CurrentAdvTier;
	public byte ItemCountAgendaIndex;
	public byte BasicIntroIndex;
	public byte LastBasicTier;
	public byte ItemsTossedThisStage;


	// Reference list is a list of all possible in-game types, in order, from which this class can tell which type is in what position in the bitmask.
	public GameStateData(ItemStateData[] inItems, Dictionary<JuggleObjectType, int> inJuggledItems, byte inLivesLeft, int inStage,
		LinkedList<JuggleObjectType> inTypesIntroduced, LinkedList<JuggleObjectType> referenceList)
	{
		ItemsInPlay = inItems;

		if (inJuggledItems != null)
		{
			JuggledItems = new Dictionary<string, int>();
			foreach (JuggleObjectType itemType in inJuggledItems.Keys)
			{
				JuggledItems.Add(itemType.BookmarkLabel, inJuggledItems[itemType]);
			}
		}
		else
		{
			JuggledItems = null;
		}

		LivesLeft = inLivesLeft;
		CurrentStage = inStage;

		Dictionary<JuggleObjectType, int> typeMap = new Dictionary<JuggleObjectType, int>(referenceList.Count);
		int i = 0;
		foreach (JuggleObjectType itemType in referenceList)
		{
			typeMap.Add(itemType, i);
			i++;
		}

		TypesIntroduced = 0;
		foreach (JuggleObjectType itemType in inTypesIntroduced)
		{
			TypesIntroduced = TypesIntroduced | (1 << typeMap[itemType]);
		}
	}

	// Alternate constructor that creates a blank, unusable game state
	public GameStateData()
	{
		JuggledItems = null;
		LivesLeft = 0;
		CurrentStage = 1;
		TypesIntroduced = 0;
		Usable = false;
	}
}

// Data for a juggleable item
[System.Serializable]
public class ItemStateData {

	public byte ID;					// identifies this item, in case another item needs to reference it
	public string TypeLabel;		// A short string that represents a type
	public float XPosition;
	public float YPosition;
	public float XVelocity;
	public float YVelocity;
	public float Rotation;
	public float RotationRate;
	public byte JuggleCount;
	public float ImmuneCounter;
	public Dictionary<string, float> CustomProps;
	public byte[] RelatedItems;		// List of IDs of items related to this one.

	public ItemStateData(byte inID, JuggleObjectType inType, Vector2 inPos, Vector2 inVelocity, float inRotation, float inRotationRate, byte inJuggleCount, float inImmuneCounter)
	{
		ID = inID;
		TypeLabel = inType.BookmarkLabel;
		XPosition = inPos.x;
		YPosition = inPos.y;
		XVelocity = inVelocity.x;
		YVelocity = inVelocity.y;
		Rotation = inRotation;
		RotationRate = inRotationRate;
		JuggleCount = inJuggleCount;
		ImmuneCounter = inImmuneCounter;
	}
}