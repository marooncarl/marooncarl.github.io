// Programmer: Carl Childers
// Date: 12/8/2017
//
// Toggles an item on or off in game when clicked on.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BanListButton : MonoBehaviour {

	JuggleBanList myBanList;

	JuggleObjectType myItemType;
	public JuggleObjectType MyItemType {
		get { return myItemType; }
	}

	Image myToggleIndicator;
	Image itemImage;

	bool isOn;
	public bool IsOn {
		get { return isOn; }
	}

	RectTransform myTransform;
	float currentScale;


	public void Initialize(JuggleBanList inBanList, Vector2 btnSize, Vector2 btnPos, JuggleObjectType itemType, bool startOn = true)
	{
		myBanList = inBanList;
		myItemType = itemType;
		isOn = startOn;

		// Create components for the button
		Button buttonComp = gameObject.AddComponent<Button>();
		buttonComp.onClick.AddListener(ToggleItem);

		// Create button sprite
		Image buttonSprite = gameObject.AddComponent<Image>();
		buttonSprite.sprite = myBanList.ButtonSprite;

		myTransform = gameObject.GetComponent<RectTransform>();
		myTransform.SetParent(myBanList.transform, false);
		myTransform.localScale = Vector3.one;
		myTransform.anchoredPosition = btnPos;
		myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, btnSize.x);
		myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, btnSize.y);

		// Create item sprite
		GameObject btnItemObj = new GameObject("Ban List Item", typeof(RectTransform));
		RectTransform btnItemTransform = btnItemObj.GetComponent<RectTransform>();
		btnItemTransform.SetParent(myTransform, false);
		btnItemTransform.localScale = Vector3.one;

		itemImage = btnItemObj.AddComponent<Image>();
		itemImage.sprite = itemType.OffScreenSprite;
		itemImage.color = GetItemColor();
		if (!startOn)
		{
			itemImage.color *= myBanList.OffDarknessMultiplier;
		}

		float itemSize = itemType.PauseScreenSize;
		if (itemSize > myBanList.ItemStandardSize)
		{
			itemSize = myBanList.ItemStandardSize + (itemType.PauseScreenSize - myBanList.ItemStandardSize) * myBanList.LargeItemScalePercent;
		}
		itemSize *= myBanList.ItemSizePercent;
		btnItemTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize);
		if (itemType.OffScreenSprite.bounds.size.y != 0)
		{
			btnItemTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
				(itemType.OffScreenSprite.bounds.size.x / itemType.OffScreenSprite.bounds.size.y) * itemSize);
		}

		btnItemTransform.rotation = Quaternion.Euler(0, 0, itemType.PauseScreenRotation);

		// Create toggle indicator, which shows up as either a checkmark or an x mark
		GameObject togObj = new GameObject("Toggle Indicator", typeof(RectTransform));
		myToggleIndicator = togObj.AddComponent<Image>();
		myToggleIndicator.sprite = (isOn ? myBanList.OnSprite : myBanList.OffSprite);

		RectTransform togTransform = togObj.GetComponent<RectTransform>();
		togTransform.SetParent(myTransform, false);
		togTransform.localScale = Vector3.one;

		togTransform.anchoredPosition = new Vector2(myBanList.ButtonSize.x / 2f - myBanList.ToggleSize.x / 2f,
			myBanList.ButtonSize.y / 2f - myBanList.ToggleSize.y / 2f);
		togTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, myBanList.ToggleSize.x);
		togTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, myBanList.ToggleSize.y);

		currentScale = 1f;
	}

	void Update()
	{
		if (currentScale < 1)
		{
			currentScale += (1 - currentScale) * Time.deltaTime * myBanList.ButtonEaseFactor;
			currentScale = Mathf.Clamp(currentScale, 0, 1);
			if (1 - currentScale < 0.01f)
			{
				currentScale = 1;
			}

			myTransform.localScale = Vector3.one * currentScale;
		}
	}

	void SetScale(float inScale)
	{
		currentScale = inScale;
		myTransform.localScale = Vector3.one * currentScale;
	}

	// Toggles whether this item is on or off, and saves.
	void ToggleItem()
	{
		SetScale(myBanList.ButtonPressScale);

		// Do not turn off if there is only one type left on
		if (myBanList.NumTypesOn > 1 || !isOn)
		{
			isOn = !isOn;
			myToggleIndicator.sprite = (isOn ? myBanList.OnSprite : myBanList.OffSprite);
			itemImage.color = GetItemColor();

			if (isOn)
			{
				myBanList.NumTypesOn++;
			}
			else
			{
				myBanList.NumTypesOn--;
			}

			myBanList.Save();

			myBanList.MyAudioSource.PlayOneShot(myBanList.ToggleSound, myBanList.ToggleSoundVolume);
		}
		else
		{
			myBanList.MyAudioSource.PlayOneShot(myBanList.ErrorSound, myBanList.ErrorSoundVolume);
		}
	}

	// Sets the item to allowed or banned, whatever it was before.  Can save afterwards, if needed.
	public void ChangeItemSetting(bool isNowOn, bool shouldSave = true)
	{
		if (isOn != isNowOn && (myBanList.NumTypesOn > 1 || isNowOn))
		{
			isOn = isNowOn;
			if (isOn)
			{
				myBanList.NumTypesOn++;
			}
			else
			{
				myBanList.NumTypesOn--;
			}
			myToggleIndicator.sprite = (isOn ? myBanList.OnSprite : myBanList.OffSprite);
			itemImage.color = GetItemColor();

			if (shouldSave)
			{
				myBanList.Save();
			}

			SetScale(myBanList.ButtonPressScale);
		}
	}

	Color GetItemColor()
	{
		Color itemColor = myItemType.ItemColor;
		if (!isOn)
		{
			itemColor.r *= myBanList.OffDarknessMultiplier;
			itemColor.g *= myBanList.OffDarknessMultiplier;
			itemColor.b *= myBanList.OffDarknessMultiplier;
		}
		return itemColor;
	}
}
