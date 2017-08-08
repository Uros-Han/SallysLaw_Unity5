using UnityEngine;
using System.Collections;

public class SallyTracker : MonoBehaviour {

//	bool m_bTrackerOn;
	
	// Update is called once per frame
//	void LateUpdate () {
//		if (SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN) {
//			if (GameObject.Find ("TrackerContainer") != null) {
//				if (GameObject.Find ("Main Camera").GetComponent<CamMoveMgr> ().m_bRunnerInScreen) {
//					transform.GetChild (0).gameObject.SetActive (false);
//					m_bTrackerOn = false;
//				}
//			}
//
//			if (GameObject.Find ("TrackerContainer") == null) {
//				if (!GameObject.Find ("Main Camera").GetComponent<CamMoveMgr> ().m_bRunnerInScreen) {
//					transform.GetChild (0).gameObject.SetActive (true);
//					m_bTrackerOn = true;
//				}
//			}
//		} else {
//			m_bTrackerOn = false;
//			transform.GetChild (0).gameObject.SetActive (false);
//		}
//
//		if (m_bTrackerOn) {
//
//			float fTmpXPos, fTmpYPos = 0f;
//
//			fTmpXPos = GameObject.Find("Runner(Clone)").transform.position.x - GameObject.Find("Guardian(Clone)").transform.position.x;
//			fTmpYPos = GameObject.Find("Runner(Clone)").transform.position.y - GameObject.Find("Guardian(Clone)").transform.position.y;
//
//			if(fTmpXPos < -1.5f)
//				fTmpXPos = -1.5f;
//			else if(fTmpXPos > 1.5f)
//				fTmpXPos = 1.5f;
//
//			if(fTmpYPos < -3f)
//				fTmpYPos = -3f;
//			else if(fTmpYPos > 3f)
//				fTmpYPos = 3f;
//
//			transform.localPosition = new Vector2( fTmpXPos ,fTmpYPos);
//		}
//	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	public IEnumerator SallyTrackTracking()
	{
		CamMoveMgr camMoveMgr = GameObject.Find ("Main Camera").GetComponent<CamMoveMgr> ();

		Transform CamTrans = GameObject.Find ("Main Camera").transform;

		Transform runnerTrans = GameObject.Find("Runner(Clone)").transform;
		Transform guardianTrans = GameObject.Find("Guardian(Clone)").transform;
		transform.GetChild (0).gameObject.SetActive (true);

		float fWidthHalf = Camera.main.orthographicSize * Camera.main.aspect;
		float fHeightHalf = Camera.main.orthographicSize;

		fWidthHalf -= 0.5f;
		fHeightHalf -= 0.5f;

		do{

			if(runnerTrans == null)
				break;

			float fTmpXPos, fTmpYPos = 0f;
			
			fTmpXPos = runnerTrans.position.x - CamTrans.position.x;
			fTmpYPos = runnerTrans.position.y - CamTrans.position.y;
			
			if(fTmpXPos < - fWidthHalf)
				fTmpXPos = - fWidthHalf;
			else if(fTmpXPos > fWidthHalf )
				fTmpXPos = fWidthHalf;
			
			if(fTmpYPos < - fHeightHalf)
				fTmpYPos = -fHeightHalf;
			else if(fTmpYPos > fHeightHalf)
				fTmpYPos = fHeightHalf;


			transform.localPosition = new Vector3( fTmpXPos ,fTmpYPos, 5f);

			float fAngle = GetAngle(transform.position, runnerTrans.position) - 90f;
			transform.GetChild (0).GetChild(0).localPosition = Vector3.Normalize(runnerTrans.position - new Vector3(transform.position.x, transform.position.y, 0)) * 0.3f;
			transform.GetChild (0).GetChild(0).localRotation = Quaternion.AngleAxis(fAngle, Vector3.forward);
			transform.GetChild (0).GetChild(1).localRotation = runnerTrans.rotation;


			yield return new WaitForEndOfFrame();
		}while(!camMoveMgr.m_bRunnerInScreen);

		transform.GetChild (0).gameObject.SetActive (false);
	}


	public static float GetAngle (Vector3 vStart, Vector3 vEnd)
	{
		Vector3 v = vEnd - vStart;
		
		return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
	}

}
