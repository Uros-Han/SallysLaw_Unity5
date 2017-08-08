using UnityEngine;
using System.Collections;

public class TapTapSDKManager : MonoBehaviour {

	private static AndroidJavaClass cls = null;
	private static AndroidJavaObject _plugins; 

	// Use this for initialization
	void Start () {
		AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		
		cls = new AndroidJavaClass("com.uros.taptpasdk.TapTapSDK");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		                                                       {
			using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
			{
				obj.Call("OnCreateThis", activity);
			}
		}));
	}

	void Destroy()
	{
		AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		
		cls = new AndroidJavaClass("com.uros.taptpasdk.TapTapSDK");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		                                                       {
			using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
			{
				obj.Call("DestroyThis", activity);
			}
		}));
	}

	public void LicenseCheck(string msg)
	{
		switch (msg) {
		case "OK":
			Debug.Log("TAPTAP LICENSE_OK");
			break;
		case "NO":
			Debug.Log("TAPTAP LICENSE_NO");
			break;
		case "NOT_INSTALL_TAPTAP":
			Debug.Log("TAPTAP LICENSE_NOT_INSTALL_TAPTAP");
			break;
		default:
			Debug.Log("License Error");
			break;
		}
	}
}
