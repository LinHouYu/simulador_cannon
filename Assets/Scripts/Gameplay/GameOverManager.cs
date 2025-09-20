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

    [Header("引用")]
    public cannon cannonScript;

    private bool isGameOver = false;

    // 排行榜数据
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
            gameOverPanel.SetActive(false); // 开始时隐藏菜单
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
            Debug.Log("GameOver 条件触发"); // 调试用
            TriggerGameOver();
        }
    }


    void TriggerGameOver()
    {
        isGameOver = true;

        // 停止计时
        ScoreAndTimerManager.Instance.StopTimer();

        // 显示结算面板
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // 玩家点击“提交成绩”按钮时调用
    public void SubmitScore()
    {
        string playerName = nameInputField != null && !string.IsNullOrEmpty(nameInputField.text)
            ? nameInputField.text
            : "Player";

        int score = ScoreAndTimerManager.Instance.GetScore();
        float time = ScoreAndTimerManager.Instance.GetTime();

        // 保存成绩
        rankingList.Add(new PlayerScore { playerName = playerName, score = score, time = time });

        // 排序（分数高到低）
        rankingList = rankingList.OrderByDescending(p => p.score).ToList();

        // 显示玩家成绩
        if (finalResultText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            finalResultText.text = $"{playerName} - Score: {score} - Time: {minutes:00}:{seconds:00}";
        }

        // 显示排行榜
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
