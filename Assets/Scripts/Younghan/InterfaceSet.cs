using UnityEngine;

/// <summary>
/// 위치 지정이 가능한 객체들이 상속 받는 인터페이스
/// </summary>
public interface IPositionable
{
    //위치 지정이 가능한 프로퍼티
    Vector2 position {
        get;
        set;
    }
}

/// <summary>
/// 한 방향으로 바라볼 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface ILookable
{
    //오른쪽으로 바라봄
    public void LookRight();

    //왼쪽으로 바라봄
    public void LookLeft();
}

/// <summary>
/// 오른쪽 또는 왼쪽으로 이동할 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IMovable
{
    //다른 물체와 충돌 시 유효 거리
    public static readonly float OverlappingDistance = 0.02f;
    //오른쪽으로 이동
    public void MoveRight();
    //왼쪽으로 이동
    public void MoveLeft();
    //이동 멈춤
    public void MoveStop();
}

/// <summary>
/// 뛰어 오를 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IJumpable
{
    //낙하 시 공중에 있는 것으로 간주하는 기준 속도
    public static readonly float UnjumpableVelocity = -0.49f;
    //뛰어 오르기
    public void Jump();
    //뛸 수 있는지 반환
    public bool CanJump();
}

/// <summary>
/// 피격 당할 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IHittable: IPositionable
{
    public bool isAlive {
        get;
    }

    public string tag
    {
        set;
        get;
    }

    public void Hit(Strike strike);

    public void Hit(Spell[] spells);

    public Collider2D GetCollider2D();
}

/// <summary>
/// 
/// </summary>
public interface IStrikeable : IPositionable
{
    public void Strike(Skill skill, uint level = 0);
}