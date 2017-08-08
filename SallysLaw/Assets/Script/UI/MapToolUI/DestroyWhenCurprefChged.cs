using UnityEngine;
using System.Collections;

public class DestroyWhenCurprefChged : MonoBehaviour {

	public GameObject m_ObjHaveToBePref;
	public bool m_bWantDestroyThis;
	
	public bool m_bThisIsObject;

	MapToolMgr m_MapToolMgr;
	string m_strPrefName;

	// Use this for initialization
	void OnEnable () {
		if (GameObject.Find ("MapToolMgr") != null)
			m_MapToolMgr = GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ();
		else
			Destroy (GetComponent<DestroyWhenCurprefChged> ());

		if (m_ObjHaveToBePref.name.IndexOf ('(') != -1)
			m_strPrefName = m_ObjHaveToBePref.name.Substring (0, m_ObjHaveToBePref.name.IndexOf ('('));
		else
			m_strPrefName = m_ObjHaveToBePref.name;

		StartCoroutine (Looping ());
	}

	void OnDisable()
	{
		StopAllCoroutines ();
	}
	
	// Update is called once per frame
	IEnumerator Looping () {
		while (true) {
			yield return new WaitForEndOfFrame();



			if(m_strPrefName != m_MapToolMgr.m_CurPref.name)
			{
				//확정체크안하구 취소해버렷을때 현재 그리드인덱스 자리차지안하도록 delete
				if(m_bThisIsObject)
				{
					for(int i = 0; i <MapToolMgr.getInstance.m_listObjects.Count; ++i)
					{
						if(MapToolMgr.getInstance.m_listObjects[i].m_iIdx[0] == GridMgr.getInstance.GetGridIdx(transform.position))
							MapToolMgr.getInstance.m_listObjects.RemoveAt(i);
					}
				}

				if(m_bWantDestroyThis)
					Destroy(gameObject);
				else
				{
					gameObject.SetActive(false);
					GetComponent<DestroyWhenCurprefChged> ().enabled = false;
				}
			}

		}
	}
}
