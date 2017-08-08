using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		StartCoroutine (Floating ());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	IEnumerator Floating()
	{
		float fFloatingSpeed = 0.0005f;


		do {
			yield return new WaitForFixedUpdate();

			transform.Translate(Vector3.left * fFloatingSpeed);

		} while(true);
	}
}
