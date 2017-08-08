using UnityEngine;
using System.Collections;

public class LoadSceneBtn : MonoBehaviour {
	public string m_SceneName;

	void OnClick()
	{
		if(GameObject.Find("SFX") != null)
			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
		StartCoroutine (Delay ());
	}

	IEnumerator Delay()
	{
		TimeMgr.Play ();
		//yield return new WaitForSeconds(0.5f);
		Application.LoadLevel (m_SceneName);
		yield return null;
	}
}
