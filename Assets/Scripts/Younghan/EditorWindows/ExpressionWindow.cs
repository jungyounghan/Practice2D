#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 특정 수식의 레벨에 대한 결과값을 알려주는 수식 에디터
/// </summary>
public class ExpressionWindow : EditorWindow
{
    private static readonly string TextFolderPath = "Assets/Resources/Expressions/";
    private static readonly string TextSelectedFormula = "선택된 수식";

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
        if (GUILayout.Button("쓰기"))
        {
            if(selection != Write)
            {
                SetPropertiesValue();
            }
            selection = Write;
        }
        if (GUILayout.Button("읽기"))
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
                text = EditorGUILayout.TextField("수식 이름", text);
                SerializedObject serializedObject = new SerializedObject(this);
                SerializedProperty textProperty = serializedObject.FindProperty("term");
                EditorGUILayout.PropertyField(textProperty);
                textProperty = serializedObject.FindProperty("extras");
                EditorGUILayout.PropertyField(textProperty);
                serializedObject.ApplyModifiedProperties();
                sum = EditorGUILayout.Toggle("수열의 합 사용 유무", sum);
                if (expression != null)
                {
                    if (GUILayout.Button("수정"))
                    {
                        if(expression.name != text)
                        {
                            if (AssetDatabase.LoadAssetAtPath(string.Format(TextFolderPath + "{0}.asset", text), typeof(Expression)) as Expression != null)
                            {
                                Debug.LogError("이미 중복되는 이름을 가진 수식이 있습니다.");
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
                    if (GUILayout.Button("삭제"))
                    {
                        AssetDatabase.DeleteAsset(TextFolderPath + expression.name + ".asset");
                        expression = null;
                    }
                }
                else if (GUILayout.Button("생성"))
                {
                    if (string.IsNullOrWhiteSpace(text) == true)
                    {
                        Debug.LogError("수식의 이름이 정해지지 않았습니다.");
                    }
                    else
                    {
                        //해당 경로에 폴더가 없다면 새로 생성
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
                        //경로를 열었는데 해당 수식이 없다면 새로 생성
                        if (expression == null)
                        {
                            expression = CreateInstance<Expression>();
                            expression.term = term;
                            expression.extras = extras;
                            expression.sum = sum;
                            AssetDatabase.CreateAsset(expression, string.Format(TextFolderPath + "{0}.asset", text));
                        }
                        //만약 있다면 내용을 새로 수정
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
                        string value = EditorGUILayout.TextField("선택된 미지수의 값", level.ToString());
                        if(uint.TryParse(value, out level) == false)
                        {
                            level = 0;
                        }
                        if (GUILayout.Button("결과 보기"))
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

    //수식의 속성을 프로퍼티 필드에 대입해주는 함수
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

    //레벨 설정 텍스트를 보이지 않게 해주는 함수
    private void HideLevelText()
    {
        text = null;
        level = 0;
    }

    //창을 열어주는 함수
    [MenuItem("ScriptableObjects/Expression")]
    private static void ShowWindow()
    {
        ExpressionWindow expressionWindow = (ExpressionWindow)GetWindow(typeof(ExpressionWindow));
        expressionWindow.titleContent.text = "수식";
        expressionWindow.Show();
    }
}
#endif