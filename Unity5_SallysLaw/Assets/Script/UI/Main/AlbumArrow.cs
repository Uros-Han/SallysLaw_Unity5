using UnityEngine;
using System.Collections;

public class AlbumArrow : MonoBehaviour {

	SpringPanel springPanel;
	public bool bRight;

	BoxCollider boxCol;
	UISprite uiSprite;

	public float keyDelay = 0.1f;  // 0.1 second
	private float timePassed = 0f;

	void Start()
	{
		springPanel = transform.parent.parent.GetChild(1).GetComponent<SpringPanel> ();

		boxCol = GetComponent<BoxCollider> ();
		uiSprite = GetComponent<UISprite> ();

		CheckPhotoEnd (0);
		StartCoroutine (SpringTargetChk ());
	}


	void OnDestroy()
	{
		StopAllCoroutines ();
	}
	#if UNITY_STANDALONE || UNITY_WEBGL
	void Update()
	{
		timePassed += Time.deltaTime;

		if (bRight) {
			if((Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0) && timePassed >= keyDelay)
			{
				OnClick();
				timePassed = 0f;
			}
		} else {
			if((Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < 0) && timePassed >= keyDelay)
			{
				OnClick();
				timePassed = 0f;
			}
		}
	}
#endif
	void OnClick()
	{
		if (!GetComponent<UISprite> ().enabled)
			return;

		if (GameMgr.getInstance.m_uiStatus != MainUIStatus.PHOTO)
			return;

		AudioMgr.getInstance.PlaySfx (GameObject.Find("SFX").GetComponent<AudioSource>(),"ui_bundle", (int)UI_SOUND_LIST.BUTTON_ENTER);

		float fNextTarget;

		if (bRight) {
			fNextTarget = springPanel.target.x - 1200f;

			springPanel.target = new Vector3 (fNextTarget, 0);
		

		} else {
			fNextTarget = springPanel.target.x + 1200f;

			springPanel.target = new Vector3 (springPanel.target.x + 1200f, 0);
		}
//
//		GameObject CurPhoto;
//		if (springPanel.target.x == 1200) {
//			CurPhoto = GameObject.Find ("PhotoList").transform.GetChild (5).gameObject;
//			
//			GameObject.Find ("ReallyBuy?").transform.GetChild (0).GetChild (0).GetComponent<UIButtonMessage> ().target = CurPhoto;
//			GameObject.Find ("ReallyBuy?").transform.GetChild (1).GetChild (0).GetComponent<UIButtonMessage> ().target = CurPhoto;
//		} else if (springPanel.target.x == 2400) {
//			CurPhoto = GameObject.Find ("PhotoList").transform.GetChild (6).gameObject;
//			
//			GameObject.Find ("ReallyBuy?").transform.GetChild (0).GetChild (0).GetComponent<UIButtonMessage> ().target = CurPhoto;
//			GameObject.Find ("ReallyBuy?").transform.GetChild (1).GetChild (0).GetComponent<UIButtonMessage> ().target = CurPhoto;
//		} else if (springPanel.target.x == 3600) {
//			CurPhoto = GameObject.Find ("PhotoList").transform.GetChild (7).gameObject;
//			
//			GameObject.Find ("ReallyBuy?").transform.GetChild (0).GetChild (0).GetComponent<UIButtonMessage> ().target = CurPhoto;
//			GameObject.Find ("ReallyBuy?").transform.GetChild (1).GetChild (0).GetComponent<UIButtonMessage> ().target = CurPhoto;
//		} 


		springPanel.enabled = true;

	}

	IEnumerator SpringTargetChk()
	{
		float fBeforeTarget = 0f;
		do {
			if(Mathf.Abs(springPanel.target.x - fBeforeTarget) > 600)
				transform.parent.BroadcastMessage ("CheckPhotoEnd", springPanel.target.x);




			fBeforeTarget = springPanel.target.x;

			yield return null;

		} while(true);
	}

	void CheckPhotoEnd(float fNextTarget)
	{
		Transform photoListTransform = transform.parent.parent.GetChild (1);

		if (bRight) {
			if (fNextTarget + 4800 > -600 && fNextTarget + 4800 < 600) {
				boxCol.enabled = false;
				uiSprite.enabled = false;
			} else {
				boxCol.enabled = true;
				uiSprite.enabled = true;
			}
		} else {
			if (fNextTarget - 3600 > -600 && fNextTarget - 3600 < 600) {
				boxCol.enabled = false;
				uiSprite.enabled = false;
			} else {
				boxCol.enabled = true;
				uiSprite.enabled = true;
			}
		}
	}

}
