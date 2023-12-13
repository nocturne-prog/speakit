using System;
using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class MyEpisodeListItem : MonoBehaviour
    {
        public RawImage image_Thumbnail;
        public TextMeshProUGUI text_title_en;
        public TextMeshProUGUI text_title_kr;
        public TextMeshProUGUI text_date;
        public GameObject obj_tag_done;
        public TextMeshProUGUI text_tag_done;
        public GameObject obj_tag_inprogress;

        private Protocol.MyEpisodeResultData data;
        private Button button;

        private void Start()
        {
            button = transform.GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            if (data.progress is Protocol.ProgressType.COMPLETED)
            {
                UIManager.s.OpenReplayModal(PlayEpisode);
            }
            else
            {
                PlayEpisode();
            }
        }

        private void PlayEpisode()
        {
            DC.Data_Selected_SubCategoryId = 0;
            GameManager.s.StartEpisode(data.id);
        }

        public void UpdateData(Protocol.MyEpisodeResultData _data)
        {
            data = _data;

            FileLoader.s.GetImage(data.thumbnail_url, (textre) =>
            {
                image_Thumbnail.texture = textre;
            });

            text_title_en.text = data.title;
            text_title_kr.text = data.description;

            if (data.modified_at is not null)
            {
                DateTime date = Convert.ToDateTime(data.modified_at);
                text_date.text = date.ToString("yyyy-MM-dd HH:mm");
            }

            text_date.SetActive(data.modified_at is not null);

            obj_tag_done.SetActive(data.progress == Protocol.ProgressType.COMPLETED);
            obj_tag_inprogress.SetActive(data.progress == Protocol.ProgressType.IN_PROGRESS);

            text_tag_done.text = data.result.Replace("_", " ");

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}


