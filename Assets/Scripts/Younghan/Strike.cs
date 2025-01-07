using UnityEditor;
using UnityEngine;

/// <summary>
/// 대상을 타격하는 구조체
/// </summary>
public struct Strike
{
    //타격하는 강도
    private int power;

    //만약 타격 용도가 공격이라면 공격량을 확장하는 용도
    private byte extension;

    //최종적으로 대상에게 가해질 결과값을 반환한다.
    public int result
    {
        get
        {
            //power가 음수이면 공격을 위한 용도
            if (power < 0)
            {
                return power - Random.Range(0, extension);
            }
            //power가 양수이면 회복을 시키기 위한 용도
            else
            {
                return power;
            }
        }
    }

    //타격 대상에게 가해질 버프 & 디버프이다.
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
    /// 해당 대상이 타격 대상인지 구별하는 클래스(단 이 값이 null이라면 대상을 구별하지 않고 광역 타격)
    /// </summary>
    public abstract class Area
    {
#if UNITY_EDITOR
        private static readonly float DotSize = 0.01f;
        protected static readonly float DrawDuration = 3;

        protected static void DrawDot(Vector2 point, Color color, float duration)
        {
            float half = DotSize * 0.5f;
            Vector2 dot1 = new Vector2(point.x + half, point.y + half);
            Vector2 dot2 = new Vector2(point.x - half, point.y + half);
            Vector2 dot3 = new Vector2(point.x - half, point.y - half);
            Vector2 dot4 = new Vector2(point.x + half, point.y - half);
            Debug.DrawLine(dot1, dot2, color, duration);
            Debug.DrawLine(dot2, dot3, color, duration);
            Debug.DrawLine(dot3, dot4, color, duration);
            Debug.DrawLine(dot4, dot1, color, duration);
        }
#endif

        protected static Vector2[] GetColliderEdges(Collider2D collider)
        {

            return new Vector2[0];
        }

        public abstract void Show();

        public abstract bool CanStrike(IHittable hittable);
    }

    /// <summary>
    /// 특정 태그만 타격 대상으로 간주하는 클래스
    /// </summary>
    public class TagArea : Area
    {
        private string[] tags;

        public TagArea(string[] tags)
        {
            this.tags = tags;
        }


        public override void Show()
        {
#if UNITY_EDITOR
            int length = tags != null ? tags.Length : 0;
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    Debug.Log($"<color=red>타격 대상:{tags[i]}</color>");
                }
            }
            else
            {
                Debug.Log($"<color=black>타격 대상 없음</color>");
            }
#endif
        }

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
    /// 지정한 대상들만 타격 대상으로 간주하는 클래스
    /// </summary>
    public class TargetArea : Area
    {
        private IHittable[] hittables;

        public TargetArea(IHittable[] hittables)
        {
            this.hittables = hittables;
        }

        public override void Show()
        {
#if UNITY_EDITOR
            int length = hittables != null? hittables.Length : 0;
            for(int i = 0; i < length; i++)
            {
                Vector2[] vectors = GetColliderEdges(hittables[i].GetCollider2D());
                for (int j = 0; j < vectors.Length; j++)
                {
                    if (j > 0)
                    {
                        Debug.DrawLine(vectors[j - 1], vectors[j], Color.red, DrawDuration);
                        if (j == vectors.Length - 1)
                        {
                            Debug.DrawLine(vectors[j], vectors[0], Color.red, DrawDuration);
                        }
                    }
                }
            }
#endif
        }

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
    /// 특정 위치의 특정 태그 값을 가지면 타격 대상으로 간주하는 클래스
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

        public override void Show()
        {
#if UNITY_EDITOR
            int length = points != null ? points.Length : 0;
            if (length > 0)
            {
                if (length > 1)
                {
                    for (int i = 1; i < length; i++)
                    {
                        if (center + points[i - 1] != center + points[i])
                        {
                            Debug.DrawLine(center + points[i - 1], center + points[i], Color.red, DrawDuration);
                        }
                        else
                        {
                            DrawDot(center + points[i], Color.red, DrawDuration);
                        }
                    }
                }
                else if (length > 0)
                {
                    DrawDot(center + points[0], Color.red, DrawDuration);
                }
            }
            else
            {
                DrawDot(center, Color.red, DrawDuration);
            }
#endif
        }

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
                    // 1. Collider의 점들 가져오기
                    Vector2[] colliderEdges = GetColliderEdges(collider2D);
                    // 2. 다각형 변과 Collider 경계 검사
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
                                return true; // 교차하는 변이 존재
                            }
                        }
                    }
                    bool DoLinesIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
                    {
                        // 선분의 교차 여부를 확인
                        float det = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);
                        if (Mathf.Abs(det) < Mathf.Epsilon)
                        {
                            return false; // 선분이 평행함
                        }
                        float t = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / det;
                        float u = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / det;
                        return (t >= 0 && t <= 1 && u >= 0 && u <= 1); // 교차 조건
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