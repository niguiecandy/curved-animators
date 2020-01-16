using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGC6543
{
    public class InstancedColorAnimator : MonoBehaviour 
    {
        #if UNITY_EDITOR
        [SerializeField] string memo;
        #endif
        [SerializeField] Renderer[] renderers;
        [SerializeField] string paramColorName = "_Color";
        [SerializeField] ColorAnimator colorAnim;
        [SerializeField] Color defaultColor = Color.white;

        MaterialPropertyBlock mpb;
        bool initialized;
        int paramColorID;

        void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            if (colorAnim != null)
                colorAnim.Updated.RemoveListener(OnColorUpdated);
        }

        private void Initialize()
        {
            if (renderers == null)
            {
                Debug.LogError("Renderers are not set!");
                return;
            }
            if (colorAnim == null)
            {
                Debug.LogError(gameObject.name + " : Color Animator is not set!");
                return;
            }
            initialized = true;
            mpb = new MaterialPropertyBlock();
            paramColorID = Shader.PropertyToID(paramColorName);
            colorAnim.Updated.AddListener(OnColorUpdated);
       }

        public void Play()
        {
            if (!initialized)
            {
                Initialize();
                if (!initialized) return;
            }
            colorAnim.PlayOneShot();
        }

        public void ResetColor()
        {
            OnColorUpdated(defaultColor);
        }

        public void ResetColor(Color _c)
        {
            OnColorUpdated(_c);
        }

        void OnColorUpdated(Color _c)
        {
            mpb.SetColor(paramColorID, _c);
            for (int i = 0; i < renderers.Length; i++)
            {
				renderers[i].SetPropertyBlock(mpb);
            }
        }
    }
}

