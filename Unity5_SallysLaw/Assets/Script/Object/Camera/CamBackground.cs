using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamBackground : MonoBehaviour
{

	public float m_fSpeed;
	Vector3 m_vecFirstPos;
	Vector3 m_vecBefore;
	Transform m_CamTransform;
	bool m_bInit;

	// Use this for initialization
	void Start ()
	{
		m_bInit = false;
		StartCoroutine (Moving ());
	}

	void OnDestroy ()
	{
		StopAllCoroutines ();
	}

	// Update is called once per frame
	IEnumerator Moving ()
	{

		while (GameObject.Find("Runner(Clone)") == null) {
			yield return null;
		}


		while (true) {

			if (!m_bInit && Camera.main.transform.localPosition.x != 0f) {
				m_CamTransform = Camera.main.transform;
				m_vecFirstPos = m_CamTransform.localPosition;
				transform.localPosition = Vector2.zero;
				m_bInit = true;
			}

			if (m_bInit) {

				if (Vector3.Distance (m_vecBefore, m_CamTransform.localPosition) > 0.01f) {
					transform.localPosition = new Vector2 (((m_CamTransform.localPosition.x - m_vecFirstPos.x) * m_fSpeed) * -1, transform.localPosition.y);

					if(gameObject.name.Equals("CloseBg"))
						transform.localPosition = new Vector2 (transform.localPosition.x, ((m_CamTransform.localPosition.y - m_vecFirstPos.y) * 1f) * -1f);
					else if(gameObject.name.Equals("MiddleBg"))
						transform.localPosition = new Vector2 (transform.localPosition.x, ((m_CamTransform.localPosition.y - m_vecFirstPos.y) * 1f) * -1f);
					else
						transform.localPosition = new Vector2 (transform.localPosition.x, ((m_CamTransform.localPosition.y - m_vecFirstPos.y) * 0.2f) * -1f);

					///이것때문에 스테이지에서 closeBg, farBg, MiddleBg 등등 , 로컬 포지션은 (0,0)되도록 세팅할것!!!
				}
				m_vecBefore = m_CamTransform.position;

			}

			yield return new WaitForEndOfFrame();
		}
	}
}
