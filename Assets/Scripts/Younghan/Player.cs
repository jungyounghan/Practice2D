using UnityEngine;

/// <summary>
/// ������ �����ϰ� �Ǵ� �÷��̾� Ŭ����
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IWalkable, IJumpable
{
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

    public void MoveRight()
    {
        getRigidbody2D.AddForce(new Vector2(+3, 0));
    }

    public void MoveLeft()
    {
        getRigidbody2D.AddForce(new Vector2(-3, 0));
    }

    public void Jump()
    {

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
