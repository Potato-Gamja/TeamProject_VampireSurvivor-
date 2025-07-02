using UnityEngine;

public class ItemAttract : MonoBehaviour
{
    [Header("경험치 설정")]
    [SerializeField] protected float magnetDistance = 0.5f;    //자석 기능 거리
    [SerializeField] protected float baseSpeed = 5f;         //기본 속도
    [SerializeField] protected float maxSpeed = 15f;         //최대 속도
    [SerializeField] protected float accelerationRate = 2f;  //초당 속도 증가량
    [SerializeField] protected float force = 3f;

    protected Player player; //플레이어 스크립트
    protected Transform target; //플레이어 중심
    [SerializeField] protected SpriteRenderer spriteRenderer; //스프라이트 렌더러
    [SerializeField] protected Rigidbody2D rb; //리지드바디2D

    protected bool isAttracting; //기본 자석 기능 활성화 여부
    protected float currentSpeed; //현재 속도
    protected float attractTimer = 0f; //자석 타이머

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        target = player.gameObject.transform.GetChild(0).transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
    }

    protected virtual void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, target.position); //아이템과 타겟의 거리

        // 플레이어와의 거리가 자석 거리보다 가까우면
        if (distanceToPlayer <= magnetDistance)
        {
            isAttracting = true; //기본 자석 기능 활성화
        }

        //기본 자석 기능 활성화 시
        if (isAttracting)
        {
            attractTimer += Time.deltaTime; //자석 타이머 시간 증가
            currentSpeed = Mathf.Min(baseSpeed + (accelerationRate * attractTimer), maxSpeed); //시간에 따라 현재 속도 증가

            transform.position = Vector2.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime); //플레이어 방향으로 이동
            
        }
    }

    //경험치 잼 튕기는 효과
    protected virtual void Launch()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized; //원형 모양으로 랜덤한 방향 정규화
        force = Random.Range(0.5f, 1.5f); //랜덤의 힘 크기
        rb.AddForce(randomDir * force, ForceMode2D.Impulse); //해당 방향으로 힘을 가함
        Invoke(nameof(AddForceZero), 0.1f); //AddForce효과를 0.1초만 유지
    }

    protected virtual void AddForceZero()
    {
        rb.linearVelocity = Vector2.zero; //정지 상태
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100); //렌더러 레이어 설정
    }

    //경험치 잼 활성화 시
    protected virtual void OnEnable()
    {
        Launch(); //경험치 잼 튕기기 함수 실행
    }

    //비활성화 시
    protected virtual void OnDisable()
    {
        isAttracting = false; //기본 자석 기능 여부 비활성화
        currentSpeed = baseSpeed; //속도 초기화
        attractTimer = 0; //타이머 초기화
        rb.linearVelocity = Vector2.zero; //정지 상태
    }

}