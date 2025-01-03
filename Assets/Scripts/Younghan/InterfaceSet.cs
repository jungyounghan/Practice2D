using UnityEngine;

public interface IHittable<T> where T : MonoBehaviour
{
    public bool isAlive
    {
        get;
    }

    public void Set(Stat.Defencer defencer);

    //public void Hit();
}

public interface IWalkable
{
    public void MoveLeft();

    public void MoveRight();
}