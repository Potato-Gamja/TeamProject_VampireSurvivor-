using System.Collections;
using UnityEngine;

public class MovedGuillotione : MonoBehaviour
{
    [SerializeField] private float speed = 15.0f; //길로틴의 날라가는 속도
    private float dir; //이동방향 - 좌우
    private float attackTime = 0.0f; //공격 진행시간
    private bool isMoving = false; //길로틴의 이동 여부
    private bool isAttack = false; //길로틴의 공격 여부

    [SerializeField] private Vector3 guillotioneVec; //길로틴의 위치
    [SerializeField] GameObject warn; //경고 오브젝트
    [SerializeField] GameObject guillotione; //길로틴 오브젝트
    [SerializeField] private Animator warnAnimator; //경고 애니메이터
    [SerializeField] private Animator guillotioneAnimator; //길로틴 애니메이터
    [SerializeField] private SpriteRenderer guillotioneSpriteRenderer; //길로틴 스프라이트 렌더러
    [SerializeField] private SpriteRenderer bladeSpriteRenderer; //길로틴 날 스프라이트 렌더러
    private Player player; //플레이어
    private BoxCollider2D boxCol; //박스 콜라이더


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        //움직임이 참 시
        if (isMoving)
            MoveGuillotine();

        Attack_Guillotione();
    }

    //길로틴 공격 활성화
    private void Attack_Guillotione()
    {
        //공격여부 참, 움직임 거짓 -> 한번만 실행되게 하기 위함
        if (isAttack && !isMoving)
        {
            boxCol.enabled = true; //박스 콜라이더 활성화
            isMoving = true; //움직임 참
            attackTime = 0f; //공격 진행시간 초기화
        }

        // 길로틴 날의 알파값이 0이 됐을 시 -> 애니메이션을 통해 알파값이 0이 됨.
        if (bladeSpriteRenderer.color.a == 0.0f)
        {
            gameObject.SetActive(false); //길로틴 그룹 비활성화
            guillotione.SetActive(false); //길로틴 비활성화
            guillotioneAnimator.SetBool("isReady", false); //길로틴 애니메이터 값 변경

            isMoving = false; //움직임 거짓
        }
    }

    //길로틴 이동
    private void MoveGuillotine()
    {
        attackTime += Time.deltaTime;

        //속도 계산: 0~1초까지 점점 빨라지고, 이후 확 느려짐
        float t = attackTime / 2.0f;
        float speedMultiplier = -Mathf.Pow(t - 1, 2) + 1; //포물선 형태로 변화 (최대값 1)
        float currentSpeed = speed * speedMultiplier;

        Vector3 moveDirection = (dir > 0.5f) ? Vector3.left : Vector3.right; //이동 방향 설정
        transform.position += moveDirection * currentSpeed * Time.deltaTime; //이동


        //이동 종료 조건
        if (attackTime > 1.0f)
        {
            boxCol.enabled = false; //박스 콜라이더 비활성화
            isMoving = false; //움직임 여부 거짓
            isAttack = false; //공격 여부 거짓
            guillotioneAnimator.SetBool("isAttack", false); //길로틴 애니메이션 값 변경
        }
    }

    //길로틴 설정
    private void SetGuillotione()
    {
        transform.position = player.transform.position; //플레이어 위치로
        guillotione.transform.localPosition = guillotioneVec; //길로틴의 위치를 로컬 포지션으로 초기화
        guillotioneSpriteRenderer.color = new Color(guillotioneSpriteRenderer.color.r, guillotioneSpriteRenderer.color.g, guillotioneSpriteRenderer.color.b, 1f); //렌더러 값 초기화
        bladeSpriteRenderer.color = new Color(bladeSpriteRenderer.color.r, bladeSpriteRenderer.color.g, bladeSpriteRenderer.color.b, 1f); //렌더러 값 초기화


        //길로틴과 길로틴 날의 렌더러 레이어 설정
        guillotioneSpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100); 
        bladeSpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        warn.SetActive(true); //경고 활성화

        StartCoroutine(nameof(Attack)); //공격 코루틴 실행
    }

    //공격 코루틴
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(1.0f); //1초 딜레이
        warn.gameObject.SetActive(false); //경고 비활성화
        isAttack = true; //공격 여부 참
        guillotione.gameObject.SetActive(true); //길로틴 오브젝트 활성화
        guillotioneAnimator.SetBool("isReady", true); //길로틴 애니메이터 값 변경
    }

    //오브젝트 활성화 시
    private void OnEnable()
    {
        dir = Random.value; //랜덤 방향
        transform.position = player.transform.position; //플레이어의 위치로 이동

        //스케일 값을 변경하여 공격방향을 설정
        if (dir > 0.5f)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1); //좌측 방향으로 공격 
        }
        else
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1); //우측 방향으로 공격
        }

        SetGuillotione(); //길로틴 설정 함수 실행
    }
}
