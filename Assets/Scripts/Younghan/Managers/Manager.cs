using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �� Ŭ������ ��ӹ��� ��ü���� ��� �̱������� �ϳ��� �� �ȿ��� ���� �ϳ��� ������Ʈ�θ� �����ϰ� �����.
/// </summary>
[DisallowMultipleComponent]
public abstract class Manager<T> : MonoBehaviour where T: MonoBehaviour
{
    private static readonly string TextDontDestroyOnLoad = "DontDestroyOnLoad";

    //�ش� ��ü�� �� ��ȯ �� ���� ���θ� �Ǵ��ϴ� ����
    protected bool _destroyOnLoad;

    //�̱��� ��ü
    private static T _instance = null;

    public static T instance {
        get
        {
            //���� �̱����� ȣ�� �ߴµ� ������ ���� ��� ���ο� ���� ������Ʈ�� ���� ���빰�� �־��ش�.
            if (_instance == null)
            {
                new GameObject().AddComponent<T>().gameObject.name = _instance.GetType().Name;
            }
            return _instance;
        }
    }

    /// <summary>
    /// �� �Լ��� ��ũ��Ʈ�� �ε�ǰų� �˻�⿡�� ���� ����� �� ȣ��ȴ�.(�����⿡���� ȣ���)
    /// </summary>
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        //��ǥ�ϴ� �̱��� ��ü�� ���ٸ� ������ ��ü�� �ش��ϴ� ������Ʈ�� ã�´�.
        if (_instance == null)
        {
            _instance = (T)FindObjectOfType(typeof(T));
        }
        //�� ����� ��ǥ�ϴ� �̱��� ��ü�� �ƴ϶�� �ݹ� �Լ��� �������ش�.
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
        //��ǥ�ϴ� �̱��� ��ü�� �� ��ü�� �ƴ� ���
        if (_instance != this)
        {
            //��ǥ�ϴ� �̱��� ��ü�� ���� ��� ������ ��ü�� �ش��ϴ� ������Ʈ�� ã�´�.
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }
            //��ǥ�ϴ� �̱��� ��ü�� �ִ� ���
            else
            {
                //��ǥ�ϴ� �̱��� ��ü�� DontDestroyOnLoad ���� �ִ� ���
                if (_instance.gameObject.scene.name == TextDontDestroyOnLoad)
                {
                    Manager<T> manager = _instance.GetComponent<Manager<T>>();
                    //�� ��ȯ �� ��� ������Ʈ�� �������־�� �Ѵٸ� �ٸ� ��ü���� ��ǥ �ڸ��� �絵�ϰ� �������� ������ �����Ѵ�.
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
        //��ǥ�ϴ� �̱��� ��ü�� �� ��ü�� ���
        if (_instance == this)
        {
            Initialize();
            //�� ��ȯ �� ��� ������Ʈ�� �� ��ȯ���� �������� �ʴ´�.
            if (_destroyOnLoad == false)
            {
                DontDestroyOnLoad(this);
            }
            //�� ��ȯ �� ��� ������Ʈ�� �� ��ȯ���� �����Ѵ�.
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