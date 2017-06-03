﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;


[System.Serializable]
public class OpenvrJoystickFrame
{
	public bool			TriggerIsDown;
	public bool			TriggerPressed;
	public bool			TriggerReleased;

	public bool			TouchpadIsDown;
	public bool			TouchpadPressed;
	public bool			TouchpadReleased;
	public Vector2		TouchpadAxis;

	public bool			TouchpadClickIsDown;
	public bool			TouchpadClickPressed;
	public bool			TouchpadClickReleased;

	public Vector3		Position;
	public Quaternion	Rotation;
}

[System.Serializable]
public class UnityEvent_PositionRotationJoystick : UnityEvent <OpenvrJoystickFrame> {}

[System.Serializable]
public class UnityEvent_PositionPosition : UnityEvent <Vector3,Vector3> {}


[ExecuteInEditMode]
public class OpenvrJoystickInterface : MonoBehaviour {

	public UnityEvent_PositionRotationJoystick	OnUpdateLeft;
	public UnityEvent_PositionRotationJoystick	OnUpdateRight;
	public UnityEvent_PositionPosition			OnUpdateCalibration;

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

	void UpdateNode(VRNode Node, string Name,UnityEvent_PositionRotationJoystick Event,KeyCode TriggerButton,KeyCode TouchpadButton,KeyCode TouchpadClickButton,KeyCode AppButton,string AxisX,string AxisY)
	{
		var JoyInput = new OpenvrJoystickFrame();
		JoyInput.Position = InputTracking.GetLocalPosition(Node);
		JoyInput.Rotation = InputTracking.GetLocalRotation(Node);

		JoyInput.TriggerIsDown = Input.GetKey( TriggerButton );
		JoyInput.TriggerPressed = Input.GetKeyDown( TriggerButton );
		JoyInput.TriggerReleased = Input.GetKeyUp( TriggerButton );

		JoyInput.TouchpadAxis = new Vector2( Input.GetAxis(AxisX), Input.GetAxis(AxisY) );

		JoyInput.TouchpadIsDown = Input.GetKey( TouchpadButton );
		JoyInput.TouchpadPressed = Input.GetKeyDown( TouchpadButton );
		JoyInput.TouchpadReleased = Input.GetKeyUp( TouchpadButton );

		JoyInput.TouchpadClickIsDown = Input.GetKey( TouchpadClickButton );
		JoyInput.TouchpadClickPressed = Input.GetKeyDown( TouchpadClickButton );
		JoyInput.TouchpadClickReleased = Input.GetKeyUp( TouchpadClickButton );

		Event.Invoke( JoyInput );
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
		UpdateNode( VRNode.LeftHand, JoystickNameLeft, OnUpdateLeft, LeftTrigger, LeftTouchpad, LeftTouchpadClick, LeftAppKeyCode, AxisLeftTouchPadX, AxisLeftTouchPadY );
		UpdateNode( VRNode.RightHand, JoystickNameRight, OnUpdateRight, RightTrigger, RightTouchpad, RightTouchpadClick, RightAppKeyCode, AxisRightTouchPadX, AxisRightTouchPadY );
		

		if ( Input.GetKey( LeftAppKeyCode ) && Input.GetKey( RightAppKeyCode ) )
			OnUpdateCalibration.Invoke( InputTracking.GetLocalPosition(VRNode.LeftHand), InputTracking.GetLocalPosition(VRNode.RightHand) );

		//	debug buttons
		for ( int btn=(int)KeyCode.JoystickButton0;	btn<(int)KeyCode.Joystick3Button0;	btn++ )
		{
			var Btn = (KeyCode)btn;
			if ( Input.GetKeyDown( Btn ) )
				Debug.Log( Btn );
		}

	}


}