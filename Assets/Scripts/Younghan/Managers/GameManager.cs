using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 진행을 총괄하는 매니저, 싱글턴을 상속 받은 클래스
/// </summary>
public sealed class GameManager : Manager<GameManager>
{
    [SerializeField]
    private Controller _controller1;

    private List<IHittable> _hittableList = new List<IHittable>();

    protected override void Initialize()
    {
        _destroyOnLoad = true;
        _controller1.player.Initialize(Report, CanInteraction);
    }

    private void ShowEffect(GameObject effectObject, Vector2 position)
    {

    }

    private void Hit(Strike strike, IHittable hittable, GameObject effectObject)
    {
        ShowEffect(effectObject, hittable.GetCollider2D().bounds.center);
        hittable.Hit(strike);
    }

    public static void Report(IHittable hittable, int strikeResult)
    {
        //데미지가 이 대상에게 얼마나 들어왔는지 보고하고 ui로 값을 전송
        //사망하면 개별 보고
    }

    public static void Report(Strike strike, Strike.Area area, GameObject effectObject)
    {
        if(area == null)
        {
            instance.Hit(strike, instance._controller1.player, effectObject);
            int count = instance._hittableList.Count;
            for (int i = 0; i < count; i++)
            {
                instance.Hit(strike, instance._hittableList[i], effectObject);
            }
        }
        else
        {
            if (area.CanStrike(instance._controller1.player) == true)
            {
                instance.Hit(strike, instance._controller1.player, effectObject);
            }
            int count = instance._hittableList.Count;
            for (int i = 0; i < count; i++)
            {
                if (area.CanStrike(instance._hittableList[i]) == true)
                {
                    instance.Hit(strike, instance._hittableList[i], effectObject);
                }
            }
        }
    }

    public static bool CanInteraction(Player.Interaction interaction)
    {

        return false;
    }
}