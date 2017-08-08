using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
	public GameObject m_objDoor;
	public bool m_bPressed;
	public bool m_bThisIsHoldDoor;

	Sprite m_ButtonSprite;
	Sprite m_KeySprite;
	Sprite m_PressedSprite;

//	SpriteRenderer m_SpriteRenderer;
	SkeletonAnimation m_skelAnim;
	// Use this for initialization
	BoxCollider2D m_boxCol;

	bool m_bMaptool;
	MapToolMgr m_MaptoolMgr;

	void Start () {
		m_boxCol = GetComponent<BoxCollider2D> ();

		if (GameObject.Find ("MapToolMgr") != null)
			m_bMaptool = true;

		if(m_bMaptool)
			m_MaptoolMgr = GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ();

		Physics2D.IgnoreCollision (GetComponent<BoxCollider2D>(), GameObject.Find("Main Camera").GetComponent<BoxCollider2D>());

		if(GameObject.Find("MapToolMgr") != null)
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D>(), GameObject.Find("GridLimit").GetComponent<BoxCollider2D>());

//		if (GetComponent<SpriteRenderer> ().sprite.name.Contains ("blue")) {
//			m_ButtonSprite = Resources.Load<Sprite> ("Sprites/Objects/door_button_blue");
//			m_KeySprite = Resources.Load<Sprite> ("Sprites/Objects/door_key_blue");
//		} else {
//			m_ButtonSprite = Resources.Load<Sprite> ("Sprites/Objects/door_button_yellow");
//			m_KeySprite = Resources.Load<Sprite> ("Sprites/Objects/door_key_yellow");
//		}

//		m_ButtonSprite = Resources.Load<Sprite> ("Sprites/Objects/Common/button_01");
//		m_KeySprite = SceneObjectPool.getInstance.m_sprite_Key;

//		m_PressedSprite = Resources.Load<Sprite> ("Sprites/Objects/Common/button_02");




//		m_SpriteRenderer = GetComponent<SpriteRenderer> ();


		m_skelAnim = GetComponent<SkeletonAnimation> ();

		if (m_bThisIsHoldDoor) {
			m_skelAnim.skeleton.SetSkin ("button");
			m_skelAnim.AnimationName = "button_on";
		} else {
			m_skelAnim.skeleton.SetSkin(string.Format("ch0{0}_key_WHITE",GameMgr.getInstance.m_iCurChpt));

		}
	
		m_skelAnim.skeleton.SetColor (m_objDoor.transform.GetComponent<DoorPosFixer> ().m_Color);


//		GetComponent<SpriteRenderer> ().color = m_objDoor.transform.GetChild (3).GetComponent<SpriteRenderer> ().color;

		//Sticker ();
	}

	void OnDestroy()
	{
		if (GameObject.Find ("MiniButtons") != null) {
			for(int i = 0; i < GameObject.Find ("MiniButtons").transform.childCount; ++i)
			{
				if(GameObject.Find ("MiniButtons").transform.GetChild(i).GetComponent<UIFollowTarget>().target.Equals(transform))
				{
					Destroy (GameObject.Find ("MiniButtons").transform.GetChild(i).gameObject);
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		//Sticker ();

		//HoldBorder ();

//		if (Input.GetMouseButtonDown (0) && GameObject.Find ("MapToolMgr") != null && !GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bNowPlaying 
//			&& !GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bOverayUIOn) {
//
//			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			
//			if(collider2D.OverlapPoint(mousePosition)) // Mouse Click In Rect
//			{
//				if(m_bThisIsHoldDoor)
//					m_bThisIsHoldDoor = false;
//				else
//					m_bThisIsHoldDoor = true;
//			}
//		}


//		if (m_SpriteRenderer.sprite.name != "button_02") {
//			if (m_bThisIsHoldDoor)
//				m_SpriteRenderer.sprite = m_ButtonSprite;
//			else
//				m_SpriteRenderer.sprite = m_KeySprite;
//		}

	}

	public void InitSpine()
	{
		if (m_bThisIsHoldDoor) {
			m_skelAnim.skeleton.SetSkin ("button");
			m_skelAnim.AnimationName = "button_on";
		} else {
			m_skelAnim.loop = true;
			m_skelAnim.AnimationName = "key_idle";
		}
	}

	void Sticker() // Stick to below box
	{
		bool bTmp = true;

		if (m_bMaptool)
			bTmp = m_MaptoolMgr.m_bNowPlaying;


		if (m_bThisIsHoldDoor && transform.parent.name.Equals("Switchs") && bTmp) {

			RaycastHit2D ray = Physics2D.Raycast(transform.position, -Vector2.up, 0.5f);
			if(ray.transform != null && ray.transform.CompareTag("Box"))
			{
				transform.parent = ray.transform;
			}

			Debug.DrawRay (transform.position , Vector2.up * -0.5f, Color.red);

		}else if(!m_bThisIsHoldDoor && transform.parent.name != "Switchs"){
			StartCoroutine(Detach ());
		}else if(transform.parent.name != "Switchs" && !bTmp)
		{
			StartCoroutine(Detach ());

//			if(m_bThisIsHoldDoor)
//				m_SpriteRenderer.sprite = m_ButtonSprite;
		}
	}

	IEnumerator Detach()
	{
		yield return new WaitForEndOfFrame ();

		transform.parent = GameObject.Find("Switchs").transform;
	}

	void HoldBorder()
	{
//		int tmpIdx = 0;
//		if (m_objDoor.name == "R_Door")
//			tmpIdx = 2;
//		else
//			tmpIdx = 1;
//		
//		if (m_bThisIsHoldDoor) 
//			m_objDoor.transform.GetChild (tmpIdx).gameObject.SetActive (true);
//		else
			m_objDoor.transform.GetChild(0).GetChild(0).gameObject.SetActive (false);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (GameObject.Find("MapToolMgr") != null && coll == GameObject.Find ("GridLimit").GetComponent<BoxCollider2D>())
			return;

		if (coll.transform.parent == null)
			return;

		if (coll.transform.parent.name.Equals("Players")) {
			if(m_objDoor.GetComponent<R_Door>())
		   	{
				if(coll.gameObject.name.Equals("Guardian(Clone)"))
				{
					m_objDoor.GetComponent<R_Door>().OpenThisDoor();

					if(m_bThisIsHoldDoor)
					{
						m_skelAnim.loop = false;
						m_skelAnim.AnimationName = "button_clickon";
						m_skelAnim.state.End += delegate {
							m_skelAnim.loop = true;
							m_skelAnim.AnimationName = "button_off";
						};


						AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.BUTTON_ON);

						StartCoroutine("Archive13Counter");
					}
					else
					{
						if(m_skelAnim.loop.Equals(true))
						{


							m_skelAnim.loop = false;

							GameMgr gMgr = GameMgr.getInstance;

							PlayerPrefs.SetInt("GetKeyCount", PlayerPrefs.GetInt("GetKeyCount") + 1);

							if(gMgr.m_iCurChpt.Equals(1) && gMgr.m_iCurStage.Equals(1) && gMgr.m_iCurAct.Equals(1))
							{
								m_skelAnim.AnimationName = "key_get_first";
								AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.GET_KEY_FIRST);

								/////Archive_01
//								#if UNITY_ANDROID
//								GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQAA", 0);
//								#elif UNITY_IOS
//								GameCenterManager.UpdateAchievement ("sally_achiv01", 100);
//								#elif UNITY_STANDALONE
//								SteamAchieveMgr.SetAchieve("sally_achiv01");
//								#endif
							}
							else if(gMgr.m_iCurChpt.Equals(5) && gMgr.m_iCurStage.Equals(6) && gMgr.m_iCurAct.Equals(3)){
								m_skelAnim.AnimationName = "key_get_first";
								AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.GET_KEY_FIRST);

								//Get LastKey
								/////Archive_19
//								#if UNITY_ANDROID
//								GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQEw", 0);
//								#elif UNITY_IOS
//								GameCenterManager.UpdateAchievement ("sally_achiv19", 100);
//								#elif UNITY_STANDALONE
//								SteamAchieveMgr.SetAchieve("sally_achiv19");
//								#endif
							}
							else
							{
								m_skelAnim.AnimationName = "key_get";
								AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.GET_KEY);
							}
						}
					}
				}

//				if(SceneStatus.getInstance.m_bFinaleStage)
//				{
//					if(coll.gameObject.name.Equals("Runner(Clone)"))
//					{
//						m_objDoor.GetComponent<R_Door>().OpenThisDoor();
//						
//						if(m_bThisIsHoldDoor)
//						{
////							m_SpriteRenderer.sprite = m_PressedSprite;
//							AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.BUTTON_PUSH);
//						}
//						else
//						{
////							if(m_SpriteRenderer.enabled.Equals(true))
////								AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.GET_KEY);
////							m_SpriteRenderer.enabled = false;
//						}
//					}
//				}

			}
//			else if(m_objDoor.GetComponent<G_Door>())
//			{
//				if(coll.gameObject.name.Equals("Runner(Clone)"))
//				{
//					if(GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
//					{
//						m_objDoor.GetComponent<G_Door>().OpenThisDoor();
//					}
//
//
//					if(m_bThisIsHoldDoor)
//					{
////						m_SpriteRenderer.sprite = m_PressedSprite;
//					}
//				}
//			}
		}
	}


	IEnumerator Archive13Counter()
	{
		float curTime = Time.time;
		float fGetArchiveTime = 10f;
		
		while(true)
		{
			float standard = Time.time - curTime;
			if(standard > fGetArchiveTime)
			{//get archive
				/////Archive_13
//				#if UNITY_ANDROID
//				GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQDQ", 0);
//				#elif UNITY_IOS
//				GameCenterManager.UpdateAchievement ("sally_achiv13", 100);
//				#elif UNITY_STANDALONE
//				SteamAchieveMgr.SetAchieve("sally_achiv13");
//				#endif
				break;
			}
			
			yield return new WaitForFixedUpdate();
		} 

	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (m_bThisIsHoldDoor) {
			if (GameObject.Find ("MapToolMgr") != null && coll == GameObject.Find ("GridLimit").GetComponent<BoxCollider2D> ())
				return;

			if (coll.transform.parent.name.Equals("Players")) {
				if(m_objDoor.GetComponent<R_Door>() && coll.gameObject.name.Equals("Guardian(Clone)"))
				{
					m_objDoor.GetComponent<R_Door>().CloseThisDoor();
//
//					m_skelAnim.loop = false;
//					m_skelAnim.AnimationName = "button_clickoff";
//					m_skelAnim.state.End += delegate {
//						m_skelAnim.loop = true;
						m_skelAnim.AnimationName = "button_on";
//					};


					AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.BUTTON_OFF);

					StopCoroutine("Archive13Counter");
				}
//				else if(m_objDoor.GetComponent<G_Door>())
//				{
//					if(coll.gameObject.name.Equals("Runner(Clone)"))
//				   	{
//						if(GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
//						{
//							m_objDoor.GetComponent<G_Door>().CloseThisDoor();
//						}
//
////						if(m_bThisIsHoldDoor)
////							m_SpriteRenderer.sprite = m_ButtonSprite;
//					}
//				}

			}
		}
	}
}
