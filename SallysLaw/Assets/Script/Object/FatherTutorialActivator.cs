using UnityEngine;
using System.Collections;

public class FatherTutorialActivator : MonoBehaviour {
	public enum TUTO {LEFT, RIGHT, FASTFORWARD};
	public TUTO m_Tuto;

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.name.Equals ("Guardian(Clone)") && SceneStatus.getInstance.m_enPlayerStatus.Equals (PLAYER_STATUS.GUARDIAN)) {
			if(GameMgr.getInstance.m_iCurChpt.Equals(1)&&GameMgr.getInstance.m_iCurStage.Equals(1)&&GameMgr.getInstance.m_iCurAct.Equals(2))
			{
				UIManager.getInstance.TutorialFade(true);
				Camera.main.GetComponent<CamMoveMgr>().m_camTutorial = CAM_TUTORIAL.ZOOM_OUT;

				if(GameMgr.getInstance.TutoWall != null)
				{
					GameMgr.getInstance.TutoWall.GetComponent<BoxCollider2D>().enabled = true;
					if(gameObject.name.Equals("FastForwardTutoActivator"))
						GameMgr.getInstance.TutoWall.transform.position = new Vector2(69.465f,-0.5f);
				}
			}

			GameObject.Find("Runner(Clone)").GetComponent<Runner>().m_bFatherTutorialOn = true;

			if (coll.gameObject.name.Equals ("Guardian(Clone)")) {
				switch(m_Tuto)
				{
				case TUTO.RIGHT:
					UIManager.getInstance.FatherTutorial (true, 0);
					break;

				case TUTO.LEFT:
					UIManager.getInstance.FatherTutorial (true, 1);
					break;

				case TUTO.FASTFORWARD:
					UIManager.getInstance.FatherTutorial (true, 2);
					break;
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.transform.name.Equals ("Guardian(Clone)")) {
			if(GameMgr.getInstance.m_iCurChpt.Equals(1)&&GameMgr.getInstance.m_iCurStage.Equals(1)&&GameMgr.getInstance.m_iCurAct.Equals(1))
				Camera.main.GetComponent<CamMoveMgr>().m_camTutorial = CAM_TUTORIAL.ZOOMING;
			else if(GameMgr.getInstance.m_iCurChpt.Equals(1)&&GameMgr.getInstance.m_iCurStage.Equals(1)&&GameMgr.getInstance.m_iCurAct.Equals(2))
			{
				Camera.main.GetComponent<CamMoveMgr>().m_camTutorial = CAM_TUTORIAL.NOT_TUTORIAL;
				if(GameMgr.getInstance.TutoWall != null)
					GameMgr.getInstance.TutoWall.GetComponent<BoxCollider2D>().enabled = false;
			}

			UIManager.getInstance.TutorialFade(false);
			switch(m_Tuto)
			{				
			case TUTO.RIGHT:
				UIManager.getInstance.FatherTutorial (false, 0);
				break;
				
			case TUTO.LEFT:
				UIManager.getInstance.FatherTutorial (false, 1);
				break;

			case TUTO.FASTFORWARD:
				UIManager.getInstance.FatherTutorial (false, 2);
				break;
			}

			GameObject.Find("Runner(Clone)").GetComponent<Runner>().m_bFatherTutorialOn = false;

			gameObject.SetActive(false);
		}
	}
}
