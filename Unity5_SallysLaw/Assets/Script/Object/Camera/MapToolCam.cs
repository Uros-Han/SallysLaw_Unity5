using UnityEngine;
using System.Collections;

public class MapToolCam : MonoBehaviour {
	float m_fSpeed;

	float m_fMaxXPos;
	float m_fSliderFixer;
	// Use this for initialization
	void OnEnable () {
		m_fSpeed = 2.0f;
		GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		m_fMaxXPos = 0.19f * GridMgr.getInstance.m_iXcount;

		m_fSliderFixer = 50f/m_fMaxXPos;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.A)) {
			if(transform.localPosition.x > -m_fMaxXPos)
				transform.localPosition = new Vector3 (transform.localPosition.x - (m_fSpeed * Time.deltaTime), transform.localPosition.y, -1);
			else
				transform.localPosition = new Vector3 (-m_fMaxXPos, transform.localPosition.y);

			GameObject.Find("SliderPointer").transform.localPosition = new Vector2(transform.localPosition.x * m_fSliderFixer, transform.localPosition.y * m_fSliderFixer);
		}
		if (Input.GetKey (KeyCode.D)) {
			if(transform.localPosition.x < m_fMaxXPos)
				transform.localPosition = new Vector3 (transform.localPosition.x + (m_fSpeed * Time.deltaTime), transform.localPosition.y, -1);
			else
				transform.localPosition = new Vector3 (m_fMaxXPos, transform.localPosition.y);
			
			GameObject.Find("SliderPointer").transform.localPosition = new Vector2(transform.localPosition.x * m_fSliderFixer, transform.localPosition.y * m_fSliderFixer);

		}


		transform.localPosition = new Vector3(GameObject.Find("SliderPointer").transform.localPosition.x/m_fSliderFixer, 0 ,-10);
	}
}
