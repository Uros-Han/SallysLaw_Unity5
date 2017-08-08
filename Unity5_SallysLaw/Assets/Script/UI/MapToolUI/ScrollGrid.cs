using UnityEngine;
using System.Collections;

public class ScrollGrid : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
//	void Update () {
//		GameObject.Find ("ScrollGrid").GetComponent<UIGrid> ().Reposition ();
//
//		if (Input.GetKey (KeyCode.Escape)) {
//			GameObject.Find ("LoadUI").transform.GetChild (0).gameObject.SetActive (false);
//			if(GameObject.Find("MapToolMgr") != null)
//				GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bOverayUIOn = false;
//		}
//	}

	 public void MakeFileList(string[] strAry, bool bInit = false)
	{
//		if (bInit) {
//			for (int i = 0; i < GameObject.Find("ScrollGrid").transform.childCount; ++i) {
//				Destroy (GameObject.Find ("ScrollGrid").transform.GetChild (i).gameObject);
//			}
//		}

		GameObject objFileBtn;

		MapListMgr.getInstance.m_MapList.Clear ();

		for(int i = 0; i < strAry.Length; ++i)
		{
			objFileBtn = Instantiate(Resources.Load("Prefabs/UI/MapIcon") as GameObject) as GameObject;
			objFileBtn.transform.parent = GameObject.Find("ScrollGrid").transform;
			objFileBtn.transform.localScale = Vector2.one;
			objFileBtn.transform.localPosition = Vector2.zero;
			MapListMgr.getInstance.m_MapList.Add(System.IO.Path.GetFileNameWithoutExtension(strAry[i]));
			objFileBtn.transform.GetChild(1).GetComponent<MapIcon>().m_strName = System.IO.Path.GetFileNameWithoutExtension(strAry[i]);
			objFileBtn.transform.GetChild(1).GetComponent<MapIcon>().m_strExtension = System.IO.Path.GetExtension(strAry[i]);
			//objFileBtn.transform.GetChild(0).GetComponent<UILabel>().text = System.IO.Path.GetFileName(strAry[i]);
		}

	}
}
