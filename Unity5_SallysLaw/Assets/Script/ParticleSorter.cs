using UnityEngine;
using System.Collections;

public class ParticleSorter : MonoBehaviour {

	public string LayerName = "FrontObj";

	// Use this for initialization
	void Start () {
//		if(GetComponent<SkeletonAnimation>())
//			GetComponent<SkeletonAnimation>().renderer.sortingLayerName = "FrontObj";

		if (GetComponent<ParticleSystem> ()) {
			GetComponent<ParticleSystem> ().GetComponent<Renderer>().sortingLayerName = LayerName;
			GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = -1;
		}

	}

}
