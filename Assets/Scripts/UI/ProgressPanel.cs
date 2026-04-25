using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// 进度面板
/// </summary>
public class ProgressPanel:BasePanel
{
  //进度条图片
public Image proImage;
    //进度文本
public TextMeshProUGUI proText;

public Image bkGround;
public float hpWidth = 800f;

 protected override void Init()
    {
        if (proImage == null || proText == null)
        {
            Debug.LogError("资源为空");
            return;
        }
        UpdateProgress(0);
        //监听场景加载进度
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_SceneLoadChange, UpdateProgress);
        ABMgr.Instance.ClearAB();
       
    }


public void UpdateProgress(int now)
    {
        if (proText == null || proImage == null)
        {
            return;
        }
        
        proText.text = now + "/" + 100;
        proImage.rectTransform.sizeDelta = new Vector2(hpWidth * (float)now / hpWidth, proImage.rectTransform.sizeDelta.y);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_SceneLoadChange, UpdateProgress);
        
    }

 }




