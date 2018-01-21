/// <summary>
/// Alpha phase
/// 
/// Programmer: Carl Childers
/// Date: 8/26/2015
/// 
/// Makes a sprite component change between two colors, or two transparancy levels.
/// </summary>

using UnityEngine;
using System.Collections;

public class AlphaPhase : MonoBehaviour {

	public Color FirstColor = Color.white;
	public Color SecondColor = Color.white;
	public float PhaseTime = 1.0f;
	public bool AffectsChildren = false;

	SpriteRenderer MySpriteRenderer;
	SpriteRenderer[] AllSpriteRenderers;

	float Alpha; // progress between the two colors
	bool Increasing; // is alpha increasing or decreasing?


	void Awake() {
		if (PhaseTime == 0) {
			enabled = false;
			return;
		}

		if (!AffectsChildren) {
			MySpriteRenderer = GetComponent<SpriteRenderer>();
			if (MySpriteRenderer == null) {
				enabled = false;
				return;
			}
		} else {
			AllSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
			if (AllSpriteRenderers.Length == 0) {
				enabled = false;
				return;
			}
		}

		Alpha = 0;
		Increasing = true;
	}

	void Update () {
		if (Increasing) {
			Alpha = Mathf.Min(Alpha + Time.deltaTime / PhaseTime, 1.0f);
			if (Alpha == 1.0f) {
				Increasing = false;
			}
		} else {
			Alpha = Mathf.Max(Alpha - Time.deltaTime / PhaseTime, 0.0f);
			if (Alpha == 0.0f) {
				Increasing = true;
			}
		}

		Color nextColor = Color.Lerp(FirstColor, SecondColor, Alpha);
		if (AffectsChildren) {
			foreach (SpriteRenderer r in AllSpriteRenderers) {
				r.color = nextColor;
			}
		} else {
			MySpriteRenderer.color = nextColor;
		}
	}
}
