// Programmer: Carl Childers
// Date: 9/16/2017
//
// Meter made of icons that can animate and move to their target position.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimIconMeter : MonoBehaviour {

	public AnimIcon IconPrefab;
	public float IconSeperation = 16;

	public delegate int GetMeterValue();

	List<AnimIcon> icons;
	int activeIconCount;
	public int ActiveIconCount {
		get { return activeIconCount; }
		set { activeIconCount = value; }
	}

	RectTransform myRectTransform;
	Canvas theCanvas;

	float iconMoveCounter;


	void Awake()
	{
		icons = new List<AnimIcon>(10);
		myRectTransform = GetComponent<RectTransform>();
	}

	void Update()
	{
		if (iconMoveCounter > 0)
		{
			iconMoveCounter -= Time.deltaTime;
			if (iconMoveCounter <= 0)
			{
				UpdateIconTargetPositions();
			}
		}
	}

	public void AddIcon(string animName, int amt = 1, float moveDelay = 0)
	{
		for (int i = 0; i < amt; ++i)
		{
			if (icons.Count > activeIconCount)
			{
				icons[activeIconCount].gameObject.SetActive(true);
			}
			else
			{
				while (icons.Count < activeIconCount + 1)
				{
					AnimIcon newIcon = Instantiate(IconPrefab, myRectTransform.anchoredPosition, Quaternion.identity) as AnimIcon;
					icons.Add(newIcon);
					newIcon.transform.SetParent(myRectTransform);
					newIcon.MyRectTransform.localScale = Vector3.one;
				}
			}
			icons[activeIconCount].MyAnimator.SetTrigger(animName);

			activeIconCount++;
		}

		if (moveDelay > 0)
		{
			iconMoveCounter = moveDelay;
		}
		else
		{
			UpdateIconTargetPositions();
			iconMoveCounter = 0;
		}
	}

	public void RemoveIcon(string animName, int amt = 1, float moveDelay = 0, bool immediate = false)
	{
		for (int i = 0; i < amt; ++i)
		{
			if (activeIconCount <= 0)
			{
				break;
			}

			activeIconCount--;

			if (icons.Count > activeIconCount)
			{
				if (!immediate)
				{
					icons[activeIconCount].MyAnimator.SetTrigger(animName);
				}
				else
				{
					icons[activeIconCount].Deactivate();
				}
			}
		}

		if (moveDelay > 0)
		{
			iconMoveCounter = moveDelay;
		}
		else
		{
			UpdateIconTargetPositions();
			iconMoveCounter = 0;
		}
	}

	void UpdateIconTargetPositions()
	{
		for (int i = 0; i < activeIconCount; ++i)
		{
			if (i >= icons.Count)
			{
				break;
			}
				
			icons[i].TargetPosition = Vector2.right * (-activeIconCount / 2f + 0.5f + i) * IconSeperation;
			if (!icons[i].GivenInitialPosition)
			{
				icons[i].MyRectTransform.anchoredPosition = icons[i].TargetPosition;
				icons[i].GivenInitialPosition = true;
			}
		}
	}
}
