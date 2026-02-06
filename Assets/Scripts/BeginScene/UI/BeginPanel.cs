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

    protected override void Init()
    {
        BeginButton.onClick.AddListener(OnBeginButtonClick);
        SettingButton.onClick.AddListener(OnSettingButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
   
    
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

        Debug.Log("setting Button Clicked");

        UIMgr.Instance.ShowPanel<SettingPanel>();




    }
    //退出游戏
    private void OnExitButtonClick()
    {
        Application.Quit();
    }
}






   