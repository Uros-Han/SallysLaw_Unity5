using UnityEngine;
using System.Collections;

public class TryGuardian : MonoBehaviour {

	// Use this for initialization
	void OnEnable()
	{
		if(GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_enPlayerStatus == PLAYER_STATUS.RUNNER)
			GameObject.Find("TryGuardian").gameObject.SetActive(false);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
