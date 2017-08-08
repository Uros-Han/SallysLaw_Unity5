using UnityEngine;
using System.Collections;

public class R_Box : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GameObject.Find("Guardian(Clone)"))
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D>(), GameObject.Find("Guardian(Clone)").GetComponent<CircleCollider2D>());
	}
}
