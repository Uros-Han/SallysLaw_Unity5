using UnityEngine;
using System.Collections;

public class Guardian : MonoBehaviour {
	public float m_fMaxSpeed;
	public float m_fForce;

	public bool m_bOnGround;
	public float m_fJump;

	public Vector2 m_vecStartPos;

	Vector2 m_vPlayPos;

	public Vector2 m_CurPlatformVelocity;
	Rigidbody2D m_RigidBody;
	SceneStatus m_SceneStatus;
	Transform m_RunnerTransform;

	float m_fPortalMoveWaitTime = 0.25f; 
	public float m_PortalMoveSick = 0f;

	float m_fRunnerFadingBoundary = 7f;
	bool m_bGuardianInBoundary = false;

	float m_fBeforeGravityScale;
	bool bStarted;
	bool m_bBeforeOnGround;

	TextMesh m_fadeLabel; //fade warining message

	GameObject m_MegamanEffect;

	bool m_bAvialable_3DTouch = false;


	void Start()
	{
		SkinSetting ();

		if(!StageLoader.getInstance.m_bMaptool && !SceneStatus.getInstance.m_bFinaleStage)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
		
		m_RigidBody = GetComponent<Rigidbody2D> ();
		m_SceneStatus = SceneStatus.getInstance;
		m_RunnerTransform = GameObject.Find ("Runner(Clone)").transform;

		m_fMaxSpeed = 2.0f;
		m_fForce = 20.0f; //좌우 이동 주는 힘
		m_fJump = 10.75f; //11.5

		m_vecStartPos = transform.position;


		bStarted = true;

		GetComponent<TrailRenderer>().sortingLayerName = "Trail";
		GetComponent<MeshRenderer>().sortingLayerName = "Object";

		StartCoroutine (GroundCheck ());
		StartCoroutine (Fixed ());

		if (m_SceneStatus.m_bFinaleStage)
			m_RigidBody.isKinematic = false;

		if(GameObject.Find ("FadeLabel") != null)
			m_fadeLabel = GameObject.Find ("FadeLabel").GetComponent<TextMesh> ();

		m_MegamanEffect = GameObject.Find ("MegamanEffect(Clone)").gameObject;

#if UNITY_IOS
		if(ForceTouchPlugin.GetForceTouchState().Equals(ForceTouchState.Available))
		{
			m_bAvialable_3DTouch = true;	
			Debug.Log ("3D Touch Available");
		}
#endif

		transform.position -= new Vector3 (0, 0.025f);

	}

	void SkinSetting()
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

			m_RunnerTransform = GameObject.Find ("Objects").transform.GetChild(GameMgr.getInstance.m_iCurAct - 1).Find("Players").Find("Runner(Clone)").transform;

			StartCoroutine (GroundCheck ());
			StartCoroutine (Fixed ());
		}
	}

	public void PortalSickEnable()
	{
		m_PortalMoveSick = m_fPortalMoveWaitTime;
		m_RigidBody.velocity = Vector2.zero;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.transform.name.Equals("PolyCollider(Clone)")) {
			if(m_RigidBody.angularVelocity < 0)
				m_RigidBody.angularVelocity = 0f;
		}
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	public void GuardianExit()
	{
//		transform.rotation = Quaternion.AngleAxis (0, Vector3.forward);
		SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation> ();

		transform.GetChild(0).gameObject.SetActive(false);

		m_RigidBody.isKinematic = true;
		skelAnim.loop = false;
		if (!SceneStatus.getInstance.m_bMemoryStage) {
			m_MegamanEffect.GetComponent<SkeletonAnimation> ().state.SetAnimation (0,"finish_soul",false);
			m_MegamanEffect.transform.position = transform.position;

			skelAnim.AnimationName = "finish_soul";
			AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.FATHER_EXIT);
		}
		else
			skelAnim.AnimationName = "finish";

		skelAnim.state.End += delegate {
			skelAnim.enabled = false;
		};
	}

	public void GuardianEnter()
	{
		SkeletonAnimation FatherSkelAnim = GetComponent<SkeletonAnimation> ();
		
		if(!SceneStatus.getInstance.m_bMemoryStage)
		{
			GetComponent<SkeletonAnimation> ().state.SetAnimation(0, "start_soul", false);
			m_MegamanEffect.GetComponent<SkeletonAnimation> ().state.SetAnimation (0,"start_soul",false);
			m_MegamanEffect.transform.position = transform.position;
			
			AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.FATHER_ENTER);
			transform.GetChild(0).gameObject.SetActive(true); //father particle
		}
		else
			GetComponent<SkeletonAnimation> ().state.SetAnimation(0, "start", false);
		
		FatherSkelAnim.state.AddAnimation(0, "idle", true, 0);
		
		GameMgr gMgr = GameMgr.getInstance;
		if (gMgr.m_iCurChpt.Equals (1) && gMgr.m_iCurStage.Equals (4) && gMgr.m_iCurAct.Equals (1)) {
			GameObject guideLine = Instantiate(Resources.Load("Prefabs/Objects/Players/Tuto_guideLine") as GameObject) as GameObject;
			guideLine.transform.position = new Vector2(12.62f, 0.72f);
		}

		StartCoroutine (GuardianEnterFrameWait ());
	}

	IEnumerator GuardianEnterFrameWait()
	{
		yield return null;
		yield return null;
		yield return null;

		GetComponent<MeshRenderer> ().enabled = true;
	}


	public void Reset()
	{
		transform.position = m_vecStartPos;
		m_RigidBody.velocity = Vector2.zero;
		m_RigidBody.angularVelocity = 0.0f;

		GetComponent<SkeletonAnimation>().state.SetAnimation(0, "idle", true);
	}


	bool bBeforeForceTouch = false;
	bool bCurForceTouch = false;

	IEnumerator Fixed()
	{
		Rigidbody2D runnerRigid = m_RunnerTransform.GetComponent<Rigidbody2D> ();

		GameMgr gMgr = null;

		if (m_SceneStatus.m_bFinaleStage)
			gMgr = GameMgr.getInstance;

		do {

			yield return new WaitForFixedUpdate();

			//아빠가 땅밑으로 사라질때
			if(transform.position.y < -7f)
				GameMgr.getInstance.GameOver();

#if UNITY_IOS
			if(m_bAvialable_3DTouch && m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
			{
				bCurForceTouch = false;

				for(int i = 0 ; i < Input.touchCount; ++i){
					Touch t = Input.GetTouch(i);
					if(t.GetForce() / t.GetMaxForce() > 0.75f)
					{
						if(!bBeforeForceTouch)
						{
							GameObject.Find("FastForwardSprite_Dontmove").GetComponent<FastForwardBtn>().OnPress (true);
						}
						UICamera.selectedObject = GameObject.Find("FastForwardSprite_Dontmove").gameObject;
						bCurForceTouch = true;
					}
				}

				if(bBeforeForceTouch && !bCurForceTouch){
					GameObject.Find("FastForwardSprite_Dontmove").GetComponent<FastForwardBtn>().OnPress (false);
					UICamera.selectedObject = null;
				}
			}

			bBeforeForceTouch = bCurForceTouch;
#endif

			if (UICamera.selectedObject == null || (UICamera.selectedObject != null && !UICamera.selectedObject.name.Contains("Dontmove"))) {

				if (m_PortalMoveSick.Equals(0f))
					Control ();
				else {
					yield return StartCoroutine (PortalSickHealing ());
				}




			}else
				m_RigidBody.velocity = new Vector2(0, m_RigidBody.velocity.y);


			if(m_SceneStatus.m_bFinaleStage)
			{
				if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER))
				{
					m_RigidBody.velocity = new Vector2(runnerRigid.velocity.x, m_RigidBody.velocity.y);

					if(gMgr.m_iCurAct.Equals(3))
						transform.rotation = m_RunnerTransform.rotation;


				}else if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_EXIT)){
					m_RigidBody.gravityScale = 0f;
					m_RigidBody.velocity = new Vector2(runnerRigid.velocity.x, 0);


				}

				if(transform.localPosition.x >= 59.2f) // get home
				{
					m_RigidBody.velocity = Vector2.zero;
					gameObject.SetActive(false);
				}
			}

//			DistanceFade();

		} while(true);
	}

	IEnumerator PortalSickHealing()
	{
		m_fBeforeGravityScale = m_RigidBody.gravityScale;
		m_RigidBody.gravityScale = 0f;
		yield return new WaitForSeconds (m_PortalMoveSick);

		m_RigidBody.gravityScale = m_fBeforeGravityScale;
		m_PortalMoveSick = 0f;
	}

	IEnumerator GroundCheck()
	{
		while (true) {
			if (Physics2D.Raycast (transform.position, -Vector2.up, 0.24f)) {


				RaycastHit2D[] allHit = Physics2D.RaycastAll (transform.position, -Vector2.up, 0.24f);

				m_bOnGround = false;
				foreach (RaycastHit2D hit in allHit) {



					if ((hit.transform.name.Equals("PolyCollider(Clone)") || hit.transform.name.Equals("Runner(Clone)"))) {

						m_bOnGround = true;

					}

				}



			} else {
				m_CurPlatformVelocity = Vector2.zero;
			}

			if(!m_bBeforeOnGround && m_bOnGround)
			{
				//착지 소리 재생
				GameObject jmpParticle = SceneObjectPool.getInstance.m_obj_father_JumpParticle;
				if(jmpParticle != null)
				{
					jmpParticle.transform.position = transform.position + new Vector3(0, -0.25f);
					jmpParticle.GetComponent<ParticleSystem>().Play();
				}
				
				if(m_SceneStatus.m_bMemoryStage)
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
			}
		
			Debug.DrawRay (transform.position, Vector2.up * -0.3f, Color.red);



			//JUMP!
			if (Vector3.Distance (m_RunnerTransform.position, transform.position) < 0.5f) {
				m_RigidBody.velocity = new Vector2 (m_RigidBody.velocity.x + m_CurPlatformVelocity.x, 0);
				m_RigidBody.AddForce (new Vector2 (0.0f, m_fJump), ForceMode2D.Impulse);
				m_RigidBody.AddTorque (-10.0f);

				if(GameObject.Find("GuardianCam") != null)
					AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource> (), "fatherJump");

				if(PlayerPrefs.GetInt("FatherJumpCount").Equals(0))
				{
					/////Archive_02
//					#if UNITY_ANDROID
//					GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQAg", 0);
//					#elif UNITY_IOS
//					GameCenterManager.UpdateAchievement ("sally_achiv02", 100);
//					#elif UNITY_STANDALONE
//					SteamAchieveMgr.SetAchieve("sally_achiv02");
//					#endif
				}

				PlayerPrefs.SetInt("FatherJumpCount", PlayerPrefs.GetInt("FatherJumpCount") + 1);

				yield return new WaitForSeconds (0.25f);
			}

			m_bBeforeOnGround = m_bOnGround;

			yield return null;
		}
	}


	public void InitalizeDistanceFade()
	{
		if (Vector3.Distance (new Vector3 (m_RunnerTransform.position.x, 0), new Vector3 (transform.position.x, 0)) > m_fRunnerFadingBoundary) {
			m_bGuardianInBoundary = false;
			EffectManager.getInstance.RunFadeEffect (true, 0.3f);
		}
	}

	/// <summary>
	/// if guardian is far away from runner he gonna disapear
	/// </summary>
	void DistanceFade()
	{
		if (m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN)) {
//			Debug.Log(Vector3.Distance (new Vector3(m_RunnerTransform.position.x, 0), new Vector3(transform.position.x, 0)));

			if (m_bGuardianInBoundary && Vector3.Distance (new Vector3(m_RunnerTransform.position.x, 0), new Vector3(transform.position.x, 0)) > m_fRunnerFadingBoundary ) {
				m_bGuardianInBoundary = false;
				EffectManager.getInstance.RunFadeEffect (true, 0.3f);


//				GetComponent<SkeletonAnimation>().skeleton.a
			} else if (!m_bGuardianInBoundary && Vector3.Distance (new Vector3(m_RunnerTransform.position.x, 0), new Vector3(transform.position.x, 0)) <= m_fRunnerFadingBoundary) {
				m_bGuardianInBoundary = true;
				EffectManager.getInstance.RunFadeEffect (false);
			}

			if(m_fadeLabel != null)
			{
				if(!m_bGuardianInBoundary)
				{
					m_fadeLabel.color = new Color(1,1,1, Mathf.Abs(Mathf.Abs(m_RunnerTransform.position.x - transform.position.x) - m_fRunnerFadingBoundary) * 0.5f);
				}
				else
					m_fadeLabel.color = new Color(1,1,1,0);
			}
		}
	}

	void Control()
	{
		if(m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
		{
			if(!Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && Input.GetAxis("Horizontal").Equals(0f))
			{
				m_RigidBody.velocity = new Vector2(0, m_RigidBody.velocity.y);
			}else{ //Move!
#if UNITY_STANDALONE
				if( Input.GetAxis("Horizontal") < 0 || Input.GetKey(KeyCode.LeftArrow)) //left
#else
				if( (Input.GetMouseButton(0) && Input.mousePosition.x < Screen.width/2) || Input.GetAxis("Horizontal") < 0 || Input.GetKey(KeyCode.LeftArrow)) //left
#endif
				{
					if(m_RigidBody.velocity.x > (m_fMaxSpeed * -1) + m_CurPlatformVelocity.x)
						m_RigidBody.AddForce(new Vector2(-1* m_fForce, 0));
					else
						m_RigidBody.velocity = new Vector2((-1 * m_fMaxSpeed), m_RigidBody.velocity.y);

					if(transform.position.x < SceneStatus.getInstance.m_fStageXPosLeftest[GameMgr.getInstance.m_iCurAct-1]) //화면끄트머리로 못나가게 막기
					{
						transform.position = new Vector3(SceneStatus.getInstance.m_fStageXPosLeftest[GameMgr.getInstance.m_iCurAct-1], transform.position.y);
						m_RigidBody.velocity = new Vector2(0, m_RigidBody.velocity.y);
					}

					return;
				}
#if UNITY_STANDALONE
				else if( Input.GetAxis("Horizontal") > 0 || Input.GetKey(KeyCode.RightArrow)) // right
#else
				else if( (Input.GetMouseButton(0) && Input.mousePosition.x >= Screen.width/2) || Input.GetAxis("Horizontal") > 0 || Input.GetKey(KeyCode.RightArrow)) // right
#endif
				{
					if(m_RigidBody.velocity.x < m_fMaxSpeed + m_CurPlatformVelocity.x)
						m_RigidBody.AddForce(new Vector2(m_fForce, 0));
					else
						m_RigidBody.velocity = new Vector2(m_fMaxSpeed, m_RigidBody.velocity.y);

					if(transform.position.x > SceneStatus.getInstance.m_fStageXPos[GameMgr.getInstance.m_iCurAct-1] - 0.5f) //화면끄트머리로 못나가게 막기
					{
						transform.position = new Vector3(SceneStatus.getInstance.m_fStageXPos[GameMgr.getInstance.m_iCurAct-1] - 0.5f, transform.position.y);
						m_RigidBody.velocity = new Vector2(0, m_RigidBody.velocity.y);
					}

					return;
				}
			}

			
			m_RigidBody.velocity += new Vector2(m_CurPlatformVelocity.x, 0);
		}
	}
}