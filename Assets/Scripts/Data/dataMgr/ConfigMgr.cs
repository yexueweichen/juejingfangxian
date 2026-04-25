using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class ConfigMgr : BaseManager<ConfigMgr>
{
    
    public RoleInfos roleInfos;
    public MonsterInfos monsterInfos;
    public TowerInfos towerInfos;
    public SceneInfos sceneInfos;
    public RoleInfos.RoleData roleSeInfo;
    public SceneInfos.SceneInfo sceneSeInfo;

    private ConfigMgr()
    {
      
      LoadAllConfigsAsync();
    }

    // 同步加载所有配置文件
    public void LoadAllConfigsAsync()
    {
        ABMgr.Instance.LoadResAsync<RoleInfos>("config", "RoleInfos", (res) => roleInfos = res, true).Forget();
        ABMgr.Instance.LoadResAsync<MonsterInfos>("config", "MonsterInfos", (res) => monsterInfos = res, true).Forget();
        ABMgr.Instance.LoadResAsync<TowerInfos>("config", "TowerInfos", (res) => towerInfos = res, true).Forget();
        ABMgr.Instance.LoadResAsync<SceneInfos>("config", "SceneInfos", (res) => sceneInfos = res, true).Forget();
        
    }

}
