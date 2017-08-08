using UnityEngine;
using System.Collections;

public class Cursor_Pause : MonoBehaviour {

	UIPanel panel;

	enum CURSOR_PAUSE_STATE { RESUME, RETRY, EXIT };

	CURSOR_PAUSE_STATE m_cursorState;

	// Use this for initialization
	void Start () {
		panel = transform.parent.parent.GetComponent<UIPanel> ();
		m_cursorState = CURSOR_PAUSE_STATE.RESUME;
	}

	public float keyDelay = 0.15f;  // 0.1 second
	private float timePassed = 0f;
	// Update is called once per frame
	void Update () {
		if (panel.alpha > 0.5f) {
			timePassed += Time.unscaledDeltaTime;

			if ((Input.GetKey(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") < 0) && timePassed >= keyDelay){
				Move_Cursor (true);
				timePassed = 0f;
			}else if ((Input.GetKey(KeyCode.UpArrow) || Input.GetAxisRaw("Vertical") > 0) && timePassed >= keyDelay){
				Move_Cursor (false);
				timePassed = 0f;
			}else if (Input.GetKeyUp (KeyCode.Return)
		          #if UNITY_STANDALONE_WIN
		          || Input.GetKeyUp (KeyCode.JoystickButton0))
				#elif UNITY_STANDALONE_OSX
				|| Input.GetKeyUp (KeyCode.JoystickButton16))
				#else
				)
					#endif
			{
				Select();
			}
		}
	}

	public void Init()
	{
		m_cursorState = CURSOR_PAUSE_STATE.RESUME;
		transform.localPosition = new Vector2 (-253f, 250f);
	}

	void Select()
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		switch(m_cursorState)
		{
		case CURSOR_PAUSE_STATE.RESUME:
			GameObject.Find("Sprite_Dontmove").GetComponent<StagePauseBtn>().OnClick();
			break;

		case CURSOR_PAUSE_STATE.RETRY:
			if (SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.RUNNER || SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN) {
				GameMgr.getInstance.GameOver ();
				UIManager.getInstance.Pause (false);
				AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
				GameObject.Find("Pause").transform.GetChild(0).GetComponent<StagePauseBtn>().RestartBtnPressed();
			}
			break;

		case CURSOR_PAUSE_STATE.EXIT:
			LoadMainScene ();
			break;
		}
	}


	void Move_Cursor(bool bDown)
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		switch(m_cursorState)
		{
		case CURSOR_PAUSE_STATE.RESUME:
			if(bDown){
				transform.localPosition = new Vector3(-253, 0);
				m_cursorState = CURSOR_PAUSE_STATE.RETRY;
			}
			break;

		case CURSOR_PAUSE_STATE.RETRY:
			if(bDown){
				transform.localPosition = new Vector3(-253, -250);
				m_cursorState = CURSOR_PAUSE_STATE.EXIT;
			}else{
				transform.localPosition = new Vector3(-253, 250);
				m_cursorState = CURSOR_PAUSE_STATE.RESUME;
			}
			break;

		case CURSOR_PAUSE_STATE.EXIT:
			if(!bDown){
				transform.localPosition = new Vector3(-253, 0);
				m_cursorState = CURSOR_PAUSE_STATE.RETRY;
			}
			break;
		}
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
