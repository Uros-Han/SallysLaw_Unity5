using UnityEngine;
using System.Collections;

public class MiniBtn : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (transform.parent.parent.name == "DragMoveBtn(Clone)")
			StartCoroutine (DragBtn ());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	void OnClick(){

		if (transform.parent.parent.name == "ConfirmBtn(Clone)") {
//			if(GameObject.Find("BoxBorder") != null)
//			{
//				GameObject.Find("BoxBorder").gameObject.GetComponent<DestroyWhenCurprefChged>().enabled = false;
//				GameObject.Find("BoxBorder").gameObject.SetActive(false);
//
//				for(int i = 0 ; i < GameObject.Find("MoveOrders").transform.childCount; ++i)
//				{
//					if(GameObject.Find("MoveOrders").transform.GetChild(i).GetComponent<DestroyWhenCurprefChged>() != null)
//						Destroy(GameObject.Find("MoveOrders").transform.GetChild(i).GetComponent<DestroyWhenCurprefChged>());
//				}

			if (transform.parent.parent.GetComponent<UIFollowTarget> ().target.name == "Portal_Exit(Clone)") { // when potal
				GameObject target = transform.parent.parent.GetComponent<UIFollowTarget> ().target.gameObject;

				target.GetComponent<Portal> ().m_objOwner.GetComponent<Portal> ().m_objOwner = target;
				target.name = "Portal(Clone)";
				target.transform.GetChild (0).GetComponent<DestroyWhenCurprefChged> ().m_ObjHaveToBePref = Resources.Load ("Prefabs/Objects/Portals/Portal_Exit") as GameObject;



//				target.GetComponent<SpriteRenderer> ().color = tmpColor;

//				target.GetComponent<SkeletonAnimation> ().initialSkinName = "";

//				target.GetComponent<Portal> ().m_objOwner.GetComponent<SpriteRenderer> ().color = tmpColor;
			}

			//}

			if (GameObject.Find ("DoorBorder") != null) {
				GameObject.Find ("DoorBorder").gameObject.GetComponent<DestroyWhenCurprefChged> ().enabled = false;
				GameObject.Find ("DoorBorder").gameObject.SetActive (false);
			}

			if (GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_CurPref.name == "MoveOrder")
				GameObject.Find ("BoxUIs").transform.Find ("BtnSprite").GetComponent<DrawBtn> ().OnClick ();
			else if (GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_CurPref.name == "Portal_Exit")
				GameObject.Find ("DoorUIs").transform.Find ("BtnSprite").GetComponent<DrawBtn> ().OnClick ();
			else if (GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_CurPref.name == "R_Switch" || GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_CurPref.name == "G_Switch")
				GameObject.Find ("DoorUIs").transform.Find ("BtnSprite").GetComponent<DrawBtn> ().OnClick ();


			Destroy (transform.parent.parent.GetComponent<UIFollowTarget> ().target.gameObject.GetComponent<DestroyWhenCurprefChged> ());

			StartCoroutine (DestroyThis ());

		} else if (transform.parent.parent.name == "DragMoveBtn(Clone)") {

			GameObject boxes = GameObject.Find ("Boxes").gameObject;
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_objCurBox.Clear ();

			for (int i = 0; i < boxes.transform.childCount; ++i) {
				if (boxes.transform.GetChild (i).GetChild (0).gameObject.activeInHierarchy) {
					boxes.transform.GetChild (i).GetComponent<BoxMaptool> ().PressMoveBtn ();
				}
			}

			StartCoroutine (DestroyThis ());

		} else if (transform.parent.parent.name == "RotateBtn(Clone)" || transform.parent.name == "RotateBtn") {

			RotateBtn (transform.parent.parent.GetComponent<UIFollowTarget> ().target);

		} else if(transform.parent.name == "RevivalBtn") {

			if(!transform.parent.parent.GetComponent<UIFollowTarget> ().target.GetComponent<Spike>().m_bRevival)
			{
				transform.parent.parent.GetComponent<UIFollowTarget> ().target.GetChild(1).GetComponent<SpriteRenderer>().color = Color.green;
				transform.parent.parent.GetComponent<UIFollowTarget> ().target.GetComponent<Spike>().m_bRevival = true;
			}else{
				transform.parent.parent.GetComponent<UIFollowTarget> ().target.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
				transform.parent.parent.GetComponent<UIFollowTarget> ().target.GetComponent<Spike>().m_bRevival = false;
			}
		}
	}

	IEnumerator DestroyThis()
	{
		yield return new WaitForEndOfFrame ();

		
		for (int i = 0; i < transform.parent.parent.parent.childCount; ++i) {
			Destroy (transform.parent.parent.parent.GetChild(i).gameObject);
		}
	}

	void RotateBtn(Transform target)
	{
		float fAngle = target.transform.rotation.eulerAngles.z;
		
		fAngle += 90f;
		
		target.transform.rotation = Quaternion.AngleAxis(fAngle, Vector3.forward);
	}

	IEnumerator DragBtn()
	{
		if (GameObject.Find ("MiniButtons").transform.childCount > 0) {
			for(int i = 0 ; i < GameObject.Find ("MiniButtons").transform.childCount; ++i)
			{
				if(GameObject.Find ("MiniButtons").transform.GetChild(i).gameObject != transform.parent.parent.gameObject)
					Destroy (GameObject.Find ("MiniButtons").transform.GetChild(i).gameObject);
			}
		}

		yield return new WaitForEndOfFrame ();

	

		while (true) {
			if(Input.GetMouseButtonDown(0))
			{
				if (UICamera.selectedObject != null) {
					if (UICamera.selectedObject.name != "MiniBtnSprite")
					{
						GameObject boxes = GameObject.Find("Boxes").gameObject;

						for(int i = 0 ; i < boxes.transform.childCount; ++i)
						{
							if(boxes.transform.GetChild(i).GetChild(0).gameObject.activeInHierarchy)
							{
								boxes.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
							}
						}
						Destroy (transform.parent.parent.gameObject);
					}
				} else{

					GameObject boxes = GameObject.Find("Boxes").gameObject;
					
					for(int i = 0 ; i < boxes.transform.childCount; ++i)
					{
						if(boxes.transform.GetChild(i).GetChild(0).gameObject.activeInHierarchy)
						{
							boxes.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
						}

					Destroy (transform.parent.parent.gameObject);
					}
				}
			}

			yield return null;

		}
	}
}
