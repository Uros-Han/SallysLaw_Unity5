using UnityEngine;
using System.Collections;

public class ColliderChker : MonoBehaviour {
	public bool m_bCollided;

	public bool m_bLeftDown;
	public bool m_bLeftUp;
	public bool m_bRightDown;
	public bool m_bRightUp;

	public void CheckCollide()
	{
		if(Physics2D.Raycast (transform.position, -Vector3.forward, 0.1f)){

			RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector3.forward, 0.1f);

			if(!m_bCollided && hit.transform.gameObject.name.Equals("PolyCollider(Clone)"))
				m_bCollided = true;
		}
	}
}
