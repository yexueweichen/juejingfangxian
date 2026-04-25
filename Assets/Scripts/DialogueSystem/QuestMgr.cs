using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestMgr : SingletonAutoMono<QuestMgr>
{
    public enum QuestState
    {
        NotStarted,   // 未开始
        InProgress,   // 进行中
        Completed,    // 已完成
        Failed        // 已失败
    }

    // 任务状态 + 任务进度
    private Dictionary<QuestData, QuestState> _states = new();
    private Dictionary<QuestData, int> _progress = new();

    // 事件
    public event Action<QuestData> OnQuestStarted;
    public event Action<QuestData, int, int> OnQuestProgress;
    public event Action<QuestData> OnQuestCompleted;
    public event Action<QuestData> OnQuestFailed;

    #region 核心公开方法
    /// <summary>
    /// 获取任务状态
    /// </summary>
    public QuestState GetState(QuestData q)
    {
        if (q == null) return QuestState.NotStarted;
        return _states.GetValueOrDefault(q, QuestState.NotStarted);
    }

    /// <summary>
    /// 【修复】获取任务进度（外部专用，无需操作字典）
    /// </summary>
    public int GetProgress(QuestData q)
    {
        if (q == null) return 0;
        return _progress.GetValueOrDefault(q, 0);
    }

    /// <summary>
    /// 【修复】开始任务（防重复启动，不重置进度）
    /// </summary>
    public void StartQuest(QuestData q)
    {
        if (q == null) return;
        // 禁止重复启动：已开始/已完成/已失败 都不执行
        if (GetState(q) != QuestState.NotStarted) return;

        _states[q] = QuestState.InProgress;
        _progress[q] = 0;
        OnQuestStarted?.Invoke(q);
    }

    /// <summary>
    /// 失败任务
    /// </summary>
    public void FailQuest(QuestData q)
    {
        if (q == null) return;
        _states[q] = QuestState.Failed;
        OnQuestFailed?.Invoke(q);
    }

    /// <summary>
    /// 【新增】提交任务（完成后调用，可扩展奖励发放）
    /// </summary>
    public void SubmitQuest(QuestData q)
    {
        if (q == null || GetState(q) != QuestState.Completed) return;
        // 这里可以添加发放奖励逻辑：PlayerData.Instance.AddGold(q.rewardGold);
        Debug.Log($"提交任务：{q.questName}，奖励金币：{q.rewardGold}");
    }

    /// <summary>
    /// 【新增】重置任务（重新接取）
    /// </summary>
    public void ResetQuest(QuestData q)
    {
        if (q == null) return;
        _states.Remove(q);
        _progress.Remove(q);
    }
    #endregion


    /// <summary>
    /// 通知关卡失败
    /// </summary>
    public void NotifyLevelFailed()
    {
        foreach (var kv in new Dictionary<QuestData, QuestState>(_states))
        {
            var q = kv.Key;
            if (kv.Value != QuestState.InProgress || q == null) continue;
            if (q.questType != QuestData.QuestType.ClearLevel) continue;

            _states[q] = QuestState.Failed;
            OnQuestFailed?.Invoke(q);
        }
    }

    /// <summary>
    /// 通知怪物被杀
    /// </summary>
    public void NotifyMonsterKilled(int monsterId)
    {
        foreach (var kv in new Dictionary<QuestData, QuestState>(_states))
        {
            var q = kv.Key;
            if (kv.Value != QuestState.InProgress || q == null) continue;
            if (q.questType != QuestData.QuestType.KillMonster) continue;
            if (q.targetMonsterId != monsterId) continue;

            UpdateQuestProgress(q, q.targetCount);
        }
    }

    /// <summary>
    /// 通知关卡完成
    /// </summary>
    public void NotifyLevelCompleted()
    {
        foreach (var kv in new Dictionary<QuestData, QuestState>(_states))
        {
            var q = kv.Key;
            if (kv.Value != QuestState.InProgress || q == null) continue;
            if (q.questType != QuestData.QuestType.ClearLevel) continue;

            UpdateQuestProgress(q, q.targetLevelCount);
        }
    }

    /// <summary>
    /// 更新任务进度
    /// </summary>
    private void UpdateQuestProgress(QuestData q, int requireCount)
    {
        // 漏洞修复：目标数量必须大于0
        if (requireCount <= 0)
        {
            _states[q] = QuestState.Completed;
            OnQuestCompleted?.Invoke(q);
            return;
        }

        // 优化：TryGetValue替代ContainsKey，更高效
        _progress.TryGetValue(q, out int now);
        now++;
        _progress[q] = now;

        OnQuestProgress?.Invoke(q, now, requireCount);

        // 进度达标 -> 完成任务
        if (now >= requireCount)
        {
            _states[q] = QuestState.Completed;
            OnQuestCompleted?.Invoke(q);
        }
    }
 
    
}