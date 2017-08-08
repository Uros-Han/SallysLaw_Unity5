using UnityEngine;
using System.Collections;

public class StartVehicle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SkeletonAnimation skelAnim_in = GetComponent<SkeletonAnimation> ();
		SkeletonAnimation skelAnim_out = transform.GetChild(0).GetComponent<SkeletonAnimation>();
		
		
		skelAnim_in.enabled = true;

		transform.localScale = new Vector3 (-1f, 1f);


		
		transform.GetChild(0).gameObject.SetActive(true);

		switch(GameMgr.getInstance.m_iCurChpt){

		case 1:
			transform.localScale = new Vector3 (1f, 1f);
			transform.GetChild(1).gameObject.SetActive(true);
			break;


		case 2:
			skelAnim_in.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/tram_atlas/Tram_in_SkeletonData");
			skelAnim_out.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/tram_atlas/Tram_out_SkeletonData");
			transform.position = new Vector3 (transform.position.x - 3f, transform.position.y);
			AudioSetting(7);
			break;
			
		case 3:
			skelAnim_in.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/train_atlas/Train_in_SkeletonData");
			skelAnim_out.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/train_atlas/Train_out_SkeletonData");
			transform.position = new Vector3 (transform.position.x - 2.75f, transform.position.y + 0.25f);
			AudioSetting(5);
			break;
			
		case 4:
			skelAnim_in.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/bus_atlas/Bus_in_SkeletonData");
			skelAnim_out.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/bus_atlas/Bus_out_SkeletonData");
			transform.position = new Vector3 (transform.position.x - 3f, transform.position.y);
			AudioSetting(1);
			break;
			
		case 5:
			skelAnim_in.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Spine/transport/carriage_atlas/Carriage_SkeletonData");
			skelAnim_out.skeletonDataAsset = null;
			skelAnim_out.gameObject.SetActive(false);
			transform.position = new Vector3 (transform.position.x - 3f, transform.position.y);
			AudioSetting(3);
			break;

		}
		
		skelAnim_in.Reset();
		skelAnim_out.Reset();

	}

	void AudioSetting(int iAudioIdx)
	{
		if (GameMgr.getInstance.m_bSoundMute)
			return;

		AudioSource audio = GetComponent<AudioSource> ();
		
		audio.clip = AudioMgr.getInstance.m_sound_Transport[iAudioIdx];
		audio.Play();

		#if UNITY_STANDALONE
		audio.volume = PlayerPrefs.GetFloat("SoundVolume");
		#endif
	}
}
