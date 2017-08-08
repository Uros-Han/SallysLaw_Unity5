using UnityEngine;
using System.Collections;

public class CancleBtn : MonoBehaviour {

	void OnClick()
	{
		transform.parent.parent.gameObject.SetActive (false);
	}
}
