using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "SimpleQuest/QuestData")]
public class QuestData : ScriptableObject
{
    [Header("基础信息")]
    public string questId;
    public string questName;
    [TextArea] public string description;

    public enum QuestType
    {
        KillMonster,    // 0 杀怪
        ClearLevel      // 1 通关关卡
    }
    public QuestType questType;

    [Header("对话配置")]
    public DgData dialogueBeforeAccept;   // 接受前
    public DgData dialogueAfterAccept;    // 接受后
    public DgData dialogueAfterComplete;  // 完成后
    public DgData dialogueAfterFailed;    // 失败后

    [Header("杀怪任务目标")]
    public int targetMonsterId;
    public int targetCount = 1;

    [Header("通关任务目标")]
    public int targetLevelCount = 3;

    [Header("奖励")]
    public int rewardGold = 200;
}