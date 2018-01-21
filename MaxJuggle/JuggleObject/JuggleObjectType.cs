// Programmer: Carl Childers
// Date: 9/7/2017
//
// Contains a juggle object prefab and data about it.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Juggle Object Type")]
public class JuggleObjectType : ScriptableObject {

	public JuggleObject MyJuggleObject;
	public PhysicsPileItem PilePrefab;
	public JuggleObjectType[] OverridePileTypes;	// For splitting items where multiple types should be created for the pile instead of this one.
	public Sprite OffScreenSprite;
	public Sprite TrailSprite;
	public Sprite AvatarSprite;						// Sprite that represents the object using a simple icon.
	public ParticleSystem NearFullEffect;
	public bool UseItemColor = true;
	public Color ItemColor = Color.white;
	public string SingularName;
	public string PluralName;
	public string BookmarkLabel;
	public bool IsAdvancedItem = false;
	public Vector2 JuggleCenter;
	public float BaseYPosition;						// Y position this object is created / restarted at, and below which it is considered having fallen
	public float TopYPosition;						// When fully juggled, becomes inactive above this position
	public float OffscreenYPosition;				// Shows offscreen arrow when above this position.
	public float MinXStart = -5;
	public float MaxXStart = 5;
	public float JuggleYOffset;						// Juggle forces are always applied at this y position relative to the object's position
	public float JuggleMinXOffset = -0.5f;
	public float JuggleMaxXOffset = 0.5f;
	public float FullJuggleDisableDelay = 1f;		// Wait time before disabling so the trail particles can fade out
	public float DropDisableDelay = 1f;				// Wait time before disabling so the particles can fade out

	public float OffScreenScale = 0.25f;

	public float PiledMinRotation = -180;
	public float PiledMaxRotation = 180;

	public float PauseScreenSize = 80;				// Size of the pause screen icon.  Greater of width or height is scaled to this size.
	public float PauseScreenRotation = 0;			// Rotation used on the icon in the pause screen

	public int TitleScreenFrequency = 1;

	public AudioClip TossSound;
	[Range(0, 2)]
	public float TossSoundVolume = 1;
	[Range(0, 2)]
	public float JuggleTossVolume = 0.5f;

	public AudioClip ImpactSound;
	[Range(0, 2)]
	public float ImpactSoundVolume = 1;
	public float ImpactMaxSpeed = 10;				// if collision velocity is >= ImpactMaxVelocity, then the sound will be played at full volume.
	

	public JuggleObject RecreateItemFromSave(ItemStateData inStateData)
	{
		JuggleObject resultObj = Instantiate(MyJuggleObject, new Vector3(inStateData.XPosition, inStateData.YPosition, 0), Quaternion.Euler(0, 0, inStateData.Rotation));
		Rigidbody2D rb = resultObj.GetComponent<Rigidbody2D>();
		if (rb != null)
		{
			rb.velocity = new Vector2(inStateData.XVelocity, inStateData.YVelocity);
			rb.angularVelocity = inStateData.RotationRate;
		}

		return resultObj;
	}
}
