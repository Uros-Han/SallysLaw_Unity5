using UnityEngine;
using System.Collections;

public class PC_InputControl : MonoBehaviour 
{

	private static PC_InputControl instance;
	
	public static PC_InputControl getInstance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(PC_InputControl)) as PC_InputControl;
			}
			
			if (instance == null) {
				GameObject obj = new GameObject ("GameMgr");
				instance = obj.AddComponent (typeof(PC_InputControl)) as PC_InputControl;
			}
			
			return instance;
		}
	}
	
	void OnApplicationQuit()
	{
		
		instance = null;
	}
	//*********************//
	// Public member data  //
	//*********************//
	
	
	//*********************//
	// Private member data //
	//*********************//
	
	public enum eInputState
	{
		MouseKeyboard,
		Controler
	};
	private eInputState m_State = eInputState.MouseKeyboard;
	
	//*************************//
	// Unity member methods    //
	//*************************//

	void Start()
	{
#if !UNITY_STANDALONE
		Destroy(this);
#endif
	}

	void OnGUI()
	{
		switch( m_State )
		{
		case eInputState.MouseKeyboard:
			if(isControlerInput())
			{
				m_State = eInputState.Controler;
				Debug.Log("DREAM - JoyStick being used");
				GameObject.Find("UI Root").BroadcastMessage("SwapController", false ,SendMessageOptions.DontRequireReceiver);
			}
			break;
		case eInputState.Controler:
			if (isMouseKeyboard())
			{
				m_State = eInputState.MouseKeyboard;
				Debug.Log("DREAM - Mouse & Keyboard being used");
				GameObject.Find("UI Root").BroadcastMessage("SwapController", true ,SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
	}
	
	//***************************//
	// Public member methods     //
	//***************************//
	
	public eInputState GetInputState()
	{
		return m_State;
	}
	
	//****************************//
	// Private member methods     //
	//****************************//
	
	private bool isMouseKeyboard()
	{
		// mouse & keyboard buttons
		if (Event.current.isKey ||
		    Event.current.isMouse)
		{
			return true;
		}
		// mouse movement
		if( Input.GetAxis("Mouse X") != 0.0f ||
		   Input.GetAxis("Mouse Y") != 0.0f )
		{
			return true;
		}
		return false;
	}
	
	private bool isControlerInput()
	{
		// joystick buttons
		if(Input.GetKey(KeyCode.JoystickButton0)  ||
		   Input.GetKey(KeyCode.JoystickButton1)  ||
		   Input.GetKey(KeyCode.JoystickButton2)  ||
		   Input.GetKey(KeyCode.JoystickButton3)  ||
		   Input.GetKey(KeyCode.JoystickButton4)  ||
		   Input.GetKey(KeyCode.JoystickButton5)  ||
		   Input.GetKey(KeyCode.JoystickButton6)  ||
		   Input.GetKey(KeyCode.JoystickButton7)  ||
		   Input.GetKey(KeyCode.JoystickButton8)  ||
		   Input.GetKey(KeyCode.JoystickButton9)  ||
		   Input.GetKey(KeyCode.JoystickButton10) ||
		   Input.GetKey(KeyCode.JoystickButton11) ||
		   Input.GetKey(KeyCode.JoystickButton12) ||
		   Input.GetKey(KeyCode.JoystickButton13) ||
		   Input.GetKey(KeyCode.JoystickButton14) ||
		   Input.GetKey(KeyCode.JoystickButton15) ||
		   Input.GetKey(KeyCode.JoystickButton16) ||
		   Input.GetKey(KeyCode.JoystickButton17) ||
		   Input.GetKey(KeyCode.JoystickButton18) ||
		   Input.GetKey(KeyCode.JoystickButton19) )
		{
			return true;
		}
		
		// joystick axis
		if(Input.GetAxis("Horizontal") != 0.0f ||
		   Input.GetAxis("Vertical") != 0.0f)
		{
			return true;
		}
		
		return false;
	}
}