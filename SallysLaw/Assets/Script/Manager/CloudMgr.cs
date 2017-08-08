using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class CloudMgr : MonoBehaviour {

	private static CloudMgr instance;

	public static CloudMgr getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(CloudMgr)) as CloudMgr;
			}

			if (instance == null) {
				GameObject obj = new GameObject ("CloudMgr");
				instance = obj.AddComponent (typeof(CloudMgr)) as CloudMgr;
			}

			return instance;
		}
	}

	void OnApplicationQuit()
	{
		instance = null;
	}	

	bool m_bCloudFileisNull = false;

	void Awake () {
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy(gameObject);


		CurrentSaveDAta = null;
	

//#if UNITY_ANDROID
//
//		GooglePlaySavedGamesManager.ActionConflict += delegate (GP_SnapshotConflict result) {
//			Debug.Log("Conflict Detected: ");
//			GP_Snapshot snapshot = result.Snapshot;
//			GP_Snapshot conflictSnapshot = result.ConflictingSnapshot;
//			// Resolve between conflicts by selecting the newest of the conflicting snapshots.
//			GP_Snapshot mResolvedSnapshot = snapshot;
//			if (snapshot.meta.LastModifiedTimestamp < conflictSnapshot.meta.LastModifiedTimestamp) {
//				mResolvedSnapshot = conflictSnapshot;
//			}
//
//			result.Resolve(mResolvedSnapshot);
//		};
//
//
//		//GooglePlay CloudSave Set Delegates
//		GooglePlaySavedGamesManager.ActionGameSaveLoaded += ActionGameSaveLoaded;
//
//
//		GooglePlaySavedGamesManager.ActionAvailableGameSavesLoaded += ActionAvailableGameSavesLoaded;
//
//#endif

	}

//	private void ActionGameSaveLoaded (GP_SpanshotLoadResult result) {
//		if(result.IsSucceeded)
//		{
//			GooglePlaySavedGamesManager.ActionGameSaveLoaded -= ActionGameSaveLoaded;
//			Debug.Log("Cloud Save Loaded Complete");
//			
//			CurrentSaveDAta = result.Snapshot.bytes;
//		}
//		
//		GameData_Load();
//		
//		if(result.IsSucceeded && Application.loadedLevelName.Equals("Main"))
//			GameData_Save ();
//	}

//	private void ActionAvailableGameSavesLoaded (GooglePlayResult res) {
//#if UNITY_ANDROID
//		
//		if(res.IsSucceeded)
//		{
//			if(GooglePlaySavedGamesManager.Instance.AvailableGameSaves.Count.Equals(0))
//			{
//				Debug.Log("CloudData is null");
//
//			}else{
//				foreach(GP_SnapshotMeta meta in GooglePlaySavedGamesManager.Instance.AvailableGameSaves) {
//					Debug.Log("Meta.Title: " 					+ meta.Title);
//					Debug.Log("Meta.Description: " 				+ meta.Description);
//					Debug.Log("Meta.CoverImageUrl): " 			+ meta.CoverImageUrl);
//					Debug.Log("Meta.LastModifiedTimestamp: " 	+ meta.LastModifiedTimestamp);
//				}
//			}
//		}else{
//			Debug.Log("Available Game Saves Load failed");
//		}
//#endif
//	}

	public bool isInitialized()
	{
//		#if UNITY_ANDROID
//		if (GameCenterManager.isAuthenticated())
//			return true;
//		#elif UNITY_IOS
//		if(iCloudBinding.documentStoreAvailable())
//			return true;
//		#endif

		return false;
	}

	byte[] m_ByteGameData;
	public GameData m_Gamedata;
	public byte[] CurrentSaveDAta;

	public void GameData_Save()
	{
		m_Gamedata = new GameData ();
		m_Gamedata.Initialize ();
		m_Gamedata.m_iChapter = GameMgr.getInstance.m_iOpenedChpt;
		m_Gamedata.m_iStage = GameMgr.getInstance.m_iOpenedStage;
		m_Gamedata.m_savedTime = System.DateTime.Now;

		for (int i = 0; i < 5; ++i) {
			for (int j = 0; j < 6; ++j) {
				m_Gamedata.m_PhotoInfo[i*6 + j] = GameMgr.getInstance.m_PhotoInfo[i].m_bPhotoGet[j];
			}
		}

		m_Gamedata = CompareBeforeSave(m_Gamedata, FileSystem.ReadGameDataFromFile ("SaveData"));

		FileSystem.WriteGameDataFromFile(m_Gamedata, "SaveData");
		Debug.Log ("Save GameData Complete");

		if (!GameMgr.getInstance.m_bCloud)
			return;

		if (isInitialized()) {
			BinaryFormatter b = new BinaryFormatter();
			MemoryStream m = new MemoryStream();

			b.Serialize(m, m_Gamedata);


			Debug.Log(m.GetBuffer().Length);
			Do_CloudSave (m.GetBuffer());
		}
	}

	public void GameData_Load()
	{
		Debug.Log ("Try Gamedata Load");
		m_ByteGameData = CurrentSaveDAta;

		if (m_ByteGameData != null && m_ByteGameData.Length != 0 &&GameMgr.getInstance.m_bCloud) {
			BinaryFormatter b = new BinaryFormatter ();
			MemoryStream m = new MemoryStream (m_ByteGameData);

			m_Gamedata = b.Deserialize (m) as GameData;

			m_Gamedata = CompareSaveData(m_Gamedata, FileSystem.ReadGameDataFromFile ("SaveData"));

			Debug.Log ("Move CloudData to CurrentSaveData");
		} else {

			Debug.Log("LocalLoad");
			m_Gamedata = FileSystem.ReadGameDataFromFile ("SaveData");
			if(m_Gamedata == null)
			{
				Create_SaveData();
				CurrentSaveDAta = null;
				return;
			}
		}

		CurrentSaveDAta = null;
		GameMgr.getInstance.m_iOpenedChpt = m_Gamedata.m_iChapter;
		GameMgr.getInstance.m_iOpenedStage = m_Gamedata.m_iStage;
		for (int i = 0; i < 5; ++i) {
			for (int j = 0; j < 6; ++j) {
				GameMgr.getInstance.m_PhotoInfo[i].m_bPhotoGet[j] = m_Gamedata.m_PhotoInfo[i*6+j];
			}
		}

		if (!Application.loadedLevelName.Equals ("Main"))
			return;

		if (GameObject.Find ("PhotoList") != null) {
			Transform PhotoList = GameObject.Find ("PhotoList").transform;
			for (int i = 0; i < 5; ++i) {
				PhotoList.GetChild (i).GetComponent<UIPhoto> ().PhotoCheck ();
			}
		}
		if(GameObject.Find ("Chapters") != null)
			GameObject.Find ("Chapters").GetComponent<StageProgressMgr> ().UpdateStages ();
	}

	GameData CompareSaveData(GameData CloudData, GameData LocalData)
	{
		if (LocalData == null) {

			GameData MyGameData = new GameData();
			MyGameData.Initialize();
			MyGameData.m_iChapter = m_Gamedata.m_iChapter;
			MyGameData.m_iStage = m_Gamedata.m_iStage;
			MyGameData.m_savedTime = m_Gamedata.m_savedTime;
			for (int i = 0; i < 5; ++i) {
				for (int j = 0; j < 6; ++j) {
					MyGameData.m_PhotoInfo[i*6+j] = m_Gamedata.m_PhotoInfo[i*6+j];
				}
			}

			Debug.Log ("Create LocalData with cloud data");
			FileSystem.WriteGameDataFromFile(MyGameData, "SaveData");

			return CloudData;
		}

		Debug.Log ("Cloud Stage Status : " + CloudData.m_iChapter * 10 + CloudData.m_iStage);
		Debug.Log ("Local Stage Status : " + LocalData.m_iChapter * 10 + LocalData.m_iStage);

		if (CloudData.m_iChapter * 10 + CloudData.m_iStage > LocalData.m_iChapter * 10 + LocalData.m_iStage) {
			Debug.Log ("Cloud saved data is recent file than local");
			return CloudData;
		} else if (CloudData.m_iChapter * 10 + CloudData.m_iStage < LocalData.m_iChapter * 10 + LocalData.m_iStage) {
			Debug.Log ("Cloud saved data is older file than local");
			return LocalData;
		}else {

			Debug.Log("Stage Status is same");

			if (CloudData.m_savedTime >= LocalData.m_savedTime) {
				Debug.Log ("Cloud saved data is recent file than local");
				return CloudData;
			} else {
				Debug.Log ("Cloud saved data is older file than local");
				return LocalData;
			}
		}
	}

	GameData CompareBeforeSave(GameData CurData, GameData LocalData)
	{
		if (LocalData == null) {			
			return CurData;
		}
		
		Debug.Log ("Current Stage Status : " + CurData.m_iChapter * 10 + CurData.m_iStage);
		Debug.Log ("Local Stage Status : " + LocalData.m_iChapter * 10 + LocalData.m_iStage);
		
		if (CurData.m_iChapter * 10 + CurData.m_iStage > LocalData.m_iChapter * 10 + LocalData.m_iStage) {
			Debug.Log ("Current saved data is recent file than local");
			return CurData;
		} else if (CurData.m_iChapter * 10 + CurData.m_iStage < LocalData.m_iChapter * 10 + LocalData.m_iStage) {
			Debug.Log ("Current saved data is older file than local");
			return LocalData;
		}else {
			
			Debug.Log("Stage Status is same");
			
			if (CurData.m_savedTime >= LocalData.m_savedTime) {
				Debug.Log ("Current saved data is recent file than local");
				return CurData;
			} else {
				Debug.Log ("Current saved data is older file than local");
				return LocalData;
			}
		}
	}



	public void Do_CloudSave(byte[] Data)
	{
//		#if UNITY_ANDROID
//		Debug.Log ("Try Save GameData to GoogleCloud");
//
//		GooglePlaySavedGamesManager.ActionGameSaveResult += ActionGameSaveResult;
//		GooglePlaySavedGamesManager.Instance.CreateNewSnapshot ("SaveData", "",
//			new Texture2D( 100, 100, TextureFormat.RGB24, false ),
//			Data, 0);
//		#elif UNITY_IOS
//		Debug.Log ("Try Save GameData to ICloud");
//		P31CloudFile.writeAllBytes("SaveData", Data);
//		Debug.Log(Data.Length);
//		#endif
	}

	public void Do_CloudLoad()
	{
//		#if UNITY_ANDROID
//		GameData_Load();
//		Debug.Log ("Try Load GameData to GoogleCloud");
//		GooglePlaySavedGamesManager.instance.LoadSpanshotByName("SaveData");
//		#elif UNITY_IOS
//		Debug.Log ("Try Load GameData to ICloud");
//
//		Debug.Log("is File in Cloud? : " + iCloudBinding.isFileInCloud("SaveData"));
//		Debug.Log("is File Downloaded? : " + iCloudBinding.isFileDownloaded("SaveData"));
//
//		bool bDontSaveCloud = false;
//
//		CurrentSaveDAta = P31CloudFile.readAllBytes("SaveData");
//		Debug.Log("SaveData is exist? : " + P31CloudFile.exists("SaveData"));
//
//		if(CurrentSaveDAta != null){
//			Debug.Log (CurrentSaveDAta.Length);
//			bDontSaveCloud = false;
//		}else{
//			Debug.Log ("CurrentSaveDAta is null");
//			bDontSaveCloud = true;
//		}
//		GameData_Load();
//
//		if(!bDontSaveCloud)
//			GameData_Save();
//
//
//		#endif
	}

//	private void ActionGameSaveResult (GP_SpanshotLoadResult result) {
//		GooglePlaySavedGamesManager.ActionGameSaveResult -= ActionGameSaveResult;
//		Debug.Log("ActionGameSaveResult: " + result.Message);
//
//	}	

	public void Create_SaveData()
	{
		GameData MyGameData = new GameData();
		MyGameData.Initialize();
		Debug.Log ("Initialzed");
		FileSystem.WriteGameDataFromFile(MyGameData, "SaveData");

		m_Gamedata = MyGameData;

		Debug.Log ("Create Save Data Complete");

		//m_SdkMgr = GameObject.Find ("GameSDKManager(Clone)").GetComponent<GameSDKManager>();
	}   

}
