using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGC6543
{
	public class MaterialUVOffsetAnimator : CurvedProgressAnimator
	{
		public enum UVAxis {NONE = 0x00, U = 0x01, V = 0x10, UV = U|V }
		#if NGC6543_CORE_EXIST
		[ClampCurve(0f, 1f)]
		#endif
		[SerializeField] AnimationCurve _uCurve, _vCurve;
		
		[Header("Material Data")]
		
		[SerializeField] UVAxis _offsetAxis = UVAxis.U;
		
		[SerializeField] List<Material> _materials = new List<Material>();
		
		[SerializeField] string _texturePropertyName = "_MainTex";
		
		int _texPropID;
		
		void Reset()
		{
			_uCurve = new AnimationCurve
					(
						new Keyframe[]
						{
							new Keyframe(0f, 0f),
							new Keyframe(1f, 1f)
						}
					);
			_vCurve = new AnimationCurve
					(
						new Keyframe[]
						{
							new Keyframe(0f, 0f),
							new Keyframe(1f, 1f)
						}
					);
		}
		
		protected override void InitializeOnAwake()
		{
			base.InitializeOnAwake();
			_texPropID = Shader.PropertyToID(_texturePropertyName);
			OnProgressUpdate += OnProgressUpdated;
		}
		
		void OnProgressUpdated(float progress)
		{
			if ( _offsetAxis == UVAxis.NONE) return;
			if (_materials == null) return;
			
			Vector2 uv = Vector2.zero;
			bool updateU = false, updateV = false;
			if ((_offsetAxis & UVAxis.U) > 0)
			{
				updateU = true;
				uv.x = _uCurve.Evaluate(progress);
			}
			if ((_offsetAxis & UVAxis.V) > 0)
			{
				updateV = true;
				uv.y = _vCurve.Evaluate(progress);
			}
			
			
			foreach (var mat in _materials)
			{
				var uv1 = mat.GetTextureOffset(_texPropID);
				if (updateU) uv1.x = uv.x;
				if (updateV) uv1.y = uv.y;
				mat.SetTextureOffset(_texPropID, uv1);
			}
		}


		public void AddMaterial(Material material)
		{
			if (_materials.Contains(material)) return;
			_materials.Add(material);
		}


		public void RemoveMaterial(Material material)
		{
			for (int i = 0; i < _materials.Count; i++)
			{
				if (_materials[i] == material)
				{
					_materials.RemoveAt(i);
					i--;
				}
			}
		}


		public void SetTexturePropertyName(string texturePropertyName)
		{
			_texturePropertyName = texturePropertyName;
			_texPropID = Shader.PropertyToID(_texturePropertyName);
		}
	}
	
}
