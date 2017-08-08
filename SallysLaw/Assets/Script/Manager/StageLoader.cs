using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StageLoader : MonoBehaviour {

	private static StageLoader instance;
	
	public static StageLoader getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(StageLoader)) as StageLoader;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("StageLoader");
				instance = obj.AddComponent (typeof(StageLoader)) as StageLoader;
			}
			
			return instance;
		}
	}

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

	
	public bool m_bMaptool;
	public bool m_bStageLoader;

	ObjectPool objPool;
	GameMgr gameMgr;
	SceneObjectPool sceneObjPool;

	void Start () {

		gameMgr = GameMgr.getInstance;
		objPool = ObjectPool.getInstance;

		gameMgr.m_ListStage_string.Clear();

		if(!m_bMaptool) // is this Stage Scene?
		{
			//LoadStage(Application.loadedLevelName);

			gameMgr.SendMessage("StartStage");


			if(!m_bStageLoader)
			{

				gameMgr.m_iCurAct = 1;
				string tmpStageName = "stage" +gameMgr.m_iCurChpt + "-" + string.Format("{0:00}", gameMgr.m_iCurStage);

				for(int i = 0 ; i < gameMgr.m_iStageActNum[gameMgr.m_iCurChpt-1, gameMgr.m_iCurStage - 1]; ++i)
				{
					LoadStage(tmpStageName + "_" +string.Format ("{0:00}", i+1));
				}

			}
			StartCoroutine(FadeWait());
		}


	}

	IEnumerator FadeWait()
	{
		UIManager.getInstance.CurtainOn ();
		yield return new WaitForEndOfFrame ();
		UIManager.getInstance.StageStart ();
	}

	void CreateStartVehicle()
	{

		GameObject vehicle = null;

		if (gameMgr.m_iCurChpt == 1 && gameMgr.m_iCurStage == 2 && gameMgr.m_iCurAct == 1) {
			vehicle = Instantiate(objPool.m_StartVehicle) as GameObject;

			Vector3 RunnerPos = GameObject.Find("Runner(Clone)").transform.position;

			vehicle.transform.position = new Vector2 (RunnerPos.x, RunnerPos.y - 0.2f);
		}else if(gameMgr.m_iCurChpt == 2 && gameMgr.m_iCurStage == 2 && gameMgr.m_iCurAct == 1){
			vehicle = Instantiate(objPool.m_StartVehicle) as GameObject;
			
			Vector3 RunnerPos = GameObject.Find("Runner(Clone)").transform.position;
			
			vehicle.transform.position = new Vector2 (RunnerPos.x, RunnerPos.y - 0.2f);
		}else if(gameMgr.m_iCurChpt == 3 && gameMgr.m_iCurStage == 2 && gameMgr.m_iCurAct == 1){
			vehicle = Instantiate(objPool.m_StartVehicle) as GameObject;
			
			Vector3 RunnerPos = GameObject.Find("Runner(Clone)").transform.position;
			vehicle.transform.position = new Vector2 (RunnerPos.x, RunnerPos.y - 0.2f);
		}else if(gameMgr.m_iCurChpt == 4 && gameMgr.m_iCurStage == 2 && gameMgr.m_iCurAct == 1){
			vehicle = Instantiate(objPool.m_StartVehicle) as GameObject;
			
			Vector3 RunnerPos = GameObject.Find("Runner(Clone)").transform.position;
			
			vehicle.transform.position = new Vector2 (RunnerPos.x, RunnerPos.y - 0.2f);
		}else if(gameMgr.m_iCurChpt == 5 && gameMgr.m_iCurStage == 1 && gameMgr.m_iCurAct == 1){
			vehicle = Instantiate(objPool.m_StartVehicle) as GameObject;
			
			Vector3 RunnerPos = GameObject.Find("Runner(Clone)").transform.position;
			
			vehicle.transform.position = new Vector2 (RunnerPos.x, RunnerPos.y - 0.2f);
		}

		if(vehicle != null)
			vehicle.transform.parent = GameObject.Find("Goal").transform;

	}

	public void LoadStage(string strFileName)
	{
		Stage m_Stage = null;

		if (m_bStageLoader || m_bMaptool) {
			string strDir = Application.persistentDataPath + "/Stages";
			if (!Directory.Exists (strDir)) {
				return;
			}

			if (!File.Exists (strDir + "/" + strFileName)) {
				Debug.LogError ("Cant Find File");
				return;
			}

			FileStream fs2 = new FileStream (strDir + "/" + strFileName, FileMode.Open, FileAccess.Read);
			BinaryFormatter bf2 = new BinaryFormatter();
			m_Stage = (Stage)bf2.Deserialize(fs2);
			fs2.Close();
		}else if(!m_bStageLoader && !m_bMaptool)
		{
			TextAsset ta = Resources.Load("Stgs/" + strFileName) as TextAsset;
			Stream s = new MemoryStream(ta.bytes);
			BinaryFormatter bf2 = new BinaryFormatter();
			m_Stage = (Stage)bf2.Deserialize(s);
			s.Close();

			StartCoroutine(DragTime ());
		}

		if (m_bMaptool) { // If this is maptool Clr already exist Objects.

			if (GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_listObjects.Count > 0) {
				for (int i = 0; i < GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_listObjects.Count; ++i) {
					Destroy (GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_listObjects [i].m_Object);
				}

				GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_listObjects.Clear ();
			}
		}


		 
		if (m_bMaptool) { //maptool
			StartCoroutine (CreateObjects (m_Stage));
		}
		else if (m_bStageLoader) { // StagePlayer
			StartCoroutine (CreateObjects (m_Stage));
			GameMgr.getInstance.m_ListStage_string.Add(m_Stage);

		}
		else { // stage
			gameMgr.m_ListStage_string.Add(m_Stage);
			if(System.Convert.ToInt32(strFileName.Substring(10,2)) == gameMgr.m_iCurAct)
				StartCoroutine (CreateObjects (m_Stage));

			objPool.MemoryPoolCreate();
		}
	}

	IEnumerator DragTime()
	{
		TimeMgr.Pause ();

		float fStart = Time.realtimeSinceStartup;
		while(Time.realtimeSinceStartup < fStart + 0.5f)
		{
			yield return null;
		}

		TimeMgr.Play ();


	}

	public IEnumerator CreateObjects(Stage m_Stage, bool bCreateOnly_MemoryPoolObject = false, GameObject StageParent = null)
	{
		yield return new WaitForEndOfFrame();

//		GameObject StageParent = null;

		sceneObjPool = SceneObjectPool.getInstance;
		SceneStatus sceneStatus = SceneStatus.getInstance;

		if (!m_bMaptool && !m_bStageLoader) {
			if(StageParent == null)
			{
				StageParent = Instantiate (objPool.m_Stage) as GameObject;
				StageParent.transform.parent = GameObject.Find ("Objects").transform;
			}

			SceneStatus.getInstance.m_objCurStage = StageParent;

//			gameMgr.DisableBeforeStage();
		}
		if (!bCreateOnly_MemoryPoolObject) {
			ObjFactory (m_Stage.m_Runner, OBJECT_ID.RUNNER, m_bMaptool, m_Stage, StageParent);
			ObjFactory (m_Stage.m_Guardian, OBJECT_ID.GUARDIAN, m_bMaptool, m_Stage, StageParent);
		}

		for(int i = 0 ; i < m_Stage.m_listBox.Count; ++i)
		{
			ObjFactory (m_Stage.m_listBox[i], OBJECT_ID.BOX, m_bMaptool, m_Stage, StageParent);
		}
		
		for(int i = 0 ; i < m_Stage.m_listSpikes.Count; ++i)
		{
			ObjFactory (m_Stage.m_listSpikes[i], OBJECT_ID.SPIKE, m_bMaptool,m_Stage,StageParent);
		}

		if (m_Stage.m_listFloor != null) {
			for (int i = 0; i < m_Stage.m_listFloor.Count; ++i) {
				ObjFactory (m_Stage.m_listFloor [i], OBJECT_ID.FLOOR, m_bMaptool,m_Stage,StageParent);
			}
		}

		if (!bCreateOnly_MemoryPoolObject) {
			ObjFactory (m_Stage.m_Goal, OBJECT_ID.GOAL, m_bMaptool, m_Stage, StageParent);

			if(m_Stage.m_Photo != null)
				ObjFactory (m_Stage.m_Photo, OBJECT_ID.PHOTO, m_bMaptool,m_Stage,StageParent);

			for(int i = 0 ; i < m_Stage.m_listR_Door.Count; ++i)
			{
				ObjFactory (m_Stage.m_listR_Door[i], OBJECT_ID.R_DOOR, m_bMaptool,m_Stage,StageParent);
			}
			for(int i = 0 ; i < m_Stage.m_listR_Switch.Count; ++i)
			{
				ObjFactory (m_Stage.m_listR_Switch[i], OBJECT_ID.R_SWITCH, m_bMaptool,m_Stage,StageParent);
			}
			for(int i = 0 ; i < m_Stage.m_listSprings.Count; ++i)
			{
				ObjFactory (m_Stage.m_listSprings[i], OBJECT_ID.SPRING, m_bMaptool,m_Stage,StageParent);
			}
			for(int i = 0 ; i < m_Stage.m_listPortals.Count; ++i)
			{
				ObjFactory (m_Stage.m_listPortals[i], OBJECT_ID.PORTAL, m_bMaptool,m_Stage,StageParent);
			}

			if (m_Stage.m_listTextFloats != null) {
				for (int i = 0; i < m_Stage.m_listTextFloats.Count; ++i) {
					ObjFactory (m_Stage.m_listTextFloats [i], OBJECT_ID.TEXTFLOAT, m_bMaptool, m_Stage, StageParent);
				}
			}
			
			if (m_Stage.m_listTimeballs != null) {
				for (int i = 0; i < m_Stage.m_listTimeballs.Count; ++i) {
					ObjFactory (m_Stage.m_listTimeballs [i], OBJECT_ID.Timeball, m_bMaptool,m_Stage,StageParent);
				}
			}

			for(int i = 0; i < m_Stage.m_listPortals.Count; ++i)
			{
				PortalAssignOwner(m_Stage.m_listPortals[i], i, StageParent);
			}

			if (m_Stage.m_listTopDecos != null) {
				for(int i = 0 ; i < m_Stage.m_listTopDecos.Count; ++i)
				{
					ObjFactory (m_Stage.m_listTopDecos[i], OBJECT_ID.TOP_DECO, m_bMaptool, m_Stage, StageParent);
				}
			}

			if (m_Stage.m_listInteractionProps != null) {
				for(int i = 0 ; i < m_Stage.m_listInteractionProps.Count; ++i)
				{
					ObjFactory (m_Stage.m_listInteractionProps[i], OBJECT_ID.INTERACTION_PROP, m_bMaptool, m_Stage, StageParent);
				}
			}


			if (StageParent != null) {
				if(!sceneStatus.m_bFirstLoaded)
				{
					StageParent.transform.position = new Vector3(sceneStatus.m_fStageXPos[sceneStatus.m_fStageXPos.Count-1] ,0);
					
					sceneStatus.m_fStageXPos.Add(sceneStatus.m_fStageXPos[sceneStatus.m_fStageXPos.Count-1] + System.Convert.ToInt32(gameMgr.m_ListStage_string[gameMgr.m_iCurAct-1].m_strWidthOfThisMap) * 0.5f);
					sceneStatus.m_fStageXPosLeftest.Add ( sceneStatus.m_fStageXPos[sceneStatus.m_fStageXPos.Count-2]);
				}else{
					StageParent.transform.position = Vector2.zero;
					
					sceneStatus.m_fStageXPos.Add(System.Convert.ToInt32(gameMgr.m_ListStage_string[gameMgr.m_iCurAct-1].m_strWidthOfThisMap) * 0.5f);
					sceneStatus.m_fStageXPosLeftest.Add (0f);
				}
			}

			if (m_bMaptool) {
				GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr>().OnCheck();
			}
			
			ColliderMgr.getInstance.CheckAllBoxes ();
		}


		if(sceneStatus.m_enPlayerStatus == PLAYER_STATUS.STAGE_CHG_FATHER || sceneStatus.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN)
		{
			
			gameMgr.ChangeBackgrounds (true);
		}

		if(!m_bMaptool && !m_bStageLoader && sceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_WAIT))
			CreateStartVehicle ();
	}


	void ObjFactory(string strInfo, OBJECT_ID objID, bool bMaptool, Stage m_Stage = default(Stage) ,GameObject StageParent = null)
	{
		Vector2 ObjectPos = StringConverter (strInfo);
		GameObject TmpObj = null;	

		List<int> tmpGridIdx = new List<int> ();

		switch (objID) {
		case OBJECT_ID.RUNNER:
//			GameObject.Find("Main Camera").GetComponent<CamMoveMgr>().m_fRunnerMaintainYPos = ObjectPos.y + GameObject.Find("Main Camera").GetComponent<CamMoveMgr>().m_fRunnerYPosFixer;
			TmpObj = Instantiate(objPool.m_Runner) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Players").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Players").gameObject.transform;

			GameObject.Find("Main Camera").GetComponent<CamMoveMgr>().m_RunnerTransform = TmpObj.transform;
			TmpObj.GetComponent<MeshRenderer>().sortingLayerName = "Object";
			if(!bMaptool)
			{
				TmpObj.AddComponent<Runner>();	
				TmpObj.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
				TmpObj.transform.position = ObjectPos;
				//GameObject.Find ("Main Camera").transform.position = TmpObj.transform.position + new Vector3 (0.2f, GameObject.Find("Main Camera").GetComponent<CamMoveMgr>().m_fYPosFixer);
			}
			break;
		case OBJECT_ID.GUARDIAN:
			TmpObj = Instantiate(objPool.m_Guardian) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Players").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Players").gameObject.transform;

			GameObject.Find("Main Camera").GetComponent<CamMoveMgr>().m_GuardianTransform = TmpObj.transform;
			TmpObj.GetComponent<MeshRenderer>().sortingLayerName = "Object";
			if(!bMaptool)
			{
				TmpObj.AddComponent<Guardian>();
				TmpObj.GetComponent<Rigidbody2D>().gravityScale = 2.0f;
			}
			break;
		case OBJECT_ID.GOAL:
			TmpObj = Instantiate(objPool.m_Goal) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Goal").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Goal").gameObject.transform;

			break;

		case OBJECT_ID.PHOTO:
			TmpObj = Instantiate(objPool.m_Photo) as GameObject;
			
			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Goal").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Goal").gameObject.transform;
			
			break;

		case OBJECT_ID.BOX:
			if(!m_bMaptool && !m_bStageLoader) // stage
			{
				for(int i = 0; i < objPool.boxInPool.Length ; ++i)
				{
					if(objPool.boxInPool[i] == null)
					{
						objPool.boxInPool[i] = objPool.pool.NewItem("Box");
						TmpObj = objPool.boxInPool[i].gameObject;
						break;
					}
				}

				if(TmpObj == null)
					Debug.LogError("ObjectPool is Full");

			}else
				TmpObj = Instantiate(objPool.m_Box) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Boxes").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Boxes").gameObject.transform;

			if(strInfo.IndexOf("$") != -1)
			{
				int iBoxIdx = System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1,strInfo.Length-(strInfo.IndexOf("$")+1)));

//				if(SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.STAGE_CHG)
//					TmpObj.GetComponent<SpriteRenderer>().sprite = SceneobjPool.m_sprite_father_Box[iBoxIdx];
//				else
					TmpObj.GetComponent<SpriteRenderer>().sprite = sceneObjPool.m_sprite_sally_Box[iBoxIdx];

				if(!SceneStatus.getInstance.m_bMemoryStage)
				{
					TmpObj.GetComponent<Background>().m_sallyBG = sceneObjPool.m_sprite_sally_Box[iBoxIdx];
					TmpObj.GetComponent<Background>().m_fatherBG = sceneObjPool.m_sprite_father_Box[iBoxIdx];
				}
			}

			if(!m_bMaptool)
				Destroy(TmpObj.GetComponent<BoxMaptool>());
			break;

		case OBJECT_ID.SPIKE:

			if(!m_bMaptool && !m_bStageLoader) // stage
			{
				for(int i = 0; i < objPool.SpikeInPool.Length ; ++i)
				{
					if(objPool.SpikeInPool[i] == null)
					{
						objPool.SpikeInPool[i] = objPool.pool.NewItem("Spike");
						TmpObj = objPool.SpikeInPool[i].gameObject;
						break;
					}
				}
				
				if(TmpObj == null)
					Debug.LogError("ObjectPool is Full");
				
			}else
				TmpObj = Instantiate(objPool.m_Spike) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Spikes").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Spikes").gameObject.transform;

			if(strInfo.IndexOf("%").Equals(-1))
				TmpObj.transform.rotation = Quaternion.AngleAxis( System.Convert.ToSingle(strInfo.Substring(strInfo.IndexOf("$")+1,strInfo.Length-(strInfo.IndexOf("$")+1))) ,Vector3.forward);
			else
				TmpObj.transform.rotation = Quaternion.AngleAxis( System.Convert.ToSingle(strInfo.Substring(strInfo.IndexOf("$")+1,strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1))) ,Vector3.forward);

			if(strInfo.Substring(strInfo.IndexOf("%")+1, 1).Equals("R")) //if this spike is Revival spike
			{
				TmpObj.GetComponent<Spike>().m_bRevival = true;
				TmpObj.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.green;
			}
			else
			{
				TmpObj.GetComponent<Spike>().m_bRevival = false;
				TmpObj.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
			}

			if(!m_bMaptool)
			{
				Destroy(TmpObj.GetComponent<BoxMaptool>());
				TmpObj.GetComponent<Spike>().enabled = true;
			}
			break;

		case OBJECT_ID.R_BOX:
			TmpObj = Instantiate (Resources.Load ("Prefabs/Objects/Boxes/R_Box") as GameObject) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Boxes").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Boxes").gameObject.transform;

			if(!m_bMaptool)
				Destroy(TmpObj.GetComponent<BoxMaptool>());
			break;
		case OBJECT_ID.G_BOX:
			TmpObj = Instantiate (Resources.Load ("Prefabs/Objects/Boxes/G_Box") as GameObject) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Boxes").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Boxes").gameObject.transform;

			if(!m_bMaptool)
				Destroy(TmpObj.GetComponent<BoxMaptool>());
			break;
		case OBJECT_ID.HOLDINGBOX:
			TmpObj = Instantiate (Resources.Load ("Prefabs/Objects/Boxes/HoldingBox") as GameObject) as GameObject;
		
			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Boxes").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Boxes").gameObject.transform;

			if(!m_bMaptool)
				Destroy(TmpObj.GetComponent<BoxMaptool>());
			break;
		case OBJECT_ID.R_DOOR:
			TmpObj = Instantiate(objPool.m_Door) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Doors").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Doors").gameObject.transform;

			if(!bMaptool)
			{
				TmpObj.transform.GetChild(0).gameObject.AddComponent<R_Door>();
				Destroy(TmpObj.transform.GetChild(0).GetChild(0).GetComponent<DoorMaptool>());
			}
			//TmpObj.transform.GetChild(0).localScale = new Vector3(1,System.Convert.ToSingle(strInfo.Substring(strInfo.IndexOf("$")+1,strInfo.Length-(strInfo.IndexOf("$")+1))));

			Color tmpColor;
			if(strInfo.IndexOf("%") != -1)
			{
				TmpObj.transform.GetChild(0).GetComponent<DoorPosFixer>().m_fSize = System.Convert.ToSingle(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)));

				tmpColor = StringToColor(strInfo.Substring(strInfo.IndexOf("%")+1, strInfo.Length-(strInfo.IndexOf("%")+1)));
			}
			else //컬러 변경 이전에 만든 맵 호완을위해
			{
				TmpObj.transform.GetChild(0).GetComponent<DoorPosFixer>().m_fSize = System.Convert.ToSingle(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.Length-(strInfo.IndexOf("$")+1)));

				tmpColor = new Color(87/255f, 223/255f, 255/255f);
			}

			Transform SpriteParent = TmpObj.transform.GetChild(0).GetChild(1).transform;

			TmpObj.transform.GetChild(0).GetComponent<DoorPosFixer>().m_Color = tmpColor;
			TmpObj.transform.GetChild(0).GetComponent<DoorPosFixer>().PosFixer();
			
			break;
		case OBJECT_ID.G_DOOR:
			TmpObj = Instantiate (Resources.Load ("Prefabs/Objects/Doors/G_Door") as GameObject) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Doors").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Doors").gameObject.transform;

			if(!bMaptool)
			{
				TmpObj.transform.GetChild(0).gameObject.AddComponent<G_Door>();
				Destroy(TmpObj.transform.GetChild(0).GetComponent<DoorMaptool>());
			}
			TmpObj.transform.GetChild(0).GetComponent<DoorPosFixer>().m_fSize = System.Convert.ToSingle(strInfo.Substring(strInfo.IndexOf("$")+1,strInfo.Length-(strInfo.IndexOf("$")+1)));
			TmpObj.transform.GetChild(0).GetComponent<DoorPosFixer>().PosFixer();
		
			break;
		case OBJECT_ID.R_SWITCH:
			TmpObj = Instantiate(objPool.m_Switch) as GameObject;

			if(StageParent == null)
			{
				TmpObj.transform.parent = GameObject.Find ("Switchs").gameObject.transform;
				TmpObj.GetComponent<Switch>().m_objDoor = GameObject.Find("Doors").transform.GetChild(System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).GetChild(0).gameObject;
			}
			else
			{
				TmpObj.transform.parent = StageParent.transform.Find("Switchs").gameObject.transform;
				TmpObj.GetComponent<Switch>().m_objDoor = StageParent.transform.Find("Doors").GetChild(System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).GetChild(0).gameObject;
			}


			if(strInfo.Substring(strInfo.IndexOf("%")+1, 1).Equals("H"))
				TmpObj.GetComponent<Switch>().m_bThisIsHoldDoor = true;
			break;
		case OBJECT_ID.G_SWITCH:
			TmpObj = Instantiate (Resources.Load ("Prefabs/Objects/Switchs/G_Switch") as GameObject) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Switchs").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Switchs").gameObject.transform;

			TmpObj.GetComponent<Switch>().m_objDoor = GameObject.Find("Doors").transform.GetChild(m_Stage.m_listR_Door.Count + System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).GetChild(0).gameObject;
			if(strInfo.Substring(strInfo.IndexOf("%")+1, 1).Equals("H"))
				TmpObj.GetComponent<Switch>().m_bThisIsHoldDoor = true;
			break;

		case OBJECT_ID.MOVEORDER:
			TmpObj = Instantiate (Resources.Load ("Prefabs/Objects/Boxes/MoveOrder") as GameObject) as GameObject;

			if(strInfo.Substring(strInfo.IndexOf("%")+1,1).Equals("B"))
				TmpObj.GetComponent<MoveOrder>().m_objOwner =  GameObject.Find("Boxes").transform.GetChild(System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject;
//			else if(strInfo.Substring(strInfo.IndexOf("%")+1,1) == "H")
//				TmpObj.GetComponent<MoveOrder>().m_objOwner =  GameObject.Find("Boxes").transform.GetChild(m_Stage.m_listBox.Count + System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject;
//			else if(strInfo.Substring(strInfo.IndexOf("%")+1,1) == "R")
//				TmpObj.GetComponent<MoveOrder>().m_objOwner =  GameObject.Find("Boxes").transform.GetChild(m_Stage.m_listBox.Count + m_Stage.m_listHoldingBox.Count + System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject;
//			else if(strInfo.Substring(strInfo.IndexOf("%")+1,1) == "G")
//				TmpObj.GetComponent<MoveOrder>().m_objOwner =  GameObject.Find("Boxes").transform.GetChild(m_Stage.m_listBox.Count + m_Stage.m_listHoldingBox.Count + m_Stage.m_listR_Box.Count + System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject;
//			else if(strInfo.Substring(strInfo.IndexOf("%")+1,1) == "S")
//				TmpObj.GetComponent<MoveOrder>().m_objOwner =  GameObject.Find("Spikes").transform.GetChild(System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject;
			else
				Debug.LogError("ERRORRR");

			if(!bMaptool)
			{
				TmpObj.GetComponent<MoveOrder>().m_bPlay = true;
				TmpObj.GetComponent<SpriteRenderer>().enabled = false;
				TmpObj.transform.GetChild(0).gameObject.SetActive(false);
			}
			//TmpObj.GetComponent<MoveOrder>().Init();

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("MoveOrders").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("MoveOrders").gameObject.transform;

			TmpObj.transform.position = ObjectPos;

			break;

		case OBJECT_ID.FLOOR:

			if(!m_bMaptool && !m_bStageLoader) // stage
			{
				for(int i = 0; i < objPool.floorInPool.Length ; ++i)
				{
					if(objPool.floorInPool[i] == null)
					{
						objPool.floorInPool[i] = objPool.pool.NewItem("Floor");
						TmpObj = objPool.floorInPool[i].gameObject;
						break;
					}
				}




				TmpObj.GetComponent<SpriteRenderer>().sprite = sceneObjPool.m_sprite_sally_Box[5];

				if(!SceneStatus.getInstance.m_bMemoryStage)
				{
					TmpObj.GetComponent<Background>().m_sallyBG = sceneObjPool.m_sprite_sally_Box[5];
					TmpObj.GetComponent<Background>().m_fatherBG = sceneObjPool.m_sprite_father_Box[5];
				}

				if(TmpObj == null)
					Debug.LogError("ObjectPool is Full");
			}else
				TmpObj = Instantiate(objPool.m_Floor) as GameObject;

			if(strInfo.Substring(strInfo.IndexOf("%")+1,1).Equals("B"))
			{
				if(StageParent == null)
					TmpObj.transform.parent = GameObject.Find("Boxes").transform.GetChild(System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject.transform;
				else
					TmpObj.transform.parent = StageParent.transform.Find("Boxes").transform.GetChild(System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject.transform;
			}
			else if(strInfo.Substring(strInfo.IndexOf("%")+1,1) == "H")
				TmpObj.transform.parent = GameObject.Find("Boxes").transform.GetChild(m_Stage.m_listBox.Count + System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject.transform;
			else if(strInfo.Substring(strInfo.IndexOf("%")+1,1) == "R")
				TmpObj.transform.parent = GameObject.Find("Boxes").transform.GetChild(m_Stage.m_listBox.Count + m_Stage.m_listHoldingBox.Count + System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject.transform;
			else if(strInfo.Substring(strInfo.IndexOf("%")+1,1) == "G")
				TmpObj.transform.parent = GameObject.Find("Boxes").transform.GetChild(m_Stage.m_listBox.Count + m_Stage.m_listHoldingBox.Count + m_Stage.m_listR_Box.Count + System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject.transform;
			else if(strInfo.Substring(strInfo.IndexOf("%")+1,1) == "S")
				TmpObj.transform.parent = GameObject.Find("Spikes").transform.GetChild(System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1)))).gameObject.transform;
			else
				Debug.LogError("ERRORRR");
			break;

		case OBJECT_ID.Timeball:
			TmpObj = Instantiate(objPool.m_Timeball) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Timeballs").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Timeballs").gameObject.transform;

			break;

		case OBJECT_ID.SPRING:
			TmpObj = Instantiate(objPool.m_Spring) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Springs").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Springs").gameObject.transform;

			TmpObj.transform.rotation = Quaternion.AngleAxis( System.Convert.ToSingle(strInfo.Substring(strInfo.IndexOf("$")+1,strInfo.Length-(strInfo.IndexOf("$")+1))) ,Vector3.forward);
			if(!m_bMaptool)
			{
				Destroy(TmpObj.GetComponent<BoxMaptool>());
				TmpObj.GetComponent<Spring>().enabled = true;
			}
			break;

		case OBJECT_ID.PORTAL:
			TmpObj = Instantiate(objPool.m_Portal) as GameObject;

			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("Portals").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("Portals").gameObject.transform;

			if(!m_bMaptool)
			{
				Destroy(TmpObj.GetComponent<BoxMaptool>());
				TmpObj.GetComponent<Portal>().enabled = true;
			}

			string tmpSkin = strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.IndexOf("%") - (strInfo.IndexOf("$")+1));

			if(tmpSkin.Contains("portal"))
			{
				TmpObj.GetComponent<SkeletonAnimation>().initialSkinName = tmpSkin;
				TmpObj.GetComponent<SkeletonAnimation>().Reset();
			}

//			TmpObj.GetComponent<SpriteRenderer>().color = StringToColor(strInfo.Substring(strInfo.IndexOf("$")+1, strInfo.Length - (strInfo.IndexOf("$")+1)));


			break;


		case OBJECT_ID.TEXTFLOAT:
			TmpObj = Instantiate(objPool.m_TextFloat_Pos) as GameObject;

			TmpObj.GetComponent<TextFloat_Pos>().m_strKey = strInfo.Substring(strInfo.IndexOf("$")+1,strInfo.IndexOf("%")-(strInfo.IndexOf("$")+1));


			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("TextFloatPos").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("TextFloat_Pos").gameObject.transform;


			if(strInfo.Substring(strInfo.IndexOf("%")+1,strInfo.Length-(strInfo.IndexOf("%")+1)).Equals("T"))
				TmpObj.GetComponent<TextFloat_Pos>().m_bFatherText = true;
			else if(strInfo.Substring(strInfo.IndexOf("%")+1,strInfo.Length-(strInfo.IndexOf("%")+1)).Contains("M"))
			{
				TmpObj.GetComponent<TextFloat_Pos>().m_bMemoryText = true;

				if(strInfo.Substring(strInfo.IndexOf("%")+1,strInfo.Length-(strInfo.IndexOf("%")+1)).Contains("T"))
					TmpObj.GetComponent<TextFloat_Pos>().m_bFatherText = true;
			}

			break;

		case OBJECT_ID.TOP_DECO:
			TmpObj = Instantiate(objPool.m_TopDeco) as GameObject;
			
			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("TopDecos").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("TopDecos").gameObject.transform;

			TmpObj.GetComponent<BoxDeco>().m_iTopDecoIdx = System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1,strInfo.Length-(strInfo.IndexOf("$")+1)));

			break;

		case OBJECT_ID.INTERACTION_PROP:
			TmpObj = Instantiate(objPool.m_InteractionProp) as GameObject;
			
			if(StageParent == null)
				TmpObj.transform.parent = GameObject.Find ("InteractionProps").gameObject.transform;
			else
				TmpObj.transform.parent = StageParent.transform.Find("InteractionProps").gameObject.transform;
			
			TmpObj.GetComponent<InteractionProp>().m_iPropIdx = System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("$")+1,strInfo.Length-(strInfo.IndexOf("$")+1)));
			
			break;
			
		default:
			Debug.LogError("ObjId is null");
			break;
		}

		if (StageParent != null)
			TmpObj.transform.position = ObjectPos + (Vector2)StageParent.transform.position;
		else
			TmpObj.transform.position = ObjectPos;

		if (bMaptool) {

			TmpObj.transform.position -= new Vector3 ((GridMgr.getInstance.m_iXcount / 2) * 0.5f, 0);  // 오브젝트만들면서 맵툴 최좌단에 정렬하여 맞춰지도록 위치조정

			if (objID != OBJECT_ID.G_DOOR && objID != OBJECT_ID.R_DOOR)
				tmpGridIdx.Add (GridMgr.getInstance.GetGridIdx (TmpObj.transform.position));
			else
				tmpGridIdx = GameObject.Find ("Grids").GetComponent<GridMgr> ().GetDoor_AllIdxs (TmpObj.transform.position, (int)TmpObj.transform.GetChild (0).localScale.y);

			IndexTag tmpTag = new IndexTag (tmpGridIdx, TmpObj, objID);
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_listObjects.Add (tmpTag);
		} else {
			if(objID.Equals(OBJECT_ID.RUNNER))
				TmpObj.transform.position -= new Vector3 (0, 0.06f);
		}

	}

	void PortalAssignOwner(string strInfo ,int iIdx, GameObject StageParent)
	{
		if(StageParent == null)
			GameObject.Find("Portals").transform.GetChild(iIdx).GetComponent<Portal>().m_objOwner =  GameObject.Find("Portals").transform.GetChild(System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("#")+1, strInfo.Length -(strInfo.IndexOf("#")+1)))).gameObject;
		else
			StageParent.transform.Find("Portals").GetChild(iIdx).GetComponent<Portal>().m_objOwner = StageParent.transform.Find("Portals").GetChild(System.Convert.ToInt32(strInfo.Substring(strInfo.IndexOf("#")+1, strInfo.Length -(strInfo.IndexOf("#")+1)))).gameObject;
	}

	Color StringToColor(string str)
	{
		string r;
		string g;
		string b;

		int idx = str.IndexOf("(") + 1;
		r = str.Substring (idx, str.IndexOf (",", idx) - idx);
		idx = str.IndexOf (",", idx) + 1;
		g = str.Substring (idx, str.IndexOf (",", idx) - idx);
		idx = str.IndexOf (",", idx) + 1;
		b =  str.Substring (idx, str.IndexOf (",", idx) - idx);

		return new Color (System.Convert.ToSingle(r), System.Convert.ToSingle(g), System.Convert.ToSingle(b));
	}

	Vector2 StringConverter(string strInfo)
	{
		int startChar = 0;
		int endChar = strInfo.IndexOf('/');
		int lastEnd;
		
		string returnstring = strInfo.Substring(startChar, endChar);
		
		startChar = 1;
		lastEnd = endChar;
		float returnx = System.Convert.ToSingle(returnstring);
		
		startChar = lastEnd+1;
		if (strInfo.IndexOf ('$') != -1)
			endChar = strInfo.IndexOf ('$');
		else
			endChar = strInfo.Length;
		lastEnd = endChar;
		returnstring = strInfo.Substring(startChar, endChar-startChar);
		float returny = System.Convert.ToSingle(returnstring);
		
		Vector2 returnVector2 = new Vector2(returnx,returny);
		
		return returnVector2;
	}
}
