using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MapToolMgr : MonoBehaviour
{

	private static MapToolMgr instance;
	
	public static MapToolMgr getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(MapToolMgr)) as MapToolMgr;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("MapToolMgr");
				instance = obj.AddComponent (typeof(MapToolMgr)) as MapToolMgr;
			}
			
			return instance;
		}
	}
	
	void Awake(){
		if (instance == null)
			instance = this;
		
		else if (instance != this)
			Destroy(gameObject);
		
//		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
	}
	
	void OnDestroy()
	{
		StopAllCoroutines ();
		instance = null;
		
	}


	public GameObject m_CurPref;
	public GameObject m_CurSelObjUI;
	public GameObject m_objCurDoor;
	public List<GameObject> m_objCurBox;
	public bool m_bUIMouseOn;
	public bool m_bSwitchUIOn;
	public bool m_bOverayUIOn;
	public Stage m_Stage;
	public List<IndexTag> m_listObjects;
	public bool m_bNowPlaying;
	bool m_bRunnerOn;
	bool m_bGuardianOn;
	bool m_bGoalOn;
	bool m_bPhotoOn;
	bool m_bESCOn;



	public bool m_bThisIsKey; //이 변수로 스위치만들때 스위치인지 키인지 결정함

	public List<int> SelectedIdxes = new List<int> (); //drag selected index
	int m_iDragCenterBox; // when drag, center index of box 

	/// <summary>
	/// //////////Play Variables..
	/// </summary>
	Vector2 m_vecRunnerPos;
	Vector2 m_vecGuardianPos;
	Vector3 m_vecCamPos;
	float m_fCamSize;
	List<Vector3> m_listDoorPos;

	void Start ()
	{
		if (m_listObjects.Count == 0)
			m_listObjects = new List<IndexTag> ();
		m_listDoorPos = new List<Vector3> ();

		m_objCurBox = new List<GameObject> ();

		if (GameObject.Find ("MapListMgr") != null) {
			if(GameObject.Find ("MapListMgr").GetComponent<MapListMgr>().m_strCurMap != "Create New")
				GameObject.Find ("StageLoader").GetComponent<StageLoader> ().LoadStage (GameObject.Find ("MapListMgr").GetComponent<MapListMgr>().m_strCurMap + GameObject.Find ("MapListMgr").GetComponent<MapListMgr>().m_strCurExtension);
		}

		if (GameObject.Find ("MapListMgr") != null) {
			if (GameObject.Find ("MapListMgr").GetComponent<MapListMgr> ().m_strCurMap != "Create New")
				GameObject.Find ("MapName").GetComponent<UIInput> ().value = GameObject.Find ("MapListMgr").GetComponent<MapListMgr> ().m_strCurMap;
		}

		GameObject.Find ("PlayerUIs").transform.GetChild (0).GetComponent<DrawBtn> ().OnClick ();

		OnCheck ();
		//StartCoroutine (DragDetector ());
	}

	public void OnCheck ()
	{
		for (int i = 0; i < m_listObjects.Count; ++i) {
			if (m_listObjects [i].m_Object.gameObject.name == "Runner(Clone)")
				m_bRunnerOn = true;
			else if (m_listObjects [i].m_Object.gameObject.name == "Guardian(Clone)")
				m_bGuardianOn = true;
			else if (m_listObjects [i].m_Object.gameObject.name == "R_Goal(Clone)")
				m_bGoalOn = true;
			else if (m_listObjects [i].m_Object.gameObject.name == "Photo(Clone)")
				m_bPhotoOn = true;
		}
	}

	void Update ()
	{
		if (!m_bNowPlaying && GameObject.Find ("GridLimit").GetComponent<BoxCollider2D> ().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
			if (UICamera.selectedObject != null && UICamera.selectedObject.name.Contains("UI Root")) {
				if (m_CurPref != null && (m_CurPref.tag == "Door" || m_CurPref.gameObject.name == "MoveOrder" || m_CurPref.gameObject.name == "R_Switch" || m_CurPref.gameObject.name == "G_Switch" 
					|| m_CurPref.gameObject.name == "Spring" || m_CurPref.gameObject.name == "Timeball" || m_CurPref.gameObject.name == "Portal" || m_CurPref.gameObject.name == "Portal_Exit" || m_CurPref.gameObject.name == "TextFloat_Pos")//클릭하는 순간에만 만들어지는 애들
				    && (GameObject.Find("MiniButtons").transform.childCount == 0 || (GameObject.Find("MiniButtons").transform.childCount == 1 && GameObject.Find("MiniButtons").transform.GetChild(0).name == "ConfirmBtn(Clone)"))) {  //확인버튼 제외한 다른버튼이있을땐 한번 일부러 생성 씹히게 ( 버튼 꺼지도록 ) 예외처리
					if (Input.GetMouseButtonDown (0))
					{
						Draw ();
					}
				} else if (Input.GetMouseButton (0) && GridMgr.getInstance.ValidIdxCheck () && GameObject.Find("DragBorder(Clone)") == null && GameObject.Find("MiniButtons").transform.childCount == 0) { //마우스 누르고 있는동안 만들어/지워 지는 애들
					if (m_CurPref != null) { // Draw
						Draw ();
					} else { // Eraser
						Eraser ();
					}
				}
			}

			ShortCut ();
		}
	}

	IEnumerator DragDetector(){

		Vector3 FirstMouseBtnDownPos = Vector3.zero; // 최초 마우스위치

		bool bDragOn = false; // drag value

		int iStartIndex = -1;
		GameObject prefDragBorder = Resources.Load("Prefabs/UI/DragBorder") as GameObject;
		GameObject objDragBorder = null;

		GridMgr gridMgr = GridMgr.getInstance;

		GameObject objStartPosBox = null; // 드래그 시작되는곳에 위치한 박스

		while(true)
		{
			if (Input.GetMouseButtonDown(0)) // 최초 마우스 누를 당시 위치 체크
			{
				FirstMouseBtnDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				iStartIndex = gridMgr.m_iGridIdx;
			}

			if(GameObject.Find("BoxBorder") != null && m_CurPref != null && (m_CurPref.name == "Box" || m_CurPref.name == "HoldingBox" || m_CurPref.name == "R_Box" || m_CurPref.name == "G_Box" ))
			{
				if(Input.GetMouseButton(0)) // while mouse btn down
				{
					if(!bDragOn && Vector3.Distance(FirstMouseBtnDownPos, Camera.main.ScreenToWorldPoint(Input.mousePosition)) > 0.05f) // 마우스 끌기시작
					{
						bDragOn = true;
						objStartPosBox = GameObject.Find("BoxBorder").transform.parent.gameObject;

						objDragBorder = Instantiate(prefDragBorder) as GameObject;
						objDragBorder.transform.parent = GameObject.Find("DragBorders").transform;
						objDragBorder.transform.localScale = Vector2.one;
						objDragBorder.GetComponent<DragBorder>().iStartIdx = iStartIndex;
						objDragBorder.GetComponent<UIFollowTarget>().target = objStartPosBox.transform;
						objDragBorder.GetComponent<UIFollowTarget>().enabled = true;

						StartCoroutine(BoxUISetter(false, objStartPosBox));
					}

					if(bDragOn)					// ON DRAG!!
					{

					}

				}

				if(Input.GetMouseButtonUp(0)) // Button Up
				{
					bDragOn = false;

					if(GameObject.Find("DragBorder(Clone)") != null){
						SelectedIdxes = objDragBorder.GetComponent<DragBorder>().SelectedIdx(); //현재 선택된 박스 인덱스 리스트 받아오기

						SelectedIdxes.Sort(); // sorting
						List<GameObject> SelectedBoxList = new List<GameObject>();

						for(int i = 0; i < SelectedIdxes.Count; ++i) // 현재 선택된 박스 border on
						{
							GameObject tmpObj = FindObjInListObj(SelectedIdxes[i]);

							if(tmpObj != null && tmpObj.CompareTag("Box"))
							{
								tmpObj.transform.GetChild(0).gameObject.SetActive(true);
								tmpObj.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
								SelectedBoxList.Add(tmpObj);
							}
						}

						if(SelectedIdxes.Count > 1)
						{
							if(GameObject.Find("MiniButtons").transform.childCount > 0)
								Destroy(GameObject.Find("MiniButtons").transform.GetChild(0).gameObject);

							m_iDragCenterBox =  GridMgr.getInstance.GetGridIdx(SelectedBoxList[SelectedBoxList.Count/2].transform.position);

							StartCoroutine(MakeMiniBtn("DragMoveBtn", SelectedBoxList[SelectedBoxList.Count/2]));
						}else{
							StartCoroutine(BoxUISetter(true, objStartPosBox));
						}
					}


					Destroy(objDragBorder);
				}
			}

			yield return null;
		}
		
	}

	IEnumerator BoxUISetter(bool bMsg, GameObject objStartPosBox )
	{
		yield return new WaitForEndOfFrame ();

		objStartPosBox.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = bMsg; // 잠깐 BoxBorder스프라이트 렌더러 
		if(GameObject.Find("MiniButtons").transform.childCount > 0)
			GameObject.Find("MiniButtons").transform.GetChild(0).gameObject.SetActive(bMsg); //기존 박스에 표시된 무브 미니버튼 
	}


	//Iterate ListObj and returnn its idx//
	int FindObjIdxInListObj(GameObject obj)
	{
		for (int i = 0; i < m_listObjects.Count; ++i) {
			if (m_listObjects [i].FindObject (obj))
				return i;
		}

		return -1;
	}

	//Iterate ListObj and returnn its obj//
	GameObject FindObjInListObj(int iIdx)
	{
		for (int i = 0; i < m_listObjects.Count; ++i) {
			if (m_listObjects [i].m_iIdx[0] == iIdx)
				return m_listObjects [i].m_Object;
		}
		
		return null;
	}

	void Eraser()
	{
		for (int i = 0; i < m_listObjects.Count; ++i) {
			for (int j = 0; j < m_listObjects [i].m_iIdx.Count; ++j)
			{
				if (m_listObjects [i].m_iIdx [j] == GridMgr.getInstance.m_iGridIdx) {
					if (m_listObjects [i].m_Object.gameObject.name == "Runner(Clone)")
						m_bRunnerOn = false;
					else if (m_listObjects [i].m_Object.gameObject.name == "Guardian(Clone)")
						m_bGuardianOn = false;	
					else if (m_listObjects [i].m_Object.gameObject.name == "R_Goal(Clone)")
						m_bGoalOn = false;
					else if (m_listObjects [i].m_Object.gameObject.name == "Photo(Clone)")
						m_bPhotoOn = false;
					else if (m_listObjects [i].m_Object.gameObject.name == "Floor(Clone)")
					{
						for (int k = 0; k < GameObject.Find("MoveOrders").transform.childCount; ++k) {// If Erase Floor, Moveorder also erase
							if (GameObject.Find ("MoveOrders").transform.GetChild (k).GetComponent<MoveOrder> ().m_objOwner == m_listObjects [i].m_Object.transform.parent.gameObject) {
								
								m_listObjects.RemoveAt (FindObjIdxInListObj(GameObject.Find ("MoveOrders").transform.GetChild (k).gameObject));
								
								Destroy (GameObject.Find ("MoveOrders").transform.GetChild (k).gameObject);
							}
						}

						m_listObjects.RemoveAt (FindObjIdxInListObj(m_listObjects [i].m_Object.transform.parent.gameObject));
						Destroy(m_listObjects [i].m_Object.transform.parent.gameObject);
						
					} else if (m_listObjects [i].m_Object.gameObject.name == "Box(Clone)" || m_listObjects [i].m_Object.gameObject.name == "HoldingBox(Clone)" 
					           || m_listObjects [i].m_Object.gameObject.name == "R_Box(Clone)" || m_listObjects [i].m_Object.gameObject.name == "G_Box(Clone)"
					           || m_listObjects [i].m_Object.gameObject.name == "Spike(Clone)") { 						// If Erase Box, Moveorder also erase
						for (int k = 0; k < GameObject.Find("MoveOrders").transform.childCount; ++k) {
							if (GameObject.Find ("MoveOrders").transform.GetChild (k).GetComponent<MoveOrder> ().m_objOwner == m_listObjects [i].m_Object.gameObject) {
								
								m_listObjects.RemoveAt (FindObjIdxInListObj(GameObject.Find ("MoveOrders").transform.GetChild (k).gameObject));
								
								Destroy (GameObject.Find ("MoveOrders").transform.GetChild (k).gameObject);
							}
						}
					} else if (m_listObjects [i].m_Object.gameObject.name == "R_Door(Clone)" || m_listObjects [i].m_Object.gameObject.name == "G_Door(Clone)") { // If Erase Door, Switch also erase
						for (int k = 0; k < GameObject.Find("Switchs").transform.childCount; ++k) {
							if (GameObject.Find ("Switchs").transform.GetChild (k).GetComponent<Switch> ().m_objDoor == m_listObjects [i].m_Object.gameObject.transform.GetChild (0).gameObject) {
								
								m_listObjects.RemoveAt (FindObjIdxInListObj(GameObject.Find ("Switchs").transform.GetChild (k).gameObject));
								
								Destroy (GameObject.Find ("Switchs").transform.GetChild (k).gameObject);
							}
						}
					}else if(m_listObjects [i].m_Object.gameObject.name == "Portal(Clone)"){
						if(m_listObjects [i].m_Object.GetComponent<Portal>().m_objOwner != null)
						{
//							m_listObjects [i].m_Object.GetComponent<Portal>().m_objOwner.GetComponent<SpriteRenderer>().color = Color.white;
							m_listObjects [i].m_Object.GetComponent<Portal>().m_objOwner.GetComponent<Portal>().m_objOwner = null;
						}
					}

					
					Destroy (m_listObjects [i].m_Object.gameObject);
					m_listObjects.RemoveAt (i);
					
					break;
				}
			}
		}
	}

	void Draw ()
	{


		if (m_CurPref.gameObject.name == "Runner") {
			if (m_bRunnerOn)
				return;
		} else if (m_CurPref.gameObject.name == "Guardian") {
			if (m_bGuardianOn)
				return;
		} else if (m_CurPref.gameObject.name == "R_Goal") {
			if (m_bGoalOn)
				return;
		} else if (m_CurPref.gameObject.name == "Photo") {
			if (m_bPhotoOn)
				return;
		} else if (m_CurPref.gameObject.tag == "Switch") {
			if (GameObject.Find ("DoorBorder") == null)
				return;
		} else if (m_CurPref.gameObject.name == "MoveOrder") {
			if (GameObject.Find ("BoxBorder") == null)
				return;
		}

		if (!SeatTakenCheck (GridMgr.getInstance.m_iGridIdx) || m_CurPref.gameObject.name == "MoveOrder" ||  m_CurPref.gameObject.name == "TextFloat_Pos") {

			if (m_CurPref.gameObject.name == "MoveOrder") { // if draw new moveline, delete before one.
				DeleteBeforeSameOne("MoveOrders");
			}if (m_CurPref.gameObject.name == "R_Switch" || m_CurPref.gameObject.name == "G_Switch") { // if draw new Switch, delete before one.
				DeleteBeforeSameOne("Switchs");
			}if (m_CurPref.gameObject.name == "Portal_Exit") { // if draw new Portal, delete before one.
				DeleteBeforeSameOne("Portals");
			}



			int iLoopCount = 1;
			if(m_objCurBox.Count > 0 && m_CurPref.gameObject.name == "MoveOrder")
				iLoopCount = m_objCurBox.Count;


			for(int i = 0 ; i < iLoopCount; ++i)
			{
				GameObject tmpObj = Instantiate (m_CurPref) as GameObject;
				tmpObj.transform.position = GridMgr.getInstance.IdxPos ();

				OBJECT_ID tmpID = OBJECT_ID.END;


				List<int> tmpIdxList = new List<int> ();

				switch (m_CurPref.gameObject.name) {
				case "Runner":
					m_bRunnerOn = true;
					tmpID = OBJECT_ID.RUNNER;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Players").transform;
					tmpObj.GetComponent<MeshRenderer>().sortingLayerName = "Object";
					break;
				case "Guardian":
					m_bGuardianOn = true;
					tmpID = OBJECT_ID.GUARDIAN;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Players").transform;
					tmpObj.GetComponent<MeshRenderer>().sortingLayerName = "Object";
					break;
				case "R_Goal":
					tmpID = OBJECT_ID.GOAL;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					m_bGoalOn = true;
					tmpObj.transform.parent = GameObject.Find ("Goal").transform;
					break;

				case "Photo":
					tmpID = OBJECT_ID.PHOTO;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					m_bPhotoOn = true;
					tmpObj.transform.parent = GameObject.Find ("Goal").transform;
					break;

				case "Box":
					tmpID = OBJECT_ID.BOX;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Boxes").transform;
					break;

				case "Spike":
					tmpID = OBJECT_ID.SPIKE;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Spikes").transform;
					break;

				case "HoldingBox":
					tmpID = OBJECT_ID.HOLDINGBOX;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Boxes").transform;
					break;
				case "R_Box":
					tmpID = OBJECT_ID.R_BOX;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Boxes").transform;
					break;
				case "G_Box":
					tmpID = OBJECT_ID.G_BOX;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Boxes").transform;
					break;
				case "G_Door":
					tmpID = OBJECT_ID.G_DOOR;
					tmpIdxList = GridMgr.getInstance.GetAll_VacantUpperIdx(GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.GetChild (0).GetComponent<DoorPosFixer> ().m_fSize = (tmpIdxList.Count * 1.0f);
					tmpObj.transform.GetChild (0).GetComponent<DoorPosFixer> ().PosFixer ();
					tmpObj.transform.position = GridMgr.getInstance.IdxPos (tmpIdxList [0]);
					tmpObj.transform.parent = GameObject.Find ("Doors").transform;
					break;
				case "R_Door":
					tmpID = OBJECT_ID.R_DOOR;
					tmpIdxList = GridMgr.getInstance.GetAll_VacantUpperIdx(GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.GetChild (0).GetComponent<DoorPosFixer> ().m_fSize = (tmpIdxList.Count * 1.0f);
					tmpObj.transform.GetChild (0).GetComponent<DoorPosFixer> ().PosFixer ();
					tmpObj.transform.position = GridMgr.getInstance.IdxPos (tmpIdxList [0]);
					tmpObj.transform.parent = GameObject.Find ("Doors").gameObject.transform;
					break;
				case "G_Switch":
					tmpID = OBJECT_ID.G_SWITCH;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.GetComponent<Switch> ().m_objDoor = m_objCurDoor;
					tmpObj.transform.parent = GameObject.Find ("Switchs").transform;
					m_bSwitchUIOn = false;
					
					tmpObj.transform.GetChild(0).gameObject.SetActive(true);

					if(m_bThisIsKey)
						tmpObj.GetComponent<Switch>().m_bThisIsHoldDoor = false;
					else
						tmpObj.GetComponent<Switch>().m_bThisIsHoldDoor = true;
					
					StartCoroutine(MakeMiniBtn("ConfirmBtn", tmpObj));
					tmpObj.GetComponent<DestroyWhenCurprefChged>().enabled = true;
					break;
				case "R_Switch":
					tmpID = OBJECT_ID.R_SWITCH;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.GetComponent<Switch> ().m_objDoor = m_objCurDoor;
					tmpObj.transform.parent = GameObject.Find ("Switchs").transform;
					m_bSwitchUIOn = false;

					tmpObj.transform.GetChild(0).gameObject.SetActive(true);

					if(m_bThisIsKey)
						tmpObj.GetComponent<Switch>().m_bThisIsHoldDoor = false;
					else
						tmpObj.GetComponent<Switch>().m_bThisIsHoldDoor = true;

					StartCoroutine(MakeMiniBtn("ConfirmBtn", tmpObj));
					tmpObj.GetComponent<DestroyWhenCurprefChged>().enabled = true;
					break;


				case "MoveOrder":
					tmpID = OBJECT_ID.MOVEORDER;

					int iCurIdx = GridMgr.getInstance.GetGridIdx( Camera.main.ScreenToWorldPoint (Input.mousePosition)) + GridMgr.getInstance.GetGridIdx(m_objCurBox[i].transform.position) - m_iDragCenterBox;
					tmpIdxList.Add (iCurIdx);
					tmpObj.GetComponent<MoveOrder> ().m_objOwner = m_objCurBox[i];
					tmpObj.transform.parent = GameObject.Find ("MoveOrders").transform;

					GameObject.Find("MoveBox").transform.GetChild(0).GetComponent<DrawBtn>().OnClick();
					tmpObj.GetComponent<DestroyWhenCurprefChged>().enabled = true;

					if(m_objCurBox.Count == 1)
						StartCoroutine(MakeMiniBtn("ConfirmBtn", tmpObj));
					else
					{
						tmpObj.transform.position = GridMgr.getInstance.IdxPos (iCurIdx);

						if(i == m_objCurBox.Count/2)
							StartCoroutine(MakeMiniBtn("ConfirmBtn", tmpObj));
					}
					break;

				case "TimeCapsule":
					tmpID = OBJECT_ID.Timeball;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Timeballs").transform;
					break;

				case "Spring":
					tmpID = OBJECT_ID.SPRING;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Springs").transform;
					break;

				case "Portal":
					tmpID = OBJECT_ID.PORTAL;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Portals").transform;
					break;

				case "TextFloat_Pos":
					tmpID = OBJECT_ID.TEXTFLOAT;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("TextFloatPos").transform;
					break;

				case "Portal_Exit":
					tmpID = OBJECT_ID.PORTAL;
					tmpObj.GetComponent<Portal> ().m_objOwner = m_objCurBox[i];
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("Portals").transform;
					tmpObj.GetComponent<DestroyWhenCurprefChged>().enabled = true;
					tmpObj.transform.GetChild(0).GetComponent<DestroyWhenCurprefChged>().enabled = true;

					tmpObj.GetComponent<SkeletonAnimation>().initialSkinName = tmpObj.GetComponent<Portal> ().m_objOwner.GetComponent<SkeletonAnimation>().initialSkinName;
					tmpObj.GetComponent<SkeletonAnimation>().Reset();
					StartCoroutine(MakeMiniBtn("ConfirmBtn", tmpObj));
					break;

				case "TopDeco":
					tmpID = OBJECT_ID.TOP_DECO;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("TopDecos").transform;
					tmpObj.GetComponent<BoxDeco>().m_iTopDecoIdx = GameObject.Find("TopDecoUIs").GetComponent<UI_Obj>().m_iObjNum;
					break;

				case "InteractionProp":
					tmpID = OBJECT_ID.INTERACTION_PROP;
					tmpIdxList.Add (GridMgr.getInstance.m_iGridIdx);
					tmpObj.transform.parent = GameObject.Find ("InteractionProps").transform;
					tmpObj.GetComponent<InteractionProp>().m_iPropIdx = GameObject.Find("InteraActionUIs").GetComponent<UI_Obj>().m_iObjNum;
					break;
					
				default:
					Debug.LogError ("ObjectId is null");
					break;
				}

				m_listObjects.Add (new IndexTag (tmpIdxList, tmpObj, tmpID));
			}
		}


		if(GameObject.Find("BoxBorder") == null)
			m_objCurBox.Clear();
	}


	void MoveElements(bool bLeft)
	{
		Vector3 MousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Transform stageTrans = GameObject.Find ("Stage(Clone)").transform;

		int iTopIdx = GridMgr.getInstance.m_iGridIdx;

		while (iTopIdx >= GridMgr.getInstance.m_iXcount) {
			iTopIdx -= GridMgr.getInstance.m_iXcount;
		}

		if (bLeft && (GridMgr.getInstance.GetAll_VacantLowerIdx (iTopIdx).Count != GridMgr.getInstance.m_iYcount - 1 || FindObjInListObj(iTopIdx) != null))
			return;

		for (int i=0; i < stageTrans.childCount; ++i) {
			for(int j=0; j < stageTrans.GetChild(i).childCount; ++j)
			{

				if(bLeft)
				{
					if(stageTrans.GetChild(i).GetChild(j).transform.position.x > MousePos.x - 0.25f)
					{
						stageTrans.GetChild(i).GetChild(j).transform.position = new Vector2(stageTrans.GetChild(i).GetChild(j).transform.position.x - 0.5f, stageTrans.GetChild(i).GetChild(j).transform.position.y);
						AddIndex(stageTrans.GetChild(i).GetChild(j).gameObject, -1);
					}
			   	}else{
					if(stageTrans.GetChild(i).GetChild(j).transform.position.x > MousePos.x - 0.25f)
					{
						stageTrans.GetChild(i).GetChild(j).transform.position = new Vector2(stageTrans.GetChild(i).GetChild(j).transform.position.x + 0.5f, stageTrans.GetChild(i).GetChild(j).transform.position.y);
						AddIndex(stageTrans.GetChild(i).GetChild(j).gameObject, 1);
					}
				}
			}
		}
	}


	void AddIndex(GameObject obj, int iOperand)
	{
		bool bFloors = false;
		if (obj.name.Equals ("Box(Clone)"))
			bFloors = true;

		for (int i =0; i < m_listObjects.Count; ++i) {
			if(obj == m_listObjects[i].m_Object)
			{
				m_listObjects[i].m_iIdx[0] += iOperand;
			}

			if(bFloors)
			{
				for(int j= 0 ; j < obj.transform.childCount; ++j)
				{
					if(obj.transform.GetChild(j).gameObject == m_listObjects[i].m_Object)
					{
						m_listObjects[i].m_iIdx[0] += iOperand;
					}
				}
			}
		}
	}
				



	void DeleteBeforeSameOne(string strParentName)
	{
		if (strParentName == "MoveOrders") {
			for (int i = 0; i < GameObject.Find(strParentName).transform.childCount; ++i) {
				for(int k = 0; k < m_objCurBox.Count; ++ k)
				{
					if (GameObject.Find (strParentName).transform.GetChild (i).GetComponent<MoveOrder> ().m_objOwner == m_objCurBox[k]) {
						for (int j = 0; j < m_listObjects.Count; ++j) {
							if (m_listObjects [j].FindObject (GameObject.Find (strParentName).transform.GetChild (i).gameObject))
								m_listObjects.RemoveAt (j);
						}
					
						Destroy (GameObject.Find (strParentName).transform.GetChild (i).gameObject);
					}
				}
			}
		}else if(strParentName == "Switchs")
		{
			for (int i = 0; i < GameObject.Find(strParentName).transform.childCount; ++i) {
				if (GameObject.Find (strParentName).transform.GetChild (i).GetComponent<Switch> ().m_objDoor == m_objCurDoor) {
					for (int j = 0; j < m_listObjects.Count; ++j) {
						if (m_listObjects [j].FindObject (GameObject.Find (strParentName).transform.GetChild (i).gameObject))
							m_listObjects.RemoveAt (j);
					}
					
					Destroy (GameObject.Find (strParentName).transform.GetChild (i).gameObject);
				}
			}
		}
		else if(strParentName == "Portals")
		{
			for (int i = 0; i < GameObject.Find(strParentName).transform.childCount; ++i) {
				for(int k = 0; k < m_objCurBox.Count; ++ k)
				{
					if (GameObject.Find (strParentName).transform.GetChild (i).GetComponent<Portal> ().m_objOwner == m_objCurBox[k]) {
						for (int j = 0; j < m_listObjects.Count; ++j) {
							if (m_listObjects [j].FindObject (GameObject.Find (strParentName).transform.GetChild (i).gameObject))
								m_listObjects.RemoveAt (j);
						}
//						GameObject.Find (strParentName).transform.GetChild (i).GetComponent<Portal> ().m_objOwner.GetComponent<SpriteRenderer>().color = Color.white; // portal color to white
						Destroy (GameObject.Find (strParentName).transform.GetChild (i).gameObject);
					}
				}
			}
		}
	}

	void ShortCut()
	{
		if (Input.GetKey (KeyCode.Alpha1)) {
			GameObject.Find("PlayerUIs").transform.Find("BtnSprite").GetComponent<DrawBtn>().OnClick();
		}else if (Input.GetKey (KeyCode.Alpha2)) {
			GameObject.Find("BoxUIs").transform.Find("BtnSprite").GetComponent<DrawBtn>().OnClick();
		}else if (Input.GetKey (KeyCode.Alpha3)) {
			GameObject.Find("DoorUIs").transform.Find("BtnSprite").GetComponent<DrawBtn>().OnClick();
		}else if (Input.GetKey (KeyCode.Alpha4)) {
			GameObject.Find("Eraser_UI").transform.Find("Selector").GetComponent<DrawBtn>().OnClick();
		}else if(Input.GetKeyDown (KeyCode.Q)){
			MoveElements(true);
		}else if(Input.GetKeyDown (KeyCode.E)){
			MoveElements(false);
		}
	}

	public bool SeatTakenCheck (int iIdx)
	{
		bool bArleadyGridTaken = false;
		for (int i = 0; i < m_listObjects.Count; ++i) {
			if (m_listObjects [i].m_iIdx.Count > 0) {
				for (int j = 0; j < m_listObjects [i].m_iIdx.Count; ++j) {
					if (m_listObjects [i].m_iIdx [j] == iIdx) {
						if(m_listObjects[i].m_objID != OBJECT_ID.MOVEORDER && m_listObjects[i].m_objID != OBJECT_ID.TEXTFLOAT)
							bArleadyGridTaken = true;
					}
				}
			}
		}

		return bArleadyGridTaken;
	}

	public void Play (bool bPlay)
	{
		if (bPlay) {
			SceneStatus.getInstance.m_fStageXPos.Add(GridMgr.getInstance.GetLeftiestOfThisMap() + (GridMgr.getInstance.GetWidthOfIndex_ThisMap() * 0.5f));
			SceneStatus.getInstance.m_fStageXPosLeftest.Add(GridMgr.getInstance.GetLeftiestOfThisMap());

			m_bNowPlaying = true;
			//GameObject groundChker;

			GameObject.Find ("Runner(Clone)").AddComponent<Runner> ();
			GameObject.Find ("Guardian(Clone)").AddComponent<Guardian> ();

			GameObject.Find ("Runner(Clone)").GetComponent<Rigidbody2D> ().gravityScale = 1.5f;
			GameObject.Find ("Guardian(Clone)").GetComponent<Rigidbody2D> ().gravityScale = 2.0f;

			GameObject.Find ("Main Camera").GetComponent<MapToolCam> ().enabled = false;
			GameObject.Find ("Main Camera").GetComponent<CamMoveMgr> ().enabled = true;
			m_vecCamPos = GameObject.Find ("Main Camera").transform.position;
			GameObject.Find ("Main Camera").transform.position = GameObject.Find ("Runner(Clone)").transform.position + new Vector3 (0.2f, GameObject.Find ("Main Camera").GetComponent<CamMoveMgr>().m_fRunnerYPosFixer, -10f);
			m_fCamSize = GameObject.Find ("Main Camera").GetComponent<Camera> ().orthographicSize;
			GameObject.Find ("Main Camera").GetComponent<Camera> ().orthographicSize = 2.5f; //portrait view = 4
			GameObject.Find ("Main Camera").transform.position = GameObject.Find ("Main Camera").GetComponent<CamMoveMgr>().CamRestriction();

			GameObject.Find ("MapToolUI").transform.GetChild (0).gameObject.SetActive (false);
			GameObject.Find ("Debug").transform.GetChild (0).gameObject.SetActive (false);

			m_vecRunnerPos = GameObject.Find ("Runner(Clone)").transform.position;
			m_vecGuardianPos = GameObject.Find ("Guardian(Clone)").transform.position;

			m_listDoorPos.Clear ();
			for (int i = 0; i < GameObject.Find("Doors").transform.childCount; ++i) {
				if (GameObject.Find ("Doors").transform.GetChild (i).name == "R_Door(Clone)")
					GameObject.Find ("Doors").transform.GetChild (i).GetChild (0).gameObject.AddComponent<R_Door> ();
				else
					GameObject.Find ("Doors").transform.GetChild (i).GetChild (0).gameObject.AddComponent<G_Door> ();


				m_listDoorPos.Add (GameObject.Find ("Doors").transform.GetChild (i).GetChild (0).transform.position);
			}

			for (int i = 0; i < GameObject.Find("MoveOrders").transform.childCount; ++i) {
				GameObject.Find ("MoveOrders").transform.GetChild (i).GetComponent<MoveOrder> ().m_bPlay = true;
				GameObject.Find ("MoveOrders").transform.GetChild (i).GetComponent<SpriteRenderer> ().enabled = false;
				GameObject.Find ("MoveOrders").transform.GetChild (i).transform.GetChild (0).gameObject.SetActive (false);
				GameObject.Find ("MoveOrders").transform.GetChild (i).GetComponent<MoveOrder> ().Start ();
			}

			ColliderMgr.getInstance.CheckAllBoxes();


			for (int i = 0; i < GameObject.Find("Boxes").transform.childCount; ++i) {
				Transform BoxParent = GameObject.Find("Boxes").transform;

				BoxParent.GetChild(i).GetComponent<Background>().SetFloorSprite();
			}

			UICamera.selectedObject = GameObject.FindGameObjectWithTag ("UI Root");

			GameObject.Find("GuardianCamContainer").transform.GetChild(0).gameObject.SetActive(false);

			UIManager.getInstance.SallyWait();


		} else {
			//GameObject.Find("PauseScreen").transform.GetChild(0).gameObject.SetActive(true);
			m_bNowPlaying = false;
			if (GameObject.Find ("Guardian(Clone)").GetComponent<Guardian> () != null)
			{
				GameObject.Find ("Guardian(Clone)").GetComponent<Guardian> ().Reset ();
				GameObject.Find ("Guardian(Clone)").GetComponent<Rigidbody2D>().isKinematic = true;
			}

			GameObject.Find ("Runner(Clone)").GetComponent<Runner> ().SallyPathCreate (false);
			GameObject.Find ("Runner(Clone)").GetComponent<Runner> ().Reset();

			Destroy (GameObject.Find ("Runner(Clone)").GetComponent<Runner> ());
			Destroy (GameObject.Find ("Guardian(Clone)").GetComponent<Guardian> ());

			GameObject.Find ("Runner(Clone)").GetComponent<Rigidbody2D> ().gravityScale = 0.0f;
			GameObject.Find ("Guardian(Clone)").GetComponent<Rigidbody2D> ().gravityScale = 0.0f;

			GameObject.Find ("Main Camera").GetComponent<MapToolCam> ().enabled = true;
			GameObject.Find ("Main Camera").GetComponent<CamMoveMgr> ().enabled = false;
			GameObject.Find ("Main Camera").transform.position = m_vecCamPos;

			GameObject.Find ("Main Camera").GetComponent<CamMoveMgr> ().ChgCamOrtho(m_fCamSize);
			//GameObject.Find ("Main Camera").GetComponent<Camera> ().orthographicSize = m_fCamSize;

			GameObject.Find ("MapToolUI").transform.GetChild (0).gameObject.SetActive (true);
			GameObject.Find ("Debug").transform.GetChild (0).gameObject.SetActive (true);

			GameObject.Find ("Runner(Clone)").transform.position = m_vecRunnerPos;
			GameObject.Find ("Guardian(Clone)").transform.position = m_vecGuardianPos;

			GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_enPlayerStatus = PLAYER_STATUS.SALLY_WAIT;
			GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ().m_iFlashBackIdx = 0;

			for (int i = 0; i < GameObject.Find("Doors").transform.childCount; ++i) {

				if (GameObject.Find ("Doors").transform.GetChild (i).name == "R_Door(Clone)")
					Destroy (GameObject.Find ("Doors").transform.GetChild (i).GetChild (0).gameObject.GetComponent<R_Door> ());
				else
					Destroy (GameObject.Find ("Doors").transform.GetChild (i).GetChild (0).gameObject.GetComponent<G_Door> ());

				GameObject.Find ("Doors").transform.GetChild (i).GetChild (0).transform.position = m_listDoorPos [i];
			}

			for (int i = 0; i < GameObject.Find("MoveOrders").transform.childCount; ++i) {
				GameObject.Find ("MoveOrders").transform.GetChild (i).GetComponent<MoveOrder> ().m_bPlay = false;
				GameObject.Find ("MoveOrders").transform.GetChild (i).GetComponent<SpriteRenderer> ().enabled = true;
				GameObject.Find ("MoveOrders").transform.GetChild (i).transform.GetChild (0).gameObject.SetActive (true);
			}

			for(int i = 0; i < GameObject.Find("Timeballs").transform.childCount; ++i) {
				GameObject.Find ("Timeballs").transform.GetChild (i).GetComponent<SpriteRenderer> ().enabled = true;
			}

			for(int i = 0; i < GameObject.Find("Spikes").transform.childCount; ++i) {
				GameObject.Find ("Spikes").transform.GetChild (i).GetComponent<Spike> ().StopAllCoroutines();
				GameObject.Find ("Spikes").transform.GetChild(i).GetComponent<Spike> ().SpikeToOriginPos();
				GameObject.Find ("Spikes").transform.GetChild(i).GetComponent<Spike>().m_list_iRevivalIdx.Clear();
			}

			for(int i = 0; i < GameObject.Find("Switchs").transform.childCount; ++i) {
				GameObject.Find ("Switchs").transform.GetChild(i).GetComponent<Switch>().m_bPressed = false;
				GameObject.Find ("Switchs").transform.GetChild(i).GetComponent<Switch>().InitSpine();
			}

			for(int i = 0; i < GameObject.Find("Springs").transform.childCount; ++i) {
				GameObject.Find ("Springs").transform.GetChild (i).GetComponent<Spring> ().Fix();
			}

			if(GameObject.Find ("Photo(Clone)") != null)
				GameObject.Find ("Photo(Clone)").GetComponent<SpriteRenderer>().enabled = true;

			GameObject.Find("Play").transform.GetChild(0).GetChild(1).GetComponent<UISprite>().spriteName = "icon_L_play";
			GameObject.Find("Play").transform.GetChild(0).GetChild(1).localPosition = new Vector3(0.6f, 0.6f);

//			GameObject.Find ("FatherWait").transform.GetChild (0).gameObject.SetActive (false);

			ColliderMgr.getInstance.DisableAllCollider();


			GameMgr.getInstance.ChangeBackgrounds (false);

			GameObject.Find("GuardianCamContainer").transform.GetChild(0).gameObject.SetActive(true);
		}
	}

	public void Save (string strFileName)
	{
		m_Stage = new Stage (m_listObjects);

		string strDir = Application.persistentDataPath + "/Stages";

		if (!Directory.Exists (strDir))
			Directory.CreateDirectory (strDir);

		FileStream fs;

		if (GameObject.Find ("MapListMgr") == null) {
			fs = new FileStream (@strDir + "/" + strFileName + ".stg", FileMode.Create, FileAccess.Write);
		} else {
			if(GameObject.Find ("MapListMgr").GetComponent<MapListMgr> ().m_SaveForm == SAVE_FORM.STG)
			{
				fs = new FileStream (@strDir + "/" + strFileName + ".stg", FileMode.Create, FileAccess.Write);
				Debug.Log (strDir + "/" +  strFileName + ".stg");
			}
			else
			{
				fs = new FileStream (@strDir + "/" + strFileName + ".txt", FileMode.Create, FileAccess.Write);
				Debug.Log (strDir + "/" + strFileName + ".txt");
			}
		}

		BinaryFormatter bf = new BinaryFormatter ();
		bf.Serialize (fs, m_Stage);
		fs.Close ();

		GameObject tmpErrMsg;
		tmpErrMsg = Instantiate (Resources.Load ("Prefabs/UI/mapToolErrorMsg")) as GameObject;
		tmpErrMsg.GetComponent<UILabel> ().text = "Save Colmplete!";
	}

	public string pathForDocumentsFile (string filename)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			string path = Application.dataPath;
			Debug.Log(path);
			return path + "/" + filename;
		} else if (Application.platform == RuntimePlatform.Android) {
			string path = Application.persistentDataPath;
			path = path.Substring (0, path.LastIndexOf ('/'));
			return Path.Combine (path, filename);
		} else {
			string path = Application.dataPath;
			path = path.Substring (0, path.LastIndexOf ('/'));
			return Path.Combine (path, filename);
		}
	}

	public IEnumerator MakeMiniBtn(string BtnName, GameObject objOwner)
	{
		yield return new WaitForEndOfFrame ();
		
		GameObject objMiniBtn = Instantiate (Resources.Load ("Prefabs/UI/MiniButtons/" + BtnName) as GameObject) as GameObject;
		
		objMiniBtn.GetComponent<UIFollowTarget> ().target = objOwner.transform;
		objMiniBtn.transform.GetChild (0).gameObject.SetActive (true);
		objMiniBtn.transform.parent = GameObject.Find ("MiniButtons").transform;
		objMiniBtn.transform.localScale = Vector3.one;
	}
}



















