using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制音效与背景音乐的播放与切换，在gameManger之后实例化
/// </summary>


public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager Instance { get; private set; }
    public AudioSource audioSource;
    public AudioClip[] audioclips;//特效音
    public AudioClip[] audioBGClips;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 播放特效音
    /// </summary>
    /// <param name="soundIndex"></param>
    public void PlaySound(int soundIndex)
    {
        audioSource.PlayOneShot(audioclips[soundIndex]);
    }

    /// <summary>
    /// 切换背景音乐
    /// </summary>
    /// <param name="soundIndex"></param>
    public void ChangeBGM(int soundIndex)
    {
        audioSource.Stop();
        audioSource.clip = audioBGClips[soundIndex];
    }
}
