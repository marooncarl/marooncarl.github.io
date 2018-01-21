/// <summary>
/// Sprite change
/// 
/// Programmer: Carl Childers
/// Date: 4/18/2015
/// 
/// Basic sprite change triggered by some event.
/// </summary>

using UnityEngine;
using System.Collections;

public class SpriteChange : MonoBehaviour {

	public Texture2D AltTexture;

	Sprite AltSprite, OriginalSprite;

	void Awake()
	{
		SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		if (spriteRenderer != null) {
			OriginalSprite = spriteRenderer.sprite;
			AltSprite = Sprite.Create (AltTexture, new Rect(0, 0, AltTexture.width, AltTexture.height), new Vector2(0.5f, 0.5f));
		}
	}

	void ChangeTexture()
	{
		SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		if (spriteRenderer != null) {
			spriteRenderer.sprite = AltSprite;
		}
	}

	void RevertTexture()
	{
		SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		if (spriteRenderer != null) {
			spriteRenderer.sprite = OriginalSprite;
		}
	}
}
