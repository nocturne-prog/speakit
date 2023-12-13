
using Marvrus.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class CardTopic : MonoBehaviour
    {
        public Button button;
        public RawImage img_Thumbnail;
        public TextMeshProUGUI text_title;
        public TextMeshProUGUI text_desc;

        private int id = 0;

        private void Start()
        {
            button.onClick.AddListener(() =>
            {
                DC.Data_Selected_SubCategoryId = id;
                UIManager.s.OpenPopup<UIPopup_EpisodeList>(_addtive: true, _args: id);
            });
        }

        public void UpdateData(Protocol.SubCategory _data)
        {
            id = _data.id;

            FileLoader.s.GetImage(_data.thumbnail_url, (texture2D) =>
            {
                img_Thumbnail.texture = texture2D;
            }, (error) =>
            {
                Debug.LogError($"{_data.thumbnail_url} image load error");
            });

            text_title.text = _data.name;
            text_desc.text = _data.title;

            gameObject.SetActive(true);
        }
    }
}


