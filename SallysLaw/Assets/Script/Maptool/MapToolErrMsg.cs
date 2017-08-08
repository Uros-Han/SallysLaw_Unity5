using UnityEngine;
using System.Collections;

public class MapToolErrMsg : MonoBehaviour {

	Color m_LabelColor;

	void OnEnable()
	{
		m_LabelColor = Color.white;

		if (!GameObject.Find ("StageLoader").GetComponent<StageLoader> ().m_bMaptool && !GameObject.Find ("StageLoader").GetComponent<StageLoader> ().m_bStageLoader) {
			string strLevel = GameMgr.getInstance.m_strSelectedStage;
			strLevel = strLevel.Substring(5,4);

			GetComponent<UILabel>().text = Localization.Get("Chapter")+ " " + strLevel.Substring(0,1) + System.Environment.NewLine + Localization.Get("Stage")+ " " + strLevel.Substring(2,2);

			m_LabelColor = Color.black;
		}
	}

	void Update () {
		if(GetComponent<UILabel>().alpha < 0.05f)
			Destroy(gameObject);

		GetComponent<UILabel> ().color = new Color (GetComponent<UILabel> ().color.r, GetComponent<UILabel> ().color.g, GetComponent<UILabel> ().color.b, GetComponent<UILabel> ().color.a - 0.5f* Time.deltaTime);
	}
}
