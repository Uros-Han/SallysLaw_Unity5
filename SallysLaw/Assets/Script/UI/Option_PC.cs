using UnityEngine;
using System.Collections;

public class Option_PC : MonoBehaviour {
	public int m_iCurResolutionIdx;
	public int m_iTmpResolutionIdx;

	public bool m_bCurFullScreen;
	public bool m_bTmpFullScreen;

	public int m_iCurLang;
	public int m_iTmpLang;

	GameMgr gMgr;
	Cursor_Option cursor;
	Transform applyTransform;
#if UNITY_STANDALONE
	// Use this for initialization
	public void Start () {
		cursor = transform.FindChild ("Cursor_option").gameObject.GetComponent<Cursor_Option> ();
		applyTransform = transform.FindChild ("Apply").transform;


		transform.FindChild ("Sound").GetChild (1).GetComponent<UISlider> ().value = PlayerPrefs.GetFloat ("SoundVolume");
		transform.FindChild ("Music").GetChild (1).GetComponent<UISlider> ().value = PlayerPrefs.GetFloat ("MusicVolume");

		m_iCurResolutionIdx = PlayerPrefs.GetInt("Resolution");
		m_iTmpResolutionIdx = m_iCurResolutionIdx;

		if (PlayerPrefs.GetInt ("FullScreen").Equals (1)) {
			m_bCurFullScreen = true;
			transform.FindChild ("WindowMode").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = Localization.Get("FullScreen");
		} else {
			m_bCurFullScreen = false;
			transform.FindChild ("WindowMode").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = Localization.Get("Window");
		}
		m_bTmpFullScreen = m_bCurFullScreen;

		transform.FindChild ("Resolution").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = string.Format("{0}x{1}",Screen.resolutions[m_iCurResolutionIdx].width,Screen.resolutions[m_iCurResolutionIdx].height);


		for(int i = 0 ; i < Localization.knownLanguages.Length; ++i)
		{
			if(Localization.language == Localization.knownLanguages[i])
			{
				m_iCurLang = i;
				m_iTmpLang = m_iCurLang;
				break;
			}
		}
		transform.FindChild ("Lang").GetChild (1).GetChild (1).GetComponent<UILabel> ().text = Localization.language;


		transform.FindChild ("Resolution").GetChild (0).GetComponent<UILabel>().text = Localization.Get("Resolution");
		transform.FindChild ("WindowMode").GetChild (0).GetComponent<UILabel>().text = Localization.Get("ScreenMode");
		transform.FindChild ("Lang").GetChild (0).GetComponent<UILabel>().text = Localization.Get("Lan");

		gMgr = GameMgr.getInstance;
	}

	void Update()
	{
		if (gMgr.m_uiStatus.Equals (MainUIStatus.OPTION)) {
			if(cursor.m_CursorState.Equals(CURSOR_OPTION_STATE.CREDIT) || cursor.m_CursorState.Equals(CURSOR_OPTION_STATE.CONTROL))
			{
				applyTransform.GetComponent<UIWidget>().alpha = 0.8f;
				applyTransform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UILabel>().text = Localization.Get("Select");
				applyTransform.GetChild(0).GetChild(1).GetChild(0).GetComponent<UILabel>().text = Localization.Get("Select");
			}else if(m_iCurResolutionIdx != m_iTmpResolutionIdx || m_bCurFullScreen != m_bTmpFullScreen || m_iCurLang != m_iTmpLang ){
				applyTransform.GetComponent<UIWidget>().alpha = 0.8f;
				applyTransform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UILabel>().text = Localization.Get("Apply");
				applyTransform.GetChild(0).GetChild(1).GetChild(0).GetComponent<UILabel>().text = Localization.Get("Apply");
			}else{
				applyTransform.GetComponent<UIWidget>().alpha = 0f;
				applyTransform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UILabel>().text = Localization.Get("Select");
				applyTransform.GetChild(0).GetChild(1).GetChild(0).GetComponent<UILabel>().text = Localization.Get("Select");
			}
		}
	}
#endif
}
