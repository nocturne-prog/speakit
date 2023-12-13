using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class CardEpisode : MonoBehaviour
    {
        public RawImage img_Thumbnail;
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_SubTitle;
        public Button button;
        public GameObject obj_play;
        public GameObject obj_check;
        public GameObject obj_tag;
        public TextMeshProUGUI text_tag;

        private Protocol.MonthOpenEpisode data;

        private void Start()
        {
            button.onClick.AddListener(OnClickPlay);
        }

        public void UpdateData(Protocol.MonthOpenEpisode _data)
        {
            data = _data;

            FileLoader.s.GetImage(data.thumbnail_url, (texture2D) =>
            {
                img_Thumbnail.texture = texture2D;
            }, (error) =>
            {
                Debug.LogError($"{data.thumbnail_url} image load error");
            });

            text_Title.text = data.title;
            text_SubTitle.text = data.description;

            obj_tag.SetActive(data.trial is true);
            obj_play.SetActive(data.completed is false);
            obj_check.SetActive(data.completed is true);

            gameObject.SetActive(true);
        }

        private void OnClickPlay()
        {
            if (data.completed is true)
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
    }
}

