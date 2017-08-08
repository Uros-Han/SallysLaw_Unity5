using UnityEngine;
using System.Collections;

public class NavigationUI : MonoBehaviour {
	int m_iChildCount;
	UIScrollBar m_ScrollBar;
	public int m_iCurPhoto;

	UISprite[] m_NavigatorSprite;

	// Use this for initialization
	void Start () {
		m_iChildCount = transform.childCount;
		m_ScrollBar = GetComponent<UIScrollBar> ();

		m_NavigatorSprite = new UISprite[8]{ transform.GetChild (0).GetComponent<UISprite> (), transform.GetChild (1).GetComponent<UISprite> (),
											transform.GetChild (2).GetComponent<UISprite> (), transform.GetChild (3).GetComponent<UISprite> (), transform.GetChild (4).GetComponent<UISprite> (), 
											transform.GetChild (5).GetComponent<UISprite> (), transform.GetChild (6).GetComponent<UISprite> (), transform.GetChild (7).GetComponent<UISprite> ()};

	}
	
	// Update is called once per frame
	void Update () {

		m_iCurPhoto = (int)(m_ScrollBar.value / (1f / (float)m_iChildCount));

		if (m_iCurPhoto == m_iChildCount)
			m_iCurPhoto -= 1;

		for (int i= 0; i< m_NavigatorSprite.Length; ++i) {
			m_NavigatorSprite[i].color = Color.white;
		}

		m_NavigatorSprite[m_iCurPhoto].color = new Color(49/255f, 89/255f, 53/255f);
	}
}
