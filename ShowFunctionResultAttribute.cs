﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif


[System.AttributeUsage(System.AttributeTargets.Field)]
public class ShowFunctionResultAttribute : PropertyAttribute
{
	public readonly string MethodName;

	public ShowFunctionResultAttribute(string MethodName)
	{
		this.MethodName = MethodName;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowFunctionResultAttribute))]
public class ShowFunctionResultAttributeDrawer : PropertyDrawer
{
	
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		var Attrib = (ShowFunctionResultAttribute)attribute;

		try
		{
			System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
			var FunctionName = Attrib.MethodName;
			var _eventMethodInfo = eventOwnerType.GetMethod( FunctionName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			var Result = _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);

			var Text = "" + Result;
			EditorGUI.HelpBox (position, Text, MessageType.Info);
		}
		catch(System.Exception e) 
		{
			EditorGUI.HelpBox (position, e.Message, MessageType.Error);
		}
	}
}
#endif
