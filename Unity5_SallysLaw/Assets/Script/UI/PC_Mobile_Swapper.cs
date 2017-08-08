using UnityEngine;
using System.Collections;

public class PC_Mobile_Swapper : MonoBehaviour {
	public bool m_bJustSwapByIdxActive;
	public bool m_bDisableSpriteInPC;
	// Use this for initialization
	void Start () {

		int iPCIdx = 0;
		for(int i = 0 ; i < transform.childCount; ++i)
		{
			if(transform.GetChild(i).gameObject.name.Contains("PC"))
			{
				iPCIdx = i;
				break;
			}
		}

		#if UNITY_STANDALONE || UNITY_WEBGL
		if(m_bJustSwapByIdxActive)
		{
			transform.GetChild(iPCIdx).gameObject.SetActive(true);
			transform.GetChild(1-iPCIdx).gameObject.SetActive(false);
		}else if(m_bDisableSpriteInPC){
			GetComponent<UISprite>().enabled = false;
		}else{
			switch(gameObject.name){

			case "RightTop":
				UIAnchor anchor = GetComponent<UIAnchor>();
				anchor.side = UIAnchor.Side.BottomRight;
				anchor.relativeOffset = new Vector2(-0.055f, 0.08f);
				anchor.enabled = true;
				break;

			case "RightBottom":
				gameObject.SetActive(false);
				break;

			}
		}
#else
		if(m_bJustSwapByIdxActive)
		{
			transform.GetChild(iPCIdx).gameObject.SetActive(false);
			transform.GetChild(1-iPCIdx).gameObject.SetActive(true);
		}
#endif
	}

}
