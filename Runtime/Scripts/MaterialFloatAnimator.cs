using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGC6543
{
	public class MaterialFloatAnimator : CurvedFloatAnimator 
	{
		[Header("Material Data")]
		
		[SerializeField] List<Material> _materials = new List<Material>();
		
		[SerializeField] string _shaderPropertyName;
		
		int _propID;


		protected override void InitializeOnAwake()
		{
			base.InitializeOnAwake();
			_propID = Shader.PropertyToID(_shaderPropertyName);
			base.OnProgressUpdate += OnProgressUpdated;
		}


		//---------------------------------------------------
		//				MaterialFloatAnimator
		//---------------------------------------------------

		void OnProgressUpdated(float progress)
		{
			if (_materials == null) return;
			foreach (var mat in _materials)
			{
				mat.SetFloat(_propID, CurrentValue);
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
		
		public void SetShaderPropertyName(string shaderPropertyName)
		{
			_shaderPropertyName = shaderPropertyName;
			_propID = Shader.PropertyToID(_shaderPropertyName);
		}
	}
}
