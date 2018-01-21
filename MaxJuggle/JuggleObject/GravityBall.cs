// Programmer: Carl Childers
// Date: 10/18/2017
//
// Juggle item that raises its gravity scale and changes color when clicked on.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBall : JuggleObject {

	public float StartGravityScale = 1;
	public float FinalGravityScale = 2;
	public Sprite[] ProgressSprites;
	//public Color FinalColor = Color.white;
	//public string ColorChangeSpriteName = "BallSprite";

	//Color startColor;
	//SpriteRenderer colorChangeSprite;

	SpriteRenderer mySprite;


	public override void Initialize(Tosser inTosser, JuggleObjectType inType)
	{
		base.Initialize(inTosser, inType);

		mySprite = GetComponent<SpriteRenderer>();

		//colorChangeSprite = transform.Find(ColorChangeSpriteName).GetComponent<SpriteRenderer>();
		//startColor = colorChangeSprite.color;
	}

	public override void Restart(Vector2 startPos, bool wasTossed = true)
	{
		base.Restart(startPos, wasTossed);

		//colorChangeSprite.color = startColor;
		if (ProgressSprites != null && ProgressSprites.Length > 0)
		{
			mySprite.sprite = ProgressSprites[0];
			if (myArrow != null)
			{
				myArrow.ChangeSprite(ProgressSprites[0]);
			}
		}
		MyRigidbody.gravityScale = StartGravityScale;
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
		//colorChangeSprite.color = Color.Lerp(startColor, FinalColor, progress);
		if (ProgressSprites != null && ProgressSprites.Length > 0)
		{
			Sprite newSprite = ProgressSprites[Mathf.FloorToInt(Mathf.Lerp(0, ProgressSprites.Length - 1, progress))];
			mySprite.sprite = newSprite;
			if (myArrow != null)
			{
				myArrow.ChangeSprite(newSprite);
			}
		}
		MyRigidbody.gravityScale = Mathf.Lerp(StartGravityScale, FinalGravityScale, progress);
	}

	public override Sprite GetStartingOffscreenSprite()
	{
		return mySprite.sprite;
	}

	protected override Sprite GetTrailSprite()
	{
		return mySprite.sprite;
	}

	protected override void UpdateTrailObject(Transform inTrailObject, TrailPosition trailPos, int order)
	{
		base.UpdateTrailObject(inTrailObject, trailPos, order);

		inTrailObject.GetComponent<SpriteRenderer>().sprite = mySprite.sprite;
	}
}
