using UnityEditor;
using UnityEngine;

public class CustomPolygonCollision : MonoBehaviour
{
    public Vector2[] customPolygon; // ����� ���� �ٰ����� ������ (���� ��ǥ)
    public Collider2D targetCollider; // �浹 ��� Collider2D

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
                    Handles.DotHandleCap(0, worldPolygon[i], Quaternion.identity, dot, EventType.Repaint); // �� �׸���
                }
            }
        }
        else if(length > 0)
        {
            Handles.DotHandleCap(0, worldPolygon[0], Quaternion.identity, dot, EventType.Repaint); // �� �׸���
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // �����̽��ٷ� �浹 �˻� ����
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
        // 1. �ٰ����� ���� ��ǥ ���
        Vector2[] worldPolygon = GetWorldPolygon(localPolygon, transform);

        // 2. �ٰ����� ���� Collider ���ο� �ִ��� Ȯ��
        foreach (Vector2 point in worldPolygon)
        {
            if (collider.OverlapPoint(point))
            {
                return true; // �ٰ��� ���� Collider ���ο� ����
            }
        }

        // 3. �ٰ��� ��(edge)�� Collider ����� ���� �˻�
        if (IsPolygonCollidingWithColliderEdges(worldPolygon, collider))
        {
            return true; // �ٰ��� ���� Collider ��谡 ����
        }

        return false; // �浹 ����
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
        // 1. Collider�� ���� ��������
        Vector2[] colliderEdges = GetColliderEdges(collider);

        // 2. �ٰ��� ���� Collider ��� �˻�
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
                    return true; // �����ϴ� ���� ����
                }
            }
        }

        return false; // ���� ����
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

        // �ٸ� Collider�� �������� ����
        Debug.LogWarning("Unsupported collider type.");
        return new Vector2[0];
    }

    bool DoLinesIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        // ������ ���� ���θ� Ȯ��
        float det = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);
        if (Mathf.Abs(det) < Mathf.Epsilon)
            return false; // ������ ������

        float t = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / det;
        float u = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / det;

        return (t >= 0 && t <= 1 && u >= 0 && u <= 1); // ���� ����
    }
}