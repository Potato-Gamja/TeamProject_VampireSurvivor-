using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적이 무기나 이펙트에 맞았을 때의 효과를 처리하는 클래스
/// </summary>
public class HitEffect : MonoBehaviour
{
    private Enemy enemy;  // 적 컴포넌트 참조
    [SerializeField] private List<WeaponData> allWeaponData;  // 모든 무기 데이터 리스트

    // 각 무기별 쿨다운 시간 상수
    private const float ATTACK_COOLDOWN = 1.5f;  // 기본 공격 쿨다운
    private const float PIPE_EFFECT_COOLDOWN = 1f;  // Pipe 이펙트의 데미지 주기
    private const float FIRECRACKER_EFFECT_COOLDOWN = 0.6f;  // 축하 폭죽 이펙트의 데미지 주기

    private float attackTimer;  // 공격 타이머
    private bool canAttack;  // 공격 가능 여부
    private Collider2D playerCol;  // 플레이어 콜라이더 참조

    private Dictionary<string, WeaponTimer> weaponTimers;  // 각 무기별 타이머 딕셔너리

    private void Start()
    {
        InitializeComponents();
        LoadAllWeaponData();
        InitializeWeaponTimers();
    }

    /// <summary>
    /// 필요한 컴포넌트들을 초기화
    /// </summary>
    private void InitializeComponents()
    {
        enemy = GetComponent<Enemy>();
    }

    /// <summary>
    /// WeaponDataManager로부터 모든 무기 데이터를 로드
    /// </summary>
    private void LoadAllWeaponData()
    {
        allWeaponData = WeaponDataManager.Instance.GetAllWeaponData();
    }

    /// <summary>
    /// 각 무기별로 타이머를 초기화
    /// </summary>
    private void InitializeWeaponTimers()
    {
        weaponTimers = new Dictionary<string, WeaponTimer>();
        
        if (allWeaponData == null || allWeaponData.Count == 0)
        {
            return;
        }

        // 모든 무기 데이터에 대해 타이머 생성
        foreach (var weaponData in allWeaponData)
        {
            if (weaponData == null) continue;

            string weaponTypeStr = weaponData.weaponType.ToString();
            
            if (!weaponTimers.ContainsKey(weaponTypeStr))
            {
                // 무기 타입에 따른 쿨다운 시간 설정
                float cooldown = weaponData.weaponType switch
                {
                    WeaponType.Pipe => PIPE_EFFECT_COOLDOWN,
                    WeaponType.Firecracker => FIRECRACKER_EFFECT_COOLDOWN,
                    _ => ATTACK_COOLDOWN
                };
                weaponTimers.Add(weaponTypeStr, new WeaponTimer(cooldown));
            }
        }
    }

    private void Update()
    {
        UpdateTimers();
        HandleAttack();
    }

    /// <summary>
    /// 모든 타이머 업데이트
    /// </summary>
    private void UpdateTimers()
    {
        attackTimer += Time.deltaTime;
        
        foreach (var timer in weaponTimers.Values)
        {
            timer.Update(Time.deltaTime);
        }
    }

    /// <summary>
    /// 플레이어 공격 처리
    /// </summary>
    private void HandleAttack()
    {
        if (attackTimer >= ATTACK_COOLDOWN)
        {
            canAttack = true;
        }

        if (canAttack && playerCol != null)
        {
            attackTimer = 0.0f;
            canAttack = false;
            enemy.Attack();
        }
    }

    /// <summary>
    /// 콜라이더 진입 시 처리
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCol = other;
            return;
        }

        Effect effect = other.GetComponent<Effect>();
        if (effect != null && effect.WeaponData != null)
        {
            if (effect.WeaponData.weaponType is WeaponType.Pipe or WeaponType.Firecracker)
            {
                return;
            }
        }

        ProcessWeaponHit(other);
    }

    /// <summary>
    /// 무기나 이펙트에 의한 데미지 처리
    /// </summary>
    private void ProcessWeaponHit(Collider2D weaponCollider)
    {
        // 투사체에서 무기 데이터를 가져옴
        Projectile projectile = weaponCollider.GetComponent<Projectile>();
        if (projectile != null)
        {
            if (projectile.weaponType == WeaponType.Book)
            {
                if (projectile.currentPierceCount <= projectile.maxPierceCount)
                {
                    projectile.currentPierceCount++; //책 의 관통수치 증가
                    ProcessDamage(projectile.WeaponData);
                }
                else
                    return;
            }
            else
                ProcessDamage(projectile.WeaponData);
            return;
        }

        // 이펙트에서 무기 데이터를 가져옴
        Effect effect = weaponCollider.GetComponent<Effect>();
        if (effect != null)
        {
            ProcessDamage(effect.WeaponData);
        }
    }

    /// <summary>
    /// 실제 데미지 처리 및 효과 적용
    /// </summary>
    private void ProcessDamage(WeaponData weaponData)
    {
        if (weaponData == null) return;

        int index = weaponData.currentLevel;
        var stats = weaponData.levelStats;


        // 데미지, 넉백, 슬로우 효과 적용
        enemy.TakeDamage(
            stats.damage[index-1],
            stats.knockbackForce[index-1],
            stats.slowForce[index-1],
            stats.slowDuration[index-1]
        );

        if(enemy.currentHealth > 0)
            SoundManager.Instance?.PlayHitSound(weaponData.weaponType);

    }

    /// <summary>
    /// 콜라이더 내부 체류 시 처리 (지속 데미지용)
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCol = other;
            return;
        }

        Effect effect = other.GetComponent<Effect>();
        if (effect != null && effect.WeaponData != null && 
            weaponTimers.TryGetValue(effect.WeaponData.weaponType.ToString(), out WeaponTimer timer))
        {
            // 이펙트 타입에 따른 특별 처리
            if (effect.WeaponData.weaponType is WeaponType.Pipe or WeaponType.Firecracker)
            {
                // Pipe와 Firecracker 이펙트는 지속적으로 데미지를 줌
                if (timer.CanHit())
                {
                    timer.Reset();
                    ProcessWeaponHit(other);
                }
            }
            else
            {
                ///필요에 따라 삭제 고려
                // 다른 이펙트는 일반적인 처리
                if (timer.CanHit())
                {
                    timer.Reset();
                    ProcessWeaponHit(other);
                }
            }
        }
    }

    /// <summary>
    /// 콜라이더 이탈 시 처리
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCol = null;
        }
    }

    private void OnDisable()
    {
        playerCol= null;
    }

    public void StopAttack()
    {
        playerCol = null;
        canAttack = false;
        attackTimer = 0f;
    }
}

/// <summary>
/// 무기별 타이머를 관리하는 보조 클래스
/// </summary>
public class WeaponTimer
{
    private readonly float cooldown;  // 쿨다운 시간
    private float timer;  // 현재 타이머 값

    public WeaponTimer(float cooldown)
    {
        this.cooldown = cooldown;
        this.timer = 0f;
    }

    /// <summary>
    /// 타이머 업데이트
    /// </summary>
    public void Update(float deltaTime)
    {
        timer += deltaTime;
    }

    /// <summary>
    /// 공격 가능 여부 확인
    /// </summary>
    public bool CanHit() => timer >= cooldown;

    /// <summary>
    /// 타이머 리셋
    /// </summary>
    public void Reset()
    {
        timer = 0f;
    }
}