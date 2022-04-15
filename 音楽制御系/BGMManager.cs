using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BGMManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip normalBgm;
    [SerializeField] private AudioClip combatBgm;
    public AudioClip NormalBgm { get { return normalBgm; } }
    private const float FADE_TIME = 2f, INTERVAL = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// BGMを徐々に変更する
    /// </summary>
    /// <param name="nextAudio">新しく流すBGMのAudioSource</param>
    /// <param name="changeDuration">BGMのフェードアウト時間</param>
    /// <param name="changeWaitTime">変化するまでの待機時間</param>
    public void ChangeAudioLeap(AudioClip overrideClip) {
        DOTween.Sequence()
            .Append(
                audioSource.DOFade(0f, FADE_TIME)
            ).AppendInterval(
                INTERVAL
            ).OnComplete(() => {
                audioSource.Stop();
                audioSource.clip = overrideClip;
                audioSource.volume = 0.2f;
                audioSource.Play();
            });
    }
}
