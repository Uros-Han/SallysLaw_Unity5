using UnityEngine;
using System.Collections;

public class Photo : MonoBehaviour {

	void OnEnable()
	{

		GameMgr gameMgr = GameMgr.getInstance;
		StageLoader stgLoader = StageLoader.getInstance;

		if (!stgLoader.m_bMaptool && !stgLoader.m_bStageLoader) {
			if(GameMgr.getInstance.m_PhotoInfo[GameMgr.getInstance.m_iCurChpt-1].m_bPhotoGet[GameMgr.getInstance.m_iCurStage-1])
				gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.parent.name.Equals("Players")) {

			if(GetComponent<SkeletonAnimation> ().loop.Equals(false))
				return;

			GetComponent<SkeletonAnimation> ().loop = false;
			GetComponent<SkeletonAnimation> ().AnimationName = "get";
			AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.GET_PHOTO);

			StartCoroutine(PhotoGetLabel());
		}
	}

	IEnumerator PhotoGetLabel()
	{
		UIManager.getInstance.m_InfoPanel.GetComponent<UIPanel> ().alpha = 1f;

		yield return new WaitForSeconds (1f);

		UIManager.getInstance.m_InfoPanel.GetComponent<TweenAlpha> ().ResetToBeginning ();
		UIManager.getInstance.m_InfoPanel.GetComponent<TweenAlpha> ().Play ();
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.FLASHBACK)) {
			if (coll.transform.parent.name.Equals("Players")) {
				GetComponent<SkeletonAnimation> ().loop = true;
				GetComponent<SkeletonAnimation> ().AnimationName = "idle";
			}
		}
	}

	public void PhotoGet()
	{
		GameMgr.getInstance.m_PhotoInfo[GameMgr.getInstance.m_iCurChpt-1].m_bPhotoGet[GameMgr.getInstance.m_iCurStage-1] = true;
//		PlayerPrefs.SetInt(string.Format("PhotoInfo{0}_{1}", GameMgr.getInstance.m_iCurChpt-1, GameMgr.getInstance.m_iCurStage-1), 1);
		GameMgr.getInstance.SendMessage("NotiAlbum", true);


		if (PlayerPrefs.GetInt ("GetPhotoCount").Equals (0)) {
			/////Archive_14
//			#if UNITY_ANDROID
//			GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQDg", 0);
//			#elif UNITY_IOS
//			GameCenterManager.UpdateAchievement ("sally_achiv14", 100);
//			#elif UNITY_STANDALONE
//			SteamAchieveMgr.SetAchieve("sally_achiv14");
//			#endif
		}
		PlayerPrefs.SetInt("GetPhotoCount", PlayerPrefs.GetInt("GetPhotoCount") + 1);
	}
}
