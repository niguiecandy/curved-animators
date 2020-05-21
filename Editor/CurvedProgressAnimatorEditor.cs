using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NGC6543
{
    [CustomEditor(typeof(CurvedProgressAnimator), true), CanEditMultipleObjects]
    public class CurvedProgressAnimatorEditor : Editor
    {
		SerializedProperty _progressCurve;
		
		protected virtual void OnEnable()
		{
			_progressCurve = serializedObject.FindProperty("_progressCurve");
			EvaluateProgressCurve();
		}
		
		public override void OnInspectorGUI()
		{
			DrawHeaderHelpBox();
			
			EditorGUI.BeginChangeCheck();
			base.OnInspectorGUI();
			#if !NGC6543_CORE_EXIST
			if (EditorGUI.EndChangeCheck())
			{
				EvaluateProgressCurve();
			}
			#endif
		}
		
		protected void DrawHeaderHelpBox()
		{
			EditorGUILayout.HelpBox("A Note for the Progress Curve\n" +
				"The first and last KeyFrame times are fixed to 0 and 1, respectively.\n" +
				"All the KeyFrame values are clamped between 0 and 1.\n" +
				"!! Curve values may exceeds range between 0 and 1 when modifying KeyFrame tangents. This may cause unexpected behaviour!!"
				, MessageType.Warning);
		}
		
		protected virtual void EvaluateProgressCurve()
		{
			serializedObject.Update();
			AnimationCurve tmpCurve = _progressCurve.animationCurveValue;
			if (tmpCurve.keys.Length == 0)
			{
				tmpCurve.AddKey(0f, 0f);
			}
			if (tmpCurve.keys.Length < 2)
			{
				Debug.LogWarning("Progress Curve must have at leat 2 KeyFrames!");
				Keyframe tmpKey = tmpCurve.keys[0];
				tmpCurve.AddKey(tmpKey.time + 1f, 1f);
				_progressCurve.animationCurveValue = tmpCurve;
			}
			
			// Progress Curve Keyframe restriction
			Keyframe[] keys = tmpCurve.keys;
			for (int i = 0; i < keys.Length; i++)
			{
				keys[i].time = Mathf.Clamp01(keys[i].time);
				keys[i].value = Mathf.Clamp01(keys[i].value);
			}
			// The first key should be at 0 second
			keys[0].time = 0f;
			// The last key should be at 1 second
			keys[keys.Length - 1].time = 1f;
			
			tmpCurve.keys = keys;
			_progressCurve.animationCurveValue = tmpCurve;
			
			serializedObject.ApplyModifiedProperties();
		}
    }

}
