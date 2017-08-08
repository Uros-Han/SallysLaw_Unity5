using UnityEngine;
using System.Collections;

public class G_Box : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(GameObject.Find("Runner(Clone)"))
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D>(), GameObject.Find("Runner(Clone)").GetComponent<CircleCollider2D>());
	}
}
