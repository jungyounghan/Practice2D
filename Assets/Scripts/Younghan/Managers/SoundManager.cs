using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Manager<SoundManager>
{
    protected override void Initialize()
    {
        _destroyOnLoad = false;
    }
}