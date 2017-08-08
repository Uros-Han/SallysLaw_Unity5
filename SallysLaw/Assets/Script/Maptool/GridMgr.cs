using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridMgr : MonoBehaviour {
	private static GridMgr instance;

	
	public static GridMgr getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(GridMgr)) as GridMgr;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("GridMgr");
				instance = obj.AddComponent (typeof(GridMgr)) as GridMgr;
			}
			
			return instance;
		}
	}

	void Awake(){
		if (instance == null)
			instance = this;
		
		else if (instance != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad (gameObject);
	}
	
	void OnDestroy()
	{
		
		instance = null;
		
	}

	public int m_iGridIdx;

	public int m_iXcount; //  Recommand NOT Even!
	public int m_iYcount; //  Recommand NOT Even!

	public Vector2 m_fStartPos;

	public float m_fXsize;
	public float m_fYsize;

	MapToolMgr m_MapToolMgr;

	// Use this for initialization
	void Start () {
		m_MapToolMgr = GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ();
		LineMaking ();
	}
	
	// Update is called once per frame
	void Update () {
		Picking ();

		//GameObject.Find("DebugTile").transform.position = IdxPos(m_iGridIdx);
	}

	void LineMaking()
	{
		m_fStartPos = new Vector2 ( -1 * (m_iXcount * m_fXsize) / 2 , (m_iYcount * m_fYsize) / 2 );
		
		int iTmpCounter = 0;
		for (int i = 0; i < m_iXcount+m_iYcount+2; ++i) {
			GameObject tmpLinePref = Resources.Load ("Prefabs/Objects/Debug/DebugLine") as GameObject;
			GameObject tmpLine = Instantiate(tmpLinePref) as GameObject;
			tmpLine.transform.parent = GameObject.Find ("Grids").transform;
			
			
			float fStartXPos = ((m_iXcount-1)/2) * -1 * m_fXsize - m_fXsize/2;
			float fStartYPos = ((m_iYcount-1)/2) * m_fYsize + m_fYsize * 3/2;
			
			if(iTmpCounter <= m_iXcount)
				tmpLine.GetComponent<DebugLine>().Init(new Vector3(fStartXPos + (i* m_fXsize), 0),true,m_fXsize,m_iXcount,m_iYcount);
			else
				tmpLine.GetComponent<DebugLine>().Init(new Vector3(0, fStartYPos - ( (i-m_iXcount) * m_fYsize)),false,m_fXsize,m_iXcount,m_iYcount);

			//바닥라인 (플로어생기는 칸) 초록색으로
			if(i == m_iXcount+m_iYcount+1 || i == m_iXcount+m_iYcount)
				tmpLine.GetComponent<SpriteRenderer>().color = Color.green;

			switch(i)
			{
			case 25:
				tmpLine.GetComponent<SpriteRenderer>().color = Color.red;
				break;
			case 50:
				tmpLine.GetComponent<SpriteRenderer>().color = Color.yellow;
				break;
			case 75:
				tmpLine.GetComponent<SpriteRenderer>().color = Color.blue;
				break;
			case 100:
				tmpLine.GetComponent<SpriteRenderer>().color = Color.yellow;
				break;
			case 125:
				tmpLine.GetComponent<SpriteRenderer>().color = Color.red;
				break;
			}
			
			iTmpCounter += 1;
		}

		GameObject.Find ("GridLimit").GetComponent<BoxCollider2D> ().size = new Vector2 (m_fXsize * m_iXcount, m_fYsize * m_iYcount);
	}

	public Vector3 GetCenterOfThisMap()
	{
		int iLeftestIdx = m_iXcount - 1;
		int iRightestIdx = 0;
		
		for (int i = 0; i < m_MapToolMgr.m_listObjects.Count; ++i) {
			if(m_MapToolMgr.m_listObjects[i].m_iIdx[0] % m_iXcount < iLeftestIdx)
				iLeftestIdx = m_MapToolMgr.m_listObjects[i].m_iIdx[0] % m_iXcount;
			
			if(m_MapToolMgr.m_listObjects[i].m_iIdx[0] % m_iXcount > iRightestIdx)
				iRightestIdx = m_MapToolMgr.m_listObjects[i].m_iIdx[0] % m_iXcount;
		}

		Vector3 tmpPos;
		if((iRightestIdx - iLeftestIdx) % 2 == 0) ////Not Even Width block count
			tmpPos = IdxPos(iLeftestIdx + ((iRightestIdx - iLeftestIdx) / 2 ));
		else //// Even Width block count
			tmpPos = IdxPos(iLeftestIdx + ((iRightestIdx - iLeftestIdx) / 2 )) + new Vector3(0.25f,0);

		tmpPos = new Vector3 (tmpPos.x, 0);

		return tmpPos;
	}

	public float GetLeftiestOfThisMap()
	{
		float fLeftiestXPos = 9999;
		float fTmpXPos = 0;

		for(int i = 0 ; i < m_MapToolMgr.m_listObjects.Count; ++i)
		{
			fTmpXPos = m_MapToolMgr.m_listObjects[i].m_Object.transform.position.x;

			if(fLeftiestXPos > fTmpXPos)
				fLeftiestXPos = fTmpXPos;
		}

		return fLeftiestXPos;
	}

	public int GetWidthOfIndex_ThisMap()
	{

		int iLeftestIdx = m_iXcount - 1;
		int iRightestIdx = 0;

		for (int i = 0; i < m_MapToolMgr.m_listObjects.Count; ++i) {
			if(m_MapToolMgr.m_listObjects[i].m_iIdx[0] % m_iXcount < iLeftestIdx)
				iLeftestIdx = m_MapToolMgr.m_listObjects[i].m_iIdx[0] % m_iXcount;

			if(m_MapToolMgr.m_listObjects[i].m_iIdx[0] % m_iXcount > iRightestIdx)
				iRightestIdx = m_MapToolMgr.m_listObjects[i].m_iIdx[0] % m_iXcount;
		}

		return iRightestIdx - iLeftestIdx + 1;
	}

	public List<int> GetAll_VacantUpperIdx(int iIdx)
	{
		List<int> tmpList = new List<int> ();
		int iFirstIdx = iIdx;
		int iBottomIdx = 0;

		for (int i = iFirstIdx/m_iXcount; i < m_iYcount-1; ++i) {
			if(GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().SeatTakenCheck(iIdx))
			{
				iIdx -= m_iXcount;
				break;
			}
			else
				iIdx += m_iXcount;
		}

		if(GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().SeatTakenCheck(iIdx))
			iIdx -= m_iXcount;

		iBottomIdx = iIdx;
		// now iIdx is chosen Grid Idx's bottom

		for (int i =0; i < iBottomIdx/m_iXcount + 1; ++i) {
			if(!GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().SeatTakenCheck(iIdx))
			{
				tmpList.Add(iIdx);
				iIdx -= m_iXcount;
			}else
				break;
		}

		return tmpList;
    }

	public int GetCount_BelowYIdxes(int iIdx)
	{
		int tmpCounter = 0;
		iIdx += m_iXcount;

		while(iIdx < m_iXcount * m_iYcount)
		{
			iIdx += m_iXcount;
			tmpCounter += 1;
		}
		return tmpCounter;
	}

	public List<int> GetAll_VacantLowerIdx(int iIdx)
	{
		List<int> tmpList = new List<int> ();
		iIdx += m_iXcount;

		while(iIdx < m_iXcount * m_iYcount)
		{
			if(!GameObject.Find("MapToolMgr").GetComponent<MapToolMgr>().SeatTakenCheck(iIdx))
				tmpList.Add(iIdx);
			iIdx += m_iXcount;
		}

		return tmpList;
	}

	public List<int> GetDoor_AllIdxs(Vector2 vPos, int iSize)
	{
		List<int> iIdxList = new List<int> ();

		for (int i = 0; i < iSize; ++i) {
			iIdxList.Add (GetGridIdx(vPos) - (i * m_iXcount));
		}


		return iIdxList;
	}

	void Picking()
	{
		Vector2 vPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		m_iGridIdx =    (int)(System.Math.Round((vPosition.y-m_fStartPos.y + (m_fYsize/2)) / m_fYsize) * m_iXcount * -1)+ (int)(System.Math.Round((vPosition.x - m_fStartPos.x- (m_fXsize/2)) / m_fXsize));

	}

	public bool ValidIdxCheck(Vector3 vPosition = default(Vector3))
	{
		if (vPosition == default(Vector3)) {
			if (m_iGridIdx >= 0 && m_iGridIdx < m_iYcount * m_iXcount)
				return true;
			else
				return false;
		} else {
			if (GetGridIdx(vPosition) >= 0 && GetGridIdx(vPosition) < m_iYcount * m_iXcount)
				return true;
			else
				return false;
		}
	}

	public int GetGridIdx(Vector2 vPosition)
	{
		m_fStartPos = new Vector2 ( -1 * (m_iXcount * m_fXsize) / 2 , (m_iYcount * m_fYsize) / 2 );

		int tmpidx = (int)(System.Math.Round((vPosition.y-m_fStartPos.y + (m_fYsize/2)) / m_fYsize) * m_iXcount * -1)+ (int)(System.Math.Round((vPosition.x - m_fStartPos.x- (m_fXsize/2)) / m_fXsize));
		return tmpidx;
	}

	public Vector3 IdxPos(int m_iIdx = -1)
	{
		if(m_iIdx == -1) // This Grid
			return new Vector3(((float)(m_iGridIdx % m_iXcount) * m_fXsize) + (m_fXsize/2) + m_fStartPos.x, (float)(m_iGridIdx / m_iXcount) * -1 * m_fXsize - (m_fYsize/2) + m_fStartPos.y);
		else // Know to Want Grid
			return new Vector3(((float)(m_iIdx % m_iXcount) * m_fXsize) + (m_fXsize/2) + m_fStartPos.x, (float)(m_iIdx / m_iXcount) * -1 * m_fXsize - (m_fYsize/2) + m_fStartPos.y);

		return Vector3.zero;
	}
}
