using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour {
	UISprite sprite;
	float fSpeed;
	// Use this for initialization
	void Start () {
		sprite = GetComponent<UISprite> ();
		fSpeed = 1f;

		StartCoroutine (SplashColor ());
	}

	IEnumerator SplashColor()
	{
		do{
			sprite.color = new Color(sprite.color.r + fSpeed * Time.deltaTime, sprite.color.r + fSpeed * Time.deltaTime, sprite.color.r + fSpeed * Time.deltaTime);
			yield return null;
		}while(sprite.color.r < 1);
		yield return new WaitForSeconds(1.5f);
		do{
			sprite.color = new Color(sprite.color.r - fSpeed * Time.deltaTime, sprite.color.r - fSpeed * Time.deltaTime, sprite.color.r - fSpeed * Time.deltaTime);
			yield return null;
		}while(sprite.color.r > 0);

		JumpManager jmpMgr = JumpManager.getInstance;
		GameObject.Find ("UI Root").transform.GetChild (3).gameObject.SetActive (true);
		while(!jmpMgr.m_bInitialized){
			
			yield return null;
		};
		Application.LoadLevel ("Main");
	}
}