using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFabFileSystem : MonoBehaviour {

	public static void WriteGameDataFromFile( GameData myGamedata )
	{
		#if UNITY_WEBGL
		PlayFabManager playFabMgr = GameObject.Find("JumpPlugin").GetComponent<PlayFabManager>();

		playFabMgr.UpdatePlayerData("iChapter", myGamedata.m_iChapter.ToString());
		playFabMgr.UpdatePlayerData("iStage", myGamedata.m_iStage.ToString());
		for (int i = 0; i < 5; ++i) {
			for (int j = 0; j < 6; ++j) {
				playFabMgr.UpdatePlayerData(string.Format("Photo-{0}_{1}",i+1,j+1), myGamedata.m_PhotoInfo[i*6 + j].ToString()); // True, False
			}
		}
		#endif
	}


	public static GameData ReadGameDataFromFile()//, int lineIndex )
	{
		#if UNITY_WEBGL
		PlayFabManager playFabMgr = GameObject.Find("JumpPlugin").GetComponent<PlayFabManager>();
		GameData readData = new GameData();
		readData.Initialize ();

		if(playFabMgr.GetDataValueForKey("iChapter") == "empty")
		{
//			UILabel label0 = GameObject.Find ("MainPanel").transform.GetChild (2).GetComponent<UILabel> ();
//			label0.text = label0.text + ", empty!";
			return null;
		}

		readData.m_iChapter = System.Convert.ToInt32(playFabMgr.GetDataValueForKey("iChapter"));
		readData.m_iStage = System.Convert.ToInt32(playFabMgr.GetDataValueForKey("iStage"));
		for (int i = 0; i < 5; ++i) {
			for (int j = 0; j < 6; ++j) {
				string strPhoto = playFabMgr.GetDataValueForKey(string.Format("Photo-{0}_{1}",i+1,j+1));

				if(strPhoto == "True")
					readData.m_PhotoInfo[i*6 + j] = true;
				else
					readData.m_PhotoInfo[i*6 + j] = false;
			}
		}

//		UILabel label = GameObject.Find ("MainPanel").transform.GetChild (2).GetComponent<UILabel> ();
//		label.text = label.text + ", Load Completed! " + readData.m_iChapter + "_" + readData.m_iStage;

		return readData;
		#else
		return null;
		#endif 
	}
}
