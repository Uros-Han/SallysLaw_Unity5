using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapListMgr : MonoBehaviour {

	private static MapListMgr instance;
	
	public static MapListMgr getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(MapListMgr)) as MapListMgr;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("MapListMgr");
				instance = obj.AddComponent (typeof(MapListMgr)) as MapListMgr;
			}
			
			return instance;
		}
	}

	void Awake(){
		if (instance == null)
			instance = this;
		
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad (gameObject);

		m_SaveForm = SAVE_FORM.TXT;
		
		m_MapList = new List<string> ();
		
		MakeMapIconList ();
	}
	
	void OnDestroy()
	{
		
		instance = null;
		
	}


	public SAVE_FORM m_SaveForm;

	public string m_strCurMap;
	public string m_strCurExtension;
	public List<string> m_MapList;


	public void MakeMapIconList()
	{
		GameObject scrollGrid = GameObject.Find ("ScrollGrid").gameObject;

		for (int i = 0; i < scrollGrid.transform.childCount; ++i) { // Initialize cleaning
			if(scrollGrid.transform.GetChild(i).name != "NewMapIcon")
				Destroy(scrollGrid.transform.GetChild(i).gameObject);
		}


		scrollGrid.GetComponent<UIGrid> ().enabled = true;
	}
}
