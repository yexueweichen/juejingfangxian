using UnityEngine;
using UnityEngine.UI;

public class FloatingHPBar : MonoBehaviour
{
    [Header("血条填充Image")]
    public Image hpFill;

    [Header("跟随目标（怪物Transform）")]
    public Transform followTarget;

    [Header("屏幕偏移量（相对于怪物位置）")]
    public Vector2 screenOffset = new Vector2(0, 30);

    [Header("最大显示距离（超过此距离隐藏血条）")]
    public float maxShowDistance = 15f;

    [Header("是否在摄像机背面时隐藏")]
    public bool hideWhenBack = true;

    [Header("血量平滑变化速度")]
    public float smoothSpeed = 8f;

    private float _maxHp;
    private float _currentHp;
    private float _targetFillAmount;
    private Camera _mainCam;
    private CanvasGroup _canvasGroup;
    private float _showDelayTime = 0.5f;
    private float _spawnTime;
    private bool _isShowing = false;
    private float _hideDistance = 15f;
    private float _showDistance = 13f;

    /// <summary>
    /// 初始化血条
    /// </summary>
    /// <param name="target">跟随的目标（怪物）</param>
    /// <param name="maxHp">最大生命值</param>
    public void Init(Transform target, float maxHp)
    {
        followTarget = target;
        _maxHp = maxHp;
        _currentHp = maxHp;
        _targetFillAmount = 1f;

        // 获取主摄像机
        _mainCam = Camera.main;
        if (_mainCam == null)
            _mainCam = GameObject.FindObjectOfType<Camera>();

        // 获取或添加CanvasGroup组件（用于控制透明度）
        if (_canvasGroup == null)
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        _spawnTime = Time.time;
        _canvasGroup.alpha = 0;

        // 初始化血条填充为满
        if (hpFill != null)
            hpFill.fillAmount = _targetFillAmount;
    }

    /// <summary>
    /// 受到伤害时调用
    /// </summary>
    public void TakeDamage(float damage)
    {
        _currentHp = Mathf.Clamp(_currentHp - damage, 0, _maxHp);
        _targetFillAmount = (_maxHp > 0f) ? _currentHp / _maxHp : 0f;
    }

    /// <summary>
    /// 设置当前血量和最大血量
    /// </summary>
    public void SetHP(float current, float max)
    {
        _maxHp = max;
        _currentHp = current;
        _targetFillAmount = (_maxHp > 0f) ? _currentHp / _maxHp : 0f;
    }

    void Update()
    {
        if (followTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        // 如果摄像机丢失，尝试再次获取
        if (_mainCam == null)
            _mainCam = Camera.main ?? GameObject.FindObjectOfType<Camera>();

        UpdateBarPosition();
        SetHpBarActive();
        SmoothUpdateHpFill();
    }

    /// <summary>
    /// 更新血条屏幕位置
    /// </summary>
    void UpdateBarPosition()
    {
        if (_mainCam == null) return;
        Vector3 screenPos = _mainCam.WorldToScreenPoint(followTarget.position);
        screenPos += (Vector3)screenOffset;
        transform.position = screenPos;
    }

    /// <summary>
    /// 控制血条显示/隐藏（使用CanvasGroup.alpha）
    /// </summary>
    void SetHpBarActive()
    {
        if (_mainCam == null || _canvasGroup == null) return;

        // 生成后延迟显示，防止刚生成时闪烁
        if (Time.time - _spawnTime < _showDelayTime)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            return;
        }

        // 获取目标在摄像机视口中的位置
        Vector3 viewportPos = _mainCam.WorldToViewportPoint(followTarget.position);

        // 在摄像机后面 -> 隐藏
        if (hideWhenBack && viewportPos.z < 0)
        {
            _isShowing = false;
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            return;
        }

        // 不在视野范围内 -> 隐藏
        bool isInView = viewportPos.x >= -0.1f && viewportPos.x <= 1.1f &&
                        viewportPos.y >= -0.1f && viewportPos.y <= 1.1f;

        if (!isInView)
        {
            _isShowing = false;
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            return;
        }

        // 使用滞回区间防止距离临界值闪烁
        float distance = Vector3.Distance(_mainCam.transform.position, followTarget.position);
        bool shouldShow = _isShowing ? distance <= _hideDistance : distance <= _showDistance;

        if (shouldShow)
        {
            _isShowing = true;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }
        else
        {
            _isShowing = false;
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    /// <summary>
    /// 平滑更新血量填充值
    /// </summary>
    void SmoothUpdateHpFill()
    {
        if (hpFill == null) return;
        hpFill.fillAmount = Mathf.Lerp(hpFill.fillAmount, _targetFillAmount, Time.deltaTime * smoothSpeed);
    }
}
