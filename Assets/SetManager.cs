using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetManager : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;

    [Header("Score Variables")]
    [SerializeField] TMP_Text setsRemainingText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text scoreAlertText;
    [SerializeField] int maxSets;
    [SerializeField] float pointAlertSpeed;
    [SerializeField] AnimationCurve pointAlertCurve;

    [Header("End Screen Variables")]
    [SerializeField] GameObject endScreen;
    [SerializeField] TMP_Text finalScore;

    int setsRemaining;
    int score;

    private void Start()
    {
        setsRemaining = maxSets;
    }

    public void StartSet()
    {
        if (setsRemaining <= 0)
        {
            EndGame();
        }

        setsRemaining--;
        setsRemainingText.text = "Sets Remaining: " + setsRemaining;
    }

    public void AddPoints(int points)
    {
        score += points;
        scoreText.text = "Score: " + score;

        StartCoroutine(ShowPointAlert(points));
    }

    IEnumerator ShowPointAlert(int points)
    {
        if (points > 1)
            scoreAlertText.text = "That's " + points + " points!";
        else
            scoreAlertText.text = "That's " + points + " point!";

        scoreAlertText.gameObject.SetActive(true);

        float newScaleFloat = 0f;
        scoreAlertText.rectTransform.localScale = new Vector3(newScaleFloat, newScaleFloat, newScaleFloat);

        while(newScaleFloat < 1f)
        {
            newScaleFloat += Time.deltaTime / pointAlertSpeed;

            float tempScale = pointAlertCurve.Evaluate(newScaleFloat);

            scoreAlertText.rectTransform.localScale = new Vector3(tempScale, tempScale, tempScale);

            yield return new WaitForEndOfFrame();
        }

        newScaleFloat = 1f;
        scoreAlertText.rectTransform.localScale = new Vector3(newScaleFloat, newScaleFloat, newScaleFloat);

        while (newScaleFloat > 0f)
        {
            newScaleFloat -= Time.deltaTime / pointAlertSpeed;

            float tempScale = pointAlertCurve.Evaluate(newScaleFloat);

            scoreAlertText.rectTransform.localScale = new Vector3(tempScale, tempScale, tempScale);

            yield return new WaitForEndOfFrame();
        }

        newScaleFloat = 0f;
        scoreAlertText.rectTransform.localScale = new Vector3(newScaleFloat, newScaleFloat, newScaleFloat);
        scoreAlertText.gameObject.SetActive(false);
    }

    void EndGame()
    {
        playerInput.SwitchCurrentActionMap("Menu");

        scoreAlertText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        setsRemainingText.gameObject.SetActive(false);

        endScreen.SetActive(true);
        finalScore.text = score + " points!";
    }
}
