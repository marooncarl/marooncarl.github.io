// Programmer: Carl Childers
// Date: 11/9/2017
//
// Only shows when the player hasn't juggled many things.  Randomizes the message shown.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotivationalText : MonoBehaviour {

	// If the player exceeds any of these counts, the message will be hidden.
	public int MaxItemsJuggled = 5;
	public int MaxTypesJuggled = 2;

	public string[] Messages;

	public string AllBannedMessage;


	void Start()
	{
		Text myText = GetComponent<Text>();
		if (myText != null)
		{
			JuggleStatKeeper statKeeper = JuggleStatKeeper.GetStatKeeper();

			int totalItems = 0;
			int keyCount = 0;

			if (statKeeper.FullyJuggledObjects != null)
			{
				totalItems = statKeeper.NumJuggledObjects;
				keyCount = statKeeper.FullyJuggledObjects.Keys.Count;
			}

			if (totalItems > MaxItemsJuggled || keyCount > MaxTypesJuggled)
			{
				myText.enabled = false;
			}
			else if (totalItems > 0 || !AreAllItemsBanned())
			{
				if (myText.enabled && Messages.Length > 0)
				{
					myText.text = Messages[ Random.Range(0, Messages.Length) ];
				}
			}
			else
			{
				myText.text = AllBannedMessage;
			}
		}
	}

	bool AreAllItemsBanned()
	{
		Dictionary<JuggleObjectType, bool> loadedBanList = JuggleBanList.Load();
		if (loadedBanList == null || loadedBanList.Count == 0)
			return false;

		foreach (JuggleObjectType itemType in loadedBanList.Keys)
		{
			if (loadedBanList[itemType])
			{
				return false;
			}
		}

		return true;
	}
}
