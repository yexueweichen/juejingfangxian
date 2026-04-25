using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏的唯一开始入口管理子单例
/// </summary>
public class GameMgr : MonoBehaviour
{
    void Start()
    {
        //游戏开始入口显示开始界面
        UIMgr.Instance.ShowPanel<BeginPanel>();
        //播放背景音乐
        MusicMgr.Instance.PlayBKMusic("BKMusic");

        Debug.Log(Application.persistentDataPath);
    
    }

   
}
