using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneObjectPool : MonoBehaviour {

	private static SceneObjectPool instance;
	
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this) {


			if(instance.m_strChapterName != this.m_strChapterName)
			{
				Destroy (instance.gameObject);
				instance = this;
				Debug.Log ("Chg newest sceneObjPool");
			}else{
				gameObject.SetActive(false);
				Debug.Log ("destroy same sceneObjPool");
				SceneObjectPool.getInstance.InstantiateJumpParticle();
				return;
			}
		}

		transform.parent = null;
		
		DontDestroyOnLoad (gameObject);

		Debug.Log ("objectPooling sceneObjPool");
		ObjectPoolSetting ();
	}

	void OnApplicationQuit()
	{
		instance = null;
	}
	
	
	public static SceneObjectPool getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(SceneObjectPool)) as SceneObjectPool;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("SceneObjectPool");
				instance = obj.AddComponent (typeof(SceneObjectPool)) as SceneObjectPool;
			}
			
			return instance;
		}
	}

	public string m_strChapterName;
	public int m_iLayer06Num;
	public int m_iLayer07Num;
	public int m_iLayer10Num;
	public int m_iTopDecoNum;
	public int m_iMidDecoNum;
	public int m_iDoorMidNum = 3;

	public List<Sprite> m_father_listLayer06;
	public List<Sprite> m_father_listLayer07;
	public Sprite m_father_Layer08;
	public Sprite m_father_Layer09;
	public List<Sprite> m_father_listLayer10;
	public Sprite m_father_Layer11;

	public List<Sprite> m_sally_listTopDeco;
	public List<Sprite> m_sally_listMidDeco;

	public List<Sprite> m_father_listTopDeco;
	public List<Sprite> m_father_listMidDeco;

	public Sprite[] m_sprite_sally_Box;
	public Sprite[] m_sprite_father_Box;

	public List<Sprite> m_sally_listDoor; // always [0] is bottom
	public List<Sprite> m_father_listDoor; // always [0] is bottom

	public List<Sprite> m_sally_listSpike; 
	public List<Sprite> m_father_listSpike;

	public int m_iStageIdx; //Chater "1" or Memory "1" 의 숫자들

	public GameObject m_obj_sally_JumpParticle;
	public GameObject m_obj_father_JumpParticle;

	public GameObject m_Resource_sally_JumpParticle;
	public GameObject m_Resource_father_JumpParticle;

	public SkeletonDataAsset m_Interaction_Bird;
	public SkeletonDataAsset m_Interaction_Light;
	public SkeletonDataAsset m_Interaction_Traffic;

	void ObjectPoolSetting()
	{

		string strResolution = "";

		switch (GameMgr.getInstance.DeviceResolutionWidth) {
		case 2726:
			strResolution = "2726x1536";
			break;

		case 1136:
			strResolution = "1136x640";
			break;

		default:
			Debug.LogError("UnAssignedResolution");
			break;

		}

		string strChapter = "";
		int iChapter = 5;
		m_iStageIdx = 0;

		if (!StageLoader.getInstance.m_bMaptool && !StageLoader.getInstance.m_bStageLoader) {
			strChapter = Application.loadedLevelName.Substring (0, Application.loadedLevelName.IndexOf("_"));
			iChapter = GameMgr.getInstance.m_iCurChpt;

			m_iStageIdx = System.Convert.ToInt32 (strChapter.Substring (strChapter.Length - 1, 1));

		} else {
			iChapter = 1;
			strChapter = "Chapter5";
			m_iStageIdx = 5;
		}

		string strExplainer = (strChapter.Substring (0, strChapter.Length - 1)).ToLower();

		m_sally_listDoor = new List<Sprite> ();
		m_father_listDoor = new List<Sprite> ();
		m_sally_listSpike = new List<Sprite> ();
		m_father_listSpike = new List<Sprite> ();

		////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////
		////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////////Backgrounds/////////
		 
		if (strExplainer == "chapter") {
			for (int i = 0; i < m_iLayer06Num; ++i)
				m_father_listLayer06.Add (Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/FatherBG/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_layer06_{0:00}", i)));
			for (int i = 0; i < m_iLayer07Num; ++i)
				m_father_listLayer07.Add (Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/FatherBG/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_layer07_{0:00}", i)));
		
			m_father_Layer08 = Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/FatherBG/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_layer08"));
			m_father_Layer09 = Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/FatherBG/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_layer09"));
		
			for (int i = 0; i < m_iLayer10Num; ++i)
				m_father_listLayer10.Add (Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/FatherBG/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_layer10_{0:00}", i)));
			m_father_Layer11 = Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/FatherBG/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_layer11"));

			for (int i = 0; i < m_iTopDecoNum; ++i)
				m_sally_listTopDeco.Add (Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/SallyBG/TopDeco/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_block_decoration_{0:00}", i)));
			for (int i = 0; i < m_iMidDecoNum; ++i)
				m_sally_listMidDeco.Add (Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/SallyBG/MidDeco/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_block_texture_{0:00}", i)));

			for (int i = 0; i < m_iTopDecoNum; ++i)
				m_father_listTopDeco.Add (Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/FatherBG/TopDeco/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_block_decoration_{0:00}", i)));
			for (int i = 0; i < m_iMidDecoNum; ++i)
				m_father_listMidDeco.Add (Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/FatherBG/MidDeco/chapter" + string.Format ("{0:00}", m_iStageIdx) + "_block_texture_{0:00}", i)));



			///Boxes///
			m_sprite_sally_Box = Resources.LoadAll<Sprite> ("Sprites/" + strResolution + "/" + strChapter + "/SallyBG/block_chapter" + string.Format ("{0:00}", m_iStageIdx) + "_all");
			m_sprite_father_Box = Resources.LoadAll<Sprite> ("Sprites/" + strResolution + "/" + strChapter + "/FatherBG/block_chapter" + string.Format ("{0:00}", m_iStageIdx) + "_all");
		} else if (strExplainer == "memory"){

			for (int i = 0; i < m_iTopDecoNum; ++i)
				m_sally_listTopDeco.Add (Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/TopDeco/memory" + string.Format ("{0:00}", m_iStageIdx) + "_block_decoration_{0:00}", i)));
			for (int i = 0; i < m_iMidDecoNum; ++i)
				m_sally_listMidDeco.Add (Resources.Load<Sprite> (string.Format ("Sprites/" + strResolution + "/" + strChapter + "/MidDeco/memory" + string.Format ("{0:00}", m_iStageIdx) + "_block_texture_{0:00}", i)));

			m_sprite_sally_Box = Resources.LoadAll<Sprite> ("Sprites/" + strResolution + "/" + strChapter + "/block_memory" + string.Format ("{0:00}", m_iStageIdx) + "_all");
		}


		////Objects//////////////////Objects/////////////////////Objects//////////////////Objects/////////////////////Objects///////////////////Objects//////////////////Objects/////////////////
		////Objects//////////////////Objects/////////////////////Objects//////////////////Objects/////////////////////Objects///////////////////Objects//////////////////Objects/////////////////
		if (strExplainer == "chapter") {

			m_sally_listDoor.Add (Resources.Load<Sprite> ("Sprites/Objects/" + strChapter + "/SallyObj/door_bottom"));
			m_father_listDoor.Add (Resources.Load<Sprite> ("Sprites/Objects/" + strChapter + "/FatherObj/door_bottom"));
			for (int i = 0; i < m_iDoorMidNum; ++i) {
				m_sally_listDoor.Add (Resources.Load<Sprite> (string.Format ("Sprites/Objects/" + strChapter + "/SallyObj/door_mid_{0:00}", i)));
				m_father_listDoor.Add (Resources.Load<Sprite> (string.Format ("Sprites/Objects/" + strChapter + "/FatherObj/door_mid_{0:00}", i)));


			}

			for (int i = 0; i < 3; i++) {
				m_sally_listSpike.Add (Resources.Load<Sprite> (string.Format ("Sprites/Objects/" + strChapter + "/SallyObj/spike_{0:00}", i)));
				m_father_listSpike.Add (Resources.Load<Sprite> (string.Format ("Sprites/Objects/" + strChapter + "/FatherObj/spike_{0:00}", i)));
			}


			m_Resource_sally_JumpParticle = Resources.Load("Particle/jump_dust_0" + iChapter) as GameObject;

			if(iChapter == 1 || iChapter == 2 || iChapter == 3)
				m_Resource_father_JumpParticle = Resources.Load("Particle/jump_dust_010203_Father") as GameObject;
			else if(iChapter == 4 || iChapter == 5)
				m_Resource_father_JumpParticle = Resources.Load("Particle/jump_dust_0405_Father") as GameObject;


			m_Interaction_Bird = Resources.Load<SkeletonDataAsset>("Spine/CH"+ iChapter.ToString("00") +"_object/bird/Bird_SkeletonData");
			m_Interaction_Light = Resources.Load<SkeletonDataAsset>("Spine/CH"+ iChapter.ToString("00") +"_object/street_light/Street_light_SkeletonData");
			m_Interaction_Traffic = Resources.Load<SkeletonDataAsset>("Spine/CH"+ iChapter.ToString("00") +"_object/traffic_light/Traffic_light_SkeletonData");

		}else if(strExplainer == "memory"){

			m_sally_listDoor.Add (Resources.Load<Sprite> ("Sprites/Objects/" + strChapter + "/door_bottom"));
			for (int i = 0; i < m_iDoorMidNum; ++i) {
				m_sally_listDoor.Add (Resources.Load<Sprite> (string.Format ("Sprites/Objects/" + strChapter + "/door_mid_{0:00}", i)));
			}
			for (int i = 0; i < 3; i++) {
				m_sally_listSpike.Add (Resources.Load<Sprite> (string.Format ("Sprites/Objects/" + strChapter + "/spike_{0:00}", i)));
			}

//			m_sprite_DoorSymbol_On = Resources.Load<Sprite> ("Sprites/Objects/" + strChapter + "/door_on");
//			m_sprite_DoorSymbol_Off = Resources.Load<Sprite> ("Sprites/Objects/" + strChapter + "/door_off");

			m_Resource_sally_JumpParticle = Resources.Load("Particle/jump_dust_memory") as GameObject;
			m_Resource_father_JumpParticle = Resources.Load("Particle/jump_dust_memory") as GameObject;


		}

		InstantiateJumpParticle ();
	}

	void InstantiateJumpParticle()
	{
		m_obj_sally_JumpParticle = Instantiate (m_Resource_sally_JumpParticle) as GameObject;
		m_obj_father_JumpParticle = Instantiate(m_Resource_father_JumpParticle) as GameObject;
	}
}
