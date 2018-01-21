// Programmer: Carl Childers
// Date: 12/16/2017
//
// Loads a scene, and sets preferences for skipping the intro stages of the game.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipIntroButton : LoadSceneButton {

	public override void BeginLoadingScene()
	{
		base.BeginLoadingScene();

		PlayerPrefs.SetInt("SkipIntro", 1);
	}
}
