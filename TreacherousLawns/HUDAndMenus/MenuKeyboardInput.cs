// Programmer: Carl Childers
// Date: 7/22/2016
//
// Sends messages to other scripts based on keyboard or gamepad input

using UnityEngine;
using System.Collections;

public class MenuKeyboardInput : MonoBehaviour {

	bool PressedUp, PressedDown, PressedLeft, PressedRight;
	

	// Update is called once per frame
	void Update () {

		float vertAxis = Input.GetAxis("Vertical");
		if (vertAxis > 0 || Input.GetButton("MenuUp"))
		{
			if (!PressedUp)
			{
				PressedUp = true;

				SendMessage("MenuUp", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			PressedUp = false;
		}

		if (vertAxis < 0 || Input.GetButton("MenuDown"))
		{
			if (!PressedDown)
			{
				PressedDown = true;

				SendMessage("MenuDown", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			PressedDown = false;
		}

		float horAxis = Input.GetAxis("Horizontal");
		if (horAxis > 0 || Input.GetButton("MenuRight"))
		{
			if (!PressedRight)
			{
				PressedRight = true;

				SendMessage("MenuRight", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			PressedRight = false;
		}

		if (horAxis < 0 || Input.GetButton("MenuLeft"))
		{
			if (!PressedLeft)
			{
				PressedLeft = true;

				SendMessage("MenuLeft", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			PressedLeft = false;
		}
	}
}
