using UnityEngine;
using System.Collections;

public class LanguagePanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Transform tmpBtnTrans = null;
		switch (Localization.language) {
		case "한국어":
			tmpBtnTrans = transform.Find ("Korean").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
		case "English":
			tmpBtnTrans = transform.Find ("English").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
		case "中文简体":
			tmpBtnTrans = transform.Find ("Simplified Chinese").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
			
		case "中文繁體":
			tmpBtnTrans = transform.Find ("Traditional Chinese").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
		case "Deutsch":
			tmpBtnTrans = transform.Find ("German").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
		case "Español":
			tmpBtnTrans = transform.Find ("Spanish").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
		case "日本語":
			tmpBtnTrans = transform.Find ("Japanese").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
		}
	}
}
