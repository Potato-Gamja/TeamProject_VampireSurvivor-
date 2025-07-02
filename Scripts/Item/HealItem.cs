using UnityEngine;

public class HealItem : ItemAttract
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            player.Heal(10.0f); //플레이어 스크립트의 회복 함수 호출
            SoundManager.Instance.PlaySFX("heal");
            gameObject.SetActive(false); //오브젝트 비활성화
        }
    }
}
