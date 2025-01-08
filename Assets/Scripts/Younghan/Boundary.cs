using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 충돌체나 영역이 가지고 있는 정점들을 담고 있는 구조체
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
    /// 바운더리 내용과 특정 트랜스폼을 더해주면 트랜스폼 위치에 대응하는 바운더리를 반환한다
    /// </summary>
    /// <param name="boundary"></param>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Boundary operator +(Boundary boundary, Transform transform)
    {
        int length = boundary.vertices != null ? boundary.vertices.Length : 0;
        Vector2[] vectors = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            vectors[i] = (Vector2)transform.TransformPoint(boundary.vertices[i]);
        }
        return new Boundary(vectors);
    }

#if UNITY_EDITOR
    /// <summary>
    /// 점을 출력해주는 함수
    /// </summary>
    /// <param name="point"></param>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    private void DrawDot(Vector2 point, Color color, float duration)
    {
        float halfSize = 0.005f;
        Vector2 dot1 = new Vector2(point.x + halfSize, point.y + halfSize);
        Vector2 dot2 = new Vector2(point.x - halfSize, point.y + halfSize);
        Vector2 dot3 = new Vector2(point.x - halfSize, point.y - halfSize);
        Vector2 dot4 = new Vector2(point.x + halfSize, point.y - halfSize);
        Debug.DrawLine(dot1, dot2, color, duration);
        Debug.DrawLine(dot2, dot3, color, duration);
        Debug.DrawLine(dot3, dot4, color, duration);
        Debug.DrawLine(dot4, dot1, color, duration);
    }
#endif

    /// <summary>
    /// 특정 점이 선분 안에 있는지 확인
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="point"></param>
    /// <returns>점이 선분 안에 있으면 참 아니면 거짓 반환</returns>
    private bool IsPointIn(Vector2 a, Vector2 b, Vector2 point)
    {
        return Mathf.Abs((point - a).magnitude + (point - b).magnitude - (b - a).magnitude) < Mathf.Epsilon;
    }

    /// <summary>
    /// 특정 점이 다각형 안에 있는지 확인하는 함수
    /// </summary>
    /// <param name="polygon"></param>
    /// <param name="point"></param>
    /// <returns>점이 다각형 안에 있으면 참 아니면 거짓 반환</returns>
    private bool IsPointIn(List<Vector2> polygon, Vector2 point)
    {
        bool inside = false;
        int count = polygon.Count;
        // 다각형의 각 선분에 대해 교차 여부를 확인
        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            // 점이 선분의 y좌표 범위 안에 있을 때만 체크
            if ((polygon[i].y > point.y) != (polygon[j].y > point.y) && point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)
            {
                inside = !inside;
            }
        }
        return inside;
    }

    /// <summary>
    /// 교차점을 반환하는 함수
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="q1"></param>
    /// <param name="q2"></param>
    /// <returns>교차점이 있으면 Vector2를 반환</returns>
    private Vector2? GetIntersection((Vector2, Vector2) line1, (Vector2, Vector2) line2)
    {
        Vector2 size1 = line1.Item2 - line1.Item1; // 선분1 벡터
        Vector2 size2 = line2.Item2 - line2.Item1; // 선분2 벡터
        float denominator = size1.x * size2.y - size1.y * size2.x;
        // 두 선분이 평행한 경우
        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            return null;
        }
        Vector2 pq = line2.Item1 - line1.Item1;
        float t = (pq.x * size2.y - pq.y * size2.x) / denominator; // 선분1의 파라미터
        float u = (pq.x * size1.y - pq.y * size1.x) / denominator; // 선분2의 파라미터
        // t와 u가 [0, 1] 범위 내에 있으면 두 선분이 교차
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            return line1.Item1 + t * size1; // 교차점 반환
        }
        // 교차하지 않음
        return null;
    }

    /// <summary>
    /// 이 경계선이 가지고 있는 정점들을 에디터에서 표시해주는 함수
    /// </summary>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    public void DrawSide(float duration = 0)
    {
#if UNITY_EDITOR
        int length = vertices != null ? vertices.Length : 0;
        if (length > 0)
        {
            switch (length)
            {
                //점
                case 1:
                    DrawDot(vertices[0], Color.black, duration);
                    break;
                //선
                case 2:
                case 3:
                    for (int i = 0; i < length - 1; i++)
                    {
                        if (vertices[i] != vertices[i + 1])
                        {
                            Debug.DrawLine(vertices[i], vertices[i + 1], Color.red, duration);
                        }
                        else
                        {
                            DrawDot(vertices[i], Color.black, duration);
                        }
                    }
                    break;
                //면
                default:

                    List<Vector2> dots = new List<Vector2>();
                    List<(Vector2, Vector2)> lines = new List<(Vector2, Vector2)>();
                    List<List<Vector2>> polygons = new List<List<Vector2>>();
                    for (int i = 0; i < length - 1; i++)
                    {
                        //점이 아닐 때
                        if (vertices[i] != vertices[i + 1])
                        {
                            (Vector2, Vector2) line = new(vertices[i], vertices[i + 1]);
                            if (lines.Contains(line) == false && lines.Contains(new(line.Item2, line.Item1)) == false)
                            {
                                lines.Add(line);
                                int count = lines.Count;
                                if(count >= 3)
                                {
                                    List<Vector2> polygon = new List<Vector2>();
                                    for (int j = 0; j < count; j++)
                                    {
                                        Vector2? intersection = GetIntersection(lines[j], line);
                                        if (intersection.HasValue == true)
                                        {
                                            if (polygon.Contains(intersection.Value) == false)
                                            {
                                                polygon.Add(intersection.Value);
                                                polygon.Add(lines[j].Item2);
                                                Debug.DrawLine(intersection.Value, lines[j].Item2, Color.green);
                                            }
                                            else
                                            {
                                                polygons.Add(polygon);
                                                polygon.Clear();
                                            }
                                        }
                                        else if(polygon.Count > 0)
                                        {
                                            Debug.DrawLine(polygon[polygon.Count - 1], lines[j].Item2, Color.green);
                                            polygon.Add(lines[j].Item2);
                                        }
                                    }
                                }
                            }
                        }
                        //점일 때
                        else if (dots.Contains(vertices[i]) == false)
                        {
                            dots.Add(vertices[i]);
                        }
                    }
                    //점
                    //for (int i = 0; i < dots.Count; i++)
                    //{
                    //    DrawDot(vertices[i], Color.black, duration);
                    //}
                    //선
                    for (int i = 0; i < lines.Count; i++)
                    {
                        Debug.DrawLine(lines[i].Item1, lines[i].Item2, Color.red, duration);
                    }
                    //면
                    //for (int i = 0; i < polygons.Count; i++)
                    //{
                    //    int count = polygons[i].Count;
                    //    Debug.Log(count);
                    //    for (int j = 0; j < count - 1; i++)
                    //    {
                    //        Debug.DrawLine(polygons[i][j], polygons[i][j + 1], Color.green, duration);
                    //    }
                    //    Debug.DrawLine(polygons[i][count - 1], polygons[i][0], Color.green, duration);
                    //}
                    break;
            }
        }
#endif
    }

    /// <summary>
    /// 주어진 포인트가 이 다각형 내부에 포함되는지 확인
    /// </summary>
    /// <param name="point">검사할 Vector2 포인트</param>
    /// <returns>포인트가 내부에 있으면 참을 반환</returns>
    public bool Overlap(Vector2 point)
    {
        int length = vertices != null ? vertices.Length : 0;
        if(length > 0)
        {
            switch(length)
            {
                //점
                case 1:
                    return vertices[0] == point;
                //선
                case 2:
                case 3:
                    for (int i = 0; i < length; i++)
                    {
                        if (i > 0 && IsPointIn(vertices[i - 1], vertices[i], point) == true)
                        {
                            return true;
                        }
                    }
                    return false;
                //면
                default:
                    List<(Vector2, Vector2)> lines = new List<(Vector2, Vector2)>();
                    List<List<Vector2>> polygons = new List<List<Vector2>>();
                    for (int i = 0; i < length - 1; i++)
                    {
                        bool done = false;
                        int count = polygons.Count;
                        for(int j = 0; j < count; j++)
                        {
                            bool item1 = IsPointIn(polygons[j], vertices[i]);
                            bool item2 = IsPointIn(polygons[j], vertices[i + 1]);
                            //다각형 안에 있는 선은 무시함
                            if(item1 == true && item2 == true)
                            {
                                done = true;
                                break;
                            }
                            //다각형을 튀어나온 선은 밖에 있는 인접 선을 담음
                            else if(item1 != item2)
                            {
                                count = polygons[j].Count;
                                for (int k = 0; k < count - 1; k++)
                                {
                                    // 다각형의 각 선분을 가져온다
                                    Vector2 point1 = polygons[j][k];
                                    Vector2 point2 = polygons[j][k + 1];


                                }
                                done = true;
                                break;
                            }
                        }
                        if (done == false)
                        {
                            (Vector2, Vector2) line = new(vertices[i], vertices[i + 1]);
                        }
                        //다각형을 이루는 선들은 polygons에 담아줌, 검색 도중 다른 다각형이 완벽하게 내부에 있으면 그 작은 다각형 추가를 무시하고 약간 옆으로 튀어나왔으면 다각형을 확장함
                        //다각형을 이루지 못하는 각각의 선분들을 lines에 담아줌 중복되는 선분은 담지 않음
                    }
                    for (int i = 0; i < polygons.Count; i++)
                    {
                        if (IsPointIn(polygons[i], point) == true)
                        {
                            return true;
                        }
                    }
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (IsPointIn(lines[i].Item1, lines[i].Item2, point) == true)
                        {
                            return true;
                        }
                    }
                    break;
            }           
        }
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
            //1. 다각형의 점이 Collider 내부에 있는지 확인
            for (int i = 0; i < length; i++)
            {
                if (collider.OverlapPoint(vertices[i]) == true)
                {
                    return true;
                }
            }
            // 2. Collider의 점들 가져오기
            Boundary boundary = new Boundary(collider);
            // 2. 다각형 변과 Collider 경계 검사
            for (int i = 0; i < length; i++)
            {
                Vector2 vertexStart = vertices[i];
                Vector2 vertexEnd = vertices[(i + 1) % length];
                for (int j = 0; j < boundary.vertices.Length; j += 2)
                {
                    Vector2 colliderStart = boundary.vertices[j];
                    Vector2 colliderEnd = boundary.vertices[j + 1];
                    // 선분의 교차 여부를 확인
                    float det = (vertexEnd.x - vertexStart.x) * (colliderEnd.y - colliderStart.y) - (vertexEnd.y - vertexStart.y) * (colliderEnd.x - colliderStart.x);
                    if (Mathf.Abs(det) < Mathf.Epsilon)
                    {
                        continue; // 선분이 평행함
                    }
                    float t = ((colliderStart.x - vertexStart.x) * (colliderEnd.y - colliderStart.y) - (colliderStart.y - vertexStart.y) * (colliderEnd.x - colliderStart.x)) / det;
                    float u = ((colliderStart.x - vertexStart.x) * (vertexEnd.y - vertexStart.y) - (colliderStart.y - vertexStart.y) * (vertexEnd.x - vertexStart.x)) / det;
                    if (t >= 0 && t <= 1 && u >= 0 && u <= 1)// 교차 조건
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}