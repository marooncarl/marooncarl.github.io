// Programmer: Carl Childers
// Date: 9/25/2017
//
// Handles pausing, and creates an indicator showing exactly when the game will resume after pausing.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pauser : MonoBehaviour {

	public string PauseButton = "Pause";
	public string UnpauseButton = "Fire1";
	public string FadeInTrigger = "FadeIn";
	public string SkipIntroTrigger = "Skip";
	public string FadeOutTrigger = "FadeOut";
	public string UnpauseWaitTrigger = "ClockStart";
	public string SaveButtonName = "Save";
	public string TitleSceneName = "TitleScene";

	public float UnpauseDelay = 1f;

	public Vector2 ItemListTopCenter;
	public float ItemHorizontalSpacing = 100;
	public float ItemVerticalSpacing = 100;
	public int ItemRowSize = 2;

	public RectTransform PauseScreen;
	public RectTransform UnpauseIndicator;

	public Font ItemListFont;
	public Color ItemListTextColor = Color.white;
	public int ItemListFontSize = 24;
	public float ItemListTextSpace = 100;

	RectTransform myPauseScreen;
	RectTransform myUnpauseIndicator;

	bool isPaused;
	public bool IsPaused {
		get { return isPaused; }
	}

	bool pausingEnabled = true;
	public bool PausingEnabled {
		get { return pausingEnabled; }
		set { pausingEnabled = value; }
	}

	float unpauseWaitCounter;

	bool exitingGame;

	static Pauser thePauser;

	GameTosser theTosser;
	Dictionary<JuggleObjectType, int> itemsInPlay;
	Dictionary<JuggleObjectType, RectTransform> itemDisplays;


	[ExecuteInEditMode]
	void OnValidate()
	{
		ItemRowSize = Mathf.Max(1, ItemRowSize);
	}

	public static Pauser GetPauser()
	{
		if (thePauser != null)
		{
			return thePauser;
		}
		else
		{
			return GameObject.FindObjectOfType<Pauser>();
		}
	}

	void Awake()
	{
		thePauser = this;
	}

	void Update()
	{
		if (!isPaused)
		{
			if (Input.GetButtonDown(PauseButton) && CanPause())
			{
				BeginPause();
			}
			else
			{
				// Update unpause delay
				if (unpauseWaitCounter > 0)
				{
					unpauseWaitCounter -= Time.unscaledDeltaTime;
					if (unpauseWaitCounter <= 0)
					{
						// Actually resume the game
						Time.timeScale = 1;
						if (theTosser != null)
						{
							theTosser.HideItemTrails();
						}
					}
				}
			}
		}
		else
		{
			if (Input.GetButtonDown(SaveButtonName))
			{
				SaveAndQuit();
			}
			if (Input.GetButtonDown(UnpauseButton))
			{
				EndPause();
			}
		}
	}

	void OnApplicationQuit()
	{
		exitingGame = true;
	}

	void OnApplicationFocus(bool hasFocus)
	{
		if (exitingGame || !CanPause())
			return;

		if (!hasFocus)
		{
			BeginPause(true);
		}
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if (exitingGame || !CanPause())
			return;

		if (pauseStatus)
		{
			BeginPause(true);
		}
	}

	public void BeginPause(bool skipIntro = false)
	{
		if (!isPaused)
		{
			isPaused = true;
			Time.timeScale = 0;
			if (myPauseScreen == null)
			{
				myPauseScreen = Instantiate(PauseScreen);
				Canvas theCanvas = GameObject.FindObjectOfType<Canvas>();
				if (theCanvas != null)
				{
					myPauseScreen.SetParent(theCanvas.transform);
					myPauseScreen.anchoredPosition = Vector2.zero;
					myPauseScreen.localScale = Vector3.one;
				}
			}
			else
			{
				myPauseScreen.gameObject.SetActive(true);
			}

			// In case the unpause warning was still playing, cancel it
			if (myUnpauseIndicator != null && myUnpauseIndicator.gameObject.activeSelf)
			{
				myUnpauseIndicator.gameObject.SetActive(false);
			}

			Animator pauseAnim = myPauseScreen.GetComponent<Animator>();
			if (!skipIntro)
			{
				pauseAnim.SetTrigger(FadeInTrigger);
			}
			else
			{
				pauseAnim.SetTrigger(SkipIntroTrigger);
			}

			InitItemList();
		}
	}

	void InitItemList()
	{
		// Find tosser and create dictionaries, if necessary
		if (theTosser == null)
		{
			theTosser = GameObject.FindObjectOfType<GameTosser>();
		}
		if (theTosser == null)
			return;

		if (itemsInPlay == null)
		{
			itemsInPlay = new Dictionary<JuggleObjectType, int>();
		}
		else
		{
			itemsInPlay.Clear();
		}

		if (itemDisplays == null)
		{
			itemDisplays = new Dictionary<JuggleObjectType, RectTransform>();
		}

		// Find how many of which types are currently in play
		foreach (JuggleObject item in theTosser.ObjectsInPlay)
		{
			if (!itemsInPlay.ContainsKey(item.MyType))
			{
				itemsInPlay.Add(item.MyType, 1);
			}
			else
			{
				itemsInPlay[item.MyType]++;
			}
		}

		// Create a display for each type in play, as needed
		foreach (JuggleObjectType itemType in itemsInPlay.Keys)
		{
			//print(itemsInPlay[itemType] + " " + itemType.SingularName + " in play");
			if (!itemDisplays.ContainsKey(itemType))
			{
				CreateItemDisplay(itemType);
			}
		}

		// Remove displays that are no longer needed
		Dictionary<JuggleObjectType, RectTransform> itemDisplaysCopy = new Dictionary<JuggleObjectType, RectTransform>(itemDisplays);
		foreach (JuggleObjectType itemType in itemDisplaysCopy.Keys)
		{
			if (!itemsInPlay.ContainsKey(itemType))
			{
				GameObject.Destroy(itemDisplays[itemType].gameObject);
				itemDisplays.Remove(itemType);
			}
		}

		// Position the displays, and update the item count for each display
		int row = 0, col = 0;
		int currentRowSize = Mathf.Min(ItemRowSize, itemDisplays.Keys.Count);
		int numItemsPositioned = 0;

		foreach (JuggleObjectType itemType in itemDisplays.Keys)
		{
			PositionItemDisplay(itemDisplays[itemType], row, col, currentRowSize);
			UpdateItemCount(itemDisplays[itemType], itemsInPlay[itemType]);
			numItemsPositioned++;

			col++;
			if (col >= currentRowSize)
			{
				row++;
				col = 0;

				currentRowSize = Mathf.Min(ItemRowSize + (row % 2), itemDisplays.Keys.Count - numItemsPositioned);
			}
		}
	}

	void CreateItemDisplay(JuggleObjectType itemType)
	{
		GameObject newDisplayObj = new GameObject(itemType.SingularName + " display", typeof(RectTransform));
		RectTransform newDisplay = newDisplayObj.GetComponent<RectTransform>();
		newDisplay.SetParent(myPauseScreen);
		newDisplay.localScale = Vector3.one;
		itemDisplays.Add(itemType, newDisplay);

		// Create the image icon as a child object, so the icon's rotation doesn't interfere with the text.
		GameObject itemImageObj = new GameObject("Item Icon", typeof(RectTransform));
		RectTransform itemImageTransform = itemImageObj.GetComponent<RectTransform>();
		itemImageTransform.SetParent(newDisplay);
		itemImageTransform.localScale = Vector3.one;

		Image displayImage = itemImageObj.AddComponent<Image>();
		displayImage.sprite = itemType.OffScreenSprite;
		if (itemType.UseItemColor)
		{
			displayImage.color = itemType.ItemColor;
		}

		itemImageTransform.localRotation = Quaternion.Euler(0, 0, itemType.PauseScreenRotation);

		// Determine display size of the icon

		Vector2 iconSize = new Vector2(itemType.OffScreenSprite.bounds.size.x, itemType.OffScreenSprite.bounds.size.y)
			* itemType.OffScreenSprite.pixelsPerUnit;
		float greaterSize = Mathf.Max(iconSize.x, iconSize.y);
		if (greaterSize > 0)
		{
			float sizeRatio = itemType.PauseScreenSize / Mathf.Max(iconSize.x, iconSize.y);
			itemImageTransform.sizeDelta = iconSize * sizeRatio;
		}
	}

	void PositionItemDisplay(RectTransform inDisplay, int row, int col, int rowSize)
	{
		Vector2 itemPos = ItemListTopCenter + Vector2.down * row * ItemVerticalSpacing;
		itemPos.x += (-(rowSize - 1) / 2f + col) * ItemHorizontalSpacing;
		itemPos.x -= ItemListTextSpace / 2f;
		inDisplay.anchoredPosition = itemPos;

		inDisplay.gameObject.SetActive(true);
	}

	// Creates a text child for an item display, or retreives it if already there, and updates the item count
	void UpdateItemCount(RectTransform inDisplay, int itemCount)
	{
		Text itemCountText = inDisplay.GetComponentInChildren<Text>();
		if (itemCountText == null)
		{
			GameObject textObj = new GameObject("Item Count", typeof(RectTransform));
			itemCountText = textObj.AddComponent<Text>();
			itemCountText.font = ItemListFont;
			itemCountText.fontSize = ItemListFontSize;
			itemCountText.color = ItemListTextColor;
			itemCountText.horizontalOverflow = HorizontalWrapMode.Overflow;
			itemCountText.verticalOverflow = VerticalWrapMode.Overflow;
			itemCountText.alignment = TextAnchor.LowerLeft;

			RectTransform textTransform = itemCountText.GetComponent<RectTransform>();
			textTransform.SetParent(inDisplay);
			textTransform.localScale = Vector3.one;
			textTransform.sizeDelta = Vector2.one * ItemListTextSpace;
			textTransform.anchoredPosition = new Vector2(inDisplay.sizeDelta.x / 2 + textTransform.sizeDelta.x / 2, 0);
		}

		itemCountText.text = " x " + itemCount;
	}

	void EndPause()
	{
		if (isPaused)
		{
			isPaused = false;
			unpauseWaitCounter = UnpauseDelay;
			if (myPauseScreen != null)
			{
				Animator pauseAnim = myPauseScreen.GetComponent<Animator>();
				pauseAnim.SetTrigger(FadeOutTrigger);
			}

			// Hide item displays before the pause screen fades out
			if (itemDisplays != null)
			{
				foreach (JuggleObjectType itemType in itemDisplays.Keys)
				{
					itemDisplays[itemType].gameObject.SetActive(false);
				}
			}

			StartUnpauseWarning();
		}
	}

	void StartUnpauseWarning()
	{
		if (myUnpauseIndicator == null)
		{
			myUnpauseIndicator = Instantiate(UnpauseIndicator);
			Canvas theCanvas = GameObject.FindObjectOfType<Canvas>();
			if (theCanvas != null)
			{
				myUnpauseIndicator.SetParent(theCanvas.transform);
				myUnpauseIndicator.anchoredPosition = Vector2.zero;
				myUnpauseIndicator.localScale = Vector3.one;
			}
		}
		else
		{
			myUnpauseIndicator.gameObject.SetActive(true);
		}

		Animator indicatorAnim = myUnpauseIndicator.GetComponent<Animator>();
		indicatorAnim.ResetTrigger(UnpauseWaitTrigger);
		indicatorAnim.SetTrigger(UnpauseWaitTrigger);

		if (theTosser == null)
		{
			theTosser = GameObject.FindObjectOfType<GameTosser>();
		}
		if (theTosser != null)
		{
			theTosser.ShowItemTrails();
		}
	}

	public float GetTimeTillUnpause()
	{
		return unpauseWaitCounter;
	}

	public bool IsPausedOrWaitingToResume()
	{
		return (isPaused || unpauseWaitCounter > 0);
	}

	public bool CanPause()
	{
		if (theTosser == null)
		{
			theTosser = GameObject.FindObjectOfType<GameTosser>();
		}
		if (theTosser != null && theTosser.TheJuggler != null)
		{
			return !theTosser.TheJuggler.InGameOver;
		}
		else
		{
			return false;
		}
	}

	void SaveAndQuit()
	{
		Bookmark.Save();

		Time.timeScale = 1;
		GameControl.GetGameControl().LoadScene(TitleSceneName);
	}
}
