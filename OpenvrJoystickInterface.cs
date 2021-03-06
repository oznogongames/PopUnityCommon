﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;



[System.Serializable]
public class JoysticksFrame
{
	public List<OpenvrJoystickFrame>	Joysticks;
}


[System.Serializable]
public class OpenvrJoystickFrame
{
	public Vector3		Position;
	public Quaternion	Rotation;

	public bool			TriggerIsDown;
	public bool			TriggerPressed;
	public bool			TriggerReleased;

	public Vector2		TouchpadAxis;

	public bool			TouchpadIsDown;
	public bool			TouchpadPressed;
	public bool			TouchpadReleased;

	public bool			TouchpadClickIsDown;
	public bool			TouchpadClickPressed;
	public bool			TouchpadClickReleased;

	public bool			AppButtonIsDown;
	public bool			AppButtonPressed;
	public bool			AppButtonReleased;

	public bool IsKeyFrame()
	{
		return TriggerPressed || TriggerReleased ||
			TouchpadPressed || TouchpadReleased ||
			TouchpadClickPressed || TouchpadClickReleased ||
			AppButtonPressed || AppButtonReleased;
	}

}

[System.Serializable]
public class UnityEvent_PositionRotationJoystick : UnityEvent <OpenvrJoystickFrame> {}

[System.Serializable]
public class UnityEvent_JoystickFrames : UnityEvent <List<OpenvrJoystickFrame>> {}

[System.Serializable]
public class UnityEvent_PositionPosition : UnityEvent <Vector3,Vector3> {}


[ExecuteInEditMode]
public class OpenvrJoystickInterface : MonoBehaviour {

	public UnityEvent_PositionRotationJoystick	OnUpdateLeft;
	public UnityEvent_PositionRotationJoystick	OnUpdateRight;
	public UnityEvent_JoystickFrames			OnUpdateAll;

	const string								JoystickNameLeft = "OpenVR Controller - Left";
	const string								JoystickNameRight = "OpenVR Controller - Right";
	const KeyCode								LeftTrigger = KeyCode.Joystick1Button14;
	const KeyCode								RightTrigger = KeyCode.Joystick2Button15;
	const KeyCode								LeftTouchpad = KeyCode.Joystick1Button16;
	const KeyCode								RightTouchpad = KeyCode.Joystick2Button17;
	const KeyCode								LeftTouchpadClick = KeyCode.Joystick1Button8;
	const KeyCode								RightTouchpadClick = KeyCode.Joystick2Button9;

	const KeyCode								LeftAppKeyCode = KeyCode.Joystick1Button2;
	const KeyCode								RightAppKeyCode = KeyCode.Joystick2Button0;


	[Header("These need to be setup in input, cant hardcode them :/")]
	public string								AxisLeftTouchPadX = "LeftController Touchpad X";
	public string								AxisLeftTouchPadY = "LeftController Touchpad Y";
	public string								AxisRightTouchPadX = "RightController Touchpad X";
	public string								AxisRightTouchPadY = "RightController Touchpad Y";

	#if UNITY_2017_2_OR_NEWER
	OpenvrJoystickFrame UpdateNode(UnityEngine.XR.XRNode Node, string Name,KeyCode TriggerButton,KeyCode TouchpadButton,KeyCode TouchpadClickButton,KeyCode AppButton,string AxisX,string AxisY)
	#else
	OpenvrJoystickFrame UpdateNode(UnityEngine.VR.VRNode Node, string Name,KeyCode TriggerButton,KeyCode TouchpadButton,KeyCode TouchpadClickButton,KeyCode AppButton,string AxisX,string AxisY)
	#endif
	{
		var JoyInput = new OpenvrJoystickFrame();

		#if UNITY_2017_2_OR_NEWER
		JoyInput.Position = UnityEngine.XR.InputTracking.GetLocalPosition(Node);
		JoyInput.Rotation = UnityEngine.XR.InputTracking.GetLocalRotation(Node);
		#else
		JoyInput.Position = UnityEngine.VR.InputTracking.GetLocalPosition(Node);
		JoyInput.Rotation = UnityEngine.VR.InputTracking.GetLocalRotation(Node);
		#endif

		JoyInput.TriggerIsDown = Input.GetKey( TriggerButton );
		JoyInput.TriggerPressed = Input.GetKeyDown( TriggerButton );
		JoyInput.TriggerReleased = Input.GetKeyUp( TriggerButton );

		//	throws if not setup
		try
		{
			JoyInput.TouchpadAxis = new Vector2(Input.GetAxis(AxisX), Input.GetAxis(AxisY));
		}
		catch
		{
			JoyInput.TouchpadAxis = Vector2.zero;
		}

		JoyInput.TouchpadIsDown = Input.GetKey( TouchpadButton );
		JoyInput.TouchpadPressed = Input.GetKeyDown( TouchpadButton );
		JoyInput.TouchpadReleased = Input.GetKeyUp( TouchpadButton );

		JoyInput.TouchpadClickIsDown = Input.GetKey( TouchpadClickButton );
		JoyInput.TouchpadClickPressed = Input.GetKeyDown( TouchpadClickButton );
		JoyInput.TouchpadClickReleased = Input.GetKeyUp( TouchpadClickButton );

		JoyInput.AppButtonIsDown = Input.GetKey( AppButton );
		JoyInput.AppButtonPressed = Input.GetKeyDown( AppButton );
		JoyInput.AppButtonReleased = Input.GetKeyUp( AppButton );

		return JoyInput;
	}

	void Start()
	{
		var Joysticks = Input.GetJoystickNames();
		var DebugText = "" + Joysticks.Length + " joysticks; ";
		foreach ( var StickName in Joysticks )
			DebugText += StickName + ", ";

		Debug.Log(DebugText);
	}

	void Update()
	{
		#if UNITY_2017_2_OR_NEWER
		var Left = UpdateNode( UnityEngine.XR.XRNode.LeftHand, JoystickNameLeft, LeftTrigger, LeftTouchpad, LeftTouchpadClick, LeftAppKeyCode, AxisLeftTouchPadX, AxisLeftTouchPadY );
		var Right = UpdateNode( UnityEngine.XR.XRNode.RightHand, JoystickNameRight, RightTrigger, RightTouchpad, RightTouchpadClick, RightAppKeyCode, AxisRightTouchPadX, AxisRightTouchPadY );
		#else
		var Left = UpdateNode( UnityEngine.VR.VRNode.LeftHand, JoystickNameLeft, LeftTrigger, LeftTouchpad, LeftTouchpadClick, LeftAppKeyCode, AxisLeftTouchPadX, AxisLeftTouchPadY );
		var Right = UpdateNode( UnityEngine.VR.VRNode.RightHand, JoystickNameRight, RightTrigger, RightTouchpad, RightTouchpadClick, RightAppKeyCode, AxisRightTouchPadX, AxisRightTouchPadY );
		#endif

		OnUpdateLeft.Invoke( Left );
		OnUpdateRight.Invoke( Right );
		OnUpdateAll.Invoke( new List<OpenvrJoystickFrame>(){Left,Right} );
	
		//	debug buttons
		for ( int btn=(int)KeyCode.JoystickButton0;	btn<(int)KeyCode.Joystick3Button0;	btn++ )
		{
			var Btn = (KeyCode)btn;
			if ( Input.GetKeyDown( Btn ) )
				Debug.Log( Btn );
		}

	}


}
