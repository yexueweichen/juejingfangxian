using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 按钮音效类型枚举
/// </summary>
public enum ButtonSoundType
{
    None,       // 无音效
    ButtonClick,// 普通按钮点击音效
    LeftRight   // 左右选择音效
}

/// <summary>
/// 按钮动效类型枚举
/// </summary>
public enum ButtonEffectType
{
    Scale,  // 缩放动效
    Color,  // 颜色动效
    Shake,  // 抖动动效
    Pulse,  // 脉冲动效
    Bounce  // 弹跳动效
}

/// <summary>
/// 按钮交互动效组件
/// 支持鼠标悬停、按下、释放、点击等多种状态的动画效果
/// </summary>
public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("按钮音效类型")]
    public ButtonSoundType soundType = ButtonSoundType.ButtonClick;

    [Header("动效类型")]
    public ButtonEffectType effectType = ButtonEffectType.Scale;

    [Header("动效时长")]
    [SerializeField]
    private float effectDuration = 0.2f;

    [Header("缩放设置")]
    [SerializeField]
    private float hoverScale = 1.15f;     // 悬停时的缩放比例
    [SerializeField]
    private float pressScale = 0.9f;       // 按下时的缩放比例
    [SerializeField]
    private float clickScale = 1.25f;     // 点击时的缩放比例

    [Header("颜色设置")]
    [SerializeField]
    private bool useColorEffect = false;  // 是否启用颜色动效
    [SerializeField]
    private Color hoverColor = new Color(1f, 0.9f, 0.8f, 1f);    // 悬停颜色
    [SerializeField]
    private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f); // 按下颜色
    [SerializeField]
    private Color clickColor = new Color(1f, 0.8f, 0.6f, 1f);   // 点击颜色

    [Header("抖动设置")]
    [SerializeField]
    private float shakeStrength = 10f;    // 抖动强度
    [SerializeField]
    private int shakeVibrato = 3;         // 抖动频率

    [Header("脉冲设置")]
    [SerializeField]
    private float pulseStrength = 0.1f;   // 脉冲强度
    [SerializeField]
    private float pulseDuration = 0.3f;   // 脉冲时长

    private Button button;                // 按钮组件引用
    private Image buttonImage;             // 按钮图片组件引用
    private Text buttonText;               // 按钮文字组件引用
    private Color originalImageColor;      // 原始图片颜色
    private Color originalTextColor;       // 原始文字颜色
    private Vector3 originalScale;         // 原始缩放值
    private bool isInteractable = true;    // 当前交互状态
    private Tween currentTween;            // 当前播放的Tween动画

    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// 初始化按钮组件，获取所需引用并设置默认状态
    /// </summary>
    private void Initialize()
    {
        // 获取或添加Button组件
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }

        // 获取Image组件用于颜色动效
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            originalImageColor = buttonImage.color;
        }

        // 获取Text组件用于文字颜色动效
        buttonText = GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            originalTextColor = buttonText.color;
        }

        // 保存原始缩放值
        originalScale = transform.localScale;
        isInteractable = button.interactable;

        // 清除旧监听器并添加点击回调
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnEnable()
    {
        // 启用时重置到原始状态
        ResetToOriginalState();
    }

    /// <summary>
    /// 按钮点击时的回调处理
    /// </summary>
    private void OnButtonClick()
    {
        // 如果按钮不可交互则直接返回
        if (!isInteractable)
            return;

        // 播放音效
        PlaySound();

        // 根据动效类型播放对应效果
        switch (effectType)
        {
            case ButtonEffectType.Scale:
                PlayScaleClickEffect();
                break;
            case ButtonEffectType.Color:
                PlayColorClickEffect();
                break;
            case ButtonEffectType.Shake:
                PlayShakeEffect();
                break;
            case ButtonEffectType.Pulse:
                PlayPulseEffect();
                break;
            case ButtonEffectType.Bounce:
                PlayBounceEffect();
                break;
        }
    }

    /// <summary>
    /// 播放按钮音效
    /// </summary>
    private void PlaySound()
    {
        if (soundType == ButtonSoundType.None)
            return;

        // 根据音效类型选择对应的音效资源名称
        string soundName = soundType == ButtonSoundType.ButtonClick ? "ButtonClick" : "LeftRight";
        MusicMgr.Instance.PlaySound(soundName);
    }

    /// <summary>
    /// 播放缩放点击动效
    /// 点击时快速放大后回弹到悬停大小，最后恢复原始大小
    /// </summary>
    private void PlayScaleClickEffect()
    {
        currentTween?.Kill();

        // 创建序列动画：点击放大 -> 回弹到悬停大小 -> 弹性恢复原始大小
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(clickScale, effectDuration * 0.5f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale.x * hoverScale, effectDuration * 0.3f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale.x, effectDuration * 0.2f).SetEase(Ease.OutElastic));

        currentTween = seq;
    }

    /// <summary>
    /// 播放颜色点击动效
    /// 点击时颜色变化序列：点击色 -> 悬停色 -> 原始色
    /// </summary>
    private void PlayColorClickEffect()
    {
        currentTween?.Kill();

        Sequence seq = DOTween.Sequence();

        // 图片颜色变化序列
        if (buttonImage != null)
        {
            seq.Append(buttonImage.DOColor(clickColor, effectDuration * 0.3f));
            seq.Append(buttonImage.DOColor(hoverColor, effectDuration * 0.4f));
            seq.Append(buttonImage.DOColor(originalImageColor, effectDuration * 0.3f));
        }

        // 文字颜色同步变化
        if (buttonText != null)
        {
            seq.Join(buttonText.DOColor(clickColor, effectDuration * 0.3f));
            seq.Join(buttonText.DOColor(originalTextColor, effectDuration * 0.7f));
        }

        currentTween = seq;
    }

    /// <summary>
    /// 播放抖动动效
    /// </summary>
    private void PlayShakeEffect()
    {
        currentTween?.Kill();
        currentTween = transform.DOShakePosition(effectDuration, shakeStrength, shakeVibrato)
            .OnComplete(() => transform.localPosition = Vector3.zero);
    }

    /// <summary>
    /// 播放脉冲动效
    /// 快速脉冲放大缩小后恢复
    /// </summary>
    private void PlayPulseEffect()
    {
        currentTween?.Kill();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originalScale.x * (1f + pulseStrength), pulseDuration * 0.5f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale.x * (1f - pulseStrength * 0.5f), pulseDuration * 0.3f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale.x, pulseDuration * 0.2f).SetEase(Ease.OutElastic));

        currentTween = seq;
    }

    /// <summary>
    /// 播放弹跳动效
    /// 先放大后产生 Punch 效果
    /// </summary>
    private void PlayBounceEffect()
    {
        currentTween?.Kill();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(clickScale, effectDuration * 0.4f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOPunchScale(Vector3.one * 0.2f, effectDuration * 0.6f, 2, 0.5f));

        currentTween = seq;
    }

    /// <summary>
    /// 鼠标进入按钮时的回调
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable)
            return;

        currentTween?.Kill();

        // 如果启用颜色效果或当前是颜色模式，则应用悬停颜色
        if (useColorEffect || effectType == ButtonEffectType.Color)
        {
            if (buttonImage != null)
            {
                buttonImage.DOColor(hoverColor, effectDuration * 0.5f);
            }
            if (buttonText != null)
            {
                buttonText.DOColor(hoverColor, effectDuration * 0.5f);
            }
        }

        // 应用悬停缩放
        transform.DOScale(hoverScale, effectDuration * 0.5f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 鼠标离开按钮时的回调
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable)
            return;

        ResetToOriginalState();
    }

    /// <summary>
    /// 鼠标按下按钮时的回调
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable)
            return;

        currentTween?.Kill();

        // 应用按下颜色
        if (useColorEffect || effectType == ButtonEffectType.Color)
        {
            if (buttonImage != null)
            {
                buttonImage.DOColor(pressedColor, effectDuration * 0.3f);
            }
            if (buttonText != null)
            {
                buttonText.DOColor(pressedColor, effectDuration * 0.3f);
            }
        }

        // 应用按下缩放
        transform.DOScale(pressScale, effectDuration * 0.3f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 鼠标释放按钮时的回调
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable)
            return;

        // 恢复到悬停状态
        if (useColorEffect || effectType == ButtonEffectType.Color)
        {
            if (buttonImage != null)
            {
                buttonImage.DOColor(hoverColor, effectDuration * 0.3f);
            }
            if (buttonText != null)
            {
                buttonText.DOColor(hoverColor, effectDuration * 0.3f);
            }
        }

        transform.DOScale(hoverScale, effectDuration * 0.3f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 重置按钮到原始状态
    /// </summary>
    private void ResetToOriginalState()
    {
        currentTween?.Kill();

        transform.localScale = originalScale;

        if (buttonImage != null)
        {
            buttonImage.DOKill();
            buttonImage.color = originalImageColor;
        }

        if (buttonText != null)
        {
            buttonText.DOKill();
            buttonText.color = originalTextColor;
        }
    }

    /// <summary>
    /// 设置按钮交互状态
    /// </summary>
    /// <param name="value">是否可交互</param>
    public void SetInteractable(bool value)
    {
        isInteractable = value;
        button.interactable = value;

        // 如果设为不可交互，重置到原始状态
        if (!value)
        {
            ResetToOriginalState();
        }
    }

    void OnDestroy()
    {
        // 销毁时清理所有Tween动画
        currentTween?.Kill();
        transform.DOKill();

        if (buttonImage != null)
        {
            buttonImage.DOKill();
        }

        if (buttonText != null)
        {
            buttonText.DOKill();
        }
    }
}