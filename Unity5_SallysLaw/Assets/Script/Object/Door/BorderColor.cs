using UnityEngine;
using System.Collections;

public class BorderColor : MonoBehaviour {

	bool m_bToInvisible;
	SpriteRenderer m_renderer;
	float m_fColorChgSpeed;
	Color m_color;

	void OnEnable()
	{
		//m_color = GetComponent<SpriteRenderer> ().color;
		m_bToInvisible = true;
		m_renderer = GetComponent<SpriteRenderer> ();
		m_fColorChgSpeed = 2.0f;

		Scaling ();
	}

	void Update()
	{
		//Color ();

	}

	void Scaling()
	{
		transform.GetChild(0).localScale = new Vector2(5, transform.parent.GetChild(0).localScale.y * 50f);
		transform.GetChild(1).localPosition = new Vector2(0, transform.parent.GetChild(0).localScale.y * 0.25f);
		transform.GetChild(2).localScale = new Vector2(5, transform.parent.GetChild(0).localScale.y * 50f);
		transform.GetChild(3).localPosition = new Vector2(0, - transform.parent.GetChild(0).localScale.y * 0.25f);
	}

	void Color()
	{
		if (m_bToInvisible) {
			if(m_renderer.color.a > 0)
			{
				m_renderer.color = new Color(m_color.r, m_color.g, m_color.b,m_renderer.color.a - (m_fColorChgSpeed * Time.deltaTime));
			}else{
				m_bToInvisible = false;
				m_renderer.color = new Color(m_color.r, m_color.g, m_color.b,0);
			}
		} else {
			if(m_renderer.color.a < 1)
			{
				m_renderer.color = new Color(m_color.r, m_color.g, m_color.b,m_renderer.color.a + (m_fColorChgSpeed * Time.deltaTime));
			}else{
				m_bToInvisible = true;
				m_renderer.color = new Color(m_color.r, m_color.g, m_color.b,1);
			}
		}
	}
}
