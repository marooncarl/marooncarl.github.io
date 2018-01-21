/// <summary>
/// Health meter
/// 
/// Programmer: Carl Childers
/// Date: 1/20/2015
/// 
/// Draws a health meter at a certain point on the screen.
/// </summary>

using UnityEngine;
using System.Collections;

public class HealthMeter : MonoBehaviour {

	public Rect meterRect;
	public bool PositionAtScreenPercentage = true;
	public float ScreenXPerc = 0.5f;
	public float ScreenYPerc = 0.9f;
	public float segmentSize = 32;
	public float segmentHealth = 10;
	public Texture2D frontTexture, backTexture;
	public Transform target; // the object the meter looks up for health

	Health healthScript;

	void Awake() {
		useGUILayout = false;
		if (segmentHealth <= 0 || target == null) {
			enabled = false;
			return;
		}
		//print ("got target");
		healthScript = target.GetComponent<Health>();
		if (healthScript == null) {
			enabled = false;
		}
		//print ("got health script");
	}

	void OnGUI() {
		if (Event.current.type.Equals (EventType.Repaint))
		{
			drawMeter (healthScript.MaxHealth, backTexture);
			drawMeter (healthScript.GetHealth(), frontTexture);
		}
	}

	void drawMeter(float amt, Texture2D tex) {
		if (segmentHealth <= 0) {
			return; // otherwise will have infinite loop
		}
		float healthLeft = amt;
		Rect drawRect = meterRect;
		if (PositionAtScreenPercentage) {
			Rect tempRect = GUIUtilityFunctions.GetRectFromScreenPerc(ScreenXPerc, ScreenYPerc, 0, 0);
			drawRect.x = tempRect.x - drawRect.width / 2.0f;
			drawRect.y = tempRect.y - drawRect.height / 2.0f;
		}

		Rect sourceRect = new Rect(0, 0, 1, 1);
		while (healthLeft > 0) {
			float thisSegHealth = Mathf.Min (healthLeft, segmentHealth);
			drawRect.width = segmentSize * (thisSegHealth / segmentHealth);
			sourceRect.width = thisSegHealth / segmentHealth;

			Graphics.DrawTexture (drawRect, tex, sourceRect, 0, 0, 0, 0);

			healthLeft -= segmentHealth;
			drawRect.x += drawRect.width;
		}
	}
}
