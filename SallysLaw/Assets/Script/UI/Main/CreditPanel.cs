using UnityEngine;
using System.Collections;

public class CreditPanel : MonoBehaviour {

	public bool m_bScrollPushed;
	float fLogoYPos;
	float fOriginYPos;

	// Use this for initialization
	void Start () {

		if (!Application.loadedLevelName.Equals ("Main")) {
			transform.localScale = new Vector2 (0.8f, 0.8f);
			transform.localPosition = new Vector2 (0, -1100f);
			fOriginYPos = -1100f;
			fLogoYPos = transform.GetChild (0).transform.localPosition.y * -0.8f;
		} else {
			fLogoYPos = transform.GetChild (0).transform.localPosition.y * -1f;
			fOriginYPos = -1200f;
		}
	}
	
	public IEnumerator CreditUpward()
	{
		float fScrollSpeed = 0.25f;
		transform.localPosition = new Vector2 (0, fOriginYPos);
		UIPanel panel = GetComponent<UIPanel> ();
		float fBeforePanelAlpha = 0f;

		float fLogoTimer = 0;
		float fCreditExitTime = 3f;

		while(panel.alpha >= fBeforePanelAlpha){

			while(transform.localPosition.y < fLogoYPos - 8 && !m_bScrollPushed){
				transform.Translate(Vector3.up * Time.deltaTime * fScrollSpeed);
				fLogoTimer = 0;


				if (panel.alpha < 0.01f  && fBeforePanelAlpha > panel.alpha)
					break;

				if (transform.localPosition.y < fOriginYPos)
					transform.localPosition = new Vector2 (0, fOriginYPos);

				fBeforePanelAlpha = panel.alpha;
				yield return null;
			};

			while(transform.localPosition.y > fLogoYPos + 8 && !m_bScrollPushed){
				transform.Translate(Vector3.down * Time.deltaTime * fScrollSpeed);
				fLogoTimer = 0;


				if (panel.alpha < 0.01f  && fBeforePanelAlpha > panel.alpha)
					break;

				fBeforePanelAlpha = panel.alpha;
				yield return null;
			};

			fLogoTimer += Time.unscaledDeltaTime;
			if(fLogoTimer > fCreditExitTime)
				Exit();

			fBeforePanelAlpha = panel.alpha;
			yield return null;
		};
	}

	void Exit()
	{
		if (Application.loadedLevelName.Equals ("Main")) {
			#if UNITY_STANDALONE
			if(PC_InputControl.getInstance.GetInputState() == PC_InputControl.eInputState.MouseKeyboard)
				GameObject.Find("LeftTop_Keyboard").GetComponent<MainSceneBack>().OnClick();
			else
				GameObject.Find("LeftTop_Joystick").GetComponent<MainSceneBack>().OnClick();
			#else
			GameObject.Find("LeftTop_Mobile").GetComponent<MainSceneBack>().OnClick();
			#endif
		}else
			GameObject.Find ("CreditExit(Clone)").GetComponent<CreditExit> ().Exit ();
	}

	void OnPress(bool isDown)
	{
		if (isDown) {
			m_bScrollPushed = true;
		} else {
			m_bScrollPushed = false;
		}
	}
}
