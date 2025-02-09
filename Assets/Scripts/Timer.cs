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

    public float gameTime = 0f;
    private bool isTiming = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        currentTime = maxTime;
        countingDown = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isTiming)
        {
            gameTime += Time.deltaTime;
        }

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

    public void StartGameTimer()
    {
        isTiming = true;
        Debug.Log("arranco el tiempo");
    }

    public void StopGameTimer()
    {
        isTiming = false;
        Debug.Log("finalizo el tiempo");
    }

    public void ResetGamerTimer()
    {
        gameTime = 0f;
    }

    public float GetGameTime()
    {
        return gameTime;
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CalculateScore(timeLeft, (int)maxTime);
        }
        else
        {
            Debug.LogError("GameeManager.Instance es null. ");
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
