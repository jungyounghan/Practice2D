using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 미지수에 대한 반환 값을 설정하는 수식 클래스
/// </summary>
[CreateAssetMenu(menuName = nameof(Expression), order = 0)]
public class Expression : ScriptableObject, ISerializationCallbackReceiver
{
    //산술 연산자 열거형 변수
    public enum Operation: byte
    {
        Add,                                                //더하기
        Subtract,                                           //빼기
        Multiply,                                           //곱하기
        Divide,                                             //나누기
        Modulo,                                             //나머지
        Power,                                              //제곱
    }

    //특정 값을 계산한 이후 어떠한 형태로 그 내용을 보정할지 선택하는 열거형 변수
    public enum Customizing: byte
    {
        None,                                               //보정 없음
        Absolute,                                           //절대값화
        Ceiling,                                            //올림화
        Round,                                              //반올림화
        Floor,                                              //내림화     
        SquareRoot,                                         //제곱근화
        CubeRoot,                                           //세제곱근화
    }

    //개별 항 하나를 의미하는 구조체
    [Serializable]
    public struct Term
    {
        [SerializeField, Header("계수")]
        private double coefficient;                 //미지수와 곱해지는 값이다.
        [SerializeField, Header("차수")]
        private double degree;                      //몇 차 방정식인지를 결정할 값이다.

        //미지수에 대한 단항의 결과 값을 반환해주는 함수
        public double GetValue(uint variable)
        {
            return coefficient * Math.Pow(variable, degree);
        }

        //미지수에 대한 해당 항의 공식을 문자열로 반환해주는 함수
        public string GetFormula()
        {
            if (coefficient != 0)                           //계수 a가 0이 아닐 때만 계산을 한다
            {
                if (degree != 0)                            //차수 b가 0이 아니라면 미지수 x와 함께 계산이 된다.
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(coefficient.ToString() + "x");
                    if (degree != 1)                         //차수가 1이 아니라면 axᵇ
                    {
                        string value = degree.ToString();
                        for (int i = 0; i < value.Length; i++)
                        {
                            switch (value[i])
                            {
                                case '0':
                                    stringBuilder.Append("⁰");
                                    break;
                                case '1':
                                    stringBuilder.Append("¹");
                                    break;
                                case '2':
                                    stringBuilder.Append("²");
                                    break;
                                case '3':
                                    stringBuilder.Append("³");
                                    break;
                                case '4':
                                    stringBuilder.Append("⁴");
                                    break;
                                case '5':
                                    stringBuilder.Append("⁵");
                                    break;
                                case '6':
                                    stringBuilder.Append("⁶");
                                    break;
                                case '7':
                                    stringBuilder.Append("⁷");
                                    break;
                                case '8':
                                    stringBuilder.Append("⁸");
                                    break;
                                case '9':
                                    stringBuilder.Append("⁹");
                                    break;
                                case '.':
                                    stringBuilder.Append("˙");
                                    break;
                                case '-':
                                    stringBuilder.Append("⁻");
                                    break;
                            }
                        }
                    }
                    return stringBuilder.ToString();        //차수가 1이라면 ax    
                }
                else                                        //차수가 0이라면 차수항의 값이 1이 되어서 미지수를 포함하지 않는 상수항으로 변경 될 수 있다.
                {
                    return coefficient.ToString();
                }
            }
            return "0";                                     //계수 a가 0이면 반환 값은 무조건 0이다.
        }
    }

    //추가 연산 함수
    [Serializable]
    public struct Extra
    {
        [SerializeField, Header("커스터마이징 함수")]
        public Customizing customizing;
        [SerializeField, Header("산술 연산자")]
        public Operation operation;
        [SerializeField, Header("항")]
        public Term term;
    }

    [SerializeField, Header("기초 항")]
    public Term term;
    [SerializeField, Header("추가 항")]
    public Extra[] extras;
    [SerializeField, Header("수열의 합 사용 유무")]
    public bool sum;

    private Dictionary<uint, double> _dictionary = new Dictionary<uint, double>();

    //직렬화 전 호출되는 콜백 함수
    public void OnBeforeSerialize() 
    {
    }

    //역직렬화 후 호출되는 콜백 함수
    public void OnAfterDeserialize()
    {
        _dictionary.Clear();
    }

    //변수 값에 대한 수식의 결과 값을 반환해주는 함수
    private double GetValue(uint variable = 1)
    {
        double result = term.GetValue(variable);
        int length = extras != null ? extras.Length : 0;
        for (int i = 0; i < length; i++)
        {
            double value = extras[i].term.GetValue(variable);
            switch(extras[i].operation)
            {
                case Operation.Add:
                    result += value;
                    break;
                case Operation.Subtract:
                    result -= value;
                    break;
                case Operation.Multiply:
                    result *= value;
                    break;
                case Operation.Divide:
                    if (value == 0)
                    {
                        if (result > 0)
                        {
                            result = double.PositiveInfinity;
                        }
                        else if (result < 0)
                        {
                            result = double.NegativeInfinity;
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                    else
                    {
                        result /= value;
                    }
                    break;
                case Operation.Modulo:
                    if (value == 0)
                    {
                        result = 0;
                    }
                    else
                    {
                        result %= value;
                    }
                    break;
                case Operation.Power:
                    result = Math.Pow(result, value);
                    break;
            }
            switch (extras[i].customizing)
            {
                case Customizing.Absolute:
                    result = Math.Abs(result);
                    break;
                case Customizing.Ceiling:
                    result = Math.Ceiling(result);
                    break;
                case Customizing.Round:
                    result = Math.Round(result);
                    break;
                case Customizing.Floor:
                    result = Math.Floor(result);
                    break;
                case Customizing.SquareRoot:
                    result = Math.Sqrt(result);
                    break;
                case Customizing.CubeRoot:
                    result = Math.Cbrt(result);
                    break;
            }
        }
        return result;
    }

    //변수 값에 대한 수식의 최종 결과 값을 반환해주는 함수
    public double GetResult(uint variable = 1)
    {
        if (_dictionary.ContainsKey(variable) == false)
        {
            double result = GetValue(variable);
            if (sum == true)
            {
                for (uint i = variable - 1; i > 0; i--)
                {
                    if(_dictionary.ContainsKey(i) == true)
                    {
                        result += _dictionary[i];
                        break;
                    }
                    else
                    {
                        result += GetValue(i);
                    }
                }
            }
            _dictionary.Add(variable, result);
        }
        return _dictionary[variable];
    }

    //미지수에 대한 해당 공식을 문자열로 반환해주는 함수
    public string GetFormula()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(term.GetFormula());
        int length = extras != null ? extras.Length : 0;
        for (int i = 0; i < length; i++)
        {
            switch (extras[i].operation)
            {
                case Operation.Add:
                    stringBuilder.Append("＋(" + extras[i].term.GetFormula() + ")");
                    break;
                case Operation.Subtract:
                    stringBuilder.Append("-(" + extras[i].term.GetFormula() + ")");
                    break;
                case Operation.Multiply:
                    stringBuilder.Append("×(" + extras[i].term.GetFormula() + ")");
                    break;
                case Operation.Divide:
                    stringBuilder.Append("÷(" + extras[i].term.GetFormula() + ")");
                    break;
                case Operation.Modulo:
                    stringBuilder.Append("％(" + extras[i].term.GetFormula() + ")");
                    break;
                case Operation.Power:
                    stringBuilder.Append("^(" + extras[i].term.GetFormula() + ")");
                    break;
            }
            switch (extras[i].customizing)
            {
                case Customizing.Absolute:
                    stringBuilder.Insert(0, "절대값(");
                    stringBuilder.Append(")");
                    break;
                case Customizing.Ceiling:
                    stringBuilder.Insert(0, "올림(");
                    stringBuilder.Append(")");
                    break;
                case Customizing.Round:
                    stringBuilder.Insert(0, "반올림(");
                    stringBuilder.Append(")");
                    break;
                case Customizing.Floor:
                    stringBuilder.Insert(0, "내림(");
                    stringBuilder.Append(")");
                    break;
                case Customizing.SquareRoot:
                    stringBuilder.Insert(0, "√(");
                    stringBuilder.Append(")");
                    break;
                case Customizing.CubeRoot:
                    stringBuilder.Insert(0, "∛(");
                    stringBuilder.Append(")");
                    break;
            }
            if (i + 1 < length)
            {
                stringBuilder.Insert(0, "(");
                stringBuilder.Append(")");
            }
        }
        if (sum == true)
        {
            stringBuilder.Insert(0, "S(");
            stringBuilder.Append(")");
        }
        return stringBuilder.ToString();
    }

    //아이템 또는 스킬의 능력치에 대응하는 수식과 가용 레벨 영역 최대 범위를 담은 구조체
    [Serializable]
    public struct Info
    {
        [SerializeField]
        private Expression expression;

        [SerializeField]
        private float value;

        public double GetResult()
        {
            if(expression != null)
            {
                uint level = (uint)Mathf.Clamp(value, uint.MinValue, uint.MaxValue);
                return expression.GetResult(level);
            }
            return value;
        }
    }
}