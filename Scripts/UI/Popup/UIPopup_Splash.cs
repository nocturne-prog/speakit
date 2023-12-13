using PPM = Marvrus.Util.PlayerPrefsManager;
using Marvrus.Util;
using UnityEngine;

namespace Marvrus.UI
{
    public class UIPopup_Splash : UIPopup
    {
        public override void UpdateData(params object[] _args)
        {
            NetworkManager.s.GetVersionInfo(null, null);

            UIManager.s.BottomBar = false;

            base.UpdateData(_args);

            Invoke(nameof(ShowOnboarding), 1.5f);
        }

        private void ShowOnboarding()
        {
            if (string.IsNullOrEmpty(PPM.RefreshToken) is true)
            {
                UIManager.s.OpenPopup<UIPopup_Onboarding>();
            }
            else
            {
                NetworkManager.s.AutoLogin(new Protocol.AutoLoginRequest()
                {
                    id = PPM.UserId,
                    refreshToken = PPM.RefreshToken,
                    service = Const.SERVICE,
                    clientId = PPM.ClientID
                }, (result) =>
                {
                    UIManager.s.OpenPopup<UIPopup_Home>(_addtive: true);
                }, (error) =>
                {
                    UIManager.s.OpenPopup<UIPopup_Onboarding>();
                });
            }
        }
    }
}