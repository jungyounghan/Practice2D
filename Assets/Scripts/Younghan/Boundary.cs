using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 충돌 영역을 판단하는 정점들을 담고 있는 구조체
/// </summary>
[Serializable]
public struct Boundary
{
    //정점들
    [SerializeField, Header("정점들")]
    private Vector2[] vertices;

    public Boundary(Vector2[] vertices)
    {
        this.vertices = vertices;
    }

    public Boundary(Collider2D collider)
    {
        // BoxCollider2D에서 경계 가져오기
        if (collider is BoxCollider2D boxCollider)
        {
            Vector2 offset = boxCollider.offset;
            Vector2 size = boxCollider.size;
            Vector2[] localEdges = new Vector2[4]
            {
                offset + new Vector2(-size.x / 2, -size.y / 2),
                offset + new Vector2(size.x / 2, -size.y / 2),
                offset + new Vector2(size.x / 2, size.y / 2),
                offset + new Vector2(-size.x / 2, size.y / 2),
            };
            vertices = new Vector2[8];
            for (int i = 0; i < 4; i++)
            {
                vertices[i * 2] = boxCollider.transform.TransformPoint(localEdges[i]);
                vertices[i * 2 + 1] = boxCollider.transform.TransformPoint(localEdges[(i + 1) % 4]);
            }
        }
        // CircleCollider2D에서 경계 가져오기
        else if (collider is CircleCollider2D circleCollider)
        {
            int segments = 32; // 원을 근사할 세그먼트 수
            float radius = circleCollider.radius;
            Vector2 offset = circleCollider.offset;
            vertices = new Vector2[segments * 2];
            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * Mathf.PI * 2 / segments;
                float angle2 = (i + 1) * Mathf.PI * 2 / segments;
                Vector2 point1 = new Vector2(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius) + offset;
                Vector2 point2 = new Vector2(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius) + offset;
                vertices[i * 2] = circleCollider.transform.TransformPoint(point1);
                vertices[i * 2 + 1] = circleCollider.transform.TransformPoint(point2);
            }
        }
        // CapsuleCollider2D에서 경계 가져오기
        else if (collider is CapsuleCollider2D capsuleCollider)
        {
            int segments = 16; // 반원을 근사할 세그먼트 수
            Vector2 offset = capsuleCollider.offset;
            Vector2 size = capsuleCollider.size;
            List<Vector2> localEdges = new List<Vector2>();
            switch (capsuleCollider.direction)
            {
                case CapsuleDirection2D.Vertical:
                    {
                        float radius = size.x / 2;
                        float height = size.y - size.x;
                        // 아래쪽 반원
                        for (int i = 0; i <= segments; i++)
                        {
                            float angle = Mathf.PI + Mathf.PI * i / segments;
                            localEdges.Add(new Vector2(radius * Mathf.Cos(angle), -height / 2 + radius * Mathf.Sin(angle)) + offset);
                        }
                        // 위쪽 반원
                        for (int i = 0; i <= segments; i++)
                        {
                            float angle = Mathf.PI * i / segments;
                            localEdges.Add(new Vector2(radius * Mathf.Cos(angle), height / 2 + radius * Mathf.Sin(angle)) + offset);
                        }
                    }
                    break;
                case CapsuleDirection2D.Horizontal:
                    {
                        float radius = size.y / 2;
                        float width = size.x - size.y;
                        // 왼쪽 반원
                        for (int i = 0; i <= segments; i++)
                        {
                            float angle = Mathf.PI / 2 + Mathf.PI * i / segments;
                            localEdges.Add(new Vector2(-width / 2 + radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)) + offset);
                        }
                        // 오른쪽 반원
                        for (int i = 0; i <= segments; i++)
                        {
                            float angle = -Mathf.PI / 2 + Mathf.PI * i / segments;
                            localEdges.Add(new Vector2(width / 2 + radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)) + offset);
                        }
                    }
                    break;
            }
            vertices = new Vector2[localEdges.Count * 2];
            for (int i = 0; i < localEdges.Count; i++)
            {
                vertices[i * 2] = capsuleCollider.transform.TransformPoint(localEdges[i]);
                vertices[i * 2 + 1] = capsuleCollider.transform.TransformPoint(localEdges[(i + 1) % localEdges.Count]);
            }
        }
        // EdgeCollider2D에서 경계 가져오기
        else if (collider is EdgeCollider2D edgeCollider)
        {
            float edgeRadius = edgeCollider.edgeRadius;
            Vector2 offset = edgeCollider.offset;
            Vector2[] points = edgeCollider.points; // 로컬 좌표
            List<Vector2> worldEdges = new List<Vector2>();
            for (int i = 0; i < points.Length; i++)
            {
                // 점을 월드 좌표로 변환
                Vector2 worldPoint = edgeCollider.transform.TransformPoint(points[i]);
                // 점의 Edge Radius를 고려한 원 경계 추가
                if (edgeRadius > 0)
                {
                    int segments = 16; // 원을 근사할 세그먼트 수
                    for (int j = 0; j < segments; j++)
                    {
                        float angle = Mathf.PI * 2 * j / segments;
                        worldEdges.Add(offset + worldPoint + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * edgeRadius);
                    }
                }
                // 선분 처리
                if (i < points.Length - 1)
                {
                    Vector2 nextWorldPoint = edgeCollider.transform.TransformPoint(points[i + 1]);
                    if (edgeRadius > 0)
                    {
                        // 엣지 방향과 수직 벡터 계산
                        Vector2 direction = (nextWorldPoint - worldPoint).normalized;
                        Vector2 perpendicular = new Vector2(-direction.y, direction.x) * edgeRadius;
                        // 상단, 하단 경계를 추가
                        worldEdges.Add(offset + worldPoint + perpendicular);
                        worldEdges.Add(offset + nextWorldPoint + perpendicular);
                        worldEdges.Add(offset + nextWorldPoint - perpendicular);
                        worldEdges.Add(offset + worldPoint - perpendicular);
                    }
                    else
                    {
                        // Edge Radius가 없는 경우, 단순히 선분만 추가
                        worldEdges.Add(offset + worldPoint);
                        worldEdges.Add(offset + nextWorldPoint);
                    }
                }
            }
            vertices = worldEdges.ToArray();
        }
        // PolygonCollider2D에서 경계 가져오기
        else if (collider is PolygonCollider2D polygonCollider)
        {
            int totalPoints = 0;
            for (int i = 0; i < polygonCollider.pathCount; i++)
            {
                totalPoints += polygonCollider.GetPath(i).Length;
            }
            int edgeIndex = 0;
            Vector2 offset = polygonCollider.offset;
            vertices = new Vector2[totalPoints * 2];
            for (int i = 0; i < polygonCollider.pathCount; i++)
            {
                Vector2[] path = polygonCollider.GetPath(i);
                for (int j = 0; j < path.Length; j++)
                {
                    int nextIndex = (j + 1) % path.Length;
                    vertices[edgeIndex++] = (Vector2)polygonCollider.transform.TransformPoint(path[j]) + offset;
                    vertices[edgeIndex++] = (Vector2)polygonCollider.transform.TransformPoint(path[nextIndex]) + offset;
                }
            }
        }
        else
        {
            vertices = null;
        }
    }

    /// <summary>
    /// 바운더리와 트랜스폼 위치를 더해주는 함수
    /// </summary>
    /// <param name="boundary"></param>
    /// <param name="transform"></param>
    /// <returns>트랜스폼 위치에 대응하는 바운더리를 반환한다</returns>
    public static Boundary operator +(Boundary boundary, Transform transform)
    {
        int length = boundary.vertices != null ? boundary.vertices.Length : 0;
        Vector2[] vectors = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            vectors[i] = transform.TransformPoint(boundary.vertices[i]);
        }
        return new Boundary(vectors);
    }

    /// <summary>
    /// 이 경계선이 가지고 있는 정점들을 에디터에서 표시해주는 함수
    /// </summary>
    /// <param name="duration"></param>
    public void Draw(float duration = 0)
    {
#if UNITY_EDITOR
        (Segment[], Polygon?) result = Polygon.GetResult(vertices);
        //면
        if(result.Item2 != null)
        {
            result.Item2.Value.Draw(Color.green, duration);
        }
        int length = result.Item1 != null ? result.Item1.Length : 0;
        for (int i = 0; i < length; i++)
        {
            //result.Item1[i].Draw(Color.red, duration);
        }
#endif
    }

    /// <summary>
    /// 주어진 포인트가 이 영역 내부에 포함되는지 확인
    /// </summary>
    /// <param name="point"></param>
    /// <returns>포인트와 겹치면 참을 반환</returns>
    public bool Overlap(Vector2 point)
    {
        (Segment[], Polygon?) result = Polygon.GetResult(vertices);
        int length = result.Item1 != null ? result.Item1.Length : 0;
        for (int i = 0; i < length; i++)
        {
            if (result.Item1[i].IsPointIn(point) == true)
            {
                return true;
            }
        }
        if (result.Item2 != null && result.Item2.Value.IsPointIn(point) == true)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 경계면 간이 겹치는지 여부를 알려주는 메서드
    /// </summary>
    /// <param name="boundary"></param>
    /// <returns>경계면과 겹치면 참을 반환</returns>
    public bool Overlap(Boundary boundary)
    {
        // 3. 다각형 변과 Collider 경계 검사
        //for (int i = 0; i < length; i++)
        //{
        //    Vector2 vertexStart = vertices[i];
        //    Vector2 vertexEnd = vertices[(i + 1) % length];
        //    for (int j = 0; j < boundary.vertices.Length; j += 2)
        //    {
        //        Vector2 colliderStart = boundary.vertices[j];
        //        Vector2 colliderEnd = boundary.vertices[j + 1];
        //        // 선분의 교차 여부를 확인
        //        float det = (vertexEnd.x - vertexStart.x) * (colliderEnd.y - colliderStart.y) - (vertexEnd.y - vertexStart.y) * (colliderEnd.x - colliderStart.x);
        //        if (Mathf.Abs(det) < Mathf.Epsilon)
        //        {
        //            continue; // 선분이 평행함
        //        }
        //        float t = ((colliderStart.x - vertexStart.x) * (colliderEnd.y - colliderStart.y) - (colliderStart.y - vertexStart.y) * (colliderEnd.x - colliderStart.x)) / det;
        //        float u = ((colliderStart.x - vertexStart.x) * (vertexEnd.y - vertexStart.y) - (colliderStart.y - vertexStart.y) * (vertexEnd.x - vertexStart.x)) / det;
        //        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)// 교차 조건
        //        {
        //            return true;
        //        }
        //    }
        //}
        return false;
    }

    /// <summary>
    /// 콜라이더와 겹치는지 여부를 알려주는 메서드
    /// </summary>
    /// <param name="collider"></param>
    /// <returns>콜라이더와 겹치면 참을 반환</returns>
    public bool Overlap(Collider2D collider)
    {
        if (collider != null)
        {
            int length = vertices != null ? vertices.Length : 0;
            for (int i = 0; i < length; i++)
            {
                if (collider.OverlapPoint(vertices[i]) == true)
                {
                    return true;
                }
            }
            if(Overlap(new Boundary(collider)) == true)
            {
                return true;
            }
        }
        return false;
    }
}