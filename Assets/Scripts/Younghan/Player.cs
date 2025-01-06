using UnityEngine;

/// <summary>
/// 유저가 조종하게 되는 플레이어 클래스
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour, IWalkable, IJumpable
{
    private static readonly float OverlappingDistance = 0.02f;
    private static readonly float UnjumpableVelocity = -0.49f;
    private static readonly float ClimingVelocity = 1;

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

    private Rigidbody2D getRigidbody2D {
        get
        {
            if(_hasRigidbody2D == false)
            {
                _hasRigidbody2D = true;
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }
            return _rigidbody2D;
        }
    }

    private bool _hasCollider2D = false;

    private Collider2D _collider2D = null;

    private Collider2D getCollider2D
    {
        get
        {
            if(_hasCollider2D == false)
            {
                _hasCollider2D = true;
                _collider2D = GetComponent<Collider2D>();
            }
            return _collider2D;
        }
    }

    [SerializeField]
    private bool _leftBlocked = false;
    [SerializeField]
    private bool _rightBlocked = false;
    [SerializeField]
    private bool _isGrounded = false;

    [SerializeField]
    private float _movingSpeed = 10;
    [SerializeField]
    private float _jumpValue = 5;

    [SerializeField]
    private float _intervalSpeed = 0.06f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
        float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 point = collision.contacts[i].point;
            if(point.y >= centerY)
            {
                if (bounds.min.x - OverlappingDistance < point.x && point.x < bounds.center.x)
                {
                    _leftBlocked = true;
                }
                if (bounds.center.x < point.x && point.x < bounds.max.x + OverlappingDistance)
                {
                    _rightBlocked = true;
                }
            }
            else if (point.x > minX && point.x < maxX)
            {
                _isGrounded = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Vector2 velocity = getRigidbody2D.velocity;
        if (velocity.y < UnjumpableVelocity)
        {
            if (_isGrounded == true)
            {
                _isGrounded = false;
            }
            if (_leftBlocked == true && +_intervalSpeed < velocity.x)
            {
                _leftBlocked = false;
            }
            if (_rightBlocked == true && -_intervalSpeed > velocity.x)
            {
                _rightBlocked = false;
            }
        }
    }

    public void MoveRight()
    {
        if (_rightBlocked == false)
        {
            getTransform.position += new Vector3(_movingSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            getRigidbody2D.velocity += new Vector2(_movingSpeed * Time.deltaTime, 0);
            if(_rightBlocked == true && getRigidbody2D.velocity.x > +ClimingVelocity)
            {
                _rightBlocked = false;
            }
        }
        _leftBlocked = false;
    }

    public void MoveLeft()
    {
        if (_leftBlocked == false)
        {
            getTransform.position -= new Vector3(_movingSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            getRigidbody2D.velocity -= new Vector2(_movingSpeed * Time.deltaTime, 0);
            if (_leftBlocked == true && getRigidbody2D.velocity.x < -ClimingVelocity)
            {
                _leftBlocked = false;
            }
        }
        _rightBlocked = false;
    }

    public void Jump()
    {
        if (_isGrounded == true)
        {
            _isGrounded = false;
            getRigidbody2D.velocity = new Vector2(0, _jumpValue);
        }
    }

    public void Attack1()
    {

    }

    public void Attack2()
    {

    }

    public void Attack3()
    {

    }
}
