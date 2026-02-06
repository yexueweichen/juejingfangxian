using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel:BasePanel
{
   public Toggle musicToggle;
   public Toggle soundToggle;
   public Slider musicSlider;
   public Slider soundSlider;
    public Button closeButton;

    protected override void Init()
    {
       //获得音乐数据对象
        MusicData data = GameDataMgr.Instance.musicData;
        //根据数据对象 来初始化UI的显示
        musicSlider.value = data.musicValue;
        soundSlider.value = data.soundValue;
        musicToggle.isOn = data.musicOpen;
        soundToggle.isOn = data.soundOpen;
        //音乐声量大小
        musicSlider.onValueChanged.AddListener((v) =>
        {
            BKMusic.Instance.ChangeValue(v);
            GameDataMgr.Instance.musicData.musicValue = v;
        });
       //音效大小
        soundSlider.onValueChanged.AddListener((v) =>
        {
          GameDataMgr.Instance.musicData.soundValue = v;
        });
       //音乐开关
        musicToggle.onValueChanged.AddListener((isOn) =>
        {
            BKMusic.Instance.SetIsOpen(isOn);
            GameDataMgr.Instance.musicData.musicOpen = isOn;
        });
      //音效开关
        soundToggle.onValueChanged.AddListener((isOn) =>
        {
           GameDataMgr.Instance.musicData.soundOpen = isOn;
        });
        //关闭按钮
        closeButton.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<SettingPanel>();
            GameDataMgr.Instance.SaveMusicData();
        });
    }

}
