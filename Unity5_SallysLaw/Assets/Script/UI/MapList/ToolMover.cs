using UnityEngine;
using System.Collections;

public class ToolMover : MonoBehaviour {

	MapListMgr m_MapListMgr;
	SpringPanel m_SpringPanel;

	void Start () {
		m_MapListMgr = GameObject.Find ("MapListMgr").GetComponent<MapListMgr> ();
		m_SpringPanel = GetComponent<SpringPanel> ();

		StartCoroutine (ToolManage ());
	}

	void OnDestroy(){
		StopAllCoroutines ();
	}

	IEnumerator ToolManage()
	{
		while (true) {
			if(m_MapListMgr.m_strCurMap == "Create New"){
				m_SpringPanel.target.x = 0;
			}else{
				m_SpringPanel.target.x = -130;
			}

			m_SpringPanel.enabled = true;

			yield return new WaitForEndOfFrame();
		}
	}
}
