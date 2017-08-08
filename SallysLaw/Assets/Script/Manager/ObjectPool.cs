using UnityEngine;
using System.Collections;

public class ObjectPool : MonoBehaviour
{
	private static ObjectPool instance;


	void Awake(){
		if (instance == null)
			instance = this;
		
		else if (instance != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad (gameObject);
		ObjectPoolSetting ();
	}

	void OnApplicationQuit()
	{
		pool.Dispose();//메모리 풀 삭제

		instance = null;
	}

	
	public static ObjectPool getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(ObjectPool)) as ObjectPool;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("ObjectPool");
				instance = obj.AddComponent (typeof(ObjectPool)) as ObjectPool;
			}
			
			return instance;
		}
	}
	public MemoryPool pool = new MemoryPool();

	public GameObject[] boxInPool;
	public GameObject[] floorInPool;
	public GameObject[] SpikeInPool;
	public GameObject[] SallyPathInPool;

	public GameObject m_Runner;
	public GameObject m_Guardian;
	public GameObject m_Goal;
	public GameObject m_Photo;
	public GameObject m_Box;
	public GameObject m_Spike;
	public GameObject m_Door;
	public GameObject m_Switch;
	public GameObject m_Floor;
	public GameObject m_Timeball;
	public GameObject m_Spring;
	public GameObject m_Portal;
	public GameObject m_SallyPath;
	public GameObject m_Stage;
	public GameObject m_StartVehicle;
	public GameObject m_TopDeco;
	public GameObject m_InteractionProp;

	public GameObject m_TextFloat_Pos;
	public GameObject m_TextFloat_UI;

	public GameObject m_PolygonCollider;

	public Sprite[] m_sprite_sally_Chpt5Box; // forMaptool
	public Sprite[] m_sprite_father_Chpt5Box; // forMaptool
	public Sprite[] m_Sprite_Vehicles;

	public GameObject m_MegamanEffect;

	public Font m_Jua;


	public void RemoveInPool(GameObject removeObj ,string objName)
	{
		if (objName == "Box") {
			for(int i = 0 ; i < boxInPool.Length; ++i)
			{
				if(boxInPool[i] != null && boxInPool[i].gameObject == removeObj)
				{
					boxInPool[i] = null;
					return;
				}
			}
		} else if (objName == "Floor") {
			for(int i = 0 ; i < floorInPool.Length; ++i)
			{
				if(floorInPool[i] != null && floorInPool[i].gameObject == removeObj)
				{
					floorInPool[i] = null;
					return;
				}
			}
		} else if (objName == "Spike") {
			for(int i = 0 ; i < SpikeInPool.Length; ++i)
			{
				if(SpikeInPool[i] != null && SpikeInPool[i].gameObject == removeObj)
				{
					SpikeInPool[i] = null;
					return;
				}
			}
		} else if (objName == "SallyPath") {
			for(int i = 0 ; i < SallyPathInPool.Length; ++i)
			{
				if(SallyPathInPool[i] != null && SallyPathInPool[i].gameObject == removeObj)
				{
					SallyPathInPool[i] = null;
					return;
				}
			}
		}
	}

	public void MemoryPoolCreate()
	{
		pool.Create(m_Box, 1500, "Box" );
		boxInPool = new GameObject[1500];

		pool.Create(m_Floor, 1250, "Floor" );
		floorInPool = new GameObject[1250];

		pool.Create(m_Spike, 200, "Spike" );
		SpikeInPool = new GameObject[200];

		pool.Create(m_SallyPath, 400, "SallyPath" );
		SallyPathInPool = new GameObject[400];

		for(int i = 0; i < boxInPool.Length; ++i)
		{
			boxInPool[i] = null;
		}

		for(int i = 0; i < floorInPool.Length; ++i)
		{
			floorInPool[i] = null;
		}

		for(int i = 0; i < SpikeInPool.Length; ++i)
		{
			SpikeInPool[i] = null;
		}

		for(int i = 0; i < SallyPathInPool.Length; ++i)
		{
			SallyPathInPool[i] = null;
		}
	}


	public void ObjectPoolSetting ()
	{
		m_Runner = Resources.Load ("Prefabs/Objects/Players/Runner") as GameObject;
		m_Guardian = Resources.Load ("Prefabs/Objects/Players/Guardian") as GameObject;
		m_Goal = Resources.Load ("Prefabs/Objects/Goal/R_Goal") as GameObject;
		m_Photo = Resources.Load ("Prefabs/Objects/Photo") as GameObject;

		m_SallyPath = Resources.Load ("Prefabs/Objects/SallyPath") as GameObject;

		m_Box = Resources.Load ("Prefabs/Objects/Boxes/Box") as GameObject;
		m_Spike = Resources.Load ("Prefabs/Objects/Spike") as GameObject;
		m_Door = Resources.Load ("Prefabs/Objects/Doors/R_Door") as GameObject;
		m_Floor = Resources.Load ("Prefabs/Objects/Boxes/Floor") as GameObject;
		m_Spring = Resources.Load ("Prefabs/Objects/Spring") as GameObject;

		m_Switch = Resources.Load ("Prefabs/Objects/Switchs/R_Switch") as GameObject;
		m_Timeball = Resources.Load ("Prefabs/Objects/TimeCapsule") as GameObject;
		m_Portal = Resources.Load ("Prefabs/Objects/Portals/Portal") as GameObject;
		m_Stage = Resources.Load ("Prefabs/Objects/Stage") as GameObject;
		m_StartVehicle = Resources.Load ("Prefabs/Objects/StartVehicle") as GameObject;
		m_TopDeco = Resources.Load ("Prefabs/Objects/Boxes/TopDeco") as GameObject;
		m_InteractionProp = Resources.Load ("Prefabs/Objects/InteractionProp") as GameObject;

		m_PolygonCollider = Resources.Load ("Prefabs/Objects/PolyCollider") as GameObject;

		m_TextFloat_Pos = Resources.Load ("Prefabs/Objects/TextFloat_Pos") as GameObject;
		m_TextFloat_UI = Resources.Load ("Prefabs/UI/TextFloat_UI") as GameObject;

		m_MegamanEffect = Resources.Load ("Prefabs/Objects/Players/MegamanEffect") as GameObject;

		//temporary///////////////////
		m_sprite_sally_Chpt5Box = Resources.LoadAll<Sprite>("Sprites/2726x1536/Chapter5/SallyBG/block_chapter05_all");
		m_sprite_father_Chpt5Box = Resources.LoadAll<Sprite>("Sprites/2726x1536/Chapter5/FatherBG/block_chapter05_all");
		m_Sprite_Vehicles = Resources.LoadAll<Sprite>("Sprites/Objects/Common/Vehicles");
		//temporary///////////////////

		m_Jua = Resources.Load ("BMJUA_ttf") as Font;

		AudioMgr.getInstance.AudioPoolSetting ();
	}
}