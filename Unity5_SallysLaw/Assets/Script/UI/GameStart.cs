using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour {

	void OnClick()
	{
		Application.LoadLevel ("SelectStage");
		TimeMgr.Play ();
	}
}
