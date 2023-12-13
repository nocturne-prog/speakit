using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class FAQItem : MonoBehaviour
    {
        public Button button_question;
        public GameObject obj_answer;
        public GameObject obj_arrow;
        public bool isOpened = false;

        private void Start()
        {
            button_question.onClick.AddListener(OnClickQuestion);
        }

        public void Init()
        {
            if (isOpened is true)
            {
                OnClickQuestion();
            }
        }

        public void OnClickQuestion()
        {
            if (isOpened is true)
            {
                Close();
            }
            else
            {
                Open();
            }

            isOpened = !isOpened;
        }

        public void Open()
        {
            obj_answer.SetActive(true);
            obj_arrow.transform.localScale = new Vector3(1, -1, 1);
        }

        public void Close()
        {
            obj_answer.SetActive(false);
            obj_arrow.transform.localScale = Vector3.one;
        }
    }
}
