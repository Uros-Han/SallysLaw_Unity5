using UnityEngine;
using System.Collections;

public class ZeroConverter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (waitOneFrame ());
	}

	IEnumerator waitOneFrame()
	{
		yield return null;

		UILabel label = GetComponent<UILabel> ();
		if(Application.loadedLevelName.Equals("Main"))
			label.text = string.Format (label.text, "       ");
		else
			label.text = string.Format (label.text, "        ");

	}

	void SwitchFont()
	{
		StartCoroutine (waitOneFrame ());
	}
}
