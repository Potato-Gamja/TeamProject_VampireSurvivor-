using UnityEngine;

public class HeartQueenAppearance : MonoBehaviour
{
    [SerializeField] GameObject heartQueen;

    public void RaidStart()
    {
        Time.timeScale = 1.0f;
        heartQueen.SetActive(true);
    }
}
