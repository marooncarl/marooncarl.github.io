/// <summary>
/// Sight cone 2D
/// 
/// Programmer: Carl Childers
/// Date: 6/7/2015
/// 
/// Allows the game object to see things with certain tags, and then sends a message to, say, an AI script to handle it.
/// </summary>

using UnityEngine;
using System.Collections;

public class SightCone2D : MonoBehaviour {

	public float SightRadius = 5.0f;
	public float SightConeAngle = 45.0f; // half of the cone
	public string[] VisibleTags;
	public int[] SightBlockingLayers;

	float SightConeDot;
	int layerMask;


	void Start() {
		if (VisibleTags.Length == 0) {
			enabled = false;
			return;
		}

		SightConeDot = Mathf.Cos(SightConeAngle * Mathf.Deg2Rad);

		layerMask = 0;
		foreach (int ly in SightBlockingLayers) {
			layerMask = layerMask | (1 << ly);
		}
	}

	// Update is called once per frame
	void Update () {
		Vector3 myPos = transform.position;
		Vector2 facingNorm = new Vector2( -Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad),
		                                 Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad));

		ArrayList seenTransforms = new ArrayList();

		for (int i = 0; i < VisibleTags.Length; ++i) {
			GameObject[] objects = GameObject.FindGameObjectsWithTag( VisibleTags[i] );
			foreach (GameObject g in objects) {
				Vector2 deltaPos = g.transform.position - myPos;
				float mag = deltaPos.magnitude;
				Vector2 norm = deltaPos.normalized;

				if (mag > SightRadius) {
					continue;
				}

				if (Vector2.Dot(norm, facingNorm) < SightConeDot) {
					continue;
				}

				RaycastHit2D raycastHit = Physics2D.Raycast(new Vector2(myPos.x, myPos.y), norm, mag, layerMask);

				if (raycastHit.collider == null) {
					seenTransforms.Add(g.transform);
				}
			}
		}

		if (seenTransforms.Count > 0) {
			// pick the closest transform
			float closestDist = Mathf.Infinity;
			Transform closestTransform = null;
			foreach (Transform t in seenTransforms) {
				float dist = (t.position - myPos).magnitude;
				if (dist < closestDist) {
					closestTransform = t;
					closestDist = dist;
				}
			}

			if (closestTransform != null) {
				SendMessage("TargetSighted", closestTransform);
			}
		}
	}
}
