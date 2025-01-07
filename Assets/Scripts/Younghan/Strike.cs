using UnityEditor;
using UnityEngine;

/// <summary>
/// ����� Ÿ���ϴ� ����ü
/// </summary>
public struct Strike
{
    //Ÿ���ϴ� ����
    private int power;

    //���� Ÿ�� �뵵�� �����̶�� ���ݷ��� Ȯ���ϴ� �뵵
    private byte extension;

    //���������� ��󿡰� ������ ������� ��ȯ�Ѵ�.
    public int result
    {
        get
        {
            //power�� �����̸� ������ ���� �뵵
            if (power < 0)
            {
                return power - Random.Range(0, extension);
            }
            //power�� ����̸� ȸ���� ��Ű�� ���� �뵵
            else
            {
                return power;
            }
        }
    }

    //Ÿ�� ��󿡰� ������ ���� & ������̴�.
    public Spell[] spells {
        private set;
        get;
    }

    public Strike(int power, byte extension, Spell[] spells = null)
    {
        this.power = power;
        this.extension = extension;
        this.spells = spells;
    }

    /// <summary>
    /// �ش� ����� Ÿ�� ������� �����ϴ� Ŭ����(�� �� ���� null�̶�� ����� �������� �ʰ� ���� Ÿ��)
    /// </summary>
    public abstract class Area
    {
#if UNITY_EDITOR
        protected static readonly float DotSize = 0.01f;
        protected static readonly float DrawDuration = 3;

        protected static void DrawDot(Vector2 point)
        {
            Debug.DrawLine(point, point, Color.red, DrawDuration);
        }

        public abstract void Draw();
#endif
        public abstract bool CanStrike(IHittable hittable);

    }

    /// <summary>
    /// Ư�� �±׸� Ÿ�� ������� �����ϴ� Ŭ����
    /// </summary>
    public class TagArea : Area
    {
        private string[] tags;

        public TagArea(string[] tags)
        {
            this.tags = tags;
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            int length = tags != null ? tags.Length : 0;
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    Debug.Log($"<color=red>Ÿ�� ���:{tags[i]}</color>");
                }
            }
            else
            {
                Debug.Log($"<color=black>Ÿ�� ��� ����</color>");
            }
        }
#endif

        public override bool CanStrike(IHittable hittable)
        {
            if (hittable != null)
            {
                int length = tags != null ? tags.Length : 0;
                for (int i = 0; i < length; i++)
                {
                    if(hittable.tag == tags[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// ������ ���鸸 Ÿ�� ������� �����ϴ� Ŭ����
    /// </summary>
    public class TargetArea : Area
    {
        private IHittable[] hittables;

        public TargetArea(IHittable[] hittables)
        {
            this.hittables = hittables;
        }


#if UNITY_EDITOR
        public override void Draw()
        {
            int length = hittables != null? hittables.Length : 0;
            for(int i = 0; i < length; i++)
            {
                Handles.DotHandleCap(0, hittables[i].position, Quaternion.identity, DotSize, EventType.Repaint);
            }
        }
#endif

        public override bool CanStrike(IHittable hittable)
        {
            if (hittable != null)
            {
                int length = hittables.Length;
                for (int i = 0; i < length; i++)
                {
                    if (hittable == hittables[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Ư�� ��ġ�� Ư�� �±� ���� ������ Ÿ�� ������� �����ϴ� Ŭ����
    /// </summary>
    public class PolygonArea : TagArea
    {
        private Vector2 center;

        private Vector2[] points;

        public PolygonArea(Vector2 center, Vector2[] points, string[] tags) : base(tags)
        {
            this.center = center;
            this.points = points;
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            Handles.color = Color.red;
            int length = points != null ? points.Length : 0;
            if (length > 0)
            {
                if (length > 1)
                {
                    for (int i = 1; i < length; i++)
                    {
                        if (center + points[i - 1] != center + points[i])
                        {
                            Handles.DrawLine(center + points[i - 1], center + points[i]);
                        }
                        else
                        {
                            Handles.DotHandleCap(0, center + points[i], Quaternion.identity, DotSize, EventType.Repaint);
                        }
                    }
                }
                else if (length > 0)
                {
                    Handles.DotHandleCap(0, center + points[0], Quaternion.identity, DotSize, EventType.Repaint);
                }
            }
            else
            {
                Handles.DotHandleCap(0, center, Quaternion.identity, DotSize, EventType.Repaint);
            }
        }
#endif

        public override bool CanStrike(IHittable hittable)
        {
            if (base.CanStrike(hittable) == true)
            {
                Collider2D collider2D = hittable.GetCollider2D();
                int length = points != null ? points.Length : 0;
                if (length > 0)
                {
                    Vector2[] polygon = new Vector2[length];
                    for(int i = 0; i < length; i++)
                    {
                        polygon[i] += center;
                        if (collider2D.OverlapPoint(polygon[i]))
                        {
                            return true;
                        }
                    }
                    // 1. Collider�� ���� ��������
                    Vector2[] colliderEdges = GetColliderEdges(collider2D);
                    // 2. �ٰ��� ���� Collider ��� �˻�
                    for (int i = 0; i < length; i++)
                    {
                        Vector2 start = polygon[i];
                        Vector2 end = polygon[(i + 1) % length];

                        for (int j = 0; j < colliderEdges.Length; j += 2)
                        {
                            Vector2 colliderStart = colliderEdges[j];
                            Vector2 colliderEnd = colliderEdges[j + 1];
                            if (DoLinesIntersect(start, end, colliderStart, colliderEnd))
                            {
                                return true; // �����ϴ� ���� ����
                            }
                        }
                    }
                    bool DoLinesIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
                    {
                        // ������ ���� ���θ� Ȯ��
                        float det = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);
                        if (Mathf.Abs(det) < Mathf.Epsilon)
                        {
                            return false; // ������ ������
                        }
                        float t = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / det;
                        float u = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / det;
                        return (t >= 0 && t <= 1 && u >= 0 && u <= 1); // ���� ����
                    }
                    Vector2[] GetColliderEdges(Collider2D collider)
                    {
                        // EdgeCollider2D���� ��� ��������
                        if (collider is EdgeCollider2D edgeCollider)
                        {
                            Vector2[] points = edgeCollider.points;
                            Vector2[] worldEdges = new Vector2[points.Length * 2];
                            for (int i = 0; i < points.Length - 1; i++)
                            {
                                worldEdges[i * 2] = edgeCollider.transform.TransformPoint(points[i]);
                                worldEdges[i * 2 + 1] = edgeCollider.transform.TransformPoint(points[i + 1]);
                            }
                            return worldEdges;
                        }
                        // PolygonCollider2D���� ��� ��������
                        if (collider is PolygonCollider2D polygonCollider)
                        {
                            Vector2[] points = polygonCollider.points;
                            Vector2[] worldEdges = new Vector2[points.Length * 2];
                            for (int i = 0; i < points.Length - 1; i++)
                            {
                                worldEdges[i * 2] = polygonCollider.transform.TransformPoint(points[i]);
                                worldEdges[i * 2 + 1] = polygonCollider.transform.TransformPoint(points[i + 1]);
                            }
                            return worldEdges;
                        }
                        return new Vector2[0];
                    }
                }
                else
                {
                    return collider2D.OverlapPoint(center);
                }
            }
            return false;
        }
    }
}