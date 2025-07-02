using System.Collections.Generic;
using UnityEngine;

public class ChessEvent : MonoBehaviour
{
    [SerializeField] private GameObject player; //플레이어 오브젝트

    [SerializeField] private List<GameObject> rooks = new(); //룩 오브젝트 리스트
    [SerializeField] private List<GameObject> bishops = new(); //비숍 오브젝트 리스트

    [SerializeField] private List<Vector3> rooks_Vec = new(); //룩 위치 좌표 리스트
    [SerializeField] private List<Vector3> bishops_Vec = new(); //비숍 위치 좌표 리스트

    [SerializeField] private List<GameObject> rooks_Warn = new(); //룩 경고 오브젝트 리스트
    [SerializeField] private List<GameObject> bishops_Warn = new(); //비숍 경고 오브젝트 리스트

    private List<List<GameObject>> pieces; //2차 리스트
    private List<List<GameObject>> warnings; //2차 리스트
    private List<List<Vector3>> positions; //2차 리스트

    private int type; //타입: 룩, 비숍
    private int num; //패턴 번호: 0~5
    private float lifeTime = 10f; //지속시간
    private float timer = 0f; //타이머
    private bool isSpawn = false; //스폰 진행 여부

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        //각 2차 리스트는 룩, 비숍의 순서
        pieces = new List<List<GameObject>> { rooks, bishops };
        warnings = new List<List<GameObject>> { rooks_Warn, bishops_Warn };
        positions = new List<List<Vector3>> { rooks_Vec, bishops_Vec };
    }

    private void Update()
    {
        if (isSpawn)
        {
            timer += Time.deltaTime; //타이머 증가
            //지속시간 이상 시 체스 비활성화
            if (timer >= lifeTime)
                ChessDisable();
        }
    }

    //패턴 설정
    public void SetPattern()
    {
        isSpawn = true; //스폰 진행 여부 참
        timer = 0f; //타이머 초기화

        type = Random.Range(0, 2); //0: 룩, 1: 비숍
        num = Random.Range(0, 6);  //0~5 패턴

        SetWarning(true); //해당 패턴의 경고 오브젝트 활성화
        Invoke(nameof(SetEvent), 1f); //공격 패턴 활성화
    }

    //경고 상태 변환
    private void SetWarning(bool enable)
    {
        foreach (int i in GetIndices(num))
            warnings[type][i].SetActive(enable); //경고 오브젝트의 상태 변환
    }

    //이벤트 설정
    private void SetEvent()
    {
        SetWarning(false); //경고 비활성화
        SetPos(); //위치 설정
        ActivatePattern(); //패턴 활성화
    }

    //위치 설정
    private void SetPos()
    {
        transform.position = player.transform.position; //플레이어의 위치로 이동

        //2차 리스트로 해당 타입의 패턴의 로컬포지션을 변경
        for (int i = 0; i < pieces[type].Count; i++)
            pieces[type][i].transform.localPosition = positions[type][i];
    }

    //패턴 활성화
    private void ActivatePattern()
    {
        foreach (int i in GetIndices(num))
            pieces[type][i].SetActive(true);
    }

    //체스 비활성화
    private void ChessDisable()
    {
        isSpawn = false;

        foreach (var obj in rooks)
            obj.SetActive(false);

        foreach (var obj in bishops) 
            obj.SetActive(false);
    }

    //패턴 처리
    private int[] GetIndices(int pattern)
    {
        return pattern switch
        {
            0 => new[] { 0, 1 }, //0번, 1번 오브젝트 활성화
            1 => new[] { 2, 3 }, //2번. 3번 오브젝트 활성화
            2 => new[] { 0, 3 }, 
            3 => new[] { 0, 2 },
            4 => new[] { 1, 3 },
            5 => new[] { 1, 2 },
            _ => new int[0],
        };
    }
}
