using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class DailyTask
{
    public string description;
    public int targetValue;
    public int currentValue;
    public int rewardGold;
    public bool isCompleted;
    public bool isClaimed;
}

public class RetentionManager : MonoBehaviour
{
    public static RetentionManager Instance { get; private set; }

    [Header("Sign-in")]
    public int consecutiveDays = 0;
    public DateTime lastSignInDate;
    public bool hasSignedToday = false;

    [Header("Daily Tasks")]
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
        // Simple PlayerPrefs for demo
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
                // Continue streak
            }
            else
            {
                consecutiveDays = 0;
            }
        }
    }

    private void InitializeTasks()
    {
        dailyTasks.Clear();
        dailyTasks.Add(new DailyTask { description = "Complete 5 levels", targetValue = 5, rewardGold = 100 });
        dailyTasks.Add(new DailyTask { description = "Get 10 Perfect Fits", targetValue = 10, rewardGold = 200 });
        dailyTasks.Add(new DailyTask { description = "Reach 5 Combo", targetValue = 5, rewardGold = 150 });
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

        Debug.Log("Signed in for day " + consecutiveDays);
    }

    public void UpdateTaskProgress(string taskType, int amount)
    {
        // Logic to update tasks based on gameplay
        foreach (var task in dailyTasks)
        {
            if (task.description.Contains(taskType))
            {
                task.currentValue += amount;
                if (task.currentValue >= task.targetValue && !task.isCompleted)
                {
                    task.isCompleted = true;
                    Debug.Log("Task Completed: " + task.description);
                }
            }
        }
    }
}
