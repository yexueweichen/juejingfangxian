using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent zombieAgent;
    private Animator animator;
    private int hp;
    private int atk;
    public MonsterInfos.MonsterInfo thisMonsterInfo;
    public bool isDead = false;
    private float frontTime;
    public float hurtOffestTime = 0.5f;
    public float overTime = 0.5f;

    public GameObject player;

    [Header("怪物价值的能量")]
    public int worthEnergy = 50;

    [Header("追击范围（超过此距离不再追击玩家）")]
    public float chaseRange = 10f;

    [Header("攻击范围")]
    public float attackRange = 6f;

    [Header("血条")]
    public FloatingHPBar hpBarPrefab;
    private FloatingHPBar _hpBar;

    private Transform attackTarget;
    private bool canMove = false;
    private bool animLoaded = false;
    private float lastHurtSoundTime = 0;

    void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    //初始化数据
    public void MonsterInfoInit(MonsterInfos.MonsterInfo info)
    {
        thisMonsterInfo = info;
        isDead = false;
        hp = info.hp;
        atk = info.atk;
        worthEnergy = info.Worth;
        chaseRange=info.chaseRange;
        attackRange = info.attackRange;
        hurtOffestTime=info.hurtOffestTime;
        overTime = hurtOffestTime;

        if (zombieAgent != null)
        {
            zombieAgent.enabled = true;
            zombieAgent.speed = info.moveSpeed;
            zombieAgent.acceleration = info.moveSpeed;
            zombieAgent.angularSpeed = info.roundSpeed;
            zombieAgent.isStopped = false;
            zombieAgent.velocity = Vector3.zero;
        }

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animLoaded = true;
            if (canMove)
                StartMove();
        }
        else
        {
            ABMgr.Instance.LoadResAsync<RuntimeAnimatorController>("animation", info.animator, (res) =>
            {
                if (this == null || isDead) return;
                if (res != null)
                {
                    animator.runtimeAnimatorController = res;
                }
                animLoaded = true;
                if (canMove)
                    StartMove();
            }, true).Forget();
        }
    }

    public void Wound(int damage)
    {
        if (isDead) return;

        hp -= damage;

        if (_hpBar != null)
            _hpBar.TakeDamage(damage);

        if (hp <= 0)
        {
            Dead();
        }
        else
        {
            if (animator.runtimeAnimatorController != null)
            {
               if(overTime >= hurtOffestTime)
                animator.SetTrigger("Dmg");
                overTime = 0;

            }
            if (Time.time - lastHurtSoundTime >= 2f)
            {
                lastHurtSoundTime = Time.time;
                MusicMgr.Instance.PlaySound(thisMonsterInfo.damageSound);
            }
            ABMgr.Instance.LoadResAsync<GameObject>("eff", "mhit", (res) =>
            {
                if (this == null || isDead) return;
                if (res != null)
                {
                    GameObject effObj = GameObject.Instantiate(res, transform.position + Vector3.up * 0.7f, transform.rotation);
                    GameObject.Destroy(effObj, 1f);
                }
            }).Forget();
        }
    }

    public void Dead()
    {
        if (isDead) return;
        isDead = true;

        if (zombieAgent != null && zombieAgent.isOnNavMesh)
        {
            zombieAgent.isStopped = true;
            zombieAgent.velocity = Vector3.zero;
        }

        if (animator.runtimeAnimatorController != null)
        {
            animator.SetBool("Run", false);
            animator.SetBool("Death", true);
            MusicMgr.Instance.PlaySound(thisMonsterInfo.deadSound);
           Death();
         }
    }

    public void Death()
    {
        GameLevelMgr.Instance.ChangeMonsterNum(-1);
        GameLevelMgr.Instance.RemoveMonster(this);
        EventCenter.Instance.EventTrigger<int>(E_EventType.E_AddEnergy, worthEnergy);

        if (_hpBar != null)
        {
            Destroy(_hpBar.gameObject);
            _hpBar = null;
        }

        Destroy(this.gameObject, 2f);
 
        if (GameLevelMgr.Instance.CheckOver())
        {
            UIMgr.Instance.ShowPanel<GameOverPanel>((panel) =>
            {
                panel.InitInfo(SaveMgr.Instance.playerData.award, true);
            });
        }
    }

    public void BornOver()
    {
        if (isDead) return;
        canMove = true;

        player = GameObject.FindGameObjectWithTag("Player");
        float playerDist = player != null ? MathUtil.GetObjDistanceXZ(transform.position, player.transform.position) : float.MaxValue;
        float towerDist = MainTower.Instance != null ? MathUtil.GetObjDistanceXZ(transform.position, MainTower.Instance.transform.position) : float.MaxValue;

        if (player != null && playerDist <= chaseRange && playerDist <= towerDist)
            attackTarget = player.transform;
        else if (MainTower.Instance != null)
            attackTarget = MainTower.Instance.transform;
        else
            attackTarget = null;

        if (hpBarPrefab != null && _hpBar == null)
            CreateHPBar();

        if (animLoaded)
            StartMove();
    }

    private void StartMove()
    {
        if (attackTarget != null && zombieAgent != null && zombieAgent.isOnNavMesh)
            zombieAgent.SetDestination(attackTarget.position);

        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetBool("Run", true);
    }

    //生成血条
    private void CreateHPBar()
    {
        Transform canvasTrans = UIMgr.Instance?.GetCanvasTransform();
        if (canvasTrans == null) return;

        _hpBar = Instantiate(hpBarPrefab, canvasTrans);

        float maxHp = thisMonsterInfo != null ? thisMonsterInfo.hp : hp;
        _hpBar.Init(transform, maxHp);
        _hpBar.SetHP(hp, maxHp);
    }

    void Update()
    {
        if (isDead || zombieAgent == null || !canMove || !animLoaded) return;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        float playerDist = player != null ? MathUtil.GetObjDistanceXZ(transform.position, player.transform.position) : float.MaxValue;
        float towerDist = MainTower.Instance != null ? MathUtil.GetObjDistanceXZ(transform.position, MainTower.Instance.transform.position) : float.MaxValue;

        // 选择目标：只有在玩家在 chaseRange 内且比塔更近时才追玩家，否则回到塔
        if (player != null && playerDist <= chaseRange && playerDist <= towerDist)
            attackTarget = player.transform;
        else if (MainTower.Instance != null)
            attackTarget = MainTower.Instance.transform;
        else
            attackTarget = null;

        if (attackTarget == null) return;

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // 更稳健地判断是否在跑
            animator.SetBool("Run", zombieAgent.velocity.sqrMagnitude > 0.01f);

            float distToTarget = MathUtil.GetObjDistanceXZ(transform.position, attackTarget.position);

            if (distToTarget <= attackRange)
            {
                // 到达攻击范围：停止导航并触发攻击
                if (zombieAgent.isOnNavMesh)
                {
                    zombieAgent.isStopped = true;
                    zombieAgent.velocity = Vector3.zero;
                }

                if (Time.time - frontTime >= thisMonsterInfo.atkTime)
                {
                    frontTime = Time.time;
                    animator.SetTrigger("Atk");
                }
            }
            else
            {
                // 在攻击范围外，继续移动到目标
                if (zombieAgent.isOnNavMesh)
                {
                    zombieAgent.isStopped = false;
                    zombieAgent.SetDestination(attackTarget.position);
                }
            }
        }
    }

    // 攻击事件
    public void AtkEvent()
    {
        if (isDead || attackTarget == null) return;

        if (!MathUtil.CheckObjDistanceXZ(transform.position, attackTarget.position, attackRange)) return;

        if (attackTarget.TryGetComponent<MainTower>(out var tower))
            tower.Wound(atk);
        else if (attackTarget.TryGetComponent<Player>(out var player))
            player.Wound(atk);
        MusicMgr.Instance.PlaySound(thisMonsterInfo.atkSound);
    }
}


