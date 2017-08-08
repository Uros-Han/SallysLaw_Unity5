using UnityEngine;
using System.Collections;

public class DestroyMapList : MonoBehaviour {

	void OnClick()
	{
		Destroy (GameObject.Find ("MapListMgr").gameObject);
	}
}
