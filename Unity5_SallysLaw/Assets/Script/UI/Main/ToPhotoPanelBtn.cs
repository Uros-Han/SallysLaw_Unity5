using UnityEngine;
using System.Collections;

public class ToPhotoPanelBtn : MonoBehaviour {

	public void OnClick()
	{
		StartCoroutine(MainScene.getInstance.ToPhoto (true));

		GameMgr.getInstance.SendMessage("NotiAlbum", false, SendMessageOptions.DontRequireReceiver);
		GameObject.Find ("LeftBottom").transform.GetChild (0).GetComponent<NotifyIcon> ().NotyRefresh ();
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
	}
}
