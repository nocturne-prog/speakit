using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Marvrus.Util
{
    public static class PlayerPrefsManager
    {

        public enum Key
        {
            User_id,
            Client_Id,
            RefreshToken,
            Check_Permission,
        }


        /// <summary>
        /// PlayerPrefs.Clear 할 때, 예외되는 키 값.
        /// </summary>
        public enum Key_Not_Deleted
        {
            PermissionCheck,
            Onboarding,
        }

        #region Base.
        public static string GetPlayerPrefs(Key _key, string _defaultValue = null)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static string GetPlayerPrefs(Key_Not_Deleted _key, string _defaultValue = null)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static int GetPlayerPrefs(Key _key, int _defaultValue = 0)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static int GetPlayerPrefs(Key_Not_Deleted _key, int _defaultValue = 0)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static float GetPlayerPrefs(Key _key, float _defaultValue = 0)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static float GetPlayerPrefs(Key_Not_Deleted _key, float _defaultValue = 0)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static string GetPlayerPrefs(string _key, string _defaultValue = null)
        {
            if (string.IsNullOrEmpty(_defaultValue))
                return PlayerPrefs.GetString(_key);

            return PlayerPrefs.GetString(_key, _defaultValue);
        }

        public static int GetPlayerPrefs(string _key, int _defaultValue = 0)
        {
            if (_defaultValue == 0)
                return PlayerPrefs.GetInt(_key);

            return PlayerPrefs.GetInt(_key, _defaultValue);
        }

        public static float GetPlayerPrefs(string _key, float _defaultValue = 0)
        {
            if (_defaultValue == 0)
                return PlayerPrefs.GetFloat(_key);

            return PlayerPrefs.GetFloat(_key, _defaultValue);
        }

        public static void SetPlayerPrefs(Key _key, string _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key_Not_Deleted _key, string _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key _key, int _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key_Not_Deleted _key, int _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key _key, float _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key_Not_Deleted _key, float _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(string _key, string _value)
        {
            PlayerPrefs.SetString(_key, _value);
            PlayerPrefs.Save();
        }

        public static void SetPlayerPrefs(string _key, int _value)
        {
            PlayerPrefs.SetInt(_key, _value);
            PlayerPrefs.Save();
        }

        public static void SetPlayerPrefs(string _key, float _value)
        {
            PlayerPrefs.SetFloat(_key, _value);
            PlayerPrefs.Save();
        }

        public static bool HasKey(string _key)
        {
            return PlayerPrefs.HasKey(_key);
        }

        public static void DeleteKey(string _key)
        {
            PlayerPrefs.DeleteKey(_key);
        }

        public static void Clear()
        {
            foreach(Key v in Enum.GetValues(typeof(Key)))
            {
                DeleteKey(v.ToString());
            }
        }
        #endregion

        public static string RefreshToken
        {
            get { return GetPlayerPrefs(Key.RefreshToken, ""); }
            set { SetPlayerPrefs(Key.RefreshToken, value); }
        }

        public static string UserId
        {
            get { return GetPlayerPrefs(Key.User_id, ""); }
            set { SetPlayerPrefs(Key.User_id, value); }
        }

        public static string ClientID
        {
            get
            {
                string id = GetPlayerPrefs(Key.Client_Id, "");

                if (string.IsNullOrEmpty(id))
                {
                    id = CreateClientID(30);
                    SetPlayerPrefs(Key.Client_Id, id);
                }

                return id;
            }
        }

        private static string CreateClientID(int _length)
        {
            using (var crypto = new RNGCryptoServiceProvider())
            {
                var bits = (_length * 6);
                var byte_size = ((bits + 7) / 8);
                var bytesarray = new byte[byte_size];
                crypto.GetBytes(bytesarray);

                string random = Convert.ToBase64String(bytesarray);
                Debug.Log($"random clientID : {random}");

                random = random.Replace("=", "");
                return random;
            }
        }

        public static bool CheckPermission
        {
            get { return GetPlayerPrefs(Key.Check_Permission, 0) == 1; }
            set { SetPlayerPrefs(Key.Check_Permission, value ? 1 : 0); }
        }
    }
}