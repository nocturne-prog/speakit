using System;
using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using UnityEngine;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class HomeSection_Episode : MonoBehaviour
    {
        public CardEpisode base_item;
        public ScrollRectEx scroll;
        private List<CardEpisode> itemList = new List<CardEpisode>();

        private void Awake()
        {
            base_item.gameObject.SetActive(false);
        }

        public void Init(Protocol.MonthOpenEpisode[] _data, Action _callback = null)
        {
            var task = new M_Task(Co_Instantiate(_data));

            task.Finished += (m) =>
            {
                _callback?.Invoke();
            };
        }

        public void Refresh()
        {
            scroll.ResetPos();

            var data = DC.Data_Home.month_open_episode_list;

            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].UpdateData(data[i]);
            }
        }

        private IEnumerator Co_Instantiate(Protocol.MonthOpenEpisode[] _data)
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