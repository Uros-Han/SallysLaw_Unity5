using UnityEngine;
using System.Collections;

public class CostumeAplliyer : MonoBehaviour {
	// Use this for initialization
	void Start () {
		if(!transform.parent.GetComponent<UIPhoto>().m_bInApp)
			GetComponent<UILabel>().text = string.Format("'{0}' costume has been applied.", transform.parent.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetComponent<UILabel>().text);
		else
			GetComponent<UILabel>().text = string.Format("'{0}' costume has been applied.", transform.parent.GetChild(0).GetChild(5).GetChild(0).GetComponent<UILabel>().text);
	}

}
