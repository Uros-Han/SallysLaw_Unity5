using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameMgr : MonoBehaviour {

	private static GameMgr instance;

	public static GameMgr getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(GameMgr)) as GameMgr;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("GameMgr");
				instance = obj.AddComponent (typeof(GameMgr)) as GameMgr;
			}
			
			return instance;
		}
	}

	void OnApplicationQuit()
	{

		instance = null;
	}

	public string m_strSelectedStage;
	public int m_iCurChpt;
	public int m_iCurStage;
	public int m_iCurAct;

	public int m_iOpenedChpt = 1;
	public int m_iOpenedStage = 1;

	public bool m_bPurchaseCostume;

	public int[,] m_iStageActNum;

	public PhotoInfo[] m_PhotoInfo = new PhotoInfo[5];
	
	public List<Stage> m_ListStage_string; //미리 챕터 내의 스테이지들 스트링파일 불러다 놓을곳

	public int DeviceResolutionWidth;

	public MainUIStatus m_uiStatus = MainUIStatus.MAIN;
	public string m_strVersion;

	public bool m_bSceneLoaded;

	public bool m_bSoundMute;
	public bool m_bBgmMute;
	public bool m_bCloud;

	public string m_strCurSkin;

	public bool m_bDevMode_AllClear;
	public bool m_bDevMode_SpeedHack;

	public OSType OS_Type;

	public bool m_bCloudLoadOnce;

	// Use this for initialization
	void Awake () {

		if (instance == null)
			instance = this;
		
		else if (instance != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad (gameObject);

		m_iCurChpt = 1;
		m_iCurStage = 1;
		m_iCurAct = 1;

		m_iOpenedChpt = 1;
		m_iOpenedStage = 1;

		//Act num
		m_iStageActNum = new int[5, 6]{ {3,3,3,3,3,3}, {3,3,3,3,3,2}, {2,3,3,3,3,2}, {3,3,3,3,3,2}, {3,3,2,2,4,3} };
		m_ListStage_string = new List<Stage> ();

		Application.targetFrameRate = 60;

		ObjectPool.getInstance.ObjectPoolSetting ();


		m_bPurchaseCostume = true;


#if UNITY_ANDROID
		OS_Type = OSType.Android;
#elif UNITY_IOS
		OS_Type = OSType.iOS;
#elif UNITY_STANDALONE || UNITY_WEBGL
		OS_Type = OSType.PC;
		m_bPurchaseCostume = true;

#endif



//		Storekit_Sally.Initialize ();
//		GameCenterManager.AuthenticateLocalPlayer ();


		DeviceResolutionWidth = 2726;

//#if !UNITY_STANDALONE
//		switch (Application.systemLanguage) {
//		case SystemLanguage.English:
//			Localization.language = "English";
//			break;
//
//		case SystemLanguage.Korean:
//			Localization.language = "한국어";
//			break;
//
//		case SystemLanguage.Chinese:
//			Localization.language = "한국어";
//			break;
//
//		default:
//			Localization.language = "English";
//			break;
//		}
//#endif


		PrefsCheck ();

		if (Application.loadedLevelName.Equals ("Main")) {
			GameObject.Find("FadePanel").GetComponent<MainFade>().DontFadeNow();
		}


//		m_bCloud = false;

#if UNITY_IOS
		if (m_bCloud && iCloudBinding.documentStoreAvailable()) {
			if(iCloudBinding.isFileInCloud("SaveData"))
				iCloudBinding.isFileDownloaded ("SaveData");
			
			CloudLoad();
		}else
			CloudMgr.getInstance.GameData_Load();
#elif UNITY_STANDALONE || UNITY_WEBGL
//		CloudMgr.getInstance.GameData_Load();
#endif
		CloudMgr.getInstance.GameData_Load();
	}

	void PrefsCheck()
	{

		//PLAY TIME CHECK
		
		if (PlayerPrefs.GetInt ("playTime").Equals (0)) { //최초실행
			Debug.Log ("First Setting");
//			Application.LoadLevel ("Loading");
			PlayerPrefs.SetInt ("Sound", 1);
			PlayerPrefs.SetInt ("Music", 1);
			PlayerPrefs.SetInt ("iCloud", 1);
			PlayerPrefs.SetInt ("PurchaseCostume", 0);
			PlayerPrefs.SetString ("SkinName", "basic");
			PlayerPrefs.SetInt ("AllClear", 0);
			PlayerPrefs.SetInt ("RatePopUp", 1);
			PlayerPrefs.SetInt ("RateMainConter", 0);

			switch (Application.systemLanguage) {
			case SystemLanguage.English:
				PlayerPrefs.SetInt ("Lang", 0);
				break;
				
			case SystemLanguage.Chinese:
				PlayerPrefs.SetInt ("Lang", 2);
				break;
				
			case SystemLanguage.German:
				PlayerPrefs.SetInt ("Lang", 4);
				break;
				
			case SystemLanguage.Spanish:
				PlayerPrefs.SetInt ("Lang", 5);
				break;
				
			default:
				PlayerPrefs.SetInt ("Lang", 0);
				break;
			}

#if UNITY_STANDALONE || UNITY_WEBGL

			PlayerPrefs.SetFloat("SoundVolume",1f);
			PlayerPrefs.SetFloat("MusicVolume",1f);
			PlayerPrefs.SetInt ("FullScreen", 1);

			PlayerPrefs.SetInt ("Resolution", Screen.resolutions.Length - 1);
//			Screen.SetResolution(Screen.resolutions[PlayerPrefs.GetInt("Resolution")].width, Screen.resolutions[PlayerPrefs.GetInt("Resolution")].height, true);
//			Screen.SetResolution(1280, 720, false);
#endif

			m_iOpenedChpt = 1;
			m_iOpenedStage = 1;
		} 

		PlayerPrefs.SetInt ("playTime", PlayerPrefs.GetInt ("playTime") + 1);

		if (!PlayerPrefs.HasKey ("Lang")) {
			Debug.Log("Language didn't set");
			switch (Application.systemLanguage) {
			case SystemLanguage.English:
				PlayerPrefs.SetInt ("Lang", 0);
				break;
				
			case SystemLanguage.Chinese:
				PlayerPrefs.SetInt ("Lang", 2);
				break;
				
			case SystemLanguage.German:
				PlayerPrefs.SetInt ("Lang", 4);
				break;
				
			case SystemLanguage.Spanish:
				PlayerPrefs.SetInt ("Lang", 5);
				break;
				
			default:
				PlayerPrefs.SetInt ("Lang", 2);
				break;
			}
		}

		PlayerPrefs.SetInt ("Lang", 0);

		Debug.Log (PlayerPrefs.GetInt ("Lang"));

		#if UNITY_STANDALONE || UNITY_WEBGL
//		if(PlayerPrefs.GetInt ("FullScreen").Equals(1))
//			Screen.SetResolution(Screen.resolutions[PlayerPrefs.GetInt("Resolution")].width, Screen.resolutions[PlayerPrefs.GetInt("Resolution")].height, true);
//		else
//			Screen.SetResolution(Screen.resolutions[PlayerPrefs.GetInt("Resolution")].width, Screen.resolutions[PlayerPrefs.GetInt("Resolution")].height, false);

//		Screen.SetResolution(1280, 720, false);
		#endif


		//SKIN CHECK
		m_strCurSkin = PlayerPrefs.GetString ("SkinName");
		if(m_strCurSkin.Equals(""))
			m_strCurSkin = "basic";

		///SOUND CHECK
		if (PlayerPrefs.GetInt ("Sound") == 0)
			m_bSoundMute = true;
		if (PlayerPrefs.GetInt ("Music") == 0)
			m_bBgmMute = true;


		if (PlayerPrefs.GetInt ("iCloud") == 1)
			m_bCloud = true;

		if (PlayerPrefs.GetInt ("PurchaseCostume") == 1)
			m_bPurchaseCostume = true;

//		#if UNITY_EDITOR
//		for (int i = 0; i < 5; ++i) {
//			for (int j = 0; j < 6; ++j)
//			{
//				PlayerPrefs.SetInt (string.Format ("PhotoAllCollected{0}", i), 0);
//			}
//
//		}
//		#endif

		//for debug
//		#if UNITY_EDITOR
//		PlayerPrefs.SetInt("FatherJumpCount", 0);
//		PlayerPrefs.SetInt("GetPhotoCount", 0);
//		PlayerPrefs.SetInt("SallyJumpCount", 0);
//		PlayerPrefs.SetInt("SpikeRemoveCount", 0);
//		PlayerPrefs.SetInt("GetKeyCount", 0);
//		#endif

	}

	void Start()
	{
//		#if UNITY_STANDALONE
//		switch (SteamApps.GetCurrentGameLanguage()) {
//		case "english":
//			PlayerPrefs.SetInt ("Lang", 0);
//			break;
//			
//		case "koreana":
//			PlayerPrefs.SetInt ("Lang", 1);
//			break;
//
//		case "schinese":
//			PlayerPrefs.SetInt ("Lang", 2);
//			break;
//
//		case "tchinese":
//			PlayerPrefs.SetInt ("Lang", 3);
//			break;
//			
//		default:
//			PlayerPrefs.SetInt ("Lang", 0);
//			break;
//		}
//		Debug.Log (SteamApps.GetCurrentGameLanguage());
//		#endif

		Localization.language = Localization.knownLanguages[PlayerPrefs.GetInt("Lang")];

		#if UNITY_STANDALONE || UNITY_WEBGL
		GameObject.Find("UI Root").BroadcastMessage("SetFont", SendMessageOptions.DontRequireReceiver);
		#endif
	}


	public void CloudLoad()
	{
		CloudMgr cloud = CloudMgr.getInstance;

		if(cloud != null)
			cloud.Do_CloudLoad ();
	}

	void NotiAlbum(bool bNotiOn)
	{
		if(bNotiOn)
			PlayerPrefs.SetInt("AlbumNoti", 1);
		else
			PlayerPrefs.SetInt("AlbumNoti", 0);

	}

	/// <summary>
	/// When Started stage.
	/// </summary>
	public void StartStage()
	{
		TimeMgr.Pause ();

	}

	/// <summary>
	/// 전에 있던 스테이지 다 풀로보내거나 비활성시킴
	/// </summary>
	public void DisableBeforeAct()
	{
		if (m_iStageActNum [m_iCurChpt - 1, m_iCurStage - 1] == 1)
			return;


		if (SceneStatus.getInstance.m_bFirstLoaded == false) {

			//전에 해제해줘야할 스테이지 오브젝트 찾기
			int iBeforeStage = 0;

			for (int i = 0; i <GameObject.Find ("Objects").transform.childCount; ++i) {
				if(GameObject.Find ("Objects").transform.GetChild(i).gameObject == SceneStatus.getInstance.m_objCurStage)
					iBeforeStage = i-1;
			}

			if(iBeforeStage < 0)
			{
				iBeforeStage = GameMgr.getInstance.m_ListStage_string.Count - 1;
			}

			//박스 해제해주기
			Transform BoxParent = GameObject.Find ("Objects").transform.GetChild (iBeforeStage).Find("Boxes").gameObject.transform;
			Transform SpikeParent = GameObject.Find ("Objects").transform.GetChild (iBeforeStage).Find("Spikes").gameObject.transform;
			Transform SallyPathParent = GameObject.Find ("Objects").transform.GetChild (iBeforeStage).Find("sallyPaths").gameObject.transform;

			for(int i=0; i < BoxParent.childCount; ++i)
			{
				if(BoxParent.GetChild(i).childCount > 1)
				{
					for(int j = 1 ; j < BoxParent.GetChild(i).childCount; ++j)
					{
						//floor to Pool
						ObjectPool.getInstance.pool.RemoveItem(BoxParent.GetChild(i).GetChild(j).gameObject, "Floor");
					}
				}
				// box to pool
				ObjectPool.getInstance.pool.RemoveItem(BoxParent.GetChild(i).gameObject, "Box");
			}

			for(int i=0; i < SpikeParent.childCount; ++i)
			{
				// spike to pool
				ObjectPool.getInstance.pool.RemoveItem(SpikeParent.GetChild(i).gameObject, "Spike");
			}

			for(int i=0; i < SallyPathParent.childCount; ++i)
			{
				// sallyPath to pool
				ObjectPool.getInstance.pool.RemoveItem(SallyPathParent.GetChild(i).gameObject, "SallyPath");
			}


			GameObject.Find ("Objects").transform.GetChild (iBeforeStage).gameObject.SetActive (false);

			UIManager.getInstance.Loading(false);
		}
	}

	public void Clear()
	{
 		if (GameObject.Find ("MapToolMgr") != null) { // This is MapTool!
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().Play (false);
		}else if(StageLoader.getInstance.m_bStageLoader){
//			GameObject.Find("UIManager").GetComponent<UIManager>().Clear();
		}else { // This isnt Maptool!

			GameObject Photo = GameObject.Find ("Photo(Clone)");
			//만약 이번스테이지에 사진이있었으면 GameMgr사진 부울값 참으로
			if (Photo != null) {
				if(Photo.GetComponent<SkeletonAnimation>().loop.Equals(false))
				{                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
					Photo.GetComponent<Photo>().PhotoGet();
				}
			}

			m_iCurAct += 1;

			if(m_iCurAct == m_ListStage_string.Count + 1) // STAGE Clear! ( All ACT in this STAGE clear!)
			{
				#if !(UNITY_STANDALONE || UNITY_WEBGL)
				GameObject.Find("SidePanel").GetComponent<UIPanel>().alpha = 0f;
				#endif
				GameObject.Find ("FastForward").transform.GetChild (0).GetComponent<FastForwardBtn>().OnDisable();

				UIManager.getInstance.m_FastForwardPanel.GetComponent<UIPanel>().alpha = 0f;

				if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.RUNNER)
				{
					m_iCurAct = GameMgr.getInstance.m_ListStage_string.Count;

					SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.SALLY_EXIT;

					if(SceneStatus.getInstance.m_bFinaleStage) // GAME CLEAR
					{
						/////Archive_07
//						#if UNITY_ANDROID
//						GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQBw", 0);
//						#elif UNITY_IOS
//						GameCenterManager.UpdateAchievement ("sally_achiv07", 100);
//						#elif UNITY_STANDALONE
//						SteamAchieveMgr.SetAchieve("sally_achiv07");
//						#endif

						m_iCurChpt = 5;
						m_iCurStage = 6;
						PlayerPrefs.SetInt("AllClear", 1);

						CloudMgr.getInstance.GameData_Save ();

						UIManager.getInstance.LoadScene("Main");
					}else
					{
						AudioMgr.getInstance.PlaySfx(GameObject.Find("SFX").GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.CHG_CHAR);
						UIManager.getInstance.StageEnd();
					}

				}else{

					SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.SALLY_EXIT;

					if(m_iCurStage.Equals(6))
					{
						switch(m_iCurChpt)
						{
						case 1:
							/////Archive_03
//							#if UNITY_ANDROID
//							GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQAw", 0);
//							#elif UNITY_IOS
//							GameCenterManager.UpdateAchievement ("sally_achiv03", 100);
//							#elif UNITY_STANDALONE
//							SteamAchieveMgr.SetAchieve("sally_achiv03");
//							#endif
							break;

						case 2:
							/////Archive_04
//							#if UNITY_ANDROID
//							GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQBA", 0);
//							#elif UNITY_IOS
//							GameCenterManager.UpdateAchievement ("sally_achiv04", 100);
//							#elif UNITY_STANDALONE
//							SteamAchieveMgr.SetAchieve("sally_achiv04");
//							#endif
							break;

						case 3:
							/////Archive_05
//							#if UNITY_ANDROID
//							GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQBQ", 0);
//							#elif UNITY_IOS
//							GameCenterManager.UpdateAchievement ("sally_achiv05", 100);
//							#elif UNITY_STANDALONE
//							SteamAchieveMgr.SetAchieve("sally_achiv05");
//							#endif
							break;

						case 4:
							/////Archive_06
//							#if UNITY_ANDROID
//							GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQBg", 0);
//							#elif UNITY_IOS
//							GameCenterManager.UpdateAchievement ("sally_achiv06", 100);
//							#elif UNITY_STANDALONE
//							SteamAchieveMgr.SetAchieve("sally_achiv06");
//							#endif
							break;
						}


						m_iCurChpt += 1;
						m_iCurStage = 1;
						m_iCurAct = 1;

						if((m_iOpenedChpt * 10) + m_iOpenedStage < (m_iCurChpt * 10) + m_iCurStage)
						{
							m_iOpenedChpt = m_iCurChpt;
							m_iOpenedStage = m_iCurStage;
						}

						CloudMgr.getInstance.GameData_Save ();

						UIManager.getInstance.LoadScene("Loading");
					}
					else
					{
						m_iCurStage += 1;
						m_iCurAct = 1;

						if((m_iOpenedChpt * 10) + m_iOpenedStage < (m_iCurChpt * 10) + m_iCurStage)
						{
							m_iOpenedChpt = m_iCurChpt;
							m_iOpenedStage = m_iCurStage;
						}

						CloudMgr.getInstance.GameData_Save ();

						UIManager.getInstance.LoadScene("Loading");
					}

//					GameObject.Find("Runner(Clone)").GetComponent<Rigidbody2D>().gravityScale = 1.5f;
				}
			}
			else  //ACT Clear!
			{
				UIManager.getInstance.Loading(true);

				if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.RUNNER)
				{
					m_iCurAct -= 1;

					SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.SALLY_EXIT;
					StartCoroutine(ActClear ());
				}else{

					if(m_iCurChpt.Equals(1) && m_iCurStage.Equals(1) && m_iCurAct.Equals(3))
					{
						UIManager.getInstance.FatherTutorial (false, 2);
						UIManager.getInstance.TutorialFade(false);
						Camera.main.GetComponent<CamMoveMgr>().m_camTutorial = CAM_TUTORIAL.NOT_TUTORIAL;
					}

					GameObject.Find("Guardian(Clone)").GetComponent<Guardian>().GuardianExit();

					#if !(UNITY_STANDALONE || UNITY_WEBGL)
					GameObject.Find("SidePanel").GetComponent<UIPanel>().alpha = 0f;
					#endif
					GameObject.Find ("FastForward").transform.GetChild (0).GetComponent<FastForwardBtn>().OnDisable();
					UIManager.getInstance.m_FastForwardPanel.GetComponent<UIPanel>().alpha = 0f;

					ToAlreadyCreatedNextAct(false);
				}
			}
		}
	}

	IEnumerator ActClear()
	{
		if (SceneStatus.getInstance.m_bFinaleStage)
			yield return new WaitForSeconds (1.5f);
		else
			yield return new WaitForSeconds (0.5f);
			
		m_iCurAct += 1;


		SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.STAGE_CHG_SALLY;
		
		GameObject.Find("Runner(Clone)").gameObject.SetActive(false);
		GameObject.Find("Guardian(Clone)").gameObject.SetActive(false);
		
		StartCoroutine (StageLoader.getInstance.CreateObjects (m_ListStage_string[m_iCurAct-1]));
		
		Camera.main.GetComponent<CamMoveMgr>().Init(true);
		
		if(SceneStatus.getInstance.m_bFirstLoaded)
			SceneStatus.getInstance.m_bFirstLoaded = false;
		
		SceneStatus.getInstance.m_iFlashBackIdx = 0;

		if(!SceneStatus.getInstance.m_bFinaleStage)
			GameObject.Find("GuardianCamContainer").transform.GetChild(0).gameObject.SetActive(false);
	}

	public void ToAlreadyCreatedNextAct(bool bSallyAllClear)
	{

		if (bSallyAllClear) {
			SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.SALLY_ENTER;

			StartCoroutine(WhiteFadeOff());
			m_iCurAct = 1;
			GameObject.Find("GuardianCamContainer").transform.GetChild(0).gameObject.SetActive(true);

			for(int i = 0; i < Camera.allCamerasCount; ++i)
			{
				if(Camera.allCameras[i].gameObject.layer != 5) // not ui Camera
					Camera.allCameras[i].orthographicSize = 3.5f;
			}
		}
		else {
			SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.STAGE_CHG_FATHER;
		}

		GameObject curStageObj = GameObject.Find ("Objects").transform.GetChild (m_iCurAct - 1).gameObject;

		curStageObj.SetActive (true);
		curStageObj.transform.Find ("Players").GetChild (0).gameObject.SetActive (true);
		curStageObj.transform.Find ("Players").GetChild (1).gameObject.SetActive (true);

		if (!bSallyAllClear) {
			curStageObj.transform.Find ("Players").Find ("Runner(Clone)").gameObject.GetComponent<SkeletonAnimation> ().enabled = false;
			curStageObj.transform.Find ("Players").Find ("Runner(Clone)").gameObject.GetComponent<SkeletonAnimation> ().Reset();
		}

		Transform InteractionPropTrans = curStageObj.transform.Find ("InteractionProps").transform;
		for(int i = 0; i < InteractionPropTrans.childCount; ++i) {
			InteractionPropTrans.GetChild (i).GetComponent<InteractionProp> ().ResetProp(false,true);
		}

		Transform springTrnas = curStageObj.transform.Find  ("Springs").transform;
		for(int i = 0; i < springTrnas.childCount; ++i) {
			springTrnas.GetChild (i).GetComponent<Spring> ().Fix();
		}

		Transform spikeTrans = curStageObj.transform.Find  ("Spikes").transform;
		for(int i = 0; i < spikeTrans.childCount; ++i) {
			spikeTrans.GetChild (i).GetComponent<Spike> ().StopAllCoroutines();
			spikeTrans.GetChild(i).GetComponent<Spike> ().SpikeToOriginPos();
			spikeTrans.GetChild(i).GetComponent<Spike>().m_list_iRevivalIdx.Clear();
		}

		StartCoroutine (StageLoader.getInstance.CreateObjects (m_ListStage_string[m_iCurAct-1], true , curStageObj));

		Camera.main.GetComponent<CamMoveMgr>().Init(true);

		if(SceneStatus.getInstance.m_bFirstLoaded)
			SceneStatus.getInstance.m_bFirstLoaded = false;

		SceneStatus.getInstance.m_iFlashBackIdx = 0;

		if (bSallyAllClear)
			StartCoroutine (SallyAllClear ());

	}

	IEnumerator WhiteFadeOff()
	{
		yield return new WaitForSeconds (1f);
		UIManager.getInstance.SallyToFather ();
	}

	IEnumerator SallyAllClear()
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();


		DisableBeforeAct ();
		ChangeBackgrounds (true, true);

		Transform TextfloatUITrans = GameObject.Find ("TextFloatUI").transform;
		for (int i = 0; i < TextfloatUITrans.childCount; ++i) {
			TextfloatUITrans.GetChild(i).localScale = new Vector2(0.714f, 0.714f);
		}

		Camera.main.GetComponent<CamMoveMgr> ().GuardianCamInit ();
	}

	public void GameOver()
	{
//		Debug.LogError(SceneStatus.getInstance.fInGameTimer);

		if(GameObject.Find("MapToolMgr") != null) // This is MapTool!
		{
			if(GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bNowPlaying)
				GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().Play(false);
			else
				return;
		}
		else{ // Game Over!
		
			//GameObject.Find("UIManager").GetComponent<UIManager>().GameOver();

			EffectManager.getInstance.RunFadeEffect(true);

			TimeMgr.Pause();

//			if (GameObject.Find ("Photo(Clone)") != null) {
//				GameObject.Find("Photo(Clone)").GetComponent<SpriteRenderer>().enabled = true;
//			}
			
		}
	}

	void BlackOut(PLAYER_STATUS playerStatus)
	{
		GameObject.Find ("Guardian(Clone)").GetComponent<TrailRenderer> ().time = -1f;
		GameObject.Find ("Runner(Clone)").GetComponent<TrailRenderer> ().time = -1f;

		if (GameObject.Find ("MapToolMgr") != null) { // This is MapTool!
			if (GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bNowPlaying)
				GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().Play (false);
			else
				return;
		} else {

			if (playerStatus == default(PLAYER_STATUS)) {
				if (SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN || SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.SALLY_CRASH || SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.FATHER_WAIT)
					RestartWithGuardian ();
				else
					RestartWithRunner ();
			} else {
				if (playerStatus == PLAYER_STATUS.GUARDIAN || playerStatus == PLAYER_STATUS.SALLY_CRASH || SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.FATHER_WAIT)
					RestartWithGuardian ();
				else
					RestartWithRunner ();
			}
		}
	}

	void FadeEnd()
	{
		//EffectManager.getInstance.SwitchBlur(false);	

		TimeMgr.Play ();

		Invoke ("TrailReset", 0.1f);
	}

	void TrailReset()
	{
		GameObject.Find ("Guardian(Clone)").GetComponent<TrailRenderer> ().time = 0.25f;
		GameObject.Find ("Runner(Clone)").GetComponent<TrailRenderer> ().time = 0.25f;
	}

	

	public void RestartWithRunner()
	{

		GameObject runner = GameObject.Find ("Runner(Clone)").gameObject;
		GameObject guardian = GameObject.Find ("Guardian(Clone)").gameObject;

		SceneStatus sceneStatus = SceneStatus.getInstance;
		sceneStatus.m_enPlayerStatus = PLAYER_STATUS.SALLY_WAIT;
		sceneStatus.m_iRestartInThisStage += 1;

		if (sceneStatus.m_iRestartInThisStage.Equals (20)) {
			/////Archive_20
//			#if UNITY_ANDROID
//			GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQFA", 0);
//			#elif UNITY_IOS
//			GameCenterManager.UpdateAchievement ("sally_achiv20", 100);
//			#elif UNITY_STANDALONE
//			SteamAchieveMgr.SetAchieve("sally_achiv20");
//			#endif
		}
		
		guardian.GetComponent<Guardian> ().Reset ();

		GameObject mainCam = Camera.main.gameObject;

		if (!SceneStatus.getInstance.m_bFinaleStage) {
			guardian.GetComponent<Rigidbody2D> ().isKinematic = true;
			GameObject.Find ("GuardianCamContainer").transform.GetChild (0).gameObject.SetActive (false);

			mainCam.GetComponent<CamMoveMgr> ().ChgCamOrtho (2.5f);

		} else {
			guardian.GetComponent<Rigidbody2D> ().isKinematic = false;
			GameObject.Find ("GuardianCamContainer").transform.GetChild (0).gameObject.SetActive (true);
			
			mainCam.GetComponent<CamMoveMgr> ().ChgCamOrtho (3.5f);
		}
		
		runner.GetComponent<Runner> ().Reset ();
		runner.GetComponent<Rigidbody2D> ().gravityScale = 1.5f;

		mainCam.transform.position = runner.transform.position + new Vector3 (0.2f, mainCam.GetComponent<CamMoveMgr>().m_fRunnerYPosFixer, -10f);
		mainCam.transform.position = mainCam.GetComponent<CamMoveMgr> ().CamRestriction ();
		mainCam.transform.position = new Vector3 (mainCam.transform.position.x, mainCam.transform.position.y, -10f);
		mainCam.GetComponent<CamMoveMgr> ().RunnerMaintainYPos ();
		
		SceneStatus.getInstance.m_iFlashBackIdx = 0;
		
		//
		//		for (int i = 0; i < GameObject.Find("MoveOrders").transform.childCount; ++i) {
		//			if(GameObject.Find("MoveOrders").transform.GetChild(i).childCount > 1)
		//				GameObject.Find("MoveOrders").transform.GetChild(i).GetChild(1).GetComponent<MoveOrder>().m_iIdx = 0;
		//		}

		
//		if(GameObject.Find ("Main Camera").GetComponent<ColorCorrectionCurves>())
//			GameObject.Find ("Main Camera").GetComponent<ColorCorrectionCurves>().enabled = false;



//		for(int i = 0 ; i < GameObject.Find("Retry").transform.childCount; ++i)
//		{
//			GameObject.Find("Retry").transform.GetChild(i).gameObject.SetActive(false);
//		}

		if (GameObject.Find ("Photo(Clone)") != null) {
			SkeletonAnimation skel = GameObject.Find("Photo(Clone)").GetComponent<SkeletonAnimation>();
			skel.loop = true;
			skel.AnimationName = "idle";
		}

		Transform Doors = GameObject.Find ("Doors").transform;
		for (int i = 0; i < Doors.childCount; ++i) {
			if(Doors.GetChild(i).GetChild(0).GetComponent<R_Door>())
				Doors.GetChild(i).GetChild(0).GetComponent<R_Door>().InitDoor();
			else
				Doors.GetChild(i).GetChild(0).GetComponent<G_Door>().InitDoor();
		}

		Transform switchs = GameObject.Find ("Switchs").transform;
		for (int i = 0; i < switchs.childCount; ++i) {
			switchs.GetChild(i).GetComponent<Switch>().m_bPressed = false;
			switchs.GetChild(i).GetComponent<Switch>().InitSpine();
		}


		Transform spikeTrans = GameObject.Find ("Spikes").transform;
		for(int i = 0; i < spikeTrans.childCount; ++i) {
			spikeTrans.GetChild (i).GetComponent<Spike> ().StopAllCoroutines();
			spikeTrans.GetChild(i).GetComponent<Spike> ().SpikeToOriginPos();
			spikeTrans.GetChild(i).GetComponent<Spike>().m_list_iRevivalIdx.Clear();
		}

		Transform springTrnas = GameObject.Find ("Springs").transform;
		for(int i = 0; i < springTrnas.childCount; ++i) {
			springTrnas.GetChild (i).GetComponent<Spring> ().Fix();
		}

		Transform InteractionPropTrans = GameObject.Find ("InteractionProps").transform;
		for(int i = 0; i < InteractionPropTrans.childCount; ++i) {
			InteractionPropTrans.GetChild (i).GetComponent<InteractionProp> ().ResetProp(false,false);
		}

		if(m_iCurChpt.Equals (1) && m_iCurStage.Equals (2) && m_iCurAct.Equals (1))
		{
			GameObject.Find("StartVehicle(Clone)").transform.GetChild(1).GetComponent<SkeletonAnimation>().state.SetAnimation(0, "idle", false);
		}

		runner.GetComponent<Runner> ().ReturnWaitTime ();
		runner.GetComponent<Runner> ().SallyPathCreate (false);

//		if(EffectManager.getInstance.m_bBlurOn)
//			EffectManager.getInstance.SwitchBlur ();

		ChangeBackgrounds (false);

//		GameObject.Find ("FatherWait").transform.GetChild (0).gameObject.SetActive (false);
		UIManager.getInstance.SallyWait ();



		SceneStatus.getInstance.fInGameTimer = 0f;

//		UIManager.getInstance.TryFatherBtn (false);
	}

	public void RestartWithGuardian()
	{
		SceneStatus sceneStatus = SceneStatus.getInstance;
		SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.FATHER_WAIT;
		sceneStatus.m_iRestartInThisStage += 1;
		
		if (sceneStatus.m_iRestartInThisStage.Equals (20)) {
			/////Archive_20
//			#if UNITY_ANDROID
//			GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQFA", 0);
//			#elif UNITY_IOS
//			GameCenterManager.UpdateAchievement ("sally_achiv20", 100);
//			#elif UNITY_STANDALONE
//			SteamAchieveMgr.SetAchieve("sally_achiv20");
//			#endif
		}

		UIManager.getInstance.FatherWait ();
//		GameObject.Find ("FatherWait").transform.GetChild (0).gameObject.SetActive (true);
//		StartCoroutine (GameObject.Find ("Main Camera").GetComponent<CamMoveMgr> ().SwipeMove ());


		GameObject.Find ("Guardian(Clone)").GetComponent<Guardian> ().Reset ();
		GameObject.Find ("Runner(Clone)").GetComponent<Runner> ().StartWithGuardian ();


		Camera.main.GetComponent<CamMoveMgr> ().GuardianRestartCam ();
	
		
//		for(int i = 0 ; i < GameObject.Find("Retry").transform.childCount; ++i)
//		{
//			GameObject.Find("Retry").transform.GetChild(i).gameObject.SetActive(false);
//		}

		if (GameObject.Find ("Photo(Clone)") != null) {
			SkeletonAnimation skel = GameObject.Find("Photo(Clone)").GetComponent<SkeletonAnimation>();
			skel.loop = true;
			skel.AnimationName = "idle";
		}

		Transform Doors = GameObject.Find ("Doors").transform;
		for (int i = 0; i < Doors.childCount; ++i) {
			if(Doors.GetChild(i).GetChild(0).GetComponent<R_Door>())
				Doors.GetChild(i).GetChild(0).GetComponent<R_Door>().InitDoor();
			else
				Doors.GetChild(i).GetChild(0).GetComponent<G_Door>().InitDoor();
		}

		Transform switchTrans = GameObject.Find ("Switchs").transform;
		for (int i = 0; i < switchTrans.childCount; ++i) {
			switchTrans.GetChild(i).GetComponent<Switch>().m_bPressed = false;
			switchTrans.GetChild(i).GetComponent<Switch>().InitSpine();
		}

		Transform timeballTrans = GameObject.Find ("Timeballs").transform;
		for (int i = 0; i < timeballTrans.childCount; ++i) {
			SkeletonAnimation skel = timeballTrans.GetChild(i).GetComponent<SkeletonAnimation>();
			skel.loop = true;
			skel.AnimationName = "idle";
		}

		Transform spikeTrans = GameObject.Find ("Spikes").transform;
		for(int i = 0; i < spikeTrans.childCount; ++i) {
			spikeTrans.GetChild (i).GetComponent<Spike> ().StopAllCoroutines();
			spikeTrans.GetChild (i).GetComponent<Spike> ().SpikeToOriginPos();
		}

		Transform springTrnas = GameObject.Find ("Springs").transform;
		for(int i = 0; i < springTrnas.childCount; ++i) {
			springTrnas.GetChild (i).GetComponent<Spring> ().Fix();
		}

		Transform InteractionPropTrans = GameObject.Find ("InteractionProps").transform;
		for(int i = 0; i < InteractionPropTrans.childCount; ++i) {
			InteractionPropTrans.GetChild (i).GetComponent<InteractionProp> ().ResetProp(false,true);
		}

		GameObject.Find ("Runner(Clone)").GetComponent<Runner> ().ReturnWaitTime ();

		SceneStatus.getInstance.fInGameTimer = 0f;

		GameObject.Find("GuardianCamContainer").transform.GetChild(0).gameObject.SetActive(true);
		
//		if(EffectManager.getInstance.m_bBlurOn)
//			EffectManager.getInstance.SwitchBlur ();

		Transform sallyPathParent = GameObject.Find ("Stage(Clone)").transform.Find("sallyPaths").gameObject.transform;
		for (int i =0; i< sallyPathParent.childCount; ++i) {
			if(sceneStatus.m_bMemoryStage)
				sallyPathParent.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(71/255f, 63/255f, 43/255f, 0);
			else
				sallyPathParent.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
		}

	}

	public void FatherEnter()
	{
		SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.FATHER_ENTER;

//		StartCoroutine (GameObject.Find ("Main Camera").GetComponent<CamMoveMgr> ().SwipeMove ());

		if(StageLoader.getInstance.m_bStageLoader)
			ChangeBackgrounds (true);




		GameObject.Find("GuardianCamContainer").transform.GetChild(0).gameObject.SetActive(true);
//		if (GameObject.Find ("Photo(Clone)") != null) {
//			GameObject.Find("Photo(Clone)").GetComponent<SpriteRenderer>().enabled = true;
//		}

//		Transform spikeTrans = GameObject.Find ("Spikes").transform;
//		for(int i = 0; i < spikeTrans.childCount; ++i) {
//			spikeTrans.GetChild (i).GetComponent<Spike> ().StopAllCoroutines();
//			spikeTrans.GetChild(i).GetComponent<Spike> ().SpikeToOriginPos();
//		}
//
//		Transform springParent = GameObject.Find ("Springs").transform;
//		for(int i = 0; i < springParent.childCount; ++i) {
//			springParent.GetChild (i).GetComponent<Spring> ().Fix();
//		}


		GameObject.Find ("R_Goal(Clone)").GetComponent<GoalMgr> ().ResetVehicle ();
	}

	GameObject RightTutoActivator = null;
	GameObject LeftTutoActivator = null;
	public GameObject TutoWall = null;
	public void GuardianStart()
	{
		UICamera.selectedObject = GameObject.FindGameObjectWithTag ("UI Root");

		SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.GUARDIAN;
		//GameObject.Find("Main Camera").GetComponent<CamMoveMgr>().ToGuardianCam();
		GameObject.Find ("Guardian(Clone)").GetComponent<Rigidbody2D> ().isKinematic = false;
//		GameObject.Find ("Guardian(Clone)").GetComponent<Guardian> ().InitalizeDistanceFade ();

		GameObject.Find("GuardianCamContainer").transform.GetChild(0).gameObject.SetActive(true);

		SceneStatus.getInstance.fInGameTimer = 0f;

		if (m_iCurChpt.Equals (1) && m_iCurStage.Equals (1)) {
			if (m_iCurAct.Equals (1) && RightTutoActivator == null) {
				RightTutoActivator = Instantiate (Resources.Load ("Prefabs/Objects/FatherTutorialActivator")as GameObject)as GameObject;
				RightTutoActivator.transform.position = GameObject.Find ("Guardian(Clone)").transform.position;
				RightTutoActivator.GetComponent<FatherTutorialActivator> ().m_Tuto = FatherTutorialActivator.TUTO.RIGHT;
				RightTutoActivator.name = "RightTutoActivator";
			} else if (m_iCurAct.Equals (2) && LeftTutoActivator == null) {
				LeftTutoActivator = Instantiate (Resources.Load ("Prefabs/Objects/FatherTutorialActivator")as GameObject)as GameObject;
				LeftTutoActivator.transform.position = new Vector3 (59.75f, -0.5f);
				LeftTutoActivator.GetComponent<BoxCollider2D>().size = new Vector2(8,1);
				LeftTutoActivator.GetComponent<FatherTutorialActivator> ().m_Tuto = FatherTutorialActivator.TUTO.LEFT;
				LeftTutoActivator.name = "LeftTutoActivator";

				TutoWall = Instantiate (Resources.Load ("Prefabs/Objects/InvisibleTutoWall")as GameObject)as GameObject;
				TutoWall.transform.position = new Vector3 (64f, -0.5f);
				TutoWall.name = "InvisibleTutoWall";

				GameObject FastForwardTuto = Instantiate (Resources.Load ("Prefabs/Objects/FatherTutorialActivator")as GameObject)as GameObject;
				FastForwardTuto.transform.position = new Vector3 (73.4f, -0.5f);
				FastForwardTuto.GetComponent<BoxCollider2D>().size = new Vector2(6.5f,1);
				FastForwardTuto.GetComponent<FatherTutorialActivator> ().m_Tuto = FatherTutorialActivator.TUTO.FASTFORWARD;
				FastForwardTuto.name = "FastForwardTutoActivator";
			}
		} else if (m_iCurChpt.Equals (1) && m_iCurStage.Equals (2) && m_iCurAct.Equals (1)) {
			GameObject.Find("StartVehicle(Clone)").transform.GetChild(1).GetComponent<SkeletonAnimation>().state.SetAnimation(0,"move",false);
		}

		GameObject.Find ("FastForward").transform.GetChild (0).gameObject.SetActive (true);
		if((m_iCurChpt.Equals (1) && m_iCurStage.Equals (1) && m_iCurAct.Equals (1)) || (m_iCurChpt.Equals (1) && m_iCurStage.Equals (1) && m_iCurAct.Equals (2)))
			GameObject.Find ("FastForward").transform.GetChild (0).gameObject.GetComponent<UISprite>().enabled = false;

		GameObject.Find("SidePanel").GetComponent<UIPanel>().alpha = 1f;

//		UIManager.getInstance.TryFatherBtn (true);
		
//		if(GameObject.Find ("Main Camera").GetComponent<ColorCorrectionCurves>())
//			GameObject.Find ("Main Camera").GetComponent<ColorCorrectionCurves>().enabled = true;

		if(!Camera.main.GetComponent<CamMoveMgr>().m_bRunnerInScreen)
			StartCoroutine (GameObject.Find ("SallyTracker").GetComponent<SallyTracker> ().SallyTrackTracking ());

	}


	public void ChangeBackgrounds(bool bToFatherView, bool bChgBackGround = false)
	{

		Transform StageTrans = GameObject.Find ("Objects").transform.GetChild (GameMgr.getInstance.m_iCurAct - 1).transform;

		if (bChgBackGround && GameObject.Find ("Backgrounds") != null) {
			GameObject.Find ("Backgrounds").gameObject.BroadcastMessage ("ChgBg", bToFatherView,SendMessageOptions.DontRequireReceiver);
			GameObject.Find("TextFloatUI").BroadcastMessage("ChgTextColorToFather",SendMessageOptions.DontRequireReceiver);
		}

		Transform boxes = StageTrans.Find ("Boxes");
		if(boxes != null)
			boxes.gameObject.BroadcastMessage ("ChgBg", bToFatherView, SendMessageOptions.DontRequireReceiver);

		Transform doors = StageTrans.Find ("Doors");
		if(doors != null)
			doors.gameObject.BroadcastMessage ("ChgBg", bToFatherView ,SendMessageOptions.DontRequireReceiver);

		if(StageTrans.Find ("Springs") != null)
			StageTrans.Find ("Springs").gameObject.BroadcastMessage ("ChgBg", bToFatherView, SendMessageOptions.DontRequireReceiver);

		if(StageTrans.Find ("Spikes") != null)
			StageTrans.Find ("Spikes").gameObject.BroadcastMessage ("ChgBg", bToFatherView, SendMessageOptions.DontRequireReceiver);

		if(StageTrans.Find ("TopDecos") != null)
			StageTrans.Find ("TopDecos").gameObject.BroadcastMessage ("ChgBg", bToFatherView, SendMessageOptions.DontRequireReceiver);

		StageTrans.Find ("Goal").BroadcastMessage ("ChgBg", bToFatherView, SendMessageOptions.DontRequireReceiver);

	}

	public void PurchaseComplete(bool bInit = false)
	{
		GameMgr.getInstance.m_bPurchaseCostume = true;
		PlayerPrefs.SetInt ("PurchaseCostume", 1);
		
		if (!Application.loadedLevelName.Equals ("Main"))
			return;
		
		for (int j = 5; j <8; ++j) {
			Transform PhotoTrans = GameObject.Find ("PhotoList").transform.GetChild (j);
			//			for (int i = 2; i < 8; ++i) {
			//				PhotoTrans.GetChild (1).GetChild (i).GetComponent<UISprite> ().GrayScale (false);
			//			}
			
			PhotoTrans.GetChild (0).Find ("Lock").gameObject.SetActive (false);
			
			PhotoTrans.GetComponent<UIPhoto>().m_bAllCollected = true;

			bool bGotAchiv = true;
			
			for (int i = 0; i < 5; ++i) {
				for (int k = 0; k < 6; ++k) {
					bGotAchiv = m_PhotoInfo[i].m_bPhotoGet[k];
					
					if(!bGotAchiv)
						break;
				}
				if(!bGotAchiv)
					break;
			}

			if(!bGotAchiv)
			{
				PhotoTrans.GetChild (0).Find ("Lock").gameObject.SetActive (true);
			}

		}
		
		if(!bInit && m_uiStatus.Equals(MainUIStatus.REALLY_BUY))
			StartCoroutine (MainScene.getInstance.ToReallyBuy (false));
	}

	#if UNITY_STANDALONE || UNITY_WEBGL

//	GameObject invisibleObject = Resources.Load ("Prefabs/invisibleObject")as GameObject;

//	void OnLevelWasLoaded(int level)
//	{
//		if (!(invisibleObject != null))
//		{
//			return;
//		}
//
//		if (level.Equals(0)||level.Equals(1)||level.Equals(2)||level.Equals(3))
//		{
//		    return;
//		}
//
//		Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
//		foreach(Camera camera in cameras)
//		{
//			GameObject invi = Instantiate(invisibleObject, camera.transform.position + camera.transform.forward * 1f, camera.transform.rotation) as GameObject;
//			invi.transform.parent = camera.transform;
//		}
//	}
#endif
}

[System.Serializable]
public class PhotoInfo{
	public bool[] m_bPhotoGet = new bool[6];
}
