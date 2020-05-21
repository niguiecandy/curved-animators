using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGC6543
{
	public class PositionAnimator : CurvedProgressAnimator
	{
		public enum TranslateAxes
		{ 
			X = 0x001, Y = 0x010, Z = 0x100,
			XY = X|Y, YZ = Y|Z, XZ = X|Z,
			XYZ = X|Y|Z
		}
		
		[Header("Position Animator Settings")]
		
		[Tooltip("Self : Translates along local transform axis.\nWorld : Translates along world axis.")]
		[SerializeField] Space _space = Space.Self;
		
		[SerializeField] TranslateAxes _axes = TranslateAxes.Y;
		
		[SerializeField] AnimationCurve _xCurve, _yCurve, _zCurve;
		
		[Header("Targets")]
		
		[SerializeField] Transform[] _targets;
		
		Vector3 _initPos;
		Vector3[] _targetInitPos;
		
		//---------------------------------------------------
		//				OVERRIDEN_METHODS
		//---------------------------------------------------
		#region OVERRIDEN_METHODS
		
		protected override void InitializeOnAwake()
		{
			Started.AddListener(InitStartPosition);
			OnProgressUpdate += UpdatePosition;
		}
	
		#endregion    // OVERRIDEN_METHODS
		
		void InitStartPosition()
		{
			_initPos = transform.position;
		}
		
		void UpdatePosition(float progress)
		{
			Vector3 delta = Vector3.zero;
			
			// Evaluate curves
			if ((_axes & TranslateAxes.X) > 0) 
			{
				delta.x = _xCurve.Evaluate(progress);
			}
			if ((_axes & TranslateAxes.Y) > 0)
			{
				delta.y = _yCurve.Evaluate(progress);
			}
			if ((_axes & TranslateAxes.Z) > 0)
			{
				delta.z = _zCurve.Evaluate(progress);
			}
			
			// Add delta
			Vector3 resultPosition = transform.position;
			switch (_space)
			{
				case Space.Self :
					resultPosition = transform.TransformPoint(delta);
					break;
				case Space.World :
					resultPosition += delta;
					break;
			}
			
			transform.position = resultPosition;
		}
		
		
	}
}
