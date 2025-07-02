using UnityEngine;

public class MagnetItem : ItemAttract
{
    //충돌 시
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //플레이어 충돌 시
        {
            AttractAllExpGems(); //경험치 잼 자석 기능 함수 호출
            SoundManager.Instance.PlaySFX("magnet");
            gameObject.SetActive(false); //자석 아이템 비활성화
        }
    }

    //경험치 잼 자석 아이템
    private void AttractAllExpGems()
    {
        GameObject[] gems = GameObject.FindGameObjectsWithTag("ExpGem"); //모든 경험치 잼 배열에 넣기

        //반복문
        foreach (GameObject gem in gems)
        {
            ExpGem expGem = gem.GetComponent<ExpGem>(); //ExpGem 스크립트 참조
            if (expGem != null)
            {
                expGem.StartAttraction(); //자석 기능 활성화
            }
        }
    }
}
