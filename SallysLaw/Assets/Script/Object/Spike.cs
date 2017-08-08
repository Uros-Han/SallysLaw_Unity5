using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : MonoBehaviour {

	public bool m_bPressed;
	public bool m_bRevival; //재생 스파이크
	IEnumerator SpikeRoutine;

	float fRevivalTime = 2f;
	public float fCurRevivalTime;

	public List<int> m_list_iRevivalIdx; //재생 스파이크가 다시 튀어나오는 샐리의 프레임

	public int m_iSpriteIdx; //sprite Idx;

	void Start()
	{
		m_iSpriteIdx = Random.Range (0, 3);

		transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = SceneObjectPool.getInstance.m_sally_listSpike[m_iSpriteIdx];

		m_list_iRevivalIdx = new List<int> ();
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	void OnDisable()
	{
		SpikeToOriginPos ();
	}

	void OnTriggerEnter2D(Collider2D coll)
	{

		if(GameObject.Find("MapToolMgr") == null || (GameObject.Find("MapToolMgr") != null && GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bNowPlaying))
		{
//				coll.GetComponent<AudioSource>().PlayOneShot(ObjectPool.getInstance.m_sound_bundle[(int)SOUND_LIST.SALLY_DIE]);
//				GameMgr.getInstance.GameOver();



			if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER) && !m_bPressed)
			{
				if(coll.transform.name.Equals("Runner(Clone)"))
				{
					if(GameMgr.getInstance.m_iCurChpt.Equals(2) && !SceneStatus.getInstance.m_bMemoryStage)
						AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "spike_metal");
					else
						AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "spike_wood");

					if (SpikeRoutine != null)
						StopCoroutine (SpikeRoutine);

					SpikeRoutine = SpikeMove(true);
					StartCoroutine(SpikeRoutine);
				}

				if(SceneStatus.getInstance.m_bFinaleStage)
				{
					if(coll.transform.name.Equals("Guardian(Clone)"))
					{
						if(GameMgr.getInstance.m_iCurChpt.Equals(2) && !SceneStatus.getInstance.m_bMemoryStage)
							AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "spike_metal");
						else
							AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "spike_wood");
						
						if (SpikeRoutine != null)
							StopCoroutine (SpikeRoutine);
						
						SpikeRoutine = SpikeMove(true);
						StartCoroutine(SpikeRoutine);
					}
				}
			}
			else if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.FLASHBACK) && m_bPressed){
				if(coll.transform.name.Equals("Runner(Clone)"))
				{
					if (SpikeRoutine != null)
						StopCoroutine (SpikeRoutine);

					SpikeRoutine = SpikeMove(false);
					StartCoroutine(SpikeRoutine);
				}
			}
			else if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
			{
				if(!m_bPressed)
				{
					if(coll.transform.name.Equals("Runner(Clone)"))
					{
						if(GameMgr.getInstance.m_bDevMode_SpeedHack)
							return;

						AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.SALLY_DIE);
						coll.gameObject.GetComponent<SkeletonAnimation>().state.SetAnimation(0,"danger", false);
						
						GameObject.Find("Guardian(Clone)").GetComponent<SkeletonAnimation>().state.SetAnimation(0,"danger",false);
						SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.SALLY_CRASH;
						coll.gameObject.GetComponent<Runner>().bExitRewinder = true;

						#if !UNITY_STANDALONE
						GameObject.Find("SidePanel").GetComponent<UIPanel>().alpha = 0f;
						#endif
						GameObject.Find ("FastForward").transform.GetChild (0).GetComponent<FastForwardBtn>().OnDisable();
						UIManager.getInstance.m_FastForwardPanel.GetComponent<UIPanel>().alpha = 0f;

						TimeMgr.SlowMotion();
					}
					else if(coll.transform.name.Equals("Guardian(Clone)"))
					{
						if(GameMgr.getInstance.m_iCurChpt.Equals(2) && !SceneStatus.getInstance.m_bMemoryStage)
							AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "spike_metal");
						else
							AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "spike_wood");

						if (SpikeRoutine != null)
							StopCoroutine (SpikeRoutine);

						SceneStatus sceneStatus = SceneStatus.getInstance;
						sceneStatus.m_iRemoveSpikeCount += 1;

						if(!sceneStatus.m_bSpikeAchievementGet && sceneStatus.m_iRemoveSpikeCount > 30)
						{
							//SpikeRemove
							/////Archive_17
//							#if UNITY_ANDROID
//							GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQEQ", 0);
//							#elif UNITY_IOS
//							GameCenterManager.UpdateAchievement ("sally_achiv17", 100);
//							#elif UNITY_STANDALONE
//							SteamAchieveMgr.SetAchieve("sally_achiv17");
//							#endif

							sceneStatus.m_bSpikeAchievementGet = true;
						}

						PlayerPrefs.SetInt("SpikeRemoveCount", PlayerPrefs.GetInt("SpikeRemoveCount") + 1);

						SpikeRoutine = SpikeMove(true);
						StartCoroutine(SpikeRoutine);
					}
				}
			}

		}
	}

//	void OnTriggerStay2D(Collider2D coll)
//	{
//		if (GameObject.Find ("MapToolMgr") == null || (GameObject.Find ("MapToolMgr") != null && GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bNowPlaying)) 
//		{
//			if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
//			{
//				if(coll.transform.name.Equals("Guardian(Clone)"))
//				{
//					fCurRevivalTime = fRevivalTime;
//				}
//			}
//		}
//	}

	IEnumerator SpikeMove(bool bDown)
	{
		bool bExit = false;

		float m_fSpeed = 2f;

		if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.FLASHBACK))
			m_fSpeed = 10f;

		fCurRevivalTime = fRevivalTime;

		Vector3 PushDir = Vector3.zero;
		Transform spriteTransform = transform.GetChild (1).transform;

		if (transform.localRotation.z > -1f && transform.localRotation.z < 1f) {
			PushDir = new Vector2 (0, -1f);
		} else if (transform.localRotation.z > 89f && transform.localRotation.z < 91f) {
			PushDir = new Vector2 (1f, 0);
		} else if (transform.localRotation.z > -91f && transform.localRotation.z < -89f) {
			PushDir = new Vector2 (-1f, 0);
		} else {
			PushDir = new Vector2 (0, -1f);
		}

		if (!bDown) 
			PushDir *= -1f;



		do{
			if(bDown)
			{
				if(Vector3.Distance(spriteTransform.localPosition , PushDir * 0.5f) > 0.05f * m_fSpeed)
					spriteTransform.localPosition = spriteTransform.localPosition + (PushDir * Time.deltaTime * m_fSpeed);
				else
				{
					spriteTransform.localPosition = PushDir * 0.5f;
					bExit = true;
				}

				m_bPressed = true;

			}else{
				if(Vector3.Distance(spriteTransform.localPosition, Vector3.zero) >  0.05f * m_fSpeed)
					spriteTransform.localPosition = spriteTransform.localPosition + (PushDir * Time.deltaTime * m_fSpeed);
				else
				{
					spriteTransform.localPosition = Vector3.zero;
					bExit = true;
				}

				m_bPressed = false;
			}

			yield return null;

		}while(!bExit);

		if (m_bRevival) {

			if(bDown)
			{

				do{

					fCurRevivalTime -= Time.deltaTime;

					if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN)
						fCurRevivalTime += (Time.deltaTime / 2f);

					yield return null;
				}while(fCurRevivalTime > 0f);

//				if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN)
//					yield return new WaitForSeconds (fRevivalTime);
//				yield return new WaitForSeconds (fRevivalTime);

				StartCoroutine(SpikeMove(false));
			}else
			{
				if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.RUNNER)
					m_list_iRevivalIdx.Add(GameObject.Find("Runner(Clone)").GetComponent<Runner>().m_ListSavePos.Count);
			}
		}
	}

	public IEnumerator FlashBackSpikeDown(){

		Runner runner = GameObject.Find ("Runner(Clone)").GetComponent<Runner> ();
		Transform runnerTransform = GameObject.Find ("Runner(Clone)").transform;

		int iIdxCount = runner.m_ListSavePos.Count;

		SceneStatus sceneStatus = SceneStatus.getInstance;

		int iBoundary = 100;

		do {
			for (int i = 0; i < m_list_iRevivalIdx.Count; ++i) {
				if ((iIdxCount - sceneStatus.m_iFlashBackIdx) - m_list_iRevivalIdx [i] < iBoundary && (iIdxCount - sceneStatus.m_iFlashBackIdx) - m_list_iRevivalIdx [i] > -iBoundary) {
					StartCoroutine(SpikeMove (true));
				}
			}

			if(m_bPressed && Vector3.Distance(runnerTransform.position, transform.position) < 1f)
			{
				if (SpikeRoutine != null)
					StopCoroutine (SpikeRoutine);
				
				SpikeRoutine = SpikeMove(false);
				StartCoroutine(SpikeRoutine);
			}

			yield return null;
		} while(sceneStatus.m_enPlayerStatus == PLAYER_STATUS.FLASHBACK);
	}


	public void SpikeToOriginPos()
	{
		transform.GetChild(1).localPosition = Vector3.zero;
		m_bPressed = false;
	}

}
