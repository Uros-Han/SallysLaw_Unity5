using UnityEngine;
using System.Collections;

public class ClearBtn : MonoBehaviour {
	void Start()
	{
//		string LevelID = GameMgr.getInstance.m_strSelectedStage;
//		LevelID = LevelID.Substring (7);
//		iTempNextLevel = System.Convert.ToInt32 (LevelID) + 1;
//		iTempNextChpt = System.Convert.ToInt32(GameMgr.getInstance.m_strSelectedStage.Substring (5, 1));
//
//		if (iTempNextLevel % (GameMgr.getInstance.m_iChptStageNum[iTempNextChpt-1]+1) == 0) {
//			++iTempNextChpt;
//			iTempNextLevel = 1;
//		}
//		//LevelID = System.Convert.ToString(iTempNextLevel);
//		LevelID = string.Format ("{0:00}", iTempNextLevel);
//
//		m_strNextLevelName = "stage" + iTempNextChpt + "-" + LevelID;
//		GameMgr.getInstance.m_strSelectedStage = m_strNextLevelName;
//
//		if (iTempNextChpt > GameMgr.getInstance.m_iChptStageNum.Length || (iTempNextChpt <= GameMgr.getInstance.m_iChptStageNum.Length && GameMgr.getInstance.m_iChptStageNum [iTempNextChpt - 1] == 0)) { // 마지막 챕터 || 해당챕터의 스테이지갯수가 0
//			transform.parent.gameObject.SetActive(false);
//		}
	}

	void OnClick()
	{
		Application.LoadLevel ("Loading");
//		GameMgr.getInstance.m_iCurChpt = iTempNextChpt;
//		GameMgr.getInstance.m_iCurStage = iTempNextLevel;
		TimeMgr.Play ();
	}
}
