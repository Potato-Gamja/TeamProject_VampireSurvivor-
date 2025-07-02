using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVLoader
{
    //파일 이름의 데이터를 로드
    public static List<WaveData> LoadWaveData(string fileName)
    {
        Dictionary<float, WaveData> waveMap = new Dictionary<float, WaveData>();
        //시간 별 웨이브 데이터를 저장할 딕셔너리 생성

        TextAsset csvFile = Resources.Load<TextAsset>(fileName); //Resources 폴더 안에 있는 CSV 로드
        StringReader reader = new StringReader(csvFile.text); //텍스트 줄 단위로 읽기 위해 초기화

        bool isFirstLine = true; //첫번째 줄 건너뛰기 위한 불

        //CSV 파일 끝까지 한 줄씩 읽기
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine(); //현재 줄 읽기

            //첫번째 줄 건너뛰기
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            } 

            string[] values = line.Split(','); //콤마 기준으로 값을 나눔

            float startTime = float.Parse(values[0]); //웨이브 시작 시간
            string enemyType = values[1]; //적의 타입
            int spawnCount = int.Parse(values[2]); //적의 스폰 수
            float spawnInterval = float.Parse(values[3]); //스폰 간격

            //서브 웨이브 데이터 생성
            SubWaveData subWave = new SubWaveData
            {
                enemyType = enemyType, //적 타입
                spawnCount = spawnCount, //적 스폰 수
                spawnInterval = spawnInterval //스폰 간격
            };

            //해당 startTime을 가진 웨이브가 이미 있다면 추가, 없으면 새로 생성
            if (!waveMap.ContainsKey(startTime))
            {
                waveMap[startTime] = new WaveData 
                { startTime = startTime, 
                  subWaves = new List<SubWaveData>()
                };
                //새로운 웨이브 데이터 생성
                
            }

            waveMap[startTime].subWaves.Add(subWave); //웨이브에 서브 웨이브 추가
        }

        List<WaveData> waveList = new List<WaveData>(waveMap.Values); // Dictionary를 리스트로 변환
        waveList.Sort((a, b) => a.startTime.CompareTo(b.startTime)); //시간 순으로 정렬

        return waveList; //웨이브 리스트 반환
    }
}