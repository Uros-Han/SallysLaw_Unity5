using UnityEngine;
using System.Collections;

public class Joypad : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (Loop());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}


	IEnumerator Loop()
	{

		Vector3 originPos = Vector3.zero;
		Vector3 curMousePos = Vector3.zero;
		Vector3 curDirNormal = Vector3.zero;

		SceneStatus m_SceneStatus = GameObject.Find ("SceneStatus").GetComponent<SceneStatus> ();

		do{
			yield return null;

			if(Time.timeScale != 1)
				transform.GetChild(0).gameObject.SetActive(false);
			else{
				if (m_SceneStatus.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN) {
						
					if(Input.GetMouseButtonDown(0)){
						originPos = UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
						transform.GetChild(0).gameObject.SetActive(true);

						transform.position = originPos;
					}else if(Input.GetMouseButton(0))
					{
						curMousePos = UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
						curDirNormal = Vector3.Normalize(curMousePos - originPos);

						if(Vector2.Distance(originPos, curMousePos) < 0.1f)
							transform.GetChild(0).GetChild(1).position = curMousePos;
						else
							transform.GetChild(0).GetChild(1).position = originPos + (curDirNormal * 0.1f);

					}else
						transform.GetChild(0).gameObject.SetActive(false);
				}
			}
		
		}while(true);
	}
}
