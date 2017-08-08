using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveOrder : MonoBehaviour
{

	float m_fSpeed;
	float m_fAccel;
	float m_fCurSpeed;
	bool m_bReachMaxSpeed;
	float m_fReachMaxSpeedDistance;


	public bool m_bToDest;
	public Vector2 DestPos;
	public Vector2 OrigPos;
	public Vector3 DirectPos;
	List<Vector2> m_ListPos;
	public int m_iIdx;
	bool m_bFlashBackBreakOn;
	public GameObject m_objOwner;
	public bool m_bPlay;

	SceneStatus m_SceneStatus;

	public void Start ()
	{

		
		DestPos = transform.position;
		OrigPos = m_objOwner.transform.position;
		
		DirectPos = Vector3.Normalize (DestPos - OrigPos);
		m_SceneStatus = GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ();

		if (m_ListPos != null) 
			m_ListPos.Clear ();
		
		m_ListPos = new List<Vector2> ();
		m_iIdx = 0;

		StartCoroutine (RunnerStartWaiting ());
		StartCoroutine (Player ());

	}

	// Use this for initialization
//	public void Init ()
//	{
//		Start ();
//
//		if (m_ListPos != null) 
//			m_ListPos.Clear ();
//
//		m_ListPos = new List<Vector2> ();
//		m_iIdx = 0;
//		m_iFlashBackIdx = 0;
//
//		StartCoroutine (Player ());
//	}

	public void MoveToOrigPos(){
		m_objOwner.transform.position = OrigPos;

		m_fCurSpeed = 0;
		m_bToDest = true;
		m_bReachMaxSpeed = false;
	}

	void OnDestroy()
	{
		StopAllCoroutines ();

		if (GameObject.Find ("MiniButtons") != null) {
			for(int i = 0; i < GameObject.Find ("MiniButtons").transform.childCount; ++i)
			{
				if(GameObject.Find ("MiniButtons").transform.GetChild(i).GetComponent<UIFollowTarget>().target == transform)
				{
					Destroy (GameObject.Find ("MiniButtons").transform.GetChild(i).gameObject);
					break;
				}
			}
		}
	}

	IEnumerator RunnerStartWaiting()
	{
		yield return null;
//		do{
//			yield return null;
//		}while(m_SceneStatus.m_enPlayerStatus == PLAYER_STATUS.LOOK_AROUND);

		if (m_objOwner != null)
			m_objOwner.transform.position = OrigPos;
		else
			Debug.LogError ("Cant find Owner");
	}

	// Update is called once per frame
	IEnumerator Player ()
	{

		m_fSpeed = 37.5f; // 37.5, 47.5, 57.5
		m_fAccel = m_fSpeed / (100f - (m_fSpeed * 1.1f));
		m_fCurSpeed = 0;
		m_bToDest = true;
		m_bReachMaxSpeed = false;

		float m_fOverallDistance = Vector3.Distance(DestPos, OrigPos);

		while (m_bPlay) {
			if (m_SceneStatus.m_enPlayerStatus == PLAYER_STATUS.RUNNER || m_SceneStatus.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN ) {

				m_objOwner.GetComponent<Rigidbody2D> ().velocity = DirectPos * m_fCurSpeed * Time.deltaTime;

				if (m_bToDest) {

					if(Vector3.Distance (m_objOwner.transform.position, DestPos) < m_fReachMaxSpeedDistance)
					{
						m_fCurSpeed -= m_fAccel;
					}else{
						if(!m_bReachMaxSpeed)
							m_fCurSpeed += m_fAccel;
						else
							m_fCurSpeed = m_fSpeed;
						
						if(!m_bReachMaxSpeed && (m_fCurSpeed > m_fSpeed || Vector3.Distance(m_objOwner.transform.position, DestPos) < m_fOverallDistance/2f))
						{
							m_fCurSpeed = m_fSpeed;
							m_bReachMaxSpeed = true;
							m_fReachMaxSpeedDistance = Vector3.Distance(m_objOwner.transform.position, OrigPos);
							Debug.Log (m_fReachMaxSpeedDistance);
						}

					}

					if (Vector3.Distance (m_objOwner.transform.position, DestPos) < 0.01f)
					{
						m_bToDest = false;
						m_fCurSpeed = 0;
						m_bReachMaxSpeed = false;
					}
				} else {

					if(Vector3.Distance (m_objOwner.transform.position, OrigPos) < m_fReachMaxSpeedDistance)
					{
						m_fCurSpeed += m_fAccel;
					}else{
						if(!m_bReachMaxSpeed)
							m_fCurSpeed -= m_fAccel;
						else
							m_fCurSpeed = -m_fSpeed;


						
						if(!m_bReachMaxSpeed && (m_fCurSpeed < -m_fSpeed || Vector3.Distance(m_objOwner.transform.position, OrigPos) < m_fOverallDistance/2f))
						{
							m_fCurSpeed = -m_fSpeed;
							m_bReachMaxSpeed = true;
							m_fReachMaxSpeedDistance = Vector3.Distance(DestPos, m_objOwner.transform.position);
							Debug.Log (m_fReachMaxSpeedDistance);
						}
					}

					if (Vector3.Distance (m_objOwner.transform.position, OrigPos) < 0.01f)
					{
						m_bToDest = true;
						m_fCurSpeed = 0;
						m_bReachMaxSpeed = false;
					}
				}

				if (Time.timeScale == 1 )
					m_ListPos.Add (m_objOwner.transform.position);

			} else if (m_SceneStatus.m_enPlayerStatus == PLAYER_STATUS.FLASHBACK) {
				if (m_ListPos.Count - 1 - m_SceneStatus.m_iFlashBackIdx > 0) {
					if (Time.timeScale == 1) {

						m_objOwner.transform.position = m_ListPos [m_ListPos.Count - 1 - m_SceneStatus.m_iFlashBackIdx];

					}
				}else{ // END FLASHBACK
					if (m_objOwner != null)
						m_objOwner.transform.position = OrigPos;

					m_fCurSpeed = 0;
					m_bToDest = true;
					m_bReachMaxSpeed = false;

				}

				m_bToDest = true;
			}

			yield return null;
		} 

		if (m_objOwner != null)
			m_objOwner.transform.position = OrigPos;

		m_objOwner.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}
}
