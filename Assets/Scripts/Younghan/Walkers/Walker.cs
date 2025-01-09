using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 위치를 지정할 수 있고 이동할 수 있는 워커 클래스
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Walker : MonoBehaviour, IPositionable, IMovable
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform
    {
        get
        {
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    private bool _hasRigidbody2D = false;

    private Rigidbody2D _rigidbody2D = null;

    private Rigidbody2D getRigidbody2D
    {
        get
        {
            if (_hasRigidbody2D == false)
            {
                _hasRigidbody2D = true;
                _rigidbody2D = GetComponent<Rigidbody2D>();
                _rigidbody2D.freezeRotation = true;
            }
            return _rigidbody2D;
        }
    }

    private bool _hasCollider2D = false;

    private Collider2D _collider2D = null;

    protected Collider2D getCollider2D
    {
        get
        {
            if (_hasCollider2D == false)
            {
                _hasCollider2D = true;
                _collider2D = GetComponent<Collider2D>();
            }
            return _collider2D;
        }
    }

    private List<Collision2D> _leftCollision2D = new List<Collision2D>();

    private List<Collision2D> _rightCollision2D = new List<Collision2D>();

    //객체의 위치값을 이동시키거나 반환하는 프로퍼티
    public Vector2 position
    {
        get
        {
            return getTransform.position;
        }
        set
        {
            getTransform.position = value;
        }
    }

    //이동 속도
    [SerializeField, Header("이동 속도"), Range(0, float.MaxValue)]
    protected float _movingSpeed = 10;

    //땅에 착륙했는지 유무를 판단하는 프로퍼티
    private bool _isGrounded = false;

    public bool isGrounded
    {
        get
        {
            return _isGrounded;
        }
    }

#if UNITY_EDITOR

    //기즈모 표시 색깔
    [SerializeField, Header("기즈모 표시 색깔")]
    private Color _gizmoColor = Color.black;

    /// <summary>
    /// 유니티 에디터에서 바닥 충돌로 간주하는 범위를 선으로 그어준다.
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
        float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        Debug.DrawLine(new Vector2(minX, centerY), new Vector2(maxX, centerY), _gizmoColor);
        Debug.DrawLine(new Vector2(minX, centerY), new Vector2(minX, bounds.min.y), _gizmoColor);
        Debug.DrawLine(new Vector2(maxX, centerY), new Vector2(maxX, bounds.min.y), _gizmoColor);
        Debug.DrawLine(new Vector2(minX, bounds.min.y), new Vector2(maxX, bounds.min.y), _gizmoColor);
    }
#endif

    /// <summary>
    /// 바닥에 충돌하는지 여부를 확인하는 메서드
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
        float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 point = collision.contacts[i].point;
            if (point.x > minX && point.x < maxX && point.y < centerY)
            {
                _isGrounded = true;
            }
        }
    }

    /// <summary>
    /// 양쪽에 충돌체가 있는지 여부를 확인하는 메서드
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (_isGrounded == false && getRigidbody2D.velocity.y == 0 && (_leftCollision2D.Count > 0 || _rightCollision2D.Count > 0))
        {
            _isGrounded = true;
        }
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 point = collision.contacts[i].point;
            if (point.y >= centerY)
            {
                if (bounds.min.x - IMovable.OverlappingDistance < point.x && point.x < bounds.center.x && _leftCollision2D.Contains(collision) == false)
                {
                    _leftCollision2D.Add(collision);
                }
                if (bounds.center.x < point.x && point.x < bounds.max.x + IMovable.OverlappingDistance && _rightCollision2D.Contains(collision) == false)
                {
                    _rightCollision2D.Add(collision);
                }
            }
        }
    }

    /// <summary>
    /// 낙하나 양쪽 충돌체와 떨어졌는지 확인하는 메서드
    /// </summary>
    /// <param name="collision"></param>
    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (getRigidbody2D.velocity.y < IJumpable.UnjumpableVelocity && _isGrounded == true)
        {
            _isGrounded = false;
        }
        _leftCollision2D.Remove(collision);
        _rightCollision2D.Remove(collision);
    }

    /// <summary>
    /// 오른쪽으로 이동 시키는 메서드
    /// </summary>
    public virtual void MoveRight()
    {
        if (_rightCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(+_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    /// <summary>
    /// 왼쪽으로 이동 시키는 메서드
    /// </summary>
    public virtual void MoveLeft()
    {
        if (_leftCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(-_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    /// <summary>
    /// 이동을 중지 시키는 메서드
    /// </summary>
    public virtual void MoveStop()
    {
        getRigidbody2D.velocity = new Vector2(0, getRigidbody2D.velocity.y);
    }
}