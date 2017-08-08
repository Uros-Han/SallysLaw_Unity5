using UnityEngine;
using System.Collections;

public class Cursor_World : MonoBehaviour {

	public CURSOR_WORLD_STATE m_cursorWorld;
	public CURSOR_STAGE_STATE m_cursorStage;

	public float keyDelay = 0.1f;  // 0.1 second
	private float timePassed = 0f;

	TweenAlpha tAlpha;
	GameMgr gMgr;

	// Use this for initialization
	void Start () {
#if !(UNITY_STANDALONE || UNITY_WEBGL)
		gameObject.SetActive (false);
		return;
#endif

		m_cursorWorld = CURSOR_WORLD_STATE.CHPT1;
		m_cursorStage = CURSOR_STAGE_STATE.STAGE1;

		transform.localPosition = new Vector2(-900, 250);

		tAlpha = GetComponent<TweenAlpha> ();
		gMgr = GameMgr.getInstance;
	}

	void Update()
	{
		timePassed += Time.deltaTime;

		if (gMgr.m_uiStatus.Equals (MainUIStatus.WORLD)) {
			if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < 0) && timePassed >= keyDelay){
				Move_WORLDCursor (false);
				timePassed = 0f;
			}else if ((Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0) && timePassed >= keyDelay){
				Move_WORLDCursor (true);
				timePassed = 0f;
			}else if (Input.GetKeyUp (KeyCode.Return)
#if UNITY_STANDALONE_WIN
			          ||   Input.GetKeyUp (KeyCode.JoystickButton0))
#elif UNITY_STANDALONE_OSX
				|| Input.GetKeyUp (KeyCode.JoystickButton16))
#else
					)
#endif
				Select_WORLDCursor ();
		} 


		else if (gMgr.m_uiStatus.Equals (MainUIStatus.STAGE)) {
			if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < 0) && timePassed >= keyDelay){
				Move_STAGECursor (false);
				timePassed = 0f;
			}else if ((Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0) && timePassed >= keyDelay){
				Move_STAGECursor (true);
				timePassed = 0f;
			}else if (Input.GetKeyUp (KeyCode.Return) 
			          #if UNITY_STANDALONE_WIN
			          ||  Input.GetKeyUp (KeyCode.JoystickButton0))
				#elif UNITY_STANDALONE_OSX
				|| Input.GetKeyUp (KeyCode.JoystickButton16))
				#else
				)
				#endif
					Select_STAGECursor ();
		}
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	void Select_STAGECursor()
	{
		GameObject.Find ("Chapters").transform.GetChild ((int)m_cursorWorld - 1).GetChild (2).GetChild ((int)m_cursorStage).GetChild (0).GetComponent<StageButton> ().OnClick ();
	}

	void Select_WORLDCursor()
	{
		switch(m_cursorWorld)
		{
		case CURSOR_WORLD_STATE.PHOTO:
			GameObject.Find("LeftBottom").GetComponent<ToPhotoPanelBtn>().OnClick();
			return;

		case CURSOR_WORLD_STATE.CHPT1:
			GameObject.Find("1-").GetComponent<ChptButton>().OnClick();
			break;

		case CURSOR_WORLD_STATE.CHPT2:
			GameObject.Find("2-").GetComponent<ChptButton>().OnClick();
			break;

		case CURSOR_WORLD_STATE.CHPT3:
			GameObject.Find("3-").GetComponent<ChptButton>().OnClick();
			break;

		case CURSOR_WORLD_STATE.CHPT4:
			GameObject.Find("4-").GetComponent<ChptButton>().OnClick();
			break;

		case CURSOR_WORLD_STATE.CHPT5:
			GameObject.Find("5-").GetComponent<ChptButton>().OnClick();
			break;

		case CURSOR_WORLD_STATE.OPTION:
			GameObject.Find("RightTop").GetComponent<Option>().OnClick();
			return;
		}

		tAlpha.from = 1;
		tAlpha.to = 0;
		tAlpha.ResetToBeginning();
		tAlpha.Play(true);

		StartCoroutine (WaitToAlpha (true));
	}

	public void EscapeInStage()
	{
		tAlpha.from = 1;
		tAlpha.to = 0;
		tAlpha.ResetToBeginning();
		tAlpha.Play(true);
		StartCoroutine (WaitToAlpha (false));
	}

	IEnumerator WaitToAlpha(bool ToStage)
	{
		yield return new WaitForSeconds (1.0f);

		tAlpha.from = 0;
		tAlpha.to = 1;
		tAlpha.ResetToBeginning();
		tAlpha.Play(true);

		if (ToStage) {
			transform.localPosition = new Vector2 (-645, 130);
			m_cursorStage = CURSOR_STAGE_STATE.STAGE1;
		}else {
			switch(m_cursorWorld)
			{
			case CURSOR_WORLD_STATE.CHPT1:
				transform.localPosition = new Vector2(-900, 250);
				break;

			case CURSOR_WORLD_STATE.CHPT2:
				transform.localPosition = new Vector2(-450, 250);
				break;

			case CURSOR_WORLD_STATE.CHPT3:
				transform.localPosition = new Vector2(0, 250);
				break;

			case CURSOR_WORLD_STATE.CHPT4:
				transform.localPosition = new Vector2(450, 250);
				break;

			case CURSOR_WORLD_STATE.CHPT5:
				transform.localPosition = new Vector2(900, 250);
				break;
			}
		}
	}

	void Move_STAGECursor(bool bRight)
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		switch(m_cursorStage)
		{
		case CURSOR_STAGE_STATE.STAGE1:
			if(bRight){
				transform.localPosition = new Vector2(-395, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE2;
			}
			break;
			
		case CURSOR_STAGE_STATE.STAGE2:
			if(bRight){
				transform.localPosition = new Vector2(-145, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE3;
			}else{
				transform.localPosition = new Vector2(-645, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE1;
			}
			break;
			
		case CURSOR_STAGE_STATE.STAGE3:
			if(bRight){
				transform.localPosition = new Vector2(105, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE4;
			}else{
				transform.localPosition = new Vector2(-395, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE2;
			}
			break;
			
		case CURSOR_STAGE_STATE.STAGE4:
			if(bRight){
				transform.localPosition = new Vector2(355, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE5;
			}else{
				transform.localPosition = new Vector2(-145, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE3;
			}
			break;
			
		case CURSOR_STAGE_STATE.STAGE5:
			if(bRight){
				transform.localPosition = new Vector2(605, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE6;
			}else{
				transform.localPosition = new Vector2(105, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE4;
			}
			break;

		case CURSOR_STAGE_STATE.STAGE6:
			if(!bRight){
				transform.localPosition = new Vector2(355, 130);
				m_cursorStage = CURSOR_STAGE_STATE.STAGE5;
			}
			break;
		}
	}


	void Move_WORLDCursor(bool bRight)
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		switch(m_cursorWorld)
		{
		case CURSOR_WORLD_STATE.PHOTO:
			if(bRight){
				transform.localPosition = new Vector2(-900, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT1;
			}
			break;
			
		case CURSOR_WORLD_STATE.CHPT1:
			if(bRight){
				transform.localPosition = new Vector2(-450, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT2;
			}else{
				transform.position = GameObject.Find("LeftBottom").transform.position;
				transform.localPosition += new Vector3(0, 75f);
				m_cursorWorld = CURSOR_WORLD_STATE.PHOTO;
			}
			break;
			
		case CURSOR_WORLD_STATE.CHPT2:
			if(bRight){
				transform.localPosition = new Vector2(0, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT3;
			}else{
				transform.localPosition = new Vector2(-900, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT1;
			}
			break;
			
		case CURSOR_WORLD_STATE.CHPT3:
			if(bRight){
				transform.localPosition = new Vector2(450, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT4;
			}else{
				transform.localPosition = new Vector2(-450, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT2;
			}
			break;
			
		case CURSOR_WORLD_STATE.CHPT4:
			if(bRight){
				transform.localPosition = new Vector2(900, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT5;
			}else{
				transform.localPosition = new Vector2(0, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT3;
			}
			break;
			
		case CURSOR_WORLD_STATE.CHPT5:
			if(bRight){
				transform.position = GameObject.Find("RightTop").transform.position;
				transform.localPosition += new Vector3(0, 75f);
				m_cursorWorld = CURSOR_WORLD_STATE.OPTION;
			}else{
				transform.localPosition = new Vector2(450, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT4;
			}
			break;
			
		case CURSOR_WORLD_STATE.OPTION:
			if(!bRight){
				transform.localPosition = new Vector2(900, 250);
				m_cursorWorld = CURSOR_WORLD_STATE.CHPT5;
			}
			break;
		}
	}
}
