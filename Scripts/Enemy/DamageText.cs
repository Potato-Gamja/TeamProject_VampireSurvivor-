using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text; //텍스트
    public float floatSpeed = 1.5f; //텍스트 이동속도
    public float lifetime = 2.5f; //텍스트 유지시간
    float timer; //타이머

    private Vector3 moveDir = new Vector3(0, 1f, 0); //움직임 방향 -> 위로 이동

    //설정
    public void Setup(float damage)
    {
        //데미지 값을 문자열로 변경하여 텍스트에 적용
        text.text = Mathf.RoundToInt(damage).ToString();
    }

    private void Update()
    {   
        //유지시간 초과 시
        if(timer >= lifetime)
            gameObject.SetActive(false); //오브젝트 비활성화

        timer += Time.deltaTime; //타이머 증가
        transform.position += moveDir * floatSpeed * Time.deltaTime; //텍스트가 위로 이동
        text.alpha = Mathf.Lerp(text.alpha, 0, Time.deltaTime * 10); //알파값을 변경하여 점차 사라지는 효과
    }

    private void OnDisable()
    {
        text.text = ""; //텍스트값 초기화
        text.alpha = 1; //알파값 초기화
        timer = 0.0f; //타이머 초기화
    }
}
