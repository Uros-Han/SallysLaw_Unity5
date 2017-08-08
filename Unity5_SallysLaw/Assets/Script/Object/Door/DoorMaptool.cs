using UnityEngine;
using System.Collections;

public class DoorMaptool : MonoBehaviour {

	public bool m_bThisIsR_DOOR;

	MapToolMgr m_mapToolMgr;

	GameObject objMiniBtn;
	bool m_bConfirmButtonOn;

	public Color m_Color;//맵툴을 위한 (색 변경을 위한) 변수. 키,스위치들은 전부 맵툴,스테이지 전범위에서 쓰이는 doorPosFixer안의 Color변수를 보고 따라감
	int m_iColorPresetNum;

	void Start()
	{
		Color thisColor = transform.parent.GetComponent<DoorPosFixer> ().m_Color;
		Debug.Log (thisColor);
		if (thisColor == new Color (87 / 255f, 223 / 255f, 255 / 255f)) {
			m_iColorPresetNum = 0;
			transform.parent.GetComponent<DoorPosFixer> ().m_Color = new Color (145 / 255f, 243 / 255f, 255 / 255f);
			ChgColor();
		} else if (thisColor == Color.red) {
			m_iColorPresetNum = 1;
			transform.parent.GetComponent<DoorPosFixer> ().m_Color = new Color(196/255f, 254/255f, 72/255f);
			ChgColor();
		} else if (thisColor == Color.yellow) {
			m_iColorPresetNum = 2;
			transform.parent.GetComponent<DoorPosFixer> ().m_Color = new Color(255/255f, 209/255f, 68/255f);
			ChgColor();
		} else if (thisColor == Color.green) {
			m_iColorPresetNum = 3;
			transform.parent.GetComponent<DoorPosFixer> ().m_Color = new Color(147/255f, 157/255f, 255/255f);
			ChgColor();
		} else {
			m_iColorPresetNum = 0;
		}

		m_mapToolMgr = GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ();
		StartCoroutine (MousePushCheck ());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	public void ChgColor()
	{
		Transform SpritesParent = transform.parent.GetChild (1).transform;
		Transform SwitchParent = GameObject.Find ("Switchs").transform;

		m_iColorPresetNum += 1;

		if (m_iColorPresetNum > 3)
			m_iColorPresetNum = 0;

		//Color Preset
		switch (m_iColorPresetNum) {
		case 0:
			m_Color = new Color (145 / 255f, 243 / 255f, 255 / 255f);
			break;
		case 1:
			m_Color = new Color(196/255f, 254/255f, 72/255f);
			break;
		case 2:
			m_Color = new Color(255/255f, 209/255f, 68/255f);
			break;
		case 3:
			m_Color = new Color(147/255f, 157/255f, 255/255f);
			break;
		}

		//문 외곽선 색 바꾸기
//		for (int i = 0; i < SpritesParent.childCount; ++i) {
//			SpritesParent.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().color = m_Color;
//		}
		transform.parent.GetChild(3).GetComponent<SkeletonAnimation>().skeleton.SetColor(m_Color);


		//문에 해당하는 열쇠,버튼 색 바꾸기
		for (int i = 0; i < SwitchParent.childCount; ++i) {
			if(SwitchParent.GetChild(i).GetComponent<Switch>().m_objDoor == transform.parent.gameObject)
				SwitchParent.GetChild(i).GetComponent<SkeletonAnimation>().skeleton.SetColor(m_Color);
		}

		transform.parent.GetComponent<DoorPosFixer> ().m_Color = m_Color;

	}

	IEnumerator MousePushCheck()
	{
		yield return new WaitForSeconds (0.1f);

		while(true){
			if (Input.GetMouseButtonDown (0) && GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_CurPref != null && !GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr>().m_bNowPlaying && !m_mapToolMgr.m_bOverayUIOn) {


				Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				if(UICamera.selectedObject != null && UICamera.selectedObject.name == "MiniBtnSprite" && UICamera.selectedObject.transform.parent.parent.GetComponent<UIFollowTarget> ().target == transform)
				{ // click MiniButton




					if(m_bThisIsR_DOOR)
					{
						if(UICamera.selectedObject.transform.parent.name != "ColorBtn")
						{
							transform.parent.GetChild(2).GetComponent<DestroyWhenCurprefChged> ().enabled = true;
							m_mapToolMgr.m_objCurDoor = transform.parent.gameObject;

							GameObject.Find("SwitchUI").transform.GetChild(0).GetChild(0).GetComponent<DrawBtn>().OnClick();

							if(UICamera.selectedObject.transform.parent.name == "Key"){
								m_mapToolMgr.m_bThisIsKey = true;
							}else{
								m_mapToolMgr.m_bThisIsKey = false;
							}

							m_bConfirmButtonOn = true;
							StartCoroutine (ConfirmButtonTargeting ());

						}else{
							transform.parent.GetChild(2).GetComponent<DestroyWhenCurprefChged> ().enabled = true;

							ChgColor();
						}
					}
					else
					{
						transform.GetChild(2).GetComponent<DestroyWhenCurprefChged> ().enabled = true;
						m_mapToolMgr.m_objCurDoor = transform.gameObject;

						GameObject.Find("SwitchUI").transform.GetChild(1).GetChild(0).GetComponent<DrawBtn>().OnClick();

						if(UICamera.selectedObject.transform.parent.name == "Key"){
							m_mapToolMgr.m_bThisIsKey = true;
						}else{
							m_mapToolMgr.m_bThisIsKey = false;
						}

						m_bConfirmButtonOn = true;
						StartCoroutine (ConfirmButtonTargeting ());
					}

					Destroy (objMiniBtn);




				} else if(GetComponent<Collider2D>().OverlapPoint(mousePosition)) // Mouse Click In Rect
				{
//					if(!GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bSwitchUIOn)
//					{

					yield return new WaitForEndOfFrame();
					
					if (GameObject.Find ("BoxBorder") != null || GameObject.Find ("DoorBorder") != null) { //누군가 무브할곳 선택중인데 이때 박스를 클릭하면 무브버튼생기는거 방지
						
					} else {

						//GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bSwitchUIOn = true;

						if(m_bThisIsR_DOOR)
						{

							transform.parent.GetChild(2).gameObject.SetActive(true); //border active
							StartCoroutine(MakeMiniBtn("SwitchBtn(Blue)"));
						}
						else
						{

							transform.GetChild(2).gameObject.SetActive(true); //border active
							StartCoroutine(MakeMiniBtn("SwitchBtn(Yellow)"));
						}


						GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bUIMouseOn = false;
					}
				}else {
					if (!m_bConfirmButtonOn)
					{
						if(m_bThisIsR_DOOR)
							transform.parent.GetChild(2).gameObject.SetActive(false);
						else
							transform.GetChild(2).gameObject.SetActive(false);
					}

					if (objMiniBtn != null) {
						Destroy (objMiniBtn);
					}
				}
			}

			yield return null;
		}
	}


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

}
