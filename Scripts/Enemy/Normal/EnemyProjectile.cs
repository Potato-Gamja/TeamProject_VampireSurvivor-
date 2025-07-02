using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("적 원거리 공격 설정")]
    public float speed; //이동 크기
    public float damage; //데미지
    public float lifeTime = 5f; //지속시간
    [SerializeField] private Player player; //플레이어 스크립트
    Transform target;
    private Rigidbody2D rb; //리지드바디
    string projectileType; //투사체의 타입

    Vector2 dir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.linearVelocity = dir * speed; //방향과 속도 값을 넣어 velocity 방식으로 이동
    }

    //활성화 시
    private void OnEnable()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
            target = player.transform.GetChild(0).GetComponent<Transform>();
        }

        if(player != null)
        {
            dir = (target.position - transform.position).normalized; //발사 방향
        }

        Invoke(nameof(DisableObject), lifeTime); //5초 뒤 투사체 비활성화 함수 실행
    }

    //비활성화 함수
    private void DisableObject()
    {
        gameObject.SetActive(false);
    }

    //비활성화 시
    private void OnDisable()
    {
        rb.linearVelocity = Vector2.zero; //이동 크기 0
        CancelInvoke(); //인보크 취소
    }

    //충돌 시
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("PlayerProjectileCol")) //플레이어 충돌 시
        {
            player.TakeDamage(damage); //플레이어의 몬스터의 피해량 값을 전달하여 피격 함수 실행
            gameObject.SetActive(false);
        }
    }
}
