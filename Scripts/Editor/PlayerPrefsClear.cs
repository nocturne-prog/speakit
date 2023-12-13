using UnityEngine;
using UnityEditor;
using System.IO;

public class PlayerPrefsClear : ScriptableObject
{
    [MenuItem("Tools/Clear all Editor Preferences")]
    static void DeleteAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Delete all PlayerPref",
            "Are you sure you want to delete all the PlayerPref? " +
            "This action cannot be undone.", "Yes", "No"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }

    [MenuItem("Tools/Clear all File Cache")]
    static void DeleteAllFileCache()
    {
        if (EditorUtility.DisplayDialog("Delete all FileCache",
            "Are you sure you want to delete all the FileCache? " +
            "This action cannot be undone.", "Yes", "No"))
        {
            if (Directory.Exists(Const.FILE_PAHT) is true)
                Directory.Delete(Const.FILE_PAHT, true);
        }
    }
}
