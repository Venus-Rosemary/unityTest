using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    [System.Serializable]
    public class AudioData
    {
        public string name;
        public AudioClip clip;
        public bool _isLoop;            //是否需要循环
    }

    public List<AudioData> audioClips = new List<AudioData>(); // 存储音效数据列表
    public List<AudioSource> audioSources = new List<AudioSource>(); // 存储所有的AudioSource


    private void Start()
    {
        // 初始化AudioSource池
        for (int i = 0; i < 20; i++) // 创建10个AudioSource池作为示例
        {
            GameObject go = new GameObject($"AudioSource_{i}");
            go.transform.SetParent(this.transform);
            AudioSource source = go.AddComponent<AudioSource>();
            audioSources.Add(source);
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="audioName">音效名称</param>
    public void PlaySound(string audioName)
    {
        AudioSource source = GetAvailableAudioSource();
        if (source != null)
        {
            AudioData data = GetAudioDataByName(audioName);
            if (data != null && data.clip != null)
            {
                source.clip = data.clip;
                if (data._isLoop)
                {
                    source.loop = true;
                }
                else
                {
                    source.loop = false;
                }
                source.Play();
            }
            else
            {
                Debug.LogWarning($"找不到名为 {audioName} 的音效");
            }
        }
        else
        {
            Debug.LogWarning("没有可用的AudioSource");
        }
    }

    /// <summary>
    /// 获取可用的AudioSource
    /// </summary>
    /// <returns></returns>
    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        // PS:如果没有找到可用的AudioSource，返回null，而不是动态创建新的AudioSource

        GameObject go = new GameObject($"AudioSource_");
        go.transform.SetParent(this.transform);
        AudioSource source02 = go.AddComponent<AudioSource>();
        audioSources.Add(source02);

        return source02;
    }

    /// <summary>
    /// 根据名称获取AudioData
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private AudioData GetAudioDataByName(string name)
    {
        return audioClips.Find(data => data.name == name);
    }
}
