using UnityEngine;
using System.Collections;

public class StagePauseBtn : MonoBehaviour {


	public bool m_bPauseOn;

//	public bool m_bPauseBtn = true; // true -> pause, false -> resume


	public void OnClick()
	{
		if (transform.parent.parent.GetComponent<UIPanel> ().alpha.Equals (0))
			return;

		if (Application.loadedLevelName.Contains("Chapter") || Application.loadedLevelName.Contains("Memory") || Application.loadedLevelName == "StagePlayer" ) {

			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

			if(!m_bPauseOn && UIManager.getInstance.m_UIFadePanel.GetComponent<UIPanel>().alpha == 0)
			{
				#if UNITY_STANDALONE
				GameObject.Find("Cursor_pause").GetComponent<Cursor_Pause>().Init();
				#endif

				UIManager.getInstance.Pause (true);
				m_bPauseOn = true;
				//EffectManager.getInstance.SwitchBlur(true);
				TimeMgr.Pause();
			}
			else if(m_bPauseOn && UIManager.getInstance.m_UIFadePanel.GetComponent<UIPanel>().alpha == 0.6f)
			{
				UIManager.getInstance.Pause (false);
				m_bPauseOn = false;
				//EffectManager.getInstance.SwitchBlur(false);
				TimeMgr.Play();
			}


		} 

//		else {
//			GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().Back ();
//		}
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	public void RestartBtnPressed()
	{
		StartCoroutine (wait1Sec ());
	}

	IEnumerator wait1Sec()
	{
		yield return new WaitForSeconds (0.5f);
		m_bPauseOn = false;
	}


	void Update()
	{
		if (Input.GetKeyUp (KeyCode.Escape)
#if UNITY_STANDALONE_OSX
		    || Input.GetKeyUp (KeyCode.JoystickButton9))
#elif UNITY_STANDALONE_WIN
			|| Input.GetKeyUp (KeyCode.JoystickButton7))
#else
			)
#endif
			OnClick ();
	}
		

//	void SkipBtn()
//	{
//		GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_enPlayerStatus = PLAYER_STATUS.STAGE_CHG;
//		GameObject.Find ("UIManager").GetComponent<UIManager> ().CloseCurtain ();
//
//		transform.parent.GetChild (1).transform.localPosition = Vector2.zero;
//		transform.parent.GetChild (1).GetComponent<UISprite> ().spriteName = "icon_S_paused"; // chg sprite to pause
//
//		m_bPauseBtn = true;
//	}



	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}
}




