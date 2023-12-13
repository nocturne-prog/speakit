using System.Net;
using UnityEngine;
using UnityEngine.UI;
using OB = Marvrus.Util.OpenBrowser;
using PPM = Marvrus.Util.PlayerPrefsManager;
using DF = Marvrus.Util.Define;

namespace Marvrus.UI
{
    public class UIPopup_ActionSheet : UIPopup
    {
        public Button button_Login;

        public override void Awake()
        {
            base.Awake();

            button_Login.onClick.AddListener(OnClick);
        }

        public override void UpdateData(params object[] _args)
        {
            UIManager.s.Dim = true;
            Show();
        }

        public override void Hide()
        {
            UIManager.s.Dim = false;
            base.Hide();
        }

        private void OnClick()
        {
            if (DF.platform == DF.Platform.UnityEditor)
            {
                Debug.Log($"GameManager Inspector 에서 로그인 ");
            }
            else
            {
                OB.SignIn((result) =>
                {
                    string[] parameters = result.Split("&");
                    string userId = parameters[0].Split("=")[1];
                    string refreshToken = parameters[1].Split("=")[1];

                    userId = WebUtility.UrlDecode(userId);
                    refreshToken = WebUtility.UrlDecode(refreshToken);

                    PPM.UserId = userId;
                    PPM.RefreshToken = refreshToken;

                    Protocol.AutoLoginRequest data = new()
                    {
                        id = PPM.UserId,
                        refreshToken = PPM.RefreshToken,
                        service = Const.SERVICE,
                        clientId = PPM.ClientID
                    };

                    NetworkManager.s.AutoLogin(data, (result) =>
                    {
                        var bar = UIManager.s.GetBottomBar;

                        if (bar.GetState() is true)
                        {
                            UIManager.s.FindPopup<UIPopup_Settings>().Refresh();
                            UIManager.s.FindPopup<UIPopup_Home>().Refresh();
                        }

                        Hide();

                    }, (error) =>
                    {
                    });
                });
            }
        }
    }
}


