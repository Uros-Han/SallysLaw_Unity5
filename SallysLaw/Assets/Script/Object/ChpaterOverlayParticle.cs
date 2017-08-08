using UnityEngine;
using System.Collections;

public class ChpaterOverlayParticle : MonoBehaviour {

	float fMinWaitTime = 10f;
	float fMaxWaitTime = 20f;

	// Use this for initialization
	void Start () {
		StartCoroutine (RandomPlayer ());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	IEnumerator RandomPlayer()
	{
		GameObject ActivatedParticle = null;


		do {
			yield return new WaitForSeconds(Random.Range(fMinWaitTime, fMaxWaitTime));

			for(int i = 0; i < 2; ++i)
			{
				if(transform.GetChild(i).gameObject.activeInHierarchy)
				{
					ActivatedParticle = transform.GetChild(i).gameObject;
					break;
				}
			}

			if(ActivatedParticle != null)
				ActivatedParticle.GetComponent<ParticleSystem>().Play();

		} while(true);
	}
}
