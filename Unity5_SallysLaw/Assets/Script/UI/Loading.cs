using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour {
	public UILabel progressLabel;
	Transform SallyIconTrans;

	void Start () {
		Load ();
	}

	void OnDestroy(){
		StopAllCoroutines ();
	}
	
	void Load()
	{
		Debug.Log ("Loading stage2726");
		
		if(GameMgr.getInstance.m_iCurStage == 1 && GameMgr.getInstance.m_iCurChpt != 5)
		{
			Application.LoadLevel("Memory1_2726");
		}else{
			
			switch (GameMgr.getInstance.m_iCurChpt) {
			case 1:
				Application.LoadLevel("Chapter1_2726");
				break;
			case 2:
				Application.LoadLevel("Chapter2_2726");
				break;
			case 3:
				Application.LoadLevel("Chapter3_2726");
				break;
			case 4:
				Application.LoadLevel("Chapter4_2726");
				break;
			case 5:
				Application.LoadLevel("Chapter5_2726");
				break;
				
			default:
				Application.LoadLevel("Chapter5_2726");
				break;
			}
		}
	}
}
