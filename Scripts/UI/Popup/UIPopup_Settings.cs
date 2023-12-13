using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;
using OB = Marvrus.Util.OpenBrowser;
using PPM = Marvrus.Util.PlayerPrefsManager;
using DF = Marvrus.Util.Define;

namespace Marvrus.UI
{
    public class UIPopup_Settings : UIPopup
    {
        public ScrollRectEx scroll;

        [Header("Profile")]
        public RawImage rawImage_profile;
        public TextMeshProUGUI text_id;
        public GameObject obj_member;
        public Button button_guest;

        [Header("My Info")]
        public GameObject obj_myInfo;
        public Button button_epdisodeFeed;
        public Button button_drawal;
        public Button button_MyInfo;
        public Button button_logout;

        [Header("Service Info")]
        public Button button_FAQ;
        public Button button_BusinessInfo;
        public Button button_TermsOfService;
        public Button button_PrivacyPolicy;

        [Header("App Version")]
        public TextMeshProUGUI text_appversion;

        public override void Awake()
        {
            base.Awake();

            button_guest.onClick.AddListener(OnClickLogin);

            button_epdisodeFeed.onClick.AddListener(OnClickEpisodeFeed);
            button_drawal.onClick.AddListener(OnClickDrawal);
            button_MyInfo.onClick.AddListener(OnClickMyInfo);
            button_logout.onClick.AddListener(OnClickLogOut);

            button_FAQ.onClick.AddListener(OnClickFAQ);
            button_BusinessInfo.onClick.AddListener(OnClickBusinessInfo);
            button_TermsOfService.onClick.AddListener(OnClickTermsOfService);
            button_PrivacyPolicy.onClick.AddListener(OnClickPrivacyPolicy);
        }

        public override void UpdateData(params object[] _args)
        {
            Refresh();
            base.UpdateData(_args);
        }

        public void Refresh()
        {
            if (DC.IsLogin is true)
            {
                var data = DC.Data_MyInfo;
                text_id.text = data.user_id;
            }

            button_guest.SetActive(DC.IsLogin is false);
            obj_member.SetActive(DC.IsLogin is true);
            obj_myInfo.SetActive(DC.IsLogin is true);

            scroll.ResetPos();

            text_appversion.text = $"{Application.version}";

            if (DC.Data_VersionInfo is not null)
            {
                if (DC.Data_VersionInfo.maximum_version.Equals(Application.version))
                {
                    text_appversion.text = text_appversion.text += $" {Const.VERSION_MAX}";
                }
            }
        }

        public override void Hide()
        {
            base.Hide();
            UIManager.s.GetBottomBar.ChangeStateVisual(UIBottomBar.State.Home);
        }

        private void OnClickLogin()
        {
            UIManager.s.OpenPopup<UIPopup_ActionSheet>(_addtive: true);
        }

        private void OnClickEpisodeFeed()
        {
            UIManager.s.OpenPopup<UIpopup_MyEpisodeList>(_addtive: true);
        }

        private void OnClickDrawal()
        {
            OB.WithDrawal((result) =>
            {
                if (result.Equals(OB.HostType.profile_withdrawal.ToString()))
                {
                    LogOutProcess();
                }
            });
        }

        private void OnClickMyInfo()
        {
            OB.MyProfile((result) =>
            {
            });
        }

        private void OnClickLogOut()
        {
            UIManager.s.OpenTwoButton(
                    _title: Const.UI_LOGOUT_TITLE,
                    _desc: Const.UI_LOGOUT_DESC,
                    _okText: Const.UI_LOGOUT,
                    _cancelText: Const.UI_CANCEL,
                    _ok: () =>
                    {
                        if (DF.platform == DF.Platform.UnityEditor)
                        {
                            Debug.Log($"GameManager Inspector 에서 로그아웃 ");
                        }
                        else
                        {
                            OB.LogOut((result) =>
                            {
                                if (result.Equals(OB.HostType.Logout.ToString()))
                                {
                                    LogOutProcess();
                                }
                            });
                        }
                    });
        }

        private void OnClickFAQ()
        {
            UIManager.s.OpenPopup<UIPopup_FAQ>(_addtive: true);
        }

        private void OnClickBusinessInfo()
        {
            OB.OpenUrl("*****");
        }

        private void OnClickTermsOfService()
        {
            OB.OpenUrl("*****");
        }

        private void OnClickPrivacyPolicy()
        {
            OB.OpenUrl("*****");
        }

        private void LogOutProcess()
        {
            PPM.Clear();
            DC.Clear();
            UIManager.s.OpenPopup<UIPopup_Splash>();
        }
    }
}


