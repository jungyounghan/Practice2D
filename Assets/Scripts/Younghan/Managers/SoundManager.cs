using System.Collections.Generic;
using UnityEngine;
using Sound;

//�Ҹ��� ������ �з��� ���� �����̽�
namespace Sound
{
    //�������
    public enum Music
    {
        End
    }

    //ȿ����
    public enum Effect
    {
        End
    }
}

/// <summary>
/// ������ ������ǰ� ȿ������ �Ѱ��ϴ� �Ŵ��� �̱����� ��� ���� Ŭ����
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundManager : Manager<SoundManager>
{
    private bool _hasAudioSource = false;

    private AudioSource _audioSource;

    //��������� ����ϴ� ����� �ҽ� ��ü
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

    //ȿ�������� ����ϴ� ����� �ҽ� ��ü��
    private List<AudioSource> _effectAudioList = new List<AudioSource>();

    //����� ������� Ŭ����
    [SerializeField, Header("����� ������� Ŭ����")]
    private AudioClip[] _musicClips = new AudioClip[(int)Music.End];

    //����� ȿ���� Ŭ����
    [SerializeField, Header("����� ȿ���� Ŭ����")]
    private AudioClip[] _effectClips = new AudioClip[(int)Effect.End];

    /// <summary>
    /// �ʱ�ȭ �Լ�: ���� ������ ������� ��ƾ�� �ݺ����� �����Ѵ�.
    /// </summary>
    protected override void Initialize()
    {
        _destroyOnLoad = false;
        getMusicAudio.loop = true;
    }

    /// <summary>
    /// ��������� �÷��� �����ִ� �Լ�(�ε����� ����� ������� ���� ��Ŵ)
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
    /// ��������� ���� �����ִ� �Լ�
    /// </summary>
    public static void StopMusic()
    {
        instance.getMusicAudio.Stop();
    }

    /// <summary>
    /// ȿ������ �÷��� �����ִ� �Լ�(�ε����� ����� ȿ������ ������� ����)
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
    /// ��� ȿ�������� ���� �����ִ� �Լ�
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