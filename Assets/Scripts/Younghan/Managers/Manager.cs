using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 이 클래스를 상속받은 객체들을 모두 싱글턴으로 하나의 씬 안에서 오직 하나의 컴포넌트로만 존재하게 만든다.
/// </summary>
[DisallowMultipleComponent]
public abstract class Manager<T> : MonoBehaviour where T: MonoBehaviour
{
    private static readonly string TextDontDestroyOnLoad = "DontDestroyOnLoad";

    //해당 객체를 씬 전환 시 삭제 여부를 판단하는 변수
    protected bool _destroyOnLoad;

    //싱글턴 객체
    private static T _instance = null;

    public static T instance {
        get
        {
            //만약 싱글턴을 호출 했는데 내용이 없을 경우 새로운 게임 오브젝트를 만들어서 내용물을 넣어준다.
            if (_instance == null)
            {
                new GameObject().AddComponent<T>().gameObject.name = _instance.GetType().Name;
            }
            return _instance;
        }
    }

    /// <summary>
    /// 이 함수는 스크립트가 로드되거나 검사기에서 값이 변경될 때 호출된다.(편집기에서만 호출됨)
    /// </summary>
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        //대표하는 싱글턴 객체가 없다면 씬에서 객체에 해당하는 오브젝트를 찾는다.
        if (_instance == null)
        {
            _instance = (T)FindObjectOfType(typeof(T));
        }
        //이 대상이 대표하는 싱글턴 객체가 아니라면 콜백 함수로 삭제해준다.
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != _instance && this != null)
            {
                UnityEditor.Undo.DestroyObjectImmediate(this);
            }
        };
    }
#endif

    private void Awake()
    {
        //대표하는 싱글턴 객체가 이 객체가 아닌 경우
        if (_instance != this)
        {
            //대표하는 싱글턴 객체가 없는 경우 씬에서 객체에 해당하는 오브젝트를 찾는다.
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }
            //대표하는 싱글턴 객체가 있는 경우
            else
            {
                //대표하는 싱글턴 객체가 DontDestroyOnLoad 씬에 있는 경우
                if (_instance.gameObject.scene.name == TextDontDestroyOnLoad)
                {
                    Manager<T> manager = _instance.GetComponent<Manager<T>>();
                    //씬 전환 시 대상 컴포넌트를 삭제해주어야 한다면 다른 객체에게 대표 자리를 양도하고 나머지를 모조리 삭제한다.
                    if (manager._destroyOnLoad == true)
                    {
                        T[] ts = FindObjectsOfType<T>();
                        T t = null;
                        int length = ts != null ? ts.Length : 0;
                        for(int i = 0; i < length; i++)
                        {
                            if(t == null && ts[i] != _instance)
                            {
                                t = ts[i];
                                break;
                            }
                        }
                        for (int i = 0; i < length; i++)
                        {
                            if (ts[i] != t)
                            {
                                Destroy(ts[i]);
                            }
                        }
                        _instance = t;
                        return;
                    }
                }
                Destroy(this);
            }
        }
        //대표하는 싱글턴 객체가 이 객체인 경우
        if (_instance == this)
        {
            Initialize();
            //씬 전환 시 대상 컴포넌트를 씬 전환에서 삭제하지 않는다.
            if (_destroyOnLoad == false)
            {
                DontDestroyOnLoad(this);
            }
            //씬 전환 시 대상 컴포넌트를 씬 전환에서 삭제한다.
            else
            {
                SceneManager.sceneUnloaded += (scene) =>
                {
                    Destroy(this);
                };
            }
        }
    }

    protected abstract void Initialize();
}