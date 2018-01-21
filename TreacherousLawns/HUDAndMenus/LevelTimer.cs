/// <summary>
/// Level timer
/// 
/// Programmer: Carl Childers
/// Date: 5/9/2015
/// 
/// Counts up, telling how much time the player is spending on the level.  Has messages
/// to start and stop the timer.
/// </summary>

using UnityEngine;
using System.Collections;

public class LevelTimer : MonoBehaviour {

	public bool IsCounting = true;
	public Vector2 ScreenPerc = new Vector2(0.5f, 0.0f);
	public Vector2 TimerArea = new Vector2(150, 50);
	public int HorizontalAlignment = 0; // -1 = left, 0 = center, 1 = right
	public GUIStyle TimerStyle;

	float TimeCounter;

	// Update is called once per frame
	void Update () {
		if (IsCounting) {
			TimeCounter += Time.deltaTime;
		}
	}

	// Messages

	void StartTimer() {
		IsCounting = true;
	}

	void StopTimer() {
		IsCounting = false;
	}

	// Get minutes and seconds
	public int GetMinutes() {
		return (int)TimeCounter / 60;
	}

	public int GetSeconds() {
		return (int)(TimeCounter - GetMinutes() * 60);
	}

	public int GetTime() {
		return (int)TimeCounter;
	}

	void OnGUI() {
		int secs;
		string timeString, secondsString;

		secs = GetSeconds();
		secondsString = secs.ToString();
		if (secs < 10) {
			secondsString = "0" + secondsString;
		}
		timeString = GetMinutes().ToString() + " : " + secondsString;

		float widthAdjust = 0;
		if (HorizontalAlignment == 0) {
			widthAdjust = TimerArea.x / 2;
		} else if (HorizontalAlignment > 0) {
			widthAdjust = TimerArea.x;
		}

		GUI.Label(new Rect(Screen.width * ScreenPerc.x - widthAdjust, Screen.height * ScreenPerc.y, TimerArea.x, TimerArea.y),
		          timeString, TimerStyle);
	}
}
