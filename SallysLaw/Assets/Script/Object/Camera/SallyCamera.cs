using UnityEngine;
using System.Collections;

public class SallyCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(GameObject.Find ("Runner(Clone)") != null)
		transform.position = GameObject.Find ("Runner(Clone)").transform.position + new Vector3(0,0,-10);
	}
}
