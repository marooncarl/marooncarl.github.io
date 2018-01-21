// Programmer: Carl Childers
// Date: 11/1/2017
//
// Categorizes items by tiers, so if the tosser is looking for items in a certain tier, it can only look through those ones.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Tiered Item List")]
public class TieredItemList : ScriptableObject {

	public JuggleItemList[] Tiers;


	public int Length {
		get { return Tiers.Length; }
	}

	public JuggleItemList this[int index] {
		get {
			if (index >= 0 && index < Tiers.Length)
			{
				return Tiers[index];
			}
			else
			{
				return null;
			}
		}
	}

	public int TotalItems {
		get {
			int count = 0;
			foreach (JuggleItemList tier in Tiers)
			{
				count += tier.Length;
			}
			return count;
		}
	}
}
