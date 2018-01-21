// Programmer: Carl Childers
// Date: 9/4/2017
//
// Allows a UI button to toggle fullscreen mode.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleFullscreenButton : ToggleButton, IPointerDownHandler {

	protected override void SetInitialState ()
	{
		isOn = Screen.fullScreen;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Toggle();
	}

	protected override void PerformAction(bool turnedOn)
	{
		Screen.fullScreen = turnedOn;
	}
}
