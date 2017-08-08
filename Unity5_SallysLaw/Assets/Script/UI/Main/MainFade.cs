using UnityEngine;
using System.Collections;

public class MainFade : MonoBehaviour {

	public bool m_bDontFade = false;

	void Awake()
	{
		if (m_bDontFade)
			return;

		AudioListener.volume = 0;
		GetComponent<UIPanel> ().alpha = 1f;
		StartCoroutine ("FadeOff");

	}


	IEnumerator FadeOff()
	{
		yield return new WaitForSeconds(0.5f);

		StartCoroutine(AudioMgr.getInstance.VolumeChg(true, 1f));

		TweenAlpha alpha = GetComponent<TweenAlpha> ();
		
		alpha.from = 1;
		alpha.to = 0;
		alpha.ResetToBeginning ();
		alpha.PlayForward ();
	}

	public void FadeOn()
	{
		StartCoroutine(AudioMgr.getInstance.VolumeChg(false, 1f));

		TweenAlpha alpha = GetComponent<TweenAlpha> ();

		alpha.from = 0;
		alpha.to = 1;
		alpha.duration = 0.75f;
		alpha.ResetToBeginning ();
		alpha.PlayForward ();
	}

	public void DontFadeNow()
	{
		m_bDontFade = true;

		AudioListener.volume = 1;
		GetComponent<UIPanel> ().alpha = 0f;
		StopCoroutine ("FadeOff");
	}

}
