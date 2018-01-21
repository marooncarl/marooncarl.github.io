using UnityEngine;
using System.Collections;

public class ParticleSystemLayer : MonoBehaviour {

	public string LayerName = "Default";
	public int LayerOrder;

	void Awake()
	{
		ParticleSystemRenderer particles = GetComponent<ParticleSystemRenderer>();
		if (particles != null) {
			particles.sortingLayerName = LayerName;
			particles.sortingOrder = LayerOrder;
		}


	}
}
