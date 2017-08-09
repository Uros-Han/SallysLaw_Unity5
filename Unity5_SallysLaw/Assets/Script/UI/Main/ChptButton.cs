using UnityEngine;
using System.Collections;

public class ChptButton : MonoBehaviour {

	TweenPosition m_tweenPos;
	int m_iThisChptNum;

	void Start()
	{
		m_tweenPos = GetComponent<TweenPosition> ();
		m_iThisChptNum = System.Convert.ToInt32(gameObject.name.Substring(0,1));
	}

	public void OnClick()
	{
		if (transform.parent.parent.GetComponent<UIPanel> ().alpha == 1f && GameMgr.getInstance.m_uiStatus == MainUIStatus.WORLD) {

			AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
			GameMgr.getInstance.m_iCurChpt = m_iThisChptNum;


			StartCoroutine(MainScene.getInstance.WorldToStage(true));

			if (gameObject.name == "5-") {
				GameObject.Find ("6-").GetComponent<TweenAlpha> ().Play (true);
			}
		}
	}

	void MoveOrder(bool bMsg)
	{
		if (bMsg) {
			m_tweenPos.to = new Vector3 ((1800f * m_iThisChptNum) - (GameMgr.getInstance.m_iCurChpt * 1800f) - 900f, 0);

		}
		if (bMsg)
			m_tweenPos.delay = 0.5f;
		else
			m_tweenPos.delay = 0f;

		m_tweenPos.Play (bMsg);
	}
}
