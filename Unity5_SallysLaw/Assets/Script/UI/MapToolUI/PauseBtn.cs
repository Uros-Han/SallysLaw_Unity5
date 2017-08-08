using UnityEngine;
using System.Collections;

/// <summary>
/// Maptool pausebtn
/// </summary>
public class PauseBtn : MonoBehaviour {

	GameObject m_PauseObj;
	bool m_bPause;

	void Start()
	{
		m_PauseObj = GameObject.Find ("PauseScreen").transform.GetChild (0).gameObject;
	}

	void OnClick()
	{
		if (!m_bPause) {
			m_PauseObj.SetActive (true);
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bOverayUIOn = true;
			m_bPause = true;
		} else if (m_bPause) {
			m_PauseObj.SetActive (false);
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bOverayUIOn = false;
			m_bPause = false;
		}

	}
}
