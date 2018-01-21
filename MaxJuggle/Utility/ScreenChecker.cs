// Programmer: Carl Childers
// Date: 9/19/2017
//
// Checks if the resolution has changed, and updates the Aspect Utility when it does.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenChecker : MonoBehaviour {

	public float ReferenceAspectRatio = 1;

	int previousWidth;
	int previousHeight;


	void Awake()
	{
		previousWidth = Screen.width;
		previousHeight = Screen.height;
	}

	void Start()
	{
		ResolutionChanged();
	}

	void Update()
	{
		if (Screen.width != previousWidth || Screen.height != previousHeight)
		{
			ResolutionChanged();
			previousWidth = Screen.width;
			previousHeight = Screen.height;
		}
	}

	void ResolutionChanged()
	{
		AspectUtility.SetCamera();
		AdjustCanvasScaler();
	}

	public void EnteredNewScene()
	{
		AdjustCanvasScaler();
	}

	void AdjustCanvasScaler()
	{
		CanvasScaler cvsScaler = GameObject.FindObjectOfType<CanvasScaler>();
		if (cvsScaler != null)
		{
			// Match either width or height depending on the new aspect ratio
			float aspectRatio = (float)Screen.width / Screen.height;
			if (aspectRatio > ReferenceAspectRatio)
			{
				cvsScaler.matchWidthOrHeight = 1;
			}
			else
			{
				cvsScaler.matchWidthOrHeight = 0;
			}
		}
	}
}
