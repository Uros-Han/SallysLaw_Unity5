using UnityEngine;
using System.Collections;

public class InGameExit : MonoBehaviour {	
	void OnClick()
	{
		if(GameObject.Find("SFX") != null)
			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		LoadMainScene ();
	}

	public void LoadMainScene()
	{
		TweenAlpha m_UIFadeAlpha = UIManager.getInstance.m_UIFadePanel.GetComponent<TweenAlpha> ();
		m_UIFadeAlpha.duration = 1f;
		
		m_UIFadeAlpha.delay = 0f;
		m_UIFadeAlpha.from = UIManager.getInstance.m_UIFadePanel.GetComponent<UIPanel>().alpha;
		m_UIFadeAlpha.to = 1f;
		m_UIFadeAlpha.ResetToBeginning ();
		m_UIFadeAlpha.Play (true);

		GameObject.Find ("PausePanel").SendMessage ("TweenActivate", false);
		StartCoroutine(AudioMgr.getInstance.VolumeChg (false));

		StartCoroutine (LoadMain ());
	}
	
	IEnumerator LoadMain()
	{
		yield return StartCoroutine(waitForUnscaledSecond (1.5f));
		
		TimeMgr.Play ();
		Application.LoadLevel ("Main");
	}


	IEnumerator waitForUnscaledSecond(float fTime)
	{
		float fTmpTime = 0f;
		
		do{
			if(TimeMgr.m_bFastForward)
				fTmpTime += (Time.unscaledDeltaTime * 3f);
			else
				fTmpTime += Time.unscaledDeltaTime;
			
			yield return null;
		}while(fTmpTime < fTime);
		
	}
}
