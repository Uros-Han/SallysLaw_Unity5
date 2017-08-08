using UnityEngine;
using System.Collections;

public class PlayBtn : MonoBehaviour {



	void OnClick()
	{
		GameObject tmpErrMsgPrf = Resources.Load ("Prefabs/UI/mapToolErrorMsg") as GameObject;
		GameObject tmpErrMsg;

		if (!GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bNowPlaying) {
			if (GameObject.Find ("Runner(Clone)") == null) {
				tmpErrMsg = Instantiate (tmpErrMsgPrf) as GameObject;
				tmpErrMsg.GetComponent<UILabel> ().text = "Runner Need!";
			} else if (GameObject.Find ("Guardian(Clone)") == null) {
				tmpErrMsg = Instantiate (tmpErrMsgPrf) as GameObject;
				tmpErrMsg.GetComponent<UILabel> ().text = "Guardian Need!";
			} else if (GameObject.Find ("R_Goal(Clone)") == null) {
				tmpErrMsg = Instantiate (tmpErrMsgPrf) as GameObject;
				tmpErrMsg.GetComponent<UILabel> ().text = "Goal Need!";
			} else { // Play
				//transform.parent.parent.parent.gameObject.SetActive(false);
				GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().Play (true);

				transform.parent.GetChild(1).GetComponent<UISprite>().spriteName = "icon_S_paused";
				transform.parent.GetChild(1).localPosition = Vector3.zero;
			}
		} else {
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().Play (false);
		}
	}
}
