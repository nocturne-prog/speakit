using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIBottomBar : MonoBehaviour
    {
        public enum State
        {
            Home = 0,
            Setting
        }

        private State state = State.Home;
        public Color color_normal;
        public Color color_selected;

        public List<Button> buttons = new();
        public List<Image> icons = new();
        public List<TextMeshProUGUI> texts = new();

        private void Start()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                int idx = i;

                buttons[idx].onClick.AddListener( () =>
                {
                    OnClickButton((State)idx);
                });
            }

            Refresh();
        }

        private void Refresh()
        {
            int value = (int)state;

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].interactable = value != i;
                icons[i].color = value == i ? color_selected : color_normal;
                texts[i].color = value == i ? color_selected : color_normal;
            }
        }

        private void OnClickButton(State _state)
        {
            ChangeStateVisual(_state);
            ChangeState();
        }

        public bool GetState()
        {
            return gameObject.activeInHierarchy;
        }

        private void ChangeState()
        {
            if (state == State.Setting)
            {
                UIManager.s.OpenPopup<UIPopup_Settings>(_addtive: true);
            }
            else
            {
                UIManager.s.ClosePopup<UIPopup_Settings>();
            }
        }

        public void ChangeStateVisual(State _state)
        {
            state = _state;
            Refresh();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}


