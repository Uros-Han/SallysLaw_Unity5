using UnityEngine;
using System.Collections;

public class MainScene : MonoBehaviour {

	private static MainScene instance;
	
	void OnApplicationQuit()
	{
		instance = null;
	}

	void Awake () {
		if(GameObject.Find("GameMgr") == null) //if gameMgr doesn't exist, make one.
		{
			GameObject gameMgr = Instantiate(Resources.Load("Prefabs/GameMgr") as GameObject) as GameObject;
			gameMgr.name = gameMgr.name.Replace("(Clone)","");
		}
		
		Time.timeScale = 1f;
	}
	

	public static MainScene getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(MainScene)) as MainScene;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("MainScene");
				instance = obj.AddComponent (typeof(MainScene)) as MainScene;
			}
			
			return instance;
		}
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}


	GameMgr gameMgr;
	GameObject m_Background;
	GameObject m_MainPanel;
	GameObject m_SideButtonsPanel;
	GameObject m_CharacterPanel;
	GameObject m_CharacterSymbolPanel;
	GameObject m_ChptPanel;
	GameObject m_OptionPanel;
	GameObject m_CreditPanel;
	GameObject m_LanguagePanel;
	GameObject m_RestorePanel;
	GameObject m_RestoreCheckPanel;
	GameObject m_PhotoPanel;
	GameObject m_ReallyBuyPanel;
	GameObject m_ReallyQuitPanel;
	GameObject m_ControlPanel;

	float fWorldToStageDelay = 0.35f;

	// Use this for initialization
	void Start () {

		#if UNITY_STANDALONE || UNITY_WEBGL
		GameObject.Find ("BGM").GetComponent<AudioSource> ().volume = PlayerPrefs.GetFloat ("MusicVolume");
		GameObject.Find ("BGM").GetComponent<AudioSource> ().Play ();

		if(PC_InputControl.getInstance.GetInputState() == PC_InputControl.eInputState.Controler)
			GameObject.Find("UI Root").BroadcastMessage("SwapController", false ,SendMessageOptions.DontRequireReceiver);
#else

		if (!GameMgr.getInstance.m_bBgmMute) {
			GameObject.Find ("BGM").GetComponent<AudioSource> ().Play ();
		}

#endif

#if !UNITY_EDITOR
//		if (PlayerPrefs.GetInt ("RatePopUp").Equals (1)) {
//			if(PlayerPrefs.GetInt("RateMainConter") < 2){
//				PlayerPrefs.SetInt("RateMainConter", PlayerPrefs.GetInt("RateMainConter") + 1);
//			}else{
//				MobileNativeRateUs ratePopUp = new MobileNativeRateUs(Localization.Get("LikeThis"), Localization.Get("RateUs"), Localization.Get("Rate"), Localization.Get("Later"), Localization.Get("No"));
//
//				#if UNITY_IOS
//				ratePopUp.SetAppleId("1108327399");
//				#elif UNITY_ANDROID
//				ratePopUp.SetAndroidAppUrl("onestore://common/product/OA00705923?view_type=1");
//
//				#endif
//				ratePopUp.Start();
//
//				ratePopUp.OnComplete += OnRatePopUpClose;
//
//
//
//				PlayerPrefs.SetInt("RateMainConter", 0);
//			}
//
//		}
#endif


		gameMgr = GameMgr.getInstance;

		m_Background = GameObject.Find ("Background").gameObject;
		m_MainPanel = GameObject.Find ("MainPanel").gameObject;
		m_SideButtonsPanel = GameObject.Find ("SideButtonsPanel").gameObject;
		m_CharacterPanel = GameObject.Find ("CharacterPanel").gameObject;
		m_CharacterSymbolPanel = GameObject.Find ("CharacterSymbolPanel").gameObject;
		m_ChptPanel = GameObject.Find ("ChptPanel").gameObject;
		m_OptionPanel = GameObject.Find ("OptionPanel").gameObject;
		m_CreditPanel = GameObject.Find ("CreditPanel").gameObject;
		m_LanguagePanel = GameObject.Find ("LanguagePanel").gameObject;
		m_RestorePanel = GameObject.Find ("RestorePanel").gameObject;
		m_RestoreCheckPanel = GameObject.Find ("RestoreCheckPanel").gameObject;
		m_PhotoPanel = GameObject.Find ("PhotoPanel").gameObject;
		m_ReallyBuyPanel = GameObject.Find ("ReallyBuyPanel").gameObject;
		m_ReallyQuitPanel = GameObject.Find ("ReallyQuitPanel").gameObject;
		m_ControlPanel = GameObject.Find ("ControlPanel").gameObject;

//		m_SideButtonsPanel.transform.FindChild ("ButtonsPanel").FindChild ("RightBottom2").GetComponent<UILabel> ().text = gameMgr.m_strVersion;


		switch (gameMgr.m_uiStatus) {
		case MainUIStatus.STAGE:
			m_ChptPanel.GetComponent<UIPanel>().alpha = 1f;
			m_CharacterSymbolPanel.GetComponent<UIPanel>().alpha = 0f;
			m_CharacterPanel.GetComponent<UIPanel>().alpha = 0f;
			m_SideButtonsPanel.GetComponent<UIPanel>().alpha = 1f;
			m_MainPanel.GetComponent<UIPanel>().alpha = 0f;
			MainToWorld();
			WorldToStage(true);
			break;

		case MainUIStatus.WORLD:
			m_ChptPanel.GetComponent<UIPanel>().alpha = 1f;
			m_CharacterSymbolPanel.GetComponent<UIPanel>().alpha = 0f;
			m_CharacterPanel.GetComponent<UIPanel>().alpha = 0f;
			m_SideButtonsPanel.GetComponent<UIPanel>().alpha = 1f;
			m_MainPanel.GetComponent<UIPanel>().alpha = 0f;
			MainToWorld();
			WorldToStage(true);
			break;
		}

		StartCoroutine (Loop ());


	}

//	private void OnRatePopUpClose(MNDialogResult result) {
//		//parsing result
//		switch(result) {
//		case MNDialogResult.RATED:
//			PlayerPrefs.SetInt ("RatePopUp", 0);
//			break;
//		case MNDialogResult.REMIND:
//			Debug.Log ("Remind Option picked");
//			break;
//		case MNDialogResult.DECLINED:
//			PlayerPrefs.SetInt ("RatePopUp", 0);
//			break;
//		}
//	}

	IEnumerator Loop() 
	{

		AudioSource bgm = GameObject.Find ("BGM").GetComponent<AudioSource> ();
		GameMgr gameMgr = GameMgr.getInstance;


		do{
			yield return null;

#if !(UNITY_STANDALONE || UNITY_WEBGL)
			if (gameMgr.m_bBgmMute && !bgm.mute)
				bgm.mute = true;
			else if(!gameMgr.m_bBgmMute && bgm.mute)
				bgm.mute = false;
#endif

			switch(gameMgr.m_uiStatus)
			{
			case MainUIStatus.MAIN:
				yield return StartCoroutine(MainUI());
				break;

			case MainUIStatus.WORLD:
				yield return StartCoroutine(WorldUI());
				break;

			case MainUIStatus.STAGE:
				yield return StartCoroutine(StageUI());
				break;
			}

		}while(true);

	}

	IEnumerator MainUI()
	{

		do{
			yield return StartCoroutine (TabCheck ());

			yield return null;
		}while(gameMgr.m_uiStatus.Equals(MainUIStatus.MAIN));
	}

	IEnumerator WorldUI()
	{
//		GameObject.Find ("Logo").transform.GetChild (0).GetComponent<TweenAlpha> ().delay = fWorldToStageDelay;
//		GameObject.Find ("Logo").BroadcastMessage ("TweenActivate", false);

		yield return new WaitForSeconds (fWorldToStageDelay - 0.1f);
		if(gameMgr.m_uiStatus.Equals(MainUIStatus.WORLD))
			GameObject.Find ("Logo").BroadcastMessage ("TweenActivate", false);

		while(gameMgr.m_uiStatus.Equals(MainUIStatus.WORLD)){

			if(Input.GetKeyDown(KeyCode.Escape))
			{
				ToMain();
			}
			
			yield return null;
		};
	}

	void Update()
	{

#if UNITY_STANDALONE || UNITY_WEBGL
		for (int i = 0;i < 20; i++) {
			if(Input.GetKeyDown("joystick button "+i)){
				print("joystick button "+i);
			}
		}
#endif
	}

	IEnumerator StageUI()
	{


		UICenterOnChild centerChild = GameObject.Find("Chapters").GetComponent<UICenterOnChild> ();
		centerChild.centeredObject = GameObject.Find("Chapters").transform.GetChild(gameMgr.m_iCurChpt-1).gameObject;
		GameObject beforeCenterObj = centerChild.centeredObject;
	 	m_ChptPanel.transform.GetChild(0).GetComponent<UIPanel> ().clipOffset = new Vector2 (-900f, 0);



		while(gameMgr.m_uiStatus.Equals(MainUIStatus.STAGE)){

			if(centerChild.centeredObject != beforeCenterObj)
			{
				centerChild.centeredObject.BroadcastMessage("GrowCircle",true);
				centerChild.centeredObject.transform.GetChild(3).GetComponent<TweenAlpha>().Play(true);

//				centerChild.centeredObject.BroadcastMessage("GrowBranch",true);
				//stage alpha on;
//				centerChild.centeredObject.transform.GetChild(2).GetComponent<TweenAlpha>().delay = 0f;
//				centerChild.centeredObject.transform.GetChild(2).GetComponent<TweenAlpha>().Play(true);

				beforeCenterObj.BroadcastMessage("GrowCircle",false);
				beforeCenterObj.transform.GetChild(3).GetComponent<TweenAlpha>().Play(false);

//				beforeCenterObj.BroadcastMessage("GrowBranch",false);
//				beforeCenterObj.transform.GetChild(2).GetComponent<TweenAlpha>().delay = 0.5f;
//				beforeCenterObj.transform.GetChild(2).GetComponent<TweenAlpha>().Play(false);
			}

			beforeCenterObj = centerChild.centeredObject;
			yield return null;
		};

		Transform scrollTrans = GameObject.Find("Chapters").transform.parent;
		Transform ChaptTrans = GameObject.Find ("Chapters").transform;

		scrollTrans.GetComponent<TweenPosition> ().from = scrollTrans.localPosition;
		scrollTrans.GetComponent<TweenPosition> ().ResetToBeginning ();
		scrollTrans.GetComponent<TweenPosition> ().delay = fWorldToStageDelay;
		scrollTrans.GetComponent<TweenPosition> ().PlayForward();
		scrollTrans.GetComponent<UIPanel> ().clipOffset = new Vector2 (-900f, 0);

		centerChild.centeredObject.BroadcastMessage ("GrowCircle", false);
		centerChild.centeredObject.transform.GetChild(3).GetComponent<TweenAlpha>().Play(false);

//		centerChild.centeredObject.BroadcastMessage ("GrowBranch", false);
//		for (int i = 0; i < ChaptTrans.childCount; ++i) {
//			if(i != 5)
//				ChaptTrans.GetChild(i).GetChild(2).GetComponent<TweenAlpha>().Play(false);
//		}
	}

	public IEnumerator WorldToStage(bool bToStage)
	{
		GameMgr.getInstance.m_uiStatus = MainUIStatus.END;

		GameObject ChptLine = GameObject.Find ("ChptLine").gameObject;

		int iCurChpt = gameMgr.m_iCurChpt;

		if (bToStage) {
			m_SideButtonsPanel.transform.GetChild(1).GetComponent<TweenAlpha>().Play(true);
			GameObject.Find ("Scroll").GetComponent<UIScrollView> ().enabled = true;
		} else {
			m_SideButtonsPanel.transform.GetChild(1).GetComponent<TweenAlpha>().Play(false);
			GameObject.Find ("Scroll").GetComponent<UIScrollView> ().enabled = false;
		}


		//Line Positioning with X
		if(bToStage)
			ChptLine.GetComponent<TweenPosition> ().to = new Vector3 (3600f - ((iCurChpt -1) * 1800f) ,0);


//		TweenAlpha stageListTween = GameObject.Find("Chapters").transform.GetChild(iCurChpt-1).GetChild(2).gameObject.GetComponent<TweenAlpha>();

		if (bToStage) {
//			stageListTween.delay = 0f;
		
			//Background sprite alpha
			m_Background.transform.GetChild (0).SendMessage ("TweenActivate", true);
			m_Background.transform.GetChild (gameMgr.m_iCurChpt).SendMessage ("TweenActivate", true);

			ChptLine.GetComponent<TweenScale>().delay = 0.5f;
			ChptLine.GetComponent<TweenPosition>().delay = 0.5f;

			m_ChptPanel.GetComponent<TweenPosition>().delay = 0.55f;
			m_ChptPanel.GetComponent<TweenPosition>().to = new Vector3(0, -150f);


		} else {
			StartCoroutine(StageUI());

//			stageListTween.delay = 0.5f;

			//Background sprite alpha
			m_Background.transform.GetChild (0).SendMessage ("TweenActivate", false);
			m_Background.transform.GetChild (iCurChpt).SendMessage ("TweenActivate", false);


			ChptLine.GetComponent<TweenScale>().delay = fWorldToStageDelay;
			ChptLine.GetComponent<TweenPosition>().delay = fWorldToStageDelay;
			m_ChptPanel.GetComponent<TweenPosition>().delay = fWorldToStageDelay;
			m_ChptPanel.GetComponent<TweenPosition>().to = m_ChptPanel.transform.localPosition;
		}

		ChptLine.GetComponent<TweenPosition> ().Play (bToStage);
		ChptLine.GetComponent<TweenScale>().Play(bToStage); //Line Scale

//		stageListTween.Play(bToStage); //stage list on

		m_ChptPanel.GetComponent<TweenPosition>().Play(bToStage); // line move up
	
		if (!bToStage) {
			yield return new WaitForSeconds (fWorldToStageDelay);
		}
	

		m_ChptPanel.BroadcastMessage("MoveOrder",bToStage);

		if (!bToStage) {
			yield return new WaitForSeconds (fWorldToStageDelay);
			gameMgr.m_uiStatus = MainUIStatus.WORLD;
		}else {
			yield return new WaitForSeconds (fWorldToStageDelay);

			GameObject.Find ("Logo").transform.GetChild (0).GetComponent<TweenAlpha> ().delay = 0f;
			GameObject.Find ("Logo").BroadcastMessage ("TweenActivate", true);
			
			//First stage button branch on with delay
			yield return StartCoroutine ("FirstBranchwithDelay", 0f);

			yield return new WaitForSeconds (fWorldToStageDelay + 0.5f);

			gameMgr.m_uiStatus = MainUIStatus.STAGE;
		}

		yield return null;
	}

	IEnumerator FirstBranchwithDelay(float fDelay)
	{
		yield return new WaitForSeconds (fDelay);

		GameObject.Find("Chapters").transform.GetChild(gameMgr.m_iCurChpt-1).GetChild(2).BroadcastMessage ("GrowCircle", true);
		GameObject.Find("Chapters").transform.GetChild(gameMgr.m_iCurChpt-1).GetChild(3).GetComponent<TweenAlpha>().Play(true);


		
		//		GameObject.Find("Chapters").transform.GetChild(gameMgr.m_iCurChpt-1).GetChild(2).BroadcastMessage ("GrowBranch", true);

	}

	public void ToMain()
	{
		m_CharacterSymbolPanel.SetActive (true);
		m_SideButtonsPanel.BroadcastMessage ("TweenActivate", false);
		m_ChptPanel.BroadcastMessage ("TweenActivate", false);
		SymbolOnorOff (false);
		
		m_ChptPanel.GetComponent<TweenAlpha> ().onFinished.Add (new EventDelegate(this, "WorldToMain"));
		
		gameMgr.m_uiStatus = MainUIStatus.CHAR_MOVING;
	}

	IEnumerator TabCheck()
	{
		bool bTab = false;
		do{

			if((Input.GetKeyUp(KeyCode.Return)
			    #if UNITY_STANDALONE_WIN
			    || Input.GetKeyUp (KeyCode.JoystickButton0))
			   #elif UNITY_STANDALONE_OSX
			    || Input.GetKeyUp (KeyCode.JoystickButton16))
				#else
				|| Input.GetMouseButtonUp(0))
				#endif 
				&& gameMgr.m_uiStatus.Equals(MainUIStatus.MAIN))
				bTab = true;

			yield return null;

		}while(!bTab);

		m_MainPanel.BroadcastMessage ("TweenActivate", true);
		m_CharacterPanel.GetComponent<TweenAlpha> ().duration = 1.45f;
		m_CharacterPanel.BroadcastMessage ("TweenActivate", true);
		m_CharacterSymbolPanel.BroadcastMessage ("TweenActivate", true);

		m_CharacterSymbolPanel.GetComponent<TweenAlpha> ().onFinished.Add (new EventDelegate(this, "MainToWorld"));

		gameMgr.m_uiStatus = MainUIStatus.CHAR_MOVING;
	}

	public void SymbolOnorOff(bool bOff)
	{
		if (bOff) {
			m_CharacterSymbolPanel.GetComponent<TweenAlpha>().Play(false);
		} else {
			m_CharacterSymbolPanel.GetComponent<UIPanel>().alpha = 1f;
			m_CharacterSymbolPanel.GetComponent<TweenAlpha>().Play(true);
		}
	}

	public void MainToWorld()
	{
		m_ChptPanel.BroadcastMessage ("TweenActivate", true);
		m_SideButtonsPanel.BroadcastMessage ("TweenActivate", true);
		SymbolOnorOff (true);

		StartCoroutine(ClearFinished(true));

		gameMgr.m_uiStatus = MainUIStatus.WORLD;
	}

	public IEnumerator ToReallyQuit(bool bToReallyQuit)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END))
			yield break;

		if (bToReallyQuit) {
			gameMgr.m_uiStatus = MainUIStatus.END;
			GameObject.Find ("Logo").transform.GetChild (0).GetComponent<TweenAlpha>().delay = 0f;
			GameObject.Find ("Logo").transform.GetChild (0).SendMessage("TweenActivate", true);
			m_MainPanel.BroadcastMessage ("TweenActivate", true);
			m_CharacterPanel.GetComponent<TweenAlpha> ().duration = 0.3f;
			m_CharacterPanel.SendMessage ("TweenActivate", true);
			
			yield return new WaitForSeconds(0.25f);
			
			m_ReallyQuitPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.REALLY_QUIT;
		} else {
			gameMgr.m_uiStatus = MainUIStatus.END;
	
			m_ReallyQuitPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);

			m_MainPanel.BroadcastMessage ("TweenActivate", false);
			m_CharacterPanel.SendMessage ("TweenActivate", false);
			GameObject.Find ("Logo").transform.GetChild (0).SendMessage("TweenActivate", false);

			gameMgr.m_uiStatus = MainUIStatus.MAIN;
			
		}
		
		yield return null;
	}

	public IEnumerator ToOption(bool bToOption)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END) || gameMgr.m_uiStatus.Equals (MainUIStatus.CHAR_MOVING))
			yield break;

		if (bToOption) {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_ChptPanel.BroadcastMessage ("TweenActivate", false);
			m_SideButtonsPanel.transform.Find("ButtonsPanel").GetComponent<TweenAlpha>().Play();
			GameObject.Find ("Logo").transform.GetChild (0).GetComponent<TweenAlpha>().delay = 0f;
			GameObject.Find ("Logo").transform.GetChild (0).SendMessage("TweenActivate", true);

			if(GameObject.Find("iCloud") != null)
				GameObject.Find("iCloud").transform.GetChild(1).GetComponent<ToggleSwitch>().CloudConnectCheck();

			#if UNITY_STANDALONE || UNITY_WEBGL
			GameObject.Find("Option_PC").GetComponent<Option_PC>().Start();
			#endif

			yield return new WaitForSeconds(0.4f);

			m_OptionPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.OPTION;
		} else {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_OptionPanel.BroadcastMessage("TweenActivate", false);

			#if UNITY_STANDALONE || UNITY_WEBGL
			GameObject.Find("Cursor").transform.position = GameObject.Find("RightTop").transform.position;
			GameObject.Find("Cursor").transform.localPosition += new Vector3(0, 75f);
			#endif

			yield return new WaitForSeconds(0.3f);

			m_ChptPanel.BroadcastMessage ("TweenActivate", true);
			m_SideButtonsPanel.transform.Find("ButtonsPanel").GetComponent<TweenAlpha>().Play(false);
			GameObject.Find ("Logo").transform.GetChild (0).SendMessage("TweenActivate", false);

			if(m_Background.transform.GetChild(0).GetComponent<UISprite>().color.a == 1)
				gameMgr.m_uiStatus = MainUIStatus.WORLD;
			else
				gameMgr.m_uiStatus = MainUIStatus.STAGE;

		}

		yield return null;
	}

	public IEnumerator ToPhoto(bool bToPhoto)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END) || gameMgr.m_uiStatus.Equals (MainUIStatus.CHAR_MOVING))
			yield break;

		if (bToPhoto) {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_ChptPanel.BroadcastMessage ("TweenActivate", false);
			m_SideButtonsPanel.transform.Find("ButtonsPanel").GetComponent<TweenAlpha>().Play();
			GameObject.Find ("Logo").transform.GetChild (0).GetComponent<TweenAlpha>().delay = 0f;
			GameObject.Find ("Logo").transform.GetChild (0).SendMessage("TweenActivate", true);

			for (int i = 0; i <5; ++i) {
				Transform PhotoTrans = GameObject.Find ("PhotoList").transform.GetChild (i);

				if(PlayerPrefs.GetInt (string.Format ("PhotoAllCollected{0}", i)).Equals (1))
					PhotoTrans.GetComponent<UIPhoto>().PhotoCollectCheck();
			}

			GameObject.Find ("PhotoList").transform.localPosition = Vector3.zero;
			
			yield return new WaitForSeconds(0.4f);

			if(gameMgr.m_bPurchaseCostume)
				GameMgr.getInstance.PurchaseComplete(true);
			
			m_PhotoPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.PHOTO;

		} else {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_PhotoPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.3f);
			
			m_ChptPanel.BroadcastMessage ("TweenActivate", true);
			m_SideButtonsPanel.transform.Find("ButtonsPanel").GetComponent<TweenAlpha>().Play(false);
			GameObject.Find ("Logo").transform.GetChild (0).SendMessage("TweenActivate", false);
			
			gameMgr.m_uiStatus = MainUIStatus.WORLD;
			
		}
		
		yield return null;
	}

	public IEnumerator ToLanguage(bool bToLanguage)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END))
			yield break;

		if (bToLanguage) {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_OptionPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);
			
			m_LanguagePanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.LANGUAGE;
		} else {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_LanguagePanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);

			m_OptionPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.OPTION;
			
		}
		
		yield return null;
	}

	public IEnumerator ToControl(bool bToControl)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END))
			yield break;
		
		if (bToControl) {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_OptionPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);
			
			m_ControlPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.CONTROL;
		} else {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_ControlPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);
			
			m_OptionPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.OPTION;
			
		}
		
		yield return null;
	}

	public IEnumerator ToRestore(bool bToRestore)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END))
			yield break;

		if (bToRestore) {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_OptionPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);
			
			m_RestorePanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.RESTORE;
		} else {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_RestorePanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);
			
			m_OptionPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.OPTION;
			
		}
		
		yield return null;
	}

	public IEnumerator ToRestoreCheck(bool bToRestoreCheck)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END))
			yield break;

		if (bToRestoreCheck) {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_RestorePanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);
			
			m_RestoreCheckPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.RESTORE_CHECK;
		} else {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_RestoreCheckPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);

			for(int i =0; i < 3; ++i)
			{
				m_RestoreCheckPanel.transform.GetChild(i+1).gameObject.SetActive(false);
			}
			
			m_OptionPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.OPTION;
			
		}
		
		yield return null;
	}

	public void ToRestoreChecker(int iStatus)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END))
			return;

		StartCoroutine (MainScene.getInstance.ToRestoreCheck (true));

		if (iStatus.Equals(1)) { //SUCCESS
			m_RestoreCheckPanel.transform.GetChild (1).gameObject.SetActive (true);
		} else if (iStatus.Equals(2)) { // ERROR
			m_RestoreCheckPanel.transform.GetChild (2).gameObject.SetActive (true);
		} else if (iStatus.Equals(3)) { // ALREADY_RESTORED
			m_RestoreCheckPanel.transform.GetChild (3).gameObject.SetActive (true);
		}
	}

	public IEnumerator ToCredit(bool bToCredit)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END))
			yield break;

		if (bToCredit) {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_OptionPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);

			StartCoroutine (m_CreditPanel.GetComponent<CreditPanel>().CreditUpward());

			m_CreditPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.CREDIT;
		} else {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_CreditPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);
			
			m_OptionPanel.BroadcastMessage("TweenActivate", true);;
			gameMgr.m_uiStatus = MainUIStatus.OPTION;
			
		}
		
		yield return null;
	}

	public IEnumerator ToReallyBuy(bool bToReallyBuy)
	{
		if (gameMgr.m_uiStatus.Equals (MainUIStatus.END))
			yield break;

		if (bToReallyBuy) {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_PhotoPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);
			
			m_ReallyBuyPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.REALLY_BUY;
		} else {
			gameMgr.m_uiStatus = MainUIStatus.END;
			m_ReallyBuyPanel.BroadcastMessage("TweenActivate", false);
			
			yield return new WaitForSeconds(0.25f);
			
			m_PhotoPanel.BroadcastMessage("TweenActivate", true);
			gameMgr.m_uiStatus = MainUIStatus.PHOTO;
			
		}
		
		yield return null;
	}



	public void PurchaseAttempt()
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
//		Storekit_Sally.BeginPurchase (StoreItemID._COSTUME01);

//#if UNITY_ANDROID
//		GameObject.Find ("OneStoreIapManager").GetComponent<OneStoreIapManager> ().CallPaymentRequest ("0910061189");
//#endif
	}

	public void CanclePurchase()
	{
		StartCoroutine (ToReallyBuy (false));
	}

	public void WorldToMain()
	{
		m_MainPanel.BroadcastMessage ("TweenActivate", false);
		m_CharacterPanel.BroadcastMessage ("TweenActivate", false);
		m_CharacterSymbolPanel.BroadcastMessage ("TweenActivate", false);

		StartCoroutine(ClearFinished(false));



		gameMgr.m_uiStatus = MainUIStatus.MAIN;
	}

	IEnumerator ClearFinished(bool ToMain)
	{
		yield return null;

		m_CharacterSymbolPanel.GetComponent<TweenAlpha> ().onFinished.Clear ();
		m_ChptPanel.GetComponent<TweenAlpha> ().onFinished.Clear ();
	}

	public void BlockPanel(bool bOn)
	{


		if (!Application.loadedLevelName.Equals ("Main"))
			return;

		GameObject.Find ("BlockPanel").BroadcastMessage ("TweenActivate", bOn);
	}


}
