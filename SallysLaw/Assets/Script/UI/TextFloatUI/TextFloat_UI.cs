using UnityEngine;
using System.Collections;

public class TextFloat_UI : MonoBehaviour {
	public bool m_bFatherText;
	public bool m_bMemoryText;
//	bool m_bShow; //알파값1 되면 true

	// Use this for initialization

	void Awake()
	{
		if (!StageLoader.getInstance.m_bMaptool)
			transform.GetChild(1).GetComponent<UIInput>().enabled = false;
	}

	void Start () {

		if (!StageLoader.getInstance.m_bMaptool) {
			GetComponent<UIPanel> ().alpha = 0f;

			StartCoroutine (AlphaController ());

			transform.GetChild (0).GetComponent<BoxCollider>().enabled = false;
			transform.GetChild (1).GetComponent<BoxCollider>().enabled = false;
		} else {
			GetComponent<UIPanel> ().alpha = 1f;

			transform.localScale = new Vector2(0.5f, 0.5f);

			transform.GetChild (0).GetComponent<UIAnchor> ().runOnlyOnce = false;
			transform.GetChild (0).GetComponent<UIAnchor> ().enabled = true; 
		}

		if (!SceneStatus.getInstance.m_bMemoryStage) {
			if (!m_bFatherText) {
				transform.GetChild (0).GetComponent<UISprite> ().color = Color.black;
				transform.GetChild (1).GetComponent<UILabel> ().color = Color.black;
			}
		} else {
			transform.GetChild (0).GetComponent<UISprite> ().color = new Color(71/255f, 63/255f, 43/255f);
			transform.GetChild (1).GetComponent<UILabel> ().color = new Color(71/255f, 63/255f, 43/255f);
		}

		if (SceneStatus.getInstance.m_bFinaleStage) {
			transform.localScale = new Vector2 (0.714f, 0.714f);

			if (m_bFatherText) {
				transform.GetChild (0).GetComponent<UISprite> ().color = new Color(89/255f, 112/255f, 65/255f);
				transform.GetChild (1).GetComponent<UILabel> ().color = new Color(89/255f, 112/255f, 65/255f);
			}else{
				transform.GetChild (0).GetComponent<UISprite> ().color = new Color(21/255f, 41/255f, 19/255f);
				transform.GetChild (1).GetComponent<UILabel> ().color = new Color(21/255f, 41/255f, 19/255f);
			}
		}

		SetLabelKey ();
	}

	IEnumerator PC_TutorialText(string strKey)
	{
//		transform.GetChild (1).GetComponent<UILocalize> ().key = strKey + "_PC";

		transform.GetChild (1).GetComponent<UILabel> ().text = string.Format(Localization.Get(strKey + "_PC"), "      ");

		transform.GetChild(2).gameObject.SetActive(true);
//		transform.GetChild (0).GetComponent<UIAnchor> ().enabled = false;
//		transform.GetChild (1).GetComponent<UILabel>().pivot = UIWidget.Pivot.Left;


//		transform.GetChild (1).localPosition = new Vector2(-61f, 0);

		yield return null;
		switch (Localization.language) {
		case "中文简体":
			transform.GetChild(2).GetChild(0).localPosition = new Vector2(105, 0); //keyboard
			transform.GetChild(2).GetChild(1).localPosition = new Vector2(53.5f, -5); //stick
			break;

		case "中文繁體":
			transform.GetChild(2).GetChild(0).localPosition = new Vector2(105, 0);
			transform.GetChild(2).GetChild(1).localPosition = new Vector2(53.5f, -5);
			break;

		default:
			break;
		}

//		transform.GetChild (0).localPosition = new Vector2(0, 40f);

		if(PC_InputControl.getInstance.GetInputState() == PC_InputControl.eInputState.Controler)
			GameObject.Find("UI Root").BroadcastMessage("SwapController", false ,SendMessageOptions.DontRequireReceiver);
	}


	void OnDestroy()
	{
		StopAllCoroutines ();
	}
	
	public void SetLabelKey()
	{
		string strKey = transform.parent.GetComponent<UIFollowTarget> ().target.GetComponent<TextFloat_Pos> ().m_strKey;

		if (!StageLoader.getInstance.m_bMaptool) {

			if(strKey.Equals("sallysLaw"))
			{
				transform.GetChild (0).GetComponent<UISprite>().enabled = false;
				transform.GetChild (1).GetComponent<UISprite>().enabled = true;
				transform.GetChild (1).GetComponent<UILabel> ().enabled = false;
				transform.GetChild (1).GetComponent<UISprite>().spriteName = Localization.Get("Logo");
				transform.GetChild (1).GetComponent<UISprite>().MakePixelPerfect();
				transform.GetChild (1).localScale = new Vector2(0.25f, 0.25f);
			}else if(strKey.Equals("1-01_02_S01") || strKey.Equals("5-01_02_S01")){
#if UNITY_STANDALONE
				StartCoroutine(PC_TutorialText(strKey));
#else
				transform.GetChild (1).GetComponent<UILocalize> ().key = strKey;
#endif
			}else
				transform.GetChild (1).GetComponent<UILocalize> ().key = strKey;




			if (strKey.Equals ("3-02_01_F01") || strKey.Equals ("3-02_01_F02")) {
				transform.GetChild (0).GetComponent<UISprite> ().color = new Color (4 / 255f, 11 / 255f, 28 / 255f);
				transform.GetChild (1).GetComponent<UILabel> ().color = new Color (4 / 255f, 11 / 255f, 28 / 255f);
			}else if (strKey.Equals ("5-01_03_F02") || strKey.Equals ("5-01_03_F03") || strKey.Equals ("5-01_03_F04")) {
				transform.GetChild (0).GetComponent<UISprite> ().color = new Color (4 / 255f, 11 / 255f, 28 / 255f);
				transform.GetChild (1).GetComponent<UILabel> ().color = new Color (4 / 255f, 11 / 255f, 28 / 255f);
			}else if (strKey.Equals ("5-04_01_F04")) {
				transform.GetChild (0).GetComponent<UISprite> ().color = new Color (4 / 255f, 11 / 255f, 28 / 255f);
				transform.GetChild (1).GetComponent<UILabel> ().color = new Color (4 / 255f, 11 / 255f, 28 / 255f);
			}else if (strKey.Equals ("4-01_01_F03") || strKey.Equals ("4-01_01_F04")) {
				transform.GetChild (0).GetComponent<UISprite> ().color = Color.white;
				transform.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			}else if (strKey.Equals ("1-01_03_F02") || strKey.Equals ("1-01_03_F03")) {
				transform.GetChild (0).GetComponent<UISprite> ().color = Color.white;
				transform.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			}else if (strKey.Equals ("2-01_02_F03")) {
				transform.GetChild (0).GetComponent<UISprite> ().color = Color.white;
				transform.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			}


		}
		else {
			Destroy (transform.GetChild (1).GetComponent<UILocalize>());
			transform.GetChild (1).GetComponent<UILabel> ().text = strKey;
		}
		transform.GetChild (0).GetComponent<UIAnchor> ().enabled = true;
	

//		transform.GetChild (0).localPosition = new Vector2 (0,0.25f);
	}

	public void ChgTextColorToFather()
	{
		if (!m_bFatherText && !SceneStatus.getInstance.m_bMemoryStage) {
			transform.GetChild (0).GetComponent<UISprite> ().color = Color.white;
			transform.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			transform.GetChild (1).GetComponent<UISprite>().color = Color.white;
		}

	}

	IEnumerator AlphaController()
	{
		Transform SallyTrans = null;
		Transform FatherTrans = null;
		GameObject SallyObj = null;
		GameObject FatherObj = null;

		if (m_bFatherText) {
			FatherObj = GameObject.Find ("Guardian(Clone)");
			FatherTrans = FatherObj.transform;
		} else {
			SallyObj = GameObject.Find ("Runner(Clone)");
			SallyTrans = SallyObj.transform;
		}

		Transform TargetTrans = transform.parent.GetComponent<UIFollowTarget> ().target;
		UIPanel panel = GetComponent<UIPanel> ();

		float fFadeSpeed = 0.01f;
		float fFadeDistance = 3.25f;

		float fMaxAlpha = 1f;

		SceneStatus sceneStatus = SceneStatus.getInstance;


		panel.alpha = 0f;
		do {

//			if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.STAGE_CHG_SALLY) || sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.STAGE_CHG_FATHER))
//			{
//				panel.alpha = 0f;
//			}

			if(m_bMemoryText)
			{
				
				fMaxAlpha = 0.2f;
				
				if(m_bFatherText)
				{
					if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
					{
						if(Vector2.Distance(FatherTrans.position , TargetTrans.position) < fFadeDistance && FatherObj.activeInHierarchy && panel.alpha != fMaxAlpha)
						{
							if(panel.alpha == 0f)
							{
								AudioMgr.getInstance.PlaySfx(FatherTrans.GetComponent<AudioSource>(), "ui_bundle", (int)UI_SOUND_LIST.TEXT_IN);
								panel.alpha += fFadeSpeed;
							}else if(panel.alpha < fMaxAlpha)
							{
								panel.alpha += fFadeSpeed;
							}else{
								panel.alpha = fMaxAlpha;
							}
							
						}else if(Vector2.Distance(FatherTrans.position , TargetTrans.position) > fFadeDistance && panel.alpha != 0f){
							if(panel.alpha.Equals(fMaxAlpha))
							{
								AudioMgr.getInstance.PlaySfx(FatherTrans.GetComponent<AudioSource>(), "ui_bundle", (int)UI_SOUND_LIST.TEXT_OUT);
								panel.alpha -= fFadeSpeed;
							}else if(panel.alpha > 0f)
							{
								panel.alpha -= fFadeSpeed;
							}else{
								panel.alpha = 0f;
							}
						}
					}else if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_CRASH))
					{
						panel.alpha = 0f;
					}
				}else{
					
					if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER) || sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
					{
						if(Vector2.Distance(SallyTrans.position , TargetTrans.position) < fFadeDistance && SallyObj.activeInHierarchy && panel.alpha != fMaxAlpha)
						{
							if(panel.alpha == 0f)
							{
								AudioMgr.getInstance.PlaySfx(SallyTrans.GetComponent<AudioSource>(), "ui_bundle", (int)UI_SOUND_LIST.TEXT_IN);
								panel.alpha += fFadeSpeed;
							}
							else if(panel.alpha < fMaxAlpha)
							{
								panel.alpha += fFadeSpeed;
							}else{
								panel.alpha = fMaxAlpha;
							}
						}else if(Vector2.Distance(SallyTrans.position , TargetTrans.position) > fFadeDistance && panel.alpha != 0f){
							if(panel.alpha.Equals(fMaxAlpha))
							{
								AudioMgr.getInstance.PlaySfx(SallyTrans.GetComponent<AudioSource>(), "ui_bundle", (int)UI_SOUND_LIST.TEXT_OUT);
								panel.alpha -= fFadeSpeed;
							}else if(panel.alpha > 0f)
							{
								panel.alpha -= fFadeSpeed;
							}else{
								panel.alpha = 0f;
							}
						}
					}else if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_CRASH))
					{
						panel.alpha = 0f;
					}
				}
				
			}else{
				if(m_bFatherText)
				{
					if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN) || (sceneStatus.m_bFinaleStage && sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER)))
					{
						if(Vector2.Distance(FatherTrans.position , TargetTrans.position) < fFadeDistance && FatherObj.activeInHierarchy && panel.alpha != 1f)
						{
							if(panel.alpha == 0f)
							{
								AudioMgr.getInstance.PlaySfx(FatherTrans.GetComponent<AudioSource>(), "ui_bundle", (int)UI_SOUND_LIST.TEXT_IN);
								panel.alpha += fFadeSpeed;
							}else if(panel.alpha < 1f)
							{
								panel.alpha += fFadeSpeed;
							}else{
								panel.alpha = 1f;
							}

						}else if(Vector2.Distance(FatherTrans.position , TargetTrans.position) > fFadeDistance && panel.alpha != 0f){
							if(panel.alpha.Equals(fMaxAlpha))
							{
								AudioMgr.getInstance.PlaySfx(FatherTrans.GetComponent<AudioSource>(), "ui_bundle", (int)UI_SOUND_LIST.TEXT_OUT);
								panel.alpha -= fFadeSpeed;
							}else if(panel.alpha > 0f)
							{
								panel.alpha -= fFadeSpeed;
							}else{
								panel.alpha = 0f;
							}
						}
					}else if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_CRASH))
					{
						panel.alpha = 0f;
					}
				}else{
					if(fMaxAlpha.Equals(1f) && SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN)
						fMaxAlpha = 0.3f;

					string strKey = transform.parent.GetComponent<UIFollowTarget> ().target.GetComponent<TextFloat_Pos> ().m_strKey;
					if(strKey.Equals("sallysLaw"))
						fMaxAlpha = 1f;

					if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER) || sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
					{
						if(Vector2.Distance(SallyTrans.position , TargetTrans.position) < fFadeDistance && SallyObj.activeInHierarchy && panel.alpha != fMaxAlpha)
						{
							if(panel.alpha == 0f)
							{
								AudioMgr.getInstance.PlaySfx(SallyTrans.GetComponent<AudioSource>(), "ui_bundle", (int)UI_SOUND_LIST.TEXT_IN);
								panel.alpha += fFadeSpeed;
							}
							else if(panel.alpha < fMaxAlpha)
							{
								panel.alpha += fFadeSpeed;
							}else{
								panel.alpha = fMaxAlpha;
							}
						}else if(Vector2.Distance(SallyTrans.position , TargetTrans.position) > fFadeDistance && panel.alpha != 0f){
							if(panel.alpha.Equals(fMaxAlpha))
							{
								AudioMgr.getInstance.PlaySfx(SallyTrans.GetComponent<AudioSource>(), "ui_bundle", (int)UI_SOUND_LIST.TEXT_OUT);
								panel.alpha -= fFadeSpeed;
							}else if(panel.alpha > 0f)
							{
								panel.alpha -= fFadeSpeed;
							}else{
								panel.alpha = 0f;
							}
						}
					}else if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_CRASH))
					{
						panel.alpha = 0f;
					}
				}
			}

			yield return new WaitForFixedUpdate();
		} while(true);
	}
}
