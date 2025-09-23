using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugHUD : MonoBehaviour
{
    [Header("引用")]
    public cannon cannonScript;        // 大炮脚本
    public TMP_Text debugText;         // 调试信息显示
    public GameObject gameOverPanel;   // 结束菜单
    public TMP_Text finalResultText;   // 最终成绩
    public TMP_InputField nameInputField; // 玩家输入名字
    public TMP_Text rankingText;       // 排行榜
    public GameObject rankPanel;       // 排行榜区域（默认隐藏）

    [Header("Ranking options")]
    public int topCount = 10;

    private bool isGameOver = false;
    private DatabaseReference dbRef;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (rankPanel != null) rankPanel.SetActive(false);

        var options = new AppOptions
        {
            DatabaseUrl = new Uri("https://cannonrank-6d870-default-rtdb.firebaseio.com/")
        };

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // 创建一个独立实例，避免 DefaultInstance URL 不生效
                var app = FirebaseApp.Create(options, "CannonRankApp");
                dbRef = FirebaseDatabase.GetInstance(app).RootReference;

                // 匿名登录
                FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
                {
                    if (authTask.IsCompleted && !authTask.IsFaulted)
                    {
                        Debug.Log("匿名登录成功: " + authTask.Result.User.UserId);
                    }
                    else
                    {
                        Debug.LogError("匿名登录失败: " + authTask.Exception);
                    }
                });

                Debug.Log("Firebase 初始化成功！");
            }
            else
            {
                Debug.LogError("Firebase 初始化失败: " + task.Result);
            }
        });
    }


    void Update()
    {
        if (debugText == null || cannonScript == null) return;

        int ammo = cannonScript.GetCurrentAmmo();
        int balls = GameObject.FindGameObjectsWithTag("Ball").Length;
        int targets = GameObject.FindGameObjectsWithTag("Target").Length;
        int walls = GameObject.FindGameObjectsWithTag("Wall").Length;

        debugText.text =
            $"[DEBUG]\n" +
            $"Ammo Left: {ammo}\n" +
            $"Balls In Scene: {balls}\n" +
            $"Targets Left: {targets}\n" +
            $"Walls Left: {walls}";

        if (!isGameOver)
        {
            bool noAmmo = (ammo <= 0);
            bool noBallInScene = (balls == 0);
            bool noTargets = (targets == 0);
            bool noWalls = (walls == 0);

            if ((noAmmo && noBallInScene) || (noTargets && noWalls))
            {
                Debug.Log("条件满足，调用 TriggerGameOver()");
                TriggerGameOver();
            }
        }
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        ScoreAndTimerManager.Instance.StopTimer();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("GameOverPanel 已经被激活: " + gameOverPanel.activeInHierarchy);
        }
        else
        {
            Debug.LogError("GameOverPanel 没有拖到 Inspector！");
        }

        int score = ScoreAndTimerManager.Instance.GetScore();
        float time = ScoreAndTimerManager.Instance.GetTime();
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if (finalResultText != null)
            finalResultText.text = $"Score: {score} - Time: {minutes:00}:{seconds:00}";
        else
            Debug.LogError("finalResultText 没有拖到 Inspector！");

        Debug.Log("TriggerGameOver() 已执行");
    }

    // 玩家点击“确认名字”按钮时调用
    public void SubmitAndShowRanking()
    {
        string playerName = !string.IsNullOrEmpty(nameInputField?.text) ? nameInputField.text : "Player";
        int score = ScoreAndTimerManager.Instance.GetScore();
        float time = ScoreAndTimerManager.Instance.GetTime();

        UploadScore(playerName, score, time, () =>
        {
            LoadRankingAndShow();
        });
    }

    private void UploadScore(string name, int score, float time, Action onSuccess)
    {
        if (dbRef == null)
        {
            Debug.LogError("Firebase 未初始化");
            return;
        }

        string key = dbRef.Child("rankings").Push().Key;
        var entry = new Dictionary<string, object>
        {
            { "name", name },
            { "score", score },
            { "time", time }
        };

        dbRef.Child("rankings").Child(key).SetValueAsync(entry)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("上传失败: " + task.Exception);
            }
            else
            {
                Debug.Log("上传成功");
                onSuccess?.Invoke();
            }
        });
    }

    private void LoadRankingAndShow()
    {
        if (dbRef == null)
        {
            Debug.LogError("Firebase 未初始化");
            return;
        }

        dbRef.Child("rankings").GetValueAsync()
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("拉取失败: " + task.Exception);
                return;
            }

            var snapshot = task.Result;
            var list = new List<(string name, int score, float time)>();

            foreach (var child in snapshot.Children)
            {
                string name = child.Child("name").Value?.ToString() ?? "Player";
                int score = int.TryParse(child.Child("score").Value?.ToString(), out var sc) ? sc : 0;
                float time = float.TryParse(child.Child("time").Value?.ToString(), out var tm) ? tm : 0f;
                list.Add((name, score, time));
            }

            var sorted = list.OrderByDescending(x => x.score).ThenBy(x => x.time).Take(topCount).ToList();

            if (rankingText != null)
            {
                rankingText.text = "Ranking:\n";
                for (int i = 0; i < sorted.Count; i++)
                {
                    int m = Mathf.FloorToInt(sorted[i].time / 60);
                    int s = Mathf.FloorToInt(sorted[i].time % 60);
                    rankingText.text += $"{i + 1}. {sorted[i].name} - {sorted[i].score} pts - {m:00}:{s:00}\n";
                }
            }

            if (rankPanel != null) rankPanel.SetActive(true);
        });
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
