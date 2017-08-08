using UnityEngine;
using System.Collections;

public class SelectStageScene : MonoBehaviour {
	GameObject m_objPressed;
	float m_fPressTime;
	// Use this for initialization
	void Start () {
		if(GameObject.Find("GameMgr") == null) //if gameMgr doesn't exist, make one.
		{
			GameObject gameMgr = Instantiate(Resources.Load("Prefabs/GameMgr") as GameObject) as GameObject;
			gameMgr.name = gameMgr.name.Replace("(Clone)","");	
		}


		StartCoroutine (CurChapterChecker ());

		Time.timeScale = 1.0f;
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	IEnumerator ChpterCount()
	{
		yield return new WaitForEndOfFrame ();

//		GameMgr.getInstance.m_iChptStageNum[0] = ChapterStageCounter(1);
//		GameMgr.getInstance.m_iChptStageNum[1] = ChapterStageCounter(2);
//		GameMgr.getInstance.m_iChptStageNum[2] = ChapterStageCounter(3);
//		GameMgr.getInstance.m_iChptStageNum[3] = ChapterStageCounter(4);
//		GameMgr.getInstance.m_iChptStageNum[4] = ChapterStageCounter(5);

	}

	// Update is called once per frame
	void Update () {

		Vector3 vMousePos = UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,-10);
		RaycastHit[] allHit = Physics.RaycastAll (vMousePos, Vector3.forward);

		if (Input.GetMouseButton (0)) {
			if (allHit.Length > 0) {
			
				foreach (RaycastHit hit in allHit) {
					if (hit.transform.name == "Box")
						m_fPressTime += Time.deltaTime;


				}
			}
		}//

		if (Input.GetMouseButtonUp (0)) {
			if (allHit.Length > 0) {
				
				foreach (RaycastHit hit in allHit) {
					if (hit.transform.name == "Box" && m_fPressTime < 0.15f)
						SendMessage("OnClick", hit.transform.gameObject);
				}
			}
		}
	}


	IEnumerator CurChapterChecker()
	{
		SpringPanel spring;

		do {

			spring = GameObject.Find ("Scroll").GetComponent<SpringPanel> ();

			yield return null;
		} while(spring == null);

		bool bBefoeEnabled = false;
		float fBeforeTargetX = 0f;

		float fWideLength = 1800f;

		do{
			yield return null;

			if(spring.enabled && !bBefoeEnabled) //스프링 활성화 됫을때
			{

				if(fBeforeTargetX - 1700f > spring.target.x)
				{
					GameObject.Find("Background").transform.GetChild(GameMgr.getInstance.m_iCurChpt).SendMessage("TweenActivate",false);
					GameMgr.getInstance.m_iCurChpt += 1;
					GameObject.Find("Background").transform.GetChild(GameMgr.getInstance.m_iCurChpt).SendMessage("TweenActivate",true);
				}
				else if(fBeforeTargetX + 1700f < spring.target.x)
				{
					GameObject.Find("Background").transform.GetChild(GameMgr.getInstance.m_iCurChpt).SendMessage("TweenActivate",false);
					GameMgr.getInstance.m_iCurChpt -= 1;
					GameObject.Find("Background").transform.GetChild(GameMgr.getInstance.m_iCurChpt).SendMessage("TweenActivate",true);
				}

				fBeforeTargetX = spring.target.x;

			}


			bBefoeEnabled = spring.enabled;

		}while(true);
	}

	int ChapterStageCounter(int iChpater)
	{
		int iCounter = 0;
//		GameObject ChpterObj = GameObject.Find (iChpater + "-").gameObject;
		GameObject ChpterObj = GameObject.Find (iChpater + "-").gameObject;

		Transform Stages = GameObject.Find (iChpater + "-").transform.GetChild(2);

//		for (int i = 0; i < ChpterObj.transform.childCount; ++i) {
//			if(ChpterObj.transform.GetChild(i).GetChild(0).GetComponent<UISprite>().spriteName == "button_stage")
//				iCounter += 1;
//		}
		for (int i = 0; i < Stages.childCount - 1; ++i) {
			if(Stages.GetChild(i).GetChild(0).GetComponent<UISprite>().spriteName == "circle_empty")
				iCounter += 1;
		}


		return iCounter;
	}
}
