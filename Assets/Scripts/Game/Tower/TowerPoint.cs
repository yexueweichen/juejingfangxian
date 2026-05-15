using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

//  造塔点
public class TowerPoint : MonoBehaviour
{
    //造塔点关联的 塔对象
    private GameObject towerobj = null;
   // 造塔点关联的 塔的数据
    // 不在字段初始化阶段引用 ConfigMgr（可能还未就绪），改为在 Start 设置
    public TowerInfos.TowerInfo nowtowerinfo;

    public int chooseIndex;

    private void Start()
    {
        // 默认使用数据表第 9 项作为"空地点"（请确认该项 id == 10）
        if (ConfigMgr.Instance != null && ConfigMgr.Instance.towerInfos != null)
        {
            nowtowerinfo = ConfigMgr.Instance.towerInfos.TowerDatas[9];
        }
        else
        {
            Debug.LogWarning("ConfigMgr 或 towerInfos 未就绪，nowtowerinfo 将在建塔时初始化。");
        }
    }

    /// <summary>
   /// 建造一个塔
   /// </summary>
   /// <param name = "id" ></ param >
    public async UniTask createtower(int id)
    {
      TowerInfos.TowerInfo info = ConfigMgr.Instance.towerInfos.TowerDatas[id - 1];
      // 如果能量不够 就不用建造了
       if (info.energy > GameLevelMgr.Instance.currentSceneInfo.energy)
           return;

       //扣能量
      EventCenter.Instance.EventTrigger<int>(E_EventType.E_ReduceEnergy, info.energy);
       
       //创建塔
      // 先判断之前是否有塔  如果有 就删除
       if (towerobj != null)
       {
           GameObject.Destroy(towerobj);
           towerobj = null;
       }
        // 实例化塔对象
        GameObject prefab = null;
        await ABMgr.Instance.LoadResAsync<GameObject>("modle", info.res, (obj) => prefab = obj, true);
        
        if (prefab == null)
        {
            Debug.LogError($"加载塔预制体失败！资源名：{info.res}");
            // 加载失败，退还能量
            EventCenter.Instance.EventTrigger<int>(E_EventType.E_AddEnergy, info.energy);
            return;
        }
        
        towerobj = GameObject.Instantiate(prefab, this.transform.position, Quaternion.identity);
        
        // 初始化塔
        TowerObject towerComponent = towerobj.GetComponent<TowerObject>();
        if (towerComponent == null)
        {
            Debug.LogError("预制体上没有 TowerObject 组件！");
            return;
        }
        
        towerComponent.InitInfo(info);

      // 记录当前塔的数据
       nowtowerinfo = info;
       // 将chooseIndex映射
       chooseIndex = (id - 1) / 3;

      //塔建造完毕 更新游戏界面上的内容
       GamePanel panel = UIMgr.Instance.GetPanel<GamePanel>();
       if (panel != null)
       {
           panel.UpdateSelTower(this);
       }
       else
       {
           Debug.LogError("GamePanel 为空！");
       }
    }

    private void OnTriggerEnter(Collider other)
    {

      
       GamePanel panel = UIMgr.Instance.GetPanel<GamePanel>();
       if (panel != null)
       {
           panel.UpdateSelTower(this);
           panel.nowSelTowerPoint = this;      
       }
    }

    private void OnTriggerExit(Collider other)
    {
     GamePanel panel = UIMgr.Instance.GetPanel<GamePanel>();
     if (panel != null)
     {
         panel.UpdateSelTower(null);
         panel.nowSelTowerPoint = null;
     }
    }
}
