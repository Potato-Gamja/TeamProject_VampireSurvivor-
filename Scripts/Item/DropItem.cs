using UnityEngine;

[System.Serializable]
public struct DropData //드랍아이템 데이터
{
    public GameObject item; //아이템
    public float dropChance; //드랍 확률
}

public class DropItem : MonoBehaviour
{
    public enum Rate //몬스터의 등급
    {
        Normal, Named
    }
    public Rate rate;
    public DropData[] normalDrops; //노말 몬스터의 드랍데이터
    public DropData[] namedDrops; //네임드 몬스터의 드랍데이터

    //비활성화 시
    private void OnDisable()
    {
        //씬이 로드 상태가 아닐 경우 리턴
        if (!gameObject.scene.isLoaded) 
            return;

        DropItems(); //아이템 드랍
    }

    private void DropItems()
    {
        //드랍 데이터가 네임드일 경우 네임드 드랍, 아닐 경우 노말 드랍
        DropData[] currentDrops = rate == Rate.Named ? namedDrops : normalDrops;

        float ran = Random.value; //랜덤 값
        float sum = 0f; //합계

        foreach (var drop in currentDrops) //드랍아이템 반복문
        {
            sum += drop.dropChance; //아이템 별 드랍 확률 합계
            if (ran <= sum)
            {
                Instantiate(drop.item, transform.position, Quaternion.identity); //해당 드랍 아이템 생성
                return;
            }
        }
    }
}
