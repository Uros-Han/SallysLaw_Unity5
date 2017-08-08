using UnityEngine;
using System.Collections;

public class LoadFileBtn : MonoBehaviour {

	void OnClick()
	{
		GameObject.Find ("StageLoader").GetComponent<StageLoader> ().LoadStage (transform.parent.GetChild(0).GetComponent<UILabel>().text);
		GameObject.Find ("LoadUI").transform.GetChild (0).gameObject.SetActive (false);
		if(GameObject.Find("MapToolMgr") != null)
			GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bOverayUIOn = false;
	}
}
