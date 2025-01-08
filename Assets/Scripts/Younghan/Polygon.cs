using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 다각형 구조체
/// </summary>
public struct Polygon
{
    //기본 정점 1
    public Vector2 point1;
    //기본 정점 2
    public Vector2 point2;
    //기본 정점 3
    public Vector2 point3;
    //추가 정점들
    public Vector2[] otherPoints;

    //정점들을 리스트화 시켜주는 프로퍼티
    private List<Vector2> vertices {
        get
        {
            List<Vector2> list = new List<Vector2>() { point1, point2, point3 };
            if (otherPoints != null)
            {
                list.AddRange(otherPoints);
            }
            return list;
        }
    }

    public Polygon(Vector2 point1, Vector2 point2, Vector2 point3, Vector2[] otherPoints = null)
    {
        this.point1 = point1;
        this.point2 = point2;
        this.point3 = point3;
        this.otherPoints = otherPoints;
    }

    public void Draw(Color color, float duration = 0)
    {
#if UNITY_EDITOR
        List<Vector2> list = vertices;
        int count = list.Count;
        for (int i = 0; i < count - 1; i++)
        {
            if (list[i] != list[i + 1])
            {
                Debug.DrawLine(list[i], list[i + 1], color, duration);
            }
            else
            {
                Segment.Draw(list[i + 1], color, duration);
            }
        }
        Debug.DrawLine(list[0], list[count - 1], color);
#endif
    }

    /// <summary>
    /// 특정 점이 다각형 안에 있는지 확인하는 함수
    /// </summary>
    /// <param name="point"></param>
    /// <returns>특정 점이 다각형 안에 있으면 참 반환</returns>
    public bool IsPointIn(Vector2 point)
    {
        List<Vector2> list = vertices;
        int count = list.Count;











        bool inside = false;
        // 다각형의 각 선분에 대해 교차 여부를 확인
        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            // 점이 선분의 y좌표 범위 안에 있을 때만 체크
            if ((list[i].y > point.y) != (list[j].y > point.y) && point.x < (list[j].x - list[i].x) * (point.y - list[i].y) / (list[j].y - list[i].y) + list[i].x)
            {
                inside = !inside;
            }
        }
        return inside;
    }

    public static Polygon[] GetArray(Vector2[] vertices, out Segment[] segments)
    {
        segments = null;
        return null;
    }

    public static Polygon[] GetArray(ref Segment[] segments)
    {

        return null;
    }
}