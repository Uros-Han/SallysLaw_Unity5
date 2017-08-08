using UnityEngine;
using System.Collections;

public class TryRunnerBtn : MonoBehaviour {

	void OnClick()
	{
		EffectManager.getInstance.RunFadeEffect(true, 2.5f, PLAYER_STATUS.RUNNER);
		UIManager.getInstance.Pause (false);
		TimeMgr.Pause ();


//		GameMgr.getInstance.RestartWithRunner ();
//		TimeMgr.Play ();
	}
}
