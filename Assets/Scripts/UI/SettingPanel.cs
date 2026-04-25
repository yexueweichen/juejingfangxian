using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置面板
/// </summary>
public class SettingPanel:BasePanel
{
   public Toggle musicToggle;
   public Toggle soundToggle;

    public Slider musicSlider;
   public Slider soundSlider;
   
    public Button closeButton;

    protected override void Init()
    {
       //获取数据对象
        MusicData data = SaveMgr.Instance.musicData;
        //根据数据对象 来初始化UI的显示
        musicSlider.value = data.musicValue;
        soundSlider.value = data.soundValue;
        musicToggle.isOn = data.musicOpen;
        soundToggle.isOn = data.soundOpen;
        //音乐声量大小
        musicSlider.onValueChanged.AddListener((v) =>
        {
            SaveMgr.Instance.musicData.musicValue = v;
            MusicMgr.Instance.ChangeBKMusicValue(v);
        });
       //音效大小
        soundSlider.onValueChanged.AddListener((v) =>
        {
          SaveMgr.Instance.musicData.soundValue = v;
          MusicMgr.Instance.ChangeSoundValue(v);
        });
       //音乐开关
        musicToggle.onValueChanged.AddListener((isOn) =>
        {
            if(isOn==true)
            MusicMgr.Instance.PlayBKMusic("BKMusic");
            else
            MusicMgr.Instance.PauseBKMusic();
            SaveMgr.Instance.musicData.musicOpen = isOn;
        });
      //音效开关
        soundToggle.onValueChanged.AddListener((isOn) =>
        {
            MusicMgr.Instance.PlayOrPauseSound(isOn);
            SaveMgr.Instance.musicData.soundOpen = isOn;
        });
        //关闭按钮
        closeButton.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<SettingPanel>();
            SaveMgr.Instance.SaveMusicData();
        });
    }

}
