using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------------------
// 메모리 풀 클래스
// 용도 : 특정 게임오브젝트를 실시간으로 생성과 삭제하지 않고,
//      : 미리 생성해 둔 게임오브젝트를 재활용하는 클래스입니다.
//-----------------------------------------------------------------------------------------
//MonoBehaviour 상속 안받음. IEnumerable 상속시 foreach 사용 가능
//System.IDisposable 관리되지 않는 메모리(리소스)를 해제 함
public class MemoryPool : IEnumerable, System.IDisposable {
    //-------------------------------------------------------------------------------------
    // 아이템 클래스
    //-------------------------------------------------------------------------------------
    class Item
    {
        public bool active; //사용중인지 여부
        public GameObject gameObject;
    }
    Item[] boxTable;
	Item[] floorTable;
	Item[] SpikeTable;
	Item[] SallyPathTable;

    //------------------------------------------------------------------------------------
    // 열거자 기본 재정의
    //------------------------------------------------------------------------------------
    public IEnumerator GetEnumerator()
    {
		if (boxTable == null)
            yield break;

		int count = boxTable.Length;

        for (int i = 0; i < count; i++)
        {
			Item item = boxTable[i];
            if (item.active)
                yield return item.gameObject;
        }
    }
    //-------------------------------------------------------------------------------------
    // 메모리 풀 생성
    // original : 미리 생성해 둘 원본소스
    // count : 풀 최고 갯수
    //-------------------------------------------------------------------------------------
	public void Create(Object original, int count, string objName)
    {
		if (objName == "Box") {
			boxTable = new Item[count];

			for (int i = 0; i < count; i++) {
				Item item = new Item ();
				item.active = false;
				item.gameObject = GameObject.Instantiate (original) as GameObject;
				item.gameObject.transform.parent = GameObject.Find (objName+"Pool").transform;
				item.gameObject.SetActive (false);
				boxTable [i] = item;
			}
		} else if (objName == "Floor") {
			floorTable = new Item[count];
			
			for (int i = 0; i < count; i++) {
				Item item = new Item ();
				item.active = false;
				item.gameObject = GameObject.Instantiate (original) as GameObject;
				item.gameObject.transform.parent = GameObject.Find (objName+"Pool").transform;
				item.gameObject.SetActive (false);
				floorTable [i] = item;
			}
		} else if (objName == "Spike") {
			SpikeTable = new Item[count];
			
			for (int i = 0; i < count; i++) {
				Item item = new Item ();
				item.active = false;
				item.gameObject = GameObject.Instantiate (original) as GameObject;
				item.gameObject.transform.parent = GameObject.Find (objName+"Pool").transform;
				item.gameObject.SetActive (false);
				SpikeTable [i] = item;
			}
		} else if (objName == "SallyPath") {
			SallyPathTable = new Item[count];
			
			for (int i = 0; i < count; i++) {
				Item item = new Item ();
				item.active = false;
				item.gameObject = GameObject.Instantiate (original) as GameObject;
				item.gameObject.transform.parent = GameObject.Find (objName+"Pool").transform;
				item.gameObject.SetActive (false);
				SallyPathTable [i] = item;
			}
		}
    }
    //-------------------------------------------------------------------------------------
    // 새 아이템 요청 - 쉬고 있는 객체를 반납한다.
    //-------------------------------------------------------------------------------------
	public GameObject NewItem(string objName)
    {
		if (objName == "Box") {
			if (boxTable == null)
				return null;
			int count = boxTable.Length;

			for (int i = 0; i < count; i++) {
				Item item = boxTable [i];
				if (item.active == false) {
					item.active = true;
					item.gameObject.SetActive (true);
					return item.gameObject;
				}
			}
		} else if (objName == "Floor") {
			if (floorTable == null)
				return null;
			int count = floorTable.Length;
			
			for (int i = 0; i < count; i++) {
				Item item = floorTable [i];
				if (item.active == false) {
					item.active = true;
					item.gameObject.SetActive (true);
					return item.gameObject;
				}
			}
		} else if (objName == "Spike") {
			if (SpikeTable == null)
				return null;
			int count = SpikeTable.Length;
			
			for (int i = 0; i < count; i++) {
				Item item = SpikeTable [i];
				if (item.active == false) {
					item.active = true;
					item.gameObject.SetActive (true);
					return item.gameObject;
				}
			}
		} else if (objName == "SallyPath") {
			if (SallyPathTable == null)
				return null;
			int count = SallyPathTable.Length;
			
			for (int i = 0; i < count; i++) {
				Item item = SallyPathTable [i];
				if (item.active == false) {
					item.active = true;
					item.gameObject.SetActive (true);
					return item.gameObject;
				}
			}
		}

		Debug.LogError("Pool is Full!!!!");
        return null;
    }
    //--------------------------------------------------------------------------------------
    // 아이템 사용종료 - 사용하던 객체를 쉬게한다.
    // gameOBject : NewItem으로 얻었던 객체
    //--------------------------------------------------------------------------------------
    public void RemoveItem(GameObject gameObject, string objName)
    {
		if (objName == "Box") {
			if (boxTable == null || gameObject == null)
				return;
			int count = boxTable.Length;

			for (int i = 0; i < count; i++) {
				Item item = boxTable [i];
				if (item.gameObject == gameObject) {
					item.active = false;
					item.gameObject.GetComponent<ColliderChker>().m_bCollided = false;
					item.gameObject.GetComponent<ColliderChker>().m_bLeftDown = false;
					item.gameObject.GetComponent<ColliderChker>().m_bRightDown = false;
					item.gameObject.GetComponent<ColliderChker>().m_bRightUp = false;
					item.gameObject.GetComponent<ColliderChker>().m_bLeftUp = false;
					item.gameObject.SetActive (false);
					break;
				}
			}
		} else if (objName == "Floor") {
			if (floorTable == null || gameObject == null)
				return;
			int count = floorTable.Length;
			
			for (int i = 0; i < count; i++) {
				Item item = floorTable [i];
				if (item.gameObject == gameObject) {
					item.active = false;
					item.gameObject.SetActive (false);
					break;
				}
			}
		} else if (objName == "Spike") {
			if (SpikeTable == null || gameObject == null)
				return;
			int count = SpikeTable.Length;
			
			for (int i = 0; i < count; i++) {
				Item item = SpikeTable [i];
				if (item.gameObject == gameObject) {
					item.active = false;
					item.gameObject.SetActive (false);
					break;
				}
			}
		} else if (objName == "SallyPath") {
			if (SallyPathTable == null || gameObject == null)
				return;
			int count = SallyPathTable.Length;
			
			for (int i = 0; i < count; i++) {
				Item item = SallyPathTable [i];
				if (item.gameObject == gameObject) {
					item.active = false;
					item.gameObject.SetActive (false);
					break;
				}
			}
		}

		ObjectPool.getInstance.RemoveInPool(gameObject, objName);
    }
    //--------------------------------------------------------------------------------------
    // 모든 아이템 사용종료 - 모든 객체를 쉬게한다.
    //--------------------------------------------------------------------------------------
	public void ClearItem(string objName)
    {
		if (objName == "Box") {
			if (boxTable == null)
				return;
			int count = boxTable.Length;

			for (int i = 0; i < count; i++) {
				Item item = boxTable [i];
				if (item != null && item.active) {
					item.active = false;
					item.gameObject.transform.parent = GameObject.Find (objName+"Pool").transform;
					item.gameObject.SetActive (false);
				}
			}
		} else if (objName == "Floor") {
			if (floorTable == null)
				return;
			int count = floorTable.Length;
			
			for (int i = 0; i < count; i++) {
				Item item = floorTable [i];
				if (item != null && item.active) {
					item.active = false;
					item.gameObject.transform.parent = GameObject.Find (objName+"Pool").transform;
					item.gameObject.SetActive (false);
				}
			}
		} else if (objName == "Spike") {
			if (SpikeTable == null)
				return;
			int count = SpikeTable.Length;
			
			for (int i = 0; i < count; i++) {
				Item item = SpikeTable [i];
				if (item != null && item.active) {
					item.active = false;
					item.gameObject.transform.parent = GameObject.Find (objName+"Pool").transform;
					item.gameObject.SetActive (false);
				}
			}
		} else if (objName == "SallyPath") {
			if (SallyPathTable == null)
				return;
			int count = SallyPathTable.Length;
			
			for (int i = 0; i < count; i++) {
				Item item = SallyPathTable [i];
				if (item != null && item.active) {
					item.active = false;
					item.gameObject.transform.parent = GameObject.Find (objName+"Pool").transform;
					item.gameObject.SetActive (false);
				}
			}
		}
    }
    //--------------------------------------------------------------------------------------
    // 메모리 풀 삭제
    //--------------------------------------------------------------------------------------
	public void Dispose()
    {
		if (boxTable == null)
			return;
		int count = boxTable.Length;

		for (int i = 0; i < count; i++) {
			Item item = boxTable [i];
			GameObject.Destroy (item.gameObject);
		}
		boxTable = null;


		if (floorTable == null)
			return;
		count = floorTable.Length;
		
		for (int i = 0; i < count; i++) {
			Item item = floorTable [i];
			GameObject.Destroy (item.gameObject);
		}
		floorTable = null;


		if (SpikeTable == null)
			return;
		count = SpikeTable.Length;
		
		for (int i = 0; i < count; i++) {
			Item item = SpikeTable [i];
			GameObject.Destroy (item.gameObject);
		}
		SpikeTable = null;

		if (SallyPathTable == null)
			return;
		count = SallyPathTable.Length;
		
		for (int i = 0; i < count; i++) {
			Item item = SallyPathTable [i];
			GameObject.Destroy (item.gameObject);
		}
		SallyPathTable = null;


    }

}
