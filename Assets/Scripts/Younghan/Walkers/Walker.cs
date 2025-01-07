using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 위치를 지정할 수 있고 이동할 수 있는 워커 클래스
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

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
    private float _movingSpeed = 10;

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
}