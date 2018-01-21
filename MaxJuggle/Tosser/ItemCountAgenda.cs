// Programmer: Carl Childers
// Date: 11/1/2017
//
// A list of item counts for the game tosser to run through when not introducing types.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Item Count Agenda")]
public class ItemCountAgenda : ScriptableObject {

	public ItemCountSet[] ItemCounts;


	public int Length {
		get { return ItemCounts.Length; }
	}

	public ItemCountSet this[int index]
	{
		get {
			if (index >= 0 && index < ItemCounts.Length)
			{
				return ItemCounts[index];
			}
			else
			{
				return null;
			}
		}
	}
}

[System.Serializable]
public class ItemCountSet {

	public int BasicCount;
	public int AdvancedCount;

	public int TotalCount {
		get { return BasicCount + AdvancedCount; }
	}
}
