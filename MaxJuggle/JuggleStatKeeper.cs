// Programmer: Carl Childers
// Date: 9/8/2017
//
// Keeps track of which objects have been juggled in a game.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleStatKeeper : ScriptableObject {

	static JuggleStatKeeper theStatKeeper;

	Dictionary<JuggleObjectType, int> fullyJuggledObjects;
	public Dictionary<JuggleObjectType, int> FullyJuggledObjects {
		get { return fullyJuggledObjects; }
	}

	int numJuggledObjects = 0;
	public int NumJuggledObjects {
		get { return numJuggledObjects; }
	}


	public static JuggleStatKeeper GetStatKeeper()
	{
		if (theStatKeeper != null)
		{
			return theStatKeeper;
		}
		theStatKeeper = FindObjectOfType<JuggleStatKeeper>();
		if (theStatKeeper != null)
		{
			return theStatKeeper;
		}

		theStatKeeper = CreateInstance<JuggleStatKeeper>();
		return theStatKeeper;
	}

	public void AddJuggledObject(JuggleObjectType inType)
	{
		if (fullyJuggledObjects == null)
		{
			fullyJuggledObjects = new Dictionary<JuggleObjectType, int>();
		}

		if (!fullyJuggledObjects.ContainsKey(inType))
		{
			fullyJuggledObjects.Add(inType, 0);
		}

		fullyJuggledObjects[inType]++;
		numJuggledObjects++;
	}

	public void Restart()
	{
		if (fullyJuggledObjects != null)
		{
			fullyJuggledObjects.Clear();
		}
		numJuggledObjects = 0;
	}

	public void RecreateFromBookmark(Bookmark inBookmark)
	{
		if (fullyJuggledObjects == null)
		{
			fullyJuggledObjects = new Dictionary<JuggleObjectType, int>();
		}
		else
		{
			fullyJuggledObjects.Clear();
		}

		if (inBookmark.GameState.JuggledItems != null)
		{
			foreach (string itemLabel in inBookmark.GameState.JuggledItems.Keys)
			{
				JuggleObjectType itemType = Bookmark.GetItemTypeFromLabel(itemLabel);
				if (!fullyJuggledObjects.ContainsKey(itemType))
				{
					fullyJuggledObjects.Add(itemType, inBookmark.GameState.JuggledItems[itemLabel]);
				}
			}
		}
	}
}
