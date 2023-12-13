using System;
using UnityEngine;
using UnityEngine.Android;
using DF = Marvrus.Util.Define;

namespace Marvrus.Util
{
    public enum PermissionType
    {
        Read = 0,
        Write,
        Camera,
        Microphone,
        FineLocation,
        CoarseLocation,
    }

    public enum PermissionCallbackType
    {
        PermissionGranted = 0,
        PermissionDenied,
        PermissionDeniedAndDontAskAgain
    }

    public static class PermissionCheck
    {
        public static bool CheckPermission(UserAuthorization _type)
        {
            return Application.HasUserAuthorization(_type);
        }

        public static bool CheckPermission(PermissionType _type)
        {
            if (DF.platform == DF.Platform.UnityEditor)
                return true;

#if UNITY_IOS
            if (_type == PermissionType.Microphone)
            {
                return Application.HasUserAuthorization(UserAuthorization.Microphone);
            }
            else if (_type == PermissionType.Camera)
            {
                return Application.HasUserAuthorization(UserAuthorization.WebCam);
            }
            else
            {
                return false;
            }
#else

            string permission = ParseString(_type);

            if (string.IsNullOrEmpty(permission) is true)
                return false;

            return Permission.HasUserAuthorizedPermission(permission);
#endif
        }
        public static void RequestPermission(PermissionType _type, Action<PermissionCallbackType> _callback)
        {
#if UNITY_IOS

            if (_type == PermissionType.Microphone)
            {
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
                _callback?.Invoke(Application.HasUserAuthorization(UserAuthorization.Microphone) ?
                    PermissionCallbackType.PermissionGranted :
                    PermissionCallbackType.PermissionDenied);
            }
            else if (_type == PermissionType.Camera)
            {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
                _callback?.Invoke(Application.HasUserAuthorization(UserAuthorization.WebCam) ?
                    PermissionCallbackType.PermissionGranted :
                    PermissionCallbackType.PermissionDenied);
            }
#else
            RequestPermission(_type,
                (granted) => { _callback(PermissionCallbackType.PermissionGranted); },
                (denied) => { _callback(PermissionCallbackType.PermissionDenied); },
                (dontAsk) => { _callback(PermissionCallbackType.PermissionDenied); });

#endif
        }

        public static void RequestPermissions(PermissionType[] _type, Action<PermissionCallbackType> _callback)
        {
            RequestPermissions(_type,
                (granted) => { _callback(PermissionCallbackType.PermissionGranted); },
                (denied) => { _callback(PermissionCallbackType.PermissionDenied); },
                (dontAsk) => { _callback(PermissionCallbackType.PermissionDenied); });
        }

        private static string ParseString(PermissionType _type)
        {
            switch (_type)
            {
                case PermissionType.Read: return Permission.ExternalStorageRead;
                case PermissionType.Write: return Permission.ExternalStorageWrite;
                case PermissionType.Camera: return Permission.Camera;
                case PermissionType.Microphone: return Permission.Microphone;
                case PermissionType.FineLocation: return Permission.FineLocation;
                case PermissionType.CoarseLocation: return Permission.CoarseLocation;

                default: return string.Empty;
            }
        }

        private static void RequestPermission(PermissionType _type, Action<string> _granted, Action<string> _denied, Action<string> _dontAsk)
        {
            PermissionCallbacks callback = new PermissionCallbacks();
            callback.PermissionGranted += _granted;
            callback.PermissionDenied += _denied;
            callback.PermissionDeniedAndDontAskAgain += _dontAsk;

            Permission.RequestUserPermission(ParseString(_type), callback);
        }

        private static void RequestPermissions(PermissionType[] _type, Action<string> _granted, Action<string> _denied, Action<string> _dontAsk)
        {
            PermissionCallbacks callback = new PermissionCallbacks();
            callback.PermissionGranted += _granted;
            callback.PermissionDenied += _denied;
            callback.PermissionDeniedAndDontAskAgain += _dontAsk;

            string[] type = new string[_type.Length];

            for (int i = 0; i < type.Length; i++)
            {
                type[i] = ParseString(_type[i]);
            }

            Permission.RequestUserPermissions(type, callback);
        }

        public static int ParseNativePluginPermissionValue(PermissionCallbackType _type)
        {
            switch (_type)
            {
                case PermissionCallbackType.PermissionGranted:
                    return 1;

                case PermissionCallbackType.PermissionDenied:
                    return 0;

                case PermissionCallbackType.PermissionDeniedAndDontAskAgain:
                    return 2;
            }

            return -1;
        }

        public static void GoSetting()
        {
            try
            {
                OepnAppSettings.GoSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}