using UnityEngine;
using System.Collections;

public class IconDepth : MonoBehaviour {
	UIPanel m_Panel;
	MapListMgr m_MapListMgr;

	GameObject m_scrollGrid;
	UILabel m_MapName;

	float m_fGap;

	// Use this for initialization
	void Start () {
		m_Panel = GetComponent<UIPanel> ();
		m_MapListMgr = GameObject.Find ("MapListMgr").GetComponent<MapListMgr> ();
		m_scrollGrid = GameObject.Find ("ScrollGrid").gameObject;
		m_MapName = GameObject.Find ("MapName").GetComponent<UILabel>();

		m_fGap = 0.1f;
		//스트레치 바뀌면 이거 바뀌어야함
		//스트레치 클수록 더 크게

		StartCoroutine (Depthing ());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}
	
	IEnumerator Depthing()
	{
		while (true) {

			if((transform.position.x / m_fGap) > 0)
				m_Panel.depth = 100 - (int)(transform.position.x / m_fGap);
			else
				m_Panel.depth = 100 + (int)(transform.position.x / m_fGap);

			if(m_Panel.depth < 100)
				transform.localScale = new Vector3(0.6f, 0.6f, 1);
			else
			{
				if(transform.position.x > 0)
					transform.localScale = new Vector3(1 - (transform.position.x * 0.7f), 1 - (transform.position.x * 0.7f), 1);
				else
					transform.localScale = new Vector3(1 + (transform.position.x * 0.7f), 1 + (transform.position.x * 0.7f), 1);

				Labeling();
			}


			yield return new WaitForEndOfFrame();
		}
	}

	void Labeling()
	{
		if(gameObject.name != "NewMapIcon")
		{
			for(int i = 0 ; i < m_scrollGrid.transform.childCount; ++i)
			{
				if(m_scrollGrid.transform.GetChild(i).gameObject == gameObject)
				{
					m_MapListMgr.m_strCurMap = m_MapListMgr.m_MapList[i-1];
					m_MapListMgr.m_strCurExtension = m_scrollGrid.transform.GetChild(i).GetChild(1).GetComponent<MapIcon>().m_strExtension;
					break;
				}
			}
		}else
			m_MapListMgr.m_strCurMap = "Create New";

		m_MapName.text = m_MapListMgr.m_strCurMap;
	}
}
