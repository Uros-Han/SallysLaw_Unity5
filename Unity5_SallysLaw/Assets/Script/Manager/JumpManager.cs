using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpManager : MonoBehaviour {
	private static JumpManager instance;

	public static JumpManager getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(JumpManager)) as JumpManager;
			}

			if (instance == null) {
				GameObject obj = new GameObject ("JumpManager");
				instance = obj.AddComponent (typeof(JumpManager)) as JumpManager;
			}

			return instance;
		}
	}

	public JumpPlugin jumpPlugin;
	public UILabel label;

	// Use this for initialization
	void Start() {
		jumpPlugin.Initialized += JumpPlugin_Initialized;
		label = GameObject.Find ("MainPanel").transform.GetChild (2).GetComponent<UILabel> ();
		label.text = "userID: " + "error";
	}

	void JumpPlugin_Initialized (object sender, System.EventArgs e)
	{
		label.text = "userID: " + jumpPlugin.userId;
		GameObject.Find("JumpPlugin").GetComponent<PlayFabManager>().LoginToPlayFab(jumpPlugin.userId);
	}
	

	public void Exit()
	{
		jumpPlugin.Exit ();
	}
}
