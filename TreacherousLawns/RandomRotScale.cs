/// <summary>
/// Random rot scale
/// 
/// Programmer: Carl Childers
/// Date: 12/8/2015
/// 
/// Randomizes rotation and scale, and can optionally grow into the random target scale.
/// </summary>

using UnityEngine;
using System.Collections;

public class RandomRotScale : MonoBehaviour {

	public float MinScale = 0.5f;
	public float MaxScale = 1.0f;
	public bool GrowIntoScale = true;
	public float GrowDuration = 1.0f;
	public float GrowExponent = 1.0f;

	float TargetScale;
	float ScaleAlpha;

	
	void Awake () {
		ScaleAlpha = 0f;
		TargetScale = Random.Range(MinScale, MaxScale);
		transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
		if (GrowIntoScale) {
			transform.localScale = Vector3.zero;
		} else {
			transform.localScale = new Vector3(TargetScale, TargetScale, TargetScale);
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (GrowIntoScale && GrowDuration > 0) {
			ScaleAlpha = Mathf.Clamp(ScaleAlpha + Time.deltaTime / GrowDuration, 0f, 1.0f);
			float newScale = TargetScale * Mathf.Pow(ScaleAlpha, GrowExponent);
			transform.localScale = new Vector3(newScale, newScale, newScale);
			if (ScaleAlpha == 1.0f) {
				enabled = false;
			}
		} else {
			enabled = false;
		}
	}
}
