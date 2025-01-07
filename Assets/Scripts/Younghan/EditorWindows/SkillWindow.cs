#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 특정 스킬의 정보를 알려주는 에디터
/// </summary>
public class SkillWindow : EditorWindow
{
    private static readonly string TextFolderPath = "Assets/Resources/Skills/";

    private Skill skill = null;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        //if (GUILayout.Button("쓰기"))
        //{
        //    if (selection != Write)
        //    {
        //        SetPropertiesValue();
        //    }
        //    selection = Write;
        //}
        //if (GUILayout.Button("읽기"))
        //{
        //    selection = Read;
        //}
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(40f);
    }

    //창을 열어주는 함수
    [MenuItem("ScriptableObjects/Skill")]
    private static void ShowWindow()
    {
        SkillWindow skillWindow = (SkillWindow)GetWindow(typeof(SkillWindow));
        skillWindow.titleContent.text = "스킬";
        skillWindow.Show();
    }
}
#endif