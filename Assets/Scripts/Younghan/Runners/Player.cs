using System;
using UnityEngine;

/// <summary>
/// 유저가 조종하게 되는 플레이어 클래스
/// </summary>
public class Player : Runner, ILookable, IHittable
{
    [SerializeField]
    private Animator _animator = null;

    public bool isAlive
    {
        get;
        set;
    }


    private Action<Strike, Strike.Area, GameObject, IStrikeable> _reportAction = null;

    private void OnDrawGizmos()
    {
      
    }

    public void Initialize(Action<Strike, Strike.Area, GameObject, IStrikeable> reportAction)
    {
        _reportAction = reportAction;
    }

    public void Heal()
    {

    }

    public void Attack1()
    {
        Strike.PolygonArea polygonArea = new Strike.PolygonArea(position, null, null);
        polygonArea.Draw();
    }

    public void Attack2()
    {

    }

    public void Attack3()
    {

    }

    public void MoveUp()
    {

    }

    public void MoveDown()
    {

    }

    public void LookRight()
    {
    }

    public void LookLeft()
    {
    }

    public void Hit(Strike strike)
    {

    }

    public void Hit(Spell[] spells)
    {

    }

    public Collider2D GetCollider2D()
    {
        return getCapsuleCollider2D;
    }
}