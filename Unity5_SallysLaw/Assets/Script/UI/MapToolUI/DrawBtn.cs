using UnityEngine;
using System.Collections;

public class DrawBtn : MonoBehaviour {
	//Pressed Object UI BTN
	public void OnClick()
	{
		GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_CurPref = transform.parent.GetComponent<UI_Obj>().m_Object;
		GetComponent<UISprite> ().color = new Color (1,0,0, GetComponent<UISprite> ().color.a);

		//transform.parent.GetChild(1).GetComponent<UISprite>().color = ;
		
		if (GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_CurSelObjUI != null && GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_CurSelObjUI != gameObject) {
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_CurSelObjUI.GetComponent<UISprite> ().color 
				= new Color (0, 0, 0, GetComponent<UISprite> ().color.a);
		}
		GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_CurSelObjUI = gameObject;

		if (gameObject.transform.parent.parent.gameObject.name != "SwitchUI") {
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bSwitchUIOn = false;
			if(GameObject.Find("Border") != null)
			GameObject.Find("Border").gameObject.SetActive(false);
		} 
	}

	void OnPress(bool isPressed)
	{
		if(!isPressed)
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bUIMouseOn = false;
		else
			GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bUIMouseOn = true;
	}

}
