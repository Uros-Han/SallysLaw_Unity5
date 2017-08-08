using UnityEngine;
using System.Collections;

public class GoalMgr : MonoBehaviour {

	public bool m_bThisIsRGoal;

	SkeletonAnimation skelAnim_in;
	SkeletonAnimation skelAnim_out;
	Transform TransportBody;
	AudioSource audio;

	AudioClip m_sound_Start;


	void Start()
	{

		audio = GetComponent<AudioSource> ();

		#if UNITY_STANDALONE
		audio.volume = PlayerPrefs.GetFloat("SoundVolume");
		#endif

		if (StageLoader.getInstance.m_bMaptool || StageLoader.getInstance.m_bStageLoader) {
			GameObject decoNum = Instantiate(Resources.Load("Prefabs/Objects/Boxes/DecoNum") as GameObject) as GameObject;
			decoNum.transform.parent = transform;
			decoNum.transform.localPosition = Vector2.zero;
			decoNum.GetComponent<TextMesh>().text = "G";
			decoNum.GetComponent<TextMesh>().color = Color.yellow;
		}else if(!StageLoader.getInstance.m_bMaptool && !StageLoader.getInstance.m_bStageLoader) //stage
		{
			GameMgr gMgr = GameMgr.getInstance;

			if (gMgr.m_iCurStage.Equals(6) && gMgr.m_iStageActNum [gMgr.m_iCurChpt - 1, gMgr.m_iCurStage - 1] == gMgr.m_iCurAct) {
				//if Last Stage, Last Act
			
				skelAnim_in = GetComponent<SkeletonAnimation> ();
				skelAnim_out = transform.GetChild(0).GetComponent<SkeletonAnimation>();


				skelAnim_in.enabled = true;


				transform.GetChild(0).gameObject.SetActive(true);
				transform.GetChild(1).gameObject.SetActive(true);

				switch(GameMgr.getInstance.m_iCurChpt){
				case 1:
					skelAnim_in.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/tram_atlas/Tram_in_SkeletonData");
					skelAnim_out.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/tram_atlas/Tram_out_SkeletonData");
					transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.25f);
					transform.localPosition = new Vector3(transform.localPosition.x + 0.25f, transform.localPosition.y);
					AudioSetting(7);
					break;

				case 2:
					skelAnim_in.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/train_atlas/Train_in_SkeletonData");
					skelAnim_out.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/train_atlas/Train_out_SkeletonData");
					transform.GetChild(1).transform.localPosition = new Vector3(0f, 0.096f);
					transform.localPosition = new Vector3(transform.localPosition.x + 0.25f, transform.localPosition.y);
					AudioSetting(5);
					break;

				case 3:
					skelAnim_in.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/bus_atlas/Bus_in_SkeletonData");
					skelAnim_out.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/bus_atlas/Bus_out_SkeletonData");
					transform.GetChild(1).transform.localPosition = new Vector3(0f, 0.175f);
					GetComponent<BoxCollider2D>().offset = new Vector2(-0.25f , 0f);
					transform.localPosition = new Vector3(transform.localPosition.x + 0.25f, transform.localPosition.y - 0.25f);
					AudioSetting(1);
					break;

				case 4:
					skelAnim_in.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/carriage_atlas/Carriage_SkeletonData");
					transform.GetChild(0).gameObject.SetActive(false);
					skelAnim_out.skeletonDataAsset = null;
					transform.GetChild(1).transform.localPosition = new Vector3(0.637f, 0.122f);
					GetComponent<BoxCollider2D>().offset = new Vector2(-0.7f , 0f);
					transform.localPosition = new Vector3(transform.localPosition.x + 0.75f, transform.localPosition.y - 0.25f);
					AudioSetting(3);
					break;

				case 5:
					skelAnim_in.initialSkinName = "home";
					skelAnim_out.initialSkinName = "home";

					skelAnim_in.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/ending_atlas/Home_door/Home_door_SkeletonData");
					skelAnim_out.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/ending_atlas/Home_out/Home_out_SkeletonData");

					transform.GetChild(1).gameObject.SetActive(false);
					GameObject homeIn = Instantiate(Resources.Load("Prefabs/Home_in") as GameObject) as GameObject;
					homeIn.transform.parent = transform;
					homeIn.transform.localPosition = new Vector3(-1.25f, 0.7f);
					transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.25f);

					if(!GameMgr.getInstance.m_bSoundMute)
						m_sound_Start = AudioMgr.getInstance.m_sound_Transport[9];

					break;
				}

				skelAnim_in.Reset();
				skelAnim_out.Reset();

				GetComponent<SkeletonUtility>().SpawnHierarchy(SkeletonUtilityBone.Mode.Follow, true, true, true);
				TransportBody = transform.Find("SkeletonUtility-Root").GetChild(0).GetChild(0);


//				GetComponent<SpriteRenderer> ().sprite = ObjectPool.getInstance.m_Sprite_Vehicles [2];


//				transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + GetComponent<SpriteRenderer> ().sprite.bounds.size.y / 2f - 0.25f);

			}else{
				GetComponent<SkeletonAnimation>().enabled = false;
				GetComponent<SkeletonAnimation>().Reset();
				GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.75f);
			}
		}
	}

	public void StartVehicle(PLAYER_STATUS en_status)
	{
		skelAnim_in.loop = false;

		if (SceneStatus.getInstance.m_bFinaleStage) {
			skelAnim_in.AnimationName = "ending";

			if(m_sound_Start != null)
			{
				audio.PlayOneShot(m_sound_Start);
			}
		}
		else {
			skelAnim_in.AnimationName = "start";

			if(m_sound_Start != null)
			{
				audio.Stop();
				audio.PlayOneShot(m_sound_Start);
			}
		}

		if (GameMgr.getInstance.m_iCurChpt != 4) {
			skelAnim_out.loop = false;

			if(SceneStatus.getInstance.m_bFinaleStage)
				skelAnim_out.AnimationName = "ending";
			else
				skelAnim_out.AnimationName = "start";
		}



		skelAnim_in.state.End += delegate {
			SceneStatus.getInstance.m_enPlayerStatus = en_status;
			GameMgr.getInstance.Clear();
		};
	}


	public void ResetVehicle()
	{
		GameMgr gMgr = GameMgr.getInstance;
		if (skelAnim_in != null ) {
			skelAnim_in.loop = true;
			skelAnim_in.AnimationName = "idle";

			if (GameMgr.getInstance.m_iCurChpt != 4) {
				skelAnim_out.loop = true;
				skelAnim_out.AnimationName = "idle";
			}

		}
	}

	void AudioSetting(int iAudioIdx)
	{
		if (GameMgr.getInstance.m_bSoundMute)
			return;

		audio.clip = AudioMgr.getInstance.m_sound_Transport[iAudioIdx];
		m_sound_Start = AudioMgr.getInstance.m_sound_Transport[iAudioIdx+1];
		audio.Play();
	}


	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Runner") {

			if(StageLoader.getInstance.m_bMaptool || StageLoader.getInstance.m_bStageLoader){
				if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.RUNNER){
					SceneStatus.getInstance.GetComponent<SceneStatus>().m_enPlayerStatus = PLAYER_STATUS.FLASHBACK;
					StartCoroutine(AudioMgr.getInstance.RewindEffect());
				}
			}else
			{
				GameMgr gMgr = GameMgr.getInstance;


				if (gMgr.m_iCurStage.Equals(6) && gMgr.m_iStageActNum [gMgr.m_iCurChpt - 1, gMgr.m_iCurStage - 1] == gMgr.m_iCurAct)
				{
					//if Last Stage, Last Act
					if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER))
					{
						StartCoroutine(coll.GetComponent<Runner>().SallyOnBoard(TransportBody));
						StartVehicle(PLAYER_STATUS.RUNNER);
						SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.SALLY_EXIT;
					}else if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN)){
						StartCoroutine(coll.GetComponent<Runner>().SallyOnBoard(TransportBody));
						StartVehicle(PLAYER_STATUS.GUARDIAN);
						SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.SALLY_EXIT;
					}
				}else
				{
					if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.RUNNER) || SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
						GameMgr.getInstance.Clear();
				}

			}
		}
	}
}
