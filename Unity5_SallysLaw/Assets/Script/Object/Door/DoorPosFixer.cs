using UnityEngine;
using System.Collections;

public class DoorPosFixer : MonoBehaviour {
	public float m_fSize;
	public Color m_Color; //저장,로드, 키,버튼 모두가 이 변수를 통해 자신의 색을 암

	public void PosFixer()
	{
		transform.localPosition = new Vector2 (0, (m_fSize / 2 * 0.5f)- 0.25f);

		if(gameObject.name == "R_Door")
			GetComponent<BoxCollider2D> ().size = new Vector2 (2, 0.5f * m_fSize);
//		else
//			GetComponent<BoxCollider2D> ().size = new Vector2 (0.5f, 0.5f * m_fSize);
	}

	void Start()
	{
		float fTmpPosFixer = ((m_fSize / 2 * 0.5f)- 0.25f) * -1;
		string tmpStirng = "";
			

		transform.GetChild(3).GetComponent<SkeletonAnimation>().skeleton.SetColor (m_Color); //symbol color

		for (int i = 0; i < m_fSize; ++i) {
			GameObject tmpObj = Instantiate (Resources.Load("Prefabs/Objects/Doors/DoorSprite") as GameObject) as GameObject;
			tmpObj.transform.parent = transform.Find("Sprites").transform;
			tmpObj.transform.localPosition = new Vector2(0,fTmpPosFixer);

			tmpObj.GetComponent<DoorSprite>().m_iDoorIdx = i;

			if(i == 0){
				tmpObj.GetComponent<SpriteRenderer>().sprite = SceneObjectPool.getInstance.m_sally_listDoor[0];

				tmpObj.GetComponent<DoorSprite>().m_iSpriteIdx = 0;
			}
			else{
				int iTmpRand = Random.Range(1, SceneObjectPool.getInstance.m_sally_listDoor.Count);

				tmpObj.GetComponent<SpriteRenderer>().sprite = SceneObjectPool.getInstance.m_sally_listDoor[iTmpRand];

				tmpObj.GetComponent<DoorSprite>().m_iSpriteIdx = iTmpRand;
			}


			fTmpPosFixer += 0.515f;
		}
	}
}
