using System.Collections.Generic;
using UnityEngine;
using Sound;

//소리의 유형을 분류한 네임 스페이스
namespace Sound
{
    //배경음악
    public enum Music
    {
        End
    }

    //효과음
    public enum Effect
    {
        End
    }
}

/// <summary>
/// 게임의 배경음악과 효과음을 총괄하는 매니저 싱글턴을 상속 받은 클래스
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundManager : Manager<SoundManager>
{
    private bool _hasAudioSource = false;

    private AudioSource _audioSource;

    //배경음악을 담당하는 오디오 소스 객체
    private AudioSource getMusicAudio
    {
        get
        {
            if(_hasAudioSource == false)
            {
                _hasAudioSource = true;
                _audioSource = GetComponent<AudioSource>();
            }
            return _audioSource;
        }
    }

    //효과음들을 담당하는 오디오 소스 객체들
    private List<AudioSource> _effectAudioList = new List<AudioSource>();

    //사용할 배경음악 클립들
    [SerializeField, Header("사용할 배경음악 클립들")]
    private AudioClip[] _musicClips = new AudioClip[(int)Music.End];

    //사용할 효과음 클립들
    [SerializeField, Header("사용할 효과음 클립들")]
    private AudioClip[] _effectClips = new AudioClip[(int)Effect.End];

    /// <summary>
    /// 초기화 함수: 삭제 방지와 배경음악 루틴을 반복으로 설정한다.
    /// </summary>
    protected override void Initialize()
    {
        _destroyOnLoad = false;
        getMusicAudio.loop = true;
    }

    /// <summary>
    /// 배경음악을 플레이 시켜주는 함수(인덱스를 벗어나면 배경음을 정지 시킴)
    /// </summary>
    /// <param name="music"></param>
    public static void Play(Music music)
    {
        int index = (int)music;
        int length = instance._musicClips.Length;
        if (index >= 0 && index < length)
        {
            instance.getMusicAudio.clip = instance._musicClips[index];
            instance.getMusicAudio.Play();
        }
        else
        {
            StopMusic();
        }
    }

    /// <summary>
    /// 배경음악을 정지 시켜주는 함수
    /// </summary>
    public static void StopMusic()
    {
        instance.getMusicAudio.Stop();
    }

    /// <summary>
    /// 효과음을 플레이 시켜주는 함수(인덱스를 벗어나면 효과음이 재생되지 않음)
    /// </summary>
    /// <param name="effect"></param>
    public static void Play(Effect effect)
    {
        int index = (int)effect;
        int length = instance._effectClips.Length;
        if (index >= 0 && index < length)
        {
            int count = instance._effectAudioList.Count;
            for(int i = 0; i < count; i++)
            {
                if (instance._effectAudioList[i].isPlaying == false)
                {
                    instance._effectAudioList[i].PlayOneShot(instance._effectClips[index]);
                    return;
                }
            }
            AudioSource audioSource = instance.gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.PlayOneShot(instance._effectClips[index]);
            instance._effectAudioList.Add(audioSource);
        }
    }

    /// <summary>
    /// 모든 효과음들을 정지 시켜주는 함수
    /// </summary>
    public static void StopEffect()
    {
        int count = instance._effectAudioList.Count;
        for (int i = 0; i < count; i++)
        {
            instance._effectAudioList[i].Stop();
        }
    }

    //public static void SetMuteSfx(bool _mute)
    //{
    //    Instance.sfxSource.mute = _mute;
    //}
    //public static void SetMuteBgm(bool _mute)
    //{
    //    Instance.bgmSource.mute = _mute;
    //}

    //public static void ChangeSfxVolume(float _value)
    //{
    //    Instance.sfxSource.volume = _value;
    //}
    //public static void ChangeBgmVolume(float _value)
    //{
    //    Instance.bgmSource.volume = _value;
    //}

    //public static float GetVolumeSfx()
    //{
    //    return Instance.sfxSource.volume;
    //}
    //public static float GetVolumeBgm()
    //{
    //    return Instance.bgmSource.volume;
    //}

    //public static bool GetMuteSfx()
    //{
    //    return Instance.sfxSource.mute;
    //}
    //public static bool GetMuteBgm()
    //{
    //    return Instance.bgmSource.mute;
    //}
}