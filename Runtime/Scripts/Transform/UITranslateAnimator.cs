using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NGC6543{
	[RequireComponent(typeof(RectTransform))]
	public sealed class UITranslateAnimator : CurvedFloatAnimator
	{
		public enum TranslateAxis{X,Y,Z,Custom}
		[Header("Transform Animator Setting")]
		[SerializeField] TranslateAxis transformAxis = TranslateAxis.X;
		//[SerializeField] Space space = Space.Self;
		[SerializeField,
		Tooltip("True : AnimationCurve value will be added to the initial position\n" +
		     "False : Transform position will be overriden")] bool additive;
		[SerializeField] Vector3 initPos;
		[SerializeField, HideInInspector] RectTransform rTransform;

		void Reset(){
			rTransform = GetComponent<RectTransform>();
		}
		
		protected override void InitializeOnAwake()
		{
			initPos = rTransform.anchoredPosition3D;
			base.OnProgressUpdate += OnProgressUpdated;
		}
		
		void OnProgressUpdated(float progress)
		{
			Vector3 v = initPos;
			switch (transformAxis)
			{
				case TranslateAxis.X:
					v.x = additive ? v.x + CurrentValue : CurrentValue;
					//if(space == Space.Self){
					//	transform.localPosition = v;
					//}else{
					//	transform.position = v;
					//}
					break;
				case TranslateAxis.Y:
					v.y = additive ? v.y + CurrentValue : CurrentValue;
					//if(space == Space.Self){
					//	transform.localPosition = v;
					//}else{
					//	transform.position = v;
					//}
					break;
				case TranslateAxis.Z:
					v.z = additive ? v.z + CurrentValue : CurrentValue;
					//if(space == Space.Self){
					//	transform.localPosition = v;
					//}else{
					//	transform.position = v;
					//}
					break;
				case TranslateAxis.Custom:
					v.x += CurrentValue;
					//if(space == Space.Self){
					//	transform.localPosition = v;
					//}else{
					//	transform.position = v;
					//}
					break;
				default:
					break;
			}
			rTransform.anchoredPosition3D = v;
		}
	}

}
