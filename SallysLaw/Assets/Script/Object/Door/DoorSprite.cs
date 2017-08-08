using UnityEngine;
using System.Collections;

public class DoorSprite : MonoBehaviour {
	public int m_iSpriteIdx;
	public int m_iDoorIdx; //0 = bottom -> last = top

	bool m_bSpriteHided;

	bool bStart;

	void Start()
	{
		bStart = true;
		StartCoroutine (Masking ());
	}

	void OnEnable()
	{
		if(bStart)
			StartCoroutine (Masking ());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	void OnDisable()
	{
		StopAllCoroutines ();
	}

	public void Init()
	{
		GetComponent<SpriteRenderer> ().enabled = true;
		m_bSpriteHided = false;
	}

	IEnumerator Masking()
	{
		m_bSpriteHided = false;
		float fSize = transform.parent.parent.GetComponent<DoorPosFixer> ().m_fSize;
		float fOriginYPos = transform.position.y;

		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();

		float fHiddenYPos = fOriginYPos + (0.5f * (fSize - m_iDoorIdx));

		do {

			if(!m_bSpriteHided && transform.position.y > fHiddenYPos)
			{
				m_bSpriteHided = true;
				sprite.enabled = false;
			}else if(m_bSpriteHided && transform.position.y < fHiddenYPos){

				m_bSpriteHided = false;
				sprite.enabled = true;
			}


			yield return null;
		} while(true);
	}
}
