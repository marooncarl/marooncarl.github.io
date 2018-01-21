/// <summary>
/// Explosion death
/// 
/// Programmer: Carl Childers
/// Date: 7/11/2015
/// 
/// Creates an explosion upon death, but only for certain types of damage.  Can also use an alternate death effect for
/// other damage types.
/// </summary>


using UnityEngine;
using System.Collections;

public class ExplosionDeath : MonoBehaviour {

	public Transform ExplosionPrefab;
	public Transform NormalDeathEffect;
	public bool UseCurrentRotation = false;
	public Defense.EDamageType[] ExplosionDamageTypes;


	// Messages
	
	void Died(Defense.EDamageType inDamageType) {
		bool shouldExplode = false;
		foreach (Defense.EDamageType d in ExplosionDamageTypes) {
			if (d == inDamageType) {
				shouldExplode = true;
				break;
			}
		}

		Transform chosenPrefab;
		if (shouldExplode) {
			chosenPrefab = ExplosionPrefab;
		} else {
			chosenPrefab = NormalDeathEffect;
		}

		if (chosenPrefab != null) {
			Instantiate(chosenPrefab, transform.position, (UseCurrentRotation == true ? transform.rotation : chosenPrefab.rotation));
		}
		
		Destroy(this.gameObject);
	}
}
