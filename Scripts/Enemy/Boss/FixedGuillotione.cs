using UnityEngine;

public class FixedGuillotione : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f; //길로틴이 떨어지는 속도
    private float dropDelay = 2f; //떨어지는 데까지 걸리는 딜레이 시간
    private bool isPlay = false; //플레이 여부
    private bool isDrop = false; //떨어짐 여부

    private Vector3 targetVec; //목표 벡터
    [SerializeField] GameObject blade; //칼날 오브젝트
    [SerializeField] private Animator animator; //애니메이터
    [SerializeField] private SpriteRenderer spriteRenderer; //스프라이트 렌더러
    [SerializeField] private SpriteRenderer bladeSpriteRenderer; //길로틴 날 스프라이트 렌더러
    [SerializeField] private ParticleSystem dustParticle; //먼지 파티클
    private Player player; //플레이어 스크립트 
    private BoxCollider2D boxCol; //박스 콜라이더

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        boxCol = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Attack_Guillotione(); //길로틴 공격 함수 실행
    }

    private void Attack_Guillotione()
    {
        //떨어짐 여부 거짓 시 리턴
        if (!isDrop)
            return;

        //길로틴 날의 위치를 목표 벡터 방향으로 이동
        blade.transform.localPosition = Vector3.MoveTowards(blade.transform.localPosition, targetVec, speed * Time.deltaTime);

        //목표 지점과의 거리가 해당 거리 미만 시 바닥에 떨어진 것으로 판단
        if (Vector3.Distance(blade.transform.localPosition, targetVec) < 0.05f)
        {
            OnHit();
        }

        //길로틴 날의 알파값이 0이 됐을 경우 실행 -> 애니메이션을 통해 알파값 변경.
        if (bladeSpriteRenderer.color.a == 0.0f)
        {
            gameObject.SetActive(false); //오브젝트 비활성화
        }
    }

    //길로틴 공격 준비
    private void Ready()
    {
        //목표 지점 설정 -> 길로틴 날의 로컬포지션에서 y값만 변경
        targetVec = new Vector3(blade.transform.localPosition.x, blade.transform.localPosition.y - 1.05f, blade.transform.localPosition.z);

        if (animator != null)
        {
            animator.SetBool("isReady", true); //공격 애니메이션 시작
        }

        Invoke(nameof(Drop), dropDelay); //길로틴 날이 떨어지는 드랍 함수 실행
    }

    //길로틴 날 떨구기
    private void Drop()
    {
        isDrop = true;
        animator.SetBool("isDrop", true);
    }

    private void SoundPlay()
    {
        SoundManager.Instance.PlaySFX("fixedGuillotione");
    }

    //바닥에 떨어짐
    private void OnHit()
    {
        dustParticle.Play();
        //충격 먼지 파티클 실행
    }

    public void Set()
    {
        animator.SetBool("isDrop", false); //애니메이터 값 변경
        animator.SetBool("isReady", false); //애니메이터 값 변경
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f); //렌더러 값 초기화
        bladeSpriteRenderer.color = new Color(bladeSpriteRenderer.color.r, bladeSpriteRenderer.color.g, bladeSpriteRenderer.color.b, 1f); //렌더러 값 초기화
        blade.transform.localPosition = new Vector3(0, 1.3f, 0);
        isDrop = false;
        isPlay = false;
    }

    //활성화 시
    private void OnEnable()
    {
        Set();
        //길로틴과 길로틴 날의 레이어 설정
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
        bladeSpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }

    //비활성화 시
    private void OnDisable()
    {
        Set();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //플레이어와 접촉, 길로틴의 공격 실행 여부 확인
        if (other.CompareTag("Player") && !isPlay)
        {
            isPlay = true; //공격 실행 참
            Ready(); //길로틴 공격 준비
        }
    }
}