using UnityEngine;
using System.Collections;
using System.IO;

public class SaveBtn : MonoBehaviour {

	Camera rdCam;

	public RenderTexture rdTex;
	public Texture2D screenShot;

	void Start()
	{
		rdCam = GameObject.Find("SnapShotCam").GetComponent<Camera>();
	}

	void OnClick()
	{
		GameObject tmpErrMsgPrf = Resources.Load ("Prefabs/UI/mapToolErrorMsg") as GameObject;
		GameObject tmpErrMsg;
		
		if (GameObject.Find ("Runner(Clone)") == null) {
			tmpErrMsg = Instantiate (tmpErrMsgPrf) as GameObject;
			tmpErrMsg.GetComponent<UILabel> ().text = "Runner Need!";
		} else if (GameObject.Find ("Guardian(Clone)") == null) {
			tmpErrMsg = Instantiate (tmpErrMsgPrf) as GameObject;
			tmpErrMsg.GetComponent<UILabel> ().text = "Guardian Need!";
		} else if (GameObject.Find ("R_Goal(Clone)") == null) {
			tmpErrMsg = Instantiate (tmpErrMsgPrf) as GameObject;
			tmpErrMsg.GetComponent<UILabel> ().text = "Goal Need!";
		} else { // Save

			if (GameObject.Find ("MapName").GetComponent<UIInput> ().value != "") {
				GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().Save (GameObject.Find ("MapName").GetComponent<UIInput> ().value);
//				StartCoroutine(TakeSnapShot(512, 256));
			}else
			{
				tmpErrMsg = Instantiate (tmpErrMsgPrf) as GameObject;
				tmpErrMsg.GetComponent<UILabel> ().text = "MapName Need!";
			}

//			GameObject.Find ("SaveUI").transform.GetChild (0).gameObject.SetActive (true);
//			GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().m_bOverayUIOn = true;
		}
	}

	IEnumerator TakeSnapShot(int width, int height)
	{
		yield return new WaitForEndOfFrame();

		GridMgr gridMgr = GridMgr.getInstance;

		float fHeight = rdCam.orthographicSize * 2f * 100f;
		//float fWidth = fHeight * rdCam.rect.width * Camera.main.aspect;
		float fWidth = (1f + ((gridMgr.GetWidthOfIndex_ThisMap () - 7f) * 0.15f)) * fHeight * (326f/579f); //(326f/579f) = aspect in portrait camera view

		//RenderTexture 생성
		RenderTexture rdTex = new RenderTexture((int)fWidth, (int)fHeight, 24);
		//RenderTexture 저장을 위한 Texture2D 생성
		Texture2D screenShot = new Texture2D((int)fWidth, (int)fHeight, TextureFormat.ARGB32, false);
		//카메라 활성화
//		rdCam.active = true;


		rdCam.transform.position = gridMgr.GetCenterOfThisMap();
		rdCam.transform.position = new Vector3 (rdCam.transform.position.x ,0 , -10f);


		if (gridMgr.GetWidthOfIndex_ThisMap() > 7)
			rdCam.rect = new Rect(rdCam.transform.position.x - (fWidth/2f), rdCam.transform.position.y - (fHeight/2f), fWidth, fHeight);

	

		
		// 카메라의 타겟 텍스쳐에 랜더 텍스쳐 할당
		rdCam.targetTexture = rdTex;
		// 카메라화면 랜더
		rdCam.Render();
		// 렌더 텍스쳐 활성화 
		RenderTexture.active = rdTex;
		// 렌더 텍스쳐 read
		screenShot.ReadPixels(new Rect(0, 0, (int)fWidth, (int)fHeight), 0, 0);
		
		RenderTexture.active = null;
		rdCam.targetTexture = null;
//		rdCam.active = false;
		Destroy(rdTex);
		yield return 0;
		
		byte[] bytes = screenShot.EncodeToPNG();

		if (!Directory.Exists (Application.persistentDataPath + "/StagesPng"))
			Directory.CreateDirectory (Application.persistentDataPath + "/StagesPng");

//		File.WriteAllBytes(Application.persistentDataPath + "/StagesPng/" + GameObject.Find ("MapName").GetComponent<UIInput> ().value, bytes);
	}
}



