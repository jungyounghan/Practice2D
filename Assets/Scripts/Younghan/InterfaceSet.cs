using UnityEngine;

//������ �Ǵ� �������� �̵��� �� �ִ� ��ü���� ��� �޴� �������̽�
public interface IWalkable
{
    //���������� �̵�
    public void MoveRight();
    //�������� �̵�
    public void MoveLeft();
}

//�پ� ���� �� �ִ� ��ü���� ��� �޴� �������̽�
public interface IJumpable
{
    //�پ� ������
    public void Jump();
}

public interface IHittable<T> where T : MonoBehaviour
{
    public bool isAlive {
        get;
    }

    //public void Set(Stat.Defencer defencer);

    //public void Hit();
}
