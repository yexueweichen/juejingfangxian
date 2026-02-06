using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{

   public Button sureButton;
    public  TextMeshProUGUI infoText;

    //ÌáÊ¾ÏÔÊ¾
    protected override void Init()
    {
        sureButton.onClick.AddListener(() =>

        {
            UIMgr.Instance.HidePanel<TipPanel>();

        });
      
    }

    public void SetInfo(string info)
    {
        infoText.text = info;
    }





}
