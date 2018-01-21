/// <summary>
/// Weapon Choice
/// 
/// Programmer: Carl Childers
/// Date: 6/6/2015
/// 
/// Displays current weapon on screen, and accepts input for choosing weapons.  The chosen weapon
/// prefab is sent to the player's weapon equip script to actually create the weapon.
/// </summary>


using UnityEngine;
using System.Collections;


[System.Serializable]
public class WeaponInfo : System.Object {
	public Texture2D HUDTexture;
	public Transform WeaponPrefab;
}

public class WeaponChoice : MonoBehaviour {

	public WeaponInfo[] Weapons;
	public Texture2D FrameTexture; // frames the currently selected weapon
	public Vector2 CellScreenPosition = new Vector2(10, 10);
	public Vector2 NextWeaponDirection = new Vector2(0, 1); // should be a normalized vector2, and will be normalized if not
	public Vector2 PrevWeaponDirection = new Vector2(0, -1); // normalized like above
	public Vector2 CellSize = new Vector2(32, 32);
	public bool FromScreenBottom = true;
	public float TransitionTime = 0.25f;
	public float InputBufferTime = 0.1f;
	public Transform WeaponParent; // player's socket for attaching weapons
	public string AxisName = "WeaponAxis";
	public string PrevButtonName = "PrevWeapon";
	public string NextButtonName = "NextWeapon";
	public string[] QuickSelectInput;
	public AudioClip WeaponChangeSound;
	public float SoundVolume = 1.0f;


	int CurrentIndex;
	int LastWeaponIndex; // index of last weapon equipped
	bool IsInTransition;
	int TransitionDelta; // 1 means moving forward, -1 means moving backwards
	float TransitionAlpha; // controls animation position
	int BufferedInputDelta;
	bool AllowWeaponChange;

	
	void Start() {
		AllowWeaponChange = true;
		CurrentIndex = 0;
		LastWeaponIndex = 0;
		IsInTransition = false;
		TransitionDelta = 1;
		TransitionAlpha = TransitionTime;
		BufferedInputDelta = 0;

		NextWeaponDirection = NextWeaponDirection.normalized;
		PrevWeaponDirection = PrevWeaponDirection.normalized;

		if (WeaponParent != null)
		{
			Transform[] weapPrefabs = new Transform[Weapons.Length];
			for (int i = 0; i < Weapons.Length; ++i) {
				weapPrefabs[i] = Weapons[i].WeaponPrefab;
			}

			WeaponParent.SendMessage("InitWeaponTypes", weapPrefabs);
		}
		/*
		else {
			print("No Weapon Parent set for the HUD's Weapon Choice script!");
		}
		*/
	}

	void Update() {
		if (!AllowWeaponChange) {
			return;
		}

		if (!IsInTransition) {
			float axisValue = Input.GetAxis(AxisName);

			// not in transition; can change weapons
			if (axisValue > 0 || Input.GetButtonDown(NextButtonName) || BufferedInputDelta > 0) {
				IsInTransition = true;
				TransitionAlpha = 0;
				TransitionDelta = 1;
				BufferedInputDelta = 0;

				CurrentIndex++;
				if (CurrentIndex >= Weapons.Length) {
					CurrentIndex = 0;
				}

				if (CurrentIndex != LastWeaponIndex) {
					SetWeapon( CurrentIndex );
				}
				LastWeaponIndex = CurrentIndex;
				return;
			} else if (axisValue < 0 || Input.GetButtonDown(PrevButtonName) || BufferedInputDelta < 0) {
				IsInTransition = true;
				TransitionAlpha = 0;
				TransitionDelta = -1;
				BufferedInputDelta = 0;

				CurrentIndex--;
				if (CurrentIndex < 0) {
					CurrentIndex = Weapons.Length -1;
				}

				if (CurrentIndex != LastWeaponIndex) {
					SetWeapon( CurrentIndex );
				}
				LastWeaponIndex = CurrentIndex;
				return;
			}

			for (int i = 0; i < QuickSelectInput.Length; ++i) {
				if (i >= Weapons.Length) {
					break;
				}

				if (Input.GetButtonDown( QuickSelectInput[i] )) {

					if (i != LastWeaponIndex) {

						IsInTransition = true;
						TransitionAlpha = 0;
						TransitionDelta = -1;
						BufferedInputDelta = 0;
						
						CurrentIndex = i;
						
						SetWeapon( CurrentIndex );
					}

					LastWeaponIndex = CurrentIndex;
				}
			}

		} else {
			// in middle of transition; animation transition
			TransitionAlpha += Time.deltaTime;
			if (TransitionAlpha >= TransitionTime) {
				IsInTransition = false;
			}

			if (TransitionTime - TransitionAlpha <= InputBufferTime) {
				float axisValue = Input.GetAxis(AxisName);
				if (axisValue > 0 || Input.GetButtonDown(NextButtonName)) {
					BufferedInputDelta = 1;
				} else if (axisValue < 0 || Input.GetButtonDown(PrevButtonName)) {
					BufferedInputDelta = -1;
				}
			}
		}
	}

	// Used in case a weapon script is not set at the start of the level
	public void InitWeapons()
	{
		if (WeaponParent != null)
		{
			Transform[] weapPrefabs = new Transform[Weapons.Length];
			for (int i = 0; i < Weapons.Length; ++i) {
				weapPrefabs[i] = Weapons[i].WeaponPrefab;
			}
			
			WeaponParent.SendMessage("InitWeaponTypes", weapPrefabs);
		}
	}

	public void OnGUI() {
		Rect currRect = new Rect(CellScreenPosition.x, CellScreenPosition.y, CellSize.x, CellSize.y);
		if (FromScreenBottom) {
			currRect.y = Screen.height - CellSize.y - CellScreenPosition.y;
		}
		GUI.DrawTexture(currRect, FrameTexture);

		if (Weapons.Length == 0) {
			return;
		}

		Texture2D currTexture = Weapons[CurrentIndex].HUDTexture;
		float opacity = 1.0f;

		float transPerc = 0;
		if (TransitionTime > 0) {
			transPerc = TransitionAlpha / TransitionTime;
			transPerc = Mathf.Clamp(transPerc, 0f, 1f);
		}

		//currRect = new Rect(CellScreenPosition.x, CellScreenPosition.y, CellSize.x, CellSize.y);
		if (TransitionDelta > 0) {
			currRect.x += NextWeaponDirection.x * (1f - transPerc) * CellSize.x;
			currRect.y += NextWeaponDirection.y * (1f - transPerc) * CellSize.y;
			opacity = transPerc;
		} else {
			currRect.x += PrevWeaponDirection.x * (1f - transPerc) * CellSize.x;
			currRect.y += PrevWeaponDirection.y * (1f - transPerc) * CellSize.y;
			opacity = transPerc;
		}

		GUI.color = new Color(1f, 1f, 1f, opacity);
		GUI.DrawTexture(currRect, currTexture);
	}

	void SetWeapon(int inIndex) {
		if (WeaponParent != null) {
			WeaponParent.SendMessage("EquipWeapon", inIndex);
		}
		if (WeaponChangeSound != null) {
			AudioSource.PlayClipAtPoint(WeaponChangeSound, Camera.main.transform.position, SoundVolume);
		}
	}

	// used when the level is over or the player dies
	void Disable() {
		AllowWeaponChange = false;
	}
}
