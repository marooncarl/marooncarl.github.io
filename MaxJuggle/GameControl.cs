// Programmer: Carl Childers
// Date: 9/8/2017
//
// Manages loading and unloading of scenes.  Scenes for menus and gameplay are loaded on top of a main scene with a background,
// so if an animated background is used, it won't restart every time a scene is loaded.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour {

	public string InitialSceneName = "";
	public BackgroundSwitcher Background;
	public JuggleItemList PossibleInGameTypes;			// List of all save-able types.  Needed to convert list of types to a bitmask and back.
	public JuggleItemList TossableTypes;			// List of all tossable juggle item types.

	static GameControl theGameControl;

	string currentSceneName = "";

	AsyncOperation loadOp, unloadOp;
	bool isLoading = false, isUnloading = false;

	Bookmark theBookmark;
	public Bookmark TheBookmark {
		get { return theBookmark; }
	}

	Dictionary<string, JuggleObjectType> labelsToTypes;


	public static GameControl GetGameControl()
	{
		if (theGameControl != null)
		{
			return theGameControl;
		}
		theGameControl = GameObject.FindObjectOfType<GameControl>();
		if (theGameControl != null)
		{
			return theGameControl;
		}

		GameObject gameControlObj = new GameObject("GameControl");
		theGameControl = gameControlObj.AddComponent<GameControl>();
		return theGameControl;
	}

	void Awake()
	{
		LoadScene(InitialSceneName);

		// Load save data if it exists
		theBookmark = new Bookmark();
	}

	void Update()
	{
		if (isLoading || isUnloading)
		{
			if (isLoading && loadOp.isDone)
			{
				isLoading = false;
			}
			if (isUnloading && unloadOp.isDone)
			{
				isUnloading = false;
			}
			if (!isLoading && !isUnloading)
			{
				FinishedLoading();
			}
		}
	}

	public void LoadScene(string sceneName)
	{
		if (currentSceneName != "")
		{
			isUnloading = true;
			unloadOp = SceneManager.UnloadSceneAsync(currentSceneName);
		}

		//print("Loading Scene: " + sceneName);
		isLoading = true;
		loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		currentSceneName = sceneName;
	}

	void FinishedLoading()
	{
		ScreenChecker screenChecker = GameObject.FindObjectOfType<ScreenChecker>();
		if (screenChecker != null)
		{
			screenChecker.EnteredNewScene();
		}
		//print("Scenes are finished loading / unloading");
	}

	public bool IsLoadingOrUnloading()
	{
		return (isLoading || isUnloading);
	}

	public LinkedList<JuggleObjectType> GetPossibleTypeList()
	{
		return new LinkedList<JuggleObjectType>(PossibleInGameTypes.Types);
	}

	public LinkedList<JuggleObjectType> GetTossableTypeList()
	{
		return new LinkedList<JuggleObjectType>(TossableTypes.Types);
	}

	public Dictionary<string, JuggleObjectType> GetLabelToTypeDictionary()
	{
		if (labelsToTypes != null)
		{
			return labelsToTypes;
		}
		else
		{
			labelsToTypes = new Dictionary<string, JuggleObjectType>(PossibleInGameTypes.Length);
			foreach (JuggleObjectType itemType in PossibleInGameTypes.Types)
			{
				labelsToTypes.Add(itemType.BookmarkLabel, itemType);
			}
			return labelsToTypes;
		}
	}

	public void ClearBookmark()
	{
		theBookmark = null;
	}

	public void RefreshBookmark()
	{
		theBookmark = new Bookmark();
	}

	/*
	// temp
	void OnGUI()
	{
		if (theBookmark != null && theBookmark.GameState != null && theBookmark.GameState.Usable)
		{
			theBookmark.DisplaySaveData();
		}
	}
	*/
}
