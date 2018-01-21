// Programmer: Carl Childers
// Date: 9/12/2017
//
// Base class for juggle item tossers.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tosser : MonoBehaviour {

	public int MaxObjectsInPlay = 3;
	public float TossForce = 10;
	public float StartupWait = 1;
	public float RefillWait = 1;
	public float TimeBetweenTosses = 1;
	public float JuggleImmunePeriod = 0.25f;					// When touched, juggle objects become immune to touching for this period
	public float JuggleHorizontalScale = 1;						// Setting to a value lower than 1 reduces max horizontal starting speed for juggle objects

	public OffScreenArrow ArrowPrefab;

	public JuggleItemList PossibleTypes;

	Dictionary<JuggleObjectType, LinkedList<JuggleObject> > inactiveJuggleObjects;
	public Dictionary<JuggleObjectType, LinkedList<JuggleObject> > InactiveJuggleObjects {
		get { return inactiveJuggleObjects; }
	}

	protected LinkedList<JuggleObject> createdJuggleObjects;
	LinkedList<JuggleObject> objectsInPlay;
	public LinkedList<JuggleObject> ObjectsInPlay {
		get { return objectsInPlay; }
	}

	int numObjectsInPlay;
	public int NumObjectsInPlay {
		get { return numObjectsInPlay; }
		set { numObjectsInPlay = value; }
	}

	protected int desiredObjectsInPlay;
	public int DesiredObjectsInPlay { 
		get { return desiredObjectsInPlay; }
	}

	// Used to assign depth indices to tossed object, controlling which ones are in front
	protected int depthCounter;
	public int DepthCounter {
		get { return depthCounter; }
	}

	protected float tossCounter;
	public float TossCounter {
		get { return tossCounter; }
	}

	bool exitingGame;

	byte nextID;

	protected const int MaxDepthCounter = 1000;


	void Start()
	{
		Restart();
	}

	public virtual void Restart()
	{
		if (inactiveJuggleObjects == null)
		{
			inactiveJuggleObjects = new Dictionary<JuggleObjectType, LinkedList<JuggleObject>>();
		}
		if (createdJuggleObjects == null)
		{
			createdJuggleObjects = new LinkedList<JuggleObject>();
		}
		if (objectsInPlay == null)
		{
			objectsInPlay = new LinkedList<JuggleObject>();
		}
		else
		{
			objectsInPlay.Clear();
		}

		tossCounter = StartupWait;
		numObjectsInPlay = 0;
		desiredObjectsInPlay = 1;
		nextID = 0;
	}

	void OnApplicationQuit()
	{
		exitingGame = true;
	}

	void OnDestroy()
	{
		if (exitingGame)
			return;

		//print("Tosser was destroyed");

		// Destroy juggle objects created in this game session
		foreach (JuggleObject obj in createdJuggleObjects)
		{
			Destroy(obj.gameObject);
			obj.Destroyed();
		}
		createdJuggleObjects.Clear();
		inactiveJuggleObjects.Clear();
	}

	protected void CleanUnusedObjects()
	{
		LinkedList<JuggleObject> copyList = new LinkedList<JuggleObject>(createdJuggleObjects);

		foreach (JuggleObject obj in copyList)
		{
			if (!obj.gameObject.activeInHierarchy)
			{
				JuggleObjectType objType = obj.MyType;
				Destroy(obj.gameObject);
				obj.Destroyed();
				createdJuggleObjects.Remove(obj);
				if (inactiveJuggleObjects.ContainsKey(objType))
				{
					inactiveJuggleObjects[objType].Remove(obj);
				}
			}
		}
	}

	void Update()
	{
		CustomUpdate();
	}

	protected virtual void CustomUpdate()
	{
		UpdateTossing();
	}

	protected void UpdateTossing()
	{
		if (numObjectsInPlay < desiredObjectsInPlay)
		{
			if (tossCounter > 0)
			{
				tossCounter -= Time.deltaTime;
			}
			else
			{
				JuggleObjectType pickedType = PickItemType();
				Toss(pickedType);
				if (numObjectsInPlay < desiredObjectsInPlay)
				{
					tossCounter = TimeBetweenTosses;
				}
				else
				{
					tossCounter =  GetRefillWait();		// will need to wait again when there is room
				}
			}
		}
	}

	protected virtual float GetRefillWait()
	{
		return RefillWait;
	}

	protected virtual JuggleObjectType PickItemType()
	{
		return PossibleTypes[ Random.Range(0, PossibleTypes.Length) ];
	}

	// Creates and tosses an object.
	// Setting addTossVelocity to false will skip adding the toss velocity, allowing another object to handle that.
	public virtual JuggleObject Toss(JuggleObjectType inType, bool addTossVelocity = true)
	{
		if (inType == null)
		{
			print("Tosser: Trying to Toss a null type!");
			return null;
		}

		if (!inactiveJuggleObjects.ContainsKey(inType))
		{
			inactiveJuggleObjects.Add(inType, new LinkedList<JuggleObject>());
		}

		JuggleObject tossedObj;
		Vector2 startPos = GetTossPosition(inType);

		if (inactiveJuggleObjects[inType].Count > 0)
		{
			// Reuse a juggle object
			tossedObj = inactiveJuggleObjects[inType].First.Value;
			inactiveJuggleObjects[inType].RemoveFirst();
		}
		else
		{
			// Create a new juggle object
			tossedObj = Instantiate(inType.MyJuggleObject, new Vector3(startPos.x, startPos.y), Quaternion.identity);
			tossedObj.DepthIndex = depthCounter;
			tossedObj.Initialize(this, inType);
			createdJuggleObjects.AddLast(tossedObj);
			depthCounter++;

			if (depthCounter > MaxDepthCounter)
			{
				depthCounter = 0;
			}
		}

		tossedObj.ID = nextID;
		nextID++;

		tossedObj.Restart(startPos, addTossVelocity);

		if (addTossVelocity)
		{
			// Add starting velocity to the tossed object
			Vector2 hitOffset = new Vector2(Random.Range(inType.JuggleMinXOffset, inType.JuggleMaxXOffset) * JuggleHorizontalScale, inType.JuggleYOffset);
			Vector2 hitDirection = (new Vector2(-hitOffset.x, 1)).normalized;
			tossedObj.MyRigidbody.AddForceAtPosition(hitDirection * TossForce, tossedObj.GetJuggleCenter() + hitOffset);
		}
		else
		{
			//print("Tossed " + inType.SingularName + " without toss velocity");
			tossedObj.MyRigidbody.velocity = Vector2.zero;
			tossedObj.MyRigidbody.angularVelocity = 0;
		}

		objectsInPlay.AddLast(tossedObj);
		numObjectsInPlay++;

		return tossedObj;
	}

	protected virtual Vector2 GetTossPosition(JuggleObjectType inType)
	{
		return new Vector2(Random.Range(inType.MinXStart, inType.MaxXStart), inType.BaseYPosition);
	}

	public virtual void ObjectDropped(JuggleObject droppedObj)
	{
	}

	public virtual void ItemRemovedFromPlay(JuggleObject removedObj)
	{
	}
}
