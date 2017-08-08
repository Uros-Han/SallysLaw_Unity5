using UnityEngine;
using System.Collections;

public class LanguagePanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Transform tmpBtnTrans = null;
		switch (Localization.language) {
		case "Korean":
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
			
		case "SChinese":
			tmpBtnTrans = transform.Find ("SChinese").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
			
		case "TChinese":
			tmpBtnTrans = transform.Find ("TChinese").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
		case "German":
			tmpBtnTrans = transform.Find ("German").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
		case "Spanish":
			tmpBtnTrans = transform.Find ("Spanish").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
			
		case "Japanese":
			tmpBtnTrans = transform.Find ("Japanese").transform;
			tmpBtnTrans.GetChild (0).GetComponent<LanguageSelect> ().m_bSelected = true;
			tmpBtnTrans.GetChild (0).GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f, 1f);
			tmpBtnTrans.GetChild (1).GetComponent<UILabel> ().color = Color.white;
			break;
		}
	}
}
