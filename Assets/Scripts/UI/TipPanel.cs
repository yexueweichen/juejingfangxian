using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 提示面板
/// </summary>
public class TipPanel : BasePanel
{

//确定按钮
   public Button sureButton;
//提示信息
   public  TextMeshProUGUI infoText;

   protected override void Init()
    {
       sureButton.onClick.AddListener(() =>

        {
            UIMgr.Instance.HidePanel<TipPanel>();

        });
      
    }
 //设置提示信息
    public void SetInfo(string info)
    {
        infoText.text = info;
    }





}
