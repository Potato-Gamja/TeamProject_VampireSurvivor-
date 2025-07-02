using UnityEngine;

public class KamikazeEnemy : Enemy
{
    [Header("자폭 설정")]
    public bool isReady = false; //폭발 준비 여부
    public float explosionReadyRange = 6.0f; //폭발 준비 시작 범위
    public float explosionMoveSpeed = 3.0f; //폭발 준비 시 이동속도
    public float explosionRange = 1.0f; //폭발  범위
    public float explosionDamage = 50.0f; //폭발 데미지
    public GameObject explosionEffect; //폭발 이펙트
    public GameObject center; //중앙에 위치한 게임오브젝트

    protected override void Update()
    {
        base.Update(); //상속한 Enemy의 Update() 함수 호출

        float dis = Vector2.Distance(transform.position, player.transform.position); //거리 계산
        
        // 폭발 범위 안에 들어오면 자폭 준비
        if (dis <= explosionReadyRange && !isReady)
        {
            isReady = true; //폭발 준비 참
            ExplodeReady(); //폭발 준비
        }
    }

    //폭발 애니메이션 재생 -> 인보크로 일정 시간 뒤 폭발 -> 폭발 시 데미지
    private void ExplodeReady()
    {
        //폭발 애니메이션 실행
        animator.SetTrigger("isReady"); //애니메이터 트리거 설정
        moveSpeed = explosionMoveSpeed; //이동속도 변경

        Invoke(nameof(Explode), 2.5f); //2.5초 뒤 폭발
    }

    //폭발
    private void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity); //폭발 파티클 생성
            SoundManager.Instance?.PlaySFX("explosion");
        }

        // 폭발 범위 내의 콜라이더 검출
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRange);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player")) //콜라이더가 플레이어 시
            {
                if (_player != null)
                {
                    _player.TakeDamage(explosionDamage); //폭발 데미지
                }
            }
        }
        gameObject.SetActive(false); //오브젝트 비활성화
    }

    protected override void Die()
    {
        CancelInvoke();
        base.Die();
    }

    //활성화 시
    private new void OnEnable()
    {
        base.OnEnable(); //상속한 Enemy의 OnEnable 함수 호출
        isReady = false; //폭발 준비 여부 초기화
        animator.ResetTrigger("isReady"); //애니메이터 트리거 초기화
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(center.transform.position, explosionRange);
    //}
} 