using UnityEngine;
using System.Collections;

public class LanguageBtn : MonoBehaviour {

	void OnClick()
	{
		if (GameMgr.getInstance.m_uiStatus == MainUIStatus.OPTION) {
			StartCoroutine (MainScene.getInstance.ToLanguage (true));
			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
		}
	}
}
