using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace NGC6543 
{
	public class ColorAnimator : CurvedProgressAnimator 
    {
		[System.Serializable]
		public class ColorEvent : UnityEvent<Color> {}
		
		[Header("Color Animation Data")]

		[SerializeField] Gradient _gradient;
		
		[Header("Event")]
		
        [SerializeField] ColorEvent _updated;

		[Header("Debug")]
		
		#if NGC6543_CORE_EXIST
		[SerializeField, NotInteractable]
		#endif
		protected Color _initColor;

		#if NGC6543_CORE_EXIST
		[SerializeField, NotInteractable]
		#endif
		protected Color _currentColor;
		

        //=== Properties
		
        public Color CurrentColor{get{return _currentColor;}}
		
        public ColorEvent Updated{ get{ return _updated;}}
		
		
		//---------------------------------------------------
		//				OVERRIDEN_METHODS
		//---------------------------------------------------

		protected override void InitializeOnAwake()
		{
			if (_gradient.colorKeys.Length < 1)
			{
				Debug.LogWarning("ColorAnimator " + gameObject.name + " doesn't have color key in Gradient");
				_initColor = Color.white;
			}
			else
			{
				_initColor = _gradient.Evaluate(0f);
			}
			_currentColor = _initColor;
			
			base.OnProgressUpdate += OnProgressUpdated;
		}

		void OnProgressUpdated(float progress)
		{
			_currentColor = _gradient.Evaluate(progress);
			_updated.Invoke(_currentColor);
		}
	}


}
