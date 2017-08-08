using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoftBody : MonoBehaviour {

	public float m_fSize;
	float m_fQuadSize;


	Mesh m_Mesh;


	// Use this for initialization
	void Awake () {
		for (int i = 0; i < transform.childCount; ++i) {

			Physics2D.IgnoreCollision( GetComponent<CircleCollider2D>(),  transform.GetChild(i).GetComponent<CircleCollider2D>());

			if(i != transform.childCount-1)
				Physics2D.IgnoreCollision( transform.GetChild(i).GetComponent<CircleCollider2D>(),  transform.GetChild(i+1).GetComponent<CircleCollider2D>());
			else
				Physics2D.IgnoreCollision( transform.GetChild(i).GetComponent<CircleCollider2D>(),  transform.GetChild(0).GetComponent<CircleCollider2D>());
		}

		GetComponent<MeshFilter> ().mesh = CreateMesh();
		m_Mesh = GetComponent<MeshFilter> ().mesh;



		StartCoroutine (VertexMapping ());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator VertexMapping()
	{
		float fColliderRadius = transform.GetChild(0).GetComponent<CircleCollider2D>().radius;
		List<Transform> colTransform = new List<Transform> ();
		//Vector3[] vert;

		for (int i= 0; i < transform.childCount; ++i) {
			colTransform.Add(transform.GetChild(i).transform);
		}



		do {
			yield return new WaitForEndOfFrame();

			//vert = m_Mesh.vertices;

			Vector3[] vert = new Vector3[m_Mesh.vertices.Length];

			float fAngle = ContAngle(new Vector3(0,1), colTransform[0].localPosition);
			if(colTransform[0].localPosition.x > 0)
				fAngle = 360f - fAngle;

			fAngle *= Mathf.Deg2Rad;

			//float fWidth = colTransform[2].localPosition.x - colTransform[6].localPosition.x;
			//float fHeight = colTransform[4].localPosition.y - colTransform[2].localPosition.y;

			vert[0]	= new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius, 					Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius, 1);
			vert[1]	= new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize,	 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius, 1);
			vert[2] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) + (m_fQuadSize), 					 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius, 1);
			vert[3] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) - (m_fQuadSize), 					 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius, 1);
			vert[4] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 	 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius, 1);
			vert[5] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius, 				 		Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius, 1);


			vert[6]	= new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius, 					Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 1);
			vert[7]	= new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize,	 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 1);
			vert[8] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) + (m_fQuadSize), 					 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 1);
			vert[9] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) - (m_fQuadSize), 					 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 1);
			vert[10] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 	 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 1);
			vert[11] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius, 				 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 1);


			vert[12] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius, 					Vector3.Distance(colTransform[0].localPosition, Vector3.zero) - m_fQuadSize, 1);
			vert[13] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize,	 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) - m_fQuadSize, 1);
			vert[14] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) + (m_fQuadSize), 					 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) - m_fQuadSize, 1);
			vert[15] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) - (m_fQuadSize), 					 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) - m_fQuadSize, 1);
			vert[16] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 	 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) - m_fQuadSize, 1);
			vert[17] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius, 				 	Vector3.Distance(colTransform[0].localPosition, Vector3.zero) - m_fQuadSize, 1);


			vert[18] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius, 					-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) + m_fQuadSize, 1);
			vert[19] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize,	 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) + m_fQuadSize, 1);
			vert[20] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) + (m_fQuadSize), 					 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) + m_fQuadSize, 1);
			vert[21] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) - (m_fQuadSize), 					 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) + m_fQuadSize, 1);
			vert[22] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 	 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) + m_fQuadSize, 1);
			vert[23] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius, 				 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) + m_fQuadSize, 1);


			vert[24] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius, 					-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize, 1);
			vert[25] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize,	 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize, 1);
			vert[26] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) + (m_fQuadSize), 					 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize, 1);
			vert[27] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) - (m_fQuadSize), 					 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize, 1);
			vert[28] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 	 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize, 1);
			vert[29] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius, 				 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize, 1);


			vert[30] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius, 					-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius, 1);
			vert[31] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) - fColliderRadius + m_fQuadSize,	 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius, 1);
			vert[32] = new Vector3(-Vector3.Distance(colTransform[6].localPosition, Vector3.zero) + (m_fQuadSize), 					 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius, 1);
			vert[33] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) - (m_fQuadSize), 					 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius, 1);
			vert[34] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius - m_fQuadSize, 	 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius, 1);
			vert[35] = new Vector3(Vector3.Distance(colTransform[2].localPosition, Vector3.zero) + fColliderRadius, 				 	-Vector3.Distance(colTransform[4].localPosition, Vector3.zero) - fColliderRadius, 1);


			for(int i = 0 ; i < 36 ; ++i)
			{
				vert[i] = new Vector3((vert[i].x * Mathf.Cos(fAngle)) - (vert[i].y * Mathf.Sin(fAngle)), (vert[i].x * Mathf.Sin(fAngle)) + (vert[i].y * Mathf.Cos(fAngle)) , 1);
			}



			m_Mesh.vertices = vert;
			m_Mesh.RecalculateBounds();


		} while(true);
	}

	public float ContAngle(Vector3 fwd, Vector3 targetDir)
	{
		float angle = Vector3.Angle(fwd, targetDir);
		
		if (AngleDir(fwd, targetDir, Vector3.up) == -1)
		{
			angle = 360.0f - angle;
			if( angle > 359.9999f )
				angle -= 360.0f;
			return angle;
		}
		else
			return angle;
	}
	
	public int AngleDir( Vector3 fwd, Vector3 targetDir, Vector3 up)
	{
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);
		
		if (dir > 0.0)
			return 1;
		else if (dir < 0.0)
			return -1;
		else
			return 0;
	}

	float CalAngleBetweenTwoVec(Vector3 ptPoint1, Vector3 ptPoint2, bool isClockWise)
	{
		//acos()나 asin()함수로 구하는 각도를 degree로 나타냈을때
		//그 범위는 0~180이다.
		//각도를 0~360으로 나타내기 위한 사전작업으로
		//기준점(ptPoint1)을 90도 회전시킨 점을 구한다.
		Vector3 ptRotated90Point1 = new Vector3(0,0);
		if(isClockWise)
		{
			ptRotated90Point1.x= -ptPoint1.y;
			ptRotated90Point1.y = ptPoint1.x;
		}
		else
		{
			ptRotated90Point1.x =  ptPoint1.y;
			ptRotated90Point1.y = -ptPoint1.x;
		}
		//앞서 계산한 점과 두번째 인자로 받은 점간의 각도를 구함.
		//이 각도가 90도보다 크다면 첫번째인자로 받은 기준점과 두번째 점간의 각도가
		//180도보다 크다는 것을 의미한다.
		float fAng = Mathf.Acos( (ptRotated90Point1.x * ptPoint2.x + ptRotated90Point1.y * ptPoint2.y) / 
		                  (Mathf.Sqrt( ptRotated90Point1.x * ptRotated90Point1.x + ptRotated90Point1.y * ptRotated90Point1.y ) * 
		 Mathf.Sqrt(ptPoint2.x * ptPoint2.x + ptPoint2.y * ptPoint2.y) ) )
			* 360 / Mathf.PI; 
		//fAng의 크기에 따라 두점사이의 각도를 다른 방식으로 구함.
		if(fAng > 90)
			return 360 - Mathf.Acos( (ptPoint1.x * ptPoint2.x + ptPoint1.y * ptPoint2.y) / 
			                        (Mathf.Sqrt( ptPoint1.x * ptPoint1.x + ptPoint1.y * ptPoint1.y ) * 
			 Mathf.Sqrt(ptPoint2.x * ptPoint2.x + ptPoint2.y * ptPoint2.y) ) )
				* 360 / Mathf.PI;
		else
			return Mathf.Acos( (ptPoint1.x * ptPoint2.x + ptPoint1.y * ptPoint2.y) / 
			            (Mathf.Sqrt( ptPoint1.x * ptPoint1.x + ptPoint1.y * ptPoint1.y ) * 
			 Mathf.Sqrt(ptPoint2.x * ptPoint2.x + ptPoint2.y * ptPoint2.y) ) )
				* 360 / Mathf.PI; 
	}

	Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();

		mesh.name = "SoftBody Mesh";

		m_fQuadSize = m_fSize * 2f / 5f;

		Vector3[] vertices = new Vector3[]
		{
			new Vector3( -m_fSize, m_fSize, 1), // 0
			new Vector3( -m_fSize + m_fQuadSize, m_fSize, 1), // 1
			new Vector3( -m_fQuadSize / 2, m_fSize, 1), // 2
			new Vector3( m_fQuadSize / 2, m_fSize, 1), // 3
			new Vector3( m_fSize - m_fQuadSize, m_fSize, 1), // 4
			new Vector3( m_fSize, m_fSize, 1), // 5



			new Vector3( -m_fSize, m_fSize- (m_fSize * 2/5), 1), // 6
			new Vector3( -m_fSize + m_fQuadSize, m_fSize- m_fQuadSize, 1), // 7
			new Vector3( -m_fQuadSize / 2, m_fSize- m_fQuadSize, 1), // 8
			new Vector3( m_fQuadSize / 2, m_fSize- m_fQuadSize, 1), // 9
			new Vector3( m_fSize - m_fQuadSize, m_fSize- m_fQuadSize, 1), // 10
			new Vector3( m_fSize, m_fSize- m_fQuadSize, 1), // 11



			new Vector3( -m_fSize, m_fSize * 1 / 5, 1), // 12
			new Vector3( -m_fSize + m_fQuadSize, m_fQuadSize / 2, 1), // 13
			new Vector3( -m_fQuadSize / 2, m_fQuadSize / 2, 1), // 14
			new Vector3( m_fQuadSize / 2, m_fQuadSize / 2, 1), // 15
			new Vector3( m_fSize - m_fQuadSize, m_fQuadSize / 2, 1), // 16
			new Vector3( m_fSize, m_fSize * 1 / 5, 1), // 17



			new Vector3( -m_fSize, -m_fSize * 1 / 5, 1), // 18
			new Vector3( -m_fSize + m_fQuadSize, -m_fQuadSize / 2, 1), // 19
			new Vector3( -m_fQuadSize / 2, -m_fQuadSize / 2, 1), // 20
			new Vector3( m_fQuadSize / 2, -m_fQuadSize / 2, 1), // 21
			new Vector3( m_fSize - m_fQuadSize, -m_fQuadSize / 2, 1), // 22
			new Vector3( m_fSize, -m_fSize * 1 / 5, 1), // 23



			new Vector3( -m_fSize, -m_fSize + m_fQuadSize, 1), // 24
			new Vector3( -m_fSize + m_fQuadSize, -m_fSize + m_fQuadSize, 1), // 25
			new Vector3( -m_fQuadSize / 2, -m_fSize + m_fQuadSize, 1), // 26
			new Vector3( m_fQuadSize / 2, -m_fSize + m_fQuadSize, 1), // 27
			new Vector3( m_fSize - m_fQuadSize, -m_fSize + m_fQuadSize, 1), // 28
			new Vector3( m_fSize, -m_fSize + m_fQuadSize, 1), // 29



			new Vector3( -m_fSize, -m_fSize, 1), // 30
			new Vector3( -m_fSize + m_fQuadSize, -m_fSize, 1), // 31
			new Vector3( -m_fQuadSize / 2, -m_fSize, 1), // 32
			new Vector3( m_fQuadSize / 2, -m_fSize, 1), // 33
			new Vector3( m_fSize - m_fQuadSize, -m_fSize, 1), // 34
			new Vector3( m_fSize, -m_fSize, 1), // 35
		};
		
		Vector2[] uv = new Vector2[]
		{
			new Vector2(0.0f, 1.0f),
			new Vector2(0.2f, 1.0f),
			new Vector2(0.4f, 1.0f),
			new Vector2(0.6f, 1.0f),
			new Vector2(0.8f, 1.0f),
			new Vector2(1.0f, 1.0f),



			new Vector2(0.0f, 0.8f),
			new Vector2(0.2f, 0.8f),
			new Vector2(0.4f, 0.8f),
			new Vector2(0.6f, 0.8f),
			new Vector2(0.8f, 0.8f),
			new Vector2(1.0f, 0.8f),



			new Vector2(0.0f, 0.6f),
			new Vector2(0.2f, 0.6f),
			new Vector2(0.4f, 0.6f),
			new Vector2(0.6f, 0.6f),
			new Vector2(0.8f, 0.6f),
			new Vector2(1.0f, 0.6f),


			new Vector2(0.0f, 0.4f),
			new Vector2(0.2f, 0.4f),
			new Vector2(0.4f, 0.4f),
			new Vector2(0.6f, 0.4f),
			new Vector2(0.8f, 0.4f),
			new Vector2(1.0f, 0.4f),



			new Vector2(0.0f, 0.2f),
			new Vector2(0.2f, 0.2f),
			new Vector2(0.4f, 0.2f),
			new Vector2(0.6f, 0.2f),
			new Vector2(0.8f, 0.2f),
			new Vector2(1.0f, 0.2f),



			new Vector2(0.0f, 0f),
			new Vector2(0.2f, 0f),
			new Vector2(0.4f, 0f),
			new Vector2(0.6f, 0f),
			new Vector2(0.8f, 0f),
			new Vector2(1.0f, 0f),
		};
		
		int[] triangles = new int[]
		{
			0, 1, 6,
			6, 1, 7,

			1, 2, 7,
			7, 2, 8,

			2, 3, 8,
			8, 3, 9,

			3, 4, 9,
			9, 4, 10,

			4, 5, 10,
			10, 5, 11,

			////////////

			6, 7, 12,
			12, 7, 13,

			7, 8, 13,
			13, 8, 14,

			8, 9, 14,
			14, 9, 15,

			9, 10, 15,
			15, 10, 16,

			10, 11, 16,
			16, 11, 17,

			/////////////


			12, 13, 18,
			18, 13, 19,

			13, 14, 19,
			19, 14, 20,

			14, 15, 20,
			20, 15, 21,

			15, 16, 21,
			21, 16, 22,

			16, 17, 22,
			22, 17, 23,

			/////////////

			18, 19, 24,
			24, 19, 25,

			19, 20, 25,
			25, 20, 26,

			20, 21, 26,
			26, 21, 27,

			21, 22, 27,
			27, 22, 28,

			22, 23, 28,
			28, 23, 29,

			//////////////

			24, 25, 30,
			30, 25, 31,

			25, 26, 31,
			31, 26 ,32,

			26, 27, 32,
			32, 27, 33,

			27, 28, 33,
			33, 28, 34,

			28, 29, 34,
			34, 29, 35
		};
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		
		return mesh;
	}
}
