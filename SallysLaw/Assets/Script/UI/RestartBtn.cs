using UnityEngine;
using System.Collections;

public class RestartBtn : MonoBehaviour {

	void OnClick()
	{
		if (SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.RUNNER || SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN) {
			GameMgr.getInstance.GameOver ();
			UIManager.getInstance.Pause (false);
			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
			GameObject.Find("Pause").transform.GetChild(0).GetComponent<StagePauseBtn>().RestartBtnPressed();
		}
	}
}
