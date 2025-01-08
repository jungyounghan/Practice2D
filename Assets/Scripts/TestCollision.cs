#if UNITY_EDITOR

using UnityEngine;

public class TestCollision : MonoBehaviour
{
    [SerializeField]
    private Boundary boundary;
    [SerializeField]
    private Vector2[] vertices;
    [SerializeField]
    private Collider2D[] collider2Ds;


    private void OnDrawGizmos()
    {
        Boundary sample = boundary + transform;
        sample.DrawSide();
        int length = collider2Ds != null ? collider2Ds.Length : 0;
        for (int i = 0; i < length; i++)
        {
            Boundary boundary = new Boundary(collider2Ds[i]);
            boundary.DrawSide();
        }
        length = vertices != null ? vertices.Length : 0;
        for (int i = 0; i < length; i++)
        {
            float halfSize = 0.005f;
            Vector2 dot1 = new Vector2(vertices[i].x + halfSize, vertices[i].y + halfSize);
            Vector2 dot2 = new Vector2(vertices[i].x - halfSize, vertices[i].y + halfSize);
            Vector2 dot3 = new Vector2(vertices[i].x - halfSize, vertices[i].y - halfSize);
            Vector2 dot4 = new Vector2(vertices[i].x + halfSize, vertices[i].y - halfSize);
            Debug.DrawLine(dot1, dot2, Color.yellow);
            Debug.DrawLine(dot2, dot3, Color.yellow);
            Debug.DrawLine(dot3, dot4, Color.yellow);
            Debug.DrawLine(dot4, dot1, Color.yellow);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바로 충돌 검사 실행
        {
            Debug.Log("클릭");
            Boundary sample = boundary + transform;
            int length = collider2Ds != null ? collider2Ds.Length: 0;
            for (int i = 0; i < length; i++)
            {
                bool isColliding = sample.Overlap(collider2Ds[i]);
                if(isColliding == true)
                {
                    Debug.Log(collider2Ds[i]);
                }
            }
            length = vertices != null ? vertices.Length : 0;
            for(int i = 0; i < length; i++)
            {
                if (sample.Overlap(vertices[i]) == true)
                {
                    Debug.Log(vertices[i]);
                }
            }
        }
    }
}
#endif