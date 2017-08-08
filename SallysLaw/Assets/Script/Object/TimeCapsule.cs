using UnityEngine;
using System.Collections;

public class TimeCapsule : MonoBehaviour {

	SceneStatus m_SceneStatus;
	float fBeforeWaitTime;

	SkeletonAnimation skelAnim;

	void Start()
	{
		m_SceneStatus = GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ();
		StartCoroutine (FindRunner ());

		skelAnim = GetComponent<SkeletonAnimation> ();
	}

	IEnumerator FindRunner()
	{
		do{
			yield return null;
		}while(GameObject.Find ("Runner(Clone)").GetComponent<Runner> () == null);

		fBeforeWaitTime = GameObject.Find ("Runner(Clone)").GetComponent<Runner> ().m_fWaitTime;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.name.Equals("Guardian(Clone)")) {
			if(m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN) && skelAnim.loop.Equals(true))
			{
				//GameObject.Find("Main Camera").GetComponent<CamMoveMgr>().EatTimeBall(gameObject);

				AudioMgr.getInstance.PlaySfx(coll.GetComponent<AudioSource>(), "bundle", (int)SOUND_LIST.EAT_TIMECAPSULE);

				GameObject.Find("Runner(Clone)").GetComponent<Runner>().m_fWaitTime = 0;

				ResetTimer();

				skelAnim.loop = false;
				skelAnim.AnimationName = "get";

				StartCoroutine(Timer ());
			}
		}
	}

	void ResetTimer()
	{
		Transform TimeBallParent = GameObject.Find ("Timeballs").transform;

		for(int i = 0 ; i < TimeBallParent.childCount; ++i)
		{
			if(TimeBallParent.GetChild(i).GetComponent<SkeletonAnimation>().loop.Equals(false))
				TimeBallParent.GetChild(i).GetComponent<TimeCapsule>().StopAllCoroutines();
		}
	}

	public void BackToBeforeWaitTime()
	{
		GameObject.Find ("Runner(Clone)").GetComponent<Runner> ().m_fWaitTime = fBeforeWaitTime;
	}

	IEnumerator Timer()
	{

		float fTimer = 0f;

//		do {
//			fTimer += Time.deltaTime;
//			Debug.Log(fTimer);
//			yield return null;
//		} while(fTimer < 3f);

		yield return StartCoroutine(waitForMysecond (3f));

		if(m_SceneStatus.m_enPlayerStatus.Equals(PLAYER_STATUS.GUARDIAN))
			BackToBeforeWaitTime ();
	}

	IEnumerator waitForMysecond(float fTime)
	{
		float fTmpTime = 0f;

		do{
			if(TimeMgr.m_bFastForward)
				fTmpTime += (Time.unscaledDeltaTime * 3f);
			else
				fTmpTime += Time.unscaledDeltaTime;

			yield return null;
		}while(fTmpTime < fTime);

	}
}
