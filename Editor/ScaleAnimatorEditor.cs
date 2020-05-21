using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NGC6543
{
	[CustomEditor(typeof(ScaleAnimator)), CanEditMultipleObjects]
	public class ScaleAnimatorEditor : CurvedProgressAnimatorEditor
	{
		const string k_axes = "_axes";
		const string k_xCurve = "_xCurve";
		const string k_yCurve = "_yCurve";
		const string k_zCurve = "_zCurve";
		const string k_isotropicCurve = "_isotropicCurve";
		const string k_targets = "_targets";
		const string k_rectTransformTargets = "_rectTransformTargets";
		const string k_includeSelf = "_includeSelf";

		ScaleAnimator _component;

		SerializedProperty _scaleMode;
		SerializedProperty _xCurve, _yCurve, _zCurve, _isotropicCurve;
		SerializedProperty _targets, _rectTransformTargets, _includeSelf;

		string[] _excludedProperties =
		{
			k_axes,
			k_xCurve, k_yCurve, k_zCurve, k_isotropicCurve,
			k_targets, k_rectTransformTargets, k_includeSelf
		};

		protected override void OnEnable()
		{
			base.OnEnable();
			_component = target as ScaleAnimator;

			_scaleMode = serializedObject.FindProperty(k_axes);
			_xCurve = serializedObject.FindProperty(k_xCurve);
			_yCurve = serializedObject.FindProperty(k_yCurve);
			_zCurve = serializedObject.FindProperty(k_zCurve);
			_isotropicCurve = serializedObject.FindProperty(k_isotropicCurve);
			_targets = serializedObject.FindProperty(k_targets);
			_rectTransformTargets = serializedObject.FindProperty(k_rectTransformTargets);
			_includeSelf = serializedObject.FindProperty(k_includeSelf);
		}

		public override void OnInspectorGUI()
		{
			DrawHeaderHelpBox();

			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			// Draw CurvedProgressAnimator inspector.
			DrawPropertiesExcluding(serializedObject, _excludedProperties);

			// Draw ScaleAnimator inspector.
			EditorGUILayout.PropertyField(_scaleMode, new GUIContent("Scale Mode"));
			ScaleAnimator.ScaleAxes scaleMode = (ScaleAnimator.ScaleAxes)_scaleMode.intValue;
			if (scaleMode == ScaleAnimator.ScaleAxes.ISOTROPIC)
			{
				// Isometric scaling. Use X Curve as a control source.
				EditorGUILayout.PropertyField(_isotropicCurve, new GUIContent("Isotropic Scale Curve"));
			}
			else
			{
				if ((scaleMode & ScaleAnimator.ScaleAxes.X) > 0)
				{
					EditorGUILayout.PropertyField(_xCurve, new GUIContent("X-Axis"));
				}
				if ((scaleMode & ScaleAnimator.ScaleAxes.Y) > 0)
				{
					EditorGUILayout.PropertyField(_yCurve, new GUIContent("Y-Axis"));
				}
				if ((scaleMode & ScaleAnimator.ScaleAxes.Z) > 0)
				{
					EditorGUILayout.PropertyField(_zCurve, new GUIContent("Z-Axis"));
				}
			}

			EditorGUILayout.PropertyField(_targets, new GUIContent("Targets"), true);

			EditorGUILayout.PropertyField(_rectTransformTargets, new GUIContent("RectTransform Targets"), true);

			if (_targets.arraySize != 0 || _rectTransformTargets.arraySize != 0)
			{
				EditorGUILayout.PropertyField(_includeSelf, new GUIContent("Include Self?"));
			}

			serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck())
			{
				EvaluateProgressCurve();
			}
		}
	}

}
