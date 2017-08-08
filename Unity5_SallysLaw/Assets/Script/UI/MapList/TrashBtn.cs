using UnityEngine;
using System.Collections;
using System.IO;

public class TrashBtn : MonoBehaviour {

	MapListMgr mapListMgr;

	// Use this for initialization
	void Start () {
		mapListMgr = GameObject.Find ("MapListMgr").GetComponent<MapListMgr>();
	}

	void OnClick()
	{
		System.IO.File.Delete (Application.persistentDataPath + "/Stages/" + mapListMgr.m_strCurMap + GameObject.Find ("MapListMgr").GetComponent<MapListMgr>().m_strCurExtension);
		System.IO.File.Delete (Application.persistentDataPath + "/StagesPng/" + mapListMgr.m_strCurMap);

		mapListMgr.MakeMapIconList ();

		GameObject.Find ("Panel(Clipped View)").GetComponent<SpringPanel> ().target = new Vector3 (GameObject.Find ("Panel(Clipped View)").GetComponent<SpringPanel> ().target.x + 100,0);
	}
}
