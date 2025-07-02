using UnityEngine;

public class GuillotioneBlade : MonoBehaviour
{
    Player player; //플레이어 스크립트
    [SerializeField] float damage; //데미지

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.TakeDamage(damage); //플레이어 데미지 함수 실행
        }
    }
}
