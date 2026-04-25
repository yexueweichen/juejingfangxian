using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhatPanel : BasePanel
{
    public Button surebt;

    protected override void Init()
    {
        surebt.onClick.AddListener(OnSureButtonClick);
    }
    private void OnSureButtonClick()
    {
        UIMgr.Instance.HidePanel<WhatPanel>();
    }
}
