using UnityEngine;
using System.Collections;

public class TextFloat_sprite : MonoBehaviour {

	void OnClick()
	{
		TextFloat_UI FloatUI = transform.parent.GetComponent<TextFloat_UI> ();


		if (UICamera.currentKey == KeyCode.Mouse0) {
			if(FloatUI.m_bMemoryText)
				return;

			if (!FloatUI.m_bFatherText) {
				FloatUI.m_bFatherText = true;
				GetComponent<UISprite> ().color = Color.cyan;
			} else {
				FloatUI.m_bFatherText = false;
				GetComponent<UISprite> ().color = Color.white;
			}
		} else {
			if (!FloatUI.m_bMemoryText) {
				FloatUI.m_bMemoryText = true;
				GetComponent<UISprite> ().color = Color.red;
			} else {
				FloatUI.m_bMemoryText = false;

				if(FloatUI.m_bFatherText)
					GetComponent<UISprite> ().color = Color.cyan;
				else
					GetComponent<UISprite> ().color = Color.white;
			}
		}


	}

}
