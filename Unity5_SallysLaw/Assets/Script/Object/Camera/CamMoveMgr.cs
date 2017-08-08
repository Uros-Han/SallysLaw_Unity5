using UnityEngine;
using System.Collections;

public class CamMoveMgr : MonoBehaviour
{
	public float fCamSpeed;
	float m_fCamEatSpeed; //샐리 멈췄을때 천천히 카메라가 잡아먹는 스피드

	public Vector3 m_vecFirstCamPos;
	public bool m_bRunnerInScreen; // Tracker 
	bool m_bRunnerInCenter;
	public bool m_bGuardianInCenter;
	float m_fRunnerBeforeYPos;
	public float m_fRunnerMaintainYPos;
	float m_fMainTainTimer;
	bool m_bMaintainPosInCenter;
	public Transform m_RunnerTransform;
	public Transform m_GuardianTransform;
	Transform m_GoalTransform;

	public bool m_bFromPortal;

	//Timeball vals
//	bool m_bAteTimeBall;
//	Vector3 m_vecExpandedCamPos;
//	public float m_fTimeBallExpandLength;

	public float m_fRunnerYPosFixer; //YPosition Plus  //샐리위치 y축 카메라 어느정도 위치일지 //높을수록 샐리가 카메라 하단에 붙어서보임
	public float m_fGuardianYPosFixer;

	bool m_bDontMoving;
	float m_fBeforeRunnerXPos;

	float fWidth_Half;
	float m_fBeforeHalf;

	enum ZoomStatus { IN, OUT, DEFAULT, END};
	ZoomStatus m_zoomStatus;

	IEnumerator m_runnerInScreen;
	bool m_bShaking = false;

	bool bGuradianCamTrigger;
	bool bCamGoingDown;

	public bool m_bFirstTuto;

	public CAM_TUTORIAL m_camTutorial = CAM_TUTORIAL.ZOOM_OUT;

	void Awake ()
	{
		m_fRunnerYPosFixer = 1.0f;
		m_fGuardianYPosFixer = -1f;

		//m_fTimeBallExpandLength = 2.0f;

		if (fCamSpeed.Equals(0.0f))
			fCamSpeed = 125; //125 dont change!!!!! 이 속도는 샐리 러닝할때 카메라속도가 아님! ( 사이사이 카메라 대상바뀌거나 이동할때 속도 )

		m_fCamEatSpeed = 110f;

		m_vecFirstCamPos = transform.localPosition;

		GetComponent<BoxCollider2D> ().size = new Vector2 (2 * Camera.main.orthographicSize * Camera.main.aspect, 2 * Camera.main.orthographicSize);

		//m_fRunnerMaintainYPos = m_RunnerTransform.position.y;

		fWidth_Half = 2.5f * Camera.main.aspect;

		m_camTutorial = CAM_TUTORIAL.NOT_TUTORIAL;

	}

	void Start()
	{
		if (GameMgr.getInstance.m_iCurChpt.Equals (1) && GameMgr.getInstance.m_iCurStage.Equals (1))
			m_bFirstTuto = true;
	}

	void OnDisable ()
	{
		StopAllCoroutines ();
	}

	void OnEnable ()
	{
		//m_bAteTimeBall = false;
		Init ();
	}

	public void Init(bool bActClear = false)
	{
		m_bRunnerInCenter = false;
		m_bGuardianInCenter = false;
		GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		StopAllCoroutines ();

		StartCoroutine (FindTransforms (bActClear));
		
//		if(GameObject.Find ("Runner(Clone)") != null)
//			m_fRunnerMaintainYPos = GameObject.Find ("Runner(Clone)").transform.position.y + m_fRunnerYPosFixer;

		transform.position = CamRestriction(transform.position, 0, true);
	}

	IEnumerator FindTransforms(bool bActClear = false)
	{
		SceneStatus sceneStatus = SceneStatus.getInstance;
		do{
			yield return null;
			
			if ( GameObject.Find ("Objects").transform.GetChild (GameMgr.getInstance.m_iCurAct - 1).Find("Players").Find ("Runner(Clone)") != null) {
				m_RunnerTransform = GameObject.Find ("Objects").transform.GetChild (GameMgr.getInstance.m_iCurAct - 1).Find("Players").Find ("Runner(Clone)").transform;

				if(!bActClear && (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER) || sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_WAIT))  )
				{
					RunnerMaintainYPos ();

					transform.position = new Vector3(m_RunnerTransform.position.x, m_fRunnerMaintainYPos, -10f);
					transform.position = CamRestriction(transform.position, 0, true);
				}
			}

			
		}while(m_RunnerTransform == null);

		do{
			yield return null;
			
			if (GameObject.Find ("Objects").transform.GetChild (GameMgr.getInstance.m_iCurAct - 1).Find("Players").Find("Guardian(Clone)") != null) {
				m_GuardianTransform = GameObject.Find ("Objects").transform.GetChild (GameMgr.getInstance.m_iCurAct - 1).Find("Players").Find ("Guardian(Clone)").transform;

				if(!bActClear && sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_EXIT))
				{
					transform.position = new Vector3(m_GuardianTransform.position.x, m_GuardianTransform.position.y + m_fGuardianYPosFixer, -10f);
					ChgCamOrtho(3.5f);
					transform.position = CamRestriction(transform.position, 0, true);
				}

			}

//			transform.position = CamRestriction(transform.position, 0, true);

		}while(m_GuardianTransform == null);

		do{
			yield return null;

			if (GameObject.Find ("R_Goal(Clone)") != null) {
				m_GoalTransform = GameObject.Find ("R_Goal(Clone)").transform;

//				//if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.LOOK_AROUND)
//					transform.position = m_GoalTransform.position;
			}



		}while(m_GoalTransform == null);



		StartCoroutine (MyRendering ());

		if (m_runnerInScreen != null) {
			StopCoroutine (m_runnerInScreen);
			GameObject.Find ("SallyTracker").transform.GetChild(0).gameObject.SetActive(false);
		}

		m_runnerInScreen = RunnerInScreen ();
		StartCoroutine (m_runnerInScreen);
	}

	public void GuardianRestartCam()
	{
		Vector3 CenterOfRunnerAndGuardian = Vector3.Lerp(m_GuardianTransform.position, new Vector2(m_RunnerTransform.position.x, CamYPosMove(true)), 0.5f);
		transform.position = new Vector3(CenterOfRunnerAndGuardian.x, CenterOfRunnerAndGuardian.y, -10f);
		transform.position = CamRestriction ();
	}

	public void GuardianCamInit()
	{
		transform.position = GameObject.Find ("Objects").transform.GetChild (0).transform.Find ("Players").GetChild (1).transform.position + new Vector3 (0, 0, -10);
		transform.position = CamRestriction ();
	}

	public void RunnerMaintainYPos()
	{
		m_fRunnerMaintainYPos = m_RunnerTransform.transform.position.y + m_fRunnerYPosFixer;
	}

	IEnumerator Waiting()
	{
		yield return new WaitForSeconds (0.2f);

		GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_enPlayerStatus = PLAYER_STATUS.RUNNER; // to runner
		GameObject.Find("StageNameContainer").transform.GetChild(0).gameObject.SetActive(true); // 스테이지 이름출력하기
	}

	// Update is called once per frame
	IEnumerator MyRendering ()
	{
		SceneStatus sceneStatus = SceneStatus.getInstance;
		yield return StartCoroutine (FindRunner ());

		if (sceneStatus.m_bFinaleStage)
			ChgCamOrtho (3.5f);

		do {
			if(m_bShaking)
			{
				do{
					yield return null;
				}while(m_bShaking);
			}

			if (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.STAGE_CHG_SALLY)) {

				yield return new WaitForEndOfFrame ();

//				CamYPosMove ();

				if (!m_bRunnerInCenter) {
					if(m_RunnerTransform.position.x < GetXPosofThisMap(true))
					{
					
						m_bRunnerInCenter = true;

						yield return StartCoroutine(CamToTarget (new Vector3(GetXPosofThisMap(true), transform.position.y, -10), 1.0f, PLAYER_STATUS.STAGE_CHG_SALLY, false));
					}else{

						m_bRunnerInCenter = true;
						yield return StartCoroutine(CamToTarget (new Vector3(m_RunnerTransform.position.x, transform.position.y, -10), 1.0f, PLAYER_STATUS.STAGE_CHG_SALLY, false));
					}

				} 
				//MainTainCamMove ();
				if (m_bRunnerInCenter) {
					transform.position = new Vector3 (m_RunnerTransform.transform.position.x, transform.position.y, -10);
					m_bRunnerInCenter = false; // Initialize to use flashback Status.
					//StartCoroutine(Waiting());
					sceneStatus.m_enPlayerStatus = PLAYER_STATUS.RUNNER; // to runner

					GameMgr.getInstance.DisableBeforeAct();
					GameObject.Find("SidePanel").GetComponent<UIPanel>().alpha = 1f;
					m_RunnerTransform.GetComponent<AudioSource> ().volume = 1f;
				}
			}else if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.STAGE_CHG_FATHER)){

				yield return new WaitForEndOfFrame ();


				yield return StartCoroutine(CamToTarget (new Vector3(GetXPosofThisMap(true), transform.position.y, -10), 1.0f, PLAYER_STATUS.STAGE_CHG_FATHER, false));
					
				sceneStatus.m_enPlayerStatus = PLAYER_STATUS.FATHER_ENTER;

				GameMgr gMgr = GameMgr.getInstance;
				gMgr.DisableBeforeAct ();
				gMgr.FatherEnter();
//				gMgr.ChangeBackgrounds (true);

			}else if (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_WAIT)) { 

				if(sceneStatus.m_bFinaleStage)
				{
					Vector3 CenterOfRunnerAndGuardian = Vector3.Lerp(m_GuardianTransform.position, new Vector2(m_RunnerTransform.position.x, CamYPosMove(true)), 0.5f);
					CenterOfRunnerAndGuardian = new Vector3(CenterOfRunnerAndGuardian.x, CenterOfRunnerAndGuardian.y, -10f);

					transform.position = CenterOfRunnerAndGuardian;
				}else
					transform.position = new Vector3 (m_RunnerTransform.transform.position.x, m_fRunnerMaintainYPos, m_RunnerTransform.transform.position.y + m_fRunnerYPosFixer);

				transform.position = CamRestriction();
				yield return StartCoroutine(WaitUnitlCountdown());

				transform.position = CamRestriction();

				m_bRunnerInCenter = true;

			}else if (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER)) {

				yield return null;

				if (m_RunnerTransform == null)
					yield return StartCoroutine (FindRunner ());

				if(sceneStatus.m_bFinaleStage)
				{
					Vector3 CenterOfRunnerAndGuardian = Vector3.Lerp(m_GuardianTransform.position, new Vector2(m_RunnerTransform.position.x, CamYPosMove(true)), 0.5f);
					if(-1.2925f - 0.5f < CenterOfRunnerAndGuardian.y)
						CenterOfRunnerAndGuardian = new Vector3(CenterOfRunnerAndGuardian.x, CenterOfRunnerAndGuardian.y, -10f);
					else
						CenterOfRunnerAndGuardian = new Vector3(CenterOfRunnerAndGuardian.x, -1.2925f - 0.5f, -10f);

					if(m_bRunnerInCenter)
						yield return StartCoroutine(CamToCenterOfRunnerAndGuardian(1f, PLAYER_STATUS.RUNNER));
					
					m_bRunnerInCenter = false;

					transform.position = CenterOfRunnerAndGuardian;
				}else{

					transform.position = new Vector3(m_RunnerTransform.position.x, transform.position.y, -10f);

					CamYPosMove ();

					m_fBeforeRunnerXPos = m_RunnerTransform.position.x;
				}

				//yield return new WaitForEndOfFrame();
				transform.position = CamRestriction();

			} else if (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.FLASHBACK)) {

				yield return new WaitForEndOfFrame ();

				CamYPosMove ();

				transform.position = new Vector3 (m_RunnerTransform.transform.position.x, transform.position.y, -10);

				transform.position = CamRestriction();

			}else if (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.FATHER_ENTER)){


				yield return new WaitForEndOfFrame ();

				if(!m_bGuardianInCenter)
				{
					m_bGuardianInCenter = true;
					StartCoroutine(CamToTarget (new Vector3(m_GuardianTransform.position.x, m_GuardianTransform.position.y, -10), 1f, PLAYER_STATUS.FATHER_ENTER));
				}


				yield return StartCoroutine(CamZoomTarget(1.5f, 1f, PLAYER_STATUS.FATHER_ENTER));


				m_GuardianTransform.GetComponent<Guardian>().GuardianEnter();

				m_RunnerTransform.GetComponent<SkeletonAnimation>().enabled = true;
				m_RunnerTransform.GetComponent<Runner>().SkinSetting();

				yield return new WaitForSeconds(0.75f);

				m_bGuardianInCenter = false;
				sceneStatus.m_enPlayerStatus = PLAYER_STATUS.FATHER_WAIT;
				UIManager.getInstance.FatherWait ();
//				StartCoroutine (SwipeMove ());
			}else if (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.FATHER_WAIT)) {


				if(m_bFirstTuto)
				{
					m_bGuardianInCenter = true;
					m_camTutorial = CAM_TUTORIAL.ZOOMED;
				}else{
					Vector3 CenterOfRunnerAndGuardian = Vector3.Lerp(m_GuardianTransform.position, new Vector2(m_RunnerTransform.position.x, CamYPosMove(true)), 0.5f);
					CenterOfRunnerAndGuardian = new Vector3(CenterOfRunnerAndGuardian.x, CenterOfRunnerAndGuardian.y, -10f);
					m_bFromPortal = false;

					if(Camera.allCameras [0].orthographicSize != 3.5f)
					{
						StartCoroutine(CamToCenterOfRunnerAndGuardian(1f, PLAYER_STATUS.FATHER_WAIT, true, 3.5f));

						yield return StartCoroutine(CamZoomTarget(3.5f, 1f));
					}else
						transform.position = CenterOfRunnerAndGuardian;
				}
			

//				if(Input.GetKey(KeyCode.A))
//					transform.position = new Vector3(transform.position.x - (4f * Time.deltaTime), transform.position.y, -10f);
//				else if(Input.GetKey(KeyCode.D))
//					transform.position = new Vector3(transform.position.x + (4f * Time.deltaTime), transform.position.y, -10f);

				transform.position = CamRestriction();
				yield return new WaitForEndOfFrame ();

			} else if (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.CAM_TO_FATHER)) {
				
				yield return new WaitForEndOfFrame ();

				yield return CamToTarget (new Vector3(m_GuardianTransform.position.x, m_GuardianTransform.transform.position.y + m_fGuardianYPosFixer, -10), 1f, PLAYER_STATUS.CAM_TO_FATHER);

//				if(CamToTarget (new Vector3(m_GuardianTransform.position.x, m_GuardianTransform.transform.position.y + m_fGuardianYPosFixer, -10), 0.04f, 0.01f))
//					GameMgr.getInstance.GuardianStart();

				GameMgr.getInstance.GuardianStart();
				m_bGuardianInCenter = false;
				bGuradianCamTrigger = false;
				transform.position = CamRestriction();
				
			} else if (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN)) { // guardian

				float fDistance = Vector3.Distance(m_GuardianTransform.position, m_RunnerTransform.position);

				if(m_camTutorial.Equals(CAM_TUTORIAL.ZOOMED))
				{
					transform.position = m_GuardianTransform.position + new Vector3(0,0,-10);
				}else if(m_camTutorial.Equals(CAM_TUTORIAL.ZOOMING)){

					Vector3 CenterOfRunnerAndGuardian = Vector3.Lerp(m_GuardianTransform.position, new Vector2(m_RunnerTransform.position.x, CamYPosMove(true)), 0.5f);
					CenterOfRunnerAndGuardian = new Vector3(CenterOfRunnerAndGuardian.x, CenterOfRunnerAndGuardian.y, -10f);
					m_bFromPortal = false;
					
					if(Camera.allCameras [0].orthographicSize != 3.5f)
					{
						StartCoroutine(CamToCenterOfRunnerAndGuardian(3f, PLAYER_STATUS.GUARDIAN));
						
						yield return StartCoroutine(CamZoomTarget(3.5f, 3f));
					}else
						m_camTutorial = CAM_TUTORIAL.NOT_TUTORIAL;

				}else{

					if(fDistance < 10 && !bGuradianCamTrigger)
					{
						bGuradianCamTrigger = false;

						Vector3 CenterOfRunnerAndGuardian = Vector3.Lerp(m_GuardianTransform.position, new Vector2(m_RunnerTransform.position.x, CamYPosMove(true)), 0.5f);
						CenterOfRunnerAndGuardian = new Vector3(CenterOfRunnerAndGuardian.x, CenterOfRunnerAndGuardian.y, -10f);

						if(m_bGuardianInCenter)
						{
							if(m_bFromPortal)
							{
								yield return StartCoroutine(CamToCenterOfRunnerAndGuardian(0.2f, PLAYER_STATUS.GUARDIAN));
								m_bFromPortal = false;
							}
							else
								yield return StartCoroutine(CamToCenterOfRunnerAndGuardian(1f, PLAYER_STATUS.GUARDIAN));
						}

						m_bGuardianInCenter = false;
						transform.position = CenterOfRunnerAndGuardian;
					}else if(fDistance > 7.5f && bGuradianCamTrigger)
					{
						bGuradianCamTrigger = true;

						if(m_bFromPortal)
						{
							yield return StartCoroutine(CamToGuardian(0.2f, PLAYER_STATUS.GUARDIAN));
							m_bFromPortal = false;
						}

						if(!m_bGuardianInCenter)
							yield return StartCoroutine(CamToGuardian(1.5f, PLAYER_STATUS.GUARDIAN));

	                    m_bGuardianInCenter = true;
						transform.position = new Vector3 (m_GuardianTransform.position.x, m_GuardianTransform.transform.position.y, -10);
					}else{
						bGuradianCamTrigger = bGuradianCamTrigger? false : true;
					}
				}

				transform.position = CamRestriction();

				yield return new WaitForFixedUpdate ();
			}else if(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_CRASH)){

				StartCoroutine(CamZoomTarget(1f, 1f, PLAYER_STATUS.SALLY_CRASH));
				yield return StartCoroutine(CamToRunner(1f, PLAYER_STATUS.SALLY_CRASH));

				GameMgr.getInstance.GameOver();
				sceneStatus.m_enPlayerStatus = PLAYER_STATUS.FATHER_WAIT;

			}else{

				yield return new WaitForEndOfFrame ();

//				transform.position = CamRestriction();
			}





		} while(true);
	}

//	public IEnumerator Shake(float fXPow,float fYPow,float fTime)
//	{
//		m_bShaking = false;
//
//		Hashtable ht = new Hashtable();
//		ht.Add("x", fXPow);
//		ht.Add("y", fYPow);
//		ht.Add("time", fTime);
//
//		iTween.ShakeRotation (gameObject, ht);
//
//		yield return new WaitForSeconds (fTime);
//		m_bShaking = false;
//	}


	public IEnumerator SwipeMove()
	{
		Vector2 ClickMousePos = Vector2.zero;
		Vector2 CurMousePos = Vector2.zero;

		float fPressTime = 0f;
		float fSpeed = 0f;
		bool bSwipeStart = false;

		SceneStatus sceneStatus = SceneStatus.getInstance;

		do{

			if(Input.GetMouseButtonDown(0))
			{
				ClickMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				fPressTime = 0f;
				bSwipeStart = true;
			}else if(bSwipeStart && Input.GetMouseButton(0))
			{
				CurMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				fPressTime += Time.deltaTime;

				fSpeed = Vector2.Distance(new Vector2(CurMousePos.x,0) , new Vector2(ClickMousePos.x,0)) * 4f * Time.deltaTime;
			}


			fSpeed -= Time.deltaTime * 0.5f;
			if(fSpeed < 0f)
				fSpeed = 0f;

			if(CurMousePos.x > ClickMousePos.x)
				transform.position = new Vector3(transform.position.x - fSpeed, transform.position.y, -10f);
			else if(CurMousePos.x < ClickMousePos.x)
				transform.position = new Vector3(transform.position.x + fSpeed, transform.position.y, -10f);


			transform.position = CamRestriction();

			yield return null;

			if(GameObject.Find("SceneStatus") == null)
				break;

		}while(sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.FATHER_WAIT));

	}

	public void ChgCamOrtho(float fSize)
	{
		for (int i = 0; i < Camera.allCamerasCount; ++i) {
			if (Camera.allCameras [i].gameObject.layer != 5) // not ui Camera
				Camera.allCameras [i].orthographicSize = fSize;
		}
	}

	/// <summary>
	/// 카메라 줌인/아웃 담당하는애
	/// </summary>
	void CameraZooming()
	{
		//float fWidth_Half = 2.5f * Camera.main.aspect;  // 2.5 is default orthographicSize

		fWidth_Half = Camera.main.orthographicSize * Camera.main.aspect;

		if (m_fBeforeHalf - fWidth_Half < 0.01f && m_fBeforeHalf - fWidth_Half > -0.01f)
			m_zoomStatus = ZoomStatus.DEFAULT;

		if (m_zoomStatus.Equals(ZoomStatus.IN)) {
			if(fWidth_Half < m_fBeforeHalf)
				fWidth_Half = m_fBeforeHalf;
		} else if(m_zoomStatus.Equals(ZoomStatus.OUT)){
			if(fWidth_Half > m_fBeforeHalf)
				fWidth_Half = m_fBeforeHalf;
		}



		if (m_zoomStatus.Equals(ZoomStatus.DEFAULT)) {
			if(m_fBeforeHalf < fWidth_Half)
				m_zoomStatus = ZoomStatus.OUT;
			else if(m_fBeforeHalf > fWidth_Half)
				m_zoomStatus = ZoomStatus.IN;
		}

		float fLeftestXPos = SceneStatus.getInstance.m_fStageXPosLeftest[GameMgr.getInstance.m_iCurAct-1] - 0.25f;
		float fRightestXPos = SceneStatus.getInstance.m_fStageXPos[GameMgr.getInstance.m_iCurAct-1] - 0.25f;

		float fMin_X_Distance = 4.2f; // 러너가 화면 밖으로 나가는(안보이게되는) 가로 거리
		float fMin_Y_Distance = 3f; // 러너가 화면 밖으로 나가는(안보이게되는) 세로 거리

		float fDistance = 0f;

		float fDefaultOrtho = 2.5f;
		float fMaxOrtho = 5.75f;

		bool bRestriction = false;

		//////////////////////////////////////////////항상 화면중앙에서 러너와의 거리를 계산하도록
		//좌측에 캠 레스틱션 걸렷을때
		if (transform.position.x < fLeftestXPos + fWidth_Half) {
			fWidth_Half = (fWidth_Half - m_fBeforeHalf) /2f + m_fBeforeHalf;

			fDistance = fLeftestXPos + fWidth_Half - m_RunnerTransform.position.x;
			bRestriction = true;
		}
		//우측에 캠 레스틱션 걸렷을때
		else if (transform.position.x > fRightestXPos - fWidth_Half) {
			fWidth_Half = (fWidth_Half - m_fBeforeHalf) /2f + m_fBeforeHalf;

			fDistance = fRightestXPos - fWidth_Half - m_RunnerTransform.position.x;
			bRestriction = true;
		}
		//레스틱션 안걸림
		else {
			fDistance = transform.position.x - m_RunnerTransform.position.x;
		}
		//////////////////////////////////////////////항상 화면중앙에서 러너와의 거리를 계산하도록

		if (fDistance < 0)
			fDistance *= -1f;
		//음수이면 양수로


		if (fDistance > fMin_X_Distance) {

			float fTmpOrtho = 0f;

			fTmpOrtho = (fDistance - fMin_X_Distance) / Camera.main.aspect;



			if(!bRestriction && fTmpOrtho < fMaxOrtho - fDefaultOrtho)
			{
				for (int i = 0; i < Camera.allCamerasCount; ++i) {
					if (Camera.allCameras [i].gameObject.layer != 5) // not ui Camera
						Camera.allCameras [i].orthographicSize = fDefaultOrtho + fTmpOrtho;
				}

			}else if(bRestriction && fTmpOrtho < fMaxOrtho - fDefaultOrtho){
				for (int i = 0; i < Camera.allCamerasCount; ++i) {
					if (Camera.allCameras [i].gameObject.layer != 5) // not ui Camera
						Camera.allCameras [i].orthographicSize = fDefaultOrtho + fTmpOrtho;
				}
			}else{//최대 줌아웃 이상으로 멀어짐
				for (int i = 0; i < Camera.allCamerasCount; ++i) {
					if (Camera.allCameras [i].gameObject.layer != 5) // not ui Camera
						Camera.allCameras [i].orthographicSize = fMaxOrtho;
				}
			}


		} else {//최소줌 안으로 들어옴
			for (int i = 0; i < Camera.allCamerasCount; ++i) {
				if (Camera.allCameras [i].gameObject.layer != 5) // not ui Camera
					Camera.allCameras [i].orthographicSize = fDefaultOrtho;
			}

//			fDistance = transform.position.y - m_RunnerTransform.position.y;
//			//가디언과 러너 세로 사이 거리구함
//			
//			if(fDistance < 0)
//				fDistance *= -1f;
//			//음수이면 양수로
//			
//			if (fDistance > fMin_Y_Distance) {
//				
//				float fTmpOrtho = fDistance - fMin_Y_Distance;
//				
//				for(int i = 0; i < Camera.allCamerasCount; ++i)
//				{
//					if(Camera.allCameras[i].gameObject.layer != 5) // not ui Camera
//						Camera.allCameras[i].orthographicSize = 2.5f + fTmpOrtho;
//				}
//			}
		}



		m_fBeforeHalf = fWidth_Half;



	}
	
	
	public Vector3 CamRestriction(Vector3 vecRestrictedPos = default(Vector3), float fTargetOrthoSize = 0f, bool bRestrictYOnly = false)
	{
		SceneStatus sceneStatus = SceneStatus.getInstance;

		if (vecRestrictedPos.Equals(default(Vector3)))
			vecRestrictedPos = transform.position;

		if (!bRestrictYOnly) {
			float fWidth_Half = Camera.main.orthographicSize * Camera.main.aspect;
			
			float fLeftestXPos = sceneStatus.m_fStageXPosLeftest[GameMgr.getInstance.m_iCurAct-1] - 0.25f;
			float fRightestXPos = sceneStatus.m_fStageXPos[GameMgr.getInstance.m_iCurAct-1] - 0.25f;

			if (vecRestrictedPos.x > fRightestXPos - fWidth_Half) {
				vecRestrictedPos = new Vector3 (fRightestXPos - fWidth_Half, vecRestrictedPos.y, -10);
				GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			}

			if (vecRestrictedPos.x < fLeftestXPos + fWidth_Half) {
				vecRestrictedPos = new Vector3 (fLeftestXPos + fWidth_Half, vecRestrictedPos.y, -10);
				GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			}
		}


		//가디언일때 젤 밑바닥밑으로 카메라안내려가게
		if (sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.FATHER_WAIT) || sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.STAGE_CHG_FATHER)
		    || sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.CAM_TO_FATHER) || sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN)) {

			if(fTargetOrthoSize == 0f)
			{
				if(vecRestrictedPos.y - Camera.main.orthographicSize < -4.75f)
					vecRestrictedPos = new Vector3(vecRestrictedPos.x, -4.75f + Camera.main.orthographicSize, -10f);
			}else{
				if(vecRestrictedPos.y < -1.25f)
					vecRestrictedPos = new Vector3(vecRestrictedPos.x, -1.25f, -10f);
			}

		}

		return vecRestrictedPos;
	}

	float GetXPosofThisMap(bool bLeft)
	{
		float fWidth_Half = Camera.main.orthographicSize * Camera.main.aspect;
		
		float fLeftestXPos =SceneStatus.getInstance.m_fStageXPosLeftest[GameMgr.getInstance.m_iCurAct-1] - 0.25f;
		float fRightestXPos = SceneStatus.getInstance.m_fStageXPos[GameMgr.getInstance.m_iCurAct-1] - 0.25f;

		if (bLeft) {
			return fLeftestXPos + fWidth_Half;
		}else 
			return fRightestXPos - fWidth_Half;

		Debug.LogError ("PosError");
		return 0f;
	}

	IEnumerator WaitUnitlCountdown()
	{
		//GameObject.Find ("CountDown").transform.GetChild (0).GetChild (2).GetComponent<UILabel> ().text = "3";

		SpriteRenderer fadeSprite = GameObject.Find ("FadeSprite").GetComponent<SpriteRenderer> ();
		UIPanel startPanel = UIManager.getInstance.m_PlayStartPanel.GetComponent<UIPanel> ();

		do{
			yield return null;
		}while(fadeSprite.color.a != 0);

		//GameObject.Find ("CountDown").transform.GetChild (0).gameObject.SetActive (true);

		do{
			yield return null;
			
		}while(startPanel.alpha < 0.001f);

		do{
			yield return null;

		}while(startPanel.alpha != 0f);

		GameMgr gMgr = GameMgr.getInstance;
		if (gMgr.m_iCurChpt.Equals (1) && gMgr.m_iCurStage.Equals (2) && gMgr.m_iCurAct.Equals (1)) {
			AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "trans", 0);
			GameObject.Find("StartVehicle(Clone)").transform.GetChild(1).GetComponent<SkeletonAnimation>().state.SetAnimation(0, "move", false);
		}

		m_RunnerTransform.GetComponent<AudioSource> ().volume = 1f;

		TimeMgr.Play();
	}

	IEnumerator FindRunner ()
	{
		SceneStatus sceneStatus = SceneStatus.getInstance;

		do {
			yield return null;

//			if(sceneStatus.m_enPlayerStatus != PLAYER_STATUS.STAGE_CHG_FATHER)
//				transform.position = new Vector3 (transform.position.x, m_fRunnerMaintainYPos, -10);
//			else
//				transform.position = CamRestriction(transform.position, 0, true);

		} while(GameObject.Find ("Runner(Clone)") == null);
	}
	
	IEnumerator RunnerInScreen()
	{
		do {
			yield return null;
		} while(m_RunnerTransform == null);


		SallyTracker sallyTracker = GameObject.Find ("SallyTracker").GetComponent<SallyTracker> ();
		m_bRunnerInScreen = true;

		do {
			float fWidthHalf = Camera.main.orthographicSize * Camera.main.aspect;
			float fHeightHalf = Camera.main.orthographicSize;

			if(!m_bRunnerInScreen)
			{
				if((Mathf.Abs(transform.position.x - m_RunnerTransform.position.x) < fWidthHalf + 0.25f) && (Mathf.Abs(transform.position.y - m_RunnerTransform.position.y) < fHeightHalf + 0.25f))
					m_bRunnerInScreen = true;
			}else{
				if((Mathf.Abs(transform.position.x - m_RunnerTransform.position.x) > fWidthHalf + 0.25f) || (Mathf.Abs(transform.position.y - m_RunnerTransform.position.y) > fHeightHalf + 0.25f))
				{
					m_bRunnerInScreen = false;

					if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.RUNNER)
						GameMgr.getInstance.GameOver();
					else if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN)
						StartCoroutine(sallyTracker.SallyTrackTracking());
				}
			}

			yield return new WaitForEndOfFrame();

		} while(true);
	}

	//해당 목표로 카메라 이동도와주는 함수

	Vector3 fBeforePos;

	IEnumerator CamZoomTarget (float fTarget, float fTime, PLAYER_STATUS playerStatus = PLAYER_STATUS.END)
	{

		float fStart = Camera.allCameras [0].orthographicSize;
		float fValue = 0f;
		SceneStatus sceneStatus = SceneStatus.getInstance;

		do{
			yield return null;
			fValue += (Time.unscaledDeltaTime / fTime);

			if(fValue > 1f)
				fValue = 1f;

			ChgCamOrtho(Mathf.Lerp(fStart, fTarget, Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, fValue))));

			transform.position = CamRestriction(transform.position);



		}while(fValue != 1 && (playerStatus.Equals(PLAYER_STATUS.END) || sceneStatus.m_enPlayerStatus.Equals(playerStatus)));

	}


	IEnumerator CamToTarget (Vector3 vecTarget, float fTime, PLAYER_STATUS playerStatus , bool bCamrRestriction = true)
	{

		vecTarget = new Vector3 (vecTarget.x, vecTarget.y, -10f);
		Vector3 vecStart = transform.position;

		float fValue = 0f;
		SceneStatus sceneStatus = SceneStatus.getInstance;

		do {
			yield return new WaitForEndOfFrame ();
			fValue += (Time.unscaledDeltaTime / fTime);

			if(fValue > 1f)
				fValue = 1f;

			transform.position = Vector3.Lerp (vecStart, vecTarget, Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, fValue)));

			if(bCamrRestriction)
				transform.position = CamRestriction(transform.position);
			else
				transform.position = CamRestriction(transform.position, 0, true);


		} while(fValue != 1 && sceneStatus.m_enPlayerStatus.Equals(playerStatus));

	}

	IEnumerator CamToCenterOfRunnerAndGuardian (float fTime, PLAYER_STATUS playerStatus , bool bCamrRestriction = true, float fTargetOrtho = 0f)
	{
		
		Vector3 vecStart = transform.position;
		
		float fValue = 0f;
		SceneStatus sceneStatus = SceneStatus.getInstance;
		
		do {
			yield return new WaitForEndOfFrame();

			Vector3 CenterOfRunnerAndGuardian = Vector3.Lerp(m_GuardianTransform.position, new Vector2(m_RunnerTransform.position.x, CamYPosMove(true)), 0.5f);
			CenterOfRunnerAndGuardian = new Vector3(CenterOfRunnerAndGuardian.x, CenterOfRunnerAndGuardian.y, -10f);

			if(fTargetOrtho != 0f)
			{
				CenterOfRunnerAndGuardian = CamRestriction(CenterOfRunnerAndGuardian, fTargetOrtho);
			}

			fValue += (Time.unscaledDeltaTime / fTime);
			
			if(fValue > 1f)
				fValue = 1f;
			
			transform.position = Vector3.Lerp (vecStart, CenterOfRunnerAndGuardian, Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, fValue)));
			
			if(bCamrRestriction)
				transform.position = CamRestriction(transform.position);
			else
				transform.position = CamRestriction(transform.position, 0, true);

	
			
		} while(fValue != 1 && sceneStatus.m_enPlayerStatus.Equals(playerStatus));
		
	}

	IEnumerator CamToGuardian (float fTime, PLAYER_STATUS playerStatus , bool bCamrRestriction = true)
	{

		Vector3 vecStart = transform.position;
		
		float fValue = 0f;
		SceneStatus sceneStatus = SceneStatus.getInstance;
		
		do {
			yield return new WaitForEndOfFrame();
			fValue += (Time.unscaledDeltaTime / fTime);
			
			if(fValue > 1f)
				fValue = 1f;
			
			transform.position = Vector3.Lerp (vecStart, new Vector3(m_GuardianTransform.position.x, m_GuardianTransform.position.y, -10), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, fValue)));
			
			if(bCamrRestriction)
				transform.position = CamRestriction(transform.position);
			else
				transform.position = CamRestriction(transform.position, 0, true);
			
		} while(fValue != 1 && sceneStatus.m_enPlayerStatus.Equals(playerStatus));
		
	}

	IEnumerator CamToRunner (float fTime, PLAYER_STATUS playerStatus , bool bCamrRestriction = true)
	{
		
		Vector3 vecStart = transform.position;
		
		float fValue = 0f;
		SceneStatus sceneStatus = SceneStatus.getInstance;
		
		do {
			yield return new WaitForEndOfFrame();
			fValue += (Time.unscaledDeltaTime / fTime);
			
			if(fValue > 1f)
				fValue = 1f;
			
			transform.position = Vector3.Lerp (vecStart, new Vector3(m_RunnerTransform.position.x, m_RunnerTransform.position.y, -10), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, fValue)));
			
			if(bCamrRestriction)
				transform.position = CamRestriction(transform.position);
			else
				transform.position = CamRestriction(transform.position, 0, true);
			
		} while(fValue != 1 && sceneStatus.m_enPlayerStatus.Equals(playerStatus));
		
	}



	bool PlusTime (Vector3 vecTarget, float fSpeed, float fDistance)
	{
		vecTarget = new Vector3 (vecTarget.x, transform.position.y, -10f);

		Vector3 m_vecTmp = Vector3.Normalize (vecTarget - transform.position);

		
		if (Vector2.Distance (transform.position, vecTarget) < fDistance)
			return true;

		transform.position = transform.position + (m_vecTmp * fCamSpeed * Time.deltaTime * fSpeed);
		transform.position = new Vector3 (transform.position.x, transform.position.y, -10);
		
		return false;
	}

//	public void EatTimeBall(GameObject timeBallObj)
//	{
//		m_vecExpandedCamPos = transform.position + new Vector3(-m_fTimeBallExpandLength,0,0);
//		m_bAteTimeBall = true;
//
//		StartCoroutine (timeBallObj.GetComponent<Timeball>().Destroyer());
//	}


	float CamYPosMove (bool bReturn = false)
	{

		float fBottomLimit_Y = -2.56f ; // 샐리 바닥리미트 이 밑으론 카메라가 안쫒아감
		float fCeiling_Y = 0.5f; // 샐리 천장높이, 넘으면 카메라가 위로쫒아감

		if (m_fRunnerMaintainYPos < fBottomLimit_Y) { // if sally die in pit initalize
			m_fRunnerMaintainYPos = m_RunnerTransform.transform.position.y + m_fRunnerYPosFixer;
			return m_RunnerTransform.transform.position.y + m_fRunnerYPosFixer;
		}

		if (m_fRunnerMaintainYPos > fBottomLimit_Y && m_RunnerTransform.position.y + m_fRunnerYPosFixer < m_fRunnerMaintainYPos) { //camera go down
			m_fRunnerMaintainYPos = m_RunnerTransform.transform.position.y + m_fRunnerYPosFixer;
			bCamGoingDown = true;
		} else if (m_fRunnerMaintainYPos < fBottomLimit_Y) {
			m_fRunnerMaintainYPos = fBottomLimit_Y;
		}

		if (m_RunnerTransform.position.y  > m_fRunnerMaintainYPos + fCeiling_Y) {// camera go up
			m_fRunnerMaintainYPos = m_RunnerTransform.transform.position.y - fCeiling_Y ;
		}

		if (bReturn)
			return m_fRunnerMaintainYPos;

		transform.position = new Vector3 (transform.position.x, m_fRunnerMaintainYPos, -10f);

		return 0;

	}
//
//	public void ToGuardianCam()
//	{
//		transform.localPosition = m_vecFirstCamPos;
//	}
}
