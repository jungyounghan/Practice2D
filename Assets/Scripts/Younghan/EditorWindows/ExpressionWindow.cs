#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using static Expression;

/// <summary>
/// 특정 수식의 레벨에 대한 결과값을 알려주는 수식 에디터
/// </summary>
public class ExpressionWindow : EditorWindow
{
    private static readonly string TEXT_SELECTED_FORMULA = "선택된 수식";
    private static readonly string TEXT_SELECTED_UNKNOWN = "선택된 미지수의 값";
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
        if (GUILayout.Button("쓰기"))
        {
            if(selection != SELECTION_WRITE)
            {
                SetPropertiesValue();
            }
            selection = SELECTION_WRITE;
        }
        if (GUILayout.Button("읽기"))
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
                            if (AssetDatabase.LoadAssetAtPath(string.Format(TEXT_FOLDER_PATH + "{0}" + Archive.TEXT_EXTENSION_ASSET, text), typeof(Expression)) as Expression != null)
                            {
                                Debug.LogError("이미 중복되는 이름을 가진 수식이 있습니다.");
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
                    if (GUILayout.Button("삭제"))
                    {
                        AssetDatabase.DeleteAsset(TEXT_FOLDER_PATH + expression.name + Archive.TEXT_EXTENSION_ASSET);
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
                        if (Directory.Exists(TEXT_FOLDER_PATH) == false)
                        {                            //Directory.CreateDirectory(path); 바로 인식이 안됨
                            AssetDatabase.CreateFolder(Archive.TEXT_PATH_ASSETS + "/" + Archive.TEXT_PATH_RESOURCES, Archive.TEXT_PATH_EXPRESSIONS);
                        }
                        expression = AssetDatabase.LoadAssetAtPath(string.Format(TEXT_FOLDER_PATH + "{0}.asset", text), typeof(Expression)) as Expression;
                        //경로를 열었는데 해당 수식이 없다면 새로 생성
                        if (expression == null)
                        {
                            expression = CreateInstance<Expression>();
                            expression.term = term;
                            expression.extras = extras;
                            expression.sum = sum;
                            AssetDatabase.CreateAsset(expression, string.Format(TEXT_FOLDER_PATH + "{0}.asset", text));
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
            term = new Term();
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
    [MenuItem("Tools/Test/Expression")]
    private static void ShowWindow()
    {
        ExpressionWindow expressionWindow = (ExpressionWindow)GetWindow(typeof(ExpressionWindow));
        expressionWindow.titleContent.text = "수식";
        expressionWindow.Show();
    }
}
#endif