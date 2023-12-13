using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Marvrus.UI
{
    public enum UIPopup_Tweener_Kind
    {
        Vertical = 0,
        Horizontal,


        FadeInOut = 101,
    }

    public class UIPopup_Tweener : MonoBehaviour
    {
        RectTransform rectTransform;

        public UIPopup_Tweener_Kind moveType;
        public Ease easeType;
        public float duration;
        public Vector2 from;
        public CanvasGroup canvasGroup;

        private void Awake()
        {
            rectTransform = transform.GetComponent<RectTransform>();

            if ((int)moveType > 100)
            {
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.GetComponent<CanvasGroup>();

                    if (canvasGroup == null)
                    {
                        Debug.LogError("Canvas Group 을 찾을 수 없습니다.");
                        return;
                    }
                }

                canvasGroup.alpha = 0;
            }
            else
            {
                rectTransform.localPosition = from;
            }
        }

        public Tween TweenAction(bool show)
        {
            switch (moveType)
            {
                case UIPopup_Tweener_Kind.Vertical:
                    return rectTransform.DOAnchorPosY(
                        show ? 0 : from.y,
                        duration).SetEase(easeType);

                case UIPopup_Tweener_Kind.Horizontal:
                    return rectTransform.DOAnchorPosX(
                        show ? 0 : from.x,
                        duration).SetEase(easeType);

                case UIPopup_Tweener_Kind.FadeInOut:
                    return canvasGroup.DOFade(
                        show ? 1 : 0,
                        duration).SetEase(easeType);

                default:
                    return null;
            }
        }

        public void Show(Action _callback = null)
        {
            Debug.Log($"Show :: {gameObject.name}");
            TweenAction(true).onComplete += () => _callback?.Invoke();
        }

        public void Hide(Action _callback = null)
        {
            Debug.Log($"Hide :: {gameObject.name}");
            TweenAction(false).onComplete += () => _callback?.Invoke();
        }

    }
}