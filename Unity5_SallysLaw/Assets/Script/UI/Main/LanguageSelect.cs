using UnityEngine;
using System.Collections;

public class LanguageSelect : MonoBehaviour {
	
	public bool m_bSelected;
	public FontType m_FontType;
	
	void Start()
	{
		
	}
	
	void OnClick()
	{
		Transform parent = transform.parent.parent;
		
		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);
		
		if (!m_bSelected) {
			
			for (int i = 0; i < parent.childCount; ++i) {
				
				LanguageSelect langSel = parent.GetChild(i).GetChild(0).GetComponent<LanguageSelect>();
				
				if(langSel.m_bSelected) //before selected object
				{
					langSel.m_bSelected = false;
					langSel.ChgColor(false);
				}
			}
			
			
			Localization.language = transform.parent.gameObject.name;
			
			for(int i = 0 ; i < Localization.knownLanguages.Length; ++i)
			{
				if(transform.parent.gameObject.name == Localization.knownLanguages[i])
				{
					PlayerPrefs.SetInt ("Lang", i);
					break;
				}
			}
			
			UIRoot.Broadcast("OnLocalize");
			
			ChgColor(true);
			m_bSelected = true;
		}
		
		FontType tmpFont = FontType.END;
		switch (m_FontType) {
		case FontType.SystemFont:
			tmpFont = FontType.SystemFont;
			break;
			
		case FontType.Londrina:
			tmpFont = FontType.Londrina;
			break;
			
		case FontType.Jua:
			tmpFont = FontType.Jua;
			break;

		case FontType.GenJyuu:
			tmpFont = FontType.GenJyuu;
			break;
		}
		
		GameObject.Find ("UI Root").BroadcastMessage ("SwitchFont", tmpFont, SendMessageOptions.DontRequireReceiver);
	}
	
	public void ChgColor(bool bSelected)
	{
		if (bSelected) {
			transform.parent.GetChild(1).GetComponent<UILabel>().color = Color.white;
			GetComponent<UISprite> ().color = new Color (49 / 255f, 83 / 255f, 56 / 255f);
		} else {
			transform.parent.GetChild(1).GetComponent<UILabel>().color = new Color (49 / 255f, 83 / 255f, 56 / 255f);
			GetComponent<UISprite> ().color = new Color (1, 1, 1, 0.5f);
		}
	}
}