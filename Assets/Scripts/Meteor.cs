using UnityEngine;

public class Meteor : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform;

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

    private static readonly float MinSpeed = 1;
    private static readonly float MaxSpeed = 5;
    private static readonly float MiddlePoint = 7f;

    private float _moveSpeed;

    private Vector2 _position;

    // 시작 메서드는 첫 번째 프레임 업데이트 전에 호출된다
    private void Start()
    {
        _position = getTransform.position;
        _moveSpeed = Random.Range(MinSpeed, MaxSpeed);
    }

    // 업데이트는 프레임당 한 번 호출된다
    private void Update()
    {
        float deltaTime = Time.deltaTime;
        _position.y -= _moveSpeed * deltaTime;
        if(_position.y < -MiddlePoint)
        {
            _position.y += MiddlePoint * 2;
        }
        getTransform.position = _position;
    }
}
