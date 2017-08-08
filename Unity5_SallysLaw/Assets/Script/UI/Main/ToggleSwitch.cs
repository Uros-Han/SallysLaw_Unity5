using UnityEngine;
using System.Collections;

public class ToggleSwitch : MonoBehaviour {

	public bool bTrue;

	void Start()
	{
		if (PlayerPrefs.GetInt (transform.parent.name) == 0) {
			Toggle(false);
			bTrue = false;
		}else
			transform.GetChild (1).GetComponent<UILabel>().text = Localization.Get("On");

		if (transform.parent.name.Equals ("iCloud")) {
			#if UNITY_ANDROID
			transform.parent.GetChild (0).GetComponent<UILabel> ().text = "Google Cloud";
			#elif UNITY_IOS
			transform.parent.GetChild(0).GetComponent<UILabel>().text = "iCloud";
			#endif
		}
	}

	public void CloudConnectCheck()
	{
		if (transform.parent.name.Equals("iCloud")) {
//			
//			#if UNITY_ANDROID
//			
//			if(!GameCenterManager.isAuthenticated())
//			{
//				Toggle(false);
//				bTrue = false;
//				GameMgr.getInstance.m_bCloud = false;
//			}
//			
//			#elif UNITY_IOS
//			
//			if(!iCloudBinding.documentStoreAvailable())
//			{
//				Toggle(false);
//				bTrue = false;
//				GameMgr.getInstance.m_bCloud = false;
//			}
//
//			#endif
//			
		}
	}

	public void OnClick()
	{

		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		if (bTrue) {
			Toggle(false);
			bTrue = false;
			PlayerPrefs.SetInt(transform.parent.name, 0);

			switch(transform.parent.name)
			{
			case "Sound":
				GameMgr.getInstance.m_bSoundMute = true;
				break;

			case "Music":
				GameMgr.getInstance.m_bBgmMute = true;
				GameObject.Find ("BGM").GetComponent<AudioSource> ().Stop ();
				break;

			case "iCloud":
				GameMgr.getInstance.m_bCloud = false;
				break;

			}

		} else {
			Toggle(true);
			bTrue = true;
			PlayerPrefs.SetInt(transform.parent.name, 1);

			switch(transform.parent.name)
			{
			case "Sound":
				GameMgr.getInstance.m_bSoundMute = false;
				break;
				
			case "Music":
				GameMgr.getInstance.m_bBgmMute = false;
				GameObject.Find ("BGM").GetComponent<AudioSource> ().Play ();
				break;
				
			case "iCloud":
				GameMgr.getInstance.m_bCloud = true;

				// login gamecenter or googleplay
//
//#if UNITY_IOS
//				if(!iCloudBinding.documentStoreAvailable())
//#elif UNITY_ANDROID
//				if(!GameCenterManager.isAuthenticated())
//#else 
				if(true)
//#endif
				{
					#if UNITY_ANDROID
//					MobileNativeMessage msg = new MobileNativeMessage("Google Cloud", "Please connect to the Play service");
					#elif UNITY_IPHONE
//					MobileNativeMessage msg = new MobileNativeMessage("iCloud", "Please check the iCloud settings");
					#endif


					Toggle(false);
					bTrue = false;
					GameMgr.getInstance.m_bCloud = false;
				}else{
//					CloudMgr.getInstance.GameData_Save();
				}

				break;
				
			}

		}
	}


	public void Toggle(bool bTrue)
	{
		if (bTrue) {
			transform.GetChild (0).localPosition = new Vector2 (150, -20);
			transform.GetChild (1).localPosition = new Vector2(-84, 0);
			transform.GetChild (1).GetComponent<UILabel>().color = Color.white;
			transform.GetChild (1).GetComponent<UILocalize>().key = "On";
			transform.GetChild (1).GetComponent<UILocalize>().value = Localization.Get("On");

		} else {
			transform.GetChild (0).localPosition = new Vector2 (-150, -20);
			transform.GetChild (1).localPosition = new Vector2(84, 0);
			transform.GetChild (1).GetComponent<UILabel>().color = new Color(151/255f, 163/255f, 153/255f);
			transform.GetChild (1).GetComponent<UILocalize>().key = "Off";
			transform.GetChild (1).GetComponent<UILocalize>().value = Localization.Get("Off");
		}
	}

}
