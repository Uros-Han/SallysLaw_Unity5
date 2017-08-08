using UnityEngine;
using System.Collections;

public class FastForwardBtn : MonoBehaviour {

	SceneStatus sceneStatus;
	AudioSource audio;

	UISprite m_sprite;
	UIPanel m_FadePanel;
	StagePauseBtn PauseStatus;

	// Use this for initialization
	void Start () {
		sceneStatus = SceneStatus.getInstance;
		audio = GetComponent<AudioSource> ();
		audio.clip = AudioMgr.getInstance.m_sound_fastforward [1];
		audio.loop = true;

		#if UNITY_STANDALONE
		audio.volume = PlayerPrefs.GetFloat("SoundVolume");
		#endif

		if (sceneStatus.m_bMemoryStage)
			UIManager.getInstance.m_FastForwardPanel.transform.GetChild (0).GetComponent<UISprite> ().color = new Color (71 / 255f, 63 / 255f, 43 / 255f);
		else {
			UIManager.getInstance.m_FastForwardPanel.transform.GetChild (0).GetComponent<TweenAlpha> ().from = 0.1f;
			UIManager.getInstance.m_FastForwardPanel.transform.GetChild (0).GetComponent<UISprite> ().color = Color.white;
		}

		m_sprite = GetComponent<UISprite> ();
		m_FadePanel = GameObject.Find ("UIFadePanel").GetComponent<UIPanel> ();
		PauseStatus = GameObject.Find ("Pause").transform.GetChild (0).GetComponent<StagePauseBtn> ();

	}

	public void OnDisable()
	{
		if(TimeMgr.m_bFastForward)
			OnPress (false);

		TimeMgr.m_bFastForward = false;
		TimeMgr.Play ();
	}

#if UNITY_STANDALONE

	void Update()
	{
		if (!PauseStatus.m_bPauseOn) {
			if (Input.GetKeyDown (KeyCode.LeftShift)
		    #if UNITY_STANDALONE_WIN
			    || Input.GetKeyDown (KeyCode.JoystickButton1))
			#elif UNITY_STANDALONE_OSX
				|| Input.GetKeyDown (KeyCode.JoystickButton17))
			#else
			)
			#endif
				OnPress (true);
		
			if (Input.GetKeyUp (KeyCode.LeftShift)
		    #if UNITY_STANDALONE_WIN
			    || Input.GetKeyUp (KeyCode.JoystickButton1))
			#elif UNITY_STANDALONE_OSX
				|| 	Input.GetKeyUp (KeyCode.JoystickButton17))
			#else
				)
			#endif
				OnPress (false);
		}

	}

#endif
	
	public void OnPress(bool bPress)
	{
		GameMgr gMgr = GameMgr.getInstance;
		if ((gMgr.m_iCurChpt.Equals (1) && gMgr.m_iCurStage.Equals (1) && gMgr.m_iCurAct.Equals (1)) || ((gMgr.m_iCurChpt.Equals (1) && gMgr.m_iCurStage.Equals (1) && gMgr.m_iCurAct.Equals (2)) && GameObject.Find("FastForwardTuto").GetComponent<UIPanel>().alpha.Equals(0) ))
			return;

		if (SceneStatus.getInstance.m_enPlayerStatus.Equals (PLAYER_STATUS.SALLY_CRASH) && !bPress) {
			audio.Stop();
			AudioMgr.getInstance.PlaySfx(audio,"fastforward", 2);

			return;
		}

		if (bPress) {
			if (sceneStatus.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN)
			{
				TimeMgr.m_bFastForward = true;
				AudioMgr.getInstance.PlaySfx(audio,"fastforward", 0);
				TimeMgr.FastForward();


				if (!GameMgr.getInstance.m_bSoundMute)
				{
					audio.Play();
				}

				UIManager.getInstance.m_FastForwardPanel.GetComponent<UIPanel>().alpha = 1f;
				UIManager.getInstance.m_FastForwardPanel.transform.GetChild(0).GetComponent<TweenAlpha>().enabled = true;

				GetComponent<TweenAlpha>().enabled = false;

			}
		} else {

			if(!TimeMgr.m_bFastForward)
				return;

			TimeMgr.m_bFastForward = false;
			audio.Stop();
			AudioMgr.getInstance.PlaySfx(audio,"fastforward", 2);
			TimeMgr.Play();

			UIManager.getInstance.m_FastForwardPanel.GetComponent<UIPanel>().alpha = 0f;
			UIManager.getInstance.m_FastForwardPanel.transform.GetChild(0).GetComponent<TweenAlpha>().enabled = false;

			if(!(gMgr.m_iCurChpt.Equals(1) && gMgr.m_iCurStage.Equals(1) && gMgr.m_iCurAct.Equals(2)))
				return;

			if(GameObject.Find("FatherWaitPanel").transform.GetChild(2).GetComponent<UIPanel>().alpha != 0)
			{
				GetComponent<TweenAlpha>().ResetToBeginning();
				GetComponent<TweenAlpha>().Play();
			}

		}
	}


}
