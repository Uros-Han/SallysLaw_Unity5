using UnityEngine;
using System.Collections;

public class FollowingRunner : MonoBehaviour {
	Transform m_RunnerTransform;
	bool m_bFindRunner;

	// Use this for initialization
	void Start () {
		StartCoroutine (Looping ());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	IEnumerator Looping()
	{
		while (true) {
			if(!m_bFindRunner)
			{
				if(GameObject.Find ("Runner(Clone)") != null)
				{
					m_RunnerTransform = GameObject.Find ("Runner(Clone)").transform;
					m_bFindRunner = true;
				}
			}else{
				transform.position = m_RunnerTransform.position;
			}

			yield return null;
		}
	}
}
