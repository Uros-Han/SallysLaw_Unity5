using UnityEngine;
using System.Collections;

public class ChapterBackground : MonoBehaviour {
	UISprite m_Sprite;
	// Use this for initialization
	void Start () {
		m_Sprite = GetComponent<UISprite> ();
	}
	
	// Update is called once per frame
	void Update () {
		m_Sprite.spriteName = "bg_0" + GameMgr.getInstance.m_iCurStage;
	}
}
