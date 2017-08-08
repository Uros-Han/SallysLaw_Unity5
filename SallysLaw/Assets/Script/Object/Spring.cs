using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour
{

	float fForce; // spring power

	public bool m_bBroken;
	public float m_fCoolTime;
	public float m_fCurCoolTime;
	private SkeletonAnimation skelAnim;
	SPRING_DIR m_dir;

	void Start ()
	{
		fForce = 9000f;  //mass1 = 9
		m_bBroken = false;
		m_fCoolTime = 0.1f;

		skelAnim = GetComponent<SkeletonAnimation> ();


		if (transform.rotation.eulerAngles.z > -1 && transform.rotation.eulerAngles.z < 1) {
			if(!StageLoader.getInstance.m_bMaptool)
				transform.position = new Vector3 (transform.position.x, transform.position.y - 0.25f);
			m_dir = SPRING_DIR.UP;
			transform.GetChild(1).gameObject.SetActive(true);

		} else {
//			GetComponents<SkeletonAnimation> () [0].enabled = false;

			if(!StageLoader.getInstance.m_bMaptool)
			{
				skelAnim.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/spring/wallspring_atlas/Wallspring_SkeletonData");
				skelAnim.Reset();
			}

			if (transform.rotation.eulerAngles.z > 89f && transform.rotation.eulerAngles.z < 91f) {
				if(!StageLoader.getInstance.m_bMaptool)
				{
					transform.position = new Vector3 (transform.position.x + 0.25f, transform.position.y);
					transform.rotation = Quaternion.AngleAxis (0, Vector3.forward);
				}
				m_dir = SPRING_DIR.LEFT;
			} else if (transform.rotation.eulerAngles.z > 269f && transform.rotation.eulerAngles.z < 271f) {
				if(!StageLoader.getInstance.m_bMaptool)
				{
					transform.position = new Vector3 (transform.position.x - 0.25f, transform.position.y);
					transform.rotation = Quaternion.AngleAxis (-180f, Vector3.forward);
				}
				m_dir = SPRING_DIR.RIGHT;
			} else
				Debug.LogError ("Spring Dir Error");
		}

		if (SceneStatus.getInstance.m_bMemoryStage) {
			skelAnim.skeleton.SetSkin ("memory");
		}

//		transform.GetComponent<SpriteRenderer>().sprite = SceneObjectPool.getInstance.m_sprite_sally_Spring;
	}

	void OnTriggerStay2D (Collider2D coll)
	{
		if (coll.transform.name == "Runner(Clone)") {
			
			if (GameObject.Find ("MapToolMgr") == null || (GameObject.Find ("MapToolMgr") != null && GameObject.Find ("MapToolMgr").GetComponent<MapToolMgr> ().m_bNowPlaying)) {
				if (SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.RUNNER) {
					if (m_fCurCoolTime <= 0) {

						float fLeftColBoundary = 0.1f;
						float fRightColBoundary = 0.1f;

						if(coll.GetComponent<Runner>().m_bLeftMove)
							fLeftColBoundary = 0.5f;
						else
							fRightColBoundary = 0.5f;

						if ((m_dir.Equals(SPRING_DIR.UP) && transform.position.x - coll.transform.position.x < fLeftColBoundary && transform.position.x - coll.transform.position.x > -fRightColBoundary)
						    || ((m_dir.Equals(SPRING_DIR.LEFT) || m_dir.Equals(SPRING_DIR.RIGHT)) && transform.position.y - coll.transform.position.y < 0.1f && transform.position.y - coll.transform.position.y > -0.1f)) { //거의 겹쳐잇을때

							m_fCurCoolTime = m_fCoolTime;

							if (!m_bBroken) {

								if (coll.GetComponent<Runner> ().SpringForce != null)
									StopCoroutine (coll.GetComponent<Runner> ().SpringForce);

								coll.GetComponent<Runner> ().SpringForce = coll.GetComponent<Runner> ().Spring (fForce, m_dir);
								StartCoroutine (coll.GetComponent<Runner> ().SpringForce);

								if (m_dir.Equals (SPRING_DIR.UP)) {
									skelAnim.state.SetAnimation(0, "action", false);
									skelAnim.state.AddAnimation(0, "idle", true, 0);
								}

							}

							if (!m_bBroken && m_dir.Equals (SPRING_DIR.LEFT)) {
								coll.GetComponent<Runner> ().m_bLeftMove = true;
								Broken ();


							} else if (!m_bBroken && m_dir.Equals (SPRING_DIR.RIGHT)) {
								coll.GetComponent<Runner> ().m_bLeftMove = false;
								Broken ();

							} 
						}
					} else {
						m_fCurCoolTime -= Time.deltaTime;
					}

				} else if (SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.GUARDIAN) {
					float fSpringRange = 0.1f;

					if(TimeMgr.m_bFastForward)
						fSpringRange = 0.5f;

					if ((m_dir.Equals(SPRING_DIR.UP) && transform.position.x - coll.transform.position.x < fSpringRange && transform.position.x - coll.transform.position.x > -fSpringRange)
					    || ((m_dir.Equals(SPRING_DIR.LEFT) || m_dir.Equals(SPRING_DIR.RIGHT)) && transform.position.y - coll.transform.position.y < fSpringRange && transform.position.y - coll.transform.position.y > -fSpringRange)) { //거의 겹쳐잇을때

						skelAnim.state.TimeScale = 0.5f;
						if (m_dir.Equals (SPRING_DIR.UP)) {
							skelAnim.state.SetAnimation(0, "action", false);
							skelAnim.state.AddAnimation(0, "idle", true, 0);
						}
						
						if (!m_bBroken && m_dir.Equals (SPRING_DIR.LEFT)) {
							Broken ();
						} else if (!m_bBroken && m_dir.Equals (SPRING_DIR.RIGHT)) {
							Broken ();
						} 
					}
				} else if (SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.FLASHBACK) {
//					if (transform.rotation.eulerAngles.z > 89f && transform.rotation.eulerAngles.z < 91f) {
//						Fix();
//					} else if (transform.rotation.eulerAngles.z > 269f && transform.rotation.eulerAngles.z < 271f) {
//						Fix();
//					} 
				}
			}
		}
	}

	public void Broken ()
	{
		m_bBroken = true;

		skelAnim.state.SetAnimation(0, "action", false);
		skelAnim.state.AddAnimation(0, "broken", false, 0);

		//		GetComponent<SpriteRenderer> ().color = Color.black;
	}

	public void Fix ()
	{
		m_bBroken = false;
//		GetComponent<SpriteRenderer> ().color = Color.white;
		m_fCurCoolTime = m_fCoolTime;

		skelAnim.state.SetAnimation(0, "idle", true);
	}
	
}
