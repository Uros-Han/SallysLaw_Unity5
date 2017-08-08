using UnityEngine;
using System.Collections;
using System.IO;

public class MapListScene : MonoBehaviour {

	void Awake () {
		if(GameObject.Find("GameMgr") == null) //if gameMgr doesn't exist, make one.
		{
			GameObject gameMgr = Instantiate(Resources.Load("Prefabs/GameMgr") as GameObject) as GameObject;
			gameMgr.name = gameMgr.name.Replace("(Clone)","");
		}

		if (GameObject.Find ("MapListMgr") == null) {
			GameObject mapListMgr = Instantiate(Resources.Load("Prefabs/MapListMgr") as GameObject) as GameObject;
			mapListMgr.name = mapListMgr.name.Replace("(Clone)","");
		}

		if (!Directory.Exists (Application.persistentDataPath + "/Stages"))
			Directory.CreateDirectory (Application.persistentDataPath + "/Stages");


		//mapList Create
		GameObject scrollGrid = GameObject.Find ("ScrollGrid").gameObject;
		if (Directory.Exists(Application.persistentDataPath + "/Stages")) {
			scrollGrid.GetComponent<ScrollGrid> ().MakeFileList (Directory.GetFiles (Application.persistentDataPath + "/Stages", "*.stg"), true);
			scrollGrid.GetComponent<ScrollGrid> ().MakeFileList (Directory.GetFiles (Application.persistentDataPath + "/Stages", "*.txt"));
		}
	}

}
