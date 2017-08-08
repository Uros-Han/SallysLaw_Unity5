using UnityEngine;
using System.Collections;

public class TweenActivator : MonoBehaviour {

	public bool m_bNotExecuteTPosition = false;
	public bool m_bNotExecuteTScale = false;
	public bool m_bNotExecuteTAlpha = false;
	public bool m_bNotExecuteTRotation = false;

	void TweenActivate(bool bPlay)
	{

		if (bPlay) {
			if(GetComponent<TweenPosition>() != null && !m_bNotExecuteTPosition)
				GetComponent<TweenPosition>().Play(true);
			if(GetComponent<TweenScale>() != null && !m_bNotExecuteTScale)
				GetComponent<TweenScale>().Play(true);
			if(GetComponent<TweenAlpha>() != null && !m_bNotExecuteTAlpha)
				GetComponent<TweenAlpha>().Play(true);
			if(GetComponent<TweenRotation>() != null && !m_bNotExecuteTRotation)
				GetComponent<TweenRotation>().Play(true);
		} else {
			if(GetComponent<TweenPosition>() != null && !m_bNotExecuteTPosition)
				GetComponent<TweenPosition>().Play(false);
			if(GetComponent<TweenScale>() != null && !m_bNotExecuteTScale)
				GetComponent<TweenScale>().Play(false);
			if(GetComponent<TweenAlpha>() != null && !m_bNotExecuteTAlpha)
				GetComponent<TweenAlpha>().Play(false);
			if(GetComponent<TweenRotation>() != null && !m_bNotExecuteTRotation)
				GetComponent<TweenRotation>().Play(false);
		}

	}
}
