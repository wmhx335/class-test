using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制音效与背景音乐的播放与切换，在gameManger之后实例化
/// </summary>


public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
