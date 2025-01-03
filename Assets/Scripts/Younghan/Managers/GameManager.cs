using System.Collections;
using UnityEngine;

public class GameManager : Manager<GameManager>
{
    [SerializeField]
    private Controller controller1;
    [SerializeField]
    private Controller controller2;

    protected override void Initialize()
    {
        _destroyOnLoad = true;
    }
}