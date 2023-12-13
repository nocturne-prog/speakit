using Marvrus.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_Onboarding : UIPopup
    {
        public Button button_start;
        public ScrollSnap scrollSnap;
        public PageIndicator[] indicators;

        public override void Awake()
        {
            base.Awake();

            button_start.onClick.AddListener(() =>
            {
                UIManager.s.OpenPopup<UIPopup_Home>(_addtive: true);
            });

            scrollSnap.OnValueChanged = UpdateIndicators;
            UpdateIndicators(0);
        }

        public override void UpdateData(object _args)
        {
            base.UpdateData(_args);
        }

        private void UpdateIndicators(int _index)
        {
            for (int i = 0; i < indicators.Length; i++)
            {
                indicators[i].IsOn = i == _index;
            }
        }
    }
}

