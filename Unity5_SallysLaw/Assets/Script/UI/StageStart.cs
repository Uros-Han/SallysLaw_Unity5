using UnityEngine;
using System.Collections;

public class StageStart : MonoBehaviour {

	bool m_bKeyPress;

	UIPanel panel;

	void OnEnable()
	{
		m_bKeyPress = false;
	#if UNITY_STANDALONE || UNITY_WEBGL
		panel = transform.parent.parent.parent.GetComponent<UIPanel> ();
	#else
		panel = transform.parent.parent.GetComponent<UIPanel> ();
	#endif
	}

//	void OnClick()
//	{
//		if (!m_bKeyPress) {
//			KeyPress();
//		}
//	}

	void Update()
	{
		if (!m_bKeyPress && (Input.GetKeyDown (KeyCode.Space)
             #if UNITY_STANDALONE_WIN
             || Input.GetKeyDown (KeyCode.JoystickButton0)))
		     #elif UNITY_STANDALONE_OSX
			 || Input.GetKeyDown (KeyCode.JoystickButton16)))
			 #else
			 || Input.GetMouseButtonUp(0)))
			 #endif
			KeyPress();
		
	}

	void KeyPress()
	{
//		m_bKeyPress = true;
//		GetComponent<UIButtonScale> ().OnPress (true);
//		GetComponent<UIButton> ().SetState (UIButtonColor.State.Pressed, false);

		if (panel.alpha.Equals(0.8f)) {
			if (SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.FATHER_WAIT))
				UIManager.getInstance.FatherStart ();
			else if (SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.SALLY_WAIT))
				UIManager.getInstance.SallyStart ();
		}
	}
}
