using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameDataMgr
{
    private static GameDataMgr instance = new GameDataMgr();
    public static GameDataMgr Instance => instance;

    public MusicData musicData;
    public RoleInfos roleInfos;
    public PlayerData playerData;
    public RoleInfos.RoleData roleSeInfo;

    public List<SceneInfo> sceneInfos;
    public List<TowerInfo> towerInfos;
    public List<MonsterInfo> monsterInfos;
    private GameDataMgr()
    {
        //初始化一些默认数据
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData");
        roleInfos = Resources.Load<RoleInfos>("ScriptableObject/RoleInfos");
        playerData = JsonMgr.Instance.LoadData<PlayerData>("PlayerData");
        sceneInfos= JsonMgr.Instance.LoadData<List<SceneInfo>>("SceneInfo");
        towerInfos= JsonMgr.Instance.LoadData<List<TowerInfo>>("TowerInfo");
        monsterInfos= JsonMgr.Instance.LoadData<List<MonsterInfo>>("MonsterInfo");

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

    public void PlaySound(string res)
    {
        GameObject musicObj = new GameObject();
        AudioSource a=musicObj.AddComponent<AudioSource>();
        a.clip=Resources.Load<AudioClip>(res);
        a.volume=musicData.soundValue;
        a.mute=!musicData.soundOpen;
        a.Play();
        GameObject.Destroy(musicObj,1);
    }









}
