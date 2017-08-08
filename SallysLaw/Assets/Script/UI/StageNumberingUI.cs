using UnityEngine;
using System.Collections;

public class StageNumberingUI : MonoBehaviour {

	// Use this for initialization
	void Labeling () {
		GameMgr gameMgr = GameMgr.getInstance;

		if (!GameObject.Find ("StageLoader").GetComponent<StageLoader> ().m_bStageLoader) { // not StagePlayer

			transform.Find("ChptTitle").GetComponent<UILabel>().text = Localization.Get("Chpt" + gameMgr.m_iCurChpt);
			transform.Find("StageNumber").GetComponent<UILabel>().text = string.Format("{0}",gameMgr.m_iCurStage);

			string stageTitle;
			string tmpTitle;

			stageTitle = "Title"+ gameMgr.m_iCurChpt +"-" + gameMgr.m_iCurStage;
			tmpTitle = Localization.Get("Title"+ gameMgr.m_iCurChpt + "-" + gameMgr.m_iCurStage);


			if(!stageTitle.Equals(tmpTitle))
				stageTitle = tmpTitle;
			else
				stageTitle = "";

			if(stageTitle != null)
				transform.Find("StageTitle").GetComponent<UILabel>().text = stageTitle;


		} else { // this is StagePlayer
//			GetComponent<UILabel> ().text = GameObject.Find("MapListMgr").GetComponent<MapListMgr>().m_strCurMap;
		}
	}

}
