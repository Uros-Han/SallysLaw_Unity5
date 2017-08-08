using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneStatus : MonoBehaviour {


	private static SceneStatus instance;
	
	public static SceneStatus getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(SceneStatus)) as SceneStatus;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("SceneStatus");
				instance = obj.AddComponent (typeof(SceneStatus)) as SceneStatus;
			}
			
			return instance;
		}
	}

	void Awake(){
		if (instance == null)
			instance = this;
		
		else if (instance != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad (gameObject);

		UICamera.selectedObject = GameObject.FindGameObjectWithTag ("UI Root");

		GameMgr gMgr = GameMgr.getInstance;



#if UNITY_STANDALONE
		GameObject.Find ("BGM").GetComponent<AudioSource> ().volume = PlayerPrefs.GetFloat("MusicVolume");

		if(GameObject.Find ("AMB") != null)
			GameObject.Find ("AMB").GetComponent<AudioSource> ().volume = PlayerPrefs.GetFloat("SoundVolume");
#else
		if (gMgr.m_bBgmMute)
			GameObject.Find ("BGM").GetComponent<AudioSource> ().mute = true;
		
		if (gMgr.m_bSoundMute) {
			if(GameObject.Find ("AMB") != null)
				GameObject.Find ("AMB").GetComponent<AudioSource> ().mute = true;
		}
#endif

		if (gMgr.m_iCurChpt.Equals (4)) {
			switch(gMgr.m_iCurStage){
				
			case 2:
				GameObject.Find("RAIN_00").GetComponent<ParticleSystem>().emissionRate = 0f;
				break;
				
			case 3:
				GameObject.Find("RAIN_00").GetComponent<ParticleSystem>().emissionRate = 300f;
				GameObject.Find("AMB").GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/Chapter4/amb_rain_s");
				break;
				
			case 4:
				GameObject.Find("RAIN_00").GetComponent<ParticleSystem>().emissionRate = 1000f;
				GameObject.Find("AMB").GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/Chapter4/amb_rain_l");
				break;
				
			case 5:
				GameObject.Find("RAIN_00").GetComponent<ParticleSystem>().emissionRate = 300f;
				GameObject.Find("AMB").GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/Chapter4/amb_rain_s");
				break;
				
			case 6:
				GameObject.Find("RAIN_00").GetComponent<ParticleSystem>().emissionRate = 0f;
				break;
				
			}
		}


		gMgr.m_uiStatus = MainUIStatus.STAGE;

	}
	
	void OnDestroy()
	{
		StopAllCoroutines ();
		instance = null;
		
	}

//	bool m_bESC;
	public TAP_STATUS m_TabStat;
	public PLAYER_STATUS m_enPlayerStatus;

	public List<float> m_fStageXPos; //점점 스테이지 생성될때마다 뒤로밀리는데 그거 여기다 적용 (현재 맵의 최우단)
	public List<float> m_fStageXPosLeftest; //현재 맵의 최좌단

	public bool m_bFirstLoaded; //최초로 불러오는 애 m_fCurStageXPosLeftest 가 0이 되도록도와줌 , stage Clear할때까지 true상태

	public int m_iFlashBackIdx;
	public GameObject m_objCurStage; // Current Stage GameObject;

	public float fInGameTimer; //for debug
	public bool m_bMemoryStage;
	public bool m_bFinaleStage;

	public int m_iRestartInThisStage;
	public int m_iJumpCount;
	public int m_iRemoveSpikeCount;

	public bool m_bJumpAchievementGet;
	public bool m_bSpikeAchievementGet;

	// Use this for initialization
	void Start () {

		m_iRestartInThisStage = 0;

		if (Application.loadedLevelName.Contains ("Memory"))
			m_bMemoryStage = true;

		GameMgr gMgr = GameMgr.getInstance;
		if (gMgr.m_iCurChpt.Equals (5) && gMgr.m_iCurStage.Equals (6)) {
			m_bFinaleStage = true;
			GameObject.Find("GuardianCamContainer").transform.GetChild(0).gameObject.SetActive(true);

			GameObject CreditObj = Instantiate (Resources.Load ("Prefabs/UI/CreditPanel") as GameObject) as GameObject;
			CreditObj.transform.parent = GameObject.Find("Stretcher").transform;
			CreditObj.GetComponent<TweenAlpha> ().duration = 1f;
			CreditObj.BroadcastMessage("TweenActivate", true);

			GameObject CreditExitBtnObj = Instantiate (Resources.Load ("Prefabs/UI/CreditExit") as GameObject) as GameObject;
			CreditExitBtnObj.transform.parent = GameObject.Find("Stretcher").transform;
			CreditExitBtnObj.GetComponent<UIAnchor> ().uiCamera = GameObject.Find ("Camera").GetComponent<Camera> ();
			CreditExitBtnObj.transform.localScale = new Vector3(0.8f, 0.8f);
//			CreditExitBtnObj.transform.GetChild(0).GetComponent<UISprite> ().color = new Color (1, 1, 1, 205 / 255f);
		}

		Instantiate (ObjectPool.getInstance.m_MegamanEffect);


		if(Application.loadedLevelName != "MapTool")
			m_enPlayerStatus = PLAYER_STATUS.SALLY_WAIT;
		else
			m_enPlayerStatus = PLAYER_STATUS.SALLY_WAIT;

		m_TabStat = TAP_STATUS.JUMP;

		Time.timeScale = 1f;

		m_bFirstLoaded = true;



		m_fStageXPos = new List<float> ();
		m_fStageXPosLeftest = new List<float> ();
		
		
		if (StageLoader.getInstance.m_bStageLoader) {
			SceneStatus.getInstance.m_fStageXPos.Add (System.Convert.ToInt32 (GameMgr.getInstance.m_ListStage_string [0].m_strWidthOfThisMap) * 0.5f);
			SceneStatus.getInstance.m_fStageXPosLeftest.Add(0f);
		}

		#if UNITY_STANDALONE
		if(PC_InputControl.getInstance.GetInputState() == PC_InputControl.eInputState.Controler)
			GameObject.Find("UI Root").BroadcastMessage("SwapController", false ,SendMessageOptions.DontRequireReceiver);
		
		#endif

		StartCoroutine(VolumeUpWait());
	}

	IEnumerator VolumeUpWait()
	{
		yield return new WaitForSeconds (0.1f);
		StartCoroutine(AudioMgr.getInstance.VolumeChg (true));
	}


	bool bPaused = false;
	void OnApplicationPause(bool pause)
	{
		if (pause) {
			bPaused = true;
			// todo : 어플리케이션을 내리는 순간에 처리할 행동들 /
		} else {
			if (bPaused) {
				bPaused = false;
				//todo : 내려놓은 어플리케이션을 다시 올리는 순간에 처리할 행동들 

				if(GameObject.Find ("Pause") != null)
				{
					if (!GameObject.Find ("Pause").transform.GetChild (0).GetComponent<StagePauseBtn> ().m_bPauseOn) {
						GameObject.Find ("Pause").transform.GetChild (0).GetComponent<StagePauseBtn> ().OnClick ();
					}
				}
			}
		}
	}

	IEnumerator DebugTimer()
	{
		do {
			fInGameTimer += Time.deltaTime;
			yield return null;
		} while(true);
	}

}
