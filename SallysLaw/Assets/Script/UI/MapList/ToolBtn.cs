using UnityEngine;
using System.Collections;

public class ToolBtn : MonoBehaviour {

	MapListMgr m_MapListMgr;

	void Start()
	{
		m_MapListMgr = GameObject.Find ("MapListMgr").GetComponent<MapListMgr>();
	}

	void OnClick()
	{
		Application.LoadLevel ("MapTool");
	}
}
