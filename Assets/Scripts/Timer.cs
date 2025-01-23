using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] private float maxTime = 10f;
    [SerializeField] private TextMeshProUGUI timerText;

    public static Timer Instance { get; private set; }
    private float currentTime;
    private bool countingDown = false;

    void Start()
    {
        currentTime = maxTime;
        countingDown = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (countingDown)
        {
            currentTime -= Time.deltaTime;
            timerText.text = Mathf.Max(0, Mathf.CeilToInt(currentTime)).ToString();

            if (currentTime <= 0)
            {
                countingDown = false;
                TimerEnded();
            }
        }
    }


    private void TimerEnded()
    {
        Debug.Log("Se acabo el tuiempo");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TimeUp();
        }
    }

    public void ResetTimer()
    {
        currentTime = maxTime;
        countingDown = true;
    }

    public void PauseTimer()
    {
        countingDown = false;
        int timeLeft = Mathf.Max(0, Mathf.CeilToInt(currentTime));
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CalculateScore(timeLeft, (int)maxTime);
        }
        else
        {
            Debug.LogError("ScoreManager.Instance es null. Asegúrate de que ScoreManager esté correctamente inicializado.");
        }
    }

    public float GetMaxTime()
    {
        return maxTime;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

}
