using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager Instance { get; private set; }
    public AudioSource audioSource;
    public AudioClip[] audioclips;
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

    public void PlaySound(int soundIndex)
    {
        audioSource.PlayOneShot(audioclips[soundIndex]);
    }

    public void ChangeBGM(int soundIndex)
    {
        audioSource.Stop();
        audioSource.clip = audioBGClips[soundIndex];
        audioSource.Play();
    }
}
