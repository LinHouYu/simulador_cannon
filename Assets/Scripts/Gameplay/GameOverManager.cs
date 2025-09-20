using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverPanel;
    public TMP_InputField nameInputField;
    public TMP_Text finalResultText;
    public TMP_Text rankingText;

    [Header("����")]
    public cannon cannonScript;

    private bool isGameOver = false;

    // ���а�����
    private class PlayerScore
    {
        public string playerName;
        public int score;
        public float time;
    }
    private static List<PlayerScore> rankingList = new List<PlayerScore>();

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // ��ʼʱ���ز˵�
    }

    void Update()
    {
        if (isGameOver) return;

        bool noAmmo = (cannonScript != null && cannonScript.GetCurrentAmmo() <= 0);
        bool noBallInScene = GameObject.FindGameObjectsWithTag("Ball").Length == 0;

        bool noTargets = GameObject.FindGameObjectsWithTag("Target").Length == 0;
        bool noWalls = GameObject.FindGameObjectsWithTag("Wall").Length == 0;

        if ((noAmmo && noBallInScene) || (noTargets && noWalls))
        {
            Debug.Log("GameOver ��������"); // ������
            TriggerGameOver();
        }
    }


    void TriggerGameOver()
    {
        isGameOver = true;

        // ֹͣ��ʱ
        ScoreAndTimerManager.Instance.StopTimer();

        // ��ʾ�������
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // ��ҵ�����ύ�ɼ�����ťʱ����
    public void SubmitScore()
    {
        string playerName = nameInputField != null && !string.IsNullOrEmpty(nameInputField.text)
            ? nameInputField.text
            : "Player";

        int score = ScoreAndTimerManager.Instance.GetScore();
        float time = ScoreAndTimerManager.Instance.GetTime();

        // ����ɼ�
        rankingList.Add(new PlayerScore { playerName = playerName, score = score, time = time });

        // ���򣨷����ߵ��ͣ�
        rankingList = rankingList.OrderByDescending(p => p.score).ToList();

        // ��ʾ��ҳɼ�
        if (finalResultText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            finalResultText.text = $"{playerName} - Score: {score} - Time: {minutes:00}:{seconds:00}";
        }

        // ��ʾ���а�
        if (rankingText != null)
        {
            rankingText.text = "Ranking:\n";
            for (int i = 0; i < rankingList.Count; i++)
            {
                var p = rankingList[i];
                int minutes = Mathf.FloorToInt(p.time / 60);
                int seconds = Mathf.FloorToInt(p.time % 60);
                rankingText.text += $"{i + 1}. {p.playerName} - {p.score} pts - {minutes:00}:{seconds:00}\n";
            }
        }
    }
}
