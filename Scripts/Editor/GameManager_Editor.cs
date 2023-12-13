using UnityEditor;
using UnityEngine;
using Marvrus;
using OB = Marvrus.Util.OpenBrowser;
using DC = Marvrus.Data.DataContainer;
using PPM = Marvrus.Util.PlayerPrefsManager;

[CustomEditor(typeof(GameManager))]
public class GameManager_Editor : Editor
{
    private string id, password;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            EditorGUILayout.Space(20);

            if (DC.IsLogin)
            {
                if(GUILayout.Button("LogOut"))
                {
                    NetworkManager.s.LogOut((result) =>
                    {
                        PPM.Clear();
                        DC.Clear();
                        Debug.Log("로그아웃되었습니다");
                    }, null);
                }
            }
            else
            {
                id = EditorGUILayout.TextField("ID", id);
                password = EditorGUILayout.PasswordField("PASSWORD", password);

                if (string.IsNullOrEmpty(id) is false && string.IsNullOrEmpty(password) is false)
                {
                    if (GUILayout.Button("Login"))
                    {
                        NetworkManager.s.Login(id, password, (result) =>
                        {
                            Debug.Log("로그인에 성공하였습니다");
                            PPM.UserId = id;
                        }, (fail) =>
                        {
                        });
                    }
                }   
            }

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Sign Up"))
            {
                OB.SignUp((result) =>
                {
                });
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
