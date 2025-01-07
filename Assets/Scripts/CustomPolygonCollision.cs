using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomPolygonCollision : MonoBehaviour
{
    public Vector2[] customPolygon; // 사용자 정의 다각형의 꼭짓점 (로컬 좌표)
    public Collider2D targetCollider; // 충돌 대상 Collider2D

    [SerializeField]
    private Color color;

    [SerializeField]
    private float dot = 0.01f;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = color;
        Vector2[] worldPolygon = GetWorldPolygon(customPolygon, transform);
        int length = worldPolygon.Length;
        if(length > 1)
        {
            for(int i = 1; i < length; i++)
            {
                if (worldPolygon[i - 1] != worldPolygon[i])
                {
                    Handles.DrawLine(worldPolygon[i - 1], worldPolygon[i]);
                }
                else
                {
                    Handles.DotHandleCap(0, worldPolygon[i], Quaternion.identity, dot, EventType.Repaint); // 점 그리기
                }
            }
        }
        else if(length > 0)
        {
            Handles.DotHandleCap(0, worldPolygon[0], Quaternion.identity, dot, EventType.Repaint); // 점 그리기
        }
        if(targetCollider != null)
        {
            Vector2[] colliderEdges = GetColliderEdges(targetCollider);
            for (int j = 0; j < colliderEdges.Length; j++)
            {
                if (j > 0)
                {
                    Handles.DrawLine(colliderEdges[j - 1], colliderEdges[j]);
                }
            }
        }
    }
#endif

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바로 충돌 검사 실행
        {
            bool isColliding = CheckPolygonCollision(customPolygon, targetCollider);

            if (isColliding)
                Debug.Log("Custom polygon is colliding with the target collider!");
            else
                Debug.Log("No collision detected.");
        }
    }

    bool CheckPolygonCollision(Vector2[] localPolygon, Collider2D collider)
    {
        // 1. 다각형의 월드 좌표 계산
        Vector2[] worldPolygon = GetWorldPolygon(localPolygon, transform);

        // 2. 다각형의 점이 Collider 내부에 있는지 확인
        foreach (Vector2 point in worldPolygon)
        {
            if (collider.OverlapPoint(point))
            {
                return true; // 다각형 점이 Collider 내부에 있음
            }
        }

        // 3. 다각형 변(edge)과 Collider 경계의 교차 검사
        if (IsPolygonCollidingWithColliderEdges(worldPolygon, collider))
        {
            return true; // 다각형 변과 Collider 경계가 교차
        }

        return false; // 충돌 없음
    }

    Vector2[] GetWorldPolygon(Vector2[] localPolygon, Transform transform)
    {
        Vector2[] worldPolygon = new Vector2[localPolygon.Length];
        for (int i = 0; i < localPolygon.Length; i++)
        {
            worldPolygon[i] = transform.TransformPoint(localPolygon[i]);
        }
        return worldPolygon;
    }

    bool IsPolygonCollidingWithColliderEdges(Vector2[] polygon, Collider2D collider)
    {
        // 1. Collider의 점들 가져오기
        Vector2[] colliderEdges = GetColliderEdges(collider);

        // 2. 다각형 변과 Collider 경계 검사
        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 start = polygon[i];
            Vector2 end = polygon[(i + 1) % polygon.Length];

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

        return false; // 교차 없음
    }

    Vector2[] GetColliderEdges(Collider2D collider)
    {
        // BoxCollider2D에서 경계 가져오기
        if (collider is BoxCollider2D boxCollider)
        {
            Debug.Log("BoxCollider2D");
            Vector2 offset = boxCollider.offset;
            Vector2 size = boxCollider.size;

            Vector2[] localEdges = new Vector2[4]
            {
            offset + new Vector2(-size.x / 2, -size.y / 2),
            offset + new Vector2(size.x / 2, -size.y / 2),
            offset + new Vector2(size.x / 2, size.y / 2),
            offset + new Vector2(-size.x / 2, size.y / 2),
            };

            Vector2[] worldEdges = new Vector2[8];
            for (int i = 0; i < 4; i++)
            {
                worldEdges[i * 2] = boxCollider.transform.TransformPoint(localEdges[i]);
                worldEdges[i * 2 + 1] = boxCollider.transform.TransformPoint(localEdges[(i + 1) % 4]);
            }
            return worldEdges;
        }

        // CircleCollider2D에서 경계 가져오기
        if (collider is CircleCollider2D circleCollider)
        {
            Debug.Log("CircleCollider2D");
            int segments = 32; // 원을 근사할 세그먼트 수
            Vector2 offset = circleCollider.offset;
            float radius = circleCollider.radius;

            Vector2[] worldEdges = new Vector2[segments * 2];
            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * Mathf.PI * 2 / segments;
                float angle2 = (i + 1) * Mathf.PI * 2 / segments;

                Vector2 point1 = new Vector2(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius) + offset;
                Vector2 point2 = new Vector2(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius) + offset;

                worldEdges[i * 2] = circleCollider.transform.TransformPoint(point1);
                worldEdges[i * 2 + 1] = circleCollider.transform.TransformPoint(point2);
            }
            return worldEdges;
        }

        // CapsuleCollider2D에서 경계 가져오기
        if (collider is CapsuleCollider2D capsuleCollider)
        {
            Debug.Log("CapsuleCollider2D");
            int segments = 16; // 반원을 근사할 세그먼트 수
            Vector2 offset = capsuleCollider.offset;
            Vector2 size = capsuleCollider.size;

            List<Vector2> localEdges = new List<Vector2>();
            if (capsuleCollider.direction == CapsuleDirection2D.Vertical)
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
            else
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

            Vector2[] worldEdges = new Vector2[localEdges.Count * 2];
            for (int i = 0; i < localEdges.Count; i++)
            {
                worldEdges[i * 2] = capsuleCollider.transform.TransformPoint(localEdges[i]);
                worldEdges[i * 2 + 1] = capsuleCollider.transform.TransformPoint(localEdges[(i + 1) % localEdges.Count]);
            }
            return worldEdges;
        }

        // EdgeCollider2D에서 경계 가져오기
        if (collider is EdgeCollider2D edgeCollider)
        {
            Vector2[] points = edgeCollider.points; // 로컬 좌표
            float edgeRadius = edgeCollider.edgeRadius;

            Vector2 offset = edgeCollider.offset;
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
            return worldEdges.ToArray();
        }

        // PolygonCollider2D에서 경계 가져오기
        if (collider is PolygonCollider2D polygonCollider)
        {
            Debug.Log("PolygonCollider2D");
            int totalPoints = 0;
            for (int i = 0; i < polygonCollider.pathCount; i++)
                totalPoints += polygonCollider.GetPath(i).Length;

            Vector2 offset = polygonCollider.offset;
            Vector2[] worldEdges = new Vector2[totalPoints * 2];
            int edgeIndex = 0;

            for (int i = 0; i < polygonCollider.pathCount; i++)
            {
                Vector2[] path = polygonCollider.GetPath(i);
                for (int j = 0; j < path.Length; j++)
                {
                    int nextIndex = (j + 1) % path.Length;
                    worldEdges[edgeIndex++] = (Vector2)polygonCollider.transform.TransformPoint(path[j]) + offset;
                    worldEdges[edgeIndex++] = (Vector2)polygonCollider.transform.TransformPoint(path[nextIndex]) + offset;
                }
            }
            return worldEdges;
        }

        

        

        return new Vector2[0]; // 지원되지 않는 Collider2D 타입
    }

    bool DoLinesIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        // 선분의 교차 여부를 확인
        float det = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);
        if (Mathf.Abs(det) < Mathf.Epsilon)
            return false; // 선분이 평행함

        float t = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / det;
        float u = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / det;

        return (t >= 0 && t <= 1 && u >= 0 && u <= 1); // 교차 조건
    }
}