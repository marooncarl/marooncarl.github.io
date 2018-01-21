// Programmer: Carl Childers
// Date: 12/16/2017
//
// Loads a scene, and sets preferences for a normal game (no skipping)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : LoadSceneButton {

	public override void BeginLoadingScene()
	{
		base.BeginLoadingScene();

		PlayerPrefs.SetInt("SkipIntro", 0);
	}
}
