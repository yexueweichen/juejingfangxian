using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保存管理器
/// </summary>
public class SaveMgr : BaseManager<SaveMgr>
{
    public MusicData musicData;
    public PlayerData playerData;
    private SaveMgr()
    {
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData");
        playerData = JsonMgr.Instance.LoadData<PlayerData>("PlayerData");
    }
    /// <summary>
    /// 存储音效数据
    /// </summary>
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicData, "MusicData");

    }
    /// <summary>
    /// 存储玩家数据
    /// </summary>
    public void SavePlayerData()
    {
        JsonMgr.Instance.SaveData(playerData, "PlayerData");
    }



}
