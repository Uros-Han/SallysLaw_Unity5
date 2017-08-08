using UnityEngine;
using System.Collections;

public class StagePlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (GameObject.Find ("MapListMgr") != null) {
			GameObject.Find ("StageLoader").GetComponent<StageLoader> ().LoadStage (GameObject.Find ("MapListMgr").GetComponent<MapListMgr>().m_strCurMap + GameObject.Find ("MapListMgr").GetComponent<MapListMgr>().m_strCurExtension);
		}
	}
}
