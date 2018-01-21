// Programmer: Carl Childers
// Date: 10/9/2017
//
// Starts animating a decorative item after a delay, and allows it to select a random sprite each time it starts its animation.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleDecoItem : MonoBehaviour {

	public float StartDelay = 0;
	public string StartTrigger = "Start";
	public JuggleItemList ItemTypes;

	float startCounter;
	List<int> availableIndices;


	void Awake()
	{
		startCounter = StartDelay;
		if (ItemTypes != null)
		{
			availableIndices = new List<int>(ItemTypes.Length);
			RestartAvailableIndices();
		}
	}

	void Update()
	{
		startCounter -= Time.deltaTime;
		if (startCounter <= 0)
		{
			Animator myAnim = GetComponent<Animator>();
			if (myAnim != null)
			{
				myAnim.SetTrigger(StartTrigger);
			}
			enabled = false;
		}
	}

	// Allows an animation to choose a random sprite
	public void ChooseRandomSprite()
	{
		if (ItemTypes == null)
			return;

		RectTransform rTransform = GetComponent<RectTransform>();
		Image img = GetComponent<Image>();
		if (rTransform != null && img != null)
		{
			int indexChoice = Random.Range(0, availableIndices.Count);
			int pickedIndex = availableIndices[indexChoice];
			availableIndices.RemoveAt(indexChoice);
			if (availableIndices.Count == 0)
			{
				RestartAvailableIndices();
			}
			Sprite pickedSprite = ItemTypes[pickedIndex].AvatarSprite;
			if (pickedSprite != null)
			{
				img.sprite = pickedSprite;
				rTransform.sizeDelta = pickedSprite.bounds.size * 100f;
			}
		}
	}

	void RestartAvailableIndices()
	{
		for (int i = 0; i < ItemTypes.Length; ++i)
		{
			for (int j = 0; j < ItemTypes[i].TitleScreenFrequency; ++j)
			{
				availableIndices.Add(i);
			}
		}
	}
}
