using UnityEngine;
using System.Collections;
using System.IO;

public class MapIcon : MonoBehaviour {

	public Texture2D m_MapTexture;
	public string m_strName;
	public string m_strExtension;

	void Start()
	{
//		m_MapTexture = LoadMapTexture (m_strName);
//		GetComponent<UITexture> ().mainTexture = m_MapTexture;
//		GetComponent<UITexture> ().width = m_MapTexture.width/2;
//		GetComponent<UITexture> ().height = m_MapTexture.height/2;
//		transform.parent.Find ("ChptImg").GetComponent<UITexture> ().mainTexture = Resources.Load<Texture2D> ("Sprites/UI/bg_03");
	}

//	Texture2D LoadMapTexture(string strFileName)
//	{
//		Texture2D tex;
//		byte[] fileData;
//
//		string strDir = Application.persistentDataPath + "/StagesPng";
//		if (!Directory.Exists (strDir)) {
//			return null;
//		}
//		
//		if (!File.Exists (strDir + "/" + strFileName)) {
//		//	Debug.LogError ("Cant Find File");
//			return null;
//		}
		
//		fileData = File.ReadAllBytes(strDir + "/" + strFileName);
//		tex = new Texture2D(2, 2);
//		tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

//		return tex;
//	}

}
