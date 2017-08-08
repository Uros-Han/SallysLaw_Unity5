using UnityEngine;
using System.Collections;

public class CreditExit : MonoBehaviour {

	public void Exit()
	{
		GameObject.Find ("CreditPanel(Clone)").BroadcastMessage("TweenActivate", false);
		GameObject.Find ("CreditExit(Clone)").BroadcastMessage("TweenActivate", false);
		StartCoroutine (LoadMain ());
	}

	bool m_bBtnActivated;
	UIPanel m_panel;

	void Start()
	{
		m_bBtnActivated = false;
		m_panel = GetComponent<UIPanel> ();
	}

	void Update()
	{
		if (!m_bBtnActivated && m_panel.alpha.Equals (1)) {
			m_bBtnActivated = true;
//			GetComponent<BoxCollider> ().enabled = true;
		}

		if (!m_bBtnActivated)
			return;

		if (Input.GetKeyUp (KeyCode.Escape)
		    #if UNITY_STANDALONE_WIN
		    || Input.GetKeyUp (KeyCode.JoystickButton1))
			#elif UNITY_STANDALONE_OSX
			|| Input.GetKeyUp (KeyCode.JoystickButton17))
			#else
			)
			#endif
				Exit();
	}

	IEnumerator LoadMain()
	{
		StartCoroutine(AudioMgr.getInstance.VolumeChg (false));
		yield return new WaitForSeconds (1f);

		TimeMgr.Play ();
		Application.LoadLevel ("Main");
	}
}
