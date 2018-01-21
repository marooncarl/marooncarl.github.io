// Programmer: Carl Childers
// Date: 12/7/2017
//
// Designed to be placed on the content component of a scroll view

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class JuggleBanList : MonoBehaviour {

	public Sprite ButtonSprite;
	public Sprite OnSprite;
	public Sprite OffSprite;
	public Vector2 ButtonSize = new Vector2(48, 48);
	public Vector2 ToggleSize = new Vector2(24, 24);
	public float ButtonSpacing = 4;
	public int ButtonsPerRow = 5;
	public float OffDarknessMultiplier = 0.75f;
	public float ButtonPressScale = 0.8f;
	public float ButtonEaseFactor = 10;

	// Used to control the size of the item pics in the buttons
	public float ItemStandardSize = 80;
	public float ItemSizePercent = 0.95f;
	public float LargeItemScalePercent = 0.5f;

	public AudioClip ErrorSound;
	[Range(0, 1)]
	public float ErrorSoundVolume = 1f;

	public AudioClip ToggleSound;
	[Range(0, 1)]
	public float ToggleSoundVolume = 1f;

	int numTypesOn;
	public int NumTypesOn {
		get { return numTypesOn; }
		set { numTypesOn = value; }
	}

	LinkedList<BanListButton> itemButtons;
	public LinkedList<BanListButton> ItemButtons {
		get { return itemButtons; }
	}

	const string BanListFilename = "banList.txt";

	AudioSource myAudioSource;
	public AudioSource MyAudioSource {
		get { return myAudioSource; }
	}


	void Awake()
	{
		myAudioSource = GetComponent<AudioSource>();
	}

	void Start()
	{
		Dictionary<JuggleObjectType, bool> loadedTypesAllowed = Load();

		LinkedList<JuggleObjectType> allTossableTypes = GameControl.GetGameControl().GetTossableTypeList();
		itemButtons = new LinkedList<BanListButton>();

		int currentColumn = 0;
		int currentRow = 0;

		RectTransform myTransform = GetComponent<RectTransform>();
		RectTransform contentContainer = myTransform.parent.parent.GetComponent<RectTransform>();
		int numRows = Mathf.CeilToInt(allTossableTypes.Count / (float)ButtonsPerRow);
		float remainingSpace = contentContainer.rect.width - (ButtonSize.x * ButtonsPerRow + ButtonSpacing * (ButtonsPerRow - 1));
		myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ButtonSize.y * numRows + ButtonSpacing * (numRows - 1) + remainingSpace / 2f);

		Vector2 buttonBasePos = new Vector2(-contentContainer.rect.width / 2f + ButtonSize.x / 2f + ButtonSpacing / 2f + remainingSpace / 2f,
			myTransform.rect.height / 2f - ButtonSize.y / 2f - ButtonSpacing / 2f - remainingSpace / 4f);

		numTypesOn = 0;

		foreach (JuggleObjectType itemType in allTossableTypes)
		{
			// Create toggle button for item
			GameObject buttonGameObject = new GameObject("Ban List Button", typeof(RectTransform));

			BanListButton newButton = buttonGameObject.AddComponent<BanListButton>();
			Vector2 btnPos = new Vector2(buttonBasePos.x + (ButtonSize.x + ButtonSpacing) * currentColumn,
								buttonBasePos.y - (ButtonSize.y + ButtonSpacing) * currentRow);

			bool typeOn = true;
			if (loadedTypesAllowed.ContainsKey(itemType))
			{
				typeOn = loadedTypesAllowed[itemType];
			}

			if (typeOn)
			{
				numTypesOn++;
			}

			newButton.Initialize(this, ButtonSize, btnPos, itemType, typeOn);

			itemButtons.AddLast(newButton);

			currentColumn++;
			if (currentColumn >= ButtonsPerRow)
			{
				currentColumn = 0;
				currentRow++;
			}
		}
	}

	public void Save()
	{
		// Gather settings from each button
		Dictionary<JuggleObjectType, bool> itemSettings = new Dictionary<JuggleObjectType, bool>(itemButtons.Count);
		foreach (BanListButton btn in itemButtons)
		{
			itemSettings.Add(btn.MyItemType, btn.IsOn);
		}

		// Create a bitmask from the types' current settings, and save it to a file.

		LinkedList<int> typesBitmask = new LinkedList<int>();
		LinkedList<JuggleObjectType> tossableTypes = GameControl.GetGameControl().GetTossableTypeList();
		int index = 0;
		int part = 0;

		foreach (JuggleObjectType itemType in tossableTypes)
		{
			bool typeIsOn = itemSettings[itemType];
			part = part | ((typeIsOn ? 1 : 0) << index);

			index++;
			if (index > 31)
			{
				typesBitmask.AddLast(part);
				// Reset next bitmask part
				part = 0;
				index = 0;
			}
		}
		if (index != 0)
		{
			typesBitmask.AddLast(part);
		}

		int[] savedTypesAllowed = new int[typesBitmask.Count];
		typesBitmask.CopyTo(savedTypesAllowed, 0);

		BanListData banListSaveData = new BanListData(savedTypesAllowed);

		BinaryFormatter bf = new BinaryFormatter();
		FileStream fStream = File.Create(Application.persistentDataPath + "/" + BanListFilename);
		bf.Serialize(fStream, banListSaveData);
		fStream.Close();

		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			Bookmark.SyncFiles();
		}

		//print("Saved ban list");
	}

	public static Dictionary<JuggleObjectType, bool> Load()
	{
		if (File.Exists(Application.persistentDataPath + "/" + BanListFilename))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fStream = File.Open(Application.persistentDataPath + "/" + BanListFilename, FileMode.Open);
			BanListData loadData = (BanListData)bf.Deserialize(fStream);
			fStream.Close();
			return GetTypesAllowedFromBitmask(loadData.TypesAllowed);
		}
		else
		{
			return new Dictionary<JuggleObjectType, bool>();
		}
	}

	static Dictionary<JuggleObjectType, bool> GetTypesAllowedFromBitmask(int[] inTypesAllowed)
	{
		if (inTypesAllowed.Length == 0)
			return new Dictionary<JuggleObjectType, bool>();

		Dictionary<JuggleObjectType, bool> typesAllowed = new Dictionary<JuggleObjectType, bool>();
		LinkedList<JuggleObjectType> tossableTypes = GameControl.GetGameControl().GetTossableTypeList();
		int typeIndex = 0;
		int partIndex = 0;
		int part = inTypesAllowed[partIndex];

		foreach (JuggleObjectType itemType in tossableTypes)
		{
			bool isItemOn = ( ((part & (1 << typeIndex)) != 0) ? true : false);
			typesAllowed.Add(itemType,  isItemOn);

			typeIndex++;
			if (typeIndex > 31)
			{
				typeIndex = 0;
				partIndex++;
				if (inTypesAllowed.Length > partIndex)
				{
					part = inTypesAllowed[partIndex];
				}
				else
				{
					break;
				}
			}
		}

		//print("Loaded ban list with " + typesAllowed.Count + " items");
		return typesAllowed;
	}
}

[System.Serializable]
public class BanListData {

	public int[] TypesAllowed;		// Bitmask telling which tossable types are turned on.  Each int represents up to 32 types.

	public BanListData(int inTypes)
	{
		TypesAllowed = new int[1];
		TypesAllowed[0] = inTypes;
	}

	public BanListData(int[] inTypes)
	{
		TypesAllowed = inTypes;
	}
}