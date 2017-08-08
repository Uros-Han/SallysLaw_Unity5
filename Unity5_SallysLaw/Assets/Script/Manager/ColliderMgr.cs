using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderMgr : MonoBehaviour {

	private static ColliderMgr instance;
	
	public static ColliderMgr getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(ColliderMgr)) as ColliderMgr;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("ColliderMgr");
				instance = obj.AddComponent (typeof(ColliderMgr)) as ColliderMgr;
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

	enum PointDir { UP, DOWN, RIGHT, LEFT, END };
	

	/// <summary>
	/// 모든 박스들을 검사.
	/// </summary>
	/// <returns>The all boxes.</returns>
	public void CheckAllBoxes()
	{
//		DisableAllCollider ();

		Transform BoxParent = null;

		if(SceneStatus.getInstance.m_objCurStage != null)
			BoxParent = SceneStatus.getInstance.m_objCurStage.transform.Find ("Boxes").transform;
		else
			BoxParent = GameObject.Find ("Boxes").transform;

		//폴리곤 콜라이더에 SetPath해줄 vector3 리스트
		List<Vector2> polygonPoints = new List<Vector2> ();



		//덩어리 만들기
		for(int i = 0; i < BoxParent.childCount; ++i)
		{
			CheckSurround(BoxParent.GetChild(i).position, BoxParent.GetChild(i).GetComponent<ColliderChker>());


			if(BoxParent.GetChild(i).GetComponent<ColliderChker>().m_bCollided == false)
			{
				Vector2 tmpPoint = BoxParent.GetChild(i).position - new Vector3(0.25f, 0.25f);

				//사방이 박스로 둘러쌓인 위치, 다음 박스 검사
				if(CheckPointDirection(tmpPoint, PointDir.END) == PointDir.END)
					continue;

				polygonPoints = DrawOuterLine(tmpPoint);

				//폴리곤 콜라이더 생성
				GameObject tmpPoly = Instantiate(ObjectPool.getInstance.m_PolygonCollider) as GameObject;
				tmpPoly.transform.parent = GameObject.Find("Colliders").transform;
				//폴리곤 콜라이더 SetPath
				tmpPoly.GetComponent<PolygonCollider2D>().SetPath(0, polygonPoints.ToArray());

				for(int j = 0; j < BoxParent.childCount; ++j)
				{
					BoxParent.GetChild(j).GetComponent<ColliderChker>().CheckCollide();
				}

				polygonPoints.Clear();
			}
		}

		//도넛 모양으로 뚫기
		for (int i = 0; i < BoxParent.childCount; ++i) {

			ColliderChker tmpChker = BoxParent.GetChild(i).GetComponent<ColliderChker>();
			Vector3 startPos = Vector3.zero;
			bool bOnDonut = false;

			//꼭지점 중에 하나라도 false이면
			if(!tmpChker.m_bLeftDown){
				startPos = BoxParent.GetChild(i).position + new Vector3(-0.25f, -0.25f);
				polygonPoints = DrawOuterLine(startPos);
				bOnDonut = true;
			}else if(!tmpChker.m_bRightDown){
				startPos = BoxParent.GetChild(i).position + new Vector3(0.25f, -0.25f);
				polygonPoints = DrawOuterLine(startPos);
				bOnDonut = true;
			}else if(!tmpChker.m_bRightUp){
				startPos = BoxParent.GetChild(i).position + new Vector3(0.25f, 0.25f);
				polygonPoints = DrawOuterLine(startPos);
				bOnDonut = true;
			}else if(!tmpChker.m_bLeftUp){
				startPos = BoxParent.GetChild(i).position + new Vector3(-0.25f, 0.25f);
				polygonPoints = DrawOuterLine(startPos);
				bOnDonut = true;
			}

			//도넛모양임. 위에서 폴리곤 포인트리스트 구햇으니 그걸로 구멍뚫자
			if(bOnDonut)
			{
				if(Physics2D.Raycast (startPos, -Vector3.forward, 0.1f)){
					
					RaycastHit2D hit = Physics2D.Raycast (startPos, -Vector3.forward, 0.1f);
					
					if(hit.transform.gameObject.name == "PolyCollider(Clone)")
					{
						hit.transform.gameObject.GetComponent<PolygonCollider2D>().pathCount += 1;
						hit.transform.gameObject.GetComponent<PolygonCollider2D>().SetPath(hit.transform.gameObject.GetComponent<PolygonCollider2D>().pathCount -1 , polygonPoints.ToArray());
					}
				}

				polygonPoints.Clear();
			}
		}
	}


	/// <summary>
	/// 바깥 지점 한바퀴 빙 둘름. 폴리곤 콜라이더 만드는데 필요한 꼭지점들 반환.
	/// </summary>
	/// <returns>The outer line.</returns>
	List<Vector2> DrawOuterLine(Vector3 startPos)
	{
		//최초 포인트 위치
		Vector3 StartPoint = Vector3.zero;

		//return value
		List<Vector2> tmpPoints = new List<Vector2> ();

		Vector3 point = Vector3.zero;

		PointDir beforeDir = PointDir.END;
		PointDir pointDir = PointDir.END;



		point = startPos;

		//최초위치 저장
		StartPoint = point;
		
		//최초위치로 다시 돌아올때까지 포인트 뺑뻉이 돌리면서 예비 폴리곤인덱스 리스트 애드해줌
		do{
			
			//다음 방향 탐색
			pointDir = CheckPointDirection(point, pointDir);
			
			//꼭지점이면 현재위치 저장
			if(beforeDir != pointDir)
				tmpPoints.Add(point);
			
			//방향으로 포인트 이동
			switch(pointDir){
			case PointDir.UP:
				point += new Vector3(0, 0.5f);
				break;
				
			case PointDir.DOWN:
				point += new Vector3(0, -0.5f);
				break;
				
			case PointDir.LEFT:
				point += new Vector3(-0.5f, 0);
				break;
				
			case PointDir.RIGHT:
				point += new Vector3(0.5f, 0);
				break;
				
			case PointDir.END:
				Debug.LogError("Collider Direction Error");
				break;
			}
			
			beforeDir = pointDir;
			
		} while(StartPoint != point);

		return tmpPoints;
	}

	public void DisableAllCollider()
	{
		Transform ColliderParent = GameObject.Find("Colliders").transform;

		for(int i = 0; i < ColliderParent.childCount; ++i)
		{
			ColliderParent.GetChild(i).gameObject.SetActive(false);
		}

		Transform BoxParent = GameObject.Find ("Boxes").transform;
		
		for (int i = 0; i < BoxParent.childCount; ++i) {
			ColliderChker colChecker = BoxParent.GetChild(i).GetComponent<ColliderChker>();

			colChecker.m_bCollided = false;
			colChecker.m_bLeftDown = false;
			colChecker.m_bRightDown = false;
			colChecker.m_bRightUp = false;
			colChecker.m_bLeftUp = false;
		}

	}

	/// <summary>
	/// 주변 4타일 검사해서 박스가 주변에 있으면 내꺼 4개의 꼭지점 정보 수정
	/// </summary>
	void CheckSurround(Vector3 pos, ColliderChker collideChker)
	{
		GameObject RightObj = FindBoxAtPosition (pos + new Vector3 (0.5f, 0f));
		GameObject LefttObj = FindBoxAtPosition (pos + new Vector3 (-0.5f, 0f));
		GameObject UpObj = FindBoxAtPosition (pos + new Vector3 (0f, 0.5f));
		GameObject DownObj = FindBoxAtPosition (pos + new Vector3 (0f, -0.5f));
		GameObject LeftDownObj = FindBoxAtPosition (pos + new Vector3 (-0.5f, -0.5f));
		GameObject RightDownObj = FindBoxAtPosition (pos + new Vector3 (0.5f, -0.5f));
		GameObject RightUpObj = FindBoxAtPosition (pos + new Vector3 (0.5f, 0.5f));
		GameObject LeftUpObj = FindBoxAtPosition (pos + new Vector3 (-0.5f, 0.5f));

		//오른쪽 아래 검사
		if (RightObj != null && RightDownObj != null && DownObj != null) {
			collideChker.m_bRightDown = true;
		}

		//오른쪽 위 검사
		if (RightObj != null && RightUpObj != null && UpObj != null) {
			collideChker.m_bRightUp = true;
		}

		//왼쪽 위 검사
		if (LefttObj != null && LeftUpObj != null && UpObj != null) {
			collideChker.m_bLeftUp = true;
		}

		//왼쪽 아래 검사
		if (LefttObj != null && LeftDownObj != null && DownObj != null) {
			collideChker.m_bLeftDown = true;
		}
	}


	/// <summary>
	/// 포인트가 다음으로 이동해야할 위치를 반환해준다.
	/// </summary>
	/// <returns>The point direction.</returns>
	/// <param name="point">Point position.</param>
	PointDir CheckPointDirection(Vector3 point, PointDir pointDir)
	{
		PointDir tmpDir = PointDir.END;

		GameObject rightUpObj = FindBoxAtPosition (point + new Vector3 (0.25f, 0.25f));
		GameObject rightDownObj = FindBoxAtPosition (point + new Vector3 (0.25f, -0.25f));
		GameObject LeftUpObj = FindBoxAtPosition (point + new Vector3 (-0.25f, 0.25f));
		GameObject LeftDownObj = FindBoxAtPosition (point + new Vector3 (-0.25f, -0.25f));

		//방향 검사
		if (rightUpObj != null && rightDownObj != null && LeftUpObj != null && LeftDownObj != null) {
			tmpDir = PointDir.END;
		}else if (rightUpObj != null && rightDownObj != null && LeftUpObj != null && LeftDownObj == null) {
			rightUpObj.GetComponent<ColliderChker>().m_bLeftDown = true;
			LeftUpObj.GetComponent<ColliderChker>().m_bRightDown = true;
			rightDownObj.GetComponent<ColliderChker>().m_bLeftUp = true;
			tmpDir = PointDir.DOWN;
		}else if (rightUpObj != null && rightDownObj != null && LeftUpObj == null && LeftDownObj != null) {
			rightUpObj.GetComponent<ColliderChker>().m_bLeftDown = true;
			rightDownObj.GetComponent<ColliderChker>().m_bLeftUp = true;
			LeftDownObj.GetComponent<ColliderChker>().m_bRightUp = true;
			tmpDir = PointDir.LEFT;
		}else if (rightUpObj != null && rightDownObj != null && LeftUpObj == null && LeftDownObj == null) {
			rightUpObj.GetComponent<ColliderChker>().m_bLeftDown = true;
			rightDownObj.GetComponent<ColliderChker>().m_bLeftUp = true;
			tmpDir = PointDir.DOWN;
		}else if (rightUpObj != null && rightDownObj == null && LeftUpObj != null && LeftDownObj != null) {
			rightUpObj.GetComponent<ColliderChker>().m_bLeftDown = true;
			LeftUpObj.GetComponent<ColliderChker>().m_bRightDown = true;
			LeftDownObj.GetComponent<ColliderChker>().m_bRightUp = true;
			tmpDir = PointDir.RIGHT;
		}else if (rightUpObj != null && rightDownObj == null && LeftUpObj != null && LeftDownObj == null) {
			rightUpObj.GetComponent<ColliderChker>().m_bLeftDown = true;
			LeftUpObj.GetComponent<ColliderChker>().m_bRightDown = true;
			tmpDir = PointDir.RIGHT;
		}else if (rightUpObj != null && rightDownObj == null && LeftUpObj == null && LeftDownObj != null) {
			if(pointDir == PointDir.DOWN)
			{
				rightUpObj.GetComponent<ColliderChker>().m_bLeftDown = true;
				tmpDir = PointDir.RIGHT;
			}
			else if(pointDir == PointDir.UP)
			{
				LeftDownObj.GetComponent<ColliderChker>().m_bRightUp = true;
				tmpDir = PointDir.LEFT;
			}
			else
				Debug.LogError("Collider Dir Error");
		}else if (rightUpObj != null && rightDownObj == null && LeftUpObj == null && LeftDownObj == null) {
			rightUpObj.GetComponent<ColliderChker>().m_bLeftDown = true;
			tmpDir = PointDir.RIGHT;
		}else if (rightUpObj == null && rightDownObj != null && LeftUpObj != null && LeftDownObj != null) {
			rightDownObj.GetComponent<ColliderChker>().m_bLeftUp = true;
			LeftUpObj.GetComponent<ColliderChker>().m_bRightDown = true;
			LeftDownObj.GetComponent<ColliderChker>().m_bRightUp = true;
			tmpDir = PointDir.UP;
		}else if (rightUpObj == null && rightDownObj != null && LeftUpObj != null && LeftDownObj == null) {
			if(pointDir == PointDir.LEFT)
			{
				rightDownObj.GetComponent<ColliderChker>().m_bLeftUp = true;
				tmpDir = PointDir.DOWN;
			}
			else if(pointDir == PointDir.RIGHT)
			{
				LeftUpObj.GetComponent<ColliderChker>().m_bRightDown = true;
				tmpDir = PointDir.UP;
			}
			else
				Debug.LogError("Collider Dir Error");
		}else if (rightUpObj == null && rightDownObj != null && LeftUpObj == null && LeftDownObj != null) {
			rightDownObj.GetComponent<ColliderChker>().m_bLeftUp = true;
			LeftDownObj.GetComponent<ColliderChker>().m_bRightUp = true;
			tmpDir = PointDir.LEFT;
		}else if (rightUpObj == null && rightDownObj != null && LeftUpObj == null && LeftDownObj == null) {
			rightDownObj.GetComponent<ColliderChker>().m_bLeftUp = true;
			tmpDir = PointDir.DOWN;
		}else if (rightUpObj == null && rightDownObj == null && LeftUpObj != null && LeftDownObj != null) {
			LeftUpObj.GetComponent<ColliderChker>().m_bRightDown = true;
			LeftDownObj.GetComponent<ColliderChker>().m_bRightUp = true;
			tmpDir = PointDir.UP;
		}else if (rightUpObj == null && rightDownObj == null && LeftUpObj != null && LeftDownObj == null) {
			LeftUpObj.GetComponent<ColliderChker>().m_bRightDown = true;
			tmpDir = PointDir.UP;
		}else if (rightUpObj == null && rightDownObj == null && LeftUpObj == null && LeftDownObj != null) {
			LeftDownObj.GetComponent<ColliderChker>().m_bRightUp = true;
			tmpDir = PointDir.LEFT;
		}else if (rightUpObj == null && rightDownObj == null && LeftUpObj == null && LeftDownObj == null) {
			Debug.LogError("Collider Dir Error");
		}

		return tmpDir;
	}

	/// <summary>
	/// Finds the box at position.
	/// </summary>
	/// <returns>The box at position.</returns>
	/// <param name="position">Position.</param>
	GameObject FindBoxAtPosition(Vector3 position)
	{
		Transform BoxParent = null;
		
		if(SceneStatus.getInstance.m_objCurStage != null)
			BoxParent = SceneStatus.getInstance.m_objCurStage.transform.Find ("Boxes").transform;
		else
			BoxParent = GameObject.Find ("Boxes").transform;

		for (int i = 0; i < BoxParent.childCount; ++i) {
			if(Vector3.Distance(BoxParent.GetChild(i).position, position) < 0.01f)
				return BoxParent.GetChild(i).gameObject;
		}

		return null;
	}

}
