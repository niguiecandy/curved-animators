using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NGC6543
{
	public class ScaleAnimator : CurvedProgressAnimator
	{
		public enum ScaleAxes
		{
			X = 0x001, Y = 0x010, Z = 0x100,
			XY = X | Y, YZ = Y | Z, XZ = X | Z,
			XYZ = X | Y | Z, ISOTROPIC = 0x1000
		};

		[Header("Scale Animator Settings")]

		[SerializeField] protected ScaleAxes _axes = ScaleAxes.XYZ;

		[SerializeField] protected AnimationCurve _xCurve, _yCurve, _zCurve, _isotropicCurve;

		[Header("Targets")]

		[Tooltip("If Targets is null, self transform will be affected.")]
		[SerializeField] Transform[] _targets;

		[SerializeField] RectTransform[] _rectTransformTargets;

		[Tooltip("Only takes effect if Targets is not null.")]
		[SerializeField] bool _includeSelf;

		Vector3 _initScale;
		Vector3[] _targetInitScales;
		Vector3[] _rectTransformInitScales;

		//-------- PROPERTIES

		public ScaleAxes Axes { get { return _axes; } }


		//---------------------------------------------------
		//						METHODS
		//---------------------------------------------------
		#region METHODS

		protected override void InitializeOnAwake()
		{
			// Scale Animator
			_initScale = transform.localScale;
			_targetInitScales = new Vector3[_targets.Length];
			for (int i = 0; i < _targets.Length; i++)
			{
				_targetInitScales[i] = _targets[i].localScale;
			}

			_rectTransformInitScales = new Vector3[_rectTransformTargets.Length];
			for (int i = 0; i < _rectTransformTargets.Length; i++)
			{
				_rectTransformInitScales[i] = _rectTransformTargets[i].localScale;
			}

			base.OnProgressUpdate += OnProgressUpdated;
		}

		void OnProgressUpdated(float progress)
		{
			Vector3 scale = Vector3.one;
			if (_axes == ScaleAxes.ISOTROPIC)
			{
				scale.x = scale.y = scale.z = _isotropicCurve.Evaluate(progress);
			}
			else
			{
				if ((_axes & ScaleAxes.X) > 0)
				{
					scale.x = _xCurve.Evaluate(progress);
				}
				if ((_axes & ScaleAxes.Y) > 0)
				{
					scale.y = _yCurve.Evaluate(progress);
				}
				if ((_axes & ScaleAxes.Z) > 0)
				{
					scale.z = _zCurve.Evaluate(progress);
				}
			}

			if (_targets.Length == 0 && _rectTransformTargets.Length == 0)
			{
				transform.localScale = GetScale(_initScale, scale);
			}
			else
			{
				for (int i = 0; i < _targets.Length; i++)
				{
					if (_targets[i] == null) continue;
					_targets[i].localScale = GetScale(_targetInitScales[i], scale);
				}

				for (int i = 0; i < _rectTransformTargets.Length; i++)
				{
					if (_rectTransformTargets[i] == null) continue;
					_rectTransformTargets[i].localScale = GetScale(_rectTransformInitScales[i], scale);
				}

				if (_includeSelf)
				{
					transform.localScale = GetScale(_initScale, scale);
				}
			}
		}

		protected Vector3 GetScale(Vector3 initScale, Vector3 scale)
		{
			Vector3 result;
			result.x = initScale.x * scale.x;
			result.y = initScale.y * scale.y;
			result.z = initScale.z * scale.z;
			return result;
		}

		#endregion // METHODS
	}
}
