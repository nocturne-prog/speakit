using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class CardEpisodeSmall : MonoBehaviour
    {
        public RawImage img_thumbnail;
        private Button button;

        public GameObject obj_badge;
        public GameObject obj_play;
        public GameObject obj_check;
        public GameObject obj_lock;

        public TextMeshProUGUI text_title;
        public TextMeshProUGUI text_desc;

        private Protocol.MonthOpenEpisode data;

        enum STATE
        {
            Play,
            Check,
            Lock
        }

        private STATE state = STATE.Play;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        public void UpdateData(Protocol.MonthOpenEpisode _data)
        {
            data = _data;

            FileLoader.s.GetImage(data.thumbnail_horiz_url, (texture) =>
            {
                img_thumbnail.texture = texture;
            }, (error) =>
            {
                Debug.Log($"Load Image Error :: {data.thumbnail_url}");
            });

            obj_badge.SetActive(data.trial);

            if (data.activated is Protocol.ActivateType.COMING_SOON)
            {
                state = STATE.Lock;
            }
            else
            {
                state = data.completed is true ? STATE.Check : STATE.Play;
            }

            obj_play.SetActive(state is STATE.Play);
            obj_check.SetActive(state is STATE.Check);
            obj_lock.SetActive(state is STATE.Lock);

            text_title.text = data.title;
            text_desc.text = data.description;

            Show();
        }

        public void OnClick()
        {
            if (state is STATE.Play)
            {
                PlayEpisode();
            }
            else if (state is STATE.Check)
            {
                UIManager.s.OpenReplayModal(PlayEpisode);
            }
            else
            {
                UIManager.s.OpenOneButton(
                    _title: Const.UI_COMMING_TITLE,
                    _desc: Const.UI_COMMING_DESC,
                    _ok: null,
                    _okText: Const.UI_OK);
            }
        }

        private void PlayEpisode()
        {
            GameManager.s.StartEpisode(data.id);
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


