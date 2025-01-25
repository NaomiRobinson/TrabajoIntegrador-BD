using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public int points = 10;
    public int questionPoints;
    public int triviaScore;
    private int timeUsed;

    public static ScoreManager Instance { get; private set; }

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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CalculateScore(int timeLeft, int maxTime)
    {
        Debug.Log("Calculando puntaje...");
        timeUsed = maxTime - timeLeft;
        questionPoints = Mathf.Max(0, points - timeUsed);
        triviaScore += questionPoints;
        Debug.Log($"Puntos obtenidos en esta pregunta: {questionPoints}");
        Debug.Log($"Puntaje total acumulado: {triviaScore}");


    }

}
