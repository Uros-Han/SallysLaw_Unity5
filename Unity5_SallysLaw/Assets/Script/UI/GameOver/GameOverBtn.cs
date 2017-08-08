using UnityEngine;
using System.Collections;

public class GameOverBtn : MonoBehaviour {

	void OnClick()
	{
		Application.LoadLevel (Application.loadedLevelName);
		GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_TabStat = TAP_STATUS.JUMP;
		TimeMgr.Play ();
	}
}
