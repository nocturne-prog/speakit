using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Marvrus.UI;
using Marvrus.Util;
using System.Text;
using UnityEngine.UI;

namespace Marvrus
{
    public enum eMenuType
    {
        NONE,
        BOTTOM
    }

    public class UIManager : Singleton<UIManager>
    {
        Transform backCanvasParent;
        Transform appCavnasPrents;
        Transform frontCanvasParent;
        Transform popupCanvasParent;
        Transform episodeCanvasParent;
        Transform episodeModalCanvasParent;

        UIBottomBar bottomBar;

        GameObject dim;
        GameObject loading;

        Dictionary<string, PopupStatus> popupDic = new();

        public class PopupStatus
        {
            public UIPopup popup;
            public int layer;
            public Transform parents;

#if UNITY_EDITOR
            public override string ToString()
            {
                return $"{popup.gameObject.name}, layer: {layer}";
            }
#endif
        }

        public bool BottomBar
        {
            get { return bottomBar.GetState(); }
            set
            {
                if (value)
                {
                    bottomBar.Show();
                }
                else
                {
                    bottomBar.Hide();
                }
            }
        }

        public UIBottomBar GetBottomBar => bottomBar;

        public bool Dim
        {
            set { dim.SetActive(value); }
        }

        public bool Loading
        {
            get
            {
                return loading.activeInHierarchy;
            }
            set
            {
                loading.SetActive(value);
            }
        }

        public void InitSafeArea()
        {
            SafeArea[] safeAreas = FindObjectsOfType<SafeArea>();

            SafeAreaCaculator.Init(safeAreas[0].scaler);
            foreach (var v in safeAreas)
                v.ApplySafeArea();
        }

        public void Init()
        {
            Transform canvasTrf = GameObject.Find("Canvas").transform;

            backCanvasParent = canvasTrf.Find("BackCanvas/SafeArea");
            appCavnasPrents = canvasTrf.Find("AppbarCanvas/SafeArea");
            frontCanvasParent = canvasTrf.Find("FrontCanvas/SafeArea");
            popupCanvasParent = canvasTrf.Find("PopupCanvas/SafeArea");
            episodeCanvasParent = canvasTrf.Find("EpisodeCanvas/SafeArea");
            episodeModalCanvasParent = canvasTrf.Find("EpisodeCanvas/SafeArea_Modal");


            if (backCanvasParent is null)
            {
                Debug.LogError("BackCanvas is null");
                return;
            }

            if (frontCanvasParent is null)
            {
                Debug.LogError("FrontCanvas is null");
                return;
            }

            if (popupCanvasParent is null)
            {
                Debug.LogError("PopupCanvas is null");
                return;
            }

            Transform[] canvas = new Transform[] {
                backCanvasParent,
                frontCanvasParent,
                popupCanvasParent,
                episodeCanvasParent
            };

            for (int i = 0; i < canvas.Length; i++)
            {
                for (int m = 0; m < canvas[i].childCount; m++)
                {
                    Transform trf = canvas[i].transform.GetChild(m);
                    var popup = trf.GetComponent<UIPopup>();

                    if (popup != null)
                    {
                        popupDic.Add(popup.GetType().ToString(), new PopupStatus()
                        {
                            popup = popup,
                            layer = 0,
                            parents = canvas[i],
                        });
                    }
                }
            }

            dim = canvasTrf.Find("PopupCanvas/SafeArea").Find("Dim").gameObject;
            loading = canvasTrf.Find("PopupCanvas/SafeArea_Loading").Find("Loading").gameObject;
            bottomBar = appCavnasPrents.Find("BottomAppBar").gameObject.GetComponent<UIBottomBar>();
        }

        #region UIPopup.
        private T OpenPopup<T>(bool _addtive = false) where T : UIPopup
        {
            Debug.LogWarning($"UIManager :: OpenPopup {typeof(T)}");

            var value = FindValue(typeof(T).ToString());

            if (_addtive is false)
            {
                CloseAll(value);
            }

            if (value != null)
            {
                value.layer = MaxLayerValue + 1;
                value.popup.transform.SetAsLastSibling();

                return (T)value.popup;
            }
            else
            {
                Debug.LogError($"UIManager :: OpenPopup.{typeof(T)}");
            }

            return null;
        }

        public T OpenPopup<T>(bool _addtive = false, params object[] _args) where T : UIPopup
        {
            var popup = OpenPopup<T>(_addtive);
            popup.UpdateData(_args);

            return popup;
        }
        public T SetEpisodePopup<T>(object _args, bool _addtive = false) where T : UIPopup
        {
            var popup = OpenPopup<T>(_addtive);
            popup.UpdateData(_args);

            return popup;
        }
        public void ShowPopup<T>() where T : UIPopup
        {
            var popup = FindPopup<T>();
            popup.Show();
        }

        public T ShowPopup<T>(bool _addtive = false, params object[] _args) where T : UIPopup
        {
            var popup = OpenPopup<T>(_addtive);
            return popup;
        }

        private PopupStatus FindValue(string _key)
        {
            var value = popupDic.FirstOrDefault(x => x.Key.Equals(_key)).Value;

            if (value is null)
            {
                Debug.LogError($"UIManager :: can't not find popup {_key}");
                return null;
            }
            else
            {
                return value;
            }
        }

        public PopupStatus FindValue<T>() where T : UIPopup
        {
            Type t = typeof(T);
            string key = t.ToString();

            return FindValue(key);
        }

        private PopupStatus FindValue(UIPopup _popup)
        {
            string key = _popup.GetType().ToString();
            return FindValue(key);
        }

        public T FindPopup<T>() where T : UIPopup
        {
            var value = FindValue<T>();
            return (T)value.popup;
        }

        public PopupStatus FindFirstPopup()
        {
            PopupStatus value = popupDic.OrderByDescending(x => x.Value.layer).FirstOrDefault().Value;
            return value;
        }

        public int MaxLayerValue => popupDic.Max(x => x.Value.layer);

        public void ClosePopup<T>() where T : UIPopup
        {
            var popup = FindPopup<T>();
            popup.Hide();
        }

        public void ClosePopup(UIPopup popup)
        {
            var value = FindValue(popup);
            value.layer = -1;

            var last = popupDic.OrderByDescending(x => x.Value.layer).FirstOrDefault();
        }

        public void CloseAll(PopupStatus _except = null)
        {
            foreach (var v in popupDic.Values)
            {
                if (_except != null)
                {
                    if (v == _except)
                        continue;
                }

                if (v.popup.gameObject.activeInHierarchy is false)
                    continue;

                v.popup.Hide();
            }
        }
        #endregion

        #region Common
        public void OpenOneButton(string _title, string _desc, Action _ok, string _okText = null)
        {
            object[] data = new object[] {
                UIPopup_Modal.BUTTON_TYPE.ONE,
                _title,
                _desc,
                _ok,
                _okText
            };

            OpenPopup<UIPopup_Modal>(_addtive: true, data);
        }

        public void OpenTwoButton(string _title, string _desc, Action _ok = null, string _okText = null, Action _cancel = null, string _cancelText = null)
        {
            object[] data = new object[] {
                UIPopup_Modal.BUTTON_TYPE.TWO,
                _title,
                _desc,
                _ok,
                _okText,
                _cancel,
                _cancelText
            };

            OpenPopup<UIPopup_Modal>(_addtive: true, data);
        }
        #endregion

        #region Util.
        public void EnterEpisode()
        {
            BottomBar = false;

            ChangeParent(dim.transform, episodeModalCanvasParent);
            ChangeParent(FindPopup<UIPopup_Modal>().transform, episodeModalCanvasParent);
        }

        public void ExitEpisode()
        {
            BottomBar = true;

            ChangeParent(dim.transform, popupCanvasParent);
            ChangeParent(FindPopup<UIPopup_Modal>().transform, popupCanvasParent);

            dim.transform.SetSiblingIndex(0);
        }

        private void ChangeParent(Transform _trf, Transform _parent)
        {
            _trf.SetParent(_parent, true);
            _trf.localPosition = Vector3.zero;
            _trf.localRotation = Quaternion.identity;
            _trf.localScale = Vector3.one;
        }

        public void OpenErrorPopup(string _msg = "")
        {
            OpenPopup<UIPopup_Error>(_addtive: true, _args: _msg);
        }

        public void OpenReplayModal(Action _ok)
        {
            OpenTwoButton(
                _title: Const.UI_EPISODE_COMPLETE_TITLE,
                _desc: Const.UI_EPISODE_COMPLETE_DESC,
                _ok: _ok,
                _okText: Const.Btn_ReplayEpisode,
                _cancelText: Const.Btn_Cancle);
        }
        #endregion

        public void CloseFirstPopup()
        {
            var firstPopup = FindFirstPopup();

            Debug.Log($"Escape :: {firstPopup.popup}");

            if (firstPopup.popup.useEscapeClose is true)
            {
                firstPopup.popup.Escape();
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.A))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in popupDic)
                {
                    sb.AppendLine($"{v}");
                }

                Debug.Log(sb.ToString());
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                OpenPopup<UIPopup_Splash>(false, null);
            }
#endif

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseFirstPopup();
            }
        }
    }
}