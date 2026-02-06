using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPanel : BasePanel
{
   
    public Button startButton;
    public Button backButton;
    public Button leftButton;
    public Button rightButton;
    public TextMeshProUGUI levelText;
    public Image levelImage;
    public int nowLevelIndex = 0;
    public SceneInfo nowSceneInfo;



    protected override void Init()
    {
        //初始化关卡信息
        UpdateLevelInfo();
        //开始游戏按钮
        startButton.onClick.AddListener(() =>
        {
            //跳转场景
            UIMgr.Instance.HidePanel<LevelPanel>();
            AsyncOperation ao = SceneManager.LoadSceneAsync(nowSceneInfo.sceneName);
            ao.completed += (obj) =>
            {
               
                GameLevelMgr.Instance.InitInfo(nowSceneInfo);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

            };
        }
           );
    
   //返回
        backButton.onClick.AddListener(() => {
    
    UIMgr.Instance.HidePanel<LevelPanel>();
        UIMgr.Instance.ShowPanel<CharacterChoosePanel>();



    }
        );

        //左一个
        leftButton.onClick.AddListener(() => {
        
        --nowLevelIndex;
            if(nowLevelIndex < 0)
            {
                nowLevelIndex = GameDataMgr.Instance.sceneInfos.Count-1;
            }
            UpdateLevelInfo();

        }
        );
        //右一个
        rightButton.onClick.AddListener(() => {

            ++nowLevelIndex;
            if (nowLevelIndex > GameDataMgr.Instance.sceneInfos.Count - 1)
            {
                nowLevelIndex = 0;
            }

            UpdateLevelInfo();
        }
        );


    }

    //更新显示的关卡
    private void UpdateLevelInfo()
    {
             
        nowSceneInfo= GameDataMgr.Instance.sceneInfos[nowLevelIndex];
        levelText.text = "名称："+nowSceneInfo.name;
        levelText.text += "\n" + "描述："+nowSceneInfo.tips;
        levelImage.sprite=  Resources.Load<Sprite>(nowSceneInfo.imgRes);

    }
















}
