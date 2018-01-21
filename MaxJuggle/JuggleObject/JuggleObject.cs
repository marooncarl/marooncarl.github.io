// Programmer: Carl Childers
// Date: 9/7/2017
//
// An object that can be juggled.  Can have colliders that are jugglable and hurtful.
// When it is hit a certain number of times, it is fully juggled and goes flying off the screen.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JuggleObject : MonoBehaviour {

	Tosser myTosser;
	public Tosser MyTosser {
		get { return myTosser; }
	}

	JuggleObjectType myType;
	public JuggleObjectType MyType {
		get { return myType; }
	}

	Rigidbody2D myRigidbody;
	public Rigidbody2D MyRigidbody {
		get { return myRigidbody; }
	}

	protected OffScreenArrow myArrow;
	ParticleSystem myNearFullEffect;

	// How many times this object has been clicked since last restarting.
	int juggleCount;
	public int JuggleCount {
		get { return juggleCount; }
	}

	protected bool fullyJuggled;
	public bool FullyJuggled {
		get { return fullyJuggled; }
	}

	// An object is failed if the player hits a dangerous spot
	bool failed;
	public bool Failed {
		get { return failed; }
		set { failed = value; }
	}

	bool inPlay;
	public bool InPlay { 
		get { return inPlay; }
	}

	int depthIndex;
	public int DepthIndex {
		get { return depthIndex; }
		set { depthIndex = value; }
	}

	// ID is used so, when saving, juggle objects can reference each other.
	byte myID;
	public byte ID {
		get { return myID; }
		set { myID = value; }
	}

	const int DepthStep = 10;

	float immuneCounter;
	float disableCounter;

	List<TrailPosition> trailPositions;
	const int NumTrailPositions = 12;				// Increasing this can make the trail cover a longer distance.
	const int TrailSpacing = 4;						// NumTrailPositions / TrailSpacing == number of trail objects shown
	protected const float TrailAlpha = 0.5f;

	LinkedList<Transform> trailObjects;				// Trail objects can be a single sprite or a hierarchy containing multiple sprites.

	AudioSource myAudioSource;
	public AudioSource MyAudioSource {
		get { return myAudioSource; }
	}

	protected bool hasDropped;


	public virtual void Initialize(Tosser inTosser, JuggleObjectType inType)
	{
		myTosser = inTosser;
		myType = inType;
		myRigidbody = GetComponent<Rigidbody2D>();
		myAudioSource = GetComponent<AudioSource>();

		// Adjust sorting orders of attached sprites and particle systems based on assigned depth index
		SpriteRenderer[] mySprites = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in mySprites)
		{
			sprite.sortingOrder += depthIndex * DepthStep;
		}

		ParticleSystemRenderer[] myParticleRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
		foreach (ParticleSystemRenderer psr in myParticleRenderers)
		{
			psr.sortingOrder += depthIndex * DepthStep;
		}
	}

	public virtual void Restart(Vector2 startPos, bool wasTossed = true)
	{
		gameObject.SetActive(true);
		juggleCount = 0;
		fullyJuggled = false;
		failed = false;
		transform.position = new Vector3(startPos.x, startPos.y);
		transform.rotation = Quaternion.Euler(0, 0, Random.value * 360f);
		myRigidbody.velocity = Vector2.zero;
		inPlay = true;
		disableCounter = 0;
		hasDropped = false;

		BecomeImmune();
		StopNearFullEffect();

		if (myArrow != null)
		{
			myArrow.gameObject.SetActive(false);
		}

		if (wasTossed && myType.TossSound != null)
		{
			myAudioSource.PlayOneShot(myType.TossSound, myType.TossSoundVolume);
		}

		if (trailPositions == null)
		{
			trailPositions = new List<TrailPosition>(NumTrailPositions);
		}
		else
		{
			trailPositions.Clear();
		}
	}

	public virtual void Destroyed()
	{
	}

	void Update()
	{
		if (fullyJuggled)
		{
			// Check if risen above the screen, or fallen below
			if (myRigidbody.position.y > myType.TopYPosition || myRigidbody.position.y < myType.BaseYPosition)
			{
				// Wait to disable so particles can finish
				if (disableCounter == 0)
				{
					disableCounter = myType.FullJuggleDisableDelay;
					StopNearFullEffect();
				}
				else
				{
					disableCounter -= Time.deltaTime;
					if (disableCounter <= 0)
					{
						Disable();
					}
				}
			}
		}
		else
		{
			// Check if the object has fallen
			if (myRigidbody.position.y < myType.BaseYPosition)
			{
				Dropped();
			}
		}

		// Update immune period
		if (immuneCounter > 0)
		{
			immuneCounter -= Time.deltaTime;
		}

		UpdateJuggleObject();

		if (!fullyJuggled && !failed)
		{
			UpdateOffScreenArrow();
		}
	}
		
	void LateUpdate()
	{
		UpdateTrail();
	}

	// Subclass for custom update behaviour
	protected virtual void UpdateJuggleObject()
	{
		
	}

	void UpdateTrail()
	{
		if (Time.timeScale > 0)
		{
			Vector2 newPos = new Vector2(transform.position.x, transform.position.y);
			if (trailPositions.Count < 1 || newPos != trailPositions[0].position || transform.rotation.eulerAngles.z != trailPositions[0].rotation)
			{
				if (trailPositions.Count >= NumTrailPositions)
				{
					trailPositions.RemoveAt(trailPositions.Count - 1);
				}
				trailPositions.Insert(0, new TrailPosition(newPos, transform.rotation.eulerAngles.z));
			}
		}
	}

	void UpdateOffScreenArrow()
	{
		if (transform.position.y > MyType.OffscreenYPosition)
		{
			// Create or restart the off screen arrow
			if (myArrow == null)
			{
				myArrow = Instantiate(myTosser.ArrowPrefab, transform.position, Quaternion.identity);
				myArrow.transform.SetParent(transform);
				myArrow.Restart();
			}
			else if (!myArrow.gameObject.activeSelf)
			{
				myArrow.gameObject.SetActive(true);
				myArrow.Restart();
			}
		}
		else if (transform.position.y < MyType.OffscreenYPosition)
		{
			if (myArrow != null && myArrow.gameObject.activeSelf)
			{
				myArrow.gameObject.SetActive(false);
			}
		}
	}

	// Allows child classes to start with a different offscreen sprite.
	public virtual Sprite GetStartingOffscreenSprite()
	{
		return myType.OffScreenSprite;
	}

	public virtual Color GetStartingOffscreenColor()
	{
		return Color.white;
	}

	protected virtual void Dropped()
	{
		if (hasDropped)
			return;

		hasDropped = true;
		Invoke("Disable", myType.DropDisableDelay);
		RemoveFromPlay();
		if (!failed)
		{
			myTosser.ObjectDropped(this);
		}
	}

	// The player clicked on this object, and did NOT hit a dangerous spot
	public virtual void WasJuggled(Juggler inJuggler, Vector2 hitPoint)
	{
		if (fullyJuggled || failed)
		{
			return;
		}

		juggleCount++;
		if (!fullyJuggled && juggleCount >= GetJuggleLimit(inJuggler))
		{
			fullyJuggled = true;
			RemoveFromPlay();

			inJuggler.FullyJuggledObject(GetGeneralJuggledType(), GetSpecificJuggledType());
		}
		else if (juggleCount == GetJuggleLimit(inJuggler) - 1)
		{
			// Almost fully juggled
			StartNearFullEffect();
		}

		float juggleForce = (fullyJuggled ? inJuggler.FullJuggleForce : inJuggler.JuggleForce);
		ApplyForce(hitPoint, juggleForce);

		if (!fullyJuggled)
		{
			inJuggler.PlayJuggleEffect(hitPoint);
		}
		else
		{
			inJuggler.PlayFullJuggleEffect(hitPoint);
		}

		BecomeImmune();

		if (MyType.TossSound != null)
		{
			myAudioSource.PlayOneShot(MyType.TossSound, MyType.JuggleTossVolume);
		}
	}

	// Used when the juggler gets hurt touching this object
	public virtual void ForcefulDrop(Juggler inJuggler, Vector2 hitPoint)
	{
		Vector2 juggleCenter = GetJuggleCenter();
		Vector2 adjustedOffset = new Vector2(Mathf.Clamp(hitPoint.x - juggleCenter.x, myType.JuggleMinXOffset, myType.JuggleMaxXOffset), myType.JuggleYOffset);
		Vector2 hitDirection = (new Vector2(-adjustedOffset.x, -1)).normalized;
		myRigidbody.velocity = Vector2.zero;
		myRigidbody.angularVelocity = 0;
		myRigidbody.AddForceAtPosition(hitDirection * inJuggler.JuggleForce, juggleCenter + adjustedOffset);
	}

	public void ApplyForce(Vector2 hitPoint, float inForce)
	{
		Vector2 juggleCenter = GetJuggleCenter();
		Vector2 adjustedOffset = new Vector2(Mathf.Clamp(hitPoint.x - juggleCenter.x, myType.JuggleMinXOffset, myType.JuggleMaxXOffset), myType.JuggleYOffset);
		Vector2 hitDirection = (new Vector2(-adjustedOffset.x, 1)).normalized;
		myRigidbody.velocity = Vector2.zero;
		myRigidbody.angularVelocity = 0;
		myRigidbody.AddForceAtPosition(hitDirection * inForce, juggleCenter + adjustedOffset);
	}

	protected void Disable()
	{
		gameObject.SetActive(false);
		myTosser.InactiveJuggleObjects[myType].AddLast(this);
	}

	public void RemoveFromPlay()
	{
		if (inPlay)
		{
			myTosser.ObjectsInPlay.Remove(this);
			myTosser.NumObjectsInPlay--;
			myTosser.ItemRemovedFromPlay(this);
			inPlay = false;
		}
	}

	public Vector2 GetJuggleCenter()
	{
		Vector3 rotatedOffset = Quaternion.Euler(0, 0, myRigidbody.rotation) * new Vector3(myType.JuggleCenter.x, myType.JuggleCenter.y);
		return myRigidbody.position + new Vector2(rotatedOffset.x, rotatedOffset.y);
	}

	void OnTriggerStay2D(Collider2D other)
	{
		// Hit a wall, so do a simple bounce
		if ((other.transform.position.x > 0 && myRigidbody.velocity.x > 0) ||
				(other.transform.position.x < 0 && myRigidbody.velocity.x < 0))
		{
			Vector2 newVelocity = myRigidbody.velocity;
			newVelocity.x *= -1;
			myRigidbody.velocity = newVelocity;
		}
	}

	public void BecomeImmune()
	{
		immuneCounter = myTosser.JuggleImmunePeriod;
	}

	public bool IsImmune()
	{
		return (immuneCounter > 0);
	}

	protected void StartNearFullEffect()
	{
		if (myNearFullEffect != null)
		{
			if (myNearFullEffect.isPlaying)
			{
				myNearFullEffect.Stop();
			}
			myNearFullEffect.Play();
		}
		else
		{
			myNearFullEffect = Instantiate(myType.NearFullEffect, transform.position, Quaternion.identity);
			myNearFullEffect.transform.SetParent(transform);
			myNearFullEffect.transform.localRotation = Quaternion.identity;
			if (!myNearFullEffect.isPlaying)
			{
				myNearFullEffect.Play();
			}
		}
	}

	void StopNearFullEffect()
	{
		if (myNearFullEffect != null && myNearFullEffect.isPlaying)
		{
			myNearFullEffect.Stop();
		}
	}

	// Returns the number of times this item needs to be juggled.
	// Allows a subclass to use a custom amount.
	protected virtual int GetJuggleLimit(Juggler inJuggler)
	{
		return inJuggler.FullJuggleThreshold;
	}

	public void ShowTrail()
	{
		// Test: Make sure the trail positions are different from the current position
		/*
		print("Current position: " + transform.position + "  rotation: " + transform.rotation.eulerAngles.z);
		int i = 0;
		foreach (TrailPosition trailPos in trailPositions)
		{
			print("Trail position " + i + ": " + trailPos.position + "  rotation: " + trailPos.rotation);
			i++;
		}
		*/

		LinkedListNode<Transform> currentNode = null;
		if (trailObjects == null)
		{
			trailObjects = new LinkedList<Transform>();
		}

		int trailCounter = 1;
		for (int trailIndex = TrailSpacing - 1; trailIndex < trailPositions.Count; trailIndex += TrailSpacing)
		{
			// Go to the next node, or start at the first one
			if (currentNode == null)
			{
				currentNode = trailObjects.First;
			}
			else
			{
				currentNode = currentNode.Next;
			}

			// If there is no node here, create a new sprite
			if (currentNode == null)
			{
				Transform newTrailObject = CreateTrailObject(trailCounter);
				trailObjects.AddLast(newTrailObject);
				currentNode = trailObjects.Last;
			}

			UpdateTrailObject(currentNode.Value, trailPositions[trailIndex], trailCounter);

			trailCounter++;
		}

		// Make unused trail objects inactive
		if (currentNode != null)
		{
			while (currentNode.Next != null)
			{
				currentNode = currentNode.Next;
				currentNode.Value.gameObject.SetActive(false);
			}
		}
	}

	protected virtual Transform CreateTrailObject(int order)
	{
		GameObject gameObj = new GameObject("Trail Image");
		Transform trailTransform = gameObj.transform;
		SpriteRenderer trailSprite = gameObj.AddComponent<SpriteRenderer>();
		trailSprite.sprite = GetTrailSprite();

		Color trailColor = GetTrailColor();
		if (order > 0)
		{
			trailColor.a *= TrailAlpha / order;
		}
		trailSprite.color = trailColor;
		trailSprite.sortingLayerName = GetTrailSortingLayer();
		trailSprite.sortingOrder = -100 - 10 * order;

		trailTransform.SetParent(transform);
		trailTransform.localScale = Vector3.one;

		return trailTransform;
	}

	protected virtual void UpdateTrailObject(Transform inTrailObject, TrailPosition trailPos, int order)
	{
		inTrailObject.gameObject.SetActive(true);
		inTrailObject.position = new Vector3(trailPos.position.x, trailPos.position.y);
		inTrailObject.rotation = Quaternion.Euler(0, 0, trailPos.rotation);
	}

	public void HideTrail()
	{
		foreach (Transform trailObj in trailObjects)
		{
			trailObj.gameObject.SetActive(false);
		}
	}

	protected virtual Sprite GetTrailSprite()
	{
		return myType.TrailSprite;
	}

	protected virtual Color GetTrailColor()
	{
		return Color.white;
	}

	protected virtual string GetTrailSortingLayer()
	{
		SpriteRenderer mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
		if (mySpriteRenderer != null)
		{
			return mySpriteRenderer.sortingLayerName;
		}
		else
		{
			return "";
		}
	}

	public virtual JuggleObjectType GetGeneralJuggledType()
	{
		return myType;
	}

	// Returns the type that should be considered as juggled when this object is juggled.
	// Usually just the item's type, but can be a specific subtype, for example the same type of object with different color varieties.
	public virtual JuggleObjectType GetSpecificJuggledType()
	{
		return myType;
	}

	// Returns data about the item that needs to be saved for the next play session.
	public virtual ItemStateData GetStateData()
	{
		return new ItemStateData(myID, myType, myRigidbody.position, myRigidbody.velocity, myRigidbody.rotation, myRigidbody.angularVelocity,
			(byte)juggleCount, immuneCounter);
	}

	public virtual void RecreateFromSaveData(ItemStateData inStateData, Juggler inJuggler)
	{
		juggleCount = inStateData.JuggleCount;
		immuneCounter = inStateData.ImmuneCounter;
		inPlay = true;
		myID = inStateData.ID;

		if (trailPositions == null)
		{
			trailPositions = new List<TrailPosition>(NumTrailPositions);
		}
		else
		{
			trailPositions.Clear();
		}
		// Determine trail from saved velocity and rotation rate
		for (int i = 0; i < NumTrailPositions - 1; ++i)
		{
			Vector2 trailPos = new Vector2(inStateData.XPosition, inStateData.YPosition) - new Vector2(inStateData.XVelocity, inStateData.YVelocity)
				* (i * (1f/60f));
			float trailRot = inStateData.Rotation - inStateData.RotationRate * (i * (1f/60f));
			trailPositions.Add(new TrailPosition(trailPos, trailRot));
		}

		if (juggleCount == GetJuggleLimit(inJuggler) - 1)
		{
			// Almost fully juggled
			StartNearFullEffect();
		}

		// Pre-warm particles
		ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem ps in particles)
		{
			if (ps.isPlaying)
			{
				ps.Simulate(1.0f);
				ps.Play();
			}
		}
	}

	// Used for items that need to recreate references to other items.
	public virtual void PostRecreate(ItemStateData inStateData)
	{
	}
}

public class TrailPosition
{
	public Vector2 position;
	public float rotation;

	public TrailPosition(Vector2 inPos, float inRot)
	{
		position = inPos;
		rotation = inRot;
	}
}
