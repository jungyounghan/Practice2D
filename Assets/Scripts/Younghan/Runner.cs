using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̵��ϰ� ���� �� �ְ� ������ �� �ִ� ���� Ŭ����
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class Runner : MonoBehaviour, IMovable, IWalkable, IJumpable
{
    private static readonly float OverlappingDistance = 0.02f;
    private static readonly float UnjumpableVelocity = -0.49f;

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

    private CapsuleCollider2D getCapsuleCollider2D
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

    //��ü�� ��ġ���� �̵���Ű�ų� ��ȯ�ϴ� ������Ƽ
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

    [SerializeField]
    private float _movingSpeed = 10;
    [SerializeField]
    private float _jumpValue = 5;

    private bool _isGrounded = false;

#if UNITY_EDITOR

    [SerializeField]
    private Color _gizmoColor = Color.black;

    /// <summary>
    /// ����Ƽ �����Ϳ��� �ٴ� �浹�� �����ϴ� ������ ������ �׾��ش�.
    /// </summary>
    private void OnDrawGizmos()
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
                Debug.DrawLine(new Vector2(bounds.min.x + radius, bounds.min.y + radius), new Vector2(minX, centerY), _gizmoColor);
                Debug.DrawLine(new Vector2(bounds.max.x - radius, bounds.min.y + radius), new Vector2(maxX, centerY), _gizmoColor);
                Debug.DrawLine(new Vector2(bounds.min.x + radius, bounds.min.y + radius), new Vector2(bounds.min.x + radius, bounds.min.y), _gizmoColor);
                Debug.DrawLine(new Vector2(bounds.max.x - radius, bounds.min.y + radius), new Vector2(bounds.max.x - radius, bounds.min.y), _gizmoColor);
            }
            else
            {
                Debug.DrawLine(new Vector2(bounds.center.x, bounds.min.y + radius), new Vector2(minX, centerY), _gizmoColor);
                Debug.DrawLine(new Vector2(bounds.center.x, bounds.min.y + radius), new Vector2(maxX, centerY), _gizmoColor);
                Debug.DrawLine(new Vector2(bounds.center.x, bounds.min.y + radius), new Vector2(bounds.center.x, bounds.min.y), _gizmoColor);
            }
            Debug.DrawLine(new Vector2(bounds.min.x, bounds.min.y + radius), new Vector2(bounds.max.x, bounds.min.y + radius), _gizmoColor);
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
                    _isGrounded = true;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector3 rotation = getTransform.rotation.eulerAngles;
        if (rotation == LeftRotation || rotation == RightRotation)
        {
            if (_isGrounded == false && getRigidbody2D.velocity.y == 0 && (_leftCollision2D.Count > 0 || _rightCollision2D.Count > 0))
            {
                _isGrounded = true;
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
                    if (bounds.min.x - OverlappingDistance < point.x && point.x < bounds.center.x && _leftCollision2D.Contains(collision) == false)
                    {
                        _leftCollision2D.Add(collision);
                    }
                    if (bounds.center.x < point.x && point.x < bounds.max.x + OverlappingDistance && _rightCollision2D.Contains(collision) == false)
                    {
                        _rightCollision2D.Add(collision);
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (getRigidbody2D.velocity.y < UnjumpableVelocity && _isGrounded == true)
        {
            _isGrounded = false;
        }
        _leftCollision2D.Remove(collision);
        _rightCollision2D.Remove(collision);
    }

    //���������� �̵� ��Ű�� �޼���
    public void MoveRight()
    {
        if (_rightCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(+_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    //�������� �̵� ��Ű�� �޼���
    public void MoveLeft()
    {
        if (_leftCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(-_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    //�̵��� ���� ��Ű�� �޼���
    public void MoveStop()
    {
        getRigidbody2D.velocity = new Vector2(0, getRigidbody2D.velocity.y);
    }

    //������ �ϰ� ����� �޼���
    public void Jump()
    {
        if (_isGrounded == true)
        {
            _isGrounded = false;
            getRigidbody2D.velocity = new Vector2(0, _jumpValue);
        }
    }
}