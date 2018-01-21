// Programmer: Carl Childers
// Date: 10/20/2017
//
// Gets smaller when clicked.  There is a tie child component that doesn't get smaller and needs to be moved to match up with the main balloon.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBalloon : JuggleObject {

	public Vector2 FinalScale = new Vector2(0.5f, 0.5f);
	public Vector2 FinalTiePos;
	public float StretchAmount = 0.1f;
	public float TouchBounceAmount = 0.2f;
	public float BounceRecedeDuration = 1f;
	public string BalloonName = "Balloon";
	public string TieName = "Tie";
	public string SplashEffectName = "WaterSplash";
	public JuggleObjectType[] Subtypes;					// Allows a random color and remembering that color in the pile
	public SpriteColliderPair[] ProgressData;

	Vector2 startScale;
	Vector2 startTiePos;
	Transform balloonTransform;
	Transform tieTransform;
	SpriteRenderer balloonSprite;
	SpriteRenderer tieSprite;
	ParticleSystem waterSplashComp;
	JuggleObjectType mySubtype;
	SpriteColliderPair currentProgressData;
	Collider2D activeCollider;
	float scaleFactor;
	float targetScaleFactor;
	float bounceCounter;
	float bounceSign = 1;


	public override void Initialize(Tosser inTosser, JuggleObjectType inType)
	{
		base.Initialize(inTosser, Subtypes[0]);

		balloonTransform = transform.Find(BalloonName);
		tieTransform = transform.Find(TieName);
		balloonSprite = balloonTransform.GetComponent<SpriteRenderer>();
		tieSprite = tieTransform.GetComponent<SpriteRenderer>();
		startScale = balloonTransform.localScale;
		startTiePos = tieTransform.localPosition;
		waterSplashComp = transform.Find(SplashEffectName).GetComponent<ParticleSystem>();
	}

	public override void Restart(Vector2 startPos, bool wasTossed = true)
	{
		base.Restart(startPos, wasTossed);

		targetScaleFactor = 0;
		scaleFactor = 0;
		balloonTransform.localScale = new Vector3(startScale.x, startScale.y);
		tieTransform.localPosition = new Vector3(startTiePos.x, startTiePos.y);

		if (Subtypes != null && Subtypes.Length > 0)
		{
			mySubtype = Subtypes[Random.Range(0, Subtypes.Length)];
			balloonSprite.color = mySubtype.ItemColor;
			tieTransform.GetComponent<SpriteRenderer>().color = mySubtype.ItemColor;

			if (myArrow != null)
			{
				myArrow.ChangeSpriteColor(mySubtype.ItemColor);
			}
		}

		if (ProgressData != null && ProgressData.Length > 0)
		{
			currentProgressData = ProgressData[0];
			UpdateSpriteAndCollision();
		}

		activeCollider.transform.localScale = balloonTransform.localScale;
	}

	public override void WasJuggled(Juggler inJuggler, Vector2 hitPoint)
	{
		base.WasJuggled(inJuggler, hitPoint);

		if (FullyJuggled || Failed)
			return;

		float progress = 0;
		float maxProgress = GetJuggleLimit(inJuggler) - 1f;
		if (maxProgress > 0)
		{
			progress = JuggleCount / maxProgress;
		}

		targetScaleFactor = progress;
		bounceCounter = 2 * Mathf.PI;

		// Check if it was hit on the side
		float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
		if (Mathf.Abs(Mathf.Cos(angle)) < 0.5f)
		{
			// Hit on the side
			bounceSign = -1;
		}
		else
		{
			// Hit on the top / bottom
			bounceSign = 1;
		}

		// Update balloon sprite and collision
		SpriteColliderPair newCurrentProgressData = GetCurrentProgressData(progress);
		if (newCurrentProgressData != currentProgressData && newCurrentProgressData != null)
		{
			currentProgressData = newCurrentProgressData;
			UpdateSpriteAndCollision();
		}
		activeCollider.transform.localScale = balloonTransform.localScale;

		waterSplashComp.Play();
	}

	// Returns the appropriate sprite for a progress from 0 to 1
	SpriteColliderPair GetCurrentProgressData(float inProgress)
	{
		if (ProgressData == null || ProgressData.Length == 0)
			return null;

		int index = Mathf.RoundToInt((ProgressData.Length - 1) * inProgress);
		if (index >= 0 && index < ProgressData.Length)
		{
			return ProgressData[index];
		}
		else
		{
			return ProgressData[0];
		}
	}

	void UpdateSpriteAndCollision()
	{
		balloonSprite.sprite = currentProgressData.Sprite;
		if (activeCollider != null)
		{
			activeCollider.gameObject.SetActive(false);
		}
		activeCollider = transform.Find(currentProgressData.ColliderName).GetComponent<Collider2D>();
		if (activeCollider != null)
		{
			activeCollider.gameObject.SetActive(true);
		}
		if (myArrow != null)
		{
			myArrow.ChangeSprite(currentProgressData.OffscreenSprite);
		}
	}

	public override Sprite GetStartingOffscreenSprite()
	{
		if (currentProgressData != null)
		{
			return currentProgressData.OffscreenSprite;
		}
		else
		{
			return MyType.OffScreenSprite;
		}
	}

	public override Color GetStartingOffscreenColor()
	{
		if (mySubtype != null)
		{
			return mySubtype.ItemColor;
		}
		else
		{
			return Color.white;
		}
	}

	protected override Transform CreateTrailObject(int order)
	{
		// Create an empty parent object with the balloon and tie as seperate sprite children
		GameObject parentObj = new GameObject("Trail Object");
		Transform trailTransform = parentObj.transform;
		trailTransform.SetParent(transform);
		trailTransform.localScale = Vector3.one;

		// Balloon
		GameObject balloonObj = new GameObject("Trail Balloon");
		balloonObj.transform.SetParent(trailTransform);
		balloonObj.transform.localScale = Vector3.one;
		balloonObj.transform.localPosition = Vector3.zero;
		balloonObj.transform.localRotation = Quaternion.identity;
		SpriteRenderer trailBalloonSprite = balloonObj.AddComponent<SpriteRenderer>();
		trailBalloonSprite.sprite = balloonSprite.sprite;
		trailBalloonSprite.sortingLayerName = GetTrailSortingLayer();
		trailBalloonSprite.sortingOrder = -100 - 10 * order;

		// Tie
		GameObject tieObj = new GameObject("Trail Tie");
		tieObj.transform.SetParent(trailTransform);
		tieObj.transform.localScale = Vector3.one;
		tieObj.transform.localPosition = Vector3.zero;
		tieObj.transform.localRotation = Quaternion.identity;
		SpriteRenderer trailTieSprite = tieObj.AddComponent<SpriteRenderer>();
		trailTieSprite.sprite = tieSprite.sprite;
		trailTieSprite.sortingLayerName = GetTrailSortingLayer();
		trailTieSprite.sortingOrder = -100 - 10 * order + 1;			// Stay in front of the balloon sprite

		return trailTransform;
	}

	protected override void UpdateTrailObject(Transform inTrailObject, TrailPosition trailPos, int order)
	{
		base.UpdateTrailObject(inTrailObject, trailPos, order);

		Color trailColor = GetTrailColor();
		if (order > 0)
		{
			trailColor.a *= TrailAlpha / order;
		}

		SpriteRenderer trailBalloonSprite = inTrailObject.Find("Trail Balloon").GetComponent<SpriteRenderer>();
		if (trailBalloonSprite != null)
		{
			trailBalloonSprite.sprite = balloonSprite.sprite;
			trailBalloonSprite.color = trailColor;
			trailBalloonSprite.transform.localScale = balloonTransform.localScale;
		}

		SpriteRenderer trailTieSprite = inTrailObject.Find("Trail Tie").GetComponent<SpriteRenderer>();
		if (trailTieSprite != null)
		{
			trailTieSprite.color = trailColor;
			trailTieSprite.transform.localPosition = tieTransform.localPosition;
		}
	}

	protected override Color GetTrailColor()
	{
		if (mySubtype != null)
		{
			return mySubtype.ItemColor;
		}
		else
		{
			return Color.white;
		}
	}

	public override JuggleObjectType GetSpecificJuggledType()
	{
		if (mySubtype != null)
		{
			return mySubtype;
		}
		else
		{
			return MyType;
		}
	}

	void UpdateScale()
	{
		float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
		float stretchFactor = Mathf.Cos(angle);

		scaleFactor = targetScaleFactor;

		Vector2 balloonScale = Vector2.Lerp(startScale, FinalScale, scaleFactor);
		float bounceAmount = TouchBounceAmount * (bounceCounter / (2 * Mathf.PI));
		balloonScale.y += (stretchFactor * StretchAmount) + (bounceSign * Mathf.Sin(bounceCounter) * bounceAmount);
		balloonScale.x -= (stretchFactor * StretchAmount) + (bounceSign * Mathf.Sin(bounceCounter) * bounceAmount);
		balloonTransform.localScale = balloonScale;

		float tieAlpha = 0;
		if (startScale.y != FinalScale.y)
		{
			tieAlpha = (balloonScale.y - startScale.y) / (FinalScale.y - startScale.y);
		}

		// Interpolate the tie position based on balloon scale, without clamping tie alpha
		tieTransform.localPosition = startTiePos + (FinalTiePos - startTiePos) * tieAlpha;
	}

	void UpdateBounce()
	{
		if (bounceCounter > 0)
		{
			if (BounceRecedeDuration > 0)
			{
				bounceCounter = Mathf.Max(bounceCounter - (2 * Mathf.PI) * (Time.deltaTime / BounceRecedeDuration), 0);
			}
			else
			{
				bounceCounter = 0;
			}
		}
	}

	protected override void UpdateJuggleObject()
	{
		UpdateScale();
		UpdateBounce();
	}

	public override ItemStateData GetStateData()
	{
		ItemStateData myState = base.GetStateData();

		myState.TypeLabel = mySubtype.BookmarkLabel;
		return myState;
	}

	public override void RecreateFromSaveData(ItemStateData inStateData, Juggler inJuggler)
	{
		base.RecreateFromSaveData(inStateData, inJuggler);

		mySubtype = Bookmark.GetItemTypeFromLabel(inStateData.TypeLabel);
		balloonSprite.color = mySubtype.ItemColor;
		tieTransform.GetComponent<SpriteRenderer>().color = mySubtype.ItemColor;

		// Set size based on juggle count
		float progress = 0;
		float maxProgress = GetJuggleLimit(inJuggler) - 1f;
		if (maxProgress > 0)
		{
			progress = JuggleCount / maxProgress;
		}

		targetScaleFactor = progress;
		scaleFactor = targetScaleFactor;

		// Update balloon sprite and collision
		SpriteColliderPair newCurrentProgressData = GetCurrentProgressData(progress);
		if (newCurrentProgressData != currentProgressData && newCurrentProgressData != null)
		{
			currentProgressData = newCurrentProgressData;
			UpdateSpriteAndCollision();
		}
		activeCollider.transform.localScale = balloonTransform.localScale;

		UpdateScale();
	}
}

[System.Serializable]
public class SpriteColliderPair {

	public Sprite Sprite;
	public Sprite OffscreenSprite;
	public string ColliderName;
}