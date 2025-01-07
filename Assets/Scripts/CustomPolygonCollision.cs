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
    }

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
        // EdgeCollider2D에서 경계 가져오기
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

        // PolygonCollider2D에서 경계 가져오기
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

        // 다른 Collider는 지원하지 않음
        Debug.LogWarning("Unsupported collider type.");
        return new Vector2[0];
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