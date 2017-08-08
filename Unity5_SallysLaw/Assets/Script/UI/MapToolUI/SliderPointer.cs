using UnityEngine;
using System.Collections;

public class SliderPointer : MonoBehaviour {
	bool m_bPressed;
	void OnPress(bool isDown)
	{
		if (isDown) {
			//Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			m_bPressed = true;

		} else {
			m_bPressed = false;
		}
	}

	void Update()
	{
		if (m_bPressed) {
			if(UICamera.mainCamera.ScreenToWorldPoint (Input.mousePosition).x > -0.46f && UICamera.mainCamera.ScreenToWorldPoint (Input.mousePosition).x < 0.46f)
				transform.position = new Vector3 (UICamera.mainCamera.ScreenToWorldPoint (Input.mousePosition).x, transform.position.y);
			else
			{
				if(UICamera.mainCamera.ScreenToWorldPoint (Input.mousePosition).x < -0.46f)
					transform.position = new Vector3(-0.46f, transform.position.y);
				else 
					transform.position = new Vector3(0.46f, transform.position.y);
			}
		}
	}
}
