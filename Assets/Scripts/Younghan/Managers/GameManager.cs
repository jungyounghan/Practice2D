using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 진행을 총괄하는 매니저, 싱글턴을 상속 받은 클래스
/// </summary>
public class GameManager : Manager<GameManager>
{
    [SerializeField]
    private Controller _controller1;

    private List<IHittable> hittableList = new List<IHittable>();

    protected override void Initialize()
    {
        _destroyOnLoad = true;
        _controller1.player.Initialize(Report);
    }

    private void ShowEffect(GameObject effectObject, Vector2 position)
    {

    }

    private void Hit(Strike strike, IHittable hittable, GameObject effectObject)
    {
        ShowEffect(effectObject, hittable.position);
        hittable.Hit(strike);
    }

    public static void Report(IHittable hittable, int strikeResult)
    {
        //데미지가 이 대상에게 얼마나 들어왔는지 보고하고 ui로 값을 전송
        //사망하면 개별 보고
    }

    /// <summary>
    /// 보고하는 함수
    /// </summary>
    /// <param name="strike"></param>
    /// <param name="area"></param>
    /// <param name="effectObject"></param>
    /// <param name="strikeable"></param>
    public static void Report(Strike strike, Strike.Area area, GameObject effectObject, IStrikeable strikeable)
    {
        if(area == null)
        {
            instance.Hit(strike, instance._controller1.player, effectObject);
            int count = instance.hittableList.Count;
            for (int i = 0; i < count; i++)
            {
                instance.Hit(strike, instance.hittableList[i], effectObject);
            }
        }
        else
        {
            if (area.CanStrike(instance._controller1.player) == true)
            {
                instance.Hit(strike, instance._controller1.player, effectObject);
            }
            int count = instance.hittableList.Count;
            for (int i = 0; i < count; i++)
            {
                if (area.CanStrike(instance.hittableList[i]) == true)
                {
                    instance.Hit(strike, instance.hittableList[i], effectObject);
                }
            }
        }
    }
}