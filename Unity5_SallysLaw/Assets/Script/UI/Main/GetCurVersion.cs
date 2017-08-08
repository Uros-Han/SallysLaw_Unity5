using UnityEngine;
using System.Collections;

public class GetCurVersion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<UILabel> ().text = "Version " + GameMgr.getInstance.m_strVersion;
	}

}
