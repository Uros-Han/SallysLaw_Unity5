using UnityEngine;
using System.Collections;

public class AudioMgr : MonoBehaviour {

	private static AudioMgr instance;
	
	public static AudioMgr getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(AudioMgr)) as AudioMgr;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("AudioMgr");
				instance = obj.AddComponent (typeof(AudioMgr)) as AudioMgr;
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

		StopAllCoroutines ();
	}

	public AudioClip[] m_sound_jump_sally;
	public AudioClip[] m_sound_jump_father;
	public AudioClip[] m_sound_land_Rock;
	public AudioClip[] m_sound_land_Grass;
	public AudioClip[] m_sound_land_Mud;
	public AudioClip[] m_sound_bundle;

	public AudioClip[] m_sound_spike_metal;
	public AudioClip[] m_sound_spike_wood;

	public AudioClip[] m_sound_door_wood;
	public AudioClip[] m_sound_door_grass;
	public AudioClip[] m_sound_door_stone;

	public AudioClip[] m_sound_portal;

	public AudioClip[] m_sound_fastforward;

	public AudioClip[] m_sound_UIbundle;

	public AudioClip[] m_sound_Interactions;
	public AudioClip[] m_sound_Wingfraps;
	public AudioClip[] m_sound_Transport;

	enum DoorIdx { GRASS, STONE, WOOD, END };

	public void AudioPoolSetting()
	{
		m_sound_jump_sally = Resources.LoadAll<AudioClip> ("Sounds/SallyJump");
		m_sound_jump_father = Resources.LoadAll<AudioClip> ("Sounds/FatherJump");
		m_sound_land_Rock = Resources.LoadAll<AudioClip> ("Sounds/Land/Rock");
		m_sound_land_Grass = Resources.LoadAll<AudioClip> ("Sounds/Land/Grass");
		m_sound_land_Mud = Resources.LoadAll<AudioClip> ("Sounds/Land/Mud");

		m_sound_spike_metal = Resources.LoadAll<AudioClip> ("Sounds/Spike/Metal");
		m_sound_spike_wood = Resources.LoadAll<AudioClip> ("Sounds/Spike/Wood");

		m_sound_door_wood = Resources.LoadAll<AudioClip> ("Sounds/Door/Wood");
		m_sound_door_grass = Resources.LoadAll<AudioClip> ("Sounds/Door/Grass");
		m_sound_door_stone = Resources.LoadAll<AudioClip> ("Sounds/Door/Stone");

		m_sound_portal = Resources.LoadAll<AudioClip> ("Sounds/Portal");

		m_sound_fastforward = Resources.LoadAll<AudioClip> ("Sounds/FastForward");


		m_sound_Interactions = Resources.LoadAll<AudioClip> ("Sounds/Interactions");
		m_sound_Wingfraps = Resources.LoadAll<AudioClip> ("Sounds/Wingfrap");
		m_sound_Transport = Resources.LoadAll<AudioClip> ("Sounds/Trans");

		m_sound_bundle = Resources.LoadAll<AudioClip> ("Sounds/SoundBundle");
		m_sound_UIbundle = Resources.LoadAll<AudioClip> ("Sounds/UISoundBundle");


	}

	/// <summary>
	/// Change the pitch.
	/// </summary>
	/// <returns>The pitch.</returns>
	/// <param name="source">Source.</param>
	/// <param name="bUp">If set to <c>true</c> b up.</param>
	public IEnumerator ChgPitch(AudioSource source, bool bUp)
	{
		float fChgSpeed = 2f;
		bool bExit = false;

		do{
			if (bUp) {

				if(source.pitch < 1f)
					source.pitch += fChgSpeed * Time.deltaTime;
				else
				{
					source.pitch = 1f;
					bExit = true;
				}

			} else {

				if(source.pitch > 0.5f)
					source.pitch -= fChgSpeed * Time.deltaTime;
				else
				{
					source.pitch = 0.5f;
					bExit = true;
				}

			}

			yield return null;

		}while(!bExit);
	}

	public IEnumerator VolumeChg(bool bToUp, float fTimePara = 0)
	{
		if (GameObject.Find ("BGM") == null)
			yield break;



		float fFrom, fTo;
		float fValue = 0f;
		float fTime;

		if (bToUp) {

			GameObject.Find ("BGM").GetComponent<AudioSource> ().Play ();
			
			if (GameObject.Find ("AMB") != null)
				GameObject.Find ("AMB").GetComponent<AudioSource> ().Play ();

			AudioListener.volume = 0f;
			fFrom = 0f;
			fTo = 1f;
			fTime = 3f;
		} else {
			fFrom = 1f;
			fTo = 0f;
			fTime = 1f;
		}

		if (fTimePara != 0)
			fTime = fTimePara;

		if(Camera.main != null)
			Camera.main.GetComponent<AudioListener> ().enabled = true;
		else
			UICamera.mainCamera.GetComponent<AudioListener> ().enabled = true;

		do{
			yield return new WaitForEndOfFrame();

			fValue += (Time.unscaledDeltaTime / fTime);

			if(fValue > 1f)
				fValue = 1f;


			AudioListener.volume = Mathf.Lerp (fFrom, fTo, Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, fValue)));


		}while(fValue != 1);

	}

	public IEnumerator RewindEffect()
	{
		AudioSource source = GameObject.Find ("BGM").GetComponent<AudioSource> ();
		float fChgSpeed = 0.75f;
		bool bUp = true;

		do {

			if (bUp) {
				
				if(source.pitch < 2f)
					source.pitch += fChgSpeed * Time.deltaTime;
				else
				{
					source.pitch = 2f;
					bUp = false;
				}
				
			}

			yield return null;

		} while( SceneStatus.getInstance.m_enPlayerStatus == PLAYER_STATUS.FLASHBACK);
	}

	public void PlaySfx(AudioSource audioSource, string strSfx, int iBundleIdx = 0)
	{
		float fVolume = 1f;
#if UNITY_STANDALONE || UNITY_WEBGL
		fVolume = PlayerPrefs.GetFloat("SoundVolume");
#else
		if (GameMgr.getInstance.m_bSoundMute)
			return;

#endif
		switch (strSfx) {
		case "sallyJump":
			audioSource.PlayOneShot(m_sound_jump_sally[Random.Range(0,3)], fVolume);
			break;

		case "fatherJump":
			audioSource.PlayOneShot (m_sound_jump_father [Random.Range (0, 1)], fVolume);
			break;

		case "land_rock":
			audioSource.PlayOneShot (m_sound_land_Rock [Random.Range (0, 3)], fVolume);
			break;
		case "land_grass":
			audioSource.PlayOneShot (m_sound_land_Grass [Random.Range (0, 3)], fVolume);
			break;
		case "land_mud":
			audioSource.PlayOneShot (m_sound_land_Mud [Random.Range (0, 3)], fVolume);
			break;

		case "spike_metal":
			audioSource.PlayOneShot (m_sound_spike_metal [Random.Range (0, 3)], fVolume);
			break;
		case "spike_wood":
			audioSource.PlayOneShot (m_sound_spike_wood [Random.Range (0, 3)], fVolume);
			break;

		case "bundle":
			audioSource.PlayOneShot (m_sound_bundle [iBundleIdx], fVolume);
			break;

		case "ui_bundle":
			audioSource.PlayOneShot (m_sound_UIbundle [iBundleIdx], fVolume);
			break;

		case "portal":
			audioSource.PlayOneShot(m_sound_portal[iBundleIdx], fVolume);
			break;

		case "fastforward":
			audioSource.PlayOneShot(m_sound_fastforward[iBundleIdx], fVolume);
			break;

		case "door_wood":
			audioSource.PlayOneShot (m_sound_door_wood [iBundleIdx], fVolume);
			break;
		case "door_stone":
			audioSource.PlayOneShot (m_sound_door_stone [iBundleIdx], fVolume);
			break;
		case "door_grass":
			audioSource.PlayOneShot (m_sound_door_grass [iBundleIdx], fVolume);
			break;

		case "interaction":
			audioSource.PlayOneShot (m_sound_Interactions [iBundleIdx], fVolume);
			break;

		case "wingfrap":
			audioSource.PlayOneShot (m_sound_Wingfraps [iBundleIdx], fVolume);
			break;

		case "trans":
			audioSource.PlayOneShot (m_sound_Transport [iBundleIdx], fVolume);
			break;

		default:
			Debug.LogError("SFX_AudioName Error : "+ strSfx);
			break;
		}
	}

}










