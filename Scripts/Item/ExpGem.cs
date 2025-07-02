using System.Collections;
using UnityEngine;

public class ExpGem : ItemAttract
{
    [Header("경험치 설정")]
    [SerializeField] private float expAmount = 10f; //보석이 제공하는 경험치량
    bool onMagnet = false; //자석 아이템 기능 활성화 여부

    protected override void Update()
    {
        if (player == null)
            return;

        base.Update();

        //자석 아이템 기능
        //자석 아이템 획득 시 플레이어 측으로 이동
        if (onMagnet && player != null)
        {
            attractTimer += Time.deltaTime; //자석 타이머 시간 증가
            currentSpeed = Mathf.Min(baseSpeed + (accelerationRate * attractTimer), maxSpeed); //시간에 따라 현재 속도 증가
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, currentSpeed * Time.deltaTime);
            // 플레이어 방향으로 이동
        }
    }

    //자석 아이템 획득
    public void StartAttraction()
    {
        onMagnet = true; //자석 아이템 활성화
    }

    //충돌 시
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //플레이어 충돌 시
        {
            player.AddExperience(expAmount); //플레이어 경험치 증가 함수 실행
            SoundManager.Instance.PlaySFX("expGem");
            gameObject.SetActive(false); //경험치 잼 비활성화
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onMagnet = false; //자석 아이템 기능 활성화 여부 거짓
    }
} 