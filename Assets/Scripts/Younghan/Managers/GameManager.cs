using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 게임의 진행을 총괄하는 매니저, 싱글턴을 상속 받은 클래스
/// </summary>
public class GameManager : Manager<GameManager>
{
    [SerializeField]
    private Controller _controller1;
    [SerializeField]
    private Controller _controller2;

    private void Set(float dd)
    {
        float dtt = dd;
    }

    protected override void Initialize()
    {
        _destroyOnLoad = true;
      
    }

    private void Update()
    {
        //float deltaTime = Time.deltaTime;
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        //Set(deltaTime);
        //stopwatch.Stop();
        //double dd = stopwatch.ElapsedMilliseconds;
        //UnityEngine.Debug.Log("deltaTime:"+ dd);
        //stopwatch.Start();
        //Set(Time.deltaTime);
        //stopwatch.Stop();
        //dd = stopwatch.ElapsedMilliseconds;
        //UnityEngine.Debug.Log("Time.deltaTime:" + dd);
    }
}