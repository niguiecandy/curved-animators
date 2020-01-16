using UnityEngine;
using System.Collections;

namespace NGC6543{
	public sealed class RotationAnimator : CurvedFloatAnimator
	{
		public enum RotationMode { X, Y, Z, Custom };
		
		[Header("Rotation Animator Settings")]
		
		[SerializeField] RotationMode _mode;
		
		[SerializeField] Vector3 _customAxis = Vector3.forward;
		
		[SerializeField] Space _space = Space.Self;
		
		[Header("Targets")]
		
		[SerializeField] Transform[] _targets;
		
		[SerializeField] bool _includeSelf;
		
		
		Vector3 _initEulerAngles;
		
		Vector3[] _targetsInitEulerAngles;
		
		
		Vector3 _axis;
		
		
		public RotationMode Mode { get { return _mode; } }
		
		public Vector3 RotationAxis { get { return _axis; } }
		
		
		//---------------------------------------------------
		//						METHODS
		//---------------------------------------------------
		#region METHODS
		
		protected override void InitializeOnAwake()
		{
			base.InitializeOnAwake();
			
			_initEulerAngles = _space == Space.Self ? transform.localEulerAngles : transform.eulerAngles;
			
			_targetsInitEulerAngles = new Vector3[_targets.Length];
			for (int i = 0; i < _targets.Length; i++)
			{
				_targetsInitEulerAngles[i] = _space == Space.Self ? _targets[i].localEulerAngles : _targets[i].eulerAngles;
			}
			
			switch (_mode)
			{
				case RotationMode.X:
					_axis = Vector3.right;
					break;
				case RotationMode.Y:
					_axis = Vector3.up;
					break;
				case RotationMode.Z:
					_axis = Vector3.forward;
					break;
				case RotationMode.Custom:
					_axis = _customAxis;
					break;
				default:
					_mode = RotationMode.Z;
					_axis = Vector3.forward;
					break;
			}
			base.OnProgressUpdate += OnProgressUpdated;
		}
		
		void OnProgressUpdated(float progress)
		{	
			Vector3 eulerAngles = Vector3.zero;
			
			if (_targets.Length == 0)
			{
				switch (_space)
				{
					case Space.Self:
						transform.localEulerAngles = _initEulerAngles + _axis * CurrentValue;
						break;
					case Space.World:
						transform.eulerAngles = _initEulerAngles + _axis * CurrentValue;
						break;
					default:
						break;
				}
			}
			else
			{
				switch (_space)
				{
					case Space.Self:
						for (int i = 0; i < _targets.Length; i++)
						{
							_targets[i].localEulerAngles = _targetsInitEulerAngles[i] + _axis * CurrentValue;
						}
						if (_includeSelf)
						{
							transform.localEulerAngles = _initEulerAngles + _axis * CurrentValue;
						}
						break;
					case Space.World:
						for (int i = 0; i < _targets.Length; i++)
						{
							_targets[i].eulerAngles = _targetsInitEulerAngles[i] + _axis * CurrentValue;
						}
						if (_includeSelf)
						{
							transform.eulerAngles = _initEulerAngles + _axis * CurrentValue;
						}
						break;
					default:
						break;
				}
				
			}
			
		}
		
		#endregion	// METHODS
	}
}

