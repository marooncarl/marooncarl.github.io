// Programmer: Carl Childers
// Date: 11/2/2017
//
// Generic class that contains a list of items shuffled in random order.
// Can use GetNext() to get the next item, and when the entire list has been gotten, it is reshuffled.
// New items can be added with Add(), which are inserted into random points in the list.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffledList<T> {

	List<T> myList;
	int currentIndex;

	T lastItem;

	/// <summary>
	/// Returns the number of items in the shuffled list.
	/// </summary>
	public int Count {
		get { return myList.Count; }
	}

	/// <summary>
	/// Gets the last item returned by GetNext() in case repetition is desired.
	/// If GetNext() hasn't been used yet, this returns the default value.
	/// </summary>
	public T LastItem {
		get { return lastItem; }
	}


	public ShuffledList(params T[] startingItems)
	{
		myList = new List<T>(startingItems);
		currentIndex = 0;
		lastItem = default(T);

		if (myList.Count > 1)
		{
			Shuffle();
		}
	}

	/// <summary>
	/// Adds an item or items to a random position in the list.
	/// If there are multiple items, each one is placed in a different random position.
	/// If an item is placed before or at the current index, the current index is pushed forward.
	/// </summary>
	/// <param name="newItems">Item(s) to add.</param>
	public void Add(params T[] newItems)
	{
		myList.Capacity = myList.Count + newItems.Length;

		foreach (T item in newItems)
		{
			int index = Random.Range(0, myList.Count);
			myList.Insert(index, item);
			if (index <= currentIndex && currentIndex < myList.Count - 1)
			{
				currentIndex++;
			}
		}
	}

	/// <summary>
	/// Removes all instances of the given item(s) from the shuffled list.
	/// </summary>
	/// <param name="oldItems">Item(s) to be removed.</param>
	public void Remove(params T[] oldItems)
	{
		foreach (T item in oldItems)
		{
			while (myList.Contains(item))
			{
				int index = myList.IndexOf(item);
				myList.RemoveAt(index);
				if (index > currentIndex && currentIndex > 0)
				{
					currentIndex--;
				}
			}
		}
		myList.TrimExcess();
	}

	/// <summary>
	/// Clears the shuffled list.
	/// </summary>
	public void Clear()
	{
		myList.Clear();
		currentIndex = 0;
	}

	/// <summary>
	/// Returns whether the shuffled list contains any of the specified item.
	/// </summary>
	public bool Contains(T item)
	{
		return myList.Contains(item);
	}

	/// <summary>
	/// Returns the next item from the shuffled list, and if the end is reached, reshuffles the list.
	/// </summary>
	/// <returns>The next item</returns>
	public T GetNext()
	{
		if (myList.Count == 0)
		{
			return default(T);
		}

		lastItem = myList[currentIndex];
		currentIndex++;
		if (currentIndex >= myList.Count)
		{
			currentIndex = 0;
			if (myList.Count > 1)
			{
				Shuffle();
			}
		}
		return lastItem;
	}

	void Shuffle()
	{
		List<T> listCopy = new List<T>(myList);
		myList.Clear();

		while (listCopy.Count > 0)
		{
			int index = Random.Range(0, listCopy.Count);
			myList.Add(listCopy[index]);
			listCopy.RemoveAt(index);
		}

		if (myList[0].Equals(lastItem) && myList.Count > 1)
		{
			// Swap the first item with a random item in the list.
			T temp = myList[0];
			int index = Random.Range(1, myList.Count - 1);
			myList[0] = myList[index];
			myList[index] = temp;
		}
	}
}
