// Programmer: Carl Childers
// Date: 7/25/2016
//
// Allows a fire wasp to choose between multiple attacks.  Receives and sends messages to
// communicate with WaspMove, and can enable and disable other scripts to choose the attack.

using UnityEngine;
using System.Collections;

public class FireWaspAttack : MonoBehaviour {

	public float FlamethrowerDelay = 0.5f;
	public float FlamethrowerDuration = 1.0f;
	public string AttackAnimName = "AttackNum";

	DelayedFireProjectile ProjectileScript;
	BugSpray FlamethrowerScript;
	Animator MyAnimator;

	enum EFireWaspAttackType {
		AT_Flame,
		AT_Bomb
	};

	EFireWaspAttackType NextAttackType, LastAttackType;


	void Awake() {
		ProjectileScript = GetComponentInChildren<DelayedFireProjectile>();
		FlamethrowerScript = GetComponentInChildren<BugSpray>();
		MyAnimator = GetComponent<Animator>();

		NextAttackType = EFireWaspAttackType.AT_Flame;
		LastAttackType = EFireWaspAttackType.AT_Flame;
	}

	void StartFlamethrower()
	{
		FlamethrowerScript.StartFire();
		Invoke("EndFlamethrower", FlamethrowerDuration);
	}

	void EndFlamethrower()
	{
		FlamethrowerScript.StopFire();
		SendMessage("FinishedAttack");
		if (MyAnimator != null)
			MyAnimator.SetInteger(AttackAnimName, 0);
	}

	// Messages

	void ChooseAttack()
	{
		// Prevent firing a bomb two times in a row
		if (LastAttackType != EFireWaspAttackType.AT_Bomb && Random.value < 0.3333)
			NextAttackType = EFireWaspAttackType.AT_Bomb;
		else
			NextAttackType = EFireWaspAttackType.AT_Flame;

		LastAttackType = NextAttackType;

		switch (NextAttackType)
		{
		case EFireWaspAttackType.AT_Flame:
			Invoke("StartFlamethrower", FlamethrowerDelay);
			if (MyAnimator != null)
				MyAnimator.SetInteger(AttackAnimName, 2);
			break;
		case EFireWaspAttackType.AT_Bomb:
			ProjectileScript.StartAttack();
			break;
		}
	}
}
