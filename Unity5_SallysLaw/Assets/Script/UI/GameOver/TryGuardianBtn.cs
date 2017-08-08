using UnityEngine;
using System.Collections;

public class TryGuardianBtn : MonoBehaviour {

	void OnClick()
	{

		if (GetComponent<UISprite> ().alpha == 1) {
			EffectManager.getInstance.RunFadeEffect (true, 2.5f);
			UIManager.getInstance.Pause (false);
			TimeMgr.Pause ();
		}


//		GameMgr.getInstance.RestartWithGuardian ();
//
//		TimeMgr.Play();
	}
}
