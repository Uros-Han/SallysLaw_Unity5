using UnityEngine;
using System.Collections;
using System.IO;

public class LoadBtn : MonoBehaviour {

	void OnClick(){
		if (Directory.Exists(Application.persistentDataPath + "/Stages") && Directory.GetFiles (Application.persistentDataPath + "/Stages", "*.stg").Length > 0) {
			GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bOverayUIOn = true;
			GameObject.Find ("LoadUI").transform.GetChild (0).gameObject.SetActive (true);
			GameObject.Find ("ScrollGrid").GetComponent<ScrollGrid> ().MakeFileList (Directory.GetFiles (Application.persistentDataPath + "/Stages", "*.stg"), true);
			GameObject.Find ("ScrollGrid").GetComponent<ScrollGrid> ().MakeFileList (Directory.GetFiles (Application.persistentDataPath + "/Stages", "*.txt"));
		} else {
			GameObject tmpErrMsgPrf = Resources.Load ("Prefabs/UI/mapToolErrorMsg") as GameObject;

			GameObject tmpErrMsg = Instantiate (tmpErrMsgPrf) as GameObject;
			tmpErrMsg.GetComponent<UILabel> ().text = "You Dont Have ANY Stage File!";
		}
	}
}
