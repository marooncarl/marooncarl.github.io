// Programmer: Carl Childers
// Date: 10/21/2017
//
// Child of a Splitting Juggle Object.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitChildItem : JuggleObject {

	public Vector2 SplitPosition;				// Position relative to the parent when splitting.
	public JuggleObjectType ItemType;
	public JuggleObjectType ParentType;

	bool otherChildWasDropped;
	public bool OtherChildWasDropped {
		get { return otherChildWasDropped; }
		set { otherChildWasDropped = value; }
	}

	public LinkedList<SplitChildItem> Siblings;


	public override void Restart(Vector2 startPos, bool wasTossed = true)
	{
		base.Restart(startPos, wasTossed);

		StartNearFullEffect();
		otherChildWasDropped = false;
	}

	public override void WasJuggled(Juggler inJuggler, Vector2 hitPoint)
	{
		if (FullyJuggled || Failed)
		{
			return;
		}

		fullyJuggled = true;

		// Count the parent item as fully juggled when no other children were dropped and this is the last one.
		if (!otherChildWasDropped && (Siblings == null || Siblings.Count == 0))
		{
			inJuggler.FullyJuggledObject(ParentType, ParentType);
		}
		else
		{
			RemoveFromChain();
		}

		RemoveFromPlay();

		float juggleForce = (fullyJuggled ? inJuggler.FullJuggleForce : inJuggler.JuggleForce);
		ApplyForce(hitPoint, juggleForce);

		if (!fullyJuggled)
		{
			inJuggler.PlayJuggleEffect(hitPoint);
		}
		else
		{
			inJuggler.PlayFullJuggleEffect(hitPoint);
		}

		BecomeImmune();

		if (MyType.TossSound != null)
		{
			MyAudioSource.PlayOneShot(MyType.TossSound, MyType.JuggleTossVolume);
		}
	}

	protected override void Dropped()
	{
		if (hasDropped)
			return;

		hasDropped = true;
		Invoke("Disable", MyType.DropDisableDelay);
		RemoveFromPlay();

		if (!Failed && !otherChildWasDropped)
		{
			MyTosser.ObjectDropped(this);
			foreach (SplitChildItem child in Siblings)
			{
				child.OtherChildWasDropped = true;
			}
		}

		RemoveFromChain();
	}

	protected override int GetJuggleLimit(Juggler inJuggler)
	{
		return 1;
	}

	// Tells sibling objects to remove this one, since it was dropped or fully juggled
	void RemoveFromChain()
	{
		if (Siblings != null)
		{
			foreach (SplitChildItem child in Siblings)
			{
				if (child.Siblings.Contains(this))
				{
					child.Siblings.Remove(this);
				}
			}
		}
	}

	public override ItemStateData GetStateData()
	{
		ItemStateData stateData = base.GetStateData();

		stateData.CustomProps = new Dictionary<string, float>(1);

		stateData.CustomProps.Add("drp", (otherChildWasDropped ? 1 : 0));
		stateData.RelatedItems = new byte[Siblings.Count];

		int i = 0;
		foreach (SplitChildItem child in Siblings)
		{
			stateData.RelatedItems[i] = child.ID;
			i++;
		}

		return stateData;
	}

	public override void RecreateFromSaveData(ItemStateData inStateData, Juggler inJuggler)
	{
		base.RecreateFromSaveData(inStateData, inJuggler);

		otherChildWasDropped = (inStateData.CustomProps["drp"] > 0);
	}

	public override void PostRecreate(ItemStateData inStateData)
	{
		Siblings = new LinkedList<SplitChildItem>();

		foreach (byte siblingID in inStateData.RelatedItems)
		{
			foreach (JuggleObject item in MyTosser.ObjectsInPlay)
			{
				if (item.ID == siblingID && item is SplitChildItem)
				{
					Siblings.AddLast((SplitChildItem)item);
					break;
				}
			}
		}
	}
}
