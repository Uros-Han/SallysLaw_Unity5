using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Runner : MonoBehaviour {

	float m_fAccel;
	float m_fMaxSpeed;

	public float m_fRewindSpeedAdjust; //코루틴 사이사이마다 속도 설정할때 오차조정해주는놈 안바꾸길 권장

	public bool m_bOnGround;
	public bool m_bLeftMove;
	bool m_bBeforeOnGround;
	public bool m_bJump;

	public float m_fJump;
	public float m_fJumpResist;

	public Vector2 m_vecStartPos;

	//public List<Vector3> m_ListSavePos;
	//public List<Quaternion> m_ListSaveRot;

	public Dictionary<int, Vector2> m_ListSavePos;
	public Dictionary<int, Quaternion> m_ListSaveRot;

	public int m_iIdx;
	public float m_fFlashBackAccel;
	public float m_fFlashBackSpeed;
	bool m_bFlashBackBreakOn;

	public bool m_bFatherTutorialOn;


	public float m_fWaitTime;//0.0165//0.017f; // wait Timeeeeeeeeeeeeeeeeee rewinder
	float m_fBeforeWaitTime;

	bool m_bRunnerDontMoving;
	public bool bExitRewinder = false;

	public IEnumerator SpringForce;

	// Use this for initialization

	Vector2 m_vPlayPos;
	Vector2 m_CurPlatformVelocity;

	Rigidbody2D m_RigidBody;

	float m_fBeforeXPos;

	SceneStatus m_SceneStatus;

	Coroutine m_rewinder;
	bool bStarted;

	float fCurFadeWaitTime;

	int iPathPadding;
	UIPanel FadePanel;

	void Start () {

		SkinSetting ();

		m_SceneStatus = SceneStatus.getInstance;
		m_RigidBody = GetComponent<Rigidbody2D> ();

//		m_ListSavePos = new List<Vector3> ();
//		m_ListSaveRot = new List<Quaternion> ();

		m_ListSavePos = new Dictionary<int, Vector2>();
		m_ListSaveRot = new Dictionary<int, Quaternion>();

		if(SceneStatus.getInstance.m_bFinaleStage)
			m_fMaxSpeed = 1.2f;
		else
			m_fMaxSpeed = 2.0f; ///3.25

		m_fAccel = 0.05f; //ori 0.07

		if (GameMgr.getInstance.m_bDevMode_SpeedHack) {
			m_fMaxSpeed = 10;
			m_fAccel = 1f;
		}

		m_fJump = 4500f; // 4.5 mass(1) 22.5 mass(5)

		m_iIdx = 0;

		m_fFlashBackAccel = 0.04f;
		m_fFlashBackSpeed = 0.0f;

		m_vecStartPos = transform.position;

		Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), GameObject.Find ("Guardian(Clone)").GetComponent<CircleCollider2D> ());

		GetComponent<TrailRenderer>().sortingLayerName = "Trail";
		GetComponent<MeshRenderer>().sortingLayerName = "Object";

		m_fWaitTime = 0.01f; //0.0165f; -> 원래 0.2.4까지의 속도. 빠르다는 피드백 받음 // 리와인드 속도조정하려면 이거 바꾸면됨 //0.02f
//		m_fRewindSpeedAdjust = 0.5f; //0.825

		iPathPadding = 10;

		m_fBeforeWaitTime = m_fWaitTime;

		m_bOnGround = true;
		bStarted = true;

		FadePanel = GameObject.Find ("UIFadePanel").GetComponent<UIPanel> ();

		StartCoroutine (GroundCheckStarter ());
		StartCoroutine (Play ());
	}

	public void SkinSetting()
	{
		GameMgr gMgr = GameMgr.getInstance;
		SceneStatus sceneStatus = SceneStatus.getInstance;

		if (sceneStatus.m_bMemoryStage) {

			switch (gMgr.m_iCurChpt) {
			case 1:
				GetComponent<SkeletonAnimation> ().skeleton.SetSkin ("5year");
				break;

			case 2:
				GetComponent<SkeletonAnimation> ().skeleton.SetSkin ("10year");
				break;

			case 3:
				GetComponent<SkeletonAnimation> ().skeleton.SetSkin ("15year");
				break;

			case 4:
				GetComponent<SkeletonAnimation> ().skeleton.SetSkin ("20year");
				break;

			default:
				Debug.Log ("Skin Setting Error");
				break;
			}
		} else {
			GetComponent<SkeletonAnimation> ().skeleton.SetSkin (gMgr.m_strCurSkin);
		}
	}

	void OnEnable()
	{
		if (bStarted) {
			Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), GameObject.Find ("Objects").transform.GetChild(GameMgr.getInstance.m_iCurAct - 1).Find("Players").Find("Guardian(Clone)").GetComponent<CircleCollider2D> ());

			StartCoroutine (GroundCheckStarter ());
			StartCoroutine (Play ());

			if(m_ListSavePos.Count != 0)
			{
				transform.position = m_ListSavePos[0] + (Vector2)transform.parent.position;
				transform.rotation = m_ListSaveRot[0];
			}
		}
	}

	public void Reset(){
		GetComponent<AudioSource> ().volume = 0f;
		m_RigidBody.velocity = Vector2.zero;
		m_RigidBody.angularVelocity = 0.0f;

		transform.position = m_vecStartPos;

		m_ListSavePos.Clear ();
		m_ListSaveRot.Clear ();

		m_bFlashBackBreakOn = false;
		m_fFlashBackSpeed = 0.0f;
		m_iIdx = 0;

		bExitRewinder = true;
		m_bLeftMove = false;
	}

	public void StartWithGuardian()
	{
		m_RigidBody.velocity = Vector2.zero;
		m_RigidBody.angularVelocity = 0.0f;
		
		transform.position = m_vecStartPos;
		
		m_bFlashBackBreakOn = false;
		m_fFlashBackSpeed = 0.0f;
		m_iIdx = 0;
		
		bExitRewinder = true;

		GetComponent<SkeletonAnimation>().state.SetAnimation(0, "idle", true);
	}

	void OnDisable()
	{
		StopAllCoroutines ();
//		Reset ();
	}

	public IEnumerator Spring(float fForce, SPRING_DIR dir)
	{
		yield return new WaitForEndOfFrame ();

//		m_RigidBody.angularDrag = 0.1f;



		GetComponent<Rigidbody2D> ().velocity = new Vector2(GetComponent<Rigidbody2D> ().velocity.x, 0);
		
		if (dir.Equals (SPRING_DIR.UP)) { // 하늘쳐다보는 스프링
			GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, fForce), ForceMode2D.Impulse);
			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource> (), "bundle", (int)SOUND_LIST.SPRING_UP);
		}
		else if (dir.Equals(SPRING_DIR.RIGHT)) { // 오른쪽 쳐다보는 스프링
			GetComponent<Rigidbody2D> ().AddForce (new Vector2 (fForce, 0), ForceMode2D.Impulse);
			m_RigidBody.angularVelocity = -840f;
			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource> (), "bundle", (int)SOUND_LIST.SPRING_SIDE);
		}
		else if (dir.Equals(SPRING_DIR.LEFT)) {//왼쪽 쳐다보는 스프링
			GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-fForce, 0), ForceMode2D.Impulse);
			m_RigidBody.angularVelocity = 840f;
			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource> (), "bundle", (int)SOUND_LIST.SPRING_SIDE);
		}
		else
			GetComponent<Rigidbody2D>().AddForce (new Vector2 (0, -fForce), ForceMode2D.Impulse);
	}

	IEnumerator GroundCheckStarter()
	{
		yield return null;
//		do{
//			yield return null;
//		}while(m_SceneStatus.m_enPlayerStatus == PLAYER_STATUS.LOOK_AROUND);

		StartCoroutine(GroundCheck ());
	}

	public void ReturnWaitTime()
	{
		m_fWaitTime = m_fBeforeWaitTime;
	}
	
	// Update is called once per frame
	public IEnumerator Play () {
		m_RigidBody.fixedAngle = false;
		m_bRunnerDontMoving = false;
		//bool bSkipThisFrameMove = false;

		m_iIdx = 0;

		SceneStatus sceneStatus = SceneStatus.getInstance;
		GameMgr gameMgr = GameMgr.getInstance;
		EffectManager effectMgr = EffectManager.getInstance;

		while (true) {

			if (m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER)) {

//				if(m_RigidBody.velocity.x < m_fSpeed + m_CurPlatformVelocity.x) // moving platform
//					m_RigidBody.velocity = new Vector2 (m_fSpeed, m_RigidBody.velocity.y) + new Vector2 (m_CurPlatformVelocity.x, 0);

				if(!m_bLeftMove)
				{
					if(m_RigidBody.velocity.x < m_fMaxSpeed)
						m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x + m_fAccel, m_RigidBody.velocity.y);
					else
						m_RigidBody.velocity = new Vector2(m_fMaxSpeed, m_RigidBody.velocity.y);
				}else{
					if(m_RigidBody.velocity.x > -m_fMaxSpeed)
						m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x - m_fAccel, m_RigidBody.velocity.y);
					else
						m_RigidBody.velocity = new Vector2(-m_fMaxSpeed, m_RigidBody.velocity.y);
				}


				if(transform.position.x > sceneStatus.m_fStageXPos[gameMgr.m_iCurAct-1] - 0.5f) //화면끄트머리로 못나가게 막기
				{
					transform.position = new Vector3(sceneStatus.m_fStageXPos[gameMgr.m_iCurAct-1] - 0.5f, transform.position.y);
					m_RigidBody.velocity = new Vector2(0, m_RigidBody.velocity.y);
				}else if(transform.position.x < sceneStatus.m_fStageXPosLeftest[gameMgr.m_iCurAct-1]) 
				{
					transform.position = new Vector3(sceneStatus.m_fStageXPosLeftest[gameMgr.m_iCurAct-1], transform.position.y);
					m_RigidBody.velocity = new Vector2(0, m_RigidBody.velocity.y);
				}

				if( (m_bLeftMove || Vector3.Distance(m_vecStartPos,transform.position) > 0.1f)  && !m_bRunnerDontMoving && (transform.position.x - m_fBeforeXPos).Equals(0f))//출발지점이 아니고, 샐리 안움직이고 있을때
				{
					if(fCurFadeWaitTime > 1f)
					{
						m_bRunnerDontMoving = true;
						effectMgr.RunFadeEffect(true,0.3f);

					}else
						fCurFadeWaitTime += Time.fixedDeltaTime;
				}else if(m_bRunnerDontMoving && (transform.position.x - m_fBeforeXPos > 0.01f || transform.position.x - m_fBeforeXPos < -0.01f) ) // 안움직이다가 움직이기 시작할때
				{
					effectMgr.RunFadeEffect(false);
					m_bRunnerDontMoving = false;
					fCurFadeWaitTime = 0f;
					GetComponent<AudioSource> ().Stop();
				}



//				if (m_SceneStatus.m_TabStat == TAP_STATUS.HOLD)
//				if (Input.GetMouseButton (0))
//					m_RigidBody.velocity = Vector2.zero + m_CurPlatformVelocity;

				if (Time.timeScale != 0) {
					m_ListSavePos.Add (m_iIdx,transform.localPosition);
					m_ListSaveRot.Add (m_iIdx,transform.localRotation);

					m_iIdx += 1;
				}

				m_fBeforeXPos = transform.position.x;

			} else if (m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.FLASHBACK)) {
				if (m_ListSavePos.Count - 1 - m_SceneStatus.m_iFlashBackIdx > 0) {
					if (Time.timeScale != 0) {
						transform.localPosition = m_ListSavePos [m_ListSavePos.Count - 1 - m_SceneStatus.m_iFlashBackIdx];
						transform.localRotation = m_ListSaveRot [m_ListSavePos.Count - 1 - m_SceneStatus.m_iFlashBackIdx];

						if (m_ListSavePos.Count - 1 - m_SceneStatus.m_iFlashBackIdx > 100)
							m_fFlashBackSpeed += m_fFlashBackAccel;
						else {
							if (!m_bFlashBackBreakOn) { // 빨라지던 플래쉬백 속도 줄어들기 시작
								m_fFlashBackSpeed = 4f;
								m_bFlashBackBreakOn = true;
								StartCoroutine(AudioMgr.getInstance.ChgPitch (GameObject.Find ("BGM").GetComponent<AudioSource> (), false));
							}

							m_fFlashBackSpeed -= 0.06f;

							if (m_fFlashBackSpeed < 1)
								m_fFlashBackSpeed = 1;
						}

						m_SceneStatus.m_iFlashBackIdx += (int)m_fFlashBackSpeed;
					}
				} else { // FLASHBACK END

					m_RigidBody.gravityScale = 0;
					m_iIdx = 0;

//					SallyPathCreate();
					gameMgr.FatherEnter();

				}
			} else if(m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN)){


				yield return StartCoroutine("Rewinder");



			}else if(m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_EXIT)){

				GameMgr gMgr = GameMgr.getInstance;
				
				if (gMgr.m_iCurStage.Equals(6) && gMgr.m_iStageActNum [gMgr.m_iCurChpt - 1, gMgr.m_iCurStage - 1] == gMgr.m_iCurAct) {
					//if board in transport do nothing
					m_RigidBody.velocity = Vector2.zero;
				}else
					m_RigidBody.velocity = new Vector2(m_fMaxSpeed, m_RigidBody.velocity.y);
				
				
			}else{
				m_RigidBody.velocity = Vector3.zero;
				m_RigidBody.angularVelocity = 0f;
			}

			yield return new WaitForFixedUpdate();
		}
	}

	public IEnumerator SallyOnBoard(Transform TransportBody)
	{


		do {
			yield return null;

			m_RigidBody.angularVelocity = 0f;

			if(GameMgr.getInstance.m_iCurChpt.Equals(1))
				transform.position = TransportBody.position + new Vector3 (0, 0.5f);
			else if(GameMgr.getInstance.m_iCurChpt.Equals(2))
				transform.position = TransportBody.position + new Vector3 (0, 0.5f);
			else if(GameMgr.getInstance.m_iCurChpt.Equals(3))
				transform.position = TransportBody.position + new Vector3 (0, 0.5f);
			else if(GameMgr.getInstance.m_iCurChpt.Equals(4))
				transform.position = TransportBody.position + new Vector3 (0, 0.25f);

		} while(m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_EXIT));
	}

	public void SallyPathCreate(bool bCreate = true)
	{
		int iCounter = 0;
		GameObject tmpObj;
		Transform sallyPathParent = GameObject.Find ("Stage(Clone)").transform.Find("sallyPaths").gameObject.transform;

		StageLoader stageLoader = StageLoader.getInstance;
		ObjectPool objPool = ObjectPool.getInstance;

		if (bCreate) {

			if(sallyPathParent.childCount != 0 && (sallyPathParent.childCount != 0 && sallyPathParent.GetChild(0).gameObject.activeInHierarchy))
				return;

			for (int i = 0; i < m_ListSavePos.Count; ++i) {
				if (iCounter > iPathPadding - 1) {
					iCounter = 0;

					if (!stageLoader.m_bMaptool && !stageLoader.m_bStageLoader) { //stage
						for (int j = 0; j < objPool.SallyPathInPool.Length; ++j) {
							if (objPool.SallyPathInPool [j] == null) {
								objPool.SallyPathInPool [j] = objPool.pool.NewItem ("SallyPath");
								tmpObj = objPool.SallyPathInPool [j].gameObject;
								tmpObj.transform.parent = sallyPathParent;
								tmpObj.transform.localPosition = m_ListSavePos [i];

								if(m_SceneStatus.m_bMemoryStage)
									tmpObj.GetComponent<SpriteRenderer>().color = new Color(71/255f, 63/255f, 43/255f, 0);

								break;
							}
						}
					} else { //stageloader or maptool
						tmpObj = Instantiate (Resources.Load ("Prefabs/Objects/SallyPath") as GameObject) as GameObject;
						tmpObj.transform.parent = sallyPathParent;
						tmpObj.transform.localPosition = m_ListSavePos [i];
					}
				}

				iCounter += 1;
			}
		} else {
			if(!stageLoader.m_bMaptool && !stageLoader.m_bStageLoader) // stage
			{
				for(int i = 0; i < sallyPathParent.childCount; ++i)
				{
					// sallyPath to pool
					objPool.pool.RemoveItem(sallyPathParent.GetChild(i).gameObject, "SallyPath");
				}
			}else{
				for(int i = 0; i < sallyPathParent.childCount; ++i)
				{
					// sallyPath to pool

					if(m_SceneStatus.m_bMemoryStage)
						sallyPathParent.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(71/255f, 63/255f, 43/255f, 0);
					else
						sallyPathParent.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
				}
			}
		}
	}

	IEnumerator WaitTimeIs_0()
	{
		do{
			yield return null;
		}while(m_fWaitTime.Equals(0));
	}

	IEnumerator SallyPathOpacitor(GameObject obj)
	{
		float fTime = 1f;

		float fMaxAlpha;
		if(m_SceneStatus.m_bMemoryStage)
			fMaxAlpha = 70f * 0.00392f; // 70 / 255
		else
			fMaxAlpha = 130f * 0.00392f; // 130 / 255

		float fBeforeAlpha = 0f;
		SpriteRenderer sprite = obj.GetComponent<SpriteRenderer> ();

		do{

			yield return null;

			if(fBeforeAlpha > sprite.color.a)
				break;

//			if(TimeMgr.m_bFastForward)
//				fTime = 0.3f;
//			else
//				fTime = 1f;

			sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b, sprite.color.a + (Time.deltaTime/fTime) * fMaxAlpha);

			fBeforeAlpha = sprite.color.a;

		}while(sprite.color.a < fMaxAlpha);
	}

	IEnumerator Rewinder() //리와인더 사이사이에 움직임 부드럽게 해줄 코루틴
	{
		m_RigidBody.velocity = Vector2.zero;
		m_RigidBody.angularVelocity = 0f;
		m_RigidBody.fixedAngle = false;
		m_RigidBody.gravityScale = 0f;
		bExitRewinder = false;

		bool bIgnoreThisFrame = false;

		int iSallyPathShowBoundary = 20;
		int iBeforeRemainder = 0;

		//m_RigidBody.angularDrag = 10f;
//
//		Dictionary<int, Vector2> posDic = SceneStatus.getInstance.m_array_DicRunnerPos [GameMgr.getInstance.m_iCurStage - 1];
//		Dictionary<int, Quaternion> rotDic = SceneStatus.getInstance.m_array_DicRunnerRot[GameMgr.getInstance.m_iCurStage - 1];


		SallyPathCreate ();

		Transform sallyPathParent = GameObject.Find ("Stage(Clone)").transform.Find ("sallyPaths").gameObject.transform;

		if (GameMgr.getInstance.m_bDevMode_SpeedHack)
			GetComponent<Rigidbody2D> ().isKinematic = true;
		else {
			for (int i = 0; i < iSallyPathShowBoundary; ++i) {
				StartCoroutine (SallyPathOpacitor (sallyPathParent.GetChild (i).gameObject));
			}
		}

		do{
			m_RigidBody.velocity = Vector2.zero;

			if((!m_bFatherTutorialOn && !bIgnoreThisFrame) || TimeMgr.m_bFastForward)
			{


				if(TimeMgr.m_bFastForward)
				{
					m_iIdx += 1;
				}

				if (m_iIdx < m_ListSavePos.Count) {
					if (Time.timeScale != 0) {
						transform.localPosition = m_ListSavePos[m_iIdx];
						transform.localRotation = m_ListSaveRot[m_iIdx];
						m_iIdx += 1;

						int iRemainder = m_iIdx%iPathPadding;
						
						if(iBeforeRemainder > iRemainder)
						{
							int iTmpChild = m_iIdx/iPathPadding;
							
							if(sallyPathParent.childCount > iTmpChild - 1)
								sallyPathParent.GetChild(iTmpChild - 1).gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
							
							if(sallyPathParent.childCount > iTmpChild + iSallyPathShowBoundary - 1)
								StartCoroutine(SallyPathOpacitor(sallyPathParent.GetChild(iTmpChild + iSallyPathShowBoundary - 1).gameObject));
						}


						iBeforeRemainder = iRemainder;
					}
				} else { //Runner Hit Goal!
					//bool bAllDoorOpend = true;
					bExitRewinder = true;

					m_RigidBody.gravityScale = 1.5f;
					m_RigidBody.velocity = new Vector2(m_fMaxSpeed, m_RigidBody.velocity.y);
//					GameMgr.getInstance.Clear();

				}

				iBeforeRemainder = m_iIdx%iPathPadding;

			}else if(!m_bFatherTutorialOn){
				if(m_iIdx.Equals(0))
					m_iIdx = 1;

				if(m_iIdx+1 < m_ListSavePos.Count)
					transform.localPosition = ((m_ListSavePos[m_iIdx+1] - m_ListSavePos[m_iIdx])* 0.5f) + m_ListSavePos[m_iIdx-1];
			}

			bIgnoreThisFrame = bIgnoreThisFrame ? false : true;

			if(gameObject.activeInHierarchy)
			{
				if(m_fWaitTime != 0)
				{
//					yield return StartCoroutine(WaitForAccurSecond(m_fWaitTime));
					yield return new WaitForFixedUpdate();
				}
				else
				{
					m_RigidBody.angularVelocity = 0f;
					yield return StartCoroutine(WaitTimeIs_0());
				}
			}

		}while(!bExitRewinder);

		m_RigidBody.fixedAngle = false;
		bExitRewinder = false;
		m_RigidBody.angularDrag = 1f;
	
	}


	IEnumerator WaitForAccurSecond(float animationTime)
	{

		float curTime = Time.time;

		while(true)
		{
			float standard = Time.time - curTime;
			if(standard>animationTime)
				break;
			
			yield return new WaitForFixedUpdate();
		} 
	}

	IEnumerator GroundCheck()
	{
		SceneStatus sceneStatus = SceneStatus.getInstance;
		ObjectPool objPool = ObjectPool.getInstance;

		while (true) {

			GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_TabStat = TAP_STATUS.JUMP;
			Debug.DrawRay (transform.position + new Vector3 (0.2f, 0f), Vector2.up * -0.24f, Color.red);
			Debug.DrawRay (transform.position - new Vector3 (0.2f, 0f), Vector2.up * -0.24f, Color.green);
			Debug.DrawRay (transform.position, Vector2.up * -0.23f, Color.green);
		

			m_bOnGround = false;
		
			if (Physics2D.Raycast (transform.position + new Vector3 (0.15f, 0f), -Vector2.up, 0.24f)) {//right below check
				RaycastHit2D[] allHit = Physics2D.RaycastAll (transform.position + new Vector3 (0.2f, 0f), -Vector2.up, 0.24f);
			
				foreach (RaycastHit2D hit in allHit) {
					if (hit.transform.name.Equals("PolyCollider(Clone)"))
						m_bOnGround = true;

//					if (hit.transform.name == "HoldingBox(Clone)") {
//						GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_TabStat = TAP_STATUS.HOLD;
//						m_CurPlatformVelocity = hit.transform.gameObject.GetComponent<Rigidbody2D> ().velocity;
//						m_RigidBody.velocity = new Vector2 (m_RigidBody.velocity.x, m_CurPlatformVelocity.y);
//					}
				}
			}

			if (Physics2D.Raycast (transform.position - new Vector3 (0.15f, 0f), -Vector2.up, 0.24f)) {//left below check
				RaycastHit2D[] allHit = Physics2D.RaycastAll (transform.position - new Vector3 (0.2f, 0f), -Vector2.up, 0.24f);
			
				foreach (RaycastHit2D hit in allHit) {
					if (hit.transform.name.Equals("PolyCollider(Clone)"))
						m_bOnGround = true;

//					if (hit.transform.name == "HoldingBox(Clone)") {
//						GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_TabStat = TAP_STATUS.HOLD;
//						m_CurPlatformVelocity = hit.transform.gameObject.GetComponent<Rigidbody2D> ().velocity;
//						m_RigidBody.velocity = new Vector2 (m_RigidBody.velocity.x, m_CurPlatformVelocity.y);
//					}
				}
			}

			if (Physics2D.Raycast (transform.position, -Vector2.up, 0.24f)) { //center below check
				RaycastHit2D[] allHit = Physics2D.RaycastAll (transform.position, -Vector2.up, 0.24f);
				
				foreach (RaycastHit2D hit in allHit) {
					if (hit.transform.name.Equals("PolyCollider(Clone)")) {
						
						m_bOnGround = true;
						sceneStatus.m_TabStat = TAP_STATUS.JUMP;
						m_CurPlatformVelocity = hit.transform.gameObject.GetComponent<Rigidbody2D> ().velocity;
						m_RigidBody.velocity = new Vector2 (m_RigidBody.velocity.x, m_CurPlatformVelocity.y);
					}
					
					//					if (hit.transform.name == "HoldingBox(Clone)") {
					//						GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_TabStat = TAP_STATUS.HOLD;
					//						m_CurPlatformVelocity = hit.transform.gameObject.GetComponent<Rigidbody2D> ().velocity;
					//						m_RigidBody.velocity = new Vector2 (m_RigidBody.velocity.x, m_CurPlatformVelocity.y);
					//					}
				}
			}

			if (!m_bOnGround) {
				m_CurPlatformVelocity = Vector2.zero;
			}
//			else
//				m_RigidBody.angularDrag = 5f;

			if(!m_bBeforeOnGround && m_bOnGround) //바닥이든 벽이든 부딛혔을때
			{
				GameObject jmpParticle = SceneObjectPool.getInstance.m_obj_sally_JumpParticle;
				if(jmpParticle != null)
				{
					jmpParticle.transform.position = transform.position + new Vector3(0, -0.2f);
					jmpParticle.GetComponent<ParticleSystem>().Play();
				}

				if(sceneStatus.m_bMemoryStage)
					AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "land_grass");
				else{
					GameMgr gMgr = GameMgr.getInstance;
					if(gMgr.m_iCurChpt.Equals(5))
						AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "land_grass");
					else if(gMgr.m_iCurChpt.Equals(4))
						AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "land_mud");
					else
						AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "land_rock");
				}

				//역방향으로 안구르게
				if(!m_bLeftMove)
				{
					if(m_RigidBody.angularVelocity > 0)
						m_RigidBody.angularVelocity = 0f;
				}else{
					if(m_RigidBody.angularVelocity < 0)
						m_RigidBody.angularVelocity = 0f;
				}
			}

			if(GameMgr.getInstance.m_bDevMode_SpeedHack)
				m_bOnGround = true;
		
			//Jump!
			if (m_bOnGround && FadePanel.alpha.Equals(0) && (
#if UNITY_STANDALONE_OSX
				Input.GetKey(KeyCode.JoystickButton16) ||
#elif UNITY_STANDALONE_WIN
				Input.GetKey(KeyCode.JoystickButton0) ||
#else
				(Input.GetMouseButton (0) && (UICamera.selectedObject != null && UICamera.selectedObject.name.Contains("UI Root"))) ||
#endif

				 Input.GetKey(KeyCode.Space))) {
				if (sceneStatus.m_TabStat == TAP_STATUS.JUMP && m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER)) {
					m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, 0);
					m_RigidBody.AddForce (new Vector2 (0, m_fJump), ForceMode2D.Impulse);

					AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "sallyJump");

					sceneStatus.m_iJumpCount += 1;
					if(!sceneStatus.m_bJumpAchievementGet && sceneStatus.m_iJumpCount >= 70)
					{
						//SallyJump
						///Archive_15
//						#if UNITY_ANDROID
//						GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQDw", 0);
//						#elif UNITY_IOS
//						GameCenterManager.UpdateAchievement ("sally_achiv15", 100);
//						#elif UNITY_STANDALONE
//						SteamAchieveMgr.SetAchieve("sally_achiv15");
//						#endif

						sceneStatus.m_bJumpAchievementGet = true;
					}


					yield return new WaitForSeconds(0.3f);
				}
			}

			m_bBeforeOnGround = m_bOnGround;

			yield return null;
		}
	}

//	IEnumerator ReturnFrequency()
//	{
//		yield return new WaitForEndOfFrame ();
//
//		for(int i = 0; i < transform.childCount; ++i)
//		{
//			transform.GetChild(i).GetComponent<SpringJoint2D>().frequency = 4;
//		}
//	}
}
