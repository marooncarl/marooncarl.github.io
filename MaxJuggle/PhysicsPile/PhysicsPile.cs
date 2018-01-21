// Programmer: Carl Childers
// Date: 9/23/2017
//
// Creates rigid body objects for the pile of juggled objects.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPile : MonoBehaviour {

	public int BackgroundIndex = 1;

	public int MaxItems = 300;
	public int MinIntervalThreshold = 100;		// When this many items or more are dropped, the minimum time between drops is used.
	public float MinX = -8;
	public float MaxX = 8;
	public float MinDropDistance = 12;
	public float MaxDropDistance = 13;
	public float MinFallSpeed = 8;
	public float MaxFallSpeed = 10;
	public float MinInterval = 0.05f;
	public float MaxInterval = 0.3333f;
	public float StartDelay = 0;
	public float MinFallAngle = 170;
	public float MaxFallAngle = 190;
	public float MinBoardDelay = 1;
	public float MaxBoardDelay = 3;

	public AudioClip DropSound;
	[Range(0, 2)]
	public float DropSoundVolume = 1;

	public Animator ResultsBoard;
	public string ResultsShowTrigger = "Intro";

	// Determined by the number of items needed
	float itemInterval;
	float startCounter, itemCounter;

	Dictionary<JuggleObjectType, int> copiedItemList;
	List<JuggleObjectType> remainingKeys;

	int numItemsCreated;

	float boardCounter;
	bool shownBoard;

	protected AudioSource myAudioSource;


	void Start()
	{
		Initialize();
	}

	protected virtual void Initialize()
	{
		myAudioSource = GetComponent<AudioSource>();

		Dictionary<JuggleObjectType, int> juggledItems = GetItemsJuggled();
		if (juggledItems != null)
		{
			SetUpPile(juggledItems);
		}

		// Post game screen - trigger background transition
		if (BackgroundIndex > 0)
		{
			GameControl.GetGameControl().Background.SwitchBackground(BackgroundIndex);
		}
	}

	protected virtual Dictionary<JuggleObjectType, int> GetItemsJuggled()
	{
		JuggleStatKeeper statKeeper = JuggleStatKeeper.GetStatKeeper();
		if (statKeeper != null && statKeeper.FullyJuggledObjects != null)
		{
			return statKeeper.FullyJuggledObjects;
		}
		else
		{
			return null;
		}
	}

	protected void SetUpPile(Dictionary<JuggleObjectType, int> juggledItems)
	{
		copiedItemList = new Dictionary<JuggleObjectType, int>(juggledItems);

		remainingKeys = new List<JuggleObjectType>(copiedItemList.Keys.Count);
		foreach (JuggleObjectType key in copiedItemList.Keys)
		{
			remainingKeys.Add(key);
		}

		// Count the items to drop
		int totalItems = 0;
		foreach (JuggleObjectType key in remainingKeys)
		{
			totalItems += copiedItemList[key];
		}

		// Determine time between items
		float alpha = 1f - Mathf.Clamp((float)totalItems / MinIntervalThreshold, 0f, 1f);
		itemInterval = Mathf.Lerp(MinInterval, MaxInterval, alpha);

		if (StartDelay > 0)
		{
			startCounter = StartDelay;
		}
		else
		{
			AddNextItem();
		}

		// Delay the stats board depending on the number of items
		if (totalItems > 0 && MaxItems > 0)
		{
			boardCounter = StartDelay + MinBoardDelay + (MaxBoardDelay - MinBoardDelay) * ((float)totalItems / MaxItems);
			if (ResultsBoard != null)
			{
				// Make the board wait a little longer until switching pages
				JuggleStatsBook statsBook = ResultsBoard.GetComponent<JuggleStatsBook>();
				if (statsBook != null)
				{
					statsBook.SetCurrentPageDuration(boardCounter + statsBook.PageDisplayDuration);
				}
			}
		}
	}

	void Update()
	{
		if (startCounter > 0)
		{
			startCounter -= Time.deltaTime;
			if (startCounter <= 0)
			{
				AddNextItem();
			}
		}
		else if (itemCounter > 0)
		{
			itemCounter -= Time.deltaTime;
			if (itemCounter <= 0)
			{
				AddNextItem();
			}
		}

		// Update delay before the stats board is shown.
		if (!shownBoard && ResultsShowTrigger != "")
		{
			boardCounter -= Time.deltaTime;
			if (boardCounter <= 0)
			{
				shownBoard = true;
				if (ResultsBoard != null)
				{
					ResultsBoard.SetTrigger(ResultsShowTrigger);
				}
			}
		}
	}

	void AddNextItem()
	{
		if (copiedItemList.Count > 0 && numItemsCreated < MaxItems)
		{
			int keyIndex = Random.Range(0, remainingKeys.Count);
			JuggleObjectType pickedType = remainingKeys[keyIndex];

			if (pickedType.OverridePileTypes == null || pickedType.OverridePileTypes.Length == 0)
			{
				AddFromJuggleItemType( pickedType );
			}
			else
			{
				// Create from override types instead of this one
				foreach (JuggleObjectType type in pickedType.OverridePileTypes)
				{
					AddFromJuggleItemType(type);
				}
			}

			copiedItemList[pickedType] -= 1;
			if (copiedItemList[pickedType] <= 0)
			{
				copiedItemList.Remove(pickedType);
				remainingKeys.RemoveAt(keyIndex);
			}

			itemCounter = itemInterval;
		}
		else
		{
			// Clean up
			copiedItemList = null;
			remainingKeys = null;
		}
	}

	void AddFromJuggleItemType(JuggleObjectType inType)
	{
		if (inType.PilePrefab == null)
		{
			Debug.LogError("No Pile Prefab for " + inType.SingularName);
			return;
		}

		Vector3 createPos = transform.position + new Vector3( Random.Range(MinX, MaxX), Random.Range(MinDropDistance, MaxDropDistance) );
		PhysicsPileItem newPileObj = Instantiate(inType.PilePrefab, createPos, Quaternion.identity);
		newPileObj.transform.SetParent(transform);
		newPileObj.MyType = inType;
		newPileObj.Initialize();

		// Set pile item velocity
		float fallDegrees = Random.Range(MinFallAngle, MaxFallAngle);
		float fallRadians = (fallDegrees + 90) * Mathf.Deg2Rad;

		Vector2 initialVelocity = new Vector2(Mathf.Cos(fallRadians), Mathf.Sin(fallRadians)) * Random.Range(MinFallSpeed, MaxFallSpeed);
		newPileObj.MyRigidbody.velocity = initialVelocity;

		newPileObj.SetInitialRotation(inType);

		if (DropSound != null)
		{
			newPileObj.MyAudioSource.PlayOneShot(DropSound, DropSoundVolume);
		}

		numItemsCreated++;

		newPileObj.PostInit();
	}
}
