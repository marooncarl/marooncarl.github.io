// Programmer: Carl Childers
// Date: 9/4/2017
//
// A button that changes appearance to reflect an on/off state.  Can be extended to actually toggle something.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ToggleButton : MonoBehaviour {

	public Sprite OnSprite;
	public Sprite OffSprite;
	
	protected bool isOn = true;
	Image buttonImage;
	

	void Awake()
	{
		SetInitialState();
		UpdateImage();
	}

	// Check if the button should start on or off, depending on the starting value of its option
	protected virtual void SetInitialState()
	{
	}
	
	public void Toggle()
	{
		isOn = !isOn;
		PerformAction(isOn);
		UpdateImage();
	}

	protected void UpdateImage()
	{
		if (buttonImage == null)
		{
			buttonImage = GetComponent<Image>();
		}

		if (buttonImage != null)
		{
			if (isOn)
			{
				buttonImage.sprite = OnSprite;
			}
			else
			{
				buttonImage.sprite = OffSprite;
			}
		}
	}

	// Turns a related option on or off
	protected virtual void PerformAction(bool turnedOn)
	{
	}
}
