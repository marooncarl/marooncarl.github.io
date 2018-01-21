// Programmer: Carl Childers
// Date: 9/13/2017
//
// Like a juggle object, but plays an animation trigger when restarting.
// If a regular juggle object is used, then that animation will only play the first time, and not when the object is reused.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedJuggleObject : JuggleObject {

	public string AnimTrigger = "";
	public string SkipTrigger = "";

	Animator myAnimator;


	public override void Restart(Vector2 startPos, bool wasTossed = true)
	{
		base.Restart(startPos, wasTossed);

		if (myAnimator == null)
		{
			myAnimator = GetComponent<Animator>();
		}
		if (myAnimator != null)
		{
			myAnimator.SetTrigger(AnimTrigger);
		}
	}

	public override void RecreateFromSaveData(ItemStateData inStateData, Juggler inJuggler)
	{
		base.RecreateFromSaveData(inStateData, inJuggler);

		if (myAnimator == null)
		{
			myAnimator = GetComponent<Animator>();
		}
		if (myAnimator != null && SkipTrigger != "")
		{
			myAnimator.SetTrigger(SkipTrigger);
		}
	}
}
