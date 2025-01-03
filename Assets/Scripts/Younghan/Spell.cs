using System.Collections.Generic;
using UnityEngine;

//������ ����� ������ ����ϴ� �ֹ� Ŭ����
public class Spell : ScriptableObject, ISerializationCallbackReceiver
{
    //�̹� ����ڿ��� � ������A�� ����� ���� �ֹ��� �ִµ� �ٸ� ������B�� ���� �ֹ��� �ɾ��� �� ��ø�� ���� ����
    [SerializeField]
    private bool _overlapOthers = true;

    public bool overlapOthers
    {
        get
        {
            return _overlapOthers;
        }
    }

    //���� �ֹ��� �ִ� ��ø ���� �ѵ��� �ǹ�
    [SerializeField]
    private byte _overlapLimit = 0;

    public byte overlapLimit
    {
        get
        {
            return _overlapLimit;
        }
    }

    //���� �ֹ��� �ߵ��ϰ� �ִ� ���� ��Ÿ�� ȿ��
    [SerializeField]
    private GameObject _effectObject;

    public GameObject effectObject
    {
        get
        {
            return _effectObject;
        }
    }

    private Dictionary<Stat.Category, Expression.Info> _dictionary = new Dictionary<Stat.Category, Expression.Info>();

    //����ȭ �� ȣ��Ǵ� �ݹ� �Լ�
    public void OnBeforeSerialize()
    {
    }

    //������ȭ �� ȣ��Ǵ� �ݹ� �Լ�
    public void OnAfterDeserialize()
    {

    }

    public Dictionary<Stat.Category, float> GetStats(uint level)
    {
        Dictionary<Stat.Category, float> dictionary = new Dictionary<Stat.Category, float>();
        foreach(KeyValuePair<Stat.Category, Expression.Info> kvp in _dictionary)
        {

        }
        return dictionary;
    }

    //����ڿ��� �ɸ� �ֹ��� ���� ����
    public class Info
    {
        //�� �ֹ��� ������
        private GameObject _user;

        public GameObject user
        {
            get
            {
                return _user;
            }
        }

        //�� �ֹ��� ���� ����
        private uint _level;

        public uint level
        {
            get
            {
                return _level;
            }
        }

        //�� �ֹ��� ��ø Ƚ��
        private byte _overlapCount;

        public byte overlapCount
        {
            get
            {
                return _overlapCount;
            }
        }

        //�� �ֹ��� ���� �ð�
        private float _durationTime;

        public float durationTime
        {
            get
            {
                return _durationTime;
            }
        }

        /// <summary>
        /// ����ڿ��� �ɸ� �ֹ� ���� �ʱⰪ�� �Է��ؾ���
        /// </summary>
        /// <param name="user"></param>
        /// <param name="level"></param>
        /// <param name="overlapCount"></param>
        /// <param name="durationTime"></param>
        public Info(GameObject user, uint level, byte overlapCount, float durationTime)
        {
            _user = user;
            _level = level;
            _overlapCount = overlapCount;
            _durationTime = Mathf.Clamp(durationTime, 0, float.MaxValue);
        }

        /// <summary>
        /// ���� ȿ���� �������� ���θ� ��ȯ�ϴ� �޼���
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>

        public bool IsFinished(float deltaTime)
        {
            _durationTime -= deltaTime;
            if (0 < durationTime)
            {
                return true;
            }
            return false;
        }
    }
}