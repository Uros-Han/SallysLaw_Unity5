using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxMaptool : MonoBehaviour
{
	bool m_bThisIsFloor;
	GridMgr m_GridMgr;
	bool m_bMapToolOn;
	MapToolMgr m_mapToolMgr;
	bool m_bConfirmButtonOn;
	GameObject objMiniBtn;

	int m_iThisGridIdx;

	public string m_BtnName = "MoveBtn";

	//스프라이트 관련 변수들
	Sprite[] m_SpriteArray;
	bool m_bTop; //위에 어떤 박스가 있을때 참
	bool m_bRight; //오른쪽에 어떤 박스가 있을때 참
	bool m_bLeft; //왼쪽에 어떤 박스가 있을때 참
	bool m_bBottom; //아래에 어떤 박스가 있을때 참
	public int m_iSpriteIdx;




	void Start ()
	{
		if (GameObject.Find ("Grids") != null)
			m_bMapToolOn = true;

		if (m_bMapToolOn)
			m_GridMgr = GridMgr.getInstance;

		m_mapToolMgr = GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ();

		m_iThisGridIdx = m_GridMgr.GetGridIdx (transform.position);

		if (m_bMapToolOn && !m_mapToolMgr.m_bNowPlaying && gameObject.name != "Spike(Clone)" && gameObject.name != "Spring(Clone)" && gameObject.name != "Portal(Clone)" && gameObject.name != "Portal_Exit(Clone)") {
			FloorCheck ();

			m_SpriteArray = ObjectPool.getInstance.m_sprite_sally_Chpt5Box;
			DirectionMapping (true);
			SpriteMapping ();
		}



		if(m_BtnName != "MoveBtn")
			StartCoroutine (MousePushCheck ());


	}

	void OnDestroy ()
	{
		StopAllCoroutines ();

		if(m_bMapToolOn)
			DirectionMapping (false);
		
		if (m_bMapToolOn) {
			for (int i = 0; i < transform.childCount; ++i) {
				
				for (int j = 0; j < m_mapToolMgr.m_listObjects.Count; ++j) {
					if (m_mapToolMgr.m_listObjects [j].FindObject (transform.GetChild (i).gameObject))
						m_mapToolMgr.m_listObjects.RemoveAt (j);
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.transform.CompareTag ("Box")) {
			SendMessageUpwards("MergeCollider", coll.gameObject.GetComponent<BoxCollider2D>());
		}
	}

	/// <summary>
	/// Box Sprites Directions mapping.
	/// </summary>
	/// <param name="bObjCreated">If set to <c>true</c> b object created.</param>
	public void DirectionMapping(bool bObjCreated) // 오브젝트가 추가될땐 bObjCreated = true,,,, 제거될땐 bObjCreated = false
	{
		if (GameObject.Find ("Boxes") == null)
			return;

		Transform BoxParent = GameObject.Find ("Boxes").gameObject.transform;

		//바로 옆에 있는 아이들 찾고 위치 부울 변수 변환해주기
		for (int i = 0; i < BoxParent.childCount; ++i) {

			BoxMaptool target = BoxParent.GetChild(i).GetComponent<BoxMaptool>();

			if(m_iThisGridIdx + 1 == target.m_iThisGridIdx) // right side
			{
				target.m_bLeft = bObjCreated;
				m_bRight = bObjCreated;
				target.SpriteMapping();
			}
			else if(m_iThisGridIdx - 1 == target.m_iThisGridIdx) // left side
			{
				target.m_bRight = bObjCreated;
				m_bLeft = bObjCreated;
				target.SpriteMapping();
			}
			else if(m_iThisGridIdx - GridMgr.getInstance.m_iXcount == target.m_iThisGridIdx) // Up side
			{
				target.m_bBottom = bObjCreated;
				m_bTop = bObjCreated;
				target.SpriteMapping();
			}
			else if(m_iThisGridIdx + GridMgr.getInstance.m_iXcount == target.m_iThisGridIdx) // Down side
			{
				target.m_bTop = bObjCreated;
				m_bBottom = bObjCreated;
				target.SpriteMapping();
			}

			//가장 밑줄, Floor 생기는 애들
			if(m_iThisGridIdx >= (GridMgr.getInstance.m_iXcount * GridMgr.getInstance.m_iYcount) - GridMgr.getInstance.m_iXcount)
				m_bBottom = true;
		}
	}

	
	void ChgPortalColor()
	{

		string skinName = gameObject.GetComponent<SkeletonAnimation> ().initialSkinName;
		switch (skinName.Substring (skinName.IndexOf ("_") + 1, skinName.Length - skinName.IndexOf ("_") - 1)) {
		case "blue":
			skinName = "portal_green";
			break;

		case "green":
			skinName = "portal_greenblue";
			break;

		case "greenblue":
			skinName = "portal_skyblue";
			break;

		case "skyblue":
			skinName = "portal_yellow";
			break;

		case "yellow":
			skinName = "portal_blue";
			break;

		default:
			Debug.LogError("Portal color error");
			break;
		}

		gameObject.GetComponent<SkeletonAnimation> ().initialSkinName = skinName;
		gameObject.GetComponent<SkeletonAnimation> ().Reset ();

		GameObject owner = gameObject.GetComponent<Portal> ().m_objOwner;
		if(owner != null)
		{
			owner.GetComponent<SkeletonAnimation> ().initialSkinName = skinName;
			owner.GetComponent<SkeletonAnimation> ().Reset ();
		}
	}

	public void SpriteMapping()
	{
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer> ();

		if (!m_bTop && !m_bLeft && !m_bRight && !m_bBottom) {
			m_iSpriteIdx = 10;
		} else if (!m_bTop && !m_bLeft && !m_bRight && m_bBottom) {
			m_iSpriteIdx = 3;
		} else if (!m_bTop && !m_bLeft && m_bRight && !m_bBottom) {
			m_iSpriteIdx = 8;
		} else if (!m_bTop && !m_bLeft && m_bRight && m_bBottom) {
			m_iSpriteIdx = 0;
		} else if (!m_bTop && m_bLeft && !m_bRight && !m_bBottom) {
			m_iSpriteIdx = 9;
		} else if (!m_bTop && m_bLeft && !m_bRight && m_bBottom) {
			m_iSpriteIdx = 2;
		} else if (!m_bTop && m_bLeft && m_bRight && !m_bBottom) {
			m_iSpriteIdx = 1;
		} else if (!m_bTop && m_bLeft && m_bRight && m_bBottom) {
			m_iSpriteIdx = 1;
		} else if (m_bTop && !m_bLeft && !m_bRight && !m_bBottom) {
			m_iSpriteIdx = 7;
		} else if (m_bTop && !m_bLeft && !m_bRight && m_bBottom) {
			m_iSpriteIdx = 5;
		} else if (m_bTop && !m_bLeft && m_bRight && !m_bBottom) {
			m_iSpriteIdx = 4;
		} else if (m_bTop && !m_bLeft && m_bRight && m_bBottom) {
			m_iSpriteIdx = 5;
		} else if (m_bTop && m_bLeft && !m_bRight && !m_bBottom) {
			m_iSpriteIdx = 6;
		} else if (m_bTop && m_bLeft && !m_bRight && m_bBottom) {
			m_iSpriteIdx = 5;
		} else if (m_bTop && m_bLeft && m_bRight && !m_bBottom) {
			m_iSpriteIdx = 5;
		} else if (m_bTop && m_bLeft && m_bRight && m_bBottom) {
			m_iSpriteIdx = 5;
		}

		spriteRenderer.sprite = m_SpriteArray [m_iSpriteIdx];
	}

	IEnumerator MousePushCheck()
	{
		yield return new WaitForSeconds (0.1f);

		while(true){

			if (Input.GetMouseButtonDown (0) && !m_mapToolMgr.m_bNowPlaying && !m_mapToolMgr.m_bOverayUIOn) {
				//Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				
				if(UICamera.selectedObject != null && UICamera.selectedObject.name == "SliderPointer")
				{
				}
				else if(UICamera.selectedObject != null && UICamera.selectedObject.name == "MiniBtnSprite" && UICamera.selectedObject.transform.parent.parent.GetComponent<UIFollowTarget> ().target == transform)
				{
					if(UICamera.selectedObject.transform.parent.parent.name == "MoveBtn(Clone)")
						PressMoveBtn();
					else if(UICamera.selectedObject.transform.parent.parent.name == "PortalBtn(Clone)")
					{
						if(UICamera.selectedObject.transform.parent.name == "Portal")
							PressPortalBtn();
						else
							ChgPortalColor();
					}
				}else if (m_iThisGridIdx == m_GridMgr.m_iGridIdx) { // Mouse Click In Rect
					yield return new WaitForEndOfFrame();

					if (GameObject.Find ("BoxBorder") != null || GameObject.Find ("DoorBorder") != null) { //누군가 무브할곳 선택중인데 이때 박스를 클릭하면 무브버튼생기는거 방지

					} else {

						if( (UICamera.selectedObject == null || (UICamera.selectedObject != null && UICamera.selectedObject.name.Contains("UI Root"))) )
						{
							transform.GetChild (0).gameObject.SetActive (true);
							StartCoroutine (MakeMiniBtn (m_BtnName));
						}
					}
				}else {
					yield return new WaitForSeconds(0.1f);
					if (!m_bConfirmButtonOn)
						transform.GetChild (0).gameObject.SetActive (false);
					
					if (objMiniBtn != null) {
						Destroy (objMiniBtn);
					}
				}
			}

			yield return null;
		}

	}

	public void PressMoveBtn()
	{
		GameObject.Find ("MoveBox").transform.GetChild (0).GetComponent<DrawBtn> ().OnClick ();

		bool tmpBool = false;
		for (int i = 0; i < m_mapToolMgr.m_objCurBox.Count; ++i) {
			if(m_mapToolMgr.m_objCurBox[i] == gameObject)
			{
				tmpBool = true;
				break;
			}
		}

		if(!tmpBool)
			m_mapToolMgr.m_objCurBox.Add (transform.gameObject);

		Destroy (objMiniBtn);
		m_bConfirmButtonOn = true;
		transform.GetChild (0).GetComponent<DestroyWhenCurprefChged> ().enabled = true;
		StartCoroutine (ConfirmButtonTargeting ());
	}

	public void PressPortalBtn()
	{
		GameObject.Find ("Portal(Exit)").transform.GetChild (0).GetComponent<DrawBtn> ().OnClick ();

		bool tmpBool = false;
		for (int i = 0; i < m_mapToolMgr.m_objCurBox.Count; ++i) {
			if(m_mapToolMgr.m_objCurBox[i] == gameObject)
			{
				tmpBool = true;
				break;
			}
		}
		
		if(!tmpBool)
			m_mapToolMgr.m_objCurBox.Add (transform.gameObject);

		Destroy (objMiniBtn);
		m_bConfirmButtonOn = true;
		transform.GetChild (0).GetComponent<DestroyWhenCurprefChged> ().enabled = true;
		StartCoroutine (ConfirmButtonTargeting ());
	}

	IEnumerator ConfirmButtonTargeting ()
	{
		while (GameObject.Find ("ConfirmBtn(Clone)") == null) {
			yield return null;
		}

		while (GameObject.Find ("ConfirmBtn(Clone)") != null) {
			yield return null;
		}

		m_bConfirmButtonOn = false;
	}
//
//	void LateUpdate ()
//	{
//		if (Input.GetMouseButtonDown (0) && !m_mapToolMgr.m_bNowPlaying && !m_mapToolMgr.m_bOverayUIOn) {
//			Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//			if (GameObject.Find ("BoxBorder") != null) {
//
//			}
//		}
//	}

	IEnumerator MakeMiniBtn (string BtnName)
	{
		if (objMiniBtn == null) {
			yield return new WaitForEndOfFrame ();

			objMiniBtn = Instantiate (Resources.Load ("Prefabs/UI/MiniButtons/" + BtnName) as GameObject) as GameObject;

			objMiniBtn.GetComponent<UIFollowTarget> ().target = transform;
			objMiniBtn.transform.GetChild (0).gameObject.SetActive (true);
			objMiniBtn.transform.parent = GameObject.Find ("MiniButtons").transform;
			objMiniBtn.transform.localScale = Vector3.one;
		}
	}

	void FloorCheck ()
	{
		int iThisIdx = m_GridMgr.GetGridIdx (transform.position);

		if (iThisIdx >= m_GridMgr.m_iXcount * m_GridMgr.m_iYcount - m_GridMgr.m_iXcount) { // 마지막 줄 일때
			if ((m_GridMgr.GetCount_BelowYIdxes (iThisIdx) == 0 && transform.childCount == 2) || (m_GridMgr.GetCount_BelowYIdxes (iThisIdx) != 0 && m_GridMgr.GetAll_VacantLowerIdx (iThisIdx).Count == m_GridMgr.GetCount_BelowYIdxes (iThisIdx))) {

				List<int> tmpIdxList = m_GridMgr.GetAll_VacantLowerIdx (iThisIdx);

				for (int i =0; i < tmpIdxList.Count + 5; ++i) {
					iThisIdx += m_GridMgr.m_iXcount;
					GameObject box = Instantiate (Resources.Load ("Prefabs/Objects/Boxes/Floor") as GameObject) as GameObject;
					box.transform.position = m_GridMgr.IdxPos (iThisIdx);
					box.transform.parent = transform;

					List<int> tmpIdx = new List<int> ();
					tmpIdx.Add (iThisIdx);
					IndexTag tmpTag = new IndexTag (tmpIdx, box, OBJECT_ID.FLOOR);

					m_mapToolMgr.m_listObjects.Add (tmpTag);
				}
			}
		}
	}

}
