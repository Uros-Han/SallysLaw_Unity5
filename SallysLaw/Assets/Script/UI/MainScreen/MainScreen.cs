using UnityEngine;
using System.Collections;

public class MainScreen : MonoBehaviour {

	void Awake () {
		if(GameObject.Find("GameMgr") == null) //if gameMgr doesn't exist, make one.
		{
			GameObject gameMgr = Instantiate(Resources.Load("Prefabs/GameMgr") as GameObject) as GameObject;
			gameMgr.name = gameMgr.name.Replace("(Clone)","");
		}

		Time.timeScale = 1f;
	}
}
