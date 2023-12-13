using System;
using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using UnityEngine;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIPopup_Home : UIPopup
    {
        public HomeSection_Episode homeSelection_episode;
        public HomeSection_Topic homeSelection_Topic;
        private List<HomeSection_Topic> homeSelectionTopicList = new List<HomeSection_Topic>();
        public ScrollRectEx scroll;

        private bool isInit = false;

        public override void Awake()
        {
            base.Awake();
            homeSelection_Topic.gameObject.SetActive(false);
        }

        private bool awaitQuit = false;
        public override void Escape()
        {
            if (awaitQuit is true)
            {
                GameManager.s.QuitApplication();
            }
            else
            {
                AndroidToast.s.ShowToastMessage(Const.TOAST_BACK);
                awaitQuit = true;
                Invoke(nameof(EscapeTimer), 3f);
            }
        }

        private void EscapeTimer()
        {
            awaitQuit = false;
        }

        public override void Show()
        {
            base.Show();

            if(DC.Data_Selected_SubCategoryId > 0)
            {
                UIManager.s.OpenPopup<UIPopup_EpisodeList>(true, DC.Data_Selected_SubCategoryId);
            }
        }

        public void Refresh()
        {
            UIManager.s.Loading = true;

            NetworkManager.s.GetHome((result) =>
            {
                scroll.ResetPos();
                homeSelection_episode.Refresh();

                foreach (var v in homeSelectionTopicList)
                    v.Refresh();

                UIManager.s.Loading = false;
            }, (fail) =>
            {
                UIManager.s.Loading = false;
                UIManager.s.OpenErrorPopup();
            });
        }

        public override void UpdateData(params object[] _args)
        {
            if (isInit is true)
            {
                Show();
                Refresh();
                UIManager.s.BottomBar = true;
                UIManager.s.ClosePopup<UIPopup_Onboarding>();
                UIManager.s.ClosePopup<UIPopup_Splash>();
                return;
            }

            UIManager.s.Loading = true;

            NetworkManager.s.GetHome((result) =>
            {
                var task = new M_Task(Co_Init(result));
                task.Finished += (m) =>
                {
                    UIManager.s.Loading = false;
                    Show();
                    UIManager.s.BottomBar = true;
                    UIManager.s.ClosePopup<UIPopup_Onboarding>();
                    UIManager.s.ClosePopup<UIPopup_Splash>();

                    isInit = true;
                };

            }, (fail) =>
            {
                UIManager.s.Loading = false;
                UIManager.s.OpenErrorPopup();
            });
        }

        private IEnumerator Co_Init(Protocol.Home _data)
        {
            //HomeSelection_Episode.
            bool isInit = false;
            homeSelection_episode.Init(_data.month_open_episode_list, () =>
            {
                isInit = true;
                Debug.Log("HomeSelection Init Done");
            });

            yield return new WaitWhile(() => isInit is false);
            yield return new WaitForSeconds(0.1f);
            isInit = false;

            //HomeSelection_Topic_Open.
            var openMainCategories = _data.open_main_category;
            var task = new M_Task(Co_Instantiate(openMainCategories));
            task.Finished += (m) =>
            {
                isInit = true;
                Debug.Log("OpenMainCategories Init Done");
            };

            yield return new WaitWhile(() => isInit is false);
            yield return new WaitForSeconds(0.1f);
            isInit = false;

            //HomeSelection_Topic_Coming.
            var comingMainCategories = _data.coming_main_category;
            task = new M_Task(Co_Instantiate(comingMainCategories, true));
            task.Finished += (m) =>
            {
                isInit = true;
                Debug.Log("ComingMainCategories Init Done");
            };

            yield return new WaitWhile(() => isInit is false);
        }

        private IEnumerator Co_Instantiate(Protocol.MainCategory[] _data, bool _isComming = false)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                bool initDone = false;

                var data = _data[i];
                var item = Instantiate(homeSelection_Topic, scroll.content);

                item.Init(data, _isComming, () =>
                {
                    item.gameObject.SetActive(true);
                    initDone = true;
                });

                homeSelectionTopicList.Add(item);

                yield return new WaitWhile(() => initDone is false);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}

