using UnityEngine;
using System.Collections;

public class StageButton : MonoBehaviour {

	bool m_bBranchOn;
	float m_fRandDelay;
	IEnumerator circleFill;

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	void GrowBranch(bool bGrow)
	{
		transform.parent.GetChild (1).GetComponent<TweenScale> ().onFinished.Clear ();
		//branch line onfisnish clear

		m_fRandDelay = Random.Range (0.35f, 0.85f);
		//stage line scale
		transform.parent.GetChild (1).GetComponent<TweenScale> ().duration = 0.2f;

		if (bGrow) {
			transform.parent.GetChild (1).GetComponent<TweenScale> ().delay = m_fRandDelay;
			transform.parent.GetChild(1).GetComponent<TweenScale>().onFinished.Add(new EventDelegate(this, "StartCircleFill"));

			transform.GetChild(0).GetComponent<TweenAlpha>().delay = 0f;
		}else {

			m_fRandDelay -= 0.35f; // 0초부터 시작되도록 위에선 0.35초부터 시작

			transform.GetChild(0).GetComponent<TweenAlpha>().delay = m_fRandDelay;
			transform.GetChild(0).GetComponent<TweenAlpha>().Play(false); // stage btn 내용물 알파 흐려지게

			transform.parent.GetChild (1).GetComponent<TweenScale> ().delay = 1.1f + m_fRandDelay;
			StartCoroutine(CircleFill(false));
		}

		transform.parent.GetChild (1).GetComponent<TweenScale> ().Play (bGrow);
	}

	void GrowCircle(bool bFill)
	{

		if (circleFill != null)
			StopCoroutine (circleFill);

		circleFill = CircleFill (bFill);

		StartCoroutine (circleFill);

	}

	IEnumerator CircleFill(bool bFill)
	{
		if (bFill)
			yield return new WaitForSeconds (0.5f);

		GetComponent<TweenAlpha> ().Play (bFill);
		GetComponent<TweenScale> ().Play (bFill);
	}

	public void StartCircleFill()
	{
		if(!m_bBranchOn)
			StartCoroutine (CircleFill (true));
		else
			StartCoroutine (CircleFill (false));
	}

	public void OnClick()
	{
		if (GetComponent<UISprite> ().spriteName == "circle_full") {
			AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
			StartCoroutine (Delay ());
		} else {
			AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.BUTTON_ERROR);
		}
	}
	
	IEnumerator Delay()
	{
		GameObject.Find ("FadePanel").GetComponent<MainFade> ().FadeOn ();
		yield return new WaitForSeconds (1.0f);

		GameMgr.getInstance.m_strSelectedStage = "stage" + transform.parent.parent.parent.parent.gameObject.name + transform.parent.gameObject.name ;
		GameMgr.getInstance.m_iCurStage = System.Convert.ToInt32(transform.parent.name);
		Application.LoadLevel ("Loading");
	}
}
