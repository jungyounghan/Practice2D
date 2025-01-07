using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ �Ѱ��ϴ� �Ŵ���, �̱����� ��� ���� Ŭ����
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
        //�������� �� ��󿡰� �󸶳� ���Դ��� �����ϰ� ui�� ���� ����
        //����ϸ� ���� ����
    }

    /// <summary>
    /// �����ϴ� �Լ�
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