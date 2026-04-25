using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackPanel : BasePanel
{
    public Button backButton;
    public Button closeButton;

    public Button settingButton;


    

    protected override void Init()
    {
        backButton.onClick.AddListener(() =>
        {
            UIMgr.Instance.ShowPanel<ProgressPanel>();
                UIMgr.Instance.HidePanel<BackPanel>();
            UIMgr.Instance.HidePanel<GamePanel>();
            SceneMgr.Instance.LoadSceneAsync("BeginScene", () =>
            {
                UIMgr.Instance.HidePanel<ProgressPanel>();
                 MusicMgr.Instance.PlayBKMusic("BKMusic");
                Time.timeScale = 1;
            }).Forget();
        
        });
        closeButton.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<BackPanel>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        });
        settingButton.onClick.AddListener(() =>
        {
            UIMgr.Instance.ShowPanel<SettingPanel>();
        });
   
   
   
   
   
   
   
   
   
   
    }
}
