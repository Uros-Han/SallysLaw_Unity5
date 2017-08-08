using UnityEngine;
using System.Collections;

public class PC_TutorialLabel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		switch(Localization.language)
		{
		case "English":
			if(transform.parent.gameObject.name.Equals("anchor"))//fastforward tuto
			{
				
			}else{
				transform.GetChild(0).GetChild(1).localPosition = new Vector2(0, -336f);
				transform.GetChild(0).GetChild(2).localPosition = new Vector2(-31, -336f);
				transform.GetChild(1).GetChild(1).localPosition = new Vector2(-45, -336f);
				transform.GetChild(1).GetChild(2).localPosition = new Vector2(405, -336f);
			}
			break;

		case "한국어":
			if(transform.parent.gameObject.name.Equals("anchor"))//fastforward tuto
			{

			}else{
				transform.GetChild(0).GetChild(1).localPosition = new Vector2(61f, -336f);
				transform.GetChild(0).GetChild(2).localPosition = new Vector2(-326, -336f);
				transform.GetChild(1).GetChild(1).localPosition = new Vector2(77, -336f);
				transform.GetChild(1).GetChild(2).localPosition = new Vector2(-368, -336f);
			}
			break;

		case "中文简体":
			if(transform.parent.gameObject.name.Equals("anchor"))//fastforward tuto
			{
				
			}else{
				transform.GetChild(0).GetChild(1).localPosition = new Vector2(61f, -336f);
				transform.GetChild(0).GetChild(2).localPosition = new Vector2(-175, -330f);
				transform.GetChild(1).GetChild(1).localPosition = new Vector2(0, -336f);
				transform.GetChild(1).GetChild(2).localPosition = new Vector2(-163, -330f);
			}
			break;

		case "中文繁體":
			if(transform.parent.gameObject.name.Equals("anchor"))//fastforward tuto
			{
				
			}else{
				transform.GetChild(0).GetChild(1).localPosition = new Vector2(61f, -336f);
				transform.GetChild(0).GetChild(2).localPosition = new Vector2(-175, -330f);
				transform.GetChild(1).GetChild(1).localPosition = new Vector2(0, -336f);
				transform.GetChild(1).GetChild(2).localPosition = new Vector2(-163, -330f);
			}
			break;

		default:
			Debug.LogError("unknown language");
		
			break;
		}
	}

}
