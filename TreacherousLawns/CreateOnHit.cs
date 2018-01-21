/// <summary>
/// Create on hit
/// 
/// Programmer: Carl Childers
/// Date: 6/27/2015
/// 
/// Creates a prefab when hitting another object
/// </summary>

using UnityEngine;
using System.Collections;

public class CreateOnHit : MonoBehaviour {

	public Transform MyPrefab;
	public float DetonateTime = 0; // if 0, the object will keep going until it hits something.  Otherwise it will detonate after a set time.


	void Start() {
		if (DetonateTime > 0) {
			Invoke("Detonate", DetonateTime);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject != null) {
			Detonate();
		}
	}

	void Detonate() {
		if (MyPrefab != null) {
			Instantiate(MyPrefab, transform.position, transform.rotation);
		}
		
		Destroy(this.gameObject);
	}
}
