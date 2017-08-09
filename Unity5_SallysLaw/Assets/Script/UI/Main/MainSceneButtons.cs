using UnityEngine;
using System.Collections;

public class MainSceneButtons : MonoBehaviour {

	void OnClick()
	{
		if (GameMgr.getInstance.m_uiStatus.Equals (MainUIStatus.OPTION)) {
			AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
			StartCoroutine (MainScene.getInstance.ToRestore (true));
		} else if (GameMgr.getInstance.m_uiStatus.Equals (MainUIStatus.RESTORE)) {
//			if (transform.parent.name.Equals ("Confirm")) {
//
//				AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
//				#if UNITY_ANDROID
//
//				if(GameMgr.getInstance.m_bPurchaseCostume)
//				{
//					MainScene.getInstance.ToRestoreChecker(3); // already have
//				}else{
//
//
//					if(!Storekit_Sally.isInited)
//					{
//						MainScene.getInstance.BlockPanel(true);
//						Storekit_Sally.Initialize();
//					}else{
//						MainScene.getInstance.ToRestoreChecker(3); // didnt buy;;;;
//					}
//
//
//					if(GameMgr.getInstance.m_bPurchaseCostume)
//					{
//						MainScene.getInstance.ToRestoreChecker(1);
//					}else{
//						MainScene.getInstance.ToRestoreChecker(2);
//					}
//				}
//
//				#elif UNITY_IOS
//				if(GameMgr.getInstance.m_bPurchaseCostume)
//				{
//					MainScene.getInstance.ToRestoreChecker(3);
//				}else
//				{
//					MainScene.getInstance.BlockPanel(true);
//					Storekit_Sally.RestoreItem();
//				}
//
//				#endif
//			} else {
//				AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.BUTTON_EXIT);
//				StartCoroutine (MainScene.getInstance.ToRestore (false));
//			}
//		} else if(GameMgr.getInstance.m_uiStatus.Equals (MainUIStatus.RESTORE_CHECK)){
//			AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
//			StartCoroutine (MainScene.getInstance.ToRestoreCheck (false));
		} else if(GameMgr.getInstance.m_uiStatus.Equals (MainUIStatus.REALLY_QUIT))
		{
			if (transform.parent.name.Equals ("Confirm")) {
				AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
//				Application.Quit();
				JumpManager.getInstance.Exit();
			}else{
				AudioMgr.getInstance.PlaySfx (GameObject.Find ("SFX").GetComponent<AudioSource> (), "ui_bundle", (int)UI_SOUND_LIST.BUTTON_EXIT);
				StartCoroutine (MainScene.getInstance.ToReallyQuit (false));
			}
		}
	}
}
