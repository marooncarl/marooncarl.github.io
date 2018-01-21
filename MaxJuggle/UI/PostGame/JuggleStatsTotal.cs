// Programmer: Carl Childers
// Date: 11/8/2017
//
// Displays the total number of items juggled.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JuggleStatsTotal : MonoBehaviour {

	void Start()
	{
		JuggleStatKeeper statKeeper = JuggleStatKeeper.GetStatKeeper();
		int totalItems = 0;
		int keyCount = 0;

		if (statKeeper.FullyJuggledObjects != null)
		{
			foreach (JuggleObjectType type in statKeeper.FullyJuggledObjects.Keys)
			{
				totalItems += statKeeper.FullyJuggledObjects[type];
				keyCount++;
			}
		}

		Text textComp = GetComponent<Text>();
		if (keyCount > 2)
		{
			textComp.text = string.Format(textComp.text, totalItems);
		}
		else
		{
			textComp.enabled = false;
		}
	}
}
