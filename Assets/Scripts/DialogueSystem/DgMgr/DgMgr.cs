using System;
using System.Collections.Generic;
using UnityEngine;

public class DgMgr : SingletonAutoMono<DgMgr>
{
    private DgData current;
    private int currentIndex = -1;
    private object currentContext;
    private bool isBranchEnd = false;

    private DgPanel _currentPanel;

    // 选项选择事件
    public event Action<int, object> OnChoiceSelected;
   
    public event Action OnDialogueClosed;
    // 是否正在对话中
    public bool IsActive { get; private set; } = false;

    // 开始对话
    public void StartDialogue(DgData data, object context = null)
    {
        
        if (IsActive || data == null)
            return;

        current = data;
        currentContext = context;
        currentIndex = Math.Clamp(data.startIndex, 0, Math.Max(0, data.nodes.Count - 1));
        isBranchEnd = false;
        IsActive = true;
        
        UIMgr.Instance.ShowPanel<DgPanel>((panel) =>
        {
            _currentPanel = panel;
            ShowCurrentNode();
        });
    }

    // 显示当前节点
    private void ShowCurrentNode()
    {
        if (_currentPanel == null || current == null || currentIndex < 0 || currentIndex >= current.nodes.Count)
        {
            StopDialogue();
            return;
        }

        var node = current.nodes[currentIndex];
        if (node.isChoice)
        {
            _currentPanel.ShowChoice(node.speaker, node.content, node.choices, OnChoiceButton);
        }
        else
        {
            _currentPanel.ShowSentence(node.speaker, node.content, OnContinue);
        }
    }

    // 继续对话
    public void OnContinue()
    {
        if (current == null) 
        { StopDialogue(); return; }
        
        if (isBranchEnd)
        { StopDialogue(); return; }
        
        // 默认取 next = currentIndex + 1；如果没有则结束
        int next = currentIndex + 1;
        if (next >= current.nodes.Count) 
        { StopDialogue(); return; }
        
        currentIndex = next;
        ShowCurrentNode();
    }

    // 选择选项
    private void OnChoiceButton(int choiceIndex)
    {
        if (current == null) { StopDialogue(); return; }

        var node = current.nodes[currentIndex];
        if (node == null || node.choices == null || choiceIndex < 0 || choiceIndex >= node.choices.Count)
        {
            StopDialogue();
            return;
        }

        OnChoiceSelected?.Invoke(choiceIndex, currentContext);

        int target = node.choices[choiceIndex].targetNodeIndex;
        if (target < 0 || target >= current.nodes.Count)
        {
            StopDialogue();
            return;
        }

        isBranchEnd = true;
        currentIndex = target;
        ShowCurrentNode();
    }

    // 结束对话
    public void StopDialogue()
    {
        IsActive = false;
        current = null;
        currentContext = null;
        currentIndex = -1;

        OnChoiceSelected = null;

        if (_currentPanel != null)
        {
            UIMgr.Instance.HidePanel<DgPanel>();
            _currentPanel = null;
        }

        OnDialogueClosed?.Invoke();
    }
}
