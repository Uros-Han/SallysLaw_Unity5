using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragBorder : MonoBehaviour {

	enum PIVOT_HORIZONTAL { LEFT, CENTER, RIGHT, END};
	enum PIVOT_VERTICAL { TOP, CENTER, BOTTOM, END};

	GridMgr gridMgr;

	public int iStartIdx = -1; // 드래그 시작되는 위치의 타일인덱스
	int iEndIdx = -1; // 드래그 끝나는 위치의 타일인덱스
	int iBeforeEndIdx;

	int iWidth = 0;
	int iHeight = 0;

	PIVOT_HORIZONTAL m_Horizon = PIVOT_HORIZONTAL.END;
	PIVOT_VERTICAL m_Vertical = PIVOT_VERTICAL.END;

	// Use this for initialization
	void Start () {
		gridMgr = GridMgr.getInstance;

		//iStartIdx = gridMgr.m_iGridIdx;

		StartCoroutine (Looping ());
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}

	public List<int> SelectedIdx()
	{
		List<int> tmpIdx = new List<int> ();

		for (int i = 0; i < iHeight; ++i) {
			for (int j =0; j < iWidth; ++j) {
				if (m_Horizon != PIVOT_HORIZONTAL.RIGHT)
					tmpIdx.Add (iStartIdx + j);
				else
					tmpIdx.Add (iStartIdx - j);
			}

			if(m_Vertical != PIVOT_VERTICAL.BOTTOM)
				iStartIdx += gridMgr.m_iXcount;
			else
				iStartIdx -= gridMgr.m_iXcount;
		}

		return tmpIdx;
	}

	IEnumerator Looping()
	{

		do 
		{
			iEndIdx = gridMgr.m_iGridIdx;

			if(iBeforeEndIdx != iEndIdx)
			{
				////////////////////스타트, 끝지점 가로방향 확인
				int iTmp = (iEndIdx % gridMgr.m_iXcount) - (iStartIdx % gridMgr.m_iXcount);

				iWidth = (iEndIdx % gridMgr.m_iXcount) - (iStartIdx % gridMgr.m_iXcount);
				if(iWidth >= 0)
					iWidth += 1;
				else
					iWidth = iWidth * -1 + 1;

	            if(iTmp > 0){
					m_Horizon = PIVOT_HORIZONTAL.LEFT;
				}else if(iTmp == 0){
					m_Horizon = PIVOT_HORIZONTAL.CENTER;
				}else
					m_Horizon = PIVOT_HORIZONTAL.RIGHT;
				///////////////////////////////////////////////////
				////////////////////스타트, 끝지점 세로방향 확인
				iTmp = (iEndIdx / gridMgr.m_iXcount) - (iStartIdx / gridMgr.m_iXcount);

				iHeight = (iEndIdx / gridMgr.m_iXcount) - (iStartIdx / gridMgr.m_iXcount);

				if(iHeight >= 0)
					iHeight += 1;
				else
					iHeight = iHeight * -1 + 1;

				if(iTmp > 0){
					m_Vertical = PIVOT_VERTICAL.TOP;
				}else if(iTmp == 0){
					m_Vertical = PIVOT_VERTICAL.CENTER;
				}else
					m_Vertical = PIVOT_VERTICAL.BOTTOM;
				///////////////////////////////////////////////////

				UIWidget.Pivot tmpPivot = UIWidget.Pivot.Center;

				if(m_Horizon == PIVOT_HORIZONTAL.LEFT && m_Vertical == PIVOT_VERTICAL.TOP)
					tmpPivot = UIWidget.Pivot.TopLeft;
				else if(m_Horizon == PIVOT_HORIZONTAL.LEFT && m_Vertical == PIVOT_VERTICAL.CENTER)
					tmpPivot = UIWidget.Pivot.Left;
				else if(m_Horizon == PIVOT_HORIZONTAL.LEFT && m_Vertical == PIVOT_VERTICAL.BOTTOM)
					tmpPivot = UIWidget.Pivot.BottomLeft;
				else if(m_Horizon == PIVOT_HORIZONTAL.CENTER && m_Vertical == PIVOT_VERTICAL.TOP)
					tmpPivot = UIWidget.Pivot.Top;
				else if(m_Horizon == PIVOT_HORIZONTAL.CENTER && m_Vertical == PIVOT_VERTICAL.CENTER)
					tmpPivot = UIWidget.Pivot.Center;
				else if(m_Horizon == PIVOT_HORIZONTAL.CENTER && m_Vertical == PIVOT_VERTICAL.BOTTOM)
					tmpPivot = UIWidget.Pivot.Bottom;
				else if(m_Horizon == PIVOT_HORIZONTAL.RIGHT && m_Vertical == PIVOT_VERTICAL.TOP)
					tmpPivot = UIWidget.Pivot.TopRight;
				else if(m_Horizon == PIVOT_HORIZONTAL.RIGHT && m_Vertical == PIVOT_VERTICAL.CENTER)
					tmpPivot = UIWidget.Pivot.Right;
				else if(m_Horizon == PIVOT_HORIZONTAL.RIGHT && m_Vertical == PIVOT_VERTICAL.BOTTOM)
					tmpPivot = UIWidget.Pivot.BottomRight;

				for(int i = 0 ; i < transform.childCount; ++i) // 아이들에게 피벗 적용하고 포지션 설정 및 스케일 설정해주기
				{
					transform.GetChild(i).GetComponent<UISprite>().pivot = tmpPivot;

					switch(transform.GetChild(i).name)
					{
					case "Left":
						if(m_Horizon == PIVOT_HORIZONTAL.LEFT)
							transform.GetChild(i).localPosition = new Vector2(-25.0f , 0);
						else if(m_Horizon == PIVOT_HORIZONTAL.CENTER)
							transform.GetChild(i).localPosition = new Vector2(-20.0f, 0);
						else
							transform.GetChild(i).localPosition = new Vector2(25.0f , 0);

						if(m_Vertical == PIVOT_VERTICAL.TOP)
							transform.GetChild(i).localPosition = new Vector2(transform.GetChild(i).localPosition.x , 25f);
						else if(m_Vertical == PIVOT_VERTICAL.CENTER)
							transform.GetChild(i).localPosition = new Vector2(transform.GetChild(i).localPosition.x , 0);
						else
							transform.GetChild(i).localPosition = new Vector2(transform.GetChild(i).localPosition.x , -25f);

						transform.GetChild(i).localScale = new Vector2(5 , iHeight * 23.5f);

						break;

					case "Top":

						if(m_Vertical == PIVOT_VERTICAL.TOP)
							transform.GetChild(i).localPosition = new Vector2(0 , 25.0f);
						else if(m_Vertical == PIVOT_VERTICAL.CENTER)
							transform.GetChild(i).localPosition = new Vector2(0, 20.0f);
						else
							transform.GetChild(i).localPosition = new Vector2(0 , -25f);
						
						if(m_Horizon == PIVOT_HORIZONTAL.LEFT)
							transform.GetChild(i).localPosition = new Vector2(-25f , transform.GetChild(i).localPosition.y);
						else if(m_Horizon == PIVOT_HORIZONTAL.CENTER)
							transform.GetChild(i).localPosition = new Vector2(0 , transform.GetChild(i).localPosition.y);
						else
							transform.GetChild(i).localPosition = new Vector2(25f , transform.GetChild(i).localPosition.y);
						
						transform.GetChild(i).localScale = new Vector2(iWidth * 23.5f , 5);

						break;
					case "Right":

						if(m_Horizon == PIVOT_HORIZONTAL.LEFT)
							transform.GetChild(i).localPosition = new Vector2(iWidth * 47f - 25f , 0);
						else if(m_Horizon == PIVOT_HORIZONTAL.CENTER)
							transform.GetChild(i).localPosition = new Vector2(20.0f, 0);
						else
							transform.GetChild(i).localPosition = new Vector2(-iWidth * 47f + 25f, 0);
						
						if(m_Vertical == PIVOT_VERTICAL.TOP)
							transform.GetChild(i).localPosition = new Vector2(transform.GetChild(i).localPosition.x , 25f);
						else if(m_Vertical == PIVOT_VERTICAL.CENTER)
							transform.GetChild(i).localPosition = new Vector2(transform.GetChild(i).localPosition.x , 0);
						else
							transform.GetChild(i).localPosition = new Vector2(transform.GetChild(i).localPosition.x , -25f);
						
						transform.GetChild(i).localScale = new Vector2(5 , iHeight * 23.5f);

						break;

					case "Bottom":

						if(m_Vertical == PIVOT_VERTICAL.TOP)
							transform.GetChild(i).localPosition = new Vector2(0 , - iHeight * 47f + 35f);
						else if(m_Vertical == PIVOT_VERTICAL.CENTER)
							transform.GetChild(i).localPosition = new Vector2(0, -20.0f);
						else
							transform.GetChild(i).localPosition = new Vector2(0 , iHeight * 47f - 35f);
						
						if(m_Horizon == PIVOT_HORIZONTAL.LEFT)
							transform.GetChild(i).localPosition = new Vector2(-25f , transform.GetChild(i).localPosition.y);
						else if(m_Horizon == PIVOT_HORIZONTAL.CENTER)
							transform.GetChild(i).localPosition = new Vector2(0 , transform.GetChild(i).localPosition.y);
						else
							transform.GetChild(i).localPosition = new Vector2(25f , transform.GetChild(i).localPosition.y);
						
						transform.GetChild(i).localScale = new Vector2(iWidth * 23.5f , 5);
						
						break;

						break;
					}
				}
			}
			iBeforeEndIdx = iEndIdx;

			yield return null;
		} while(true);
	}
}








