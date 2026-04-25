using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 游戏面板
/// </summary>
public class GamePanel : BasePanel
{
    //当前塔血量UI图片
    public Image hpImage;
    //当前玩家血量UI图片
    public Image playerHpImage;
    //当前血量UI文字
    public TextMeshProUGUI hpText;
    //当前玩家血量UI文字
    public TextMeshProUGUI playerHpText;
    //当前能量UI文字
    public TextMeshProUGUI energyText;
    //当前敌人波数UI文字
    public TextMeshProUGUI enemyWaveText;
    public float hpWidth = 800f;
   //TowerTips根对象
    public Transform Root;
   //管理towertips
    public List<TowerTips> towerTips;
   //防御塔信息
    private TowerInfos useTowerInfos;
   
//当前进入和选中的造塔点 
public TowerPoint nowSelTowerPoint;

//是否检测造塔点输入
private bool checkInput;

    protected override void Init()
    {
        Root.gameObject.SetActive(false);
        useTowerInfos = ConfigMgr.Instance.towerInfos;
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_AddEnergy, AddEnergy);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_ReduceEnergy, ReduceEnergy);
        EventCenter.Instance.AddEventListener(E_EventType.E_UpdateTowerHp, InitGLMgrTH);
        EventCenter.Instance.AddEventListener(E_EventType.E_UpdateTEnergy, InitGLMgrEG);

        if (towerTips == null || towerTips.Count == 0)
            Debug.LogWarning("towerTips 未在 Inspector 中正确绑定或为空。");
    }

    protected virtual void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_AddEnergy, AddEnergy);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_ReduceEnergy, ReduceEnergy);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_UpdateTowerHp, InitGLMgrTH);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_UpdateTEnergy, InitGLMgrEG);
    }

//初始化能量值
public void InitGLMgrEG()
{
energyText.text = GameLevelMgr.Instance.currentSceneInfo.energy.ToString();
} 
//初始化血量值
public void InitGLMgrTH()
{
   UpdateTowerHp(GameLevelMgr.Instance.currentSceneInfo.towerHp, GameLevelMgr.Instance.currentSceneInfo.towerHp);
   UpdatePlayerHp(ConfigMgr.Instance.roleSeInfo.hp, ConfigMgr.Instance.roleSeInfo.hp);
}

    // 更新主塔血量UI
    public void UpdateTowerHp(int hp, int maxHP)
    {
        // 添加空值检查，防止对象被销毁后访问
        if (hpText == null || hpImage == null || hpImage.rectTransform == null)
        {
            Debug.LogWarning("GamePanel.UpdateTowerHp: UI组件为空，可能已被销毁");
            return;
        }
        
        hpText.text = hp + "/" + maxHP;
        if (maxHP <= 0)
        {
            hpImage.rectTransform.sizeDelta = new Vector2(0, hpImage.rectTransform.sizeDelta.y);
            return;
        }
        float ratio = Mathf.Clamp01((float)hp / maxHP);
        hpImage.rectTransform.sizeDelta = new Vector2(hpWidth * ratio, hpImage.rectTransform.sizeDelta.y);
    }
//更新玩家血量UI
public void UpdatePlayerHp(int hp, int maxHP)
{
    if (playerHpText == null || playerHpImage == null || playerHpImage.rectTransform == null)
    {
        Debug.LogWarning("GamePanel.UpdatePlayerHp: UI组件为空，可能已被销毁");
        return;
    }
    playerHpText.text = hp + "/" + maxHP;
    if (maxHP <= 0)
    {
        playerHpImage.rectTransform.sizeDelta = new Vector2(0, playerHpImage.rectTransform.sizeDelta.y);
        return;
    }
    float ratio = Mathf.Clamp01((float)hp / maxHP);
    playerHpImage.rectTransform.sizeDelta = new Vector2(hpWidth * ratio, playerHpImage.rectTransform.sizeDelta.y);
}

    //增加能量值更新UI
    public void AddEnergy(int energy)
    {
        GameLevelMgr.Instance.currentSceneInfo.energy += energy;
       energyText.text = GameLevelMgr.Instance.currentSceneInfo.energy.ToString();
    }
    //减少能量值更新UI
    public void ReduceEnergy(int energy)
    {
        GameLevelMgr.Instance.currentSceneInfo.energy -= energy;
       energyText.text = GameLevelMgr.Instance.currentSceneInfo.energy.ToString();
    }
    
    //更新敌人波数UI
    public void UpdateEnemy(int enemy,int enemyMax)
    {
        if (enemyWaveText != null)
        {
            enemyWaveText.text = enemy + "/" + enemyMax;
        }
    }

    public override void Update()
    {
        base.Update();
        //checkinput为false，造塔点为空return
        if (!checkInput)
            return;
        if (nowSelTowerPoint == null)
            return;

        //造塔功能
        if (nowSelTowerPoint.nowtowerinfo == null || nowSelTowerPoint.nowtowerinfo.id == 10)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

                if (useTowerInfos != null && useTowerInfos.TowerDatas.Count > 0)
                    nowSelTowerPoint.createtower(useTowerInfos.TowerDatas[0].id).Forget();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {

                if (useTowerInfos != null && useTowerInfos.TowerDatas.Count > 0)
                    nowSelTowerPoint.createtower(useTowerInfos.TowerDatas[3].id).Forget();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {

                if (useTowerInfos != null && useTowerInfos.TowerDatas.Count > 0)
                    nowSelTowerPoint.createtower(useTowerInfos.TowerDatas[6].id).Forget();
            }
        }
        //造过塔 就检测E键 去升级
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {

                if (nowSelTowerPoint.nowtowerinfo.next != 0)
                    nowSelTowerPoint.createtower(nowSelTowerPoint.nowtowerinfo.next).Forget();
            }
        }
    }
    /// <summary>
    /// 更新当前选中造塔点界面
    /// </summary>
    public void UpdateSelTower(TowerPoint point)
{
    nowSelTowerPoint = point;
    //如果传入数据是空
    if (nowSelTowerPoint == null)
    {
        checkInput = false;
        //隐藏下方造塔按钮
    Root.gameObject.SetActive(false);
    }
    else
    {
        checkInput = true;
        //显示下方造塔按钮
        Root.gameObject.SetActive(true);

        //如果没有造过塔
        if (nowSelTowerPoint.nowtowerinfo == null || nowSelTowerPoint.nowtowerinfo.id == 10)
        {
            for (int i = 0; i < towerTips.Count; i++)
            {
                towerTips[i].gameObject.SetActive(true);
                // 每个按钮对应数据表中的 0,3,6 索引
                int dataIndex = i * 3;
                if (useTowerInfos != null && dataIndex < useTowerInfos.TowerDatas.Count)
                {
                    towerTips[i].InitInfo(useTowerInfos.TowerDatas[dataIndex].id, "数字键" + (i + 1));
                }
                else
                {
                    towerTips[i].gameObject.SetActive(false);
                    Debug.LogWarning($"Tower 数据表索引越界 dataIndex={dataIndex}");
                }
            }
        }
        //如果造过塔
        else
        {
            //先隐藏所有 towerTips
            for (int i = 0; i < towerTips.Count; i++)
            {
                towerTips[i].gameObject.SetActive(false);
            }
            
            //只显示一个升级按钮
            if (nowSelTowerPoint.nowtowerinfo.next != 0)
            {
                //检查 chooseIndex 是否有效
                if (point.chooseIndex >= 0 && point.chooseIndex < towerTips.Count)
                {
                    towerTips[point.chooseIndex].gameObject.SetActive(true);
                    towerTips[point.chooseIndex].InitInfo(nowSelTowerPoint.nowtowerinfo.next, "E 键");
                }
                else
                {
                    //如果 chooseIndex 无效，使用第一个
                    towerTips[0].gameObject.SetActive(true);
                    towerTips[0].InitInfo(nowSelTowerPoint.nowtowerinfo.next, "E 键");
                }
            }
        }
    }
}
}
