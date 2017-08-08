using UnityEngine;
using System.Collections;

public class MapToolUIBtn : MonoBehaviour {
	public GameObject[] m_prefObj;

	float m_fTime;
	bool m_bPressed;
	float m_fPressTime;

	public bool m_bLabel; // true is Label like TopDeco

	void Start () {
		m_bPressed = false;
		m_fPressTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_bPressed) {
			m_fTime += Time.deltaTime;
			if (m_fTime > m_fPressTime) {
				transform.parent.GetChild (2).gameObject.SetActive (true);
				transform.parent.GetChild (3).gameObject.SetActive (true);

//				if(UICamera.selectedObject != null)
//				{
//					for(int i = 0; i < transform.parent.GetChild(2).childCount; ++i)
//					{
//						if(transform.parent.GetChild(2).GetChild(i).GetChild(0).gameObject == UICamera.selectedObject)
//						{
//							transform.parent.GetChild (3).gameObject.transform.position = UICamera.selectedObject.transform.position; // highlighter
//							return;
//						}
//					}
//					//transform.parent.GetChild (3).gameObject.SetActive (false);
//				}
			}
		}
	}

	bool ThisisChild()
	{
		for(int i = 0; i < transform.parent.GetChild(2).childCount; ++i)
		{
			if(transform.parent.GetChild(2).GetChild(i).GetChild(0).gameObject == UICamera.hoveredObject)
			{
				transform.parent.GetChild (3).gameObject.transform.position = UICamera.hoveredObject.transform.position;
				return true;
			}
		}

		return false;
	}

	void OnPress(bool isDown)
	{
		if (isDown) {
			m_bPressed = true;

		} else {
			if(m_fTime > m_fPressTime)
			{

				if(UICamera.hoveredObject != null && ThisisChild()) // select Child UI
				{
					transform.parent.GetComponent<UI_Obj> ().m_Object = UICamera.hoveredObject.transform.parent.GetComponent<UI_Obj> ().m_Object;

					if(!m_bLabel)
					{
						transform.parent.GetChild(1).GetComponent<UISprite> ().atlas = UICamera.hoveredObject.transform.parent.GetComponent<UISprite> ().atlas;
						transform.parent.GetChild(1).GetComponent<UISprite> ().spriteName = UICamera.hoveredObject.transform.parent.GetComponent<UISprite> ().spriteName;
						transform.parent.GetChild(1).GetComponent<UISprite> ().color = UICamera.hoveredObject.transform.parent.GetComponent<UISprite> ().color;
						transform.parent.GetChild(1).GetComponent<UISprite> ().width = UICamera.hoveredObject.transform.parent.GetComponent<UISprite> ().width;
						transform.parent.GetChild(1).GetComponent<UISprite> ().height = UICamera.hoveredObject.transform.parent.GetComponent<UISprite> ().height;
					}else{
						transform.parent.GetComponent<UI_Obj> ().m_iObjNum = UICamera.hoveredObject.transform.parent.GetComponent<UI_Obj> ().m_iObjNum;

						transform.parent.GetChild(1).GetComponent<UILabel> ().text = UICamera.hoveredObject.transform.parent.GetComponent<UILabel> ().text;
						transform.parent.GetChild(1).GetComponent<UILabel> ().fontSize = UICamera.hoveredObject.transform.parent.GetComponent<UILabel> ().fontSize;
					}
					transform.parent.GetChild(1).localScale = UICamera.hoveredObject.transform.parent.localScale;
					//transform.parent.GetChild(1).GetComponent<UISprite> ().color = UICamera.hoveredObject.transform.parent.GetComponent<UISprite> ().color;
					GetComponent<DrawBtn>().OnClick();
				}

				transform.parent.GetChild (2).gameObject.SetActive (false);
				transform.parent.GetChild (3).gameObject.SetActive (false);
			}
			m_fTime = 0.0f;
			m_bPressed = false;
		}
	}

}
