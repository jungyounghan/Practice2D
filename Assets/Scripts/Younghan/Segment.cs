using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����ü
/// </summary>
public struct Segment
{
    private static readonly float DotSize = 0.01f;

    public Vector2 start;

    public Vector2 end;

    /// <summary>
    /// ����� �� ���� ���������� ��ȯ�Ѵ�.
    /// </summary>
    public bool isSegment {
        get
        {
            return start != end;
        }
    }

    /// <summary>
    /// ������ ���̸� ��ȯ�Ѵ�.
    /// </summary>
    public float distance
    {
        get
        {
            return Vector2.Distance(start, end);
        }
    }

    /// <summary>
    /// ������ ���۰� ���� �ٲ㼭 ��ȯ�ϴ� �Լ�
    /// </summary>
    public Segment reverse
    {
        get
        {
            return new Segment(end, start);
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

    /// <summary>
    /// ���� ������ִ� �Լ�
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
    /// ������ �������� ���� ���̿� ���� �׸���.
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
    /// ������� �̾����� ���е��� ��ȯ�ϴ� �Լ�
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
    /// Ư�� ���� ���� �ȿ� �ִ��� Ȯ��
    /// </summary>
    /// <param name="point"></param>
    /// <returns>���� �ȿ� ������ ���� ��ȯ</returns>
    public bool IsPointIn(Vector2 point)
    {
        return Mathf.Abs((point - start).magnitude + (point - end).magnitude - (end - start).magnitude) < Mathf.Epsilon;
    }


    /// <summary>
    /// �������� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="segment1"></param>
    /// <param name="segment2"></param>
    /// <returns>�������� ������ ����2�� ��ȯ</returns>
    public static Vector2? GetIntersection(Segment segment1, Segment segment2)
    {
        Vector2 segment1Vector = segment1.end - segment1.start; // ����1 ����
        Vector2 segment2Vector = segment2.end - segment2.start; // ����2 ����
        float denominator = segment1Vector.x * segment2Vector.y - segment1Vector.y * segment2Vector.x;
        // �� ������ ������ ���
        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            return null;
        }
        Vector2 vectorDistance = segment2.start - segment1.start;
        // ����1�� �Ķ����
        float segment1Parameter = (vectorDistance.x * segment2Vector.y - vectorDistance.y * segment2Vector.x) / denominator;
        // ����2�� �Ķ����
        float segment2Parameter = (vectorDistance.x * segment1Vector.y - vectorDistance.y * segment1Vector.x) / denominator;
        // ����1�� �Ķ���Ϳ� ����2�� �Ķ���Ͱ� [0, 1] ���� ���� ������ �� ������ ����
        if (segment1Parameter >= 0 && segment1Parameter <= 1 && segment2Parameter >= 0 && segment2Parameter <= 1)
        {
            return segment1.start + segment1Parameter * segment1Vector; // ������ ��ȯ
        }
        // �������� ����
        return null;
    }
}