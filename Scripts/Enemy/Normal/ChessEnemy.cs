using UnityEngine;

public class ChessEnemy : Enemy
{
    public enum ChessType //체스 말 종류
    {
        Pawn, Rook, Bishop, Knight, //일반 적
        Rook_Event_Move, Bishop_Event_Move, //이동하는 이벤트형 적
        Rook_Event_NoMove //고정된 이벤트형 적
    } 

    [Header("이벤트형 체스 관련")]
    public ChessType type; //체스 말 종류
    private bool isSpawn = false; //스폰 여부
    [SerializeField] Vector3 vec; //체스말의 위치

    //Rook_Event_NoMove 관련 변수
    [SerializeField] float lifeTime = 30.0f; //이벤트 활성화 시간
    private float timer = 0; //활성화 시간 타이머

    private void Awake()
    {
        vec = transform.localPosition; //체스말 위치 설정
    }

    private new void Update()
    {
        switch (type)
        {
            case ChessType.Pawn: //흰 폰: 플레이어 추격하는 일반형 체스말
                base.Update(); //상속한 Enemy의 Update 함수 호출
                break;

            case ChessType.Rook: //흰 룩: 플레이어 추격하는 일반형 체스말
                base.Update(); //상속한 Enemy의 Update 함수 호출
                break;

            case ChessType.Bishop: //흰 비숍: 플레이어 추격하는 일반형 체스말
                base.Update(); //상속한 Enemy의 Update 함수 호출
                break;

            case ChessType.Knight: //흰 나이트: 플레이어 추격하는 일반형 체스말
                base.Update(); //상속한 Enemy의 Update 함수 호출
                break;

            case ChessType.Rook_Event_Move: //검정 룩: 십자 모양으로만 이동하는 이벤트형 체스말
                if (transform.parent.gameObject.activeSelf == true && !isSpawn)
                {
                    isSpawn = true;
                    UpdateSpriteFlip(); //상속한 Enemy의 UpdateSpriteFlip 함수 호츨
                }

                UpdateSpriteLayer(); //상속한 Enemy의 UpdateSpriteLayer 함수 호출
                break;

            case ChessType.Bishop_Event_Move: //검정 비숍: 엑스자 모양으로만 이동하는 이벤트형 체스말
                if (transform.parent.gameObject.activeSelf == true && !isSpawn)
                {
                    isSpawn = true;
                    UpdateSpriteFlip(); //상속한 Enemy의 UpdateSpriteFlip 함수 호츨
                }

                UpdateSpriteLayer(); //상속한 Enemy의 UpdateSpriteLayer 함수 호출
                break;

            case ChessType.Rook_Event_NoMove: //검정 룩: 이동을 하지 않고, 타원형으로 생성되어 플레이어를 가두는 체스말
                base.Update();
                timer += Time.deltaTime; //타이머 값 증가
                if (timer >= lifeTime) //활성화 시간 이상 달성
                {
                    gameObject.SetActive(false); //게임오브젝트 비활성화
                }
                break;
        }
    }

    //활성화 시
    protected override void OnEnable()
    {
        base.OnEnable(); //상속한 Enemy의 OnEnable 함수 호출
        rb.linearVelocity = Vector3.zero; //정지 상태
        transform.localPosition = vec; //로컬포지션을 설정해둔 값으로 설정
    }

    private void OnDisable()
    {
        isSpawn= false; //스폰 여부 거짓
    }

}
