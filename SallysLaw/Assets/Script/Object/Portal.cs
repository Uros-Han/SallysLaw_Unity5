using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
	public GameObject m_objOwner;
	public Color m_color;


	private SkeletonAnimation skelAnim;


	int iPortalEnterCount;

	bool m_bGuardianTeleportToHere = false;

	// Use this for initialization
	void Start () {
		skelAnim = GetComponent<SkeletonAnimation> ();
	}


	void OnDestroy()
	{

		if (GameObject.Find ("MiniButtons") != null) {
			for(int i = 0; i < GameObject.Find ("MiniButtons").transform.childCount; ++i)
			{
				if(GameObject.Find ("MiniButtons").transform.GetChild(i).GetComponent<UIFollowTarget>().target.Equals(transform))
				{
					Destroy (GameObject.Find ("MiniButtons").transform.GetChild(i).gameObject);
					break;
				}
			}
		}

		StopAllCoroutines ();
	}

	public void GuardianToHere()
	{
		m_bGuardianTeleportToHere = true;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.name.Equals("Guardian(Clone)") && SceneStatus.getInstance.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN)) {

			if(!m_bGuardianTeleportToHere) //포탈 안타고 왓을때 (걸어왓을때)
			{
				if (GameObject.Find ("MapToolMgr") == null || (GameObject.Find ("MapToolMgr") != null && GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bNowPlaying)) {
					if(m_objOwner != null)
					{
						m_objOwner.GetComponent<Portal>().GuardianToHere();
						coll.GetComponent<TrailRenderer>().time = 0f;
						coll.transform.position = m_objOwner.transform.position;

						skelAnim.state.SetAnimation(0, "portal_enter", false);
						skelAnim.state.AddAnimation(0, "portal_idle", true, 0);

						AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "portal", 0);
						AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "portal", 1);

						Camera.main.GetComponent<CamMoveMgr>().m_bGuardianInCenter = true;
						Camera.main.GetComponent<CamMoveMgr>().m_bFromPortal = true;

						iPortalEnterCount += 1;

						if(iPortalEnterCount.Equals(10))
						{
							/////Archive_16
//							#if UNITY_ANDROID
//							GameCenterManager.UpdateAchivement_ForAndroid("CgkIy-L3tPYMEAIQEA", 0);
//							#elif UNITY_IOS
//							GameCenterManager.UpdateAchievement ("sally_achiv16", 100);
//							#elif UNITY_STANDALONE
//							SteamAchieveMgr.SetAchieve("sally_achiv16");
//							#endif
						}
					}
				}
			}else{ //포탈 타고 왓을때
				coll.GetComponent<Guardian>().PortalSickEnable();
				coll.GetComponent<TrailRenderer>().time = 0.25f;

				skelAnim.state.SetAnimation(0, "portal_enter", false);
				skelAnim.state.AddAnimation(0, "portal_idle", true, 0);

				AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "portal", 2);
			}

		}
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (m_bGuardianTeleportToHere && coll.transform.name.Equals("Guardian(Clone)")) {
			
			m_bGuardianTeleportToHere = false;
		}
	}
}
