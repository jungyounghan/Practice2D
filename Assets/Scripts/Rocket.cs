using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class Rocket : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform;

    private Transform getTransform
    {
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

    private bool _hasRigidbody = false;

    private Rigidbody2D _rigidbody;

    private Rigidbody2D getRigidbody
    {
        get
        {
            if(_hasRigidbody == false)
            {
                _hasRigidbody = true;
                _rigidbody = GetComponent<Rigidbody2D>();
            }
            return _rigidbody;
        }
    }

    // 업데이트는 프레임당 한 번 호출된다
    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if(Input.GetKey(KeyCode.A))
        {
            getRigidbody.AddForce(new Vector2(-1, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            getRigidbody.AddForce(new Vector2(+1, 0));
        }
        if (Input.GetKey(KeyCode.W))
        {
        }
        if (Input.GetKey(KeyCode.S))
        {
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("충돌");
    }
}