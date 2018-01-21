/// <summary>
/// Bug spray
/// 
/// Programmer: Carl Childers
/// Date: 5/24/2015
/// 
/// Damages anything with health in an area, and damages bugs more.
/// </summary>

using UnityEngine;
using System.Collections;

public class BugSpray : MonoBehaviour {

	public Transform ProjectilePrefab;
	public float ProjectileRadius;
	public float ProjectileSpeed = 0;
	public float FireInterval = 0;

	AudioSource MyAudio;

	bool IsFiring;
	float CooldownCounter;


	void Awake() {
		MyAudio = GetComponent<AudioSource>();

		IsFiring = false;
		CooldownCounter = 0;
	}

	void Update() {
		CooldownCounter = Mathf.Max(CooldownCounter - Time.deltaTime, 0);

		if (IsFiring && ProjectilePrefab != null && CooldownCounter == 0) {
			Transform proj = (Transform)Instantiate(ProjectilePrefab, transform.position + transform.rotation * (Vector3.up * ProjectileRadius), ProjectilePrefab.rotation);
			if (ProjectileSpeed != 0) {
				float angle = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
				Vector2 projVelocity = new Vector2(Mathf.Cos(angle) * ProjectileSpeed, Mathf.Sin(angle) * ProjectileSpeed);
				proj.SendMessage("SetMoveDelta", projVelocity, SendMessageOptions.DontRequireReceiver);
			}
			CooldownCounter = FireInterval;
		}
	}

	// Messages

	public void StartFire() {
		if (!enabled) {
			return;
		}
		IsFiring = true;

		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem p in ps) {
			p.Play();
		}

		if (MyAudio != null) {
			MyAudio.Play();
		}
	}

	public void StopFire() {
		if (!enabled) {
			return;
		}

		IsFiring = false;

		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem p in ps) {
			p.Stop();
		}

		if (MyAudio != null) {
			MyAudio.Stop();
		}
	}

	void TurnOff() {
		StopFire();
		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem p in ps) {
			p.Clear();
		}
	}

	void Disable() {
		enabled = false;
	}
}
