// Programmer: Carl Childers
// Date: 7/26/2016
//
// Menu musics needs to persist between scenes, but stop when a level is selected.

using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour {

	static MenuMusic TheMenuMusic;


	void Awake() {
		if (TheMenuMusic == null) {
			TheMenuMusic = this;
		}
		else {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(this.gameObject);
	}

	public static MenuMusic GetTheMenuMusic()
	{
		return TheMenuMusic;
	}
}
