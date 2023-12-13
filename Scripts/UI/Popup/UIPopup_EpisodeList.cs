using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_EpisodeList : UIPopup
    {
        public ScrollRectEx scroll;

        [Header("Topic Main Section")]
        public RawImage image_Thumbnail;
        public TextMeshProUGUI text_title;
        public TextMeshProUGUI text_desc;

        [Header("Topic Section")]
        public GameObject obj_topic_parent;
        private List<CardEpisodeSmall> topicCardList = new();

        [Header("Topic Comming")]
        public GameObject obj_comming_parent;
        private List<CardEpisodeSmall> commingCardList = new();

        [Header("Card Base")]
        public CardEpisodeSmall obj_base;


        public override void Awake()
        {
            base.Awake();
            obj_base.gameObject.SetActive(false);
        }

        public override void UpdateData(params object[] _args)
        {
            scroll.ResetPos();

            int id = (int)_args[0];

            UIManager.s.Loading = true;

            NetworkManager.s.GetEpisodesBySubCategory(id, (result) =>
            {
                var task = new M_Task(Co_Init(result));
                task.Finished += (m) =>
                {
                    UIManager.s.Loading = false;
                    Show();
                };
            }, (fail) =>
            {
                UIManager.s.Loading = false;
                UIManager.s.OpenErrorPopup();
            });
        }

        private IEnumerator Co_Init(Protocol.EpisodesBySubCategory _data)
        {
            FileLoader.s.GetImage(_data.thumbnail_url, (texture) =>
            {
                image_Thumbnail.texture = texture;
            });

            text_title.text = _data.name;
            text_desc.text = _data.title;

            bool isDone = false;
            var task = new M_Task(Co_Instantiate(_data.open_episodes, obj_topic_parent.transform, topicCardList));
            task.Finished += (m) =>
            {
                isDone = true;
            };

            yield return new WaitWhile(() => isDone is false);
            yield return new WaitForSeconds(0.1f);
            isDone = false;

            obj_comming_parent.SetActive(_data.coming_episodes.Length is not 0);

            task = new M_Task(Co_Instantiate(_data.coming_episodes, obj_comming_parent.transform, commingCardList));
            task.Finished += (m) =>
            {
                isDone = true;
            };

            yield return new WaitWhile(() => isDone is false);
        }

        private IEnumerator Co_Instantiate(Protocol.MonthOpenEpisode[] _data, Transform _target, List<CardEpisodeSmall> _list)
        {
            int instantiateCount = _data.Length - _list.Count;

            if (instantiateCount > 0)
            {
                int listCount = _list.Count;

                for (int i = 0; i < instantiateCount + listCount; i++)
                {
                    if (i >= listCount)
                    {
                        var data = _data[i];
                        var item = Instantiate(obj_base, _target);
                        _list.Add(item);

                        item.UpdateData(data);
                        yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        _list[i].UpdateData(_data[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    if (i >= _data.Length)
                    {
                        _list[i].Hide();
                    }
                    else
                    {
                        _list[i].UpdateData(_data[i]);
                    }
                }
            }
        }
    }
}
