using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class BuildTool : EditorWindow
{
    public static string currentAppVersion;
    public static string buildAppVersion;
    public static string currentAppBuildNumber;
    public static string buildNumber;

    [MenuItem("Tools/Open BuildTool %&#_1", false, 0)]
    static void Init()
    {
        currentAppVersion = PlayerSettings.bundleVersion;

#if UNITY_IOS
        currentAppBuildNumber = PlayerSettings.iOS.buildNumber;
#else
        currentAppBuildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
#endif

        buildAppVersion = currentAppVersion;
        buildNumber = currentAppBuildNumber;

        BuildTool window = (BuildTool)GetWindow(typeof(BuildTool));
        window.minSize = new Vector2(430, 600);
        window.maxSize = new Vector2(430, 600);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(20);
        GUILayout.Label(">>>> [ Application Build Part ]");

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"현재 Application 버전: ");
        EditorGUILayout.LabelField($"{currentAppVersion}");
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("빌드 Application 버전: ");
        buildAppVersion = EditorGUILayout.TextField(buildAppVersion);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"현재 BuildNumber: ");
        EditorGUILayout.LabelField($"{currentAppBuildNumber}");
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("빌드 BuildNumber:");
        buildNumber = EditorGUILayout.TextField(buildNumber);
        GUILayout.EndHorizontal();


        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
#if UNITY_IOS

        if (GUILayout.Button("Build iOS", GUILayout.Height(30)))
        {
            ShowDialog("Build iOS", $"Build iOS :: {buildAppVersion}.{buildNumber}", () =>
            {
                BuildiOS();
            }, () =>
            {
                Debug.Log("Cancel");
            });
        }

#else
        if (GUILayout.Button("Build Debug APK", GUILayout.Height(30)))
        {
            ShowDialog("Build APK", $"Build Debug APK :: {buildAppVersion}.{buildNumber}", () =>
            {
                BuildAndroid(false);
            }, () =>
            {
                Debug.Log("Cancel");
            });
        }

        if (GUILayout.Button("Build Release APK", GUILayout.Height(30)))
        {
            ShowDialog("Build APK", $"Build Release APK :: {buildAppVersion}.{buildNumber}", () =>
            {
                BuildAndroid(false);
            }, () =>
            {
                Debug.Log("Cancel");
            });
        }

        if (GUILayout.Button("Build Release AAB", GUILayout.Height(30)))
        {
            ShowDialog("Build AAB", $"Build Release AAB :: {buildAppVersion}.{buildNumber}", () =>
            {
                BuildAndroid(true);
            }, () =>
            {
                Debug.Log("Cancel");
            });
        }

#endif
        GUILayout.EndHorizontal();
    }

    static void ShowDialog(string _title, string _desc, Action _ok, Action _cancel)
    {
        if (EditorUtility.DisplayDialog(_title, _desc, "OK", "Cancel"))
        {
            _ok?.Invoke();
        }
        else
        {
            _cancel?.Invoke();
        }
    }

    static void BuildAndroid(bool _isAAB)
    {
        PlayerSettings.bundleVersion = buildAppVersion;
        PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
        Builder.BuildAPK(_isAAB);
    }

    static void BuildiOS()
    {
        PlayerSettings.bundleVersion = buildAppVersion;
        PlayerSettings.iOS.buildNumber = buildNumber;
        Builder.BuildiOS();
    }
}