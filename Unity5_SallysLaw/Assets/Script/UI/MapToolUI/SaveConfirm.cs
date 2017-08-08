using UnityEngine;
using System.Collections;

public class SaveConfirm : MonoBehaviour {

	void Update()
	{
		if (Input.GetKey (KeyCode.Escape)) {
			GameObject.Find ("SaveUI").transform.GetChild (0).gameObject.SetActive (false);
			GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bOverayUIOn = false;
		}
	}

	void OnClick()
	{
		if (GameObject.Find ("NamePlate").GetComponent<UIInput> ().value != "") {
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().Save (GameObject.Find ("NamePlate").GetComponent<UIInput> ().value);
			GameObject.Find ("SaveUI").transform.GetChild (0).gameObject.SetActive (false);
			GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bOverayUIOn = false;
		}
	}

}
