using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_Modal : UIPopup
    {
        public enum BUTTON_TYPE
        {
            ONE,
            TWO
        }

        public TextMeshProUGUI text_title;
        public TextMeshProUGUI text_desc;

        public Button button_secondary;
        public TextMeshProUGUI text_secondary;
        public Button button_primary;
        public TextMeshProUGUI text_primary;

        private Action callback_ok;
        private Action callback_cancel;

        public override void Awake()
        {
            base.Awake();

            button_primary.onClick.AddListener(() =>
            {
                callback_ok?.Invoke();
                Hide();
            });

            button_secondary.onClick.AddListener(() =>
            {
                callback_cancel?.Invoke();
                Hide();
            });
        }

        public override void UpdateData(params object[] _args)
        {
            UIManager.s.Dim = true;

            BUTTON_TYPE type = (BUTTON_TYPE)_args[0];
            string title = _args[1].ToString();
            string desc = _args[2].ToString();

            callback_ok = (Action)_args[3];
            text_primary.text = _args[4].ToString();

            if (type == BUTTON_TYPE.TWO)
            {
                callback_cancel = (Action)_args[5];
                text_secondary.text = _args[6].ToString();
            }

            button_secondary.SetActive(type == BUTTON_TYPE.TWO);

            text_title.text = title;
            text_desc.text = desc;

            Show();
        }

        public override void Hide()
        {
            callback_ok = null;
            callback_cancel = null;

            UIManager.s.Dim = false;
            base.Hide();
        }
    }
}


