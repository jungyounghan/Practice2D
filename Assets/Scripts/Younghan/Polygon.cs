using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ٰ��� ����ü
/// </summary>
public struct Polygon
{
    //�⺻ ���� 1
    public Vector2 point1;
    //�⺻ ���� 2
    public Vector2 point2;
    //�⺻ ���� 3
    public Vector2 point3;
    //�߰� ������
    public Vector2[] otherPoints;

    //�������� ����Ʈȭ �����ִ� ������Ƽ
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
    /// Ư�� ���� �ٰ��� �ȿ� �ִ��� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="point"></param>
    /// <returns>Ư�� ���� �ٰ��� �ȿ� ������ �� ��ȯ</returns>
    public bool IsPointIn(Vector2 point)
    {
        List<Vector2> list = vertices;
        int count = list.Count;











        bool inside = false;
        // �ٰ����� �� ���п� ���� ���� ���θ� Ȯ��
        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            // ���� ������ y��ǥ ���� �ȿ� ���� ���� üũ
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