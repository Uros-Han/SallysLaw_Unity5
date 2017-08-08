using UnityEngine;
using System.Collections;

public class InteractionProp : MonoBehaviour {

	SkeletonAnimation m_skelAnim;
	AudioSource m_audio;

	public int m_iPropIdx;

	int m_iBirdIdx = -1;
	float m_fBirdPos;

	void Start()
	{
		m_skelAnim = GetComponent<SkeletonAnimation> ();
		m_audio = GetComponent<AudioSource> ();

		#if UNITY_STANDALONE
		m_audio.volume = PlayerPrefs.GetFloat("SoundVolume");
		#endif

		if (StageLoader.getInstance.m_bMaptool) {
			GameObject decoNum = Instantiate (Resources.Load ("Prefabs/Objects/Boxes/DecoNum") as GameObject) as GameObject;
			decoNum.transform.parent = transform;
			decoNum.transform.localPosition = Vector2.zero;
			decoNum.GetComponent<TextMesh> ().text = m_iPropIdx.ToString ();
			decoNum.GetComponent<TextMesh> ().color = Color.red;
			
			return;
		} else {
			m_skelAnim.enabled = true;
			 
			if(m_iPropIdx.Equals(0))
			{
				m_iBirdIdx = Random.Range(1,4);
				m_fBirdPos = Random.Range(-0.2f, 0.2f);
			}

			ResetProp(true);
		}
	}

	public void ResetProp(bool Init, bool bGuardianView = false)
	{
		string strStatus;

		if(bGuardianView)
			strStatus = "fafa";
		else
			strStatus = "sally";

		switch(m_iPropIdx)
		{
		case 0://Bird
			m_skelAnim.skeletonDataAsset = SceneObjectPool.getInstance.m_Interaction_Bird;

			m_skelAnim.Reset();

			GameMgr gMgr = GameMgr.getInstance;

			if(gMgr.m_iCurChpt.Equals(1) || gMgr.m_iCurChpt.Equals(2)) //챕터 1,2만 새 3종류
				m_skelAnim.skeleton.SetSkin(strStatus + "_" + m_iBirdIdx.ToString("00"));
			else
				m_skelAnim.skeleton.SetSkin(strStatus + "_01");


			if(Init)
			{
				transform.position -= new Vector3(m_fBirdPos, 0.25f);
				GetComponent<BoxCollider2D>().offset += new Vector2(0, 0.75f);


			}

			switch(gMgr.m_iCurChpt)
			{
			case 1:
				if(!gMgr.m_bSoundMute)
					m_audio.clip = AudioMgr.getInstance.m_sound_Interactions[(int)SOUND_INTERACTION.DOVE];
				m_audio.loop = true;
				m_skelAnim.state.Start += delegate {
					m_audio.Stop();

					if(m_skelAnim.AnimationName.Equals("00"))
						m_audio.Play();
					if(m_skelAnim.AnimationName.Equals("01"))
						AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "wingfrap", Random.Range(1,3));
				};
				break;
			case 2:
				if(!gMgr.m_bSoundMute)
					m_audio.clip = AudioMgr.getInstance.m_sound_Interactions[Random.Range(0,3)]; // Crow sound
				m_audio.loop = false;
				m_skelAnim.state.Event += Event;
				m_skelAnim.state.Start += delegate {
					m_audio.Stop();

					if(m_skelAnim.AnimationName.Equals("00"))
						m_audio.Play();
					if(m_skelAnim.AnimationName.Equals("01"))
						AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "wingfrap", Random.Range(1,3));
				};
				break;
			case 3:
				if(!gMgr.m_bSoundMute)
					m_audio.clip = AudioMgr.getInstance.m_sound_Interactions[(int)SOUND_INTERACTION.SPARROW];
				m_audio.loop = true;
				
				m_skelAnim.state.Start += delegate {
					m_audio.Stop();

					if(m_skelAnim.AnimationName.Equals("00"))
						m_audio.Play();
					if(m_skelAnim.AnimationName.Equals("01"))
						AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "wingfrap", Random.Range(3,5));
				};
				m_audio.Play();
				break;
			case 4:
				m_skelAnim.state.Start += delegate {
					if(m_skelAnim.AnimationName.Equals("01"))
						AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "wingfrap", 0);
				};
				break;
			case 5:
//				if(!gMgr.m_bSoundMute)
//					m_audio.clip = AudioMgr.getInstance.m_sound_Interactions[Random.Range(0,3)]; //Crow sound
//				m_audio.loop = false;
//				m_skelAnim.state.Event += Event;
				m_skelAnim.state.Start += delegate {
					m_audio.Stop();
					if(m_skelAnim.AnimationName.Equals("01"))
						AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "wingfrap", Random.Range(1,3));
				};
				break;
			}


			
			GetComponent<MeshRenderer>().enabled = true;
			
			break;
			
		case 1://Street Light sally_01
			m_skelAnim.skeletonDataAsset = SceneObjectPool.getInstance.m_Interaction_Light;
			m_skelAnim.Reset();
			
			m_skelAnim.skeleton.SetSkin(strStatus + "_01");

			if(Init)
			{
				transform.position -= new Vector3(0, 0.25f);
				GetComponent<BoxCollider2D>().offset += new Vector2(0, 0.75f);
			}
			break;
			
		case 2://Street Light sally_02
			m_skelAnim.skeletonDataAsset = SceneObjectPool.getInstance.m_Interaction_Light;
			m_skelAnim.Reset();
			
			m_skelAnim.skeleton.SetSkin(strStatus + "_02");

			if(Init)
			{


				if(!GameMgr.getInstance.m_iCurChpt.Equals(5))
				{
					transform.position += new Vector3(0, 0.25f);
					GetComponent<BoxCollider2D>().offset -= new Vector2(0, 0.75f);
				}
				else
				{
					transform.position -= new Vector3(0, 0.25f);
					GetComponent<BoxCollider2D>().offset += new Vector2(0, 0.75f);
				}
			}
			break;
			
		case 3://Traffic sally_01
			m_skelAnim.skeletonDataAsset = SceneObjectPool.getInstance.m_Interaction_Traffic;
			m_skelAnim.Reset();
			
			m_skelAnim.skeleton.SetSkin(strStatus + "_01");

			if(Init)
			{
				transform.position -= new Vector3(0, 0.25f);
				GetComponent<BoxCollider2D>().offset += new Vector2(0, 0.75f);
			}
			break;
			
		case 4://Traffic sally_02
			m_skelAnim.skeletonDataAsset = SceneObjectPool.getInstance.m_Interaction_Traffic;
			m_skelAnim.Reset();
			
			m_skelAnim.skeleton.SetSkin(strStatus + "_02");

			if(Init)
			{
				transform.position += new Vector3(0, 0.25f);
				GetComponent<BoxCollider2D>().offset -= new Vector2(0, 0.75f);
			}
			break;
		}
		

		m_skelAnim.state.SetAnimation (0, "00", true);

	}

	public void Event (Spine.AnimationState state, int trackIndex, Spine.Event e)
	{
		m_audio.Stop();
		m_audio.Play();
	}


	void OnTriggerEnter2D(Collider2D coll)
	{
		if (StageLoader.getInstance.m_bMaptool)
			return;


		if(coll.gameObject.name.Equals("Runner(Clone)"))
		{
			if (m_skelAnim.AnimationName != "00")
				return;

			m_skelAnim.state.SetAnimation(0,"01", false);



			if(m_skelAnim.skeletonDataAsset.name.Equals("Traffic_light_SkeletonData"))
			{
				if(GameMgr.getInstance.m_iCurChpt.Equals(4))
				{
					AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "interaction", (int)SOUND_INTERACTION.WOOD_SIGN);
				}else if(!GameMgr.getInstance.m_iCurChpt.Equals(5))
				{
					AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "interaction", (int)SOUND_INTERACTION.SIGNAL_LIGHT);
				}
				m_skelAnim.state.AddAnimation(0,"02", true, 0);

			}else if(m_skelAnim.skeletonDataAsset.name.Equals("Street_light_SkeletonData"))
			{
				if(!GameMgr.getInstance.m_iCurChpt.Equals(5))
					AudioMgr.getInstance.PlaySfx(GetComponent<AudioSource>(), "interaction", (int)SOUND_INTERACTION.LIGHT);
				m_skelAnim.state.AddAnimation(0,"02", true, 0);

			}else if(m_skelAnim.skeletonDataAsset.name.Equals("Bird_SkeletonData"))
			{
				m_skelAnim.state.End += delegate {
					GetComponent<MeshRenderer>().enabled = false;
				};
			}
		}
	}

}
