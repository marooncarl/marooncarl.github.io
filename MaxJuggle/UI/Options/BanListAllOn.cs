// Programmer: Carl Childers
// Date: 12/12/2017
//
// Turns all items on for the ban list, and then saves the ban list.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanListAllOn : MonoBehaviour {

	public JuggleBanList MyBanList;
	public AudioClip AllOnSound;
	[Range(0, 1)]
	public float AllOnSoundVolume = 1f;


	public void AllowAll()
	{
		if (MyBanList != null)
		{
			foreach (BanListButton btn in MyBanList.ItemButtons)
			{
				btn.ChangeItemSetting(true, false);
			}
			MyBanList.Save();

			if (AllOnSound != null)
			{
				MyBanList.MyAudioSource.PlayOneShot(AllOnSound, AllOnSoundVolume);
			}
		}
	}
}
