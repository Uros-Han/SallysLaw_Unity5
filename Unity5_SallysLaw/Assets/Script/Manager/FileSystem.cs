using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileSystem : MonoBehaviour {
	
	public static byte[] WriteGameDataFromFile( GameData myGamedata, string filename )
	{
		#if !WEB_BUILD
		string path = pathForDocumentsFile( filename );
		FileStream file = new FileStream (path, FileMode.Create, FileAccess.ReadWrite);
		
		//StreamWriter sw = new StreamWriter( file );


		BinaryFormatter b = new BinaryFormatter();
		b.Serialize(file, myGamedata);

		//cloud
		byte[] fileBytes = null;
		fileBytes = new byte[file.Length];

		Debug.Log("file Reading");

		file.Close();

		return fileBytes;
		#endif
	}
	
	
	public static GameData ReadGameDataFromFile( string filename)//, int lineIndex )
	{
		#if !WEB_BUILD
		string path = pathForDocumentsFile( filename );
		
		if (File.Exists(path))
		{
			FileStream file = new FileStream (path, FileMode.Open, FileAccess.ReadWrite);
			
			BinaryFormatter b = new BinaryFormatter();
			GameData ReadData = b.Deserialize(file) as GameData;
		
			file.Close();
			
			return ReadData;
		}
		else
		{
			Debug.Log("Can't find SaveFile");
			return null;
		}
		#else
		return null;
		#endif 
	}

	public static string pathForDocumentsFile( string filename ) 
	{
		string path;

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			path = Application.persistentDataPath;
			path = path.Substring (0, path.LastIndexOf ('/'));
			path = Path.Combine (Path.Combine (path, "Documents"), filename);
			Debug.Log (path);
			return path;
		} else if (Application.platform == RuntimePlatform.Android) {
			path = Application.persistentDataPath;
			path = path.Substring (0, path.LastIndexOf ('/'));
			return Path.Combine (path, filename);
		}
		else
		{
			path = Application.dataPath;
			path = path.Substring(0, path.LastIndexOf( '/' ) );
			return Path.Combine (path, filename);
		}
	}

}

[System.Serializable]
public class GameData {
	public System.DateTime m_savedTime;
	public int m_iChapter;
	public int m_iStage;
	public bool[] m_PhotoInfo;

	public void Initialize()
	{
		m_iChapter = 1;
		m_iStage = 1;
		m_PhotoInfo = new bool[30];
		m_savedTime = System.DateTime.Now;
	}
};