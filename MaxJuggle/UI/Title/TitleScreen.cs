// Programmer: Carl Childers
// Date: 11/28/2017
//
// Shows the regular play button or resume from bookmark button depending on whether there's a usable bookmark.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {

	public Animator NormalMenu, BookmarkMenu;
	public string ExitTrigger = "Exit";
	public float ExitDuration = 1.0f;

	Animator myAnimator;
	Animator chosenMenu;


	void Start()
	{
		myAnimator = GetComponent<Animator>();

		GameControl ctrl = GameControl.GetGameControl();
		if (ctrl.TheBookmark != null && ctrl.TheBookmark.GameState.Usable)
		{
			CreatePlayMenu(BookmarkMenu);
		}
		else
		{
			CreatePlayMenu(NormalMenu);
		}
	}

	public void Exit()
	{
		chosenMenu.SetTrigger(ExitTrigger);

		Button[] childButtons = chosenMenu.GetComponentsInChildren<Button>();
		foreach (Button b in childButtons)
		{
			b.interactable = false;
		}
	}

	public void SwitchToNormalMenu()
	{
		Destroy(chosenMenu.gameObject);
		CreatePlayMenu(NormalMenu);
	}

	void CreatePlayMenu(Animator menuPrefab)
	{
		chosenMenu = Instantiate(menuPrefab, menuPrefab.transform.position, menuPrefab.transform.rotation);
		chosenMenu.transform.SetParent(transform, false);

		LoadSceneButton[] loadButtons = chosenMenu.GetComponentsInChildren<LoadSceneButton>();
		foreach (LoadSceneButton b in loadButtons)
		{
			b.AnimatedObject = myAnimator;
		}
	}
}
