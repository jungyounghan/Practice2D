using System.Collections.Generic;
using UnityEngine;

//버프와 디버프 내용을 담당하는 주문 클래스
public class Spell : ScriptableObject, ISerializationCallbackReceiver
{
    //이미 대상자에게 어떤 시전자A가 사용한 현재 주문이 있는데 다른 시전자B가 같은 주문을 걸었을 때 중첩이 될지 여부
    [SerializeField]
    private bool _overlapOthers = true;

    public bool overlapOthers
    {
        get
        {
            return _overlapOthers;
        }
    }

    //현재 주문의 최대 중첩 가능 한도를 의미
    [SerializeField]
    private byte _overlapLimit = 0;

    public byte overlapLimit
    {
        get
        {
            return _overlapLimit;
        }
    }

    //현재 주문이 발동하고 있는 동안 나타날 효과
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

    //직렬화 전 호출되는 콜백 함수
    public void OnBeforeSerialize()
    {
    }

    //역직렬화 후 호출되는 콜백 함수
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

    //대상자에게 걸린 주문의 개별 정보
    public class Info
    {
        //이 주문의 시전자
        private GameObject _user;

        public GameObject user
        {
            get
            {
                return _user;
            }
        }

        //이 주문의 현재 레벨
        private uint _level;

        public uint level
        {
            get
            {
                return _level;
            }
        }

        //이 주문의 중첩 횟수
        private byte _overlapCount;

        public byte overlapCount
        {
            get
            {
                return _overlapCount;
            }
        }

        //이 주문의 지속 시간
        private float _durationTime;

        public float durationTime
        {
            get
            {
                return _durationTime;
            }
        }

        /// <summary>
        /// 대상자에게 걸린 주문 정보 초기값을 입력해야함
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
        /// 지속 효과가 끝났는지 여부를 반환하는 메서드
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