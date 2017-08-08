using UnityEngine;
using System.Collections;

#if UNITY_STANDALONE
using Steamworks;
#endif

public class UIPhoto : MonoBehaviour {

	public int iPhotoIdx;
	public int iNeedPhotoCount;

	public bool m_bInApp;
	public bool m_bAllCollected;

	public string m_strSkinName;

	public bool m_bSelected;
	bool checkOnce;

	GameMgr gameMgr;
	UICenterOnChild centerChild;
	// Use this for initialization
	void Start () {

		gameMgr = GameMgr.getInstance;

		PhotoCheck ();

		if (!m_bInApp && PlayerPrefs.GetInt (string.Format ("PhotoAllCollected{0}", iPhotoIdx)).Equals (1))
			PhotoCollectCheck ();

		centerChild = transform.parent.GetComponent<UICenterOnChild> ();

		if (gameMgr.m_strCurSkin.Equals (m_strSkinName)) {
			OnClick();
		}

		if(m_bInApp)
		{
			bool bGotAchiv = BookAchivCheck();
			
			if(!bGotAchiv)
			{
				transform.GetChild(2).gameObject.SetActive(true);
				transform.GetChild(2).GetComponent<UILabel>().text = string.Format(Localization.Get("PC_AchievementExplain"), transform.GetChild(1).GetChild(1).GetComponent<UILabel>().text);
				transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
				transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
				transform.GetChild(0).GetChild(6).GetChild(2).gameObject.SetActive(false);
			}
		}
	}

	bool BookAchivCheck()
	{
		bool bGotAchiv = true;
		
		for (int i = 0; i < 5; ++i) {
			for (int j = 0; j < 6; ++j) {
				bGotAchiv = GameMgr.getInstance.m_PhotoInfo[i].m_bPhotoGet[j];
				
				if(!bGotAchiv){
					return false;
				}
			}
		}

		return true;
	}

	public void UnlockTaleBook()
	{
		if(m_bInApp)
		{
			bool bGotAchiv = BookAchivCheck();
			
			if(bGotAchiv)
			{
				transform.GetChild(2).gameObject.SetActive(false);
				transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
				transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
				transform.GetChild(0).GetChild(6).GetChild(2).gameObject.SetActive(false);
				transform.GetComponent<UIButtonScale>().enabled = true;
			}
		}
	}
	
	// Update is called once per frame

	public void PhotoCheck()
	{
		gameMgr = GameMgr.getInstance;

		for(int j = 0; j < 6; ++j)
		{
			if(gameMgr.m_PhotoInfo[iPhotoIdx].m_bPhotoGet[j] == true)
				PhotoGet(j+1, true);
			else
				PhotoGet(j+1, false);
		}
	}

	void Update () {
#if UNITY_EDITOR
		GameMgr gameMgr = GameMgr.getInstance;
		if (!m_bInApp) {
			for (int j = 0; j < 6; ++j) {
				if (gameMgr.m_PhotoInfo [iPhotoIdx].m_bPhotoGet [j] == true)
					PhotoGet (j + 1, true);
				else
					PhotoGet (j + 1, false);
			}
		}

#endif

#if UNITY_STANDALONE

		if(Input.GetKeyDown(KeyCode.Return) || 
		   #if UNITY_STANDALONE_WIN
		   Input.GetKeyDown (KeyCode.JoystickButton0))
			#elif UNITY_STANDALONE_OSX
			Input.GetKeyDown (KeyCode.JoystickButton16))
				#endif
		{
			if(gameMgr.m_uiStatus.Equals (MainUIStatus.PHOTO) && transform.position.x < 0.1f && transform.position.x > -0.1f)
			{
				OnClick();
				StartCoroutine(Press());
			}
		}
#endif

		if (m_bAllCollected)
			return;

		if (!m_bInApp && !checkOnce && gameMgr.m_uiStatus.Equals (MainUIStatus.PHOTO)) {
			if(transform.parent.localPosition.x + transform.localPosition.x < 15.1f && transform.parent.localPosition.x + transform.localPosition.x > -15.1f)
			{
				PhotoCollectCheck();
			}
		}
	}

	IEnumerator Press()
	{
		GetComponent<UIButtonScale>().OnPress(true);
		yield return new WaitForSeconds (0.1f);
		GetComponent<UIButtonScale>().OnPress(false);
	}


	void PhotoGet(int iStg, bool bGet)
	{

		if (m_bAllCollected || m_bInApp)
			return;

		Transform PhotoTrans = transform.GetChild (0);
		Transform NavList = null;
		Transform ExplainLabel = null;

		NavList = transform.GetChild (2);
		ExplainLabel = transform.GetChild (1);

		if (bGet) {
			PhotoTrans.GetChild (iStg + 2).GetComponent<UISprite> ().enabled = true;
			PhotoTrans.GetChild (iStg + 2).GetComponent<UISprite> ().GrayScale (true);
			NavList.GetChild (iStg - 1).GetComponent<UISprite> ().spriteName = "piece_00";
			//StageOrangeCircle
			GameObject.Find("Chapters").transform.GetChild(iPhotoIdx).Find("Stages").GetChild(iStg-1).GetChild(0).GetChild(0).Find("photostage").GetComponent<UISprite>().color = new Color(235/255f, 60/255f, 2/255f);
		} else {
			PhotoTrans.GetChild (iStg + 2).GetComponent<UISprite> ().enabled = false;
			NavList.GetChild (iStg - 1).GetComponent<UISprite> ().spriteName = "piece_empty";
		}

		ExplainLabel.GetComponent<UILabel> ().text = string.Format(Localization.Get ("Album_Need"), Localization.Get (string.Format("Album_skin{0}",iPhotoIdx+1)), NeedPhotoCount(), iPhotoIdx+1);




	}

	void OnLocalize()
	{
		if (gameObject.name.Contains("Pay"))
			return;

		Transform ExplainLabel = transform.GetChild (1);


		ExplainLabel.GetComponent<UILabel> ().text = string.Format(Localization.Get ("Album_Need"), Localization.Get (string.Format("Album_skin{0}",iPhotoIdx+1)), NeedPhotoCount(), iPhotoIdx+1);
	}

	IEnumerator PhotoFrameActive()
	{
		yield return new WaitForSeconds (0.4f);

		Transform FrameTrans = transform.GetChild(0).GetChild(0);
		FrameTrans.GetComponent<UIPanel>().alpha = 1;
	}

	public void PhotoCollectCheck()
	{
		checkOnce = true;
		GameMgr gameMgr = GameMgr.getInstance;
		int iCounter = 0;

		for(int j = 0; j < 6; ++j)
		{
			if(gameMgr.m_PhotoInfo[iPhotoIdx].m_bPhotoGet[j] == true)
				++iCounter;
		}

		iNeedPhotoCount = 6-iCounter;

		if (iCounter == 6) { // all collected!

			transform.GetComponent<UIButtonScale>().enabled = true;

			if(!PlayerPrefs.GetInt (string.Format ("PhotoAllCollected{0}", iPhotoIdx)).Equals(1))
			{
				AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.ALBUM);
				GameObject objEffect = Instantiate(Resources.Load("Prefabs/UI/AlbumCompleteEffect") as GameObject) as GameObject;
				objEffect.transform.position = transform.position;
				objEffect.transform.parent = transform;
			}

			m_bAllCollected = true;
			PlayerPrefs.SetInt (string.Format ("PhotoAllCollected{0}", iPhotoIdx), 1);

			Transform PhotoTrans = transform.GetChild (0);
			Transform ExplainLabel = transform.GetChild(1);

			for(int i = 3; i < 9; ++i)
			{
				PhotoTrans.GetChild(i).GetComponent<UISprite>().GrayScale(false);
			}

			StartCoroutine(PhotoFrameActive());
			ExplainLabel.GetComponent<UILabel>().enabled = false;


			switch(iPhotoIdx)
			{
			case 0:
				/////Archive_08
//				#if UNITY_ANDROID
//				GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQCA", 0);
//				#elif UNITY_IOS
//				GameCenterManager.UpdateAchievement ("sally_achiv08", 100);
//				#elif UNITY_STANDALONE
//				SteamAchieveMgr.SetAchieve("sally_achiv08");
//				#endif
				break;

			case 1:
				/////Archive_09
//				#if UNITY_ANDROID
//				GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQCQ", 0);
//				#elif UNITY_IOS
//				GameCenterManager.UpdateAchievement ("sally_achiv09", 100);
//				#elif UNITY_STANDALONE
//				SteamAchieveMgr.SetAchieve("sally_achiv09");
//				#endif
				break;

			case 2:
				/////Archive_10
//				#if UNITY_ANDROID
//				GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQCg", 0);
//				#elif UNITY_IOS
//				GameCenterManager.UpdateAchievement ("sally_achiv10", 100);
//				#elif UNITY_STANDALONE
//				SteamAchieveMgr.SetAchieve("sally_achiv10");
//				#endif
				break;

			case 3:
				/////Archive_11
//				#if UNITY_ANDROID
//				GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQCw", 0);
//				#elif UNITY_IOS
//				GameCenterManager.UpdateAchievement ("sally_achiv11", 100);
//				#elif UNITY_STANDALONE
//				SteamAchieveMgr.SetAchieve("sally_achiv11");
//				#endif
				break;

			case 4:
				/////Archive_12
//				#if UNITY_ANDROID
//				GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQDA", 0);
//				#elif UNITY_IOS
//				GameCenterManager.UpdateAchievement ("sally_achiv12", 100);
//				#elif UNITY_STANDALONE
//				SteamAchieveMgr.SetAchieve("sally_achiv12");
//				#endif
				break;

			}

			bool bEveryFreePhotoCollected = true;
			for(int i =0 ; i < 5; ++i)
			{
				if(PlayerPrefs.GetInt(string.Format("PhotoAllCollected{0}", i)).Equals(0))
				{
					bEveryFreePhotoCollected = false;
					break;
				}
			}

			if(bEveryFreePhotoCollected)
			{
				/////Archive_18
//				#if UNITY_ANDROID
//				GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQEg", 0);
//				#elif UNITY_IOS
//				GameCenterManager.UpdateAchievement ("sally_achiv18", 100);
//				#elif UNITY_STANDALONE
//				SteamAchieveMgr.SetAchieve("sally_achiv18");
//
				StartCoroutine(WaitOneFrameToUnlock());
//
//				#endif
			}
		}


	}

	IEnumerator WaitOneFrameToUnlock()
	{
		yield return null;
		yield return null;

		for(int i = 0 ; i < 3; ++i)
		{
			transform.parent.GetChild(5+i).GetComponent<UIPhoto>().UnlockTaleBook();
		}
	}

	public int NeedPhotoCount()
	{
		GameMgr gameMgr = GameMgr.getInstance;
		int iCounter = 0;
		
		for(int j = 0; j < 6; ++j)
		{
			if(gameMgr.m_PhotoInfo[iPhotoIdx].m_bPhotoGet[j] == true)
				++iCounter;
		}

		iNeedPhotoCount = 6-iCounter;
		return 6-iCounter;
	}

	void OnClick()
	{
		if(GameMgr.getInstance.m_uiStatus.Equals(MainUIStatus.PHOTO))
			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		if (m_bAllCollected) {

			if(m_bInApp)
			{
				bool bGotAchiv = BookAchivCheck();
				
				if(!bGotAchiv)
				{
					return;
				}
			}

			if(!m_bSelected) //Dress Up
			{
				gameMgr.m_strCurSkin = m_strSkinName;
				PlayerPrefs.SetString("SkinName", m_strSkinName);

				for(int i = 0; i < transform.parent.childCount; ++i)
				{
					if(!transform.parent.GetChild(i).GetComponent<UIPhoto>().m_bInApp)
						transform.parent.GetChild(i).GetChild(0).GetChild(0).GetChild(5).GetChild(1).GetComponent<UISprite>().enabled = false;
					else
						transform.parent.GetChild(i).GetChild(0).GetChild(5).GetChild(1).GetComponent<UISprite>().enabled = false;
					transform.parent.GetChild(i).GetComponent<UIPhoto>().m_bSelected = false;
				}

				if(!m_bInApp)
					transform.GetChild(0).GetChild(0).GetChild(5).GetChild(1).GetComponent<UISprite>().enabled = true;
				else
					transform.GetChild(0).GetChild(5).GetChild(1).GetComponent<UISprite>().enabled = true;

				if(UICamera.selectedObject != null)
					AudioMgr.getInstance.PlaySfx(GameObject.Find("SFX").GetComponent<AudioSource>(), "ui_bundle", (int)UI_SOUND_LIST.CLOTH_SELECT);

				m_bSelected = true;

				transform.GetChild(3).GetComponent<TweenAlpha>().ResetToBeginning();
				transform.GetChild(3).GetComponent<TweenAlpha>().Play(true);
			}else{
				gameMgr.m_strCurSkin = "basic";
				PlayerPrefs.SetString("SkinName", "basic");

				if(!m_bInApp)
					transform.GetChild(0).GetChild(0).GetChild(5).GetChild(1).GetComponent<UISprite>().enabled = false;
				else
					transform.GetChild(0).GetChild(5).GetChild(1).GetComponent<UISprite>().enabled = false;

				m_bSelected = false;
			}

		} else if (m_bInApp) {


			// Confrim?
			float fTargetX = transform.parent.parent.GetChild(1).GetComponent<SpringPanel> ().target.x;
			if(fTargetX != 1200 && fTargetX != 2400 && fTargetX != 3600)
				return;

			StartCoroutine(MainScene.getInstance.ToReallyBuy(true));


		}
	}


}






