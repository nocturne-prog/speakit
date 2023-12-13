using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using UnityEngine;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIpopup_MyEpisodeList : UIPopup
    {
        public MyEpisodeListItem obj_base;
        public ScrollRectEx scroll;
        private List<MyEpisodeListItem> list = new List<MyEpisodeListItem>();
        public GameObject obj_loading;
        public GameObject obj_noContents;

        public override void Awake()
        {
            base.Awake();
            obj_base.gameObject.SetActive(false);
            obj_loading.SetActive(false);
        }

        public override void UpdateData(object[] _args)
        {
            scroll.ResetPos();
            UIManager.s.Loading = true;

            NetworkManager.s.GetMyEpisodesResult((result) =>
            {
                var task = new M_Task(Co_Init(result));
                task.Finished += (m) =>
                {
                    obj_noContents.SetActive(result.total_count == 0);

                    UIManager.s.Loading = false;
                    Show();
                };
            }, (fail) =>
            {
                UIManager.s.Loading = false;
                UIManager.s.OpenErrorPopup();
            });
        }

        private IEnumerator Co_Init(Protocol.MyEpisodesResult _data)
        {
            bool isDone = false;
            var task = new M_Task(Co_Instantiate(_data.data));
            task.Finished += (m) =>
            {
                isDone = true;
            };

            yield return new WaitWhile(() => isDone is false);
        }

        private IEnumerator Co_Instantiate(Protocol.MyEpisodeResultData[] _data)
        {
            int instantiateCount = _data.Length - list.Count;

            if (instantiateCount > 0)
            {
                int listCount = list.Count;

                for (int i = 0; i < instantiateCount + listCount; i++)
                {
                    if (i >= listCount)
                    {
                        var data = _data[i];
                        var item = Instantiate(obj_base, scroll.content);
                        list.Add(item);

                        item.UpdateData(data);
                        yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        list[i].UpdateData(_data[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (i >= _data.Length)
                    {
                        list[i].Hide();
                    }
                    else
                    {
                        list[i].UpdateData(_data[i]);
                    }
                }
            }
        }
    }
}


