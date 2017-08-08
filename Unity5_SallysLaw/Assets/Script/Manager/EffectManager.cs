using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour {

	private static EffectManager instance;
	
	public static EffectManager getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(EffectManager)) as EffectManager;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("EffectManager");
				instance = obj.AddComponent (typeof(EffectManager)) as EffectManager;
			}
			
			return instance;
		}
	}

	IEnumerator fadeEffect;

	void Awake(){
		if (instance == null)
			instance = this;
		
		else if (instance != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad (gameObject);
	}
	
	void OnDestroy()
	{
		
		instance = null;
		
	}

	public bool m_bBlurOn ;

	void Start(){
		m_bBlurOn = false;
	}


	/// <summary>
	/// 페이드 이펙트 실행용
	/// </summary>
	/// <param name="bOn">If set to <c>true</c> b on.</param>
	/// <param name="fFadeSpeed">F fade speed.</param>
	/// <param name="playerStatus">Player status.</param>
	public void RunFadeEffect(bool bOn, float fFadeSpeed = 2.5f, PLAYER_STATUS playerStatus = default(PLAYER_STATUS))
	{

		if (fadeEffect != null)
			StopCoroutine (fadeEffect);

		if(fFadeSpeed.Equals(2.5f) && playerStatus == default(PLAYER_STATUS))
			fadeEffect = FadeEffect (bOn);
		else if(fFadeSpeed != 2.5f && playerStatus == default(PLAYER_STATUS))
			fadeEffect = FadeEffect (bOn,fFadeSpeed);
		else
			fadeEffect = FadeEffect (bOn,fFadeSpeed,playerStatus);

		StartCoroutine (fadeEffect);
	}

	/// <summary>
	/// Fades the effect.
	/// </summary>
	/// <returns>The effect.</returns>
	/// <param name="bOn">If set to <c>true</c> b on.</param>
	IEnumerator FadeEffect(bool bOn, float fFadeSpeed = 2.5f, PLAYER_STATUS playerStatus = default(PLAYER_STATUS))
	{
		// how long want fade
		bool bExit = false;
		float fFadeMaintainTime = 0.2f;


		//to make TimeScale ignored TimeDelta
		float fStartTime = Time.realtimeSinceStartup; 
		float fTimeDelta = 0;

		SpriteRenderer fadeSprite = GameObject.Find ("FadeSprite").GetComponent<SpriteRenderer> ();

		SceneStatus sceneStatus = SceneStatus.getInstance;
//		if(bOn)
//			SwitchBlur(true);

		if(bOn)
			AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.FADE_IN);
		else
			AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.FADE_OUT);


		UIPanel UIFadePanel = UIManager.getInstance.m_UIFadePanel.GetComponent<UIPanel>();

		do{

			fTimeDelta = Time.realtimeSinceStartup - fStartTime; 
			fStartTime = Time.realtimeSinceStartup; 

			if (bOn) {

				if(!UIFadePanel.alpha.Equals(0))
				{
					yield return null;
					continue;
				}

				fadeSprite.color = new Color(0,0,0, fadeSprite.color.a + (fFadeSpeed * fTimeDelta));

				if(fadeSprite.color.a > 1.0f)
				{
					fadeSprite.color = new Color(0,0,0,1);
					bExit = true;

					// 게임매니저에게 리스타트하라구 알려줌
					GameMgr.getInstance.SendMessage("BlackOut", playerStatus);

					// fFadeMaintainTime초 후에 fade off 시작
					yield return StartCoroutine(WaitForRealSeconds(fFadeMaintainTime));
					StartCoroutine(FadeEffect(false));
				}

			} else {

				fadeSprite.color = new Color(0,0,0, fadeSprite.color.a - (fFadeSpeed * fTimeDelta));

				if(fadeSprite.color.a < 0.0f)
				{
					fadeSprite.color = new Color(0,0,0,0);
					bExit = true;

					GameMgr.getInstance.SendMessage("FadeEnd");
				}

			}

			yield return null;

		}while(!bExit);
	}

	/// <summary>
	/// Switchs the blur.
	/// </summary>
	public void SwitchBlur(bool bOn)
	{
		Camera m_BlurCam = GameObject.Find ("Blur Camera").GetComponent<Camera> ();
//		Blur m_blur = GameObject.Find ("Blur Camera").GetComponent<Blur> ();
		bool m_bGrayOn;
		
		if (GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_enPlayerStatus == PLAYER_STATUS.GUARDIAN)
			m_bGrayOn = true;
		else
			m_bGrayOn = false;

//		if (bOn)
//			StartCoroutine (BlurOn (m_BlurCam, m_blur));
//		else
//			StartCoroutine (BlurOff (m_BlurCam, m_blur));
	}

	/// <summary>
	/// Blurs the on.
	/// </summary>
	/// <returns>The on.</returns>
	/// <param name="m_BlurCam">M_ blur cam.</param>
	/// <param name="m_blur">M_blur.</param>
//	IEnumerator BlurOn(Camera m_BlurCam, Blur m_blur)
//	{
//		m_BlurCam.enabled = true;
//		GameObject.Find ("Main Camera").GetComponent<Camera> ().enabled = false;
//		
//		while(m_blur.blurIterations < 2f)
//		{
//			if(m_blur.downsample < 2)
//			{
//				m_blur.downsample += 1;
//			}
//			
//			m_blur.blurIterations += 1;
//			
//			yield return StartCoroutine(WaitForRealSeconds(0.045f));
//		}
//		
//		m_bBlurOn = true;
//	}

	/// <summary>
	/// Blurs the off.
	/// </summary>
	/// <returns>The off.</returns>
	/// <param name="m_BlurCam">M_ blur cam.</param>
	/// <param name="m_blur">M_blur.</param>
//	IEnumerator BlurOff(Camera m_BlurCam, Blur m_blur)
//	{
//		while(m_blur.blurIterations > 1)
//		{
//			
//			if(m_blur.downsample > 0)
//			{
//				m_blur.downsample -= 1;
//			}
//			
//			m_blur.blurIterations -= 1;
//			
//			if(m_blur.blurIterations > 1)
//				yield return StartCoroutine(WaitForRealSeconds(0.045f));
//		}
//		
//		m_bBlurOn = false;
//		
//		m_BlurCam.enabled = false;
//		GameObject.Find ("Main Camera").GetComponent<Camera> ().enabled = true;
//	}

	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}
}
