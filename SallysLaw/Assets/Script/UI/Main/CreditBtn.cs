using UnityEngine;
using System.Collections;

public class CreditBtn : MonoBehaviour {

	void OnClick()
	{
		if (GameMgr.getInstance.m_uiStatus == MainUIStatus.OPTION) {
			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
			StartCoroutine (MainScene.getInstance.ToCredit (true));
		}
	}
}
