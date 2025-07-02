using System.Collections.Generic;

[System.Serializable]
public class SubWaveData
{
    public string enemyType; //적의 타입
    public int spawnCount; //적의 수
    public float spawnInterval; //적 스폰 간격
}

[System.Serializable]
public class WaveData
{
    public float startTime; //웨이브 시작 시간
    public List<SubWaveData> subWaves; //여러 적 리스트
}