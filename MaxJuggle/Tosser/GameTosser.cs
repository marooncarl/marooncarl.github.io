// Programmer: Carl Childers
// Date: 9/7/2017
//
// Tosses objects into the air for the player to juggle.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTosser : Tosser {

	public int MinStagesBetweenIntros = 1;
	public int MaxStagesBetweenIntros = 2;
	public int ItemsPerTier = 2;
	public int DefaultMaxItems = 4;				// Used when agenda counts can't be used due to all basic or all advanced items banned
	public int StagesBetweenCleanup = 5;
	public int[] BasicIntroStages;				// Stages at which new basic items are introduced.
	public float EndGameWait = 1;
	public float RepeatChance = 0.1f;
	public float CleanUpDelay = 1.5f;
	public ItemCountAgenda MyItemCounts;

	public int AgendaSkipIndex = 3;
	public int SkipStartingBasicTiers = 2;
	public int SkipStartingAdvTiers = 2;
	public int SkipStartingStage = 10;

	#if UNITY_EDITOR
	public bool GodMode = false;
	#endif

	public TieredItemList BasicTypes;
	public TieredItemList AdvancedTypes;

	public AudioClip ItemDropSound;
	[Range(0, 2)]
	public float ItemDropSoundVolume = 1;

	public AudioClip GameOverSound;
	[Range(0, 2)]
	public float GameOverSoundVolume = 1;

	Juggler theJuggler;
	public Juggler TheJuggler { 
		get { return theJuggler; }
	}

	int fullJugglesToNextStage;
	public int FullJugglesToNextStage {
		get { return fullJugglesToNextStage; }
	}

	int currentStage = 1;
	public int CurrentStage {
		get { return currentStage; }
	}

	// The type of object being introduced to the player.  Will be spawned solo until the player juggles one.
	JuggleObjectType introducedType;
	public JuggleObjectType IntroducedType {
		get { return introducedType; }
	}

	// Introduced type decided before it is actually introduced
	JuggleObjectType pendingIntroducedType;
	JuggleObjectType lastIntroducedType;

	int stageToIntroduce;					// The last decided stage to introduce a type at.  Can be in the future or in the past.

	int countdownToNextIntroduction;
	public int CountdownToNextIntroduction {
		get { return countdownToNextIntroduction; }
	}

	ShuffledList<JuggleObjectType> availableBasicTypes;
	ShuffledList<JuggleObjectType> availableAdvancedTypes;

	LinkedList<JuggleObjectType> allAvailableTypes;
	public LinkedList<JuggleObjectType> AllAvailableTypes {
		get { return allAvailableTypes; }
	}

	float endGameCounter;

	int desiredAdvancedItems;				// How many advanced items can be in play (i.e. items other than balls)

	int advancedItemsInPlay;
	public int AdvancedItemsInPlay {
		get { return advancedItemsInPlay; }
	}

	bool usedMinStagesBetweenLast = false;

	// When all basic or all advanced items are banned, the normal item count agenda cannot be followed.
	// In this case, this index is instead used to keep track of the target item count, so another variable is not needed for save data.
	int itemCountAgendaIndex;

	int totalAdvancedTypes;
	int currentAdvTier;
	int typesIntroducedThisTier;

	int basicIntroIndex;
	int lastBasicTier;

	AudioSource myAudioSource;

	int stagesSinceLastCleanup = 0;
	float cleanUpCountdown = 0;

	int itemsTossedThisStage;

	Dictionary<JuggleObjectType, bool> loadedBanList;
	bool allBasicBanned, allAdvancedBanned;

	#if UNITY_EDITOR
	bool pressedLeftOrRight;
	#endif


	void Awake()
	{
		theJuggler = GameObject.FindObjectOfType<Juggler>();
		myAudioSource = GetComponent<AudioSource>();

		totalAdvancedTypes = AdvancedTypes.TotalItems;
	}

	public override void Restart()
	{
		base.Restart();
		loadedBanList = JuggleBanList.Load();
		CheckBanList();

		// Restart the list of item types that are available to toss
		if (availableAdvancedTypes == null)
		{
			availableAdvancedTypes = new ShuffledList<JuggleObjectType>();
		}
		else
		{
			availableAdvancedTypes.Clear();
		}

		advancedItemsInPlay = 0;

		allAvailableTypes = new LinkedList<JuggleObjectType>();

		basicIntroIndex = 0;
		if (availableBasicTypes == null)
		{
			availableBasicTypes = new ShuffledList<JuggleObjectType>();
		}
		else
		{
			availableBasicTypes.Clear();
		}

		stagesSinceLastCleanup = 0;
		cleanUpCountdown = 0;

		if (allBasicBanned && allAdvancedBanned)
		{
			theJuggler.EndGame();
		}

		GameControl ctrl = GameControl.GetGameControl();
		if (ctrl.TheBookmark != null && ctrl.TheBookmark.GameState.Usable)
		{
			// Wait a tiny bit before checking bookmark to make sure both the juggler and tosser have restarted
			Invoke("StartLoadingFromBookmark", 0.01f);
		}
		else
		{
			StartNewGame();
		}
	}

	// Called when NOT loading from a bookmark.
	void StartNewGame()
	{
		bool skippingIntro = (PlayerPrefs.GetInt("SkipIntro", 0) != 0);

		AddInitialBasicTypes(skippingIntro);
		introducedType = null;
		pendingIntroducedType = null;
		lastIntroducedType = null;
		usedMinStagesBetweenLast = false;
		stageToIntroduce = 0;
		lastBasicTier = 0;
		itemsTossedThisStage = 0;

		if (!skippingIntro)
		{
			// Normal game from the beginning
			currentStage = 1;

			if (allBasicBanned)
			{
				itemCountAgendaIndex = 1;
				desiredObjectsInPlay = 1;
				desiredAdvancedItems = 1;
				// Need to introduce an advanced type right away
				countdownToNextIntroduction = 0;
				introducedType = GetTypeToIntroduce(currentStage);
			}
			else if (allAdvancedBanned)
			{
				itemCountAgendaIndex = 1;
				desiredObjectsInPlay = 1;
				desiredAdvancedItems = 0;
			}
			else
			{
				itemCountAgendaIndex = 0;
				desiredObjectsInPlay = MyItemCounts[itemCountAgendaIndex].TotalCount;
				desiredAdvancedItems = MyItemCounts[itemCountAgendaIndex].AdvancedCount;
			}

			currentAdvTier = 0;
			fullJugglesToNextStage = desiredObjectsInPlay;
			countdownToNextIntroduction = MaxStagesBetweenIntros;
		}
		else
		{
			// Skipping intro, starting with more items, and with some advanced types already introduced
			// Since there's so much to start with, it takes a little longer to get past the first stage and get to the first introduced type (if any)
			currentStage = SkipStartingStage;

			if (allBasicBanned)
			{
				itemCountAgendaIndex = 3;
				desiredObjectsInPlay = 3;
				desiredAdvancedItems = 3;
			}
			else if (allAdvancedBanned)
			{
				itemCountAgendaIndex = 3;
				desiredObjectsInPlay = 3;
				desiredAdvancedItems = 0;
			}
			else
			{
				itemCountAgendaIndex = AgendaSkipIndex;
				desiredObjectsInPlay = MyItemCounts[itemCountAgendaIndex].TotalCount;
				desiredAdvancedItems = MyItemCounts[itemCountAgendaIndex].AdvancedCount;
			}

			SkipThroughAdvancedTiers();
			currentAdvTier = SkipStartingAdvTiers;
			fullJugglesToNextStage = desiredObjectsInPlay * 2;
			countdownToNextIntroduction = MaxStagesBetweenIntros * 2;
		}
	}

	// Should happen after adding initial basic types, since it only goes outside the intended tiers if there are no items available at all.
	void SkipThroughAdvancedTiers()
	{
		int tiersLeft = SkipStartingAdvTiers;
		foreach (JuggleItemList tier in AdvancedTypes.Tiers)
		{
			List<JuggleObjectType> copiedTypes = new List<JuggleObjectType>(tier.Types);

			int itemsLeft = ItemsPerTier;
			while(itemsLeft > 0 && copiedTypes.Count > 0)
			{
				int pickedIndex = Random.Range(0, copiedTypes.Count);

				if (!IsTypeBanned(copiedTypes[pickedIndex]))
				{
					availableAdvancedTypes.Add(copiedTypes[pickedIndex]);
					allAvailableTypes.AddLast(copiedTypes[pickedIndex]);
					itemsLeft--;
				}

				copiedTypes.RemoveAt(pickedIndex);
			}

			tiersLeft--;
			if (tiersLeft <= 0 && allAvailableTypes.Count > 0)
			{
				break;
			}
		}
	}

	// Add first-tier basic types to the available list, when not banned.
	// If all of these are banned, then try adding ones from later tiers until one is added.
	// skipping tells whether the player decided to skip the intro stages or not.
	void AddInitialBasicTypes(bool skipping = false)
	{
		int tiersLeft = 1;
		if (skipping)
		{
			tiersLeft = SkipStartingBasicTiers;
		}
		foreach (JuggleItemList tier in BasicTypes.Tiers)
		{
			foreach (JuggleObjectType itemType in tier.Types)
			{
				if (!IsTypeBanned(itemType))
				{
					availableBasicTypes.Add(itemType);
					allAvailableTypes.AddLast(itemType);
				}
			}

			tiersLeft--;
			if (tiersLeft <= 0)
			{
				break;
			}
		}

		if (availableBasicTypes.Count == 0 && BasicTypes.Tiers.Length > 1)
		{
			bool foundBasicItem = false;
			for (int i = 1; i < BasicTypes.Tiers.Length; ++i)
			{
				foreach (JuggleObjectType itemType in BasicTypes.Tiers[i].Types)
				{
					if (!IsTypeBanned(itemType))
					{
						availableBasicTypes.Add(itemType);
						allAvailableTypes.AddLast(itemType);
						foundBasicItem = true;
						break;
					}
				}
				if (foundBasicItem)
				{
					break;
				}
			}
		}
	}

	// Checks if all basic types are banned or all advanced types are banned
	void CheckBanList()
	{
		allBasicBanned = true;
		allAdvancedBanned = true;

		for (int i = 0; i < BasicTypes.Tiers.Length; ++i)
		{
			foreach (JuggleObjectType itemType in BasicTypes.Tiers[i].Types)
			{
				if (!IsTypeBanned(itemType))
				{
					allBasicBanned = false;
					break;
				}
			}
			if (!allBasicBanned)
				break;
		}

		for (int i = 0; i < AdvancedTypes.Tiers.Length; ++i)
		{
			foreach (JuggleObjectType itemType in AdvancedTypes.Tiers[i].Types)
			{
				if (!IsTypeBanned(itemType))
				{
					allAdvancedBanned = false;
					break;
				}
			}
			if (!allAdvancedBanned)
				break;
		}
	}

	protected override void CustomUpdate()
	{
		if (!theJuggler.InGameOver)
		{
			UpdateTossing();
		}
		else if (NumObjectsInPlay <= 0)
		{
			endGameCounter -= Time.deltaTime;
			if (endGameCounter <= 0)
			{
				theJuggler.MoveToPostGame();
			}
		}

		if (cleanUpCountdown > 0)
		{
			cleanUpCountdown -= Time.deltaTime;
			if (cleanUpCountdown <= 0)
			{
				CleanUnusedObjects();
			}
		}

		#if UNITY_EDITOR
		if (GodMode)
		{
			CheckChangeStage();
		}
		#endif
	}

	protected override JuggleObjectType PickItemType()
	{
		if (introducedType != null)
		{
			// Always toss the object being introduced
			return introducedType;
		}
		else if (allBasicBanned)
		{
			return GetRandomAdvancedType();
		}
		else if (allAdvancedBanned)
		{
			return GetRandomBasicType();
		}
		else if (WasItemIntroducedLastStage())
		{
			// Stage after introduction: ensure one basic item and one recently introduced item
			if (advancedItemsInPlay < 1)
				return lastIntroducedType;
			else
				return GetRandomBasicType();
		}
		else
		{
			// Regular case: Determine whether a basic type is needed, advanced, or if either one will do
			// Even if an advanced type is needed, if there are none available, fall back on a basic type
			int itemsLeft = desiredObjectsInPlay - NumObjectsInPlay;
			int advItemsLeft = desiredAdvancedItems - advancedItemsInPlay;
			if (advItemsLeft <= 0 || availableAdvancedTypes.Count == 0)
			{
				return GetRandomBasicType();
			}
			else if (itemsLeft <= advItemsLeft)
			{
				return GetRandomAdvancedType();
			}
			else
			{
				// either one will do
				if (Random.value < 0.5f)
				{
					return GetRandomBasicType();
				}
				else
				{
					return GetRandomAdvancedType();
				}
			}
		}
	}

	public void ObjectWasFullyJuggled(JuggleObjectType juggledType)
	{
		if (itemsTossedThisStage > 0)
		{
			// If a new type is being introduced, then the juggled object has to be that type to progress.
			if (introducedType == null || juggledType == introducedType)
			{
				fullJugglesToNextStage--;
			}

			if (fullJugglesToNextStage <= 0)
			{
				AdvanceStage();
			}
		}
	}

	void AdvanceStage()
	{
		currentStage++;

		if (!AreAllItemsIntroduced())
		{
			JuggleObjectType oldIntroducedType = introducedType;
			if (oldIntroducedType != null)
			{
				ClearedIntroducedType();
			}

			if (oldIntroducedType == null || NeedImmediateIntroduction())
			{
				UpdateTypeIntroduction();
			}
		}

		UpdateItemCount();
		UpdateStageGoal();
		CheckBasicTypeIntroduction();

		stagesSinceLastCleanup++;
		if (stagesSinceLastCleanup > StagesBetweenCleanup)
		{
			//Invoke("CleanUnusedObjects", CleanUpDelay);
			cleanUpCountdown = CleanUpDelay;
			stagesSinceLastCleanup = 0;
		}

		itemsTossedThisStage = 0;
	}

	bool NeedImmediateIntroduction()
	{
		return (availableBasicTypes.Count == 0 && availableAdvancedTypes.Count < 2);
	}

	// Introduce a type if needed, or clear the previous introduced type
	void UpdateTypeIntroduction()
	{
		bool needImmediate = NeedImmediateIntroduction();

		if ((introducedType == null && !WasItemIntroducedLastStage()) || needImmediate)
		{
			if (needImmediate)
			{
				countdownToNextIntroduction = 0;
			}
			else
			{
				countdownToNextIntroduction--;
			}

			if (countdownToNextIntroduction <= 0)
			{
				// Try to introduce a new type
				if (pendingIntroducedType != null)
				{
					introducedType = pendingIntroducedType;
					pendingIntroducedType = null;
				}
				else
				{
					introducedType = GetTypeToIntroduce(currentStage);
				}

				stageToIntroduce = currentStage;

				// Advance to next tier every x items introduced
				if (introducedType != null)
				{
					typesIntroducedThisTier++;
					if (typesIntroducedThisTier >= ItemsPerTier)
					{
						currentAdvTier++;
						typesIntroducedThisTier = 0;
					}

					// Clean up items
					//Invoke("CleanUnusedObjects", CleanUpDelay);
					cleanUpCountdown = CleanUpDelay;
					stagesSinceLastCleanup = -1;		// Since AdvanceStage() will increment it back to zero
				}
			}
		}
	}

	void ClearedIntroducedType()
	{
		availableAdvancedTypes.Add(introducedType);
		allAvailableTypes.AddLast(introducedType);
		lastIntroducedType = introducedType;
		introducedType = null;
		countdownToNextIntroduction = GetStagesBeforeNextIntro() + 1;
	}

	int GetStagesBeforeNextIntro()
	{
		if (!allBasicBanned)
		{
			int returnValue = (usedMinStagesBetweenLast ? MaxStagesBetweenIntros : MinStagesBetweenIntros);
			usedMinStagesBetweenLast = !usedMinStagesBetweenLast;
			return returnValue;
		}
		else
		{
			return 0;
		}
	}

	public bool AreAllItemsIntroduced()
	{
		return (availableAdvancedTypes.Count >= totalAdvancedTypes);
	}

	// update desired / pending desired objects in play
	// If there are non-basic types in play, then just update pending desired count
	// and wait until there are only basic types for desired count to catch up
	void UpdateItemCount()
	{
		if (introducedType != null)
		{
			desiredObjectsInPlay = 1;
			desiredAdvancedItems = 1;
		}
		else if (allBasicBanned)
		{
			itemCountAgendaIndex = Mathf.Min(itemCountAgendaIndex + 1, DefaultMaxItems);
			desiredObjectsInPlay = itemCountAgendaIndex;
			desiredAdvancedItems = itemCountAgendaIndex;
		}
		else if (allAdvancedBanned)
		{
			itemCountAgendaIndex = Mathf.Min(itemCountAgendaIndex + 1, DefaultMaxItems);
			desiredObjectsInPlay = itemCountAgendaIndex;
			desiredAdvancedItems = 0;
		}
		else if (WasItemIntroducedLastStage())
		{
			desiredObjectsInPlay = 2;
			desiredAdvancedItems = 1;
		}
		else
		{
			// Move to next item count
			itemCountAgendaIndex++;
			if (itemCountAgendaIndex >= MyItemCounts.Length)
			{
				// When the end is reached, alternate between the last two counts.
				itemCountAgendaIndex = MyItemCounts.Length - 2;
			}

			desiredObjectsInPlay = MyItemCounts[itemCountAgendaIndex].TotalCount;
			desiredAdvancedItems = MyItemCounts[itemCountAgendaIndex].AdvancedCount;
		}
	}

	// Updates the number of full juggles to move to the next stage
	void UpdateStageGoal()
	{
		if (countdownToNextIntroduction > 1)
		{
			fullJugglesToNextStage = desiredObjectsInPlay;
		}
		else
		{
			// If about to introduce a type, and there is a type available to introduce, then end this stage early
			if (introducedType == null)
			{
				pendingIntroducedType = GetTypeToIntroduce(currentStage + 1);
			}
			if (pendingIntroducedType != null)
			{
				fullJugglesToNextStage = 1;
				stageToIntroduce = currentStage + 1;
			}
			else
			{
				fullJugglesToNextStage = desiredObjectsInPlay;
			}
		}
	}

	void CheckBasicTypeIntroduction()
	{
		if (basicIntroIndex < BasicIntroStages.Length && lastBasicTier + 1 < BasicTypes.Tiers.Length)
		{
			if (currentStage >= BasicIntroStages[basicIntroIndex])
			{
				lastBasicTier++;
				basicIntroIndex++;

				foreach (JuggleObjectType type in BasicTypes.Tiers[lastBasicTier].Types)
				{
					if (!IsTypeBanned(type) && !availableBasicTypes.Contains(type))
					{
						availableBasicTypes.Add(type);
						allAvailableTypes.AddLast(type);
					}
				}
			}
		}
	}

	public override void ObjectDropped(JuggleObject droppedObj)
	{
		theJuggler.ObjectDropped(droppedObj);
		if (ItemDropSound != null)
		{
			
			if (!myAudioSource.isPlaying)
			{
				myAudioSource.PlayOneShot(ItemDropSound, ItemDropSoundVolume);
			}
			else
			{
				// Play the sound somewhere else so it will sound better
				ExtraSoundPlayer.GetSoundPlayer().Play(ItemDropSound, droppedObj.transform.position, ItemDropSoundVolume, myAudioSource.outputAudioMixerGroup);
			}
		}
	}

	// Called when an item is fully juggled, dropped, or the player hurts themselves on it.
	public override void ItemRemovedFromPlay(JuggleObject removedObj)
	{
		base.ItemRemovedFromPlay(removedObj);

		if (removedObj.MyType.IsAdvancedItem)
		{
			advancedItemsInPlay--;
		}
	}

	public void GameEnded()
	{
		endGameCounter = EndGameWait;

		if (GameOverSound != null)
		{
			myAudioSource.PlayOneShot(GameOverSound, GameOverSoundVolume);
		}
	}

	JuggleObjectType GetTypeToIntroduce(int stageForUse)
	{
		if (availableAdvancedTypes.Count >= totalAdvancedTypes)
		{
			// No more types to introduce
			return null;
		}

		// Check the tier we're currently on.
		List<JuggleObjectType> validTypes = new List<JuggleObjectType>(totalAdvancedTypes - availableAdvancedTypes.Count);
		int tier = currentAdvTier;
		if (tier < AdvancedTypes.Length)
		{
			foreach (JuggleObjectType itemType in AdvancedTypes[tier].Types)
			{
				if (!availableAdvancedTypes.Contains(itemType) && !IsTypeBanned(itemType))
				{
					validTypes.Add(itemType);
				}
			}
		}

		// If there's nothing to introduce, check the earlier tiers, starting with the one before the current one.
		if (validTypes.Count == 0)
		{
			tier--;
			while(tier >= 0 && validTypes.Count == 0)
			{
				foreach (JuggleObjectType itemType in AdvancedTypes[tier].Types)
				{
					if (!availableAdvancedTypes.Contains(itemType) && !IsTypeBanned(itemType))
					{
						validTypes.Add(itemType);
					}
				}
				tier--;
			}
		}

		// If there's still nothing, check the tiers after the current one, starting with the one right after.
		if (validTypes.Count == 0)
		{
			tier = currentAdvTier + 1;
			while(tier < AdvancedTypes.Length && validTypes.Count == 0)
			{
				foreach (JuggleObjectType itemType in AdvancedTypes[tier].Types)
				{
					if (!availableAdvancedTypes.Contains(itemType) && !IsTypeBanned(itemType))
					{
						validTypes.Add(itemType);
					}
				}
				tier++;
			}
		}

		if (validTypes.Count > 0)
		{
			return validTypes[ Random.Range(0, validTypes.Count) ];
		}
		else
		{
			return null;
		}
	}

	JuggleObjectType GetRandomAdvancedType()
	{
		if (Random.value < RepeatChance && availableAdvancedTypes.LastItem != null)
		{
			return availableAdvancedTypes.LastItem;
		}
		else
		{
			return availableAdvancedTypes.GetNext();
		}
	}

	JuggleObjectType GetRandomBasicType()
	{
		//return BasicTypes[ Random.Range(0, BasicTypes.Length) ];
		if (Random.value < RepeatChance && availableBasicTypes.LastItem != null)
		{
			return availableBasicTypes.LastItem;
		}
		else
		{
			return availableBasicTypes.GetNext();
		}
	}

	// Item type cannot spawn if it is banned (unless everything is banned, in which case the tosser will toss the first possible basic item)
	bool IsTypeBanned(JuggleObjectType inType)
	{
		if (loadedBanList.ContainsKey(inType))
		{
			return !loadedBanList[inType];
		}
		else
		{
			return false;
		}
	}

	bool WasItemIntroducedLastStage()
	{
		return (currentStage - stageToIntroduce == 1 && stageToIntroduce > 0 && lastIntroducedType != null);
	}

	public override JuggleObject Toss(JuggleObjectType inType, bool addTossVelocity = true)
	{
		JuggleObject tossedObj = base.Toss(inType, addTossVelocity);

		if (inType.IsAdvancedItem)
		{
			advancedItemsInPlay++;
		}

		itemsTossedThisStage++;

		return tossedObj;
	}

	public void ShowItemTrails()
	{
		foreach (JuggleObject item in ObjectsInPlay)
		{
			item.ShowTrail();
		}
	}

	public void HideItemTrails()
	{
		foreach (JuggleObject item in ObjectsInPlay)
		{
			item.HideTrail();
		}
	}

	/*
	void CheckBookmark()
	{
		GameControl ctrl = GameControl.GetGameControl();
		if (ctrl.TheBookmark != null && ctrl.TheBookmark.GameState.Usable)
		{
			RecreateFromBookmark(ctrl.TheBookmark);
		}
	}
	*/

	void StartLoadingFromBookmark()
	{
		RecreateFromBookmark(GameControl.GetGameControl().TheBookmark);
	}

	public void SaveToGameState(GameStateData inGameState)
	{
		inGameState.TossCounter = tossCounter;
		if (introducedType != null)
		{
			inGameState.IntroducedType = introducedType.BookmarkLabel;
		}
		if (pendingIntroducedType != null)
		{
			inGameState.PendingIntroducedType = pendingIntroducedType.BookmarkLabel;
		}
		if (lastIntroducedType != null)
		{
			inGameState.LastIntroducedType = lastIntroducedType.BookmarkLabel;
		}
		inGameState.JugglesToNextStage = (byte)fullJugglesToNextStage;
		inGameState.CountdownToNextIntro = (byte)countdownToNextIntroduction;
		inGameState.UsedMinStagesBetweenLast = usedMinStagesBetweenLast;
		inGameState.StageToIntroduce = stageToIntroduce;
		inGameState.CurrentAdvTier = (byte)currentAdvTier;
		inGameState.ItemCountAgendaIndex = (byte)itemCountAgendaIndex;
		inGameState.BasicIntroIndex = (byte)basicIntroIndex;
		inGameState.LastBasicTier = (byte)lastBasicTier;
		inGameState.ItemsTossedThisStage = (byte)itemsTossedThisStage;
	}

	// Sets up stats and objects in play from the last play session
	void RecreateFromBookmark(Bookmark inBookmark)
	{
		currentStage = inBookmark.GameState.CurrentStage;

		tossCounter = inBookmark.GameState.TossCounter;
		introducedType = Bookmark.GetItemTypeFromLabel(inBookmark.GameState.IntroducedType);
		pendingIntroducedType = Bookmark.GetItemTypeFromLabel(inBookmark.GameState.PendingIntroducedType);
		lastIntroducedType = Bookmark.GetItemTypeFromLabel(inBookmark.GameState.LastIntroducedType);
		fullJugglesToNextStage = (int)inBookmark.GameState.JugglesToNextStage;
		countdownToNextIntroduction = (int)inBookmark.GameState.CountdownToNextIntro;
		usedMinStagesBetweenLast = inBookmark.GameState.UsedMinStagesBetweenLast;
		stageToIntroduce = inBookmark.GameState.StageToIntroduce;
		currentAdvTier = (int)inBookmark.GameState.CurrentAdvTier;
		itemCountAgendaIndex = (int)inBookmark.GameState.ItemCountAgendaIndex;
		basicIntroIndex = (int)inBookmark.GameState.BasicIntroIndex;
		lastBasicTier = (int)inBookmark.GameState.LastBasicTier;
		itemsTossedThisStage = (int)inBookmark.GameState.ItemsTossedThisStage;

		if (introducedType != null)
		{
			desiredObjectsInPlay = 1;
			desiredAdvancedItems = 1;
		}
		else if (WasItemIntroducedLastStage())
		{
			desiredObjectsInPlay = 2;
			desiredAdvancedItems = 1;
		}
		else if (allBasicBanned)
		{
			desiredObjectsInPlay = itemCountAgendaIndex;
			desiredAdvancedItems = itemCountAgendaIndex;
		}
		else if (allAdvancedBanned)
		{
			desiredObjectsInPlay = itemCountAgendaIndex;
			desiredAdvancedItems = 0;
		}
		else
		{
			desiredObjectsInPlay = MyItemCounts[itemCountAgendaIndex].TotalCount;
			desiredAdvancedItems = MyItemCounts[itemCountAgendaIndex].AdvancedCount;
		}

		// determine types that have been introduced
		LinkedList<JuggleObjectType> allPossibleTypes = GameControl.GetGameControl().GetPossibleTypeList();
		int index = 0;
		foreach (JuggleObjectType itemType in allPossibleTypes)
		{
			if ((inBookmark.GameState.TypesIntroduced & (1 << index)) != 0)
			{
				if (!allAvailableTypes.Contains(itemType))
				{
					allAvailableTypes.AddLast(itemType);
				}
				if (itemType.IsAdvancedItem)
				{
					if (!availableAdvancedTypes.Contains(itemType))
					{
						availableAdvancedTypes.Add(itemType);
					}
				}
				else
				{
					if (!availableBasicTypes.Contains(itemType))
					{
						availableBasicTypes.Add(itemType);
					}
				}
				// print("Added introduced type: " + itemType.SingularName);
			}
			index++;
		}

		RecreateItemsInPlay(inBookmark);

		theJuggler.RecreateFromBookmark(inBookmark);
		JuggleStatKeeper.GetStatKeeper().RecreateFromBookmark(inBookmark);

		Bookmark.ClearBookmark();
		Pauser.GetPauser().BeginPause(true);
	}

	void RecreateItemsInPlay(Bookmark inBookmark)
	{
		if (inBookmark.GameState.ItemsInPlay != null)
		{
			Dictionary<ItemStateData, JuggleObject> recreatedItems = new Dictionary<ItemStateData, JuggleObject>(inBookmark.GameState.ItemsInPlay.Length);

			// Recreate each item
			foreach (ItemStateData itemState in inBookmark.GameState.ItemsInPlay)
			{
				JuggleObjectType itemType = Bookmark.GetItemTypeFromLabel(itemState.TypeLabel);
				JuggleObject newItem = itemType.RecreateItemFromSave(itemState);
				newItem.DepthIndex = depthCounter;
				depthCounter++;
				if (depthCounter > MaxDepthCounter)
				{
					depthCounter = 0;
				}

				newItem.Initialize(this, itemType);
				newItem.RecreateFromSaveData(itemState, theJuggler);

				createdJuggleObjects.AddLast(newItem);
				ObjectsInPlay.AddLast(newItem);
				NumObjectsInPlay++;

				if (itemType.IsAdvancedItem)
				{
					advancedItemsInPlay++;
				}

				if (!InactiveJuggleObjects.ContainsKey(newItem.MyType))
				{
					InactiveJuggleObjects.Add(newItem.MyType, new LinkedList<JuggleObject>());
				}

				recreatedItems.Add(itemState, newItem);
			}

			// After all items have been recreated, let each do a post-recreate step
			foreach (ItemStateData itemState in recreatedItems.Keys)
			{
				recreatedItems[itemState].PostRecreate(itemState);
			}
		}
	}

	#if UNITY_EDITOR

	void CheckChangeStage()
	{
		if (Input.GetAxis("Horizontal") > 0)
		{
			if (!pressedLeftOrRight)
			{
				AdvanceStage();
				pressedLeftOrRight = true;
			}
		}
		else
		{
			pressedLeftOrRight = false;
		}
	}

	void OnGUI()
	{
		if (GodMode)
		{
			GUI.color = new Color(0.05f, 0.05f, 0.05f);
			GUI.Label(new Rect(0, 0, 1000, 24), "God Mode");

			// Stage number
			GUI.Label(new Rect(0, 24, 1000, 24), "Current stage: " + currentStage);

			// Desired total and advanced item count
			GUI.Label(new Rect(0, 48, 1000, 24), "Target items: " + desiredObjectsInPlay);
			GUI.Label(new Rect(0, 72, 1000, 24), "Target adv. items: " + desiredAdvancedItems);

			// Type intro info
			if (introducedType != null)
			{
				GUI.Label(new Rect(0, 96, 1000, 24), "Introducing: " + introducedType.SingularName);
			}
			else if (lastIntroducedType != null && WasItemIntroducedLastStage())
			{
				GUI.Label(new Rect(0, 96, 1000, 24), "Last introduced: " + lastIntroducedType.SingularName);
			}

			// Countdown to next introduction
			GUI.Label(new Rect(0, 120, 1000, 24), "Countdown to intro: " + countdownToNextIntroduction);

			// Item count agenda index
			GUI.Label(new Rect(0, 144, 1000, 24), "Item count agenda index: " + itemCountAgendaIndex);

			// Item counts
			GUI.Label(new Rect(0, 168, 1000, 24), "Num. Basic Items: " + (NumObjectsInPlay - advancedItemsInPlay).ToString());
			GUI.Label(new Rect(0, 192, 1000, 24), "Num. Advanced Items: " + advancedItemsInPlay);

			GUI.Label(new Rect(0, 216, 1000, 24), "Items Juggled: " + JuggleStatKeeper.GetStatKeeper().NumJuggledObjects);
		}
	}

	#endif
}
