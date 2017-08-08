using UnityEngine;
using System.Collections;

public class Cursor_Option : MonoBehaviour {

	GameMgr gMgr;

	public CURSOR_OPTION_STATE m_CursorState;
	Option_PC option;


	// Use this for initialization
	void Start () {
		gMgr = GameMgr.getInstance;
		option = transform.parent.GetComponent<Option_PC> ();
	}

	public float keyDelay = 0.15f;  // 0.1 second
	private float timePassed = 0f;
	// Update is called once per frame
	void Update()
	{
		timePassed += Time.deltaTime;

		if (gMgr.m_uiStatus.Equals (MainUIStatus.OPTION)) {
			if ((Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("Vertical") < 0) && timePassed >= keyDelay){
				Move_Cursor (true);
				timePassed = 0f;
			}else if ((Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("Vertical") > 0) && timePassed >= keyDelay){
				Move_Cursor (false);
				timePassed = 0f;
			}else if ((Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0) && timePassed >= keyDelay)
			{
				ChgOption(true);
				timePassed = 0f;
			}
			else if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < 0) && timePassed >= keyDelay)
			{
				ChgOption(false);
				timePassed = 0f;
			}
			else if (Input.GetKeyUp (KeyCode.Return)
				#if UNITY_STANDALONE_WIN
				||  Input.GetKeyUp (KeyCode.JoystickButton0))
				#elif UNITY_STANDALONE_OSX
				||  Input.GetKeyUp (KeyCode.JoystickButton16))
				#else
			)
				#endif
			{
				if(transform.parent.Find("Apply").GetChild(0).GetChild(0).GetChild(0).GetComponent<UILabel>().text.Equals(Localization.Get("Apply")))
					ConfirmChange ();
				else if(m_CursorState.Equals(CURSOR_OPTION_STATE.CREDIT))
				{
					AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
					StartCoroutine (MainScene.getInstance.ToCredit (true));
				}else if(m_CursorState.Equals(CURSOR_OPTION_STATE.CONTROL)){
					AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
					StartCoroutine (MainScene.getInstance.ToControl (true));
				}
			}
		} 
	}

	void ChgOption(bool bRight)
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		switch(m_CursorState)
		{
		case CURSOR_OPTION_STATE.SOUND:
			if(bRight){
				if(PlayerPrefs.GetFloat ("SoundVolume") + 0.1f <= 1f)
					PlayerPrefs.SetFloat ("SoundVolume", PlayerPrefs.GetFloat ("SoundVolume") + 0.1f);
				else
					PlayerPrefs.SetFloat ("SoundVolume", 1f);
			}else{
				if(PlayerPrefs.GetFloat ("SoundVolume") - 0.1f >= 0f)
					PlayerPrefs.SetFloat ("SoundVolume", PlayerPrefs.GetFloat ("SoundVolume") - 0.1f);
				else
					PlayerPrefs.SetFloat ("SoundVolume", 0f);
			}
			transform.parent.Find ("Sound").GetChild (1).GetComponent<UISlider> ().value = PlayerPrefs.GetFloat ("SoundVolume");
			break;

		case CURSOR_OPTION_STATE.MUSIC:
			if(bRight){
				if(PlayerPrefs.GetFloat ("MusicVolume") + 0.1f <= 1f)
					PlayerPrefs.SetFloat ("MusicVolume", PlayerPrefs.GetFloat ("MusicVolume") + 0.1f);
				else
					PlayerPrefs.SetFloat ("MusicVolume", 1f);
			}else{
				if(PlayerPrefs.GetFloat ("MusicVolume") - 0.1f >= 0f)
					PlayerPrefs.SetFloat ("MusicVolume", PlayerPrefs.GetFloat ("MusicVolume") - 0.1f);
				else
					PlayerPrefs.SetFloat ("MusicVolume", 0f);
			}
			transform.parent.Find ("Music").GetChild (1).GetComponent<UISlider> ().value = PlayerPrefs.GetFloat ("MusicVolume");
			GameObject.Find ("BGM").GetComponent<AudioSource> ().volume = PlayerPrefs.GetFloat ("MusicVolume");
			break;

		case CURSOR_OPTION_STATE.RESOLUTION:
			if(bRight){
				if(option.m_iTmpResolutionIdx < Screen.resolutions.Length - 1)
					option.m_iTmpResolutionIdx += 1;
				else
					option.m_iTmpResolutionIdx = 0;
			}else{
				if(option.m_iTmpResolutionIdx > 0)
					option.m_iTmpResolutionIdx -= 1;
				else
					option.m_iTmpResolutionIdx = Screen.resolutions.Length - 1;
			}
			transform.parent.Find ("Resolution").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = string.Format("{0}x{1}",Screen.resolutions[option.m_iTmpResolutionIdx].width,Screen.resolutions[option.m_iTmpResolutionIdx].height);

			if(option.m_iTmpResolutionIdx != option.m_iCurResolutionIdx)
				transform.parent.Find ("Resolution").GetChild (0).GetComponent<UILabel>().text = Localization.Get("Resolution") + "*";
			else
				transform.parent.Find ("Resolution").GetChild (0).GetComponent<UILabel>().text = Localization.Get("Resolution");

			break;

		case CURSOR_OPTION_STATE.SCREEN_MODE:
			if(option.m_bTmpFullScreen){
				option.m_bTmpFullScreen = false;
				transform.parent.Find ("WindowMode").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = Localization.Get("Window");
			}else{
				option.m_bTmpFullScreen = true;
				transform.parent.Find ("WindowMode").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = Localization.Get("FullScreen");
			}

			if(option.m_bTmpFullScreen != option.m_bCurFullScreen)
				transform.parent.Find ("WindowMode").GetChild (0).GetComponent<UILabel>().text = Localization.Get("ScreenMode") + "*";
			else
				transform.parent.Find ("WindowMode").GetChild (0).GetComponent<UILabel>().text = Localization.Get("ScreenMode");

			break;

		case CURSOR_OPTION_STATE.LANGUAGE:
			if(bRight){
				if(option.m_iTmpLang < Localization.knownLanguages.Length - 1)
					option.m_iTmpLang += 1;
				else
					option.m_iTmpLang = 0;
			}else{
				if(option.m_iTmpLang > 0)
					option.m_iTmpLang -= 1;
				else
					option.m_iTmpLang = Localization.knownLanguages.Length - 1;
			}

			switch(Localization.knownLanguages[option.m_iTmpLang])
			{
			case "English":
				transform.parent.Find ("Lang").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = "English";
				break;

			case "Korean":
				transform.parent.Find ("Lang").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = "한국어";
				break;

			case "SChinese":
				transform.parent.Find ("Lang").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = "中文简体";
				break;

			case "TChinese":
				transform.parent.Find ("Lang").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = "中文繁體";
				break;

			case "German":
				transform.parent.Find ("Lang").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = "Deutsch";
				break;

			case "Spanish":
				transform.parent.Find ("Lang").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = "Español";
				break;

			case "Japanese":
				transform.parent.Find ("Lang").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = "日本語";
				break;

			default:
				Debug.LogError("unknown langugae");
				break;
			}


			if(option.m_iTmpLang != option.m_iCurLang)
				transform.parent.Find ("Lang").GetChild (0).GetComponent<UILabel>().text = Localization.Get("Lan") + "*";
			else
				transform.parent.Find ("Lang").GetChild (0).GetComponent<UILabel>().text = Localization.Get("Lan");

			SetFont_Cursor(option.m_iTmpLang);

			break;
		}
	}

	void SetFont_Cursor(int iTmpLang, bool bConfirmChg = false)
	{
		FontType tmpFont = FontType.END;
		switch(iTmpLang)
		{
		case 0:
			tmpFont = FontType.Londrina;
			break;
		case 1:
			tmpFont = FontType.Jua;
			break;
		case 2:
			tmpFont = FontType.SystemFont;
			break;
		case 3:
			tmpFont = FontType.SystemFont;
			break;
		case 4:
			tmpFont = FontType.Londrina;
			break;
		case 5:
			tmpFont = FontType.Londrina;
			break;
		case 6:
			tmpFont = FontType.GenJyuu;
			break;
		default:
			Debug.LogError("Lang Font error");
			break;
		}

		transform.parent.Find ("Lang").GetChild (1).GetChild (1).SendMessage ("SwitchFont", tmpFont, SendMessageOptions.RequireReceiver);

		if (bConfirmChg) {
			GameObject.Find ("UI Root").BroadcastMessage ("SwitchFont", tmpFont, SendMessageOptions.DontRequireReceiver);
		}
	}

	void Move_Cursor(bool bDown)
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		switch(m_CursorState)
		{
		case CURSOR_OPTION_STATE.SOUND:
			if(bDown){
				transform.localPosition = new Vector2(-550, 205);
				m_CursorState = CURSOR_OPTION_STATE.MUSIC;
			}
			break;

		case CURSOR_OPTION_STATE.MUSIC:
			if(bDown){
				transform.localPosition = new Vector2(-550, 105);
				m_CursorState = CURSOR_OPTION_STATE.RESOLUTION;
			}else{
				transform.localPosition = new Vector2(-550, 305);
				m_CursorState = CURSOR_OPTION_STATE.SOUND;
			}
			break;

		case CURSOR_OPTION_STATE.RESOLUTION:
			if(bDown){
				transform.localPosition = new Vector2(-550, 5);
				m_CursorState = CURSOR_OPTION_STATE.SCREEN_MODE;
			}else{
				transform.localPosition = new Vector2(-550, 205);
				m_CursorState = CURSOR_OPTION_STATE.MUSIC;
			}
			break;

		case CURSOR_OPTION_STATE.SCREEN_MODE:
			if(bDown){
				transform.localPosition = new Vector2(-550, -95);
				m_CursorState = CURSOR_OPTION_STATE.LANGUAGE;
			}else{
				transform.localPosition = new Vector2(-550, 105);
				m_CursorState = CURSOR_OPTION_STATE.RESOLUTION;
			}
			break;

		case CURSOR_OPTION_STATE.LANGUAGE:
			if(bDown){
				transform.localPosition = new Vector2(-550, -195);
				m_CursorState = CURSOR_OPTION_STATE.CONTROL;
			}else{
				transform.localPosition = new Vector2(-550, 5);
				m_CursorState = CURSOR_OPTION_STATE.SCREEN_MODE;
			}
			break;

		case CURSOR_OPTION_STATE.CONTROL:
			if(bDown){
				transform.localPosition = new Vector2(-550, -295);
				m_CursorState = CURSOR_OPTION_STATE.CREDIT;
			}else{
				transform.localPosition = new Vector2(-550, -95);
				m_CursorState = CURSOR_OPTION_STATE.LANGUAGE;
			}
			break;

		case CURSOR_OPTION_STATE.CREDIT:
			if(!bDown){
				transform.localPosition = new Vector2(-550, -195);
				m_CursorState = CURSOR_OPTION_STATE.CONTROL;
			}
			break;
		}
	}

	void ConfirmChange()
	{
		option.m_iCurResolutionIdx = option.m_iTmpResolutionIdx;
		option.m_bCurFullScreen = option.m_bTmpFullScreen;
		option.m_iCurLang = option.m_iTmpLang;

		Screen.SetResolution (Screen.resolutions [option.m_iCurResolutionIdx].width, Screen.resolutions [option.m_iCurResolutionIdx].height, option.m_bCurFullScreen);
		Localization.language = Localization.knownLanguages[option.m_iCurLang];
		Debug.Log (option.m_iCurLang);
		PlayerPrefs.SetInt ("Lang", option.m_iCurLang);

		if (option.m_bCurFullScreen)
			PlayerPrefs.SetInt ("FullScreen", 1);
		else
			PlayerPrefs.SetInt ("FullScreen", 0);

		PlayerPrefs.SetInt ("Resolution", option.m_iCurResolutionIdx);

		UICamera.mainCamera.orthographicSize = (Screen.resolutions [option.m_iCurResolutionIdx].height / 760f) * (1280f / Screen.resolutions [option.m_iCurResolutionIdx].width);

		transform.parent.Find ("Resolution").GetChild (0).GetComponent<UILabel>().text = Localization.Get("Resolution");
		transform.parent.Find ("WindowMode").GetChild (0).GetComponent<UILabel>().text = Localization.Get("ScreenMode");
		transform.parent.Find ("Lang").GetChild (0).GetComponent<UILabel>().text = Localization.Get("Lan");

		SetFont_Cursor (option.m_iCurLang, true);
	}
}