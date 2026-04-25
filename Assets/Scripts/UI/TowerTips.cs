using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

/// <summary>
///  防御塔图标
/// </summary>
public class TowerTips : MonoBehaviour
{
     //炮塔图片
    public Image imgPic;

    // 按键名称
    public TextMeshProUGUI txtTip;

    // 所需能量
    public TextMeshProUGUI energy;

    // 电池图片
    public Image dianchi;


    /// <summary>
    /// 初始化 按钮信息的方法
    /// </summary>
    /// <param name="id">炮塔ID</param>
    /// <param name="inputStr">按键名称</param>
    public void InitInfo(int id, string inputStr)
    {
 
        TowerInfos.TowerInfo info = ConfigMgr.Instance.towerInfos.TowerDatas[id - 1];


        if (imgPic != null)
        {
            ABMgr.Instance.LoadResAsync<Sprite>("img", info.imgRes, (sprite) =>
            {
                if (sprite != null)
                {
                    imgPic.sprite = sprite;
                    imgPic.enabled = true;
       
                }
               
            }, true).Forget();
        }

        if (energy != null)
            energy.text = info.energy.ToString();
        if (txtTip != null)
            txtTip.text = inputStr;
       
        //判断 能量够不够
        if (GameLevelMgr.Instance.currentSceneInfo != null && info.energy > GameLevelMgr.Instance.currentSceneInfo.energy)
        {
            if (energy != null)
                energy.text = "能量不足";
            dianchi.enabled = false;

        }
    }
}
