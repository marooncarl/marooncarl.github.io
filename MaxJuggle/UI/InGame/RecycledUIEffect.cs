// Programmer: Carl Childers
// Date: 11/8/2017
//
// UI effect that is reused when needed again.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecycledUIEffect : MonoBehaviour {

	public string StartTrigger = "Play";

	RectTransform myRectTransform;
	Animator myAnimator;
	bool attachedToCanvas = false;

	Vector2 referenceResolution = Vector2.zero;


	public virtual void Restart(Vector3 worldPos)
	{
		gameObject.SetActive(true);
		if (!attachedToCanvas)
		{
			// Attach to canvas
			Canvas theCanvas = GameObject.FindObjectOfType<Canvas>();
			if (theCanvas != null)
			{
				transform.SetParent(theCanvas.transform);
				attachedToCanvas = true;
				referenceResolution = theCanvas.GetComponent<CanvasScaler>().referenceResolution;

			}
		}

		// Find components
		if (myRectTransform == null)
		{
			myRectTransform = GetComponent<RectTransform>();
		}
		if (myAnimator == null)
		{
			myAnimator = GetComponent<Animator>();
		}

		// Set position
		Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
		screenPoint -= new Vector3(Screen.width / 2, Screen.height / 2);
		if (referenceResolution != Vector2.zero)
		{
			screenPoint.x *= (referenceResolution.x / AspectUtility.screenWidth);
			screenPoint.y *= (referenceResolution.y / AspectUtility.screenHeight);
		}

		myRectTransform.anchoredPosition = new Vector2(screenPoint.x, screenPoint.y);
		myRectTransform.localScale = Vector3.one;

		// Restart animation
		myAnimator.ResetTrigger(StartTrigger);
		myAnimator.SetTrigger(StartTrigger);
	}

	// Called at the end of the animation
	public void Deactivate()
	{
		gameObject.SetActive(false);
	}
}
