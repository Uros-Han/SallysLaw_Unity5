using UnityEngine;
using System.Collections;

public class SelStageBtn : MonoBehaviour {

	void OnClick()
	{
		if(GetComponent<UISprite>().spriteName == "button_stage")
			StartCoroutine (Delay ());
	}

	IEnumerator Delay()
	{
		yield return new WaitForSeconds (0.1f);
		GameMgr.getInstance.m_strSelectedStage = "stage" + transform.parent.parent.gameObject.name + transform.parent.gameObject.name ;
		GameMgr.getInstance.m_iCurStage = System.Convert.ToInt32(transform.parent.name);
		Application.LoadLevel ("Loading");
	}
}
