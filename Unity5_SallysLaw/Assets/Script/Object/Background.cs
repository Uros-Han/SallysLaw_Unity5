using UnityEngine;
using System.Collections;

/// <summary>
/// 배경이 샐리 배경과 아빠배경을 저절로 전환하도록 도와주는 애
/// </summary>
public class Background : MonoBehaviour {

	public Sprite m_sallyBG;
	public Sprite m_fatherBG;

	bool m_bFatherScene;

	public bool m_bBox;
	public bool m_bDeco;
	public bool m_bPlatform;
	public bool m_bObject;
	public bool m_bSpine;
	public bool m_bParticle;


	public Color m_platform_SallyColor;
	public Color m_platform_FatherColor;

	SceneObjectPool sceneObjPool;


	// Use this for initialization


	void Start () {

		sceneObjPool = SceneObjectPool.getInstance;

		if (SceneStatus.getInstance.m_bMemoryStage) {
			return;
		}

		m_bFatherScene = false;

		if(!m_bSpine)
			m_sallyBG = GetComponent<SpriteRenderer>().sprite;

		if (!m_bBox && !m_bDeco && !m_bPlatform && !m_bObject) {
			int iLayerNum = 0;
			int iSpriteNum = 0;

			if (m_sallyBG.name.Contains ("layer")) {
				iLayerNum = System.Convert.ToInt32 (m_sallyBG.name.Substring (15, 2));

				switch (iLayerNum) {
				case 6:
					iSpriteNum = System.Convert.ToInt32 (m_sallyBG.name.Substring (m_sallyBG.name.IndexOf ("_", 15) + 1, 2));
					m_fatherBG = sceneObjPool.m_father_listLayer06 [iSpriteNum];
					break;
				
				case 7:
					iSpriteNum = System.Convert.ToInt32 (m_sallyBG.name.Substring (m_sallyBG.name.IndexOf ("_", 15) + 1, 2));
					m_fatherBG = sceneObjPool.m_father_listLayer07 [iSpriteNum];
					break;
				
				case 8:
					m_fatherBG = sceneObjPool.m_father_Layer08;
					break;
				
				case 9:
					m_fatherBG = sceneObjPool.m_father_Layer09;
					break;
				
				case 10:
					iSpriteNum = System.Convert.ToInt32 (m_sallyBG.name.Substring (m_sallyBG.name.IndexOf ("_", 15) + 1, 2));
					m_fatherBG = sceneObjPool.m_father_listLayer10 [iSpriteNum];
					break;
				
				case 11:
					m_fatherBG = sceneObjPool.m_father_Layer11;
					break;

				default:
					Debug.LogError ("SpriteLoadError");
					break;
				}
			}
			//m_fatherBG = Resources.Load<Sprite> ("Sprites/Chapter5/FatherBG/" + GetComponent<SpriteRenderer> ().sprite.name);
		} else if (m_bBox) {

			//여기있는건 floor땜시 구현
			//박스는 StageLoader  box생성할때 Sprite 넣어줌

			SetFloorSprite ();
		
		} else if (m_bDeco && !StageLoader.getInstance.m_bMaptool) {

			int m_iIdx = 0;

			if (gameObject.name.Equals("TopDeco(Clone)")) {
				m_iIdx = GetComponent<BoxDeco>().m_iTopDecoIdx;
				m_fatherBG = sceneObjPool.m_father_listTopDeco [m_iIdx];
				m_sallyBG = sceneObjPool.m_sally_listTopDeco [m_iIdx];
			} else if (gameObject.name.Equals("MidDeco")) {
				m_iIdx = GetComponent<BoxDeco>().m_iMidDecoIdx;
				m_fatherBG = sceneObjPool.m_father_listMidDeco [m_iIdx];
				m_sallyBG = sceneObjPool.m_sally_listMidDeco [m_iIdx];
			}
//			m_fatherBG = Resources.Load<Sprite> ("Sprites/Chapter5/FatherBG/" + gameObject.name + "/" + GetComponent<SpriteRenderer> ().sprite.name);
//			m_sallyBG = Resources.Load<Sprite> ("Sprites/Chapter5/SallyBG/" + gameObject.name + "/" + GetComponent<SpriteRenderer> ().sprite.name);
		} else if (m_bObject) {

			switch(gameObject.name)
			{
			case "DoorSprite(Clone)":
				m_fatherBG = sceneObjPool.m_father_listDoor[GetComponent<DoorSprite>().m_iSpriteIdx];

				break;

			case "spikeSprite":
				m_fatherBG = sceneObjPool.m_father_listSpike[transform.parent.GetComponent<Spike>().m_iSpriteIdx];
				break;

			}

		}

	}

	public void SetFloorSprite()
	{
//		string strBG = GetComponent<SpriteRenderer> ().sprite.name;
//		string strBox = strBG.Substring (0, strBG.IndexOf ("_", strBG.Length - 3));
		
//		int iIdx = System.Convert.ToInt32 (strBG.Substring (strBG.IndexOf ("_", strBG.Length - 3) + 1, strBG.Length - (strBG.IndexOf ("_", strBG.Length - 3) + 1)));
		
//		m_fatherBG = Resources.LoadAll<Sprite> ("Sprites/Chapter5/FatherBG/" + strBox) [iIdx];
//		m_sallyBG = Resources.LoadAll<Sprite> ("Sprites/Chapter5/SallyBG/" + strBox) [iIdx];

		m_fatherBG = sceneObjPool.m_sprite_father_Box [5];
		m_sallyBG = sceneObjPool.m_sprite_sally_Box [5];
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}	

	void OnEnable(){

		m_bFatherScene = false;
//		ChgBg ();
	}

	
	void ChgBg(bool bToFatherView)
	{




//		if((SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.FATHER_ENTER || SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.STAGE_CHG_FATHER) && !m_bFatherScene)
//			{
//				StartCoroutine(AudioMgr.getInstance.ChgPitch (GameObject.Find ("BGM").GetComponent<AudioSource> (), false));
//
//				if(m_bPlatform)
//					GetComponent<SpriteRenderer>().color = m_platform_FatherColor;
//				else
//					GetComponent<SpriteRenderer>().sprite = m_fatherBG;
//
//
//
//				m_bFatherScene = true;
//			}
//			else if( m_bFatherScene)
//			{
//				StartCoroutine(AudioMgr.getInstance.ChgPitch (GameObject.Find ("BGM").GetComponent<AudioSource> (), true));
//
//				if(m_bPlatform)
//					GetComponent<SpriteRenderer>().color = m_platform_SallyColor;
//				else
//					GetComponent<SpriteRenderer>().sprite = m_sallyBG;
//
//				m_bFatherScene = false;
//			}

		if(!SceneStatus.getInstance.m_bMemoryStage){
			if(bToFatherView)
			{
//				StartCoroutine(AudioMgr.getInstance.ChgPitch (GameObject.Find ("BGM").GetComponent<AudioSource> (), false));
				
				if(m_bPlatform)
					GetComponent<SpriteRenderer>().color = m_platform_FatherColor;
				else if(m_bSpine)
				{
					string strSkinName = GetComponent<SkeletonAnimation>().Skeleton.skin.name;

					if(strSkinName.IndexOf("_") != -1)
						strSkinName = "fafa" + strSkinName.Substring(strSkinName.IndexOf("_"), strSkinName.Length - strSkinName.IndexOf("_"));
					else
						strSkinName = "fafa";

//					GetComponent<SkeletonAnimation>().skeleton.SetSkin(strSkinName);
					GetComponent<SkeletonAnimation>().initialSkinName = strSkinName;
					GetComponent<SkeletonAnimation>().Reset();
				}else if(m_bParticle)
				{
					transform.Find("sally").gameObject.SetActive(false);
					transform.Find("fafa").gameObject.SetActive(true);
				}else
					GetComponent<SpriteRenderer>().sprite = m_fatherBG;				
				
				m_bFatherScene = true;
			}
			else
			{
//				StartCoroutine(AudioMgr.getInstance.ChgPitch (GameObject.Find ("BGM").GetComponent<AudioSource> (), true));
				
				if(m_bPlatform)
					GetComponent<SpriteRenderer>().color = m_platform_SallyColor;
				else if(m_bSpine)
				{
				}else if(m_bParticle)
				{
				}else
					GetComponent<SpriteRenderer>().sprite = m_sallyBG;
				
				m_bFatherScene = false;
			}
		}


	}
}
