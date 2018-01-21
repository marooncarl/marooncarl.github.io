// Programmer: Carl Childers
// Date: 9/16/2017
//
// Icon in an AnimIconMeter.  Can move to target position and play animations.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimIcon : MonoBehaviour {

	public float MoveFactor = 10;

	Vector2 targetPosition;
	public Vector2 TargetPosition {
		get { return targetPosition; }
		set { targetPosition = value; }
	}

	Animator myAnimator;
	public Animator MyAnimator {
		get
		{
			if (myAnimator == null)
			{
				myAnimator = GetComponent<Animator>();
			}
			return myAnimator;
		}
	}

	RectTransform myRectTransform;
	public RectTransform MyRectTransform {
		get { return myRectTransform; }
	}

	bool givenInitialPosition = false;
	public bool GivenInitialPosition {
		get { return givenInitialPosition; }
		set { givenInitialPosition = value; }
	}


	void Awake()
	{
		myRectTransform = GetComponent<RectTransform>();
	}

	void Update()
	{
		// Move to target position
		myRectTransform.anchoredPosition += (targetPosition - myRectTransform.anchoredPosition) * Time.deltaTime * MoveFactor;
	}

	// Called from removal animations
	public void Deactivate()
	{
		gameObject.SetActive(false);
		givenInitialPosition = false;
	}
}
