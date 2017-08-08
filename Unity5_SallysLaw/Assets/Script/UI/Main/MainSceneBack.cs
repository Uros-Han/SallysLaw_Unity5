using UnityEngine;
using System.Collections;

public class MainSceneBack : MonoBehaviour {

	public void OnClick()
	{
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_EXIT);

		switch(GameMgr.getInstance.m_uiStatus)
		{
		case MainUIStatus.WORLD:
			MainScene.getInstance.ToMain();
			break;

		case MainUIStatus.STAGE:
			#if UNITY_STANDALONE || UNITY_WEBGL
			GameObject.Find("Cursor").GetComponent<Cursor_World>().EscapeInStage();
			#endif
			StartCoroutine(MainScene.getInstance.WorldToStage(false));
			break;

		case MainUIStatus.OPTION:
			StartCoroutine(MainScene.getInstance.ToOption (false));
			break;

		case MainUIStatus.LANGUAGE:
			StartCoroutine(MainScene.getInstance.ToLanguage (false));
			break;

		case MainUIStatus.CREDIT:
			StartCoroutine(MainScene.getInstance.ToCredit (false));
			break;

		case MainUIStatus.PHOTO:
			StartCoroutine(MainScene.getInstance.ToPhoto (false));
			break;

		case MainUIStatus.REALLY_BUY:
			StartCoroutine(MainScene.getInstance.ToReallyBuy (false));
			break;

		case MainUIStatus.RESTORE:
			StartCoroutine(MainScene.getInstance.ToRestore (false));
			break;

		case MainUIStatus.RESTORE_CHECK:
			StartCoroutine(MainScene.getInstance.ToRestoreCheck (false));
			break;

		case MainUIStatus.MAIN:
			StartCoroutine(MainScene.getInstance.ToReallyQuit (true));
			break;

		case MainUIStatus.REALLY_QUIT:
			StartCoroutine(MainScene.getInstance.ToReallyQuit (false));
			break;

		case MainUIStatus.CONTROL:
			StartCoroutine(MainScene.getInstance.ToControl (false));
			break;
		}
	}

	void Update()
	{
		if (Input.GetKeyUp (KeyCode.Escape)
		    #if UNITY_STANDALONE_WIN
		    ||  Input.GetKeyUp (KeyCode.JoystickButton1))
			#elif UNITY_STANDALONE_OSX
			|| Input.GetKeyUp (KeyCode.JoystickButton17))
			#else
			)
			#endif
			OnClick ();
	}
}
