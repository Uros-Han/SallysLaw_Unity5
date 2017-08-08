using UnityEngine;
using System.Collections;

public class G_Door : MonoBehaviour {

	public float m_fDoorOpenSpeed;

	Vector3 m_vClosedPos;

	void Start () {
		m_fDoorOpenSpeed = 2.0f;
		m_vClosedPos = transform.localPosition;
	}

//	void OpenDoor()
//	{
//		if(transform.localPosition.y < 9.5f)
//			transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + m_fDoorOpenSpeed);
//	}

	public void OpenThisDoor()
	{
		StopCoroutine ("CloseDoor");
		StartCoroutine ("OpenDoor");
	}
	
	public void CloseThisDoor()
	{
		StopCoroutine ("OpenDoor");
		StartCoroutine ("CloseDoor");
	}

	public IEnumerator OpenDoor()
	{
		while (transform.localPosition.y < m_vClosedPos.y + 2.0f) {
			transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + m_fDoorOpenSpeed * Time.deltaTime);
			
			yield return null;
		}
		
		
		transform.localPosition = new Vector2(transform.localPosition.x, m_vClosedPos.y + 2.0f);
	}

	public IEnumerator CloseDoor()
	{
		while (transform.localPosition.y > m_vClosedPos.y) {
			transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - m_fDoorOpenSpeed * Time.deltaTime);
			
			yield return null;
		}
		
		transform.localPosition = new Vector2(transform.localPosition.x, m_vClosedPos.y);
	}

	public void InitDoor()
	{
		transform.localPosition = m_vClosedPos;
	}

}
