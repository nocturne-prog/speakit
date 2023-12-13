using System;
using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using TMPro;
using UnityEngine;

namespace Marvrus.UI
{
    public class HomeSection_Topic : MonoBehaviour
    {
        public TextMeshProUGUI text_title;
        public GameObject obj_badge;
        public CardTopic base_item;
        public ScrollRectEx scroll;
        private List<CardTopic> itemList = new List<CardTopic>();

        private void Awake()
        {
            base_item.gameObject.SetActive(false);
        }

        public void Refresh()
        {
            scroll.ResetPos();
        }

        public void Init(Protocol.MainCategory _data, bool _isComming = false, Action _callback = null)
        {
            text_title.text = _data.name;
            obj_badge.SetActive(_isComming is true);
            var task = new M_Task(Co_Instantiate(_data.sub_category));
            task.Finished += (m) =>
            {
                _callback?.Invoke();
            };
        }

        private IEnumerator Co_Instantiate(Protocol.SubCategory[] _data)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                var data = _data[i];
                var item = Instantiate(base_item, scroll.content);
                item.UpdateData(data);
                itemList.Add(item);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
