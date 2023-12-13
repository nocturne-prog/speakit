using DG.Tweening;
using Marvrus.UI;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIPopup_Tweener))]
public class UIPopup_Tweener_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        UIPopup_Tweener t = (UIPopup_Tweener)target;

        t.moveType = (UIPopup_Tweener_Kind)EditorGUILayout.EnumPopup("Tween Type", t.moveType);
        t.easeType = (Ease)EditorGUILayout.EnumPopup("Ease Type", t.easeType);
        t.duration = EditorGUILayout.FloatField("Duration", t.duration);

        if (t.moveType == UIPopup_Tweener_Kind.FadeInOut)
        {
            if (t.canvasGroup == null)
            {
                CanvasGroup cg = target.AddComponent<CanvasGroup>();
                t.canvasGroup = cg;
            }
        }
        else
        {
            t.from = EditorGUILayout.Vector2Field("From", t.from);

            if (t.canvasGroup != null)
            {
                CanvasGroup cg = target.GetComponent<CanvasGroup>();

                if (cg == null)
                    return;

                DestroyImmediate(cg);
                t.canvasGroup = null;
            }
        }

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        if (Application.isPlaying)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show"))
            {
                t.Show();
            }

            if (GUILayout.Button("Hide"))
            {
                t.Hide();
            }

            GUILayout.EndHorizontal();
        }
    }
}
