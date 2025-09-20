using UnityEngine;
using TMPro;

public class ScoreAndTimerManager : MonoBehaviour
{
    public static ScoreAndTimerManager Instance;

    private int score = 0;
    private float timer = 0f;
    private bool isRunning = true;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    public int GetScore() => score;
    public float GetTime() => timer;

    public void StopTimer() => isRunning = false;

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
