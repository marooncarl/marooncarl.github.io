// Programmer: Carl Childers
// Date: 11/9/2017
//
// Load scene button that triggers a background transition a short while before the current scene unloads,
// so objects will be destroyed when the screen is covered up.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneButtonTransition : LoadSceneButton {

	public float TransitionPeriod = 0.4f;
	public int BackgroundIndex = 0;

	bool startedBackgroundAnimation;

	protected override void UpdateButton()
	{
		base.UpdateButton();

		if (BeganLoadingScene && !startedBackgroundAnimation && LoadCounter < TransitionPeriod)
		{
			startedBackgroundAnimation = true;
			if (BackgroundIndex >= 0)
			{
				GameControl.GetGameControl().Background.SwitchBackground(BackgroundIndex);
			}
		}
	}
}
