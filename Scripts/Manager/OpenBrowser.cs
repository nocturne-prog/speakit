using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PPM = Marvrus.Util.PlayerPrefsManager;
using DF = Marvrus.Util.Define;
#if UNITY_IOS
using KS = Kogane.SFSafariView;
#endif

namespace Marvrus.Util
{
    public static class OpenBrowser
    {
        public enum HostType
        {
            None = 0,
            SignUp,
            SignIn,
            Logout,
            Profile,
            Profile_close,
            profile_withdrawal
        }

        private static readonly Dictionary<HostType, string> redirectUrl = new Dictionary<HostType, string>()
        {
            { HostType.SignUp,                      "*****" },
            { HostType.SignIn,                      "*****" },
            { HostType.Logout,                      "*****" },
            { HostType.Profile,                     "*****" },
            { HostType.Profile_close,               "*****" },
            { HostType.profile_withdrawal,          "*****" },
        };

        public static Action<string> callback = null;
        private static string baseUrl = "-";

        public static void OpenUrl(string _url)
        {
            if (DF.platform == DF.Platform.Android)
            {
                Debug.Log($"OpenURL :: {_url}");
                Application.OpenURL($"{_url}");
            }
            else if (DF.platform == DF.Platform.iOS)
            {
                Debug.Log($"SFSafariView :: {_url}");

                #if UNITY_IOS
                KS.OpenURL($"{_url}");
                #endif
            }
            else
            {
                Debug.Log($"{DF.platform} is not support..");
            }
        }

        public static void SignUp(Action<string> _callback)
        {
            string url = "*****";

            callback = _callback;
            OpenUrl(url);
        }

        public static void SignIn(Action<string> _callback)
        {
            string url = "*****";

            callback = _callback;
            OpenUrl(url);
        }

        public static void LogOut(Action<string> _callback)
        {
            string url = "*****";

            callback = _callback;
            OpenUrl(url);
        }

        public static void WithDrawal(Action<string> _callback)
        {
            string url = "*****";

            callback = _callback;
            OpenUrl(url);
        }

        public static void MyProfile(Action<string> _callback)
        {
            string url = "*****";

            callback = _callback;
            OpenUrl(url);
        }

        public static HostType GetHostType(string _value)
        {
            foreach (var v in redirectUrl)
            {
                if (v.Value.Equals(_value))
                {
                    return v.Key;
                }
            }

            return HostType.None;
        }
    }
}
