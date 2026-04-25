using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 音乐音效管理器
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    //背景音乐播放组件
    private AudioSource bkMusic = null;

    //背景音乐大小
    private float bkMusicValue = SaveMgr.Instance.musicData.musicValue;

    //预加载的背景音乐
    private Dictionary<string, AudioClip> preloadedBkMusic = new Dictionary<string, AudioClip>();
    private bool isPreloading = false;

    //管理正在播放的音效
    private List<AudioSource> soundList = new ();
    //音效音量大小
    private float soundValue = SaveMgr.Instance.musicData.soundValue;
    //音效是否在播放
    private bool soundIsPlay = true;


    private MusicMgr() 
    {
        MonoMgr.Instance.AddFixedUpdateListener(soundUpdate);
    }


    private void soundUpdate()
    {
        if (!soundIsPlay)
            return;

        //遍历容器检测有没有音效播放完毕播放完了就移除销毁它
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            var source = soundList[i];
            if (source == null)
            {
                soundList.RemoveAt(i);
                continue;
            }

            if(!source.isPlaying)
            {
                //音效播放完毕音效切片置空
                source.clip = null;
                GameObject.Destroy(source.gameObject);
                soundList.RemoveAt(i);
            }
        }
    }


    //播放背景音乐
    public void PlayBKMusic(string name)
    {
        //动态创建播放背景音乐的组件并且不会过场景移除 
        //保证背景音乐在过场景时也能播放
        if(bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }
        
        //检查是否需要加载新音乐
        if (bkMusic.clip == null || bkMusic.clip.name != name)
        {
            //如果已预加载，直接使用
            if (preloadedBkMusic.TryGetValue(name, out AudioClip clip) && clip != null)
            {
                bkMusic.clip = clip;
                bkMusic.loop = true;
                bkMusic.volume = bkMusicValue;
                bkMusic.Play();
                return;
            }
            
            //否则异步加载
            ABMgr.Instance.LoadResAsync<AudioClip>("music", name, (c) =>
            {
                if (c == null)
                {
                    Debug.LogError($"加载失败: {name}");
                    return;
                }
                
                bkMusic.clip = c;
                bkMusic.loop = true;
                bkMusic.volume = bkMusicValue;
                if (!bkMusic.isPlaying)
                {
                    bkMusic.Play();
                }
            }).Forget();
        }
        else
        {
            //如果是同一首音乐，只在没有播放时播放
            if (!bkMusic.isPlaying)
            {
                bkMusic.volume = bkMusicValue;
                bkMusic.Play();
            }
        }
    }

    //预加载背景音乐（进入关卡时调用）
    public void PreloadBKMusic(params string[] names)
    {
        if (isPreloading) return;
        isPreloading = true;
        
        foreach (var name in names)
        {
            if (preloadedBkMusic.ContainsKey(name)) continue;
            
            ABMgr.Instance.LoadResAsync<AudioClip>("music", name, (clip) =>
            {
                if (clip != null)
                {
                    preloadedBkMusic[name] = clip;
                }
            }).Forget();
        }
        
        isPreloading = false;
    }

    //停止背景音乐
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    //暂停背景音乐
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    //设置背景音乐大小
    public void ChangeBKMusicValue(float v)
    {
        bkMusicValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkMusicValue;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名字</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="isSync">是否同步加载</param>
    /// <param name="callBack">加载结束后的回调</param>
    public async void PlaySound(string name, bool isLoop = false, bool isSync = false, UnityAction<AudioSource> callBack = null)
    {
        AudioClip clip = null;
        await ABMgr.Instance.LoadResAsync<AudioClip>("music", name, (c) => clip = c, isSync);
        
        if (clip == null)
        {
            Debug.LogError($"加载音效切片失败: {name}");
            return;
        }
        
        GameObject soundObj = new GameObject("Sound_" + name);
        AudioSource source = soundObj.AddComponent<AudioSource>();
        
        source.clip = clip;
        source.loop = isLoop;
        source.volume = soundValue;
        source.Play();
        
        if(!soundList.Contains(source))
            soundList.Add(source);
        
        callBack?.Invoke(source);
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    /// <param name="source">音效组件对象</param>
    public void StopSound(AudioSource source)
    {
        if(soundList.Contains(source))
        {
            //停止播放
            source.Stop();
            //从容器中移除
            soundList.Remove(source);
            //不用了 清空切片 避免占用
            source.clip = null;
            //直接销毁对象
            GameObject.Destroy(source.gameObject);
        }
    }

    /// <summary>
    /// 改变音效大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeSoundValue(float v)
    {
        soundValue = v;
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            var source = soundList[i];
            if (source == null)
            {
                soundList.RemoveAt(i);
                continue;
            }
            source.volume = v;
        }
    }

    /// <summary>
    /// 继续播放或者暂停所有音效
    /// </summary>
    /// <param name="isPlay">是否是继续播放 true为播放 false为暂停</param>
    public void PlayOrPauseSound(bool isPlay)
    {
        if(isPlay)
        {
            soundIsPlay = true;
            for (int i = soundList.Count - 1; i >= 0; --i)
            {
                var source = soundList[i];
                if (source == null)
                {
                    soundList.RemoveAt(i);
                    continue;
                }
                source.Play();
            }
        }
        else
        {
            soundIsPlay = false;
            for (int i = soundList.Count - 1; i >= 0; --i)
            {
                var source = soundList[i];
                if (source == null)
                {
                    soundList.RemoveAt(i);
                    continue;
                }
                source.Pause();
            }
        }
    }

    //清空所有音效
    public void ClearSound()
    {
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            var source = soundList[i];
            if (source != null)
            {
                source.Stop();
                source.clip = null;
                GameObject.Destroy(source.gameObject);
            }
        }
        //清空音效列表
        soundList.Clear();
    }
}
