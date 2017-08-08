using UnityEngine;
using System.Collections;

public class DeadLine : MonoBehaviour {

	public enum DEADLINE_POSITION { LEFT, BOTTOM };

	public DEADLINE_POSITION m_deadLinePos;	


	void Start()
	{
		if(m_deadLinePos == DEADLINE_POSITION.LEFT)
			GetComponent<BoxCollider2D> ().size = new Vector2 ( GetComponent<BoxCollider2D> ().size.x , 2* Camera.main.orthographicSize);
		else if(m_deadLinePos == DEADLINE_POSITION.BOTTOM)
			GetComponent<BoxCollider2D> ().size = new Vector2 ( 2* Camera.main.orthographicSize*Camera.main.aspect , GetComponent<BoxCollider2D> ().size.y);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if(SceneStatus.getInstance.GetComponent<SceneStatus>().m_enPlayerStatus == PLAYER_STATUS.RUNNER)
		{
			if(coll.gameObject.tag == "Runner")
			{
				GameMgr.getInstance.GameOver();
			}
		}
	}

}
