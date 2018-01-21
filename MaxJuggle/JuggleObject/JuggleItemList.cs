// Programmer: Carl Childers
// Date: 9/12/2017
//
// List of juggle object types that are used in gameplay.  This allows multiple monobehaviours to reference the same list.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Juggle Item List")]
public class JuggleItemList : ScriptableObject {

	public JuggleObjectType[] Types;

	// Array-like accessors for convenience

	public int Length {
		get { return Types.Length; }
	}

	public JuggleObjectType this[int key]
	{
		get { return Types[key]; }
	}
}
