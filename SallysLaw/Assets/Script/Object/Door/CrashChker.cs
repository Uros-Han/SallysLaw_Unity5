using UnityEngine;
using System.Collections;

public class CrashChker : MonoBehaviour {


	void Start()
	{
		transform.localScale = new Vector2 (1, transform.parent.GetComponent<DoorPosFixer> ().m_fSize);
	}

	void OnCollisionEnter2D(Collision2D coll)
	{

		if (coll.gameObject.CompareTag("Runner")) {
			if(SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
			{
				if(transform.parent.GetComponent<R_Door>().m_bSwitchPressed.Equals(false))
				{
					AudioMgr.getInstance.PlaySfx(coll.transform.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.SALLY_DIE);
					SceneStatus.getInstance.m_enPlayerStatus = PLAYER_STATUS.SALLY_CRASH;
					coll.gameObject.GetComponent<Runner>().bExitRewinder = true;
					coll.gameObject.GetComponent<SkeletonAnimation>().state.SetAnimation(0,"danger", false);

					GameObject.Find("Guardian(Clone)").GetComponent<SkeletonAnimation>().state.SetAnimation(0,"danger",false);

					transform.parent.GetComponent<AudioSource>().Stop();

					GameMgr gMgr = GameMgr.getInstance;
					if(gMgr.m_iCurChpt == 1 && gMgr.m_iCurStage == 1 && gMgr.m_iCurAct == 1)
						UIManager.getInstance.FatherTutorial(false, 0);
					else if(gMgr.m_iCurChpt == 1 && gMgr.m_iCurStage == 1 && gMgr.m_iCurAct == 2)
					{
						UIManager.getInstance.FatherTutorial(false, 1);
						UIManager.getInstance.FatherTutorial(false, 2);
					}

					#if !UNITY_STANDALONE
					GameObject.Find("SidePanel").GetComponent<UIPanel>().alpha = 0f;
					#endif
					GameObject.Find ("FastForward").transform.GetChild (0).GetComponent<FastForwardBtn>().OnDisable();
					UIManager.getInstance.m_FastForwardPanel.GetComponent<UIPanel>().alpha = 0f;

					TimeMgr.SlowMotion();
				}
			}
		}
	}
}
