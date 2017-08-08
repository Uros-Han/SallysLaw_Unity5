using UnityEngine;
using System.Collections;

public class ArchiveCenterBtn : MonoBehaviour {

	void Start()
	{
#if UNITY_IOS
		GetComponent<UISprite> ().spriteName = "button_achivement";
		GetComponent<UISprite> ().MakePixelPerfect ();
		transform.localScale = new Vector2 (0.35f, 0.35f);
#endif
	}

	void OnClick()
	{
//		if (!GameCenterManager.isAuthenticated ()) {
//			#if UNITY_ANDROID
//			GameCenterManager.AuthenticateLocalPlayer ();
//			#elif UNITY_IOS
//			MobileNativeMessage m_msg;
//
//			if(Application.internetReachability.Equals(NetworkReachability.NotReachable))
//			{
//				m_msg = new MobileNativeMessage("Game Center Unavailable", "Please connect to the Internet");
//
//			}else
//				m_msg = new MobileNativeMessage("Game Center Unavailable", "Player is not signed in");
//
//			#endif
//		}else
//			GameCenterManager.ShowAchievementView ();

		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
	}
}
