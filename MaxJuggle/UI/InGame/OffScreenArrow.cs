// Programmer: Carl Childers
// Date: 9/15/2017
//
// An arrow that points to an offscreen object.  Lights up when the object is almost on screen again.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenArrow : MonoBehaviour {

	public float YPosition = 4.5f;
	public float MiniSpriteOffset = -1f;
	[Range(0, 1)]
	public float MiniSpriteOpacity = 0.5f;

	SpriteRenderer lightUpSprite;
	JuggleObject myJuggleObject;
	Transform miniMe;
	SpriteRenderer miniSprite;

	Collider2D[] myColliders;


	void Awake()
	{
		lightUpSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
	}

	void Start()
	{
		myJuggleObject = transform.root.GetComponent<JuggleObject>();
		myColliders = myJuggleObject.GetComponentsInChildren<Collider2D>();

		GameObject miniMeObj = new GameObject("Off Screen Sprite");
		miniMe = miniMeObj.transform;
		miniMe.SetParent(transform);
		miniMe.localPosition = new Vector3(0, MiniSpriteOffset);

		miniSprite = miniMeObj.AddComponent<SpriteRenderer>();
		miniSprite.sprite = myJuggleObject.GetStartingOffscreenSprite();
		miniMe.localScale = Vector3.one * myJuggleObject.MyType.OffScreenScale;
		SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
		miniSprite.sortingLayerName = mySprite.sortingLayerName;
		miniSprite.sortingOrder = mySprite.sortingOrder - 1;

		ChangeSpriteColor(myJuggleObject.GetStartingOffscreenColor());
	}

	public void Restart()
	{
		SetOpacity(0);
	}

	void LateUpdate()
	{
		transform.position = new Vector3(GetBoundsCenter().x, YPosition);
		transform.rotation = Quaternion.identity;
		miniMe.rotation = myJuggleObject.transform.rotation;

		if (myJuggleObject != null && myJuggleObject.MyRigidbody.velocity.y < 0)
		{
			float opacity = 1 - Mathf.Clamp(myJuggleObject.transform.position.y - myJuggleObject.MyType.OffscreenYPosition, 0, 1);
			SetOpacity(opacity);
		}
		else
		{
			SetOpacity(0);
		}
	}

	void SetOpacity(float inValue)
	{
		Color newColor = lightUpSprite.color;
		newColor.a = inValue;
		lightUpSprite.color = newColor;
	}

	Vector3 GetBoundsCenter()
	{
		if (myColliders.Length == 0)
		{
			return transform.position;
		}

		Vector3 sum = Vector3.zero;
		foreach (Collider2D coll in myColliders)
		{
			sum += coll.bounds.center;
		}
		sum /= myColliders.Length;
		return sum;
	}

	public void ChangeSprite(Sprite newSprite)
	{
		miniSprite.sprite = newSprite;
	}

	public Sprite GetCurrentSprite()
	{
		return miniSprite.sprite;
	}

	public void ChangeSpriteColor(Color newColor)
	{
		Color actualColor = new Color(newColor.r, newColor.g, newColor.b, newColor.a * MiniSpriteOpacity);
		miniSprite.color = actualColor;
	}
}
