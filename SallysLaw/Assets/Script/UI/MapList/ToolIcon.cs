using UnityEngine;
using System.Collections;

public class ToolIcon : MonoBehaviour {
	MapListMgr m_MapListMgr;
	UISprite m_UISprite;
	float m_fChgColorSpeed;

	void Start () {
		m_fChgColorSpeed = 7.5f * Time.deltaTime;
		m_MapListMgr = GameObject.Find ("MapListMgr").GetComponent<MapListMgr> ();
		m_UISprite = GetComponent<UISprite> ();

		StartCoroutine (ScaleChging ());

		if(gameObject.name != "Tool")
			StartCoroutine (ColorChging ());
	}
	
	void OnDestroy()
	{
		StopAllCoroutines ();
	}
	
	IEnumerator ScaleChging()
	{
		while (true) {

			if((transform.position.x / 0.65f) > 1 || (transform.position.x / 0.65f) < -1 ){
				transform.localScale = new Vector3(0.6f, 0.6f, 1);

			}else{
				if(transform.position.x > 0)
					transform.localScale = new Vector3(1 - (transform.position.x * 0.7f), 1 - (transform.position.x * 0.7f), 1);
				else
					transform.localScale = new Vector3(1 + (transform.position.x * 0.7f), 1 + (transform.position.x * 0.7f), 1);
			}
			
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator ColorChging()
	{
		while (true) {

			if(m_MapListMgr.m_strCurMap != "Create New")
			{
				if(m_UISprite.color.a < 1)
					m_UISprite.color = new Color(1,1,1, m_UISprite.color.a + m_fChgColorSpeed);
				else
					m_UISprite.color = new Color(1,1,1,1);
			}else{
				if(m_UISprite.color.a > 0)
					m_UISprite.color = new Color(1,1,1, m_UISprite.color.a - m_fChgColorSpeed);
				else
					m_UISprite.color = new Color(1,1,1,0);
			}


			yield return new WaitForEndOfFrame();
		}
	}
}
