using UnityEngine;
using System.Collections;

/// <summary>
/// 인게임 유아이 연출 관리자
/// </summary>
public class UIManager : MonoBehaviour {

	bool m_bOnTryFather = false;
	bool m_bOnTrySally = false;
	bool m_bPauseBlock = false;
	TweenPosition[] m_TweenPosition;


	private static UIManager instance;
	
	void OnApplicationQuit()
	{
		instance = null;
	}	
	
	public static UIManager getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(UIManager)) as UIManager;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("UIManager");
				instance = obj.AddComponent (typeof(UIManager)) as UIManager;
			}
			
			return instance;
		}
	}


	void Awake () {

		//m_TweenPosition = GameObject.Find("Container(Panel)").GetComponents<TweenPosition>();

//		StartCoroutine (CurSceneStatusChecker ());
//		StartCoroutine (StageCurtain ());

		m_PausePanel = GameObject.Find ("PausePanel");
		m_ReallyQuitPanel = GameObject.Find ("ReallyQuitPanel");
		m_StageInfoPanel = GameObject.Find ("StageInfoPanel");
		m_FatherWaitPanel = GameObject.Find ("FatherWaitPanel");
		m_PlayStartPanel = GameObject.Find ("PlayStartPanel");
		m_UIFadePanel = GameObject.Find ("UIFadePanel");
		m_LoadingPanel = GameObject.Find ("LoadingPanel");
	

		if (!StageLoader.getInstance.m_bMaptool) {
			m_PauseAlpha = m_PausePanel.GetComponent<TweenAlpha> ();
			m_ReallyQuitAlpha = m_ReallyQuitPanel.GetComponent<TweenAlpha> ();
			m_StageInfoAlpha = m_StageInfoPanel.GetComponent<TweenAlpha> ();
			m_InfoPanel = GameObject.Find ("InfoPanel");
			m_FastForwardPanel = GameObject.Find ("FastForwardPanel");
		}
		m_PlayStartAlpha = m_PlayStartPanel.GetComponent<TweenAlpha> ();
		m_UIFadeAlpha = m_UIFadePanel.GetComponent<TweenAlpha> ();
	}

	public GameObject m_PausePanel;
	public GameObject m_ReallyQuitPanel;
	public GameObject m_StageInfoPanel;
	public GameObject m_FatherWaitPanel;
	public GameObject m_PlayStartPanel;
	public GameObject m_UIFadePanel;
	public GameObject m_LoadingPanel;
	public GameObject m_InfoPanel;
	public GameObject m_FastForwardPanel;

	TweenAlpha m_PauseAlpha;
	TweenAlpha m_ReallyQuitAlpha;
	TweenAlpha m_StageInfoAlpha;
	TweenAlpha m_PlayStartAlpha;
	TweenAlpha m_UIFadeAlpha;


	//StageStart -> StageStartFinished -> SallyStart -> FatherWait -> FatherStart -> StageEnd

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	public void FatherTutorial(bool bActivate, int iTutoIdx)
	{
		if (iTutoIdx.Equals (0)) {
			m_FatherWaitPanel.transform.GetChild (0).SendMessage ("TweenActivate", bActivate, SendMessageOptions.DontRequireReceiver);
		} else if (iTutoIdx.Equals (1)) {
			m_FatherWaitPanel.transform.GetChild (1).SendMessage ("TweenActivate", bActivate, SendMessageOptions.DontRequireReceiver);
		} else if (iTutoIdx.Equals (2)) {
			m_FatherWaitPanel.transform.GetChild (2).SendMessage ("TweenActivate", bActivate, SendMessageOptions.DontRequireReceiver);

			if(bActivate)
			{
				GameObject.Find("SidePanel").GetComponent<UIPanel>().depth = 0;
				GameObject.Find("SidePanel").transform.GetChild(0).gameObject.SetActive(false);

#if !(UNITY_STANDALONE || UNITY_WEBGL)
				GameObject.Find ("FastForward").transform.GetChild (0).gameObject.GetComponent<UISprite>().enabled = true;

				GameObject.Find("FastForward").transform.GetChild(0).GetComponent<TweenAlpha>().ResetToBeginning();
				GameObject.Find("FastForward").transform.GetChild(0).GetComponent<TweenAlpha>().Play();
#endif

			}else{
				GameObject.Find("SidePanel").GetComponent<UIPanel>().depth = -2;
				GameObject.Find("SidePanel").transform.GetChild(0).gameObject.SetActive(true);

#if !(UNITY_STANDALONE || UNITY_WEBGL)
				GameObject.Find("FastForward").transform.GetChild(0).GetComponent<TweenAlpha>().enabled = false;
				GameObject.Find("FastForward").transform.GetChild(0).GetComponent<UISprite>().color = new Color(1,1,1,105/255f);
#endif
			}
		}
	}

	public void CurtainOn()
	{
		m_UIFadePanel.GetComponent<UIPanel> ().alpha = 1f; 	
	}

	public void StageStart()
	{
		m_StageInfoPanel.SendMessage("Labeling",SendMessageOptions.DontRequireReceiver);
		m_StageInfoAlpha.duration = 2f;

		m_StageInfoAlpha.from = 0f;
		m_StageInfoAlpha.to = 0.8f;

		m_StageInfoAlpha.ResetToBeginning ();
		m_StageInfoAlpha.Play (true);

//		TryFatherBtn(false);

		StartCoroutine (StageStartFinished ());
	}

	IEnumerator StageStartFinished()
	{
		yield return StartCoroutine(WaitForRealSeconds(2f));

		SallyWait ();
	}

	public void Loading(bool bOn)
	{
		if (bOn)
			m_LoadingPanel.GetComponent<UIPanel> ().alpha = 1;
		else
			m_LoadingPanel.GetComponent<UIPanel> ().alpha = 0;
	}

	public void SallyWait()
	{

		if (StageLoader.getInstance.m_bMaptool) {

			m_UIFadeAlpha.delay = 0f;
			m_PlayStartAlpha.delay = 0f;

			m_UIFadeAlpha.duration = 0.2f;
			m_PlayStartAlpha.duration = 0.2f;
		} else {
			m_UIFadeAlpha.delay = 0.5f;
			m_PlayStartAlpha.delay = 0.5f;
			
			m_UIFadeAlpha.duration = 1f;
			m_PlayStartAlpha.duration = 1f;
		}
		
		m_UIFadeAlpha.from = 1f;
		m_UIFadeAlpha.to = 0.6f;
		m_UIFadeAlpha.ResetToBeginning ();
		
		m_UIFadeAlpha.Play (true);
		m_PlayStartAlpha.Play (true);
	}

	public void FatherWait()
	{
		m_UIFadeAlpha.from = 0f;
		m_UIFadeAlpha.to = 0.6f;
		m_UIFadeAlpha.ResetToBeginning ();

//		m_FatherWaitAlpha.duration = 0.5f;
		m_PlayStartAlpha.duration = 0.5f;
		m_UIFadeAlpha.duration = 0.5f;

		m_UIFadeAlpha.Play (true);
		m_PlayStartAlpha.Play (true);
//		m_FatherWaitAlpha.Play (true);
	}

	public void StageEnd()
	{
		m_UIFadePanel.transform.GetChild (0).GetComponent<UISprite> ().color = Color.white;
		m_UIFadeAlpha.duration = 1f;
		m_UIFadeAlpha.delay = 0f;
		m_UIFadeAlpha.from = 0f;
		m_UIFadeAlpha.to = 1f;
		m_UIFadeAlpha.ResetToBeginning ();
		m_UIFadeAlpha.Play (true);

		StartCoroutine (StageEndFinished ());
	}

	public void SallyToFather()
	{
		m_UIFadeAlpha.duration = 1f;
		m_UIFadeAlpha.delay = 0f;
		m_UIFadeAlpha.from = 1f;
		m_UIFadeAlpha.to = 0f;
		m_UIFadeAlpha.ResetToBeginning ();
		m_UIFadeAlpha.Play (true);
		
		StartCoroutine (FatherReady ());
	}

	public void TutorialFade(bool bOn)
	{
		if (bOn) {
			m_UIFadeAlpha.from = 0f;
			m_UIFadeAlpha.to = 0.6f;
		} else {
			m_UIFadeAlpha.from = 0.6f;
			m_UIFadeAlpha.to = 0f;
		}
		m_UIFadeAlpha.ResetToBeginning ();

		m_UIFadeAlpha.duration = 0.5f;
		
		m_UIFadeAlpha.Play (true);
	}

	IEnumerator StageEndFinished()
	{
		yield return new WaitForSeconds (m_UIFadeAlpha.duration);


		GameMgr.getInstance.ToAlreadyCreatedNextAct (true);
	}

	IEnumerator FatherReady()
	{
		yield return new WaitForSeconds (m_UIFadeAlpha.duration);

		m_UIFadePanel.transform.GetChild (0).GetComponent<UISprite> ().color = Color.black;
		
		GameMgr.getInstance.FatherEnter ();
	}

	public void FatherStart()
	{
		m_PlayStartAlpha.delay = 0f;
		m_UIFadeAlpha.delay = 0f;

		m_PlayStartAlpha.duration = 0.5f;
		m_UIFadeAlpha.duration = 0.5f;

//		m_FatherWaitAlpha.Play (false);
		m_PlayStartAlpha.Play (false);

		if (m_StageInfoAlpha != null) {
			m_StageInfoAlpha.delay = 0f;
			m_StageInfoAlpha.duration = 0.5f;
		}

		m_UIFadeAlpha.from = 0.6f;
		m_UIFadeAlpha.to = 0f;
		m_UIFadeAlpha.ResetToBeginning ();
		if (Camera.main.GetComponent<CamMoveMgr>().m_bFirstTuto) {
			Camera.main.GetComponent<CamMoveMgr>().m_bFirstTuto = false;
		} else {
			m_UIFadeAlpha.Play (true);
		}

		SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.CAM_TO_FATHER;
	}

	public void SallyStart()
	{
			
		m_PlayStartAlpha.delay = 0f;
		m_UIFadeAlpha.delay = 0f;

		m_PlayStartAlpha.duration = 0.5f;
		m_UIFadeAlpha.duration = 0.5f;

		m_PlayStartAlpha.Play (false);

		if(m_StageInfoAlpha != null)
		{
			m_StageInfoAlpha.delay = 0f;
			m_StageInfoAlpha.duration = 0.5f;
			m_StageInfoAlpha.Play (false);
		}

		m_UIFadeAlpha.from = 0.6f;
		m_UIFadeAlpha.to = 0f;
		m_UIFadeAlpha.ResetToBeginning ();
		m_UIFadeAlpha.Play (true);

		TimeMgr.Play ();

		SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.RUNNER;
	}

	public void LoadScene(string LoadSceneName)
	{
		m_UIFadeAlpha.duration = 1f;
		if (SceneStatus.getInstance.m_bFinaleStage) { // GAME CLEAR
			m_UIFadeAlpha.duration = 5f;
		}

		m_UIFadeAlpha.delay = 0f;
		m_UIFadeAlpha.from = 0f;
		m_UIFadeAlpha.to = 1f;
		m_UIFadeAlpha.ResetToBeginning ();
		m_UIFadeAlpha.Play (true);

		if (SceneStatus.getInstance.m_bFinaleStage)  // GAME CLEAR
			StartCoroutine (Credit ());
		else
			StartCoroutine (LoadMain (LoadSceneName));
	}

	IEnumerator LoadMain(string LoadSceneName)
	{
		StartCoroutine(AudioMgr.getInstance.VolumeChg (false));
		yield return StartCoroutine(WaitForRealSeconds (m_UIFadeAlpha.duration + 0.5f));

		TimeMgr.Play ();
		Application.LoadLevel (LoadSceneName);
	}

	IEnumerator Credit()
	{
		yield return new WaitForSeconds (m_UIFadeAlpha.duration + 0.5f);

		StartCoroutine(GameObject.Find ("CreditPanel(Clone)").GetComponent<CreditPanel> ().CreditUpward ());
		GameObject.Find ("CreditExit(Clone)").SendMessage ("TweenActivate", true);
	}

	public void Pause(bool bPause)
	{

		m_UIFadeAlpha.duration = 0.25f;
		m_PauseAlpha.duration = 0.25f;

		if (bPause) {
			m_UIFadeAlpha.from = 0f;
			m_UIFadeAlpha.to = 0.6f;

			m_PauseAlpha.from = 0f;
			m_PauseAlpha.to = 0.8f;
		} else {
			m_UIFadeAlpha.from = 0.6f;
			m_UIFadeAlpha.to = 0f;

			m_PauseAlpha.from = 0.8f;
			m_PauseAlpha.to = 0f;
		}

		m_UIFadeAlpha.ResetToBeginning ();
		m_PauseAlpha.ResetToBeginning ();

		m_UIFadeAlpha.Play (true);
		m_PauseAlpha.Play (true);
	}

	public void TryFatherBtn(bool bOn)
	{
		if (!StageLoader.getInstance.m_bMaptool) {
			if (bOn) {
				m_PausePanel.transform.Find ("TryFather").GetComponent<UISprite> ().color = new Color (1, 1, 1, 1);
			} else {
				m_PausePanel.transform.Find ("TryFather").GetComponent<UISprite> ().color = new Color (1, 1, 1, 0.2f);
			}
		}
	}

	public IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}
}
