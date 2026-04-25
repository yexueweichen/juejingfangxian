using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestCompletePanel : BasePanel
{
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI rewardText;
    public Button confirmButton;

    protected override void Init()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => UIMgr.Instance.HidePanel<QuestCompletePanel>());
        }
    }

    public void InitInfo(string questName, int rewardGold)
    {
        if (questNameText != null)
        {
            questNameText.text = questName;
        }
        
        if (rewardText != null)
        {
            rewardText.text = $"奖励金币：{rewardGold}";
        }
    }
}
