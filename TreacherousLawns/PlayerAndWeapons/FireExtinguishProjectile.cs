/// <summary>
/// Fire extinguish projectile
/// 
/// Programmer: Carl Childers
/// Date: 7/4/2015
/// 
/// Projectile that extinguishes fires and some enemies.
/// </summary>

using UnityEngine;
using System.Collections;

public class FireExtinguishProjectile : MonoBehaviour {
	
	public float DPS = 0.5f; // Damage per second
	public Defense.EDamageType DamageType = Defense.EDamageType.DT_Water;
	public float CircleCastRadius = 1.0f;
	public int[] DamageLayers;
	

	// Update is called once per frame
	void Update () {
		int layerMask = 0;
		for (int i = 0; i < DamageLayers.Length; ++i) {
			layerMask = layerMask | (1 << DamageLayers[i]);
		}

		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), CircleCastRadius, layerMask);

		if (hitColliders.Length > 0) {
			foreach (Collider2D c in hitColliders) {
				Damage dmg = new Damage(DPS, DamageType, true);
				c.transform.BroadcastMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);
			}
		 }
	}
}
