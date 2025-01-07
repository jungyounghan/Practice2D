using System;
using UnityEngine;

/// <summary>
/// ������ �����ϰ� �Ǵ� �÷��̾� Ŭ����
/// </summary>
public class Player : Runner, IHittable
{
    public enum Interaction
    {
        Pick,
        MoveUp,
        MoveDown
    }

    [SerializeField]
    private Animator _animator = null;

    public bool isAlive
    {
        get;
    }

    //�ǰ� ���� �׼�

    private Action<Strike, Strike.Area, GameObject> _strikeAction = null;
    
    //������ü �߻� �׼�

    private Func<Interaction, bool> _interactionFunction = null;

    public void Initialize(Action<Strike, Strike.Area, GameObject> strikeAction, Func<Interaction, bool> interactionFunction)
    {
        _strikeAction = strikeAction;
        _interactionFunction = interactionFunction;
    }

    public void Heal()
    {

    }

    public void UseLethalMove()
    {

    }

    public void Attack1()
    {
        //Strike.PolygonArea polygonArea = new Strike.PolygonArea(position, null, null);
        //polygonArea.Show();
        //Strike.TagArea tagArea = new Strike.TagArea(new string[] {"Player"});
        //tagArea.Show();
        Strike.TargetArea targetArea = new Strike.TargetArea(new IHittable[1] { this});
        targetArea.Show();

    }

    public void Attack2()
    {

    }

    public void Attack3()
    {

    }

    public void MoveUp()
    {
        if(_interactionFunction != null && _interactionFunction.Invoke(Interaction.MoveUp) == true)
        {
            //�����ϸ� �ִϸ��̼� �ٲٱ�
        }
    }

    public void MoveDown()
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