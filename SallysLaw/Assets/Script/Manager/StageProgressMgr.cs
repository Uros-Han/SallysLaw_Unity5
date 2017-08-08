using UnityEngine;
using System.Collections;

public class StageProgressMgr : MonoBehaviour {

	Transform ChaptersTrans;
	GameMgr gMgr;

	// Use this for initialization
	void Start () {
		UpdateStages ();
	}

	public void UpdateStages() 
	{
		gMgr = GameMgr.getInstance;
		ChaptersTrans = GameObject.Find ("Chapters").transform;

		if (!gMgr.m_bDevMode_AllClear) {
			for (int i = 0; i < gMgr.m_iOpenedChpt - 1; ++i) {
				for (int j = 0; j < 6; ++j) {
					StageUnLock (ChaptersTrans.GetChild (i).GetChild (2).GetChild (j));
				}
			}

			for (int i = 0; i < gMgr.m_iOpenedStage; ++i) {
				StageUnLock (ChaptersTrans.GetChild (gMgr.m_iOpenedChpt - 1).GetChild (2).GetChild (i));
			}

			Transform chapters = GameObject.Find("Chapters").transform;
			for(int i = 0 ; i < gMgr.m_iOpenedChpt; ++i)
			{
				chapters.GetChild(i).GetChild(0).GetChild(0).GetComponent<UITexture>().enabled = false;
			}
			if(PlayerPrefs.GetInt("AllClear").Equals(1))
				chapters.parent.GetChild(2).GetChild(1).GetChild(0).GetComponent<UITexture>().enabled = false;

		} else {
			for (int i = 0; i < 5; ++i) {
				for (int j = 0; j < 6; ++j) {
					StageUnLock (ChaptersTrans.GetChild (i).GetChild (2).GetChild (j));
				}
			}

			Transform chapters = GameObject.Find("Chapters").transform;
			for(int i = 0 ; i < 5; ++i)
			{
				chapters.GetChild(i).GetChild(0).GetChild(0).GetComponent<UITexture>().enabled = false;
			}
			chapters.parent.GetChild(2).GetChild(1).GetChild(0).GetComponent<UITexture>().enabled = false;
		}
	}

	void StageUnLock(Transform stageTrans)
	{
		stageTrans.GetChild(0).GetComponent<UISprite>().spriteName = "circle_full";
		stageTrans.GetChild (0).GetChild (0).GetChild (0).GetComponent<UILabel> ().color = stageTrans.GetChild (0).GetChild (0).GetChild (2).GetComponent<UISprite> ().color;
	}
}
