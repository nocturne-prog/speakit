
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;
using System;
using System.Text;

public class Builder
{
    public static string[] SCENES = FindEnabledEditorScenes();
    public static string APP_NAME = "";
#if UNITY_IOS
    public static string TARGET_DIR = "Build/iOS";
#else
    public static string TARGET_DIR = "Build/Android";
#endif
    public static string KEYSTORE_PASS = "";
    public static string KEYALIAS_PASS = "";

    public static void BuildJenkins()
    {
        StringBuilder sb = new StringBuilder();

        var cLines = Environment.GetCommandLineArgs();

        foreach (var v in cLines)
        {
            sb.AppendLine(v);
        }

        Debug.Log(sb.ToString());

        Debug.Log("Build Start !!");

        BuildAPK();
    }

    [MenuItem("Tools/Builder/Android/Build APK")]
    public static void Build_APK()
    {
        BuildAPK();
    }

    [MenuItem("Tools/Builder/Android/Build AAB")]
    public static void Build_AAB()
    {
        BuildAPK(true);
    }

    [MenuItem("Tools/Builder/iOS/Build")]
    public static void Build_iOS()
    {
        BuildiOS();
    }

    public static void CommandLineBuild()
    {
        /**
         * -buildTarget
         * -version
         * -buildNumber
         * -isAAB
         * -isRelease
         */

        string buildTarget = GetCommandLine("-buildTarget");
        string version = GetCommandLine("-appVersion");
        int buildNumber = GetCommandLineToInteger("-buildNumber");
        bool isAAB = GetCommandLineToBoolean("isAAB");
        string relese = GetCommandLine("-isRelease");

        if (string.IsNullOrEmpty(buildTarget))
        {
            buildTarget = "android";
        }

        if (string.IsNullOrEmpty(version))
        {
            version = "1.0.0";
        }

        if (string.IsNullOrEmpty(relese))
        {
            relese = "debug";
        }

        if (buildNumber < 1)
        {
            buildNumber = 1;
        }    

        Debug.Log($"BuildTarget :: {buildTarget}\n" +
                  $"Version :: {version}.{buildNumber}\n" +
                  $"isAAB :: {isAAB}\n" +
                  $"isRelease :: {relese}");

        PlayerSettings.bundleVersion = version;

        if (buildTarget.ToLower().Equals("ios") is true)
        {
            PlayerSettings.iOS.buildNumber = buildNumber.ToString();
        }
        else
        {
            PlayerSettings.Android.bundleVersionCode = buildNumber;
        }

        if (buildTarget.ToLower().Equals("ios") is true)
        {
            BuildiOS(_updatePod : true);
        }
        else
        {
            BuildAPK(isAAB);
        }
    }

    public static void BuildAPK(bool _isAAB = false)
    {
        string fileName = string.Format("{0}_Debug.{1}", APP_NAME, _isAAB ? "aab" : "apk");

        EditorUserBuildSettings.buildAppBundle = _isAAB;

        string strOutputDir = string.Format("{0}/{1}", Directory.GetCurrentDirectory(), TARGET_DIR);
        if (Directory.Exists(strOutputDir) == false)
        {
            var di = Directory.CreateDirectory(strOutputDir);
            if (di != null)
            {
                Debug.Log($"Make output Dir =>({di.FullName})");
            }
            else
            {
                Debug.Log("Directory is null");
            }
        }
        else
        {
            Debug.Log($"Make output Dir Exists , strOutputDir is {strOutputDir}");
        }

        bool result = GenericBuild(SCENES, strOutputDir + @"\" + fileName, BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.None);
    }

    public static void BuildiOS(bool _updatePod = false)
    {
        bool result = GenericBuild(SCENES, TARGET_DIR, BuildTargetGroup.iOS, BuildTarget.iOS, BuildOptions.None);

        if (result)
        {
            Directory.Delete($"{Directory.GetCurrentDirectory()}/Build/iOS_BurstDebugInformation_DoNotShip", true);

            if(_updatePod is true)
            {
                File.Copy("iOS_BuildSupport/podfile", $"{TARGET_DIR}/podfile");

                System.Diagnostics.Process psi = new System.Diagnostics.Process
                {
                    StartInfo =
                {
                    FileName = @"/System/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    Arguments = $"sh {Directory.GetCurrentDirectory()}/iOS_BuildSupport/BuildSupporter.sh"
                }
                };

                psi.Start();
            }
        }
    }

    static bool GenericBuild(string[] scenes, string target_dir, BuildTargetGroup build_group, BuildTarget build_target, BuildOptions build_options)
    {
        //return true;

        PlayerSettings.keystorePass = KEYSTORE_PASS;
        PlayerSettings.keyaliasPass = KEYALIAS_PASS;

        EditorUserBuildSettings.SwitchActiveBuildTarget(build_group, build_target);
        BuildReport report = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed: " + summary.totalErrors + " erros");
        }
        else
        {
            Debug.Log("Build failed: " + summary.result);
        }

        return summary.result == BuildResult.Succeeded;        
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    public static string GetCommandLine(string _key)
    {
        var args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            if (_key.Equals(args[i]))
            {
                try
                {
                    string value = args[i + 1];
                    Debug.Log($"GetCommandLine :: {_key} // {value}");
                    return value.Trim();
                }
                catch (Exception e)
                {
                    Debug.LogError($"GetCommandLine Exception :: {e}");
                }
            }
        }

        return string.Empty;
    }

    public static int GetCommandLineToInteger(string _key)
    {
        string value = GetCommandLine(_key);

        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }
        else
        {
            if (int.TryParse(value, out int v))
            {
                return v;
            }
            else
            {
                return 0;
            }
        }
    }

    public static bool GetCommandLineToBoolean(string _key)
    {
        string value = GetCommandLine(_key);

        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        else
        {
            if (bool.TryParse(value, out bool v))
            {
                return v;
            }
            else
            {
                return false;
            }
        }
    }
}