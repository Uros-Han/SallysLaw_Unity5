using UnityEngine;
using System.Collections;

public class ControllerSwapper : MonoBehaviour {

	// Use this for initialization
	void SwapController(bool bToKeyboard)
	{
#if UNITY_STANDALONE
		int iKeyboardIdx = 0;
		for(int i = 0 ; i < transform.childCount; ++i)
		{
			if(transform.GetChild(i).gameObject.name.Contains("Keyboard"))
			{
				iKeyboardIdx = i;
				break;
			}
		}

		if (bToKeyboard) {
			transform.GetChild (iKeyboardIdx).gameObject.SetActive (true);
			transform.GetChild (1 - iKeyboardIdx).gameObject.SetActive (false);
		} else {
			transform.GetChild (iKeyboardIdx).gameObject.SetActive (false);
			transform.GetChild (1 - iKeyboardIdx).gameObject.SetActive (true);
		}
#endif
	}
}
