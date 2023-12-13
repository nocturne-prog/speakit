using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.Util
{
    public static class SafeAreaCaculator
    {
        public static float Ratio
        {
            get; private set;
        }

        public static Vector2 SizeDelta
        {
            get; private set;
        }

        public static void Init(CanvasScaler _scaler)
        {
            Vector2 scalerRef = _scaler.referenceResolution;
            Ratio = scalerRef.x / Screen.safeArea.width;
            SizeDelta = new Vector2(Screen.safeArea.width * Ratio, Screen.safeArea.height * Ratio);

            Debug.Log($"Screen :: {Screen.width}x{Screen.height}" +
                        $"\nSafeArea :: {Screen.safeArea.width}x{Screen.safeArea.height}" +
                        $"\nRatio :: {Ratio}" +
                        $"\nSizeDelta :: {SizeDelta}");
        }
    }
}