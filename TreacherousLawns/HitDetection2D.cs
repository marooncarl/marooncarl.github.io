/// <summary>
/// Hit detection 2D
/// 
/// Programmer: Carl Childers
/// Date: 12/19/2015
/// 
/// Detects hits against colliders with certain tags.  Can be enabled for certain parts of an animation.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CircleData {
	public Vector2 Position;
	public float Radius;
}

public class HitDetection2D : MonoBehaviour {
	
	public int MaxNumberOfHits = 5; // max number of hits per update
	public string[] TargetTags;
	public int[] TargetLayers;
	public CircleData[] HurtCircles;

	Collider2D[] HitColliders;
	int MyLayerMask;
	List<Transform> ObjectsHit; // list of objects that were hit so far in the same attack


	void Start() {
		HitColliders = new Collider2D[MaxNumberOfHits];
		ObjectsHit = new List<Transform>(MaxNumberOfHits);

		MyLayerMask = 0;
		foreach (int i in TargetLayers) {
			MyLayerMask = MyLayerMask | (1 << i);
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach (CircleData c in HurtCircles) {
			Vector2 center = new Vector2(transform.position.x, transform.position.y);
			float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

			center.x += Mathf.Cos(angle) * c.Position.x - Mathf.Sin(angle) * c.Position.y;
			center.y += Mathf.Sin(angle) * c.Position.x + Mathf.Cos(angle) * c.Position.y;

			int numHits = Physics2D.OverlapCircleNonAlloc(center, c.Radius, HitColliders, MyLayerMask);
			if (numHits > 0) {
				//foreach (Collider2D coll in HitColliders) {
				for (int i = 0; i < numHits; ++i) {
					Collider2D coll = HitColliders[i];
					if (ObjectsHit.Count < MaxNumberOfHits && !ObjectsHit.Contains(coll.transform.root)) {
						ObjectsHit.Add(coll.transform.root);
						BroadcastMessage("HitTarget", coll.transform.root, SendMessageOptions.DontRequireReceiver);
						//print("ObjectsHit.Count: " + ObjectsHit.Count);
					}
				}
			}
		}
	}

	// Messages

	// Marks the start of a new attack, so targets that were hit previously can be hit again
	void RefreshAttack() {
		if (ObjectsHit != null) {
			ObjectsHit.Clear();
		}
	}
}
