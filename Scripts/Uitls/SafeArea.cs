using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.Util
{
    public class SafeArea : MonoBehaviour
    {
        public CanvasScaler scaler;
        private RectTransform trf;

        public void Awake()
        {
            trf = GetComponent<RectTransform>();
            trf.sizeDelta = new Vector2(Screen.width, Screen.height);
        }

        public void ApplySafeArea()
        {
            if (scaler is null)
            {
                scaler = Extension.FindParent<CanvasScaler>(this.transform);

                if (scaler is null)
                {
                    Debug.LogError("SafeArea :: can't find CanvasScaler..");
                    return;
                }
            }

            trf.sizeDelta = SafeAreaCaculator.SizeDelta;
        }

#if UNITY_EDITOR
        private void FixedUpdate()
        {
            ApplySafeArea();
        }
#endif
    }
}