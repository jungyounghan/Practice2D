using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 위치를 지정할 수 있고 이동할 수 있고 뛸 수 있는 러너 클래스
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class Runner : MonoBehaviour, IPositionable, IMovable, IJumpable
{
    private static readonly Vector3 LeftRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 RightRotation = new Vector3(0, 0, 0);

    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform {
        get
        {
            if(_hasTransform == false)
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

    private bool _hasCapsuleCollider2D = false;

    private CapsuleCollider2D _capsuleCollider2D = null;

    protected CapsuleCollider2D getCapsuleCollider2D
    {
        get
        {
            if (_hasCapsuleCollider2D == false)
            {
                _hasCapsuleCollider2D = true;
                _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
            }
            return _capsuleCollider2D;
        }
    }

    private List<Collision2D> _leftCollision2D = new List<Collision2D>();

    private List<Collision2D> _rightCollision2D = new List<Collision2D>();

    //객체의 위치값을 이동시키거나 반환하는 프로퍼티
    public Vector2 position {
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
    private float _movingSpeed = 10;

    //점프력
    [SerializeField, Header("점프력"), Range(0, float.MaxValue)]
    protected float _jumpValue = 5;

    //최대 점프 한도
    [SerializeField, Header("최대 점프 한도"), Range(1, byte.MaxValue)]
    protected byte _jumpLimit = 1;

    //현재 점프 횟수
    private byte _jumpCount = 0;

#if UNITY_EDITOR

    //기즈모 표시 색깔
    [SerializeField, Header("기즈모 표시 색깔")]
    private Color _gizmoColor = Color.black;

    /// <summary>
    /// 유니티 에디터에서 바닥 충돌로 간주하는 범위를 선으로 그어준다.
    /// </summary>
    private void OnDrawGizmos()
    {
        Vector3 rotation = getTransform.rotation.eulerAngles;
        if (rotation == LeftRotation || rotation == RightRotation)
        {
            Handles.color = _gizmoColor;
            Bounds bounds = getCapsuleCollider2D.bounds;
            CapsuleDirection2D capsuleDirection2D = getCapsuleCollider2D.direction;
            float radius = capsuleDirection2D == CapsuleDirection2D.Vertical ? bounds.size.x * 0.5f : bounds.size.y * 0.5f;
            float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
            float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
            float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
            if (capsuleDirection2D == CapsuleDirection2D.Horizontal)
            {
                minX -= bounds.center.x - bounds.min.x - radius;
                maxX += bounds.max.x - bounds.center.x - radius;
            }
            Handles.DrawLine(new Vector2(minX, centerY), new Vector2(maxX, centerY));
            Handles.DrawLine(new Vector2(minX, centerY), new Vector2(minX, bounds.min.y));
            Handles.DrawLine(new Vector2(maxX, centerY), new Vector2(maxX, bounds.min.y));
            Handles.DrawLine(new Vector2(minX, bounds.min.y), new Vector2(maxX, bounds.min.y));
        }
    }
#endif

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 rotation = getTransform.rotation.eulerAngles;
        if (rotation == LeftRotation || rotation == RightRotation)
        {
            Bounds bounds = getCapsuleCollider2D.bounds;
            CapsuleDirection2D capsuleDirection2D = getCapsuleCollider2D.direction;
            float radius = capsuleDirection2D == CapsuleDirection2D.Vertical ? bounds.size.x * 0.5f : bounds.size.y * 0.5f;
            float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
            float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
            float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
            if (capsuleDirection2D == CapsuleDirection2D.Horizontal)
            {
                minX -= bounds.center.x - bounds.min.x - radius;
                maxX += bounds.max.x - bounds.center.x - radius;
            }
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector2 point = collision.contacts[i].point;
                if (point.x > minX && point.x < maxX && point.y < centerY)
                {
                    _jumpCount = _jumpLimit;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector3 rotation = getTransform.rotation.eulerAngles;
        if (rotation == LeftRotation || rotation == RightRotation)
        {
            if (_jumpCount < _jumpLimit && getRigidbody2D.velocity.y == 0 && (_leftCollision2D.Count > 0 || _rightCollision2D.Count > 0))
            {
                _jumpCount = _jumpLimit;
            }
            Bounds bounds = getCapsuleCollider2D.bounds;
            CapsuleDirection2D capsuleDirection2D = getCapsuleCollider2D.direction;
            float radius = capsuleDirection2D == CapsuleDirection2D.Vertical ? bounds.size.x * 0.5f : bounds.size.y * 0.5f;
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
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (getRigidbody2D.velocity.y < IJumpable.UnjumpableVelocity && _jumpLimit > 0 && _jumpCount == _jumpLimit)
        {
            _jumpCount--;
        }
        _leftCollision2D.Remove(collision);
        _rightCollision2D.Remove(collision);
    }

    //오른쪽으로 이동 시키는 메서드
    public virtual void MoveRight()
    {
        if (_rightCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(+_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    //왼쪽으로 이동 시키는 메서드
    public virtual void MoveLeft()
    {
        if (_leftCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(-_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    //이동을 중지 시키는 메서드
    public virtual void MoveStop()
    {
        getRigidbody2D.velocity = new Vector2(0, getRigidbody2D.velocity.y);
    }

    //점프를 하게 만드는 메서드
    public virtual void Jump()
    {
        if (_jumpCount > 0)
        {
            _jumpCount--;
            getRigidbody2D.velocity = new Vector2(0, _jumpValue);
        }
    }

    /// <summary>
    /// 점프를 할 수 있는지 판단하는 메서드
    /// </summary>
    /// <returns>점프가 가능하면 true를 반환함</returns>
    public bool CanJump()
    {
        if(_jumpCount > 0)
        {
            return true;
        }
        return false;
    }
}