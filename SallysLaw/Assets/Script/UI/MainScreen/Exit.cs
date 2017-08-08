using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {

	void OnClick()
	{
		transform.parent.gameObject.SetActive (false);
	}
}
