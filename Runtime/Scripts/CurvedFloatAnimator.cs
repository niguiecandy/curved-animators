using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGC6543
{
	public class CurvedFloatAnimator : CurvedProgressAnimator 
	{
		public enum CurveEvaluationMethod
		{
			/// <summary>
			/// The curve's total time span is normalized to duration.
			/// </summary>
			Normalized,

			/// <summary>
			/// The curve's time is absolutely equivalent to that of duration.
			/// </summary>
			AbsoluteDuration
		}

		[Header("Float Curve Data")]

		[SerializeField] protected AnimationCurve _curve;

		[Header("Event")]

		[SerializeField] CurvedProgressAnimator.UnityFloatEvent _updated;

		[Header("Debug")]

		// first keyframe's time in AnimationCurve
		float _key0Time;

		// time span between the first keyframe and last keyframe
		float _curveTimespan;

		float _initValue;

		// 1 if there is only one or zero keyframe.
		#if NGC6543_CORE_EXIST
		[NotInteractable]
		[SerializeField]
		#endif
		float _currentValue;

		//=== Properties

		/// <summary>
		/// The initial value. Equal to the value at the first keyframe.
		/// </summary>
		/// <value></value>
		public float InitialValue { get { return _initValue; } }

		/// <summary>
		/// The current animation value.
		/// </summary>
		/// <value></value>
		public float CurrentValue { get { return _currentValue; } }

		/// <summary>
		/// The evaluated value is invoked for each Update.
		/// </summary>
		/// <returns></returns>
		public CurvedProgressAnimator.UnityFloatEvent Updated { get { return _updated; } }

		
		//---------------------------------------------------
		//					UNITY_FRAMEWORK
		//---------------------------------------------------
		
		protected override void OnReset()
		{
			_curve = new AnimationCurve();
			_curve.AddKey(0f, 0f);
			_curve.AddKey(1f, 1f);
		}
		
		
		//---------------------------------------------------
		//				OVERRIDEN_METHODS
		//---------------------------------------------------

		protected override void InitializeOnAwake()
		{
			if (_curve.keys.Length <= 1)
			{
				Debug.LogWarning("AnimationCurve 'Scale Curve' doesn't have enough keys.");
				_key0Time = 0f;
				_curveTimespan = 1f;
				_initValue = _currentValue = 0f;
			}
			else
			{
				_key0Time = _curve.keys[0].time;
				_curveTimespan = _curve.keys[_curve.length - 1].time - _key0Time;
				_initValue = _currentValue = _curve.Evaluate(_key0Time);
			}

			base.OnProgressUpdate += OnProgressUpdated;
		}

		void OnProgressUpdated(float progress)
		{
			_currentValue = _curve.Evaluate(_key0Time + progress * _curveTimespan);
			_updated.Invoke(_currentValue);
		}
	}	
}