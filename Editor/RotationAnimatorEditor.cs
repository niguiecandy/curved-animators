using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NGC6543
{
	[CustomEditor(typeof(RotationAnimator)), CanEditMultipleObjects]
	public class RotationAnimatorEditor : CurvedProgressAnimatorEditor
	{
		const string k_mode = "_mode";
		const string k_space = "_space";
		const string k_customAxis = "_customAxis";
		const string k_targets = "_targets";
		const string k_includeSelf = "_includeSelf";
		
		RotationAnimator _component;
		
		SerializedProperty _mode, _space;
		SerializedProperty _customAxis;
		SerializedProperty _targets, _includeSelf;
		
		
		string[] _excludedProperties =
		{
			k_mode, k_space,
			k_customAxis,
			k_targets, k_includeSelf
		};
		
		protected override void OnEnable()
		{
			base.OnEnable();
			_component = target as RotationAnimator;
			
			_mode = serializedObject.FindProperty(k_mode);
			_space = serializedObject.FindProperty(k_space);
			_customAxis = serializedObject.FindProperty(k_customAxis);
			_targets = serializedObject.FindProperty(k_targets);
			_includeSelf = serializedObject.FindProperty(k_includeSelf);
		}
		
		public override void OnInspectorGUI()
		{
			DrawHeaderHelpBox();
			
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			
			// Draw CurvedProgressAnimator inspector.
			DrawPropertiesExcluding(serializedObject, _excludedProperties);
			
			// Draw RotationAnimator inspector.
			EditorGUILayout.PropertyField(_mode, new GUIContent("Rotation Mode"));
			RotationAnimator.RotationMode rotationMode = (RotationAnimator.RotationMode)_mode.intValue;
			if (rotationMode == RotationAnimator.RotationMode.Custom)
			{
				EditorGUILayout.PropertyField(_customAxis, new GUIContent("Custom Axis"));
			}
			
			EditorGUILayout.PropertyField(_space, new GUIContent("Space"));
			
			EditorGUILayout.PropertyField(_targets, new GUIContent("Targets"), true);
			
			if (_targets.arraySize > 0)
			{
				EditorGUILayout.PropertyField(_includeSelf, new GUIContent("Include self?"));
			}
			
			serializedObject.ApplyModifiedProperties();
			
			if (EditorGUI.EndChangeCheck())
			{
				EvaluateProgressCurve();
			}
		}
	}
}
