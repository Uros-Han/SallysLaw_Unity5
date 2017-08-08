using UnityEngine;
using System.Collections;

public class NotifyIcon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		NotyRefresh ();
	}

	public void NotyRefresh()
	{
		if (transform.parent.name.Equals ("LeftBottom")) {
			if(PlayerPrefs.GetInt("AlbumNoti") == 1)
				GetComponent<UISprite>().enabled = true;
			else
				GetComponent<UISprite>().enabled = false;
		} else {
		}
	}

}
