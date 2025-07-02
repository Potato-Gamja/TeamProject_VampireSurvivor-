using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float sec = 0f;
    private int min = 0;

    private void Update()
    {
        sec += Time.deltaTime;

        if (sec >= 60f)
        {
            min++;
            sec = 0;
            BGMPitchChange();
        }

        timerText.text = string.Format("{0:D2}:{1:D2}", min, (int)sec);
    }

    private void BGMPitchChange()
    {
        switch (min)
        {
            case 2:
                SoundManager.Instance.bgmSource.pitch = 0.75f;
                break;
            case 3:
                SoundManager.Instance.bgmSource.pitch = 0.7f;
                break;
            case 4:
                SoundManager.Instance.bgmSource.pitch = 0.65f;
                break;
            case 5:
                SoundManager.Instance.bgmSource.pitch = 0.6f;
                break;
        }
    }
}
