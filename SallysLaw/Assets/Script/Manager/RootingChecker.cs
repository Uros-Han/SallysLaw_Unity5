using UnityEngine;
using System.Collections;

public class RootingChecker : MonoBehaviour
{
#if UNITY_ANDROID
	private void Awake()
	{
		using (AndroidJavaClass cls = new AndroidJavaClass("com.nanali.rootingcheck.Plugin"))
		{
			using (AndroidJavaObject stars = cls.CallStatic<AndroidJavaObject>("getInstance"))
			{

				stars.Call("RootingCheck", gameObject.name,"Callback_RootingCheck");
			}
		}
	}
#endif

	public void Callback_RootingCheck(string value)
	{
		//value 1 = is rooting phone, value 0 = not rooting phone.
		Debug.Log ("Phone Rooting state : "+value);

		if (value.Equals ("1")) {
//			MobileNativeMessage msg = new MobileNativeMessage("Root Detected", "Root has been detected. Exit the app"); 
//			Application.Quit ();
		}else
			Destroy (gameObject);
	}
}
