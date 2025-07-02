using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<WaveData> waveList; //웨이브 데이터 리스트
    [SerializeField] private float timer = 0.0f; //타이머
    private int currentWaveIndex = 0; //현재 웨이브의 인덱스 

    [SerializeField] private ChessEvent chessEvent;
    float eventTimer = 0f;
    float eventTime = 30.0f

    Player player; //플레이어 스크립트
    [SerializeField] Transform target; //타겟의 위치
    [SerializeField] GameObject bossAppearancet; //보스 출연 오브젝트
    bool isBossSpawn = false;

    WaitForSeconds spawnDelay; //스폰 딜레이

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>(); //플레이어 스크립트 참조
        target = player.transform.GetChild(0).GetComponent<Transform>(); //타겟에 플레이어 위치 값 저장
        waveList = CSVLoader.LoadWaveData("WaveData"); //CSVLoader의 WaveData 로드

        eventTimer = 0.0f;
        eventTime = 30.0f;
    }

    private void Update()
    {
        if (isBossSpawn)
            return;

        timer += Time.deltaTime; //타이머 증가

        //이벤트 몬스터 조건문
        if(eventTimer >= eventTime)
        {
            float ran = Random.Range(25.0f, 35.0f); //랜덤한 이벤트 시간
            eventTime = ran; //이벤트 시간 랜덤
            eventTimer = 0.0f; //이벤트 타이머 초기화
            chessEvent.SetPattern(); //이벤트 패턴 함수 호출
        }

        //현재 웨이브 인덱스가 웨이브 리스트의 카운트보다 적고, 타이머가 웨이브 리스트의 시작시간보다 적을 동안 반복
        while (currentWaveIndex < waveList.Count && timer >= waveList[currentWaveIndex].startTime)
        {
            StartCoroutine(SpawnWave(waveList[currentWaveIndex])); //스폰 웨이브 코루틴 실행
            currentWaveIndex++; //현재 웨이브 인덱스 값 1 증가
        }

        //보스 소환 조건문
        if(timer >= 300.0f && !isBossSpawn)
        {
            isBossSpawn = true; //보스 스폰여부 참
            bossAppearancet.SetActive(true); //보스 등장 오브젝트 활성화
            Time.timeScale = 0.0f; //타임스케일 0
        }
    }

    //스폰 웨이브 코루틴
    private IEnumerator SpawnWave(WaveData wave)
    {
        //웨이브가 가지고 있는 서브 웨이브의 서브 웨이브를 반복
        foreach (var subWave in wave.subWaves)
        {
            StartCoroutine(SpawnSubWave(subWave)); //서브 웨이브 코루틴 실행
        }
        yield return null; //Update 함수로 이동 후 한 프레임 이후 호출 -> currentWaveIndex++이 실행되고 다시 돌아옴
    }

    //스폰 서브 웨이브 코루틴 실행
    private IEnumerator SpawnSubWave(SubWaveData subWave)
    {
        spawnDelay = new WaitForSeconds(subWave.spawnInterval); //스폰 딜레이 값 변경

        //서브 웨이브의 스폰 카운트 값 이상까지 반복
        for (int i = 0; i < subWave.spawnCount; i++)
        {
            SpawnEnemy(subWave.enemyType); //실질적인 적 스폰 함수 실행
            yield return spawnDelay; //스폰 딜레이 시간 이후 호출
        }
    }

    //적 스폰
    private void SpawnEnemy(string enemyType) //EnemyType enemyType
    {
        //적 타입이 Rook_Event_NoMove일 경우
        if (enemyType == "Rook_Event_NoMove")
        {
            EllipseSpawn(100, 14f, 9f, enemyType); //타원형 스폰 함수 실행
            return;
        }

        //Vector3 spawnPos = GetRandomSpawnPosition(); //화면 밖 랜덤 스폰 함수의 값 가져오기
        Vector3 spawnPos = GetRandomSpawnPosition_Circle(); //화면 밖 랜덤 스폰 함수의 값 가져오기

        GameObject enemyToSpawn = ObjectPool.Instance.SpawnFromPool_Enemy(enemyType, spawnPos);
        //오브젝트 풀링에 해당 적의 타입과 위치의 값을 전달하여 가져오기

        if (enemyToSpawn != null)
        {
            enemyToSpawn.SetActive(true); //스폰할 적 활성화하기
        }
    }

    //일반 몬스터 스폰 -> 화면 밖에서 스폰 (사각형 모양 스폰)
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPos = Vector3.zero; //랜덤위치 초기화
        float min = -0.2f; //화면에서 벗어나는 최소 거리
        float max = 1.2f; //화면에서 벗어나는 최대 거리
        float zPos = 10; //3D Z축 위치 설정

        int flag = Random.Range(0, 4); //방향 선택

        switch (flag)
        {
            case 0: //오른쪽
                randomPos = new Vector3(max, Random.Range(min, max), zPos);
                break;
            case 1: //왼쪽
                randomPos = new Vector3(min, Random.Range(min, max), zPos);
                break;
            case 2: //위
                randomPos = new Vector3(Random.Range(min, max), max, zPos);
                break;
            case 3: //아래
                randomPos = new Vector3(Random.Range(min, max), min, zPos);
                break;
        }
        return Camera.main.ViewportToWorldPoint(randomPos);
        //월드 좌표에서 뷰포트 좌표로 변환
    }

    //일반 몬스터 스폰 -> 화면 밖에서 스폰 (원형 모양 스폰)
    private Vector3 GetRandomSpawnPosition_Circle()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI); // 랜덤한 방향 각도 (0~360도)

        float circleX = 0.7f; // x방향 반지름
        float circleY = 0.7f; // y방향 반지름

        float x = Mathf.Cos(angle) * circleX + 0.5f; // 중심을 기준으로 이동
        float y = Mathf.Sin(angle) * circleY + 0.5f;

        float zPos = 10f; // Z축 위치

        Vector3 viewportPos = new Vector3(x, y, zPos);
        return Camera.main.ViewportToWorldPoint(viewportPos);
    }

    //타원 모양으로 적 스폰 -> 플레이어를 이동 제한하는 Rook_Event_NoMove 적 스폰
    private void EllipseSpawn(int count, float a, float b, string enemyType) //적의 수, +- x값, +- y갑, 적 타입
    {
        Vector3 center = target.transform.position; //타겟의 위치를 중앙으로 설정

        //적의 수만큼 반복하여 실행
        for (int i = 0; i < count; i++)
        {
            float angle = (float)i / (float)count * 2 * Mathf.PI; //균등 분할
            float x = a * Mathf.Cos(angle); //타원의 가로 반지름
            float y = b * Mathf.Sin(angle); //타원의 세로 반지름

            Vector3 spawnPos = center + new Vector3(x, y, 0f); //타원형으로 위치 이동 시키기
            GameObject enemyToSpawn = ObjectPool.Instance.SpawnFromPool_Enemy(enemyType, spawnPos);
            //오브젝트 풀링에 해당 적의 타입과 위치의 값을 전달하여 가져오기
        }

    }
}
