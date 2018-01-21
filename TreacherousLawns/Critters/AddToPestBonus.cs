/// <summary>
/// Add to pest bonus
/// 
/// Programmer: Carl Childers
/// Date: 10/10/2015
/// 
/// Adds to score keeper's pest threshold at the beginning, and when the critter
/// is defeated, adds to the pest bonus
/// </summary>


using UnityEngine;
using System.Collections;

public class AddToPestBonus : MonoBehaviour {

	public int MyBonus = 10;

	// Use this for initialization
	void Start () {
		LawnScoreKeeper sKeeperScript = FindObjectOfType<LawnScoreKeeper>();
		Transform ScoreKeeper = null;
		if (sKeeperScript != null) {
			ScoreKeeper = sKeeperScript.transform;
		}
		if (ScoreKeeper != null) {
			//print("Adding to Pest Threshold");
			ScoreKeeper.SendMessage("AddToPestThreshold", MyBonus);
		}
	}
	
	void Died(Defense.EDamageType inDmg) {
		LawnScoreKeeper sKeeperScript = FindObjectOfType<LawnScoreKeeper>();
		Transform ScoreKeeper = null;
		if (sKeeperScript != null) {
			ScoreKeeper = sKeeperScript.transform;
		}
		if (ScoreKeeper != null) {
			//print("Adding my Pest Bonus");
			ScoreKeeper.SendMessage("AddToPestBonus", MyBonus);
		}
	}

	void Bombed() {
		LawnScoreKeeper sKeeperScript = FindObjectOfType<LawnScoreKeeper>();
		Transform ScoreKeeper = null;
		if (sKeeperScript != null) {
			ScoreKeeper = sKeeperScript.transform;
		}
		if (ScoreKeeper != null) {
			//print("Adding my Pest Bonus");
			ScoreKeeper.SendMessage("AddToPestBonus", MyBonus);
		}
	}
}
