using UnityEngine;
using System.Collections;

public class DebugLine : MonoBehaviour {
	public bool m_bVertical;

	public void Init(Vector3 pos, bool bVertical,float fSize,int iCountX, int iCountY)
	{
		m_bVertical = bVertical;

		transform.position = pos;

		if(m_bVertical)
			transform.localScale = new Vector2 (2,fSize * 100 *iCountY);
		else
			transform.localScale = new Vector2 (fSize * 100 *iCountX,2);
	}
}
