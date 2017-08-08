using UnityEngine;
using System.Collections;

public class Cursor_Quit : MonoBehaviour {

	enum CURSOR_QUIT_STATE { YES, NO };

	CURSOR_QUIT_STATE m_cursorState;
	GameMgr gMgr;
	// Use this for initialization
	void Start () {
		m_cursorState = CURSOR_QUIT_STATE.YES;

		gMgr = GameMgr.getInstance;
	}

	public float keyDelay = 0.15f;  // 0.1 second
	private float timePassed = 0f;
	// Update is called once per frame
	void Update () {
		timePassed += Time.unscaledDeltaTime;

		if (gMgr.m_uiStatus.Equals (MainUIStatus.REALLY_QUIT)) {
			if ((Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("Vertical") < 0) && timePassed >= keyDelay){
				Move_Cursor (true);
				timePassed = 0f;
			}else if ((Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("Vertical") > 0) && timePassed >= keyDelay){
				Move_Cursor (false);
				timePassed = 0f;
			}else if (Input.GetKeyUp (KeyCode.Return)
		          #if UNITY_STANDALONE_WIN
		          ||  Input.GetKeyUp (KeyCode.JoystickButton0))
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

	void Move_Cursor(bool bDown)
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		switch(m_cursorState)
		{
		case CURSOR_QUIT_STATE.YES:
			if(bDown){
				transform.localPosition = new Vector2(-115, -120);
				m_cursorState = CURSOR_QUIT_STATE.NO;
			}
			break;

		case CURSOR_QUIT_STATE.NO:
			if(!bDown){
				transform.localPosition = new Vector2(-115, 5);
				m_cursorState = CURSOR_QUIT_STATE.YES;
			}
			break;
		}
	}

	void Select()
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		switch(m_cursorState)
		{
		case CURSOR_QUIT_STATE.YES:
			Application.Quit();
			break;
			
		case CURSOR_QUIT_STATE.NO:
#if UNITY_STANDALONE
			if(PC_InputControl.getInstance.GetInputState() == PC_InputControl.eInputState.MouseKeyboard)
				GameObject.Find("LeftTop_Keyboard").GetComponent<MainSceneBack>().OnClick();
			else
				GameObject.Find("LeftTop_Joystick").GetComponent<MainSceneBack>().OnClick();
#else
			GameObject.Find("LeftTop_Mobile").GetComponent<MainSceneBack>().OnClick();
#endif
			break;
		}
	}
}
