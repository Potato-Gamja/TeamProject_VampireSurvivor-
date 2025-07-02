using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    WaitForSeconds hitCool = new(0.1f); //피격 효과 쿨타임
    WaitForSeconds knockbackDuration = new(0.1f); //넉백 지속시간

    [Header("기본 속성")]
    public float maxHealth = 10.0f;  //최대 체력
    public float currentHealth; //현재 체력
    public float moveSpeed = 2.0f; //이동속도
    public float baseSpeed = 1.3f;
    public float deffense = 0.0f; //방어력
    public float attackDamage = 10.0f; //피해량
    public float attackRange = 0.1f; //공격 범위 -> 플레이어와의 최소 거리
    public float attackCooldown = 1.0f; //공격 쿨타임
    public float knockbackResistance = 0.5f; //넉백 저항력

    [Header("드롭 아이템")]
    public string expGemRate; //경험치 잼 종류

    public float nextAttackTime; //다음 공격 시간
    public bool isKnockbacked; //넉백 상태 여부
    public bool isSlowed; //슬로우 여부

    protected Rigidbody2D rb; //리지드바디2d
    protected SpriteRenderer spriteRenderer; //스프라이트 렌더러
    protected Animator animator; //애니메이터
    protected GameObject player; //플레이어 게임오브젝트
    protected Player _player; //플레이어 스크립트
    protected HitEffect hitEffect; //피격 스크립트
    [SerializeField] protected Transform textPos; //텍스트 위치
    protected bool isDead = false; //사망 여부

    // 스프라이트 업데이트 관련 변수
    protected float lastUpdateTime = 0.0f; //마지막 업데이트 시간
    protected bool isVisible = true; //활성화 여부
    protected float marginArea = 0.1f; //여백 공간
    protected Color originalColor; //원래 색깔

    //초기화
    public virtual void Initialize()
    {
        if (rb == null) //초기화 함수 호출 시 계속해서 컴포넌트 할당하지 않게 널일 때만 할당하게
            rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (_player == null)
            _player = player.GetComponent<Player>();

        if(hitEffect == null)
            hitEffect = GetComponent<HitEffect>();

        if (rb != null)
        {
            rb.gravityScale = 0.0f; //중력 없애기
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; //충돌로 인한 회전 방지
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; //스프라이트 렌더러 기존 색상
        }


        float time = Time.time;
        maxHealth = (maxHealth + (time * 0.15f)) * (1 + (_player.Level * 0.01f)); //현재 체력을 시간과 플레이어 레벨에 맞게 증가하도록 변경
        currentHealth = maxHealth; //현재 체력을 최대 체력으로 설정
        nextAttackTime = 0.0f; //다음 공격 시간 초기화
        isKnockbacked = false; //넉백 상태 초기화
        isSlowed = false; //슬로우 상태 초기화
        isDead = false; //사망 여부 초기화
        moveSpeed = baseSpeed; //이동속도 초기화
    }

    protected virtual void Start()
    {
        Initialize(); //초기화 실행
    }

    protected virtual void Update()
    {
        if (isKnockbacked) //넉백 중에는 이동 못하게
        {
            return;
        }
        else //넉백 시 이동x
        {
            float dis = (transform.position - player.transform.position).sqrMagnitude; //플레이어와의 거리 계산

            if (dis > attackRange * attackRange) //추격 범위와의 비교를 통해 이동 여부
            {
                MoveTowardsPlayer(); //플레이어 방향으로 이동하는 함수
            }
            else
            {
                rb.linearVelocity = Vector2.zero; //이동 정지
            }
        }

        UpdateSprite(); //스프라이트 관련 업데이트 함수
    }

    //스프라이트 관련 함수
    protected virtual void UpdateSprite()
    {
        UpdateSpriteLayer(); // 스프라이트 레이어 업데이트
        if (Time.time - lastUpdateTime >= 0.1f) //0.1초 딜레이 주기
        {
            lastUpdateTime = Time.time; //마지막 업데이트 시간 갱신
            UpdateSpriteFlip(); //스프라이트 플립 함수 실행
            spriteRenderer.enabled = IsVisible(); //렌더링 기능 함수 실행 이후 해당 값 적용
        }
    }
    //스프라이트 렌더러 레이어 업데이트
    protected virtual void UpdateSpriteLayer()
    {
        if (spriteRenderer != null)
        {
            // Y 좌표가 낮을수록(화면 아래) 더 앞에 표시
            // Y 좌표 값에 따른 레이어 값 변경
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
        }
    }
    //스프라이트 플립 업데이트
    protected virtual void UpdateSpriteFlip()
    {
        if (spriteRenderer != null && player != null)
        {
            bool shouldFlip = player.transform.position.x < transform.position.x; //플레이어 방향 체크
            if (shouldFlip != spriteRenderer.flipX) //플립 여부 체크 -> 값이 달라지지 않았을 경우에는 넘어감
            {
                spriteRenderer.flipX = shouldFlip; //플립 값 변경
            }
        }
    }
     //스프라이트 렌더링 기능
    protected virtual bool IsVisible()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position); //월드 좌표를 뷰포트로 변환하여 범위 내 오브젝트 위치 확인
        bool newVisibility = screenPoint.x > -marginArea && screenPoint.x < 1 + marginArea &&
                             screenPoint.y > -marginArea && screenPoint.y < 1 + marginArea;
        //화면 내 여부에 대한 값

        if (isVisible != newVisibility) //화면 내 여부 변경 확인
        {
            isVisible = newVisibility; //상태 업데이트
            spriteRenderer.enabled = isVisible; //렌더러 상태 변경
        }

        return isVisible; //상태 반환
    }

    //플레이어 방향으로 이동
    protected virtual void MoveTowardsPlayer()
    {
        if (player != null && !isKnockbacked)
        {
            Vector2 dir = (player.transform.position - transform.position).normalized; //플레이어 방향을 구한 뒤 벡터 정규화
            rb.linearVelocity = dir * moveSpeed; //방향과 속도 값을 넣어 velocity 방식으로 이동
        }
    }

    //플레이어 공격
    public virtual void Attack()
    { 
        if (_player != null && gameObject.activeSelf)
        {
            _player.TakeDamage(attackDamage); //플레이어에게 몬스터의 피해량 값을 전달하여 피격 함수 실행
        }
    }

    //무기 피격                              //순서대로 무기 데미지, 넉백 크기, 슬로우 크기, 슬로우 지속시간
    public virtual void TakeDamage(float damage, float knockbackForce, float slowForce, float slowDuration)
    {
        knockbackForce = knockbackForce - (knockbackForce * knockbackResistance); //넉백 크기 계산
        if(knockbackForce > 0 && !isKnockbacked) //넉백의 크기가 있으며 넉백 중이 아닐 경우에만 넉백 실행
        {
            StartCoroutine(ApplyKnockback(knockbackForce)); //넉백 코루틴
        }

        if(spriteRenderer.isVisible) //게임오브젝트가 활성화 상태일 경우에만
            StartCoroutine(HitColor()); //피격 효과 코루틴 실행

        if (slowForce > 0) //슬로우의 크기가 있으면 실행
        {
            StartCoroutine(ApplySlow(slowForce, slowDuration)); //슬로우 코루틴
        }

        damage -= deffense; //방어력만큼 데미지 감소
        damage = Random.Range(damage - 1, damage + 1); //데미지 +- 1씩하여 같은 데미지가 나오는걸 방지
        float totalDamage = Mathf.Max(1, damage); //총합 데미지, 피해량 0을 방지하기 위한 Max() 함수

        DamageText(totalDamage); //데미지 텍스트 함수
        currentHealth -= totalDamage; //현재 체력 감소

        if(currentHealth <= 0.0f && !isDead) //현재 체력이 0이하일 경우 실행
        {
            isDead= true; //사망 여부 참
            Die(); //사망
        }
    }

    public virtual void DamageText(float damage)
    {
        Vector3 spawnPos = textPos.position; //텍스트를 활성화할 위치 가져오기

        GameObject textToSpawn = ObjectPool.Instance.SpawnFromPool_DamageText("DamageText", spawnPos, damage);

        if (textToSpawn != null)
        {
            textToSpawn.SetActive(true); //스폰할 텍스트 활성화하기
        }
    }

    //넉백 적용 코루틴
    private IEnumerator ApplyKnockback(float knockbackForce)
    {
        isKnockbacked = true; //넉백 상태 참 -> 이동 불가능
        Vector3 knockbackDir = transform.position - player.transform.position; //넉백 방향
        rb.AddForce(knockbackDir.normalized * knockbackForce, ForceMode2D.Impulse); //AddForce 함수로 순간적인 힘을 가한 넉백 효과
        yield return knockbackDuration; //넉백 지속시간 종료 후 호출
        isKnockbacked = false; //넉백 상태 거짓 -> 이동 가능
    }

    //슬로우 적용 코루틴
    private IEnumerator ApplySlow(float slowForce, float slowDuration)
    {
        moveSpeed *= (1 - slowForce); //이동속도 감소
        yield return slowDuration; //슬로우 지속시간 종료 후 호출

        moveSpeed = baseSpeed; //원래 이동속도로 초기화
    }

    //피격 효과
    protected virtual IEnumerator HitColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; //스프라이트 렌더러의 색을 빨간색으로
            yield return hitCool; //피격 지속시간 종료 후 호출
            spriteRenderer.color = originalColor; //스프라이트 렌더러의 색을 원래대로
        }
    }

    //죽음 
    protected virtual void Die()
    {
        if (hitEffect != null)
        {
            hitEffect.StopAttack(); //공격 정지 함수 실행 -> 몬스터가 사망 시에도 플레이어에게 피해를 입히는 현상 방지
        }

        SoundManager.Instance?.PlaySFX("enemyDeath"); //사망 사운드 재생
        CreateExpgem(); //경험치 잼 생성
        attackDamage = 0; //데미지 0 초기화
        spriteRenderer.color = originalColor; //스프라이트 색 원래대로
        StopAllCoroutines(); //모든 코루틴 종료
        gameObject.SetActive(false); //게임오브젝트 비활성화
    }

    //경험치 잼 생성
    protected virtual void CreateExpgem()
    {
        float ran = Random.value;

        expGemRate = SetExpGemRate(ran); //레벨 별 경험치 잼 확률

        GameObject expgemToSpawn = ObjectPool.Instance.SpawnFromPool_Expgem(expGemRate, transform.position);
        //오브젝트 풀링을 한 경험치 잼 가져오기

        if (expgemToSpawn != null)
        {
            expgemToSpawn.SetActive(true); //경험치 잼 활성화
        }
    }

    //레벨 별 경험치 잼 등급 확률 설정
    protected virtual string SetExpGemRate(float ran)
    {
        string rate = "Common";

        if (_player.Level <= 3)
        {
                rate = "Common";
        }
        else if (_player.Level <= 8)
        {
            if (ran < 0.80f)
            {
                rate = "Common";
            }
            else
            {
                rate = "Rare";
            }
        }
        else if (_player.Level <= 13)
        {
            if (ran < 0.75f)
            {
                rate = "Common";
            }
            else if (ran < 0.95f)
            {
                rate = "Rare";
            }
            else
            {
                rate = "Unique";
            }
        }
        else
        {
            if (ran < 0.65f)
            {
                rate = "Common";
            }
            else if (ran < 0.90f)
            {
                rate = "Rare";
            }
            else
            {
                rate = "Unique";
            }
        }

        return rate;
    }

    //오브젝트가 활성화 상태가 됐을 경우
    protected virtual void OnEnable()
    {
        Initialize(); //값 초기화
    }

    //공격 범위 표시
    //protected virtual void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, attackRange);
    //}
} 