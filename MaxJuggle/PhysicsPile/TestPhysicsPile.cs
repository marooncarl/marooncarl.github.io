// Programmer: Carl Childers
// Date: 10/30/2017
//
// A physics pile that can take a list of item types and counts for each type, and spawn items based on that.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhysicsPile : PhysicsPile {

	public ItemDropData[] ItemsToDrop;


	protected override void Initialize()
	{
		// Skip background animation, and do everything else

		myAudioSource = GetComponent<AudioSource>();

		Dictionary<JuggleObjectType, int> juggledItems = GetItemsJuggled();
		if (juggledItems != null)
		{
			SetUpPile(juggledItems);
		}
	}

	protected override Dictionary<JuggleObjectType, int> GetItemsJuggled()
	{
		Dictionary<JuggleObjectType, int> outDictionary = new Dictionary<JuggleObjectType, int>(ItemsToDrop.Length);
		foreach (ItemDropData data in ItemsToDrop)
		{
			if (!outDictionary.ContainsKey(data.ItemType))
			{
				outDictionary.Add(data.ItemType, data.Count);
			}
			else
			{
				outDictionary[data.ItemType] += data.Count;
			}
		}

		return outDictionary;
	}
}

[System.Serializable]
public class ItemDropData {

	public JuggleObjectType ItemType;
	public int Count = 1;
}
