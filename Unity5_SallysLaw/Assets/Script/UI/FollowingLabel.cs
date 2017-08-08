using UnityEngine;
using System.Collections;

public class FollowingLabel : MonoBehaviour {

	// Use this for initialization
	void Update () {
		transform.localPosition = new Vector2 (transform.parent.GetComponent<UILabel>().localSize.x/2, 0);
	}
}
