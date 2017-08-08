using UnityEngine;
using System.Collections;

public class Option : MonoBehaviour {

	public void OnClick()
	{
		StartCoroutine(MainScene.getInstance.ToOption (true));
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
	}

}
