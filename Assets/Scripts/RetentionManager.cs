using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class DailyTask
{
    public string description; // 任务描述
    public int targetValue;    // 目标值
    public int currentValue;   // 当前进度值
    public int rewardGold;     // 奖励金币
    public bool isCompleted;   // 是否完成
    public bool isClaimed;     // 是否已领取奖励
}

public class RetentionManager : MonoBehaviour
{
    public static RetentionManager Instance { get; private set; }

    [Header("签到")]
    public int consecutiveDays = 0;    // 连续签到天数
    public DateTime lastSignInDate;    // 上次签到日期
    public bool hasSignedToday = false; // 今日是否已签到

    [Header("每日任务")]
    public List<DailyTask> dailyTasks = new List<DailyTask>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        LoadRetentionData();
        InitializeTasks();
    }

    private void LoadRetentionData()
    {
        // 演示用的简单 PlayerPrefs 存储
        consecutiveDays = PlayerPrefs.GetInt("ConsecutiveDays", 0);
        string lastDateStr = PlayerPrefs.GetString("LastSignInDate", "");
        if (!string.IsNullOrEmpty(lastDateStr))
        {
            lastSignInDate = DateTime.Parse(lastDateStr);
            if (lastSignInDate.Date == DateTime.Today)
            {
                hasSignedToday = true;
            }
            else if (lastSignInDate.Date == DateTime.Today.AddDays(-1))
            {
                // 继续连签
            }
            else
            {
                // 连签中断
                consecutiveDays = 0;
            }
        }
    }

    private void InitializeTasks()
    {
        dailyTasks.Clear();
        dailyTasks.Add(new DailyTask { description = "完成 5 个关卡", targetValue = 5, rewardGold = 100 });
        dailyTasks.Add(new DailyTask { description = "获得 10 次完美贴合", targetValue = 10, rewardGold = 200 });
        dailyTasks.Add(new DailyTask { description = "达到 5 次连击", targetValue = 5, rewardGold = 150 });
    }

    public void SignIn()
    {
        if (hasSignedToday) return;

        consecutiveDays++;
        lastSignInDate = DateTime.Today;
        hasSignedToday = true;

        PlayerPrefs.SetInt("ConsecutiveDays", consecutiveDays);
        PlayerPrefs.SetString("LastSignInDate", lastSignInDate.ToString());
        PlayerPrefs.Save();

        GameLogger.LogSuccess("第 " + consecutiveDays + " 天签到成功", "RETENTION");
    }

    public void UpdateTaskProgress(string taskType, int amount)
    {
        // 根据游戏过程更新任务进度的逻辑
        foreach (var task in dailyTasks)
        {
            if (task.description.Contains(taskType))
            {
                task.currentValue += amount;
                if (task.currentValue >= task.targetValue && !task.isCompleted)
                {
                    task.isCompleted = true;
                    GameLogger.LogSuccess("任务完成: " + task.description, "RETENTION");
                }
            }
        }
    }
}
