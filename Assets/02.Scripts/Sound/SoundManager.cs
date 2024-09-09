
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] BgmClips;
    public float BgmVolume = 1;
    AudioSource BgmPlayer;

    public AudioClip[] SfxClips;
    public float SfxVolume = 1;
    public int Channels;
    AudioSource[] SfxPlayer;
    int channelIndex;

    public enum Bgm { LobbyScene, IntroFireball, Loading, MainScene, RainGauge, SundialScene, AstronomicalChart, ClepsydraScene }
    public enum Sfx { Rock, WaterItem, PuzzleInPlace, FloatingItem, Quiz_OX, Quiz_Blank }

    public static SoundManager instance;

    public Bgm CurrentBgm { get; internal set; }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }
    private void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        BgmPlayer = bgmObject.AddComponent<AudioSource>();
        BgmPlayer.playOnAwake = false;
        BgmPlayer.loop = true;
        BgmPlayer.volume = BgmVolume;

        GameObject SfxObject = new GameObject("SfxPlayer");
        SfxObject.transform.parent = transform;
        SfxPlayer = new AudioSource[Channels];

        for (int i = 0; i < SfxPlayer.Length; i++)
        {
            SfxPlayer[i] = SfxObject.AddComponent<AudioSource>();
            SfxPlayer[i].playOnAwake = false;
            SfxPlayer[i].volume = SfxVolume;
        }
    }
    public void PlayBgm(Bgm bgm)
    {
        BgmPlayer.clip = BgmClips[(int)bgm];
        BgmPlayer.volume = BgmVolume;
        BgmPlayer.Play();
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < SfxPlayer.Length; i++)
        {
            int loopIndex = (i + channelIndex) % SfxPlayer.Length;
            SfxPlayer[i].volume = SfxVolume;
            if (SfxPlayer[loopIndex].isPlaying)
            {
                continue;
            }
            channelIndex = loopIndex;
            SfxPlayer[loopIndex].clip = SfxClips[(int)sfx];
            SfxPlayer[loopIndex].Play();
            break;
        }
    }
    public void StopBgm()
    {
        if (BgmPlayer.isPlaying)
        {
            BgmPlayer.Stop();
        }
    }
    public void StopSfx(Sfx sfx)
    {
        for (int i = 0; i < SfxPlayer.Length; i++)
        {
            if (SfxPlayer[i].clip == SfxClips[(int)sfx])
            {
                SfxPlayer[i].Stop();
            }
        }
    }

    // 사운드 페이드 아웃 기능(자격루 씬에서 자연스럽게 음악 꺼지게 할 때 사용)
    public void FadeOutBgm(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = BgmPlayer.volume;

        while (BgmPlayer.volume > 0)
        {
            BgmPlayer.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        BgmPlayer.Stop();
        BgmPlayer.volume = startVolume; // 볼륨을 원래 값으로 복구
    }
}