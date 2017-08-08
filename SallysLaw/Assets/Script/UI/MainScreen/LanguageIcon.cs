using UnityEngine;
using System.Collections;

public class LanguageIcon : MonoBehaviour {

	void OnClick()
	{
		transform.parent.GetChild (0).gameObject.SetActive (true);
	}
}
