using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup : MonoBehaviour
    {
        public Button button_Close;
        public bool useEscapeClose = false;

        protected UIPopup_Tweener tweener;
        protected bool useTween = false;

        public virtual void Awake()
        {
            if (button_Close != null)
            {
                button_Close.onClick.RemoveAllListeners();
                button_Close.onClick.AddListener(() => Hide());
            }

            tweener = gameObject.GetComponent<UIPopup_Tweener>();
            useTween = tweener != null;
        }

        public virtual void UpdateData(params object[] _args)
        {
            Show();
        }
        public virtual void UpdateData(object _args)
        {

        }
        public virtual void Hide()
        {
            if (useTween)
            {
                tweener.Hide(() =>
                {
                    HideInvoke();
                });
            }
            else
            {
                HideInvoke();
            }
        }

        private void HideInvoke()
        {
            gameObject.SetActive(false);
            UIManager.s.ClosePopup(this);
        }

        public virtual void Escape()
        {
            Hide();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);

            if (useTween)
            {
                tweener.Show();
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (button_Close != null)
            {
                button_Close.onClick.RemoveAllListeners();
                button_Close = null;
            }

            tweener = null;
        }
    }
}