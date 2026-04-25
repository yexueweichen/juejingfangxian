using UnityEngine;

public class Npc : MonoBehaviour
{
    public QuestData questData;
    public DgData beforeAccept;
    public DgData afterAccept;
    public DgData afterComplete;
    public DgData afterFailed;
    private bool _playerInside = false;
    private bool enableE = true;

    void OnEnable()
    {
        // 安全订阅
        if (DgMgr.Instance != null)
        {
            DgMgr.Instance.OnChoiceSelected += OnNpcChoiceSelected;
            // 【新增】监听对话结束 → 恢复交互（核心修复）
            DgMgr.Instance.OnDialogueClosed += OnDialogueClosed;
        }
        if (QuestMgr.Instance != null)
        {
            QuestMgr.Instance.OnQuestCompleted += OnNpcQuestCompleted;
            QuestMgr.Instance.OnQuestFailed += OnNpcQuestFailed;
        }
    }

    void OnDisable()
    {
        // 安全取消订阅
        if (DgMgr.Instance != null)
        {
            DgMgr.Instance.OnChoiceSelected -= OnNpcChoiceSelected;
            DgMgr.Instance.OnDialogueClosed -= OnDialogueClosed;
        }
        if (QuestMgr.Instance != null)
        {
            QuestMgr.Instance.OnQuestCompleted -= OnNpcQuestCompleted;
            QuestMgr.Instance.OnQuestFailed -= OnNpcQuestFailed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInside = true;
            enableE = true;
            // 空安全判断
            if (UIMgr.Instance != null)
                UIMgr.Instance.ShowPanel<TipUI>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInside = false;
            // 空安全判断
            if (UIMgr.Instance != null)
                UIMgr.Instance.HidePanel<TipUI>();
            // 退出范围重置按键
            enableE = false;
        }
    }

    void Update()
    {
        if (!_playerInside || !enableE) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartDialogueForState();
            enableE = false; // 按下后禁用，防止重复触发
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (UIMgr.Instance != null)
                UIMgr.Instance.HidePanel<TipUI>();
        }
    }

    private void StartDialogueForState()
    {
        if (DgMgr.Instance == null) return;

        // 防止重复打开对话
        if (DgMgr.Instance.IsActive) return;

        if (questData == null)
        {
            if (beforeAccept != null)
                DgMgr.Instance.StartDialogue(beforeAccept, this);
            return;
        }

        var state = QuestMgr.Instance.GetState(questData);
        DgData dlg = state switch
        {
            QuestMgr.QuestState.NotStarted => beforeAccept,
            QuestMgr.QuestState.InProgress => afterAccept,
            QuestMgr.QuestState.Completed => afterComplete,
            QuestMgr.QuestState.Failed => afterFailed,
            _ => null
        };

        if (dlg != null)
            DgMgr.Instance.StartDialogue(dlg, this);
    }


    private void OnDialogueClosed()
    {
        enableE = true;
        if (_playerInside && UIMgr.Instance != null)
        {
            UIMgr.Instance.ShowPanel<TipUI>();
        }
        // 恢复游戏鼠标状态
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnNpcChoiceSelected(int choiceIndex, object context)
    {
        if (context as Npc != this || questData == null) return;

        var state = QuestMgr.Instance.GetState(questData);

        // 接受任务
        if (state == QuestMgr.QuestState.NotStarted && choiceIndex == 0)
        {
            QuestMgr.Instance.StartQuest(questData);
        }
        // 拒绝任务
        else if (state == QuestMgr.QuestState.NotStarted && choiceIndex == 1)
        {
            Debug.Log("玩家拒绝了任务");
        }
    }

    private void OnNpcQuestCompleted(QuestData q)
    {
        if (q != questData) return;

        if (_playerInside && afterComplete != null && DgMgr.Instance != null)
        {
            DgMgr.Instance.StartDialogue(afterComplete, this);
            DgMgr.Instance.OnDialogueClosed += () => ShowQuestCompletePanel(q);
        }
        else
        {
            ShowQuestCompletePanel(q);
        }
    }

    private void ShowQuestCompletePanel(QuestData q)
    {
        if (UIMgr.Instance != null)
        {
            UIMgr.Instance.ShowPanel<QuestCompletePanel>((panel) =>
            {
                if (panel != null)
                    panel.InitInfo(q.questName, q.rewardGold);
            });
        }
    }

    private void OnNpcQuestFailed(QuestData q)
    {
        if (q != questData) return;

        if (_playerInside && afterFailed != null && DgMgr.Instance != null)
        {
            DgMgr.Instance.StartDialogue(afterFailed, this);
        }
    }
}