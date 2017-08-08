using UnityEngine;
using System.Collections;

public class ScaleToScreenSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<UISprite> ().width = Screen.width;
		GetComponent<UISprite> ().height = Screen.height;
	}

}
