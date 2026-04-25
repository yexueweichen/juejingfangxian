using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 关卡选择面板
/// </summary>
public class LevelPanel : BasePanel
{
   //开始按钮
    public Button startButton;
    //返回按钮
    public Button backButton;
    //上一关按钮
    public Button leftButton;
    //下一关按钮
    public Button rightButton;
    //当前关卡文本
    public TextMeshProUGUI levelText;
    //当前关卡文本2
    public TextMeshProUGUI levelText2;
    //当前关卡图片
    public Image levelImage;
    //当前关卡索引
    public int nowLevelIndex = 0;
    //当前场景信息
    public SceneInfos.SceneInfo nowSceneInfo;

    protected override void Init()
    {  
        UpdateLevelInfo();
        startButton.onClick.AddListener(() =>
        {
          ConfigMgr.Instance.sceneSeInfo = nowSceneInfo;
            UIMgr.Instance.ShowPanel<ProgressPanel>();
            UIMgr.Instance.HidePanel<LevelPanel>();
  
            SceneMgr.Instance.LoadSceneAsync(nowSceneInfo.sceneName,
            ()=>
            {
               UIMgr.Instance.HidePanel<ProgressPanel>(); 
                UIMgr.Instance.ShowPanel<GamePanel>((panel)=>{

                    //初始化游戏关卡管理器
                    GameLevelMgr.Instance.InitInfo(nowSceneInfo);
                    MusicMgr.Instance.PreloadBKMusic(nowSceneInfo.BKMusic, "BossMusic", "BKMusic4");
                    MusicMgr.Instance.PlayBKMusic(nowSceneInfo.BKMusic);
                      Cursor.lockState = CursorLockMode.Locked;
                       Cursor.visible = false;
               },true);
               
            }
            ).Forget();

        });
    
   //返回
        backButton.onClick.AddListener(() => {
    
    UIMgr.Instance.HidePanel<LevelPanel>();
        UIMgr.Instance.ShowPanel<BeginPanel>();

    }
        );

        //左一个
        leftButton.onClick.AddListener(() => {
        
        --nowLevelIndex;
            if(nowLevelIndex < 0)
            {
               nowLevelIndex = ConfigMgr.Instance.sceneInfos.SceneDatas.Count-1;
            }
            UpdateLevelInfo();

        }
        );
        //右一个
        rightButton.onClick.AddListener(() => {

            ++nowLevelIndex;
           if (nowLevelIndex > ConfigMgr.Instance.sceneInfos.SceneDatas.Count - 1)
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
        if (ConfigMgr.Instance.sceneInfos == null || ConfigMgr.Instance.sceneInfos.SceneDatas == null)
        {
            return;
        }
        
        if (nowLevelIndex >= 0 && nowLevelIndex < ConfigMgr.Instance.sceneInfos.SceneDatas.Count)
        {
            nowSceneInfo = ConfigMgr.Instance.sceneInfos.SceneDatas[nowLevelIndex];
            if (nowSceneInfo != null)
            {
                levelText.text = "名称：" + nowSceneInfo.name;
                levelText2.text = "简介：" + nowSceneInfo.tips;
                if (!string.IsNullOrEmpty(nowSceneInfo.imgRes))
                {
                    ABMgr.Instance.LoadResAsync<Sprite>("img", nowSceneInfo.imgRes, (sprite) =>
                    {
                        levelImage.sprite = sprite;
                    }).Forget();
                }
            }
        }
    }
















}
