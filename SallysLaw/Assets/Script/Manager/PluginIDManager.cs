using UnityEngine;
using System.Collections;

public class PluginIDManager : MonoBehaviour {

	private static PluginIDManager _instance;
	public static PluginIDManager Instance
	{
		get
		{
			if(Application.isPlaying)
			{
				if(_instance == null)
				{
					_instance =  FindObjectOfType(typeof (PluginIDManager)) as PluginIDManager;
					if (_instance == null)
					{
						GameObject obj = new GameObject("PluginIDManager");
						_instance = obj.AddComponent(typeof (PluginIDManager)) as PluginIDManager;
					}
				}
			}
			return _instance;
		}
	}
	
	void Awake()
	{
		DontDestroyOnLoad(this);
	}

	public string ConvertToItemID(StoreItemID sID)
	{
		string returnString = "";
		switch(GameMgr.getInstance.OS_Type)
		{
		case OSType.Android:
			switch(sID)
			{
			case StoreItemID.END: returnString="none"; break;
			case StoreItemID._COSTUME01: returnString="com.sally.costume"; break;
			}
			break;

		case OSType.iOS:
			switch(sID)
			{
			case StoreItemID.END: returnString="none"; break;
			case StoreItemID._COSTUME01: returnString="com.sally.costume"; break;
			}
			break;
		}

		return returnString;
	}

	public StoreItemID ConvertToItemID(string sID)
	{
		StoreItemID storeitemID=StoreItemID.END;

		if (sID == ConvertToItemID(StoreItemID._COSTUME01))
			storeitemID = StoreItemID._COSTUME01;

		return storeitemID;
	}

	public string GetAppStoreLink()
	{
		string returnString="";
		switch(GameMgr.getInstance.OS_Type)
		{
		case OSType.Android: returnString = "https://www.naver.com"; break;
		case OSType.iOS: returnString = "https://www.google.com"; break;
		}
		return returnString;
	}
}

public enum OSType
{
	Android=0,
	iOS=1,
	PC=2
}

public enum StoreItemID
{
	_COSTUME01,
	END
}