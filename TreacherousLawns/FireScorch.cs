/// <summary>
/// Fire scorch
/// 
/// Programmer: Carl Childers
/// Date: 6/26/2015
/// 
/// Used with a fire object.  After a time, reduces the fire to a smaller version and creates a scorch object.
/// </summary>

using UnityEngine;
using System.Collections;

public class FireScorch : MonoBehaviour {

	public float ScorchTime = 10.0f;
	public float PostScorchTime = 2.0f; // destroy this many seconds after scorching.  Particle effects are stopped when scorching.
	public Transform SmallFirePrefab;
	public Transform ScorchPrefab;


	void Start () {
		Invoke("Scorch", ScorchTime);
	}

	void Scorch() {
		if (SmallFirePrefab != null) {
			Instantiate(SmallFirePrefab, transform.position, transform.rotation);
		}
		if (ScorchPrefab != null) {
			Instantiate(ScorchPrefab, transform.position, transform.rotation);
		}

		if (particleSystem != null) {
			particleSystem.Stop();
		}
		for (int i = 0; i < transform.childCount; ++i) {
			StopChildParticles(transform.GetChild(i));
		}

		if (collider2D != null) {
			collider2D.enabled = false;
		}

		Invoke("DestroySelf", PostScorchTime);
	}

	void DestroySelf() {
		Destroy(this.gameObject);
	}
	
	// Recursive; goes through children and stops particle effects
	void StopChildParticles(Transform inChild) {
		if (inChild.particleSystem != null) {
			inChild.particleSystem.Stop();
		}

		for (int i = 0; i < inChild.childCount; ++i) {
			StopChildParticles(inChild.GetChild(i));
		}
	}
}
