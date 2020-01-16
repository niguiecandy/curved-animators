using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NGC6543
{
	public class CurvedProgressAnimator : MonoBehaviour
	{
		[System.Serializable]
		public class UnityFloatEvent : UnityEvent<float> {}
		
		protected delegate void ProgressUpdatedDelegate(float progress);


		#region INSPECTOR
		
		[Tooltip("Add memo here for Inspector use")]
		[SerializeField] string _memo = "This is a memo";

		[Header("Base Settings")]
		
		#if NGC6543_CORE_EXIST
		[ClampCurve(0f, 1f, new float[] {0f, 1f})]
		#endif
		[SerializeField] AnimationCurve _progressCurve;
		
		[SerializeField] float _duration = 1f;
		
		[Tooltip("If true, the progress will be looped unless Stop() is called explicitly.")]
		public bool _loop = false;

		[Tooltip("If true, the progress will be played in reversed direction.")]
		public bool _reversed = false;

		[Tooltip("If true, the progress will be played on Start().")]
		public bool _animateOnStart = true;

		[Tooltip("If true, the progress will be played when the GameObject becomes active.")]
		public bool _animateOnEnable = true;

		[Tooltip("If true, the progress value will be the Progress Curve value at t=0 when stopped.")]
		public bool _revertOnStop = true;

		[Tooltip("If true, deactivates this GameObject when stopped.")]
		public bool _deactivateOnStop = true;


		[Header("Events")]

		[SerializeField] UnityEvent _started;
		
		[SerializeField] UnityFloatEvent _progressUpdated;
		
		[SerializeField] UnityEvent _loopPointReached;
		
		[SerializeField] UnityEvent _stopped;
		
		[Header("Debug")]
		
		#if NGC6543_CORE_EXIST
		[NotInteractable]
		[SerializeField] 
		#endif
		float _progress;
	
		#endregion	// INSPECTOR
		

		Coroutine _animateCoroutine;
		
		float _currentDuration;

		/// <summary>
		/// If you inherit from this class, register a method to this delegate.
		/// This will be invoked only when the progress is updated.
		/// </summary>
		protected ProgressUpdatedDelegate OnProgressUpdate;
		
		/// <summary>
		/// Temporary flag for current animation.
		/// </summary>
		bool _tmpRevertOnStop;
		
		/// <summary>
		/// Temporary flag for current animation.
		/// </summary>
		bool _tmpDeactivateOnStop;
		
		#region PROPERTIES
		
		/// <summary>
		/// The duration of this progress.
		/// </summary>
		/// <value></value>
		public float Duration 
		{
			get
			{
				return _duration;
				// HACK
				if (_animateCoroutine == null) return _duration;
				return _currentDuration;
			}
		}
		
		/// <summary>
		/// A normalized progress of the animation for a single loop.
		/// </summary>
		/// <value></value>
		public float Progress { get { return _progress; } }
		
		/// <summary>
		/// Invoked when this animator starts playing.
		/// </summary>
		/// <value></value>
		public UnityEvent Started { get { return _started; } }
		
		/// <summary>
		/// Invoked when the progress is updated.
		/// </summary>
		/// <value></value>
		public UnityFloatEvent ProgressUpdated { get { return _progressUpdated; } }
		
		/// <summary>
		/// Invoked when the animation has reached a loop point.
		/// </summary>
		/// <value></value>
		public UnityEvent LoopPointReached { get { return _loopPointReached; } }
	
		/// <summary>
		/// Invoked when the animation is stopped.
		/// For OneShot animations, this is invoked after LoopPointReached is invoked.
		/// </summary>
		/// <value></value>
		public UnityEvent Stopped { get { return _stopped; } }
		
		#endregion	// PROPERTIES
		

		//---------------------------------------------------
		//					VIRTUAL_METHODS
		//---------------------------------------------------
		#region VIRTUAL_METHODS
		
		/// <summary>
		/// [EMPTY_BASE_METHOD] Invoked on Reset(). If you need to initialize variables when this component is reset, do it here.
		/// </summary>
		protected virtual void OnReset() {}

		/// <summary>
		/// [EMPTY_BASE_METHOD] Invoked on Awake(). If you need to initialize variables on Awake(), do it here.
		/// </summary>
		protected virtual void InitializeOnAwake() {}

		/// <summary>
		/// [EMPTY_BASE_METHOD] Invoked on Start(). If you need to initialize variables on Start(), do it here.
		/// </summary>
		protected virtual void InitializeOnStart() {}

		#endregion  // VIRTUAL_METHODS
		

		//---------------------------------------------------
		//					UNITY_FRAMEWORK
		//---------------------------------------------------
		#region UNITY_FRAMEWORK
		
		void Reset()
		{
			_progressCurve = new AnimationCurve();
			_progressCurve.AddKey(0f, 0f);
			_progressCurve.AddKey(1f, 1f);
			OnReset();
		}
		
		void Awake()
		{
			// Initialize temporary flags
			_tmpRevertOnStop = _revertOnStop;
			_tmpDeactivateOnStop = _deactivateOnStop;
			
			InitializeOnAwake();
		}

		void Start()
		{
			InitializeOnStart();
			
			if (_animateOnStart)
			{
				if (_loop)
				{
					if (_reversed) PlayLoopReversed();
					else PlayLoop();
				}
				else
				{
					if (_reversed) PlayOneShotReversed();
					else PlayOneShot();
				}
			}
		}

		void OnEnable()
		{
			if (_animateOnEnable)
			{
				if (_loop)
				{
					if (_reversed) PlayLoopReversed();
					else PlayLoop();
				}
				else
				{
					if (_reversed) PlayOneShotReversed();
					else PlayOneShot();
				}
			}
		}
		
		void OnDisable()
		{
			// If this GameObject is deactivated while playing, force stop playing.
			if (_animateCoroutine != null)
			{
				Stop(_tmpRevertOnStop, _tmpDeactivateOnStop);
			}
		}
		
		#endregion // UNITY_FRAMEWORK


		//---------------------------------------------------
		//				PUBLIC_METHODS_PLAY_STOP
		//---------------------------------------------------
		#region PUBLIC_METHODS_PLAY_STOP

		/// <summary>
		/// Plays the animation once.
		/// </summary>
		public void PlayOneShot()
		{
			PlayOneShot(_duration, _revertOnStop, _deactivateOnStop);
		}
		
		/// <summary>
		/// Plays the animation once.
		/// </summary>
		/// <param name="duration"></param>
		/// <param name="revertOnStop"></param>
		/// <param name="deactivateOnStop"></param>
		public void PlayOneShot(float duration, bool revertOnStop, bool deactivateOnStop)
		{
			gameObject.SetActive(true);
			if (_animateCoroutine != null) StopCoroutine(_animateCoroutine);
			_tmpRevertOnStop = revertOnStop;
			_tmpDeactivateOnStop = deactivateOnStop;
			_animateCoroutine = StartCoroutine(AnimateOneShot(duration, revertOnStop, deactivateOnStop));
		}

		/// <summary>
		/// Plays the animation once(in reversed direction : from 1 to 0).
		/// </summary>
		public void PlayOneShotReversed()
		{
			PlayOneShotReversed(_duration, _revertOnStop, _deactivateOnStop);
		}

		/// <summary>
		/// Plays the animation once(in reversed direction : from 1 to 0).
		/// </summary>
		/// <param name="duration"></param>
		/// <param name="revertOnStop"></param>
		/// <param name="deactivateOnStop"></param>
		public void PlayOneShotReversed(float duration, bool revertOnStop, bool deactivateOnStop)
		{
			gameObject.SetActive(true);
			if (_animateCoroutine != null) StopCoroutine(_animateCoroutine);
			_tmpRevertOnStop = revertOnStop;
			_tmpDeactivateOnStop = deactivateOnStop;
			_animateCoroutine = StartCoroutine(AnimateOneShotReversed(duration, revertOnStop, deactivateOnStop));
		}

		/// <summary>
		/// Plays the looping animation.
		/// </summary>
		public void PlayLoop()
		{
			PlayLoop(_duration);
		}

		/// <summary>
		/// Plays the looping animation.
		/// </summary>
		/// <param name="duration">Duration of animation for each loop.</param>
		public void PlayLoop(float duration)
		{
			gameObject.SetActive(true);
			if (_animateCoroutine != null) StopCoroutine(_animateCoroutine);
			_animateCoroutine = StartCoroutine(AnimateLoop(duration));
		}

		/// <summary>
		/// Plays the looping animation(in reversed direction : from 1 to 0).
		/// </summary>
		public void PlayLoopReversed()
		{
			PlayLoopReversed(_duration);
		}

		/// <summary>
		/// Plays the looping animation(in reversed direction : from 1 to 0).
		/// </summary>
		/// <param name="duration">Duration of animation for each loop.s</param>
		public void PlayLoopReversed(float duration)
		{
			gameObject.SetActive(true);
			if (_animateCoroutine != null) StopCoroutine(_animateCoroutine);
			_animateCoroutine = StartCoroutine(AnimateLoopReversed(duration));
		}
		
		/// <summary>
		/// Stops this animation.
		/// </summary>
		public void Stop()
		{
			Stop(_tmpRevertOnStop, _tmpDeactivateOnStop);
		}
		
		/// <summary>
		/// Stops this animation.
		/// </summary>
		/// <param name="revert">If true, the progress will be the Progress Curve value at t=0.</param>
		/// <param name="deactivate">If true, the GameObject this component is attached to will be deactivated.</param>
		public void Stop(bool revert, bool deactivate)
		{
			if (_animateCoroutine != null)
			{
				StopCoroutine(_animateCoroutine);
				_animateCoroutine = null;
	
			} 
			if (revert) Revert();
			if (deactivate) gameObject.SetActive(false);
			
			_stopped.Invoke();
		}

		#endregion // PUBLIC_METHODS_PLAY_STOP

		
		//---------------------------------------------------
		//				PUBLIC_METHODS_PROGRESS
		//---------------------------------------------------
		#region PUBLIC_METHODS_PROGRESS
		
		/// <summary>
		/// Sets a Progress Curve. It is recommended that the curve timespan lies between 0 and 1.
		/// </summary>
		/// <param name="progressCurve">If the curve value is outside the range between 0 and 1, it may cause unexpected behaviours.</param>
		public void SetProgressCurve(AnimationCurve progressCurve)
		{
			_progressCurve = progressCurve;
		}
		
		/// <summary>
		/// Sets a progress.
		/// </summary>
		/// <param name="progress">The value will be clamped between 0 and 1.</param>
		public void SetProgress(float progress)
		{
			_progress = Mathf.Clamp01(progress);
			UpdateProgress(_progress);
		}
		
		#endregion	// PUBLIC_METHODS_PROGRESS
		
		
		//---------------------------------------------------
		//					IENUMERATORS
		//---------------------------------------------------
		#region IENUMERATORS
		
		IEnumerator AnimateOneShot(float duration, bool revertOnEnd, bool deactivateOnStop)
		{
			_started.Invoke();

			if (duration > 0f)
			{
				float prog = 0f;
				while (prog < 1f)
				{
					_progress = _progressCurve.Evaluate(prog);
					UpdateProgress(_progress);
					prog += Time.deltaTime / duration;
					yield return null;
				}
			}
			_progress = _progressCurve.Evaluate(1f);
			UpdateProgress(_progress);
			Stop(revertOnEnd, deactivateOnStop);
			_loopPointReached.Invoke();
		}

		IEnumerator AnimateOneShotReversed(float duration, bool revertOnEnd, bool deactivateOnStop)
		{
			_started.Invoke();

			if (duration > 0f)
			{
				float prog = 1f;
				while (0 < prog)
				{
					_progress = _progressCurve.Evaluate(prog);
					UpdateProgress(_progress);
					prog -= Time.deltaTime / duration;
					yield return null;
				}
			}
			_progress = _progressCurve.Evaluate(0f);
			UpdateProgress(_progress);
			Stop(revertOnEnd, deactivateOnStop);
			_loopPointReached.Invoke();
		}

		IEnumerator AnimateLoop(float duration)
		{
			_started.Invoke();
			if (duration <= 0f)
			{
				Debug.LogWarning(gameObject.name + " : The duration should be greater than 0! Changed to 1.");
				duration = 1f;
			}
			while (true)
			{
				float prog = 0f;
				while (prog < 1)
				{
					_progress = _progressCurve.Evaluate(prog);
					UpdateProgress(_progress);
					prog += Time.deltaTime / duration;
					yield return null;
				}
				_progress = _progressCurve.Evaluate(1f);
				UpdateProgress(_progress);
				_loopPointReached.Invoke();
				yield return null;
			}
		}

		IEnumerator AnimateLoopReversed(float duration)
		{
			_started.Invoke();
			if (duration <= 0f)
			{
				Debug.LogWarning(gameObject.name + " : The duration should be greater than 0! Changed to 1.");
				duration = 1f;
			}
			while (true)
			{
				float prog = 1f;
				while (0 < prog)
				{
					_progress = _progressCurve.Evaluate(prog);
					UpdateProgress(_progress);
					prog -= Time.deltaTime / duration;
					yield return null;
				}
				_progress = _progressCurve.Evaluate(0f);
				UpdateProgress(_progress);
				_loopPointReached.Invoke();
				yield return null;
			}
		}
		
		#endregion	// IENUMERATORS
		
		
		/// <summary>
		/// Invoked when the progress is updated.
		/// </summary>
		/// <param name="progress"></param>
		void UpdateProgress(float progress)
		{
			// Update delegates
			if (OnProgressUpdate != null)
			{
				OnProgressUpdate(_progress);
			}
			
			// Invoke events
			_progressUpdated.Invoke(progress);
		}
		
		/// <summary>
		/// Set progress to 0.
		/// </summary>
		void Revert()
		{
			_progress = 0f;
			UpdateProgress(_progress);
		}
	}
}
