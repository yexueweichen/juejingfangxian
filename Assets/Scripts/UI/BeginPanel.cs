using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    [SerializeField]
    private Button BeginButton;
    [SerializeField]
    private Button SettingButton;
    [SerializeField]
    private Button ExitButton;
    [SerializeField]
    private Button HelpButton;
        [SerializeField]
    private Button TaskButton;
    [SerializeField]
    private Button WhatButton;


    protected override void Init()
    {
        BeginButton.onClick.AddListener(OnBeginButtonClick);
        SettingButton.onClick.AddListener(OnSettingButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
        HelpButton.onClick.AddListener(OnHelpButtonClick);
        TaskButton.onClick.AddListener(OnTaskButtonClick);
        WhatButton.onClick.AddListener(OnWhatButtonClick);

   
    
    }

    //开始
    private void OnBeginButtonClick()
    {
      Camera.main.GetComponent<CameraMove>().Turnleft(() =>
        {
            UIMgr.Instance.ShowPanel<CharacterChoosePanel>();
            UIMgr.Instance.HidePanel<BeginPanel>();
        });
    }
    //设置
    private void OnSettingButtonClick()
    {
        UIMgr.Instance.ShowPanel<SettingPanel>();
    }
    //退出游戏
    private void OnExitButtonClick()
    {
        Application.Quit();
    }

    private void OnHelpButtonClick()
    {
        UIMgr.Instance.ShowPanel<HelpPanel>();
    }

    private void OnTaskButtonClick()
    {
       UIMgr.Instance.ShowPanel<ProgressPanel>();
       UIMgr.Instance.HidePanel<BeginPanel>();

        SceneMgr.Instance.LoadSceneAsync("GameScene4",()=>
        {
           
           
            UIMgr.Instance.HidePanel<ProgressPanel>(); 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            MusicMgr.Instance.PlayBKMusic("BKMusic4");
            
          
        }).Forget();
    }

    private void OnWhatButtonClick()
    {
        UIMgr.Instance.ShowPanel<WhatPanel>();
       
    }

}






   