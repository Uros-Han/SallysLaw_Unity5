using UnityEngine;
using System.Collections;

public class OpenURL : MonoBehaviour {

	public string url = "";

	void OnClick()
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
		Application.OpenURL (url);
	}
}
