using UnityEngine;
using System.Collections;

public class MoveLine : MonoBehaviour {

	public float m_fDistance;
	public float m_fAngle;

	// Use this for initialization
	void Start () {
		m_fDistance = Vector3.Distance (transform.parent.position, transform.parent.gameObject.GetComponent<Portal>().m_objOwner.transform.position);


		Vector3 relative = transform.parent.InverseTransformPoint (transform.parent.gameObject.GetComponent<Portal>().m_objOwner.transform.position);
		m_fAngle = Mathf.Atan2 (relative.x, relative.y) * Mathf.Rad2Deg;
		transform.Rotate (0, 0, m_fAngle * -1);



		transform.position = transform.parent.position + transform.parent.gameObject.GetComponent<Portal> ().m_objOwner.transform.position;
		transform.position = new Vector2 (transform.position.x/2.0f, transform.position.y/2.0f);

		transform.localScale = new Vector2 (2, m_fDistance * 100.0f);
	}

}
