using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Stage {
	public string m_Runner;
	public string m_Guardian;

	public List<string> m_listBox;
	public List<string> m_listHoldingBox;
	public List<string> m_listFloor;

	public List<string> m_listR_Box;
	public List<string> m_listG_Box;

	public List<string> m_listR_Door;
	public List<string> m_listG_Door;

	public List<string> m_listR_Switch;
	public List<string> m_listG_Switch;

	public List<string> m_listMoveOrder;
	public List<string> m_listTimeballs;

	public List<string> m_listSpikes;
	public List<string> m_listSprings;
	public List<string> m_listPortals;

	public List<string> m_listTextFloats;

	public List<string> m_listTopDecos;
	public List<string> m_listInteractionProps;

	public string m_Goal;
	public string m_strWidthOfThisMap;
	public string m_Photo;

	public Stage(List<IndexTag> objList)
	{
		m_listBox = new List<string> ();
		m_listHoldingBox = new List<string> ();
		m_listFloor = new List<string> ();
		m_listR_Box = new List<string> ();
		m_listG_Box = new List<string> ();
		m_listR_Door = new List<string> ();
		m_listG_Door = new List<string> ();
		m_listR_Switch = new List<string> ();
		m_listG_Switch = new List<string> ();
		m_listMoveOrder = new List<string> ();
		m_listTimeballs = new List<string> ();
		m_listSpikes = new List<string> ();
		m_listSprings = new List<string> ();
		m_listPortals = new List<string> ();
		m_listTextFloats = new List<string> ();
		m_listTopDecos = new List<string> ();
		m_listInteractionProps = new List<string> ();

		for (int i = 0; i < objList.Count; ++i) {
			switch(objList[i].m_objID)
			{
			case OBJECT_ID.RUNNER:
				m_Runner = StringConverter(objList[i].m_Object.transform.position);
				break;

			case OBJECT_ID.GUARDIAN:
				m_Guardian = StringConverter(objList[i].m_Object.transform.position);
				break;

			case OBJECT_ID.GOAL:
				m_Goal = StringConverter(objList[i].m_Object.transform.position);
				break;

			case OBJECT_ID.PHOTO:
				m_Photo = StringConverter(objList[i].m_Object.transform.position);
				break;

			case OBJECT_ID.BOX:
				m_listBox.Add(StringConverter(objList[i].m_Object.transform.position) +"$"+ objList[i].m_Object.GetComponent<BoxMaptool>().m_iSpriteIdx);
				break;

			case OBJECT_ID.HOLDINGBOX:
				m_listHoldingBox.Add(StringConverter(objList[i].m_Object.transform.position));
				break;

			case OBJECT_ID.R_BOX:
				m_listR_Box.Add(StringConverter(objList[i].m_Object.transform.position));
				break;

			case OBJECT_ID.G_BOX:
				m_listG_Box.Add(StringConverter(objList[i].m_Object.transform.position));
				break;

			case OBJECT_ID.R_DOOR:
				m_listR_Door.Add(StringConverter(objList[i].m_Object.transform.position) +"$"+ objList[i].m_Object.transform.GetChild(0).GetComponent<DoorPosFixer>().m_fSize +"%"+ objList[i].m_Object.transform.GetChild(0).GetComponent<DoorPosFixer>().m_Color);
				break;

			case OBJECT_ID.G_DOOR:
				m_listG_Door.Add(StringConverter(objList[i].m_Object.transform.position) +"$"+ objList[i].m_Object.transform.GetChild(0).GetComponent<DoorPosFixer>().m_fSize);
				break;

			case OBJECT_ID.R_SWITCH:
				m_listR_Switch.Add(StringConverter(objList[i].m_Object.transform.position) + "$"+ GetDoorIdx(objList[i].m_Object.GetComponent<Switch>().m_objDoor.transform.parent.gameObject, true)
				                   + GetIsThisHoldSwitch(objList[i].m_Object));
				break;

			case OBJECT_ID.G_SWITCH:
				m_listG_Switch.Add(StringConverter(objList[i].m_Object.transform.position) + "$"+ GetDoorIdx(objList[i].m_Object.GetComponent<Switch>().m_objDoor.transform.parent.gameObject, false)
				                   + GetIsThisHoldSwitch(objList[i].m_Object));
				break;

			case OBJECT_ID.FLOOR:
				string tmpString;
				
				if(objList[i].m_Object.transform.parent.name.Equals("Box(Clone)"))
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetBoxIdx(objList[i].m_Object.transform.parent.gameObject, OBJECT_ID.BOX);
					tmpString += "%B";
				}
				else if(objList[i].m_Object.transform.parent.name.Equals("HoldingBox(Clone)"))
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetBoxIdx(objList[i].m_Object.transform.parent.gameObject, OBJECT_ID.HOLDINGBOX);
					tmpString += "%H";
				}
				else if(objList[i].m_Object.transform.parent.name.Equals("R_Box(Clone)"))
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetBoxIdx(objList[i].m_Object.transform.parent.gameObject, OBJECT_ID.R_BOX);
					tmpString += "%R";
				}
				else if(objList[i].m_Object.transform.parent.name.Equals("G_Box(Clone)"))
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetBoxIdx(objList[i].m_Object.transform.parent.gameObject, OBJECT_ID.G_BOX);
					tmpString += "%G";
				}
				else if(objList[i].m_Object.transform.parent.name.Equals("Spike(Clone)"))
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetSpikeIdx(objList[i].m_Object.transform.parent.gameObject);
					tmpString += "%S";
				}
				else{
					Debug.LogError("ERRRRRROR");
					tmpString= "";
				}

				m_listFloor.Add(tmpString);
				break;

			case OBJECT_ID.MOVEORDER:


				if(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner.name == "Box(Clone)")
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetBoxIdx(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner, OBJECT_ID.BOX);
					tmpString += "%B";
				}
				else if(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner.name == "HoldingBox(Clone)")
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetBoxIdx(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner, OBJECT_ID.HOLDINGBOX);
					tmpString += "%H";
				}
				else if(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner.name == "R_Box(Clone)")
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetBoxIdx(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner, OBJECT_ID.R_BOX);
					tmpString += "%R";
				}
				else if(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner.name == "G_Box(Clone)")
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetBoxIdx(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner, OBJECT_ID.G_BOX);
					tmpString += "%G";
				}
				else if(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner.name == "Spike(Clone)")
				{
					tmpString = StringConverter(objList[i].m_Object.transform.position) + "$" + GetSpikeIdx(objList[i].m_Object.GetComponent<MoveOrder>().m_objOwner);
					tmpString += "%S";
				}
				else{
					Debug.LogError("ERRRRRROR");
					tmpString= "";
				}

				m_listMoveOrder.Add(tmpString);

				break;

			case OBJECT_ID.Timeball:
				m_listTimeballs.Add(StringConverter(objList[i].m_Object.transform.position));
				break;

			case OBJECT_ID.SPIKE:

				tmpString = StringConverter(objList[i].m_Object.transform.position);
				tmpString += "$" + objList[i].m_Object.transform.rotation.eulerAngles.z;
				tmpString += GetIsThisRevivalSpike(objList[i].m_Object);

                m_listSpikes.Add(tmpString);
				break;

			case OBJECT_ID.SPRING:
				
				tmpString = StringConverter(objList[i].m_Object.transform.position);
				tmpString += "$" + objList[i].m_Object.transform.rotation.eulerAngles.z;
				
				m_listSprings.Add(tmpString);
				break;

			case OBJECT_ID.PORTAL:
				
				tmpString = StringConverter(objList[i].m_Object.transform.position) ;//+ "$" + GetPortalIdx(objList[i].m_Object.GetComponent<Portal>().m_objOwner);
				tmpString += ("$" + objList[i].m_Object.GetComponent<SkeletonAnimation>().initialSkinName);
				tmpString += ("%" + i);
				
				m_listPortals.Add(tmpString);
				break;

			case OBJECT_ID.TEXTFLOAT:
				tmpString = StringConverter(objList[i].m_Object.transform.position);
				tmpString += "$" + objList[i].m_Object.GetComponent<TextFloat_Pos>().m_objFloatUI.transform.GetChild(0).GetChild(1).GetComponent<UILabel>().text;

				if(objList[i].m_Object.GetComponent<TextFloat_Pos>().m_objFloatUI.transform.GetChild(0).GetComponent<TextFloat_UI>().m_bMemoryText)
				{
					if(objList[i].m_Object.GetComponent<TextFloat_Pos>().m_objFloatUI.transform.GetChild(0).GetComponent<TextFloat_UI>().m_bFatherText)
						tmpString += "%MT";
					else
						tmpString += "%MF";
				}
				else{
					if(objList[i].m_Object.GetComponent<TextFloat_Pos>().m_objFloatUI.transform.GetChild(0).GetComponent<TextFloat_UI>().m_bFatherText)
						tmpString += "%T";
					else
						tmpString += "%F";
				}

				m_listTextFloats.Add(tmpString);
				break;

			case OBJECT_ID.TOP_DECO:
				
				tmpString = StringConverter(objList[i].m_Object.transform.position);
				tmpString += "$" + objList[i].m_Object.GetComponent<BoxDeco>().m_iTopDecoIdx;
				
				m_listTopDecos.Add(tmpString);
				break;

			case OBJECT_ID.INTERACTION_PROP:
				
				tmpString = StringConverter(objList[i].m_Object.transform.position);
				tmpString += "$" + objList[i].m_Object.GetComponent<InteractionProp>().m_iPropIdx;
				
				m_listInteractionProps.Add(tmpString);
				break;

			default:
				Debug.LogError("ObjectID Error");
				break;
			}
		}

		for(int i = 0 ; i < m_listPortals.Count; ++i)
		{
			string tmpStr = m_listPortals[i].Substring(m_listPortals[i].IndexOf("%") +1 , m_listPortals[i].Length - (m_listPortals[i].IndexOf("%") +1));
			m_listPortals[i] +=  ("#" + GetPortalIdx(objList[System.Convert.ToInt32(tmpStr)].m_Object.GetComponent<Portal>().m_objOwner));
		}

		m_strWidthOfThisMap = System.Convert.ToString(GameObject.Find ("Grids").GetComponent<GridMgr>().GetWidthOfIndex_ThisMap());
	}

	string GetIsThisHoldSwitch(GameObject swc)
	{
		if (swc.GetComponent<Switch> ().m_bThisIsHoldDoor)
			return "%H";


		return "%N";
	}

	string GetIsThisRevivalSpike(GameObject spk)
	{
		if (spk.GetComponent<Spike> ().m_bRevival)
			return "%R";
		
		
		return "%N";
	}


	int GetDoorIdx(GameObject door, bool bThisIsR_Door)
	{
		if (bThisIsR_Door) {
			for (int i = 0; i < m_listR_Door.Count; ++i) {
				if (m_listR_Door [i].Substring(0, m_listR_Door [i].IndexOf("%")) == StringConverter (door.transform.position) + "$" + door.transform.GetChild (0).GetComponent<DoorPosFixer>().m_fSize)
					return i;
			}
		} else {
			for (int i = 0; i < m_listG_Door.Count; ++i) {
				if (m_listG_Door [i] == StringConverter (door.transform.position) + "$" + door.transform.GetChild (0).GetComponent<DoorPosFixer>().m_fSize)
					return i;
			}
		}

		Debug.LogError("Idx error");
		return -1;
	}

	int GetSpikeIdx(GameObject Spike)
	{
		for (int i = 0; i < m_listSpikes.Count; ++i) {
			if (m_listSpikes [i] == StringConverter (Spike.transform.position))
				return i;
		}
		
		Debug.LogError("Idx error");
		return -1;
	}

	int GetPortalIdx(GameObject Portal)
	{
		for (int i = 0; i < m_listPortals.Count; ++i) {
			if (m_listPortals [i].Substring(0, m_listPortals [i].IndexOf("$")) == StringConverter (Portal.transform.position))
				return i;
		}
		
		Debug.LogError("Idx error");
		return -1;
	}


	int GetBoxIdx(GameObject box, OBJECT_ID objID)
	{

		switch(objID)
		{
		case OBJECT_ID.BOX:
			for (int i = 0; i < m_listBox.Count; ++i) {
				if (m_listBox [i].Substring(0, m_listBox [i].IndexOf("$")) == StringConverter (box.transform.position))
					return i;
			}
			break;
		case OBJECT_ID.HOLDINGBOX:
			for (int i = 0; i < m_listHoldingBox.Count; ++i) {
				if (m_listHoldingBox [i] == StringConverter (box.transform.position))
					return i;
			}
			break;
		case OBJECT_ID.R_BOX:
			for (int i = 0; i < m_listR_Box.Count; ++i) {
				if (m_listR_Box [i] == StringConverter (box.transform.position))
					return i;
			}
			break;
		case OBJECT_ID.G_BOX:
			for (int i = 0; i < m_listG_Box.Count; ++i) {
				if (m_listG_Box [i] == StringConverter (box.transform.position))
					return i;
			}
			break;
		}

		Debug.LogError("Idx error");
		return -1;
	}

	string StringConverter(Vector2 pos)
	{

		float fTmpLeftiestXPos = GridMgr.getInstance.GetLeftiestOfThisMap ();

		pos -= new Vector2(fTmpLeftiestXPos , 0);


		string strResult;
		strResult = pos.x + "/" + pos.y;
		return strResult;
	}
	

}
