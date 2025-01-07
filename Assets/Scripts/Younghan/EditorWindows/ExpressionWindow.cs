#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Ư�� ������ ������ ���� ������� �˷��ִ� ���� ������
/// </summary>
public class ExpressionWindow : EditorWindow
{
    private static readonly string TextFolderPath = "Assets/Resources/Expressions/";
    private static readonly string TextSelectedFormula = "���õ� ����";

    private const bool Write = true;
    private const bool Read = false;

    private bool selection = true;

    [SerializeField]
    private string text;
    [SerializeField]
    private Expression.Term term;
    [SerializeField]
    private Expression.Extra[] extras;
    [SerializeField]
    private bool sum;

    private uint level = 0;
    private Expression expression = null;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("����"))
        {
            if(selection != Write)
            {
                SetPropertiesValue();
            }
            selection = Write;
        }
        if (GUILayout.Button("�б�"))
        {
            selection = Read;
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(40f);
        switch(selection)
        {
            case Write:
                Expression previous = expression;
                expression = EditorGUILayout.ObjectField(TextSelectedFormula, expression, typeof(Expression), true) as Expression;
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
                            if (AssetDatabase.LoadAssetAtPath(string.Format(TextFolderPath + "{0}.asset", text), typeof(Expression)) as Expression != null)
                            {
                                Debug.LogError("�̹� �ߺ��Ǵ� �̸��� ���� ������ �ֽ��ϴ�.");
                            }
                            else
                            {
                                AssetDatabase.RenameAsset(TextFolderPath + expression.name + ".asset", text);
                            }
                        }
                        expression.term = term;
                        expression.extras = extras;
                        expression.sum = sum;
                        AssetDatabase.SaveAssets();
                    }
                    if (GUILayout.Button("����"))
                    {
                        AssetDatabase.DeleteAsset(TextFolderPath + expression.name + ".asset");
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
                        StringBuilder currentPath = new StringBuilder();
                        StringBuilder previousPath = new StringBuilder();
                        string[] path = TextFolderPath.Split('/');
                        int length = path.Length - 1;
                        for (int i = 0; i < length; ++i)
                        {
                            currentPath.Append(path[i]);
                            if (Directory.Exists(currentPath.ToString()) == false)
                            {
                                AssetDatabase.CreateFolder(previousPath.ToString(), path[i]);
                            }
                            previousPath.Clear();
                            previousPath.Append(currentPath);
                            currentPath.Append("/");
                        }
                        expression = AssetDatabase.LoadAssetAtPath(string.Format(TextFolderPath + "{0}.asset", text), typeof(Expression)) as Expression;
                        //��θ� �����µ� �ش� ������ ���ٸ� ���� ����
                        if (expression == null)
                        {
                            expression = CreateInstance<Expression>();
                            expression.term = term;
                            expression.extras = extras;
                            expression.sum = sum;
                            AssetDatabase.CreateAsset(expression, string.Format(TextFolderPath + "{0}.asset", text));
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
            case Read:
                expression = EditorGUILayout.ObjectField(TextSelectedFormula, expression, typeof(Expression), true) as Expression;
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
                        string value = EditorGUILayout.TextField("���õ� �������� ��", level.ToString());
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
            term = new Expression.Term();
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
    [MenuItem("ScriptableObjects/Expression")]
    private static void ShowWindow()
    {
        ExpressionWindow expressionWindow = (ExpressionWindow)GetWindow(typeof(ExpressionWindow));
        expressionWindow.titleContent.text = "����";
        expressionWindow.Show();
    }
}
#endif