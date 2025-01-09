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
        bool overlap = false;
        int count = list.Count;
        for (int i = 0; i < count - 1; i++)
        {
            if (list[i] != list[i + 1])
            {
                Debug.DrawLine(list[i], list[i + 1], color, duration);
                overlap = false;
            }
            else
            {
                if (overlap == false)
                {
                    Debug.DrawLine(list[i], list[i + 1], color, duration);
                    overlap = true;
                }
                else
                {
                    Segment.Draw(list[i], color, duration);
                }
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

    /// <summary>
    /// ��, ��, ���� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static (Segment[], Polygon?) GetResult(Vector2[] vertices)
    {
        int length = vertices != null ? vertices.Length: 0;
        //��
        if (length > 1)
        {
            bool dot = true;
            List<Segment> segments = new List<Segment>();
            Polygon? polygon = null;
            for (int i = 0; i < length - 1; i++)
            {
                Segment segment = new Segment(vertices[i], vertices[i + 1]);
                if (segments.Contains(segment) == false && segments.Contains(segment.reverse) == false)
                {
                    if (vertices[i] != vertices[i + 1])
                    {
                        segments.Add(segment);
                        segment.Draw(Color.red);
                        if (dot == true)
                        {
                            segments.Remove(new Segment(vertices[i], vertices[i]));
                            dot = false;
                        }
                    }
                    else if (dot == true)
                    {
                        segments.Add(segment);
                    }
                }
            }
            int count = segments.Count;
            if(count >= 3)
            {
                if (segments[0].start == segments[count - 1].end)
                {
                    Vector2 point1 = segments[0].start;
                    Vector2 point2 = segments[0].end;
                    Vector2 point3 = segments[1].start;
                    List<Vector2> otherPoints = new List<Vector2>();
                    for(int i = 1; i < count - 1; i++)
                    {
                        otherPoints.Add(segments[i].end);
                        otherPoints.Add(segments[i + 1].start);
                    }
                    polygon = new Polygon(point1, point2, point3, otherPoints.ToArray());
                    segments.Clear();
                }
                else
                {
                    bool startInteraction = false;
                    List<Segment> startSegments = new List<Segment>();
                    for (int i = 0; i < count - 2; i++)
                    {
                        Segment? segment = null;
                        for(int j = count - 1; j >= i + 1; j--)
                        {
                            Vector2? interaction = Segment.GetIntersection(segments[i], segments[j]);
                            if (interaction != null)
                            {
                                Segment temp = new Segment(segments[i].start, interaction.Value);
                                if (segment == null || segments[i].distance > temp.distance)
                                {
                                    segment = temp;
                                }
                                if(startInteraction == false)
                                {
                                    startInteraction = true;
                                }
                            }
                        }
                        if(segment != null)
                        {
                            startSegments.Add(segment.Value);
                            break;
                        }
                        else
                        {
                            startSegments.Add(segments[i]);
                        }
                    }
                    bool endInteraction = false;
                    List<Segment> endSegments = new List<Segment>();
                    //for(int i = count - 1; )
                    //{

                    //}



                    //if (startInteraction == true)
                    {
                        for (int i = 0; i < startSegments.Count; i++)
                        {
                            startSegments[i].Draw(Color.green, 0);
                        }
                        //segments = startSegments;
                    }
                }
            }
            return (segments.Count > 0 ? segments.ToArray(): null, polygon);
        }
        //��
        else if(length > 0)
        {
            return (new Segment[] {new Segment(vertices[0], vertices[0])}, null);
        }
        //����
        else
        {
            return (null, null);
        }
    }
}