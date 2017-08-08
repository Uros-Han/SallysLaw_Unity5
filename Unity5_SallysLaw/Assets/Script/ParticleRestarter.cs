using UnityEngine;
using System.Collections;

public class ParticleRestarter : MonoBehaviour {
	ParticleSystem particle;
	// Use this for initialization
	void OnEnable () {
		particle = GetComponent<ParticleSystem> ();

		StartCoroutine (ParticleRestart ());
	}
	
	IEnumerator ParticleRestart()
	{
		float fMinWait = 10f;
		float fMaxWait = 20f;

		float fRandomWait = Random.Range(fMinWait, fMaxWait);
		float fCurrentWait = 0;
		do{

			yield return null;

			fCurrentWait += Time.deltaTime;

			if(fCurrentWait > fRandomWait)
			{
				particle.Play();

				fRandomWait = Random.Range(fMinWait, fMaxWait);
				fCurrentWait = 0f;
			}

		}while(true);
	}
}
