using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유저가 조종하게 되는 플레이어 클래스
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class Runner : MonoBehaviour
{
    private static readonly float OverlappingDistance = 0.02f;
    private static readonly float UnjumpableVelocity = -0.49f;

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

    [SerializeField]
    private float _movingSpeed = 10;
    [SerializeField]
    private float _jumpValue = 5;

    private bool _isGrounded = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bounds bounds = getCapsuleCollider2D.bounds;
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_isGrounded == false && getRigidbody2D.velocity.y == 0 && (_leftCollision2D.Count > 0 || _rightCollision2D.Count > 0))
        {
            _isGrounded = true;
        }
        Bounds bounds = getCapsuleCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (getRigidbody2D.velocity.y < UnjumpableVelocity && _isGrounded == true)
        {
            _isGrounded = false;
        }
        _leftCollision2D.Remove(collision);
        _rightCollision2D.Remove(collision);
    }

    public void MoveRight()
    {
        if (_rightCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(+_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    public void MoveLeft()
    {
        if (_leftCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(-_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    public void MoveStop()
    {
        getRigidbody2D.velocity = new Vector2(0, getRigidbody2D.velocity.y);
    }

    public void Jump()
    {
        if (_isGrounded == true)
        {
            _isGrounded = false;
            getRigidbody2D.velocity = new Vector2(0, _jumpValue);
        }
    }
}