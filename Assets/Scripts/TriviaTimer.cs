using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] private float countdownTime = 10f;
    [SerializeField] private TextMeshProUGUI timerText;
    private float currentTime;
    private bool countingDown = false;

    void Start()
    {
        currentTime = countdownTime;
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
        currentTime = countdownTime;
        countingDown = true;
    }

    public void PauseTimer()
    {
        countingDown = false;
    }
}
