using UnityEngine;
using System.Collections;

public class R_Door : MonoBehaviour {

	public bool m_bSwitchPressed;

	public float m_fDoorOpenSpeed;
	public bool m_bOpenDoor;

	public Vector2 m_vecClosePos; 
	bool m_bCloseOnce;
	float m_fOpenYPos;

	Vector2 m_vPlayPos;

	bool m_bSymbolSpriteOff;

	SceneStatus sceneStatus;

	SkeletonAnimation m_symbolAnim;
	MeshRenderer m_symbolMesh;

	AudioSource m_audio;

	void Start()
	{
		m_fDoorOpenSpeed = 4.0f;
		m_fOpenYPos = GetComponent<DoorPosFixer>().m_fSize * 0.5f - 0.25f;
		m_vecClosePos = transform.position;

		m_symbolAnim = transform.GetChild (3).GetComponent<SkeletonAnimation> ();
		m_symbolAnim.skeleton.SetSkin (string.Format("chpt0{0}",GameMgr.getInstance.m_iCurChpt));

		sceneStatus = SceneStatus.getInstance;

		m_audio = GetComponent<AudioSource> ();

#if UNITY_STANDALONE
		m_audio.volume = PlayerPrefs.GetFloat("SoundVolume");
#endif


		if (sceneStatus.m_bMemoryStage)
			m_audio.clip = AudioMgr.getInstance.m_sound_door_wood[3];
		else {
			if (GameMgr.getInstance.m_iCurChpt.Equals (1) || GameMgr.getInstance.m_iCurChpt.Equals (2))
				m_audio.clip = AudioMgr.getInstance.m_sound_door_stone[3];
			else if(GameMgr.getInstance.m_iCurChpt.Equals (4))
				m_audio.clip = AudioMgr.getInstance.m_sound_door_grass[3];
			else
				m_audio.clip = AudioMgr.getInstance.m_sound_door_wood[3];
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.parent.name.Equals("Players")) {
			if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER) && !m_bOpenDoor)
				StartCoroutine ("OpenDoor");
		}
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.transform.parent.name.Equals("Players")) {
			if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.FLASHBACK))
				StartCoroutine ("CloseDoor");
		}
	}


	void OnEnable()
	{
		if(!StageLoader.getInstance.m_bMaptool)
			InitDoor ();

		Physics2D.IgnoreCollision ( GameObject.Find ("Objects").transform.GetChild(GameMgr.getInstance.m_iCurAct - 1).Find("Players").Find("Guardian(Clone)").GetComponent<CircleCollider2D>(), transform.GetChild(0).GetComponent<BoxCollider2D>());
	}

	void Update()
	{

		if (!m_bCloseOnce && sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN)) {
			InitDoor();
			StopAllCoroutines ();
		}

		if (m_bCloseOnce && sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER))
			m_bCloseOnce = false;

		GetComponent<BoxCollider2D> ().offset = new Vector2 (0f, -(transform.position.y- m_vecClosePos.y) / m_fOpenYPos / (2f/m_fOpenYPos*0.5f));
	}

	public void InitDoor()
	{
		m_bCloseOnce = true;
		m_bSwitchPressed = false;
		transform.position = m_vecClosePos;
		transform.GetChild(0).GetComponent<BoxCollider2D> ().isTrigger = false;
		m_bOpenDoor = false;

		StopAllCoroutines ();


		m_symbolAnim = transform.GetChild (3).GetComponent<SkeletonAnimation> ();
		m_symbolAnim.state.TimeScale = 1f;
		m_symbolAnim.state.SetAnimation(0, "on_idle", true);
		transform.GetChild (3).GetComponent<MeshRenderer> ().enabled = true;
		m_bSymbolSpriteOff = false;

		Transform crashChckerTrans = transform.GetChild (0).transform;
		crashChckerTrans.localScale = new Vector2 (1, GetComponent<DoorPosFixer> ().m_fSize);
		crashChckerTrans.localPosition = Vector2.zero;

		if(m_audio != null)
			m_audio.Stop ();

		//DoorSprites enable on
		for(int i = 0 ; i < transform.GetChild(1).childCount; ++i)
		{
			transform.GetChild(1).GetChild(i).GetComponent<DoorSprite>().Init();
		}
	}

	public void OpenThisDoor()
	{
		StopCoroutine ("CloseDoor");
		StartCoroutine ("OpenDoor");
	}

	public void CloseThisDoor()
	{
		StopCoroutine ("OpenDoor");
		StartCoroutine ("CloseDoor");
	}

	public IEnumerator OpenDoor()
	{
//		iTween.ShakePosition (gameObject, new Vector3 (0.05f, 0f), 0.02f);
		if (m_bOpenDoor)
			yield break;

		m_bOpenDoor = true;

		if (sceneStatus.m_bMemoryStage)
			AudioMgr.getInstance.PlaySfx (m_audio, "door_wood", Random.Range(0,3));
		else {
			if (GameMgr.getInstance.m_iCurChpt.Equals (1) || GameMgr.getInstance.m_iCurChpt.Equals (2))
				AudioMgr.getInstance.PlaySfx (m_audio, "door_stone", Random.Range(0,3));
			else if(GameMgr.getInstance.m_iCurChpt.Equals (4))
				AudioMgr.getInstance.PlaySfx (m_audio, "door_grass", Random.Range(0,3));
			else
				AudioMgr.getInstance.PlaySfx (m_audio, "door_wood", Random.Range(0,3));
		}

		if(!GameMgr.getInstance.m_bSoundMute)
			m_audio.Play ();

		transform.GetChild(0).GetComponent<BoxCollider2D> ().isTrigger = true;

		if(sceneStatus.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN)
			transform.GetChild(0).GetComponent<BoxCollider2D> ().isTrigger = true;

		m_symbolAnim.state.SetAnimation (0, "off", false);
		m_symbolAnim.state.TimeScale = 3.5f;

		Transform crashChckerTrans = transform.GetChild (0).transform;

		while (transform.position.y < m_vecClosePos.y + m_fOpenYPos) {

			if(!m_bSymbolSpriteOff && transform.position.y > m_vecClosePos.y + (m_fOpenYPos / 2f)+ 0.25f)
			{
				transform.GetChild (3).GetComponent<MeshRenderer> ().enabled = false;
				m_bSymbolSpriteOff = true;
			}

			crashChckerTrans.localScale = new Vector2( 1, crashChckerTrans.localScale.y - (m_fDoorOpenSpeed * Time.deltaTime * 2f));
			crashChckerTrans.localPosition = new Vector2 (0, crashChckerTrans.localPosition.y - m_fDoorOpenSpeed * Time.deltaTime * 0.5f);

			transform.position = new Vector2 (transform.position.x, transform.position.y + m_fDoorOpenSpeed * Time.deltaTime);
			
			yield return null;
		}

		crashChckerTrans.localScale = new Vector2 (1, 0);
		crashChckerTrans.localPosition = new Vector2 (0, -m_fOpenYPos / 2f);

		transform.position = new Vector2(transform.position.x, m_vecClosePos.y + m_fOpenYPos);

		m_audio.Stop ();
		if (sceneStatus.m_bMemoryStage)
			AudioMgr.getInstance.PlaySfx (m_audio, "door_wood", 4);
		else {
			if (GameMgr.getInstance.m_iCurChpt.Equals (1) || GameMgr.getInstance.m_iCurChpt.Equals (2))
				AudioMgr.getInstance.PlaySfx (m_audio, "door_stone", 4);
			else if(GameMgr.getInstance.m_iCurChpt.Equals (4))
				AudioMgr.getInstance.PlaySfx (m_audio, "door_grass", 4);
			else
				AudioMgr.getInstance.PlaySfx (m_audio, "door_wood", 4);
		}

	}

	public IEnumerator CloseDoor()
	{
		if (!m_bOpenDoor)
			yield break;

		m_bOpenDoor = false;

		transform.GetChild(0).GetComponent<BoxCollider2D> ().isTrigger = false;

		if (sceneStatus.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN)
			transform.GetChild (0).GetComponent<BoxCollider2D> ().isTrigger = false;

//		m_symbolAnim.loop = false;
//		m_symbolAnim.AnimationName = "on";
//		m_symbolAnim.state.End += delegate {
//			m_symbolAnim.loop = true;
//			m_symbolAnim.AnimationName = "on_idle";
//		};

		if(!GameMgr.getInstance.m_bSoundMute)
			m_audio.Play ();

		m_symbolAnim.state.TimeScale = 1f;
		m_symbolAnim.state.SetAnimation(0, "on_idle", true);

		Transform crashChckerTrans = transform.GetChild (0).transform;

		while (transform.position.y > m_vecClosePos.y) {

			if(m_bSymbolSpriteOff && transform.position.y < m_vecClosePos.y + (m_fOpenYPos/2f) + 0.5f)
			{
				transform.GetChild (3).GetComponent<MeshRenderer> ().enabled = true;
				m_bSymbolSpriteOff = false;
			}

			crashChckerTrans.localScale = new Vector2( 1, crashChckerTrans.localScale.y + ((m_fDoorOpenSpeed/3f) * Time.deltaTime * 2f));
			crashChckerTrans.localPosition = new Vector2 (0, crashChckerTrans.localPosition.y + (m_fDoorOpenSpeed/3f) * Time.deltaTime * 0.5f);

			transform.position = new Vector2 (transform.position.x, transform.position.y - (m_fDoorOpenSpeed/3f) * Time.deltaTime);

			yield return null;
		}

		crashChckerTrans.localScale = new Vector2 (1, GetComponent<DoorPosFixer> ().m_fSize);
		crashChckerTrans.localPosition = Vector2.zero;

		transform.position = new Vector2(transform.position.x, m_vecClosePos.y);

		m_audio.Stop ();
		if (sceneStatus.m_bMemoryStage)
			AudioMgr.getInstance.PlaySfx (m_audio, "door_wood", 4);
		else {
			if (GameMgr.getInstance.m_iCurChpt.Equals (1) || GameMgr.getInstance.m_iCurChpt.Equals (2))
				AudioMgr.getInstance.PlaySfx (m_audio, "door_stone", 4);
			else if(GameMgr.getInstance.m_iCurChpt.Equals (4))
				AudioMgr.getInstance.PlaySfx (m_audio, "door_grass", 4);
			else
				AudioMgr.getInstance.PlaySfx (m_audio, "door_wood", 4);
		}
	}

	public void SwitchThisDoor()
	{
		m_bSwitchPressed = true;
		m_bOpenDoor = true;
	}

}
