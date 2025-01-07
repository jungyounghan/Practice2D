#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Ư�� ��ų�� ������ �˷��ִ� ������
/// </summary>
public class SkillWindow : EditorWindow
{
    private static readonly string TextFolderPath = "Assets/Resources/Skills/";

    private Skill skill = null;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        //if (GUILayout.Button("����"))
        //{
        //    if (selection != Write)
        //    {
        //        SetPropertiesValue();
        //    }
        //    selection = Write;
        //}
        //if (GUILayout.Button("�б�"))
        //{
        //    selection = Read;
        //}
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(40f);
    }

    //â�� �����ִ� �Լ�
    [MenuItem("ScriptableObjects/Skill")]
    private static void ShowWindow()
    {
        SkillWindow skillWindow = (SkillWindow)GetWindow(typeof(SkillWindow));
        skillWindow.titleContent.text = "��ų";
        skillWindow.Show();
    }
}
#endif