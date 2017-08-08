using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IndexTag {
	public List<int> m_iIdx;
	public GameObject m_Object;
	public OBJECT_ID m_objID;

	public IndexTag(List<int> iIdx, GameObject obj, OBJECT_ID objID)
	{
		m_iIdx = iIdx;
		m_Object = obj;
		m_objID = objID;
	}

	public bool FindObject(GameObject obj)
	{
		if (m_Object == obj)
			return true;

		return false;
	}
}
