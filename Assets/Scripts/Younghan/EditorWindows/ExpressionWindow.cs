#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using static Expression;

/// <summary>
/// Ư�� ������ ������ ���� ������� �˷��ִ� ���� ������
/// </summary>
public class ExpressionWindow : EditorWindow
{
    private static readonly string TEXT_SELECTED_FORMULA = "���õ� ����";
    private static readonly string TEXT_SELECTED_UNKNOWN = "���õ� �������� ��";
    private static readonly string TEXT_FOLDER_PATH = "Assets/Resources/Managers/";

    private const bool SELECTION_WRITE = true;
    private const bool SELECTION_READ = false;

    private bool selection = true;

    [SerializeField]
    private string text;
    [SerializeField]
    private Term term;
    [SerializeField]
    private Extra[] extras;
    [SerializeField]
    private bool sum;

    private uint level = 0;
    private Expression expression = null;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("����"))
        {
            if(selection != SELECTION_WRITE)
            {
                SetPropertiesValue();
            }
            selection = SELECTION_WRITE;
        }
        if (GUILayout.Button("�б�"))
        {
            selection = SELECTION_READ;
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(40f);
        switch(selection)
        {
            case SELECTION_WRITE:
                Expression previous = expression;
                expression = EditorGUILayout.ObjectField(TEXT_SELECTED_FORMULA, expression, typeof(Expression), true) as Expression;
                if(previous != expression)
                {
                    SetPropertiesValue();
                }
                text = EditorGUILayout.TextField("���� �̸�", text);
                SerializedObject serializedObject = new SerializedObject(this);
                SerializedProperty textProperty = serializedObject.FindProperty("term");
                EditorGUILayout.PropertyField(textProperty);
                textProperty = serializedObject.FindProperty("extras");
                EditorGUILayout.PropertyField(textProperty);
                serializedObject.ApplyModifiedProperties();
                sum = EditorGUILayout.Toggle("������ �� ��� ����", sum);
                if (expression != null)
                {
                    if (GUILayout.Button("����"))
                    {
                        if(expression.name != text)
                        {
                            if (AssetDatabase.LoadAssetAtPath(string.Format(TEXT_FOLDER_PATH + "{0}" + Archive.TEXT_EXTENSION_ASSET, text), typeof(Expression)) as Expression != null)
                            {
                                Debug.LogError("�̹� �ߺ��Ǵ� �̸��� ���� ������ �ֽ��ϴ�.");
                            }
                            else
                            {
                                AssetDatabase.RenameAsset(TEXT_FOLDER_PATH + expression.name + Archive.TEXT_EXTENSION_ASSET, text);
                            }
                        }
                        expression.term = term;
                        expression.extras = extras;
                        expression.sum = sum;
                        AssetDatabase.SaveAssets();
                    }
                    if (GUILayout.Button("����"))
                    {
                        AssetDatabase.DeleteAsset(TEXT_FOLDER_PATH + expression.name + Archive.TEXT_EXTENSION_ASSET);
                        expression = null;
                    }
                }
                else if (GUILayout.Button("����"))
                {
                    if (string.IsNullOrWhiteSpace(text) == true)
                    {
                        Debug.LogError("������ �̸��� �������� �ʾҽ��ϴ�.");
                    }
                    else
                    {
                        //�ش� ��ο� ������ ���ٸ� ���� ����
                        if (Directory.Exists(TEXT_FOLDER_PATH) == false)
                        {                            //Directory.CreateDirectory(path); �ٷ� �ν��� �ȵ�
                            AssetDatabase.CreateFolder(Archive.TEXT_PATH_ASSETS + "/" + Archive.TEXT_PATH_RESOURCES, Archive.TEXT_PATH_EXPRESSIONS);
                        }
                        expression = AssetDatabase.LoadAssetAtPath(string.Format(TEXT_FOLDER_PATH + "{0}.asset", text), typeof(Expression)) as Expression;
                        //��θ� �����µ� �ش� ������ ���ٸ� ���� ����
                        if (expression == null)
                        {
                            expression = CreateInstance<Expression>();
                            expression.term = term;
                            expression.extras = extras;
                            expression.sum = sum;
                            AssetDatabase.CreateAsset(expression, string.Format(TEXT_FOLDER_PATH + "{0}.asset", text));
                        }
                        //���� �ִٸ� ������ ���� ����
                        else
                        {
                            expression.term = term;
                            expression.extras = extras;
                            expression.sum = sum;
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
                break;
            case SELECTION_READ:
                expression = EditorGUILayout.ObjectField(TEXT_SELECTED_FORMULA, expression, typeof(Expression), true) as Expression;
                if (expression == null)
                {
                    HideLevelText();
                }
                else
                {
                    string formula = expression.GetFormula();
                    if (double.TryParse(formula, out double result))
                    {
                        HideLevelText();
                        GUILayout.Space(40f);
                        GUILayout.Label(result.ToString(), EditorStyles.boldLabel);
                    }
                    else
                    {
                        string value = EditorGUILayout.TextField(TEXT_SELECTED_UNKNOWN, level.ToString());
                        if(uint.TryParse(value, out level) == false)
                        {
                            level = 0;
                        }
                        if (GUILayout.Button("��� ����"))
                        {
                            text = formula + "=" + expression.GetResult(level);
                        }
                        if (text == formula + "=" + expression.GetResult(level))
                        {
                            GUILayout.Space(40f);
                            GUILayout.Label("x:" + level + "\t" + text, EditorStyles.boldLabel);
                        }
                    }
                }
                break;
        }
    }

    //������ �Ӽ��� ������Ƽ �ʵ忡 �������ִ� �Լ�
    private void SetPropertiesValue()
    {
        if (expression != null)
        {
            text = expression.name;
            term = expression.term;
            extras = expression.extras;
            sum = expression.sum;
        }
        else
        {
            text = null;
            term = new Term();
            extras = null;
            sum = false;
        }
    }

    //���� ���� �ؽ�Ʈ�� ������ �ʰ� ���ִ� �Լ�
    private void HideLevelText()
    {
        text = null;
        level = 0;
    }

    //â�� �����ִ� �Լ�
    [MenuItem("Tools/Test/Expression")]
    private static void ShowWindow()
    {
        ExpressionWindow expressionWindow = (ExpressionWindow)GetWindow(typeof(ExpressionWindow));
        expressionWindow.titleContent.text = "����";
        expressionWindow.Show();
    }
}
#endif