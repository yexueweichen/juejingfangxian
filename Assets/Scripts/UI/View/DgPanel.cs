using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DgPanel : BasePanel
{
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI contentText;
    public Button continueButton;
    public GameObject choiceContainer;
    public Button choiceButtonPrefab;

    private List<GameObject> _instancedChoices = new List<GameObject>();

    protected override void Init()
    {
        
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => DgMgr.Instance.OnContinue());
        }
    }

    public void ShowSentence(string speaker, string content, Action onContinue)
    {
        ClearChoices();

        // 空安全赋值，绝不报错
        if (speakerText != null) speakerText.text = speaker;
        if (contentText != null) contentText.text = content;

        // 显示继续按钮
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => onContinue?.Invoke());
        }

        // 空安全！有容器才隐藏，没有就跳过
        if (choiceContainer != null)
            choiceContainer.SetActive(false);
    }

    public void ShowChoice(string speaker, string content, List<DgData.Choice> choices, Action<int> onChoice)
    {
        ClearChoices();

        // 空安全
        if (speakerText != null) speakerText.text = speaker;
        
        if (contentText != null) contentText.text = content;

        // 隐藏继续按钮
        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        
        if (choiceContainer != null && choiceButtonPrefab != null && choices != null && choices.Count > 0)
        {
            choiceContainer.SetActive(true);

            for (int i = 0; i < choices.Count; i++)
            {
                var btn = Instantiate(choiceButtonPrefab, choiceContainer.transform);
                var index = i;
                var txt = btn.GetComponentInChildren<TMP_Text>();

                if (txt != null) txt.text = choices[i].text;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => onChoice?.Invoke(index));

                _instancedChoices.Add(btn.gameObject);
            }
        }
    }

    // 清空选项
    public void ClearChoices()
    {
        foreach (var go in _instancedChoices)
        {
            if (go != null) Destroy(go);
        }
        _instancedChoices.Clear();
    }

    public void HideImmediate()
    {
        ClearChoices();
        gameObject.SetActive(false);
    }
}