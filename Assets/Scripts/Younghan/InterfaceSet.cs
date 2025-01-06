using UnityEngine;

public interface IMovable
{
    Vector2 position {
        get;
        set;
    }
}

//오른쪽 또는 왼쪽으로 이동할 수 있는 객체들이 상속 받는 인터페이스
public interface IWalkable
{
    //오른쪽으로 이동
    public void MoveRight();
    //왼쪽으로 이동
    public void MoveLeft();
    //
    public void MoveStop();
}

//뛰어 오를 수 있는 객체들이 상속 받는 인터페이스
public interface IJumpable
{
    //뛰어 오르기
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
