using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisplayTime : MonoBehaviour
{
    [SerializeField]
    Game game;
    [SerializeField]
    float initialSeconds = 20;
    [SerializeField]
    TextMeshPro text;
    TimeSpan timeSpan;

    [SerializeField]
    string winText;
    [SerializeField]
    string loseText;
    bool gameOver;

    [SerializeField]
    float predelayWin;
    [SerializeField]
    float displayDurationWin;
    [SerializeField]
    AnimationCurve flickerProbabilityThreshholdWin;

    [SerializeField]
    float predelayLose;
    [SerializeField]
    float displayDurationLose;
    [SerializeField]
    AnimationCurve flickerProbabilityThreshholdLose;

    // Start is called before the first frame update
    void Awake()
    {
        game.onGameLost.AddListener(LostGame);
        game.onGameWon.AddListener(WonGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            timeSpan = TimeSpan.FromSeconds(initialSeconds - game.Timer);
            text.text = timeSpan.ToString(@"ss\:fff");
        }
    }

    void WonGame() {
        gameOver = true;
        StartCoroutine(Flicker(winText, predelayWin, displayDurationWin, flickerProbabilityThreshholdWin));
    }

    void LostGame() {
        text.text = "00:000";
        gameOver = true;
        StartCoroutine(Flicker(loseText, predelayLose, displayDurationLose, flickerProbabilityThreshholdLose));
    }


    IEnumerator Flicker(string displayText, float predelay, float displayDuration, AnimationCurve flickerProbabilityThreshhold) {
        yield return new WaitForSeconds(predelay);
        text.text = displayText;
        float timer = 0;
        while (timer < displayDuration) {
            timer += Time.deltaTime;
            float threshhold = flickerProbabilityThreshhold.Evaluate(timer / displayDuration);
            if (UnityEngine.Random.Range(0, 1f) > threshhold) {
                text.gameObject.SetActive(false);
            } else {
                text.gameObject.SetActive(true);
            }
            yield return null;
        }
        text.gameObject.SetActive(false);
    }
}
