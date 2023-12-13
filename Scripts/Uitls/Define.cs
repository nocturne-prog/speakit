using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marvrus.Util
{
    public static class Define
    {
        public enum Platform
        {
            UnityEditor = 0,
            Android,
            iOS,
        }

        public static Platform platform
        {
            get
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return Platform.Android;
#elif !UNITY_EDITOR && UNITY_IOS
                return Platform.iOS;
#else
                return Platform.UnityEditor;
#endif

            }
        }
    }
}