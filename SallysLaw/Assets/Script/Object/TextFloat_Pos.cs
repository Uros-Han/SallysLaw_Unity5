using UnityEngine;
using System.Collections;

public class TextFloat_Pos : MonoBehaviour {

	public string m_strKey; //Text Index
	public GameObject m_objFloatUI;
	public bool m_bFatherText;
	public bool m_bMemoryText;

	// Use this for initialization
	void Start () {
		CreateUI ();
	}

	void OnDestroy()
	{
		Destroy (m_objFloatUI);
	}

	void CreateUI()
	{
		m_objFloatUI = Instantiate (ObjectPool.getInstance.m_TextFloat_UI) as GameObject;

		UIFollowTarget followUI = m_objFloatUI.GetComponent<UIFollowTarget> ();
		followUI.transform.parent = GameObject.Find ("TextFloatUI").transform;
		followUI.transform.localScale = Vector3.one;
		followUI.target = transform;

		if (m_bFatherText) {
			m_objFloatUI.transform.GetChild (0).GetComponent<TextFloat_UI> ().m_bFatherText = true;
//			m_objFloatUI.transform.GetChild (0).GetChild(0).GetComponent<UISprite>().color = Color.white;
		}

		if(m_bMemoryText)
			m_objFloatUI.transform.GetChild (0).GetComponent<TextFloat_UI> ().m_bMemoryText = true;
	}
}
