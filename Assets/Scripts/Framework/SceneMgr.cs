using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换管理器
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr() { }

    //同步切换场景的方法
    public void LoadScene(string name, UnityAction callBack = null)
    {
        //切换场景
        SceneManager.LoadScene(name);
        //调用回调
        callBack?.Invoke();
    }

    //异步切换场景的方法
    public async UniTask LoadSceneAsync(string name, UnityAction callBack = null)
    {      
        // 开始异步加载场景
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        
        // 使用 UniTask 等待加载完成
        while (!ao.isDone)
        {
            // 每一帧将进度发送给监听者
            if (EventCenter.Instance != null)
            {
                EventCenter.Instance.EventTrigger(E_EventType.E_SceneLoadChange, ao.progress);
            }
            
            // 等待一帧
            await UniTask.Yield();
        }
        
        // 确保最后一帧同步进度为 1
        if (EventCenter.Instance != null)
        {
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange, 1);
        }

        // 调用回调
        callBack?.Invoke();
    }
}
