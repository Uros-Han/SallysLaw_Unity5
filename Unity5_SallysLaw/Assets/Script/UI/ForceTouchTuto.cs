using UnityEngine;
using System.Collections;

public class ForceTouchTuto : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_IOS
		if(!ForceTouchPlugin.SupportsForceTouch())
			gameObject.SetActive(false);
#else
		gameObject.SetActive(false);
#endif
	}
}