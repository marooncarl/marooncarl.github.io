/// <summary>
/// Fade out
/// 
/// Programmer: Carl Childers
/// Date: 10/20/2015
/// 
/// Causes an object's sprites to fade out, with an optional delay.  Good to use in conjunction with Timed Destroy.
/// </summary>


using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {

	public float Delay = 1.0f;
	public float FadeTime = 1.0f;

	float DelayCounter, FadeCounter;

	void Awake() {
		DelayCounter = 0;
		FadeCounter = 0;
	}
	
	// Update is called once per frame
	void Update () {
		DelayCounter += Time.deltaTime;
		if (DelayCounter >= Delay) {
			FadeCounter += Time.deltaTime;

			if (FadeTime > 0) {
				SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer s in allSprites) {
					Color newColor = s.color;
					newColor.a = 1.0f - (FadeCounter / FadeTime);
					s.color = newColor;
				}
			}
		}
	}
}
