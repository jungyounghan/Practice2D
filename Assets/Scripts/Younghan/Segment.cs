using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 선분 구조체
/// </summary>
public struct Segment
{
    private static readonly float DotSize = 0.01f;

    public Vector2 start;

    public Vector2 end;

    /// <summary>
    /// 제대로 된 선분 형태인지를 반환한다.
    /// </summary>
    public bool isSegment {
        get
        {
            return start != end;
        }
    }

    public Segment(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }

    public Segment(Segment segment)
    {
        start = segment.start;
        end = segment.end;
    }

    public static bool operator >(Segment a, Segment b)
    {
        return a.GetDistance() > b.GetDistance();
    }

    public static bool operator <(Segment a, Segment b)
    {
        return a.GetDistance() < b.GetDistance();
    }

    /// <summary>
    /// 점을 출력해주는 함수
    /// </summary>
    /// <param name="point"></param>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    public static void Draw(Vector2 point, Color color, float duration)
    {
#if UNITY_EDITOR
        float halfSize = DotSize * 0.5f;
        Vector2 dot1 = new Vector2(point.x + halfSize, point.y + halfSize);
        Vector2 dot2 = new Vector2(point.x - halfSize, point.y + halfSize);
        Vector2 dot3 = new Vector2(point.x - halfSize, point.y - halfSize);
        Vector2 dot4 = new Vector2(point.x + halfSize, point.y - halfSize);
        Debug.DrawLine(dot1, dot2, color, duration);
        Debug.DrawLine(dot2, dot3, color, duration);
        Debug.DrawLine(dot3, dot4, color, duration);
        Debug.DrawLine(dot4, dot1, color, duration);
#endif
    }

    /// <summary>
    /// 선을 출력해주는 함수
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="duration"></param>
    public static void Draw(Vector2[] vertices, Color color, float duration)
    {
#if UNITY_EDITOR
        int length = vertices != null ? vertices.Length : 0;
        for (int i = 0; i < length - 1; i++)
        {
            if (vertices[i] != vertices[i + 1])
            {
                Debug.DrawLine(vertices[i], vertices[i + 1], color, duration);
            }
            else
            {
                Draw(vertices[i + 1], color, duration);
            }
        }
#endif
    }

    /// <summary>
    /// 지정된 시작점과 끝점 사이에 선을 그린다.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    public void Draw(Color color, float duration = 0)
    {
#if UNITY_EDITOR
        if(isSegment == true)
        {
            Debug.DrawLine(start, end, color, duration);
        }
        else
        {
            Draw(start, color, duration);
        }
#endif
    }

    /// <summary>
    /// 정점들로 이어지는 선분들을 반환하는 함수
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static Segment[] GetArray(Vector2[] vertices)
    {
        int length = vertices != null ? vertices.Length : 0;
        if(length > 1)
        {
            List<Segment> segments = new List<Segment>();
            for(int i = 0; i < length - 1; i++)
            {
                segments.Add(new Segment(vertices[i], vertices[i + 1]));
            }
            return segments.ToArray();
        }
        return null;
    }

    /// <summary>
    /// 특정 점이 선분 안에 있는지 확인
    /// </summary>
    /// <param name="point"></param>
    /// <returns>선분 안에 있으면 참을 반환</returns>
    public bool IsPointIn(Vector2 point)
    {
        return Mathf.Abs((point - start).magnitude + (point - end).magnitude - (end - start).magnitude) < Mathf.Epsilon;
    }

    public float GetDistance()
    {
        return Vector2.Distance(start, end);
    }

    /// <summary>
    /// 교차점을 반환하는 함수
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="q1"></param>
    /// <param name="q2"></param>
    /// <returns>교차점이 있으면 Vector2를 반환</returns>
    public Vector2? GetIntersection(Segment segment)
    {
        Vector2 segment1Vector = end - start; // 선분1 벡터
        Vector2 segment2Vector = segment.end - segment.start; // 선분2 벡터
        float denominator = segment1Vector.x * segment2Vector.y - segment1Vector.y * segment2Vector.x;
        // 두 선분이 평행한 경우
        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            return null;
        }
        Vector2 vectorDistance = segment.start - start;
        float segment1Parameter = (vectorDistance.x * segment2Vector.y - vectorDistance.y * segment2Vector.x) / denominator; // 선분1의 파라미터
        float segment2Parameter = (vectorDistance.x * segment1Vector.y - vectorDistance.y * segment1Vector.x) / denominator; // 선분2의 파라미터
        // 선분1의 파라미터와 선분2의 파라미터가 [0, 1] 범위 내에 있으면 두 선분이 교차
        if (segment1Parameter >= 0 && segment1Parameter <= 1 && segment2Parameter >= 0 && segment2Parameter <= 1)
        {
            return start + segment1Parameter * segment1Vector; // 교차점 반환
        }
        // 교차하지 않음
        return null;
    }

    /// <summary>
    /// 선분의 시작과 끝을 바꿔서 반환하는 함수
    /// </summary>
    /// <returns>시작과 끝점을 거꾸로 위치 시켜서 반환</returns>
    public Segment GetReverse()
    {
        return new Segment(end, start);
    }

}