using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; //싱글톤을 위한 인스턴스

    [Header("오디오 소스")]
    public AudioSource bgmSource; //배경음악 오디오소스
    [SerializeField] private List<AudioSource> sfxSources; //효과음 오디오소스 리스트
    private int currentSfxIndex = 0; //효과음 오디오소스 리스트 인덱스 변수

    [Header("무기 오디오 클립")]
    public AudioClip bookAttackSound; //책 무기 사운드
    public AudioClip watchAttackSound; //회중시계 무기 사운드
    public AudioClip teaAttackSound; //홍차 무기 사운드
    public AudioClip catAttackSound; //체셔캣 무기 사운드
    public AudioClip swordAttackSound; //보팔검 무기 사운드
    public AudioClip hatAttackSound; //모자 무기 사운드
    public AudioClip cardAttackSound; //카드 무기 사운드
    public AudioClip appleAttackSound; //사과 무기 사운드
    public AudioClip defaultAttackSound; //기본 무기 사운드 -> 컴파일 오류 방지를 위한 사운드 기본값

    [Header("몬스터 오디오 클립")]
    public AudioClip fixedGuillotione; //고정형 길로틴 사운드
    public AudioClip movedGuillotione; //이동형 길로틴 사운드
    [SerializeField] private AudioClip enemyHit; //적 피격 사운드
    [SerializeField] private AudioClip enemyDeath; //적 사망 사운드
    [SerializeField] private AudioClip enemyExplosion; //적 폭발 사운드

    [Header("기타 오디오 클립")]
    [SerializeField] private AudioClip heal; //회복 획득 사운드
    [SerializeField] private AudioClip expGem; //경험치 잼 획득 사운드
    [SerializeField] private AudioClip levelUp; //레벨업 사운드
    [SerializeField] private AudioClip magnet; //자석 획득 사운드

    float hitTimer = 0;   //사운드가 중첩되어 사운드가 깨지지않게 하기 위한 타이머
    float expTimer = 0;   //고로 효과음 중복 방지를 위한 타이머
    float deathTimer = 0; //...

    private void Awake()
    {
        //싱글톤 패턴을 구현하여 인스턴스가 중복되지 않도록 한다
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        //각 타이머의 값들을 매 프레임 갱신
        hitTimer += Time.deltaTime;
        expTimer += Time.deltaTime;
        deathTimer += Time.deltaTime;
    }


    //효과음 재생
    public void PlaySFX(string effect)
    {
        //효과음의 종류을 구분
        AudioSource source = sfxSources[currentSfxIndex];

        //switch 표현식
        AudioClip clip = effect switch
        {
            "heal" => heal,
            "expGem" when expTimer > 0.08f => expGem,
            "levelUp" => levelUp,
            "enemyDeath" when deathTimer > 0.1f => enemyDeath,
            "explosion" => enemyExplosion,
            "magnet" => magnet,
            "fixedGuillotione" => fixedGuillotione,
            "movedGuillotione" => movedGuillotione,
            _ => null
        };

        if (clip != null)
        {
            if (effect == "expGem") expTimer = 0;
            if (effect == "enemyDeath") deathTimer = 0;

            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(clip);
        }

        currentSfxIndex = (currentSfxIndex + 1) % sfxSources.Count; //인덱스 값 변경
    }

    //몬스터 피격 사운드
    public void PlayHitSound(WeaponType weaponType)
    {
        //파이프와 폭죽 무기의 경우 사운드가 없으므로 리턴을 해줌
        if (weaponType == WeaponType.Pipe || weaponType == WeaponType.Firecracker)
            return;

        //피격 타이머 조건문
        if (hitTimer > 0.1f)
        {
            AudioSource source = sfxSources[currentSfxIndex];

            hitTimer = 0; //피격 타이머 초기화
            source.pitch = Random.Range(0.8f, 1.2f); //랜덤 피치값
            source.PlayOneShot(enemyHit); //사운드 재생

            currentSfxIndex = (currentSfxIndex + 1) % sfxSources.Count; //인덱스 값 변경
        }
    }

    //플레이어 무기 사운드
    public void PlayWeaponSound(WeaponType weaponType)
    {
        AudioSource source = sfxSources[currentSfxIndex];

        //switch 표현식
        AudioClip clip = weaponType switch
        {
            WeaponType.Book => bookAttackSound,
            WeaponType.Watch => watchAttackSound,
            WeaponType.Cat => catAttackSound,
            WeaponType.Sword => swordAttackSound,
            WeaponType.Tea=> teaAttackSound,
            WeaponType.Hat=> hatAttackSound,
            WeaponType.Card => cardAttackSound,
            WeaponType.Apple=> appleAttackSound,
            _ => defaultAttackSound
        };

        if (clip != null)
        {
            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(clip);
            currentSfxIndex = (currentSfxIndex + 1) % sfxSources.Count; //인덱스 값 변경
        }

    }
}