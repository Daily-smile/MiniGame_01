using UnityEngine;
using System.Collections.Generic;

public enum GameState
{
    Start,
    Playing,
    LevelComplete,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("引用")]
    public RingController ring;
    public PatchController patch;

    [Header("游戏设置")]
    public float perfectFitThreshold = 2f; // 度数
    public float goodFitThreshold = 10f;    // 度数
    public float failFitThreshold = 20f;   // 度数

    [Header("统计数据")]
    public int score = 0;
    public int combo = 0;
    public int currentLevel = 1;
    public int targetFits = 5;
    public int successfulFits = 0;

    public GameState currentState = GameState.Start;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartLevel(1);
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                patch.Fire();
            }
        }
    }

    [Header("关卡配置")]
    public List<LevelData> levelDataList;
    public LevelData endlessLevelBase;

    public void StartLevel(int level)
    {
        currentLevel = level;
        successfulFits = 0;
        currentState = GameState.Playing;

        LevelData data = null;
        if (level <= levelDataList.Count)
        {
            data = levelDataList[level - 1];
        }
        else
        {
            // 对于无尽模式使用基础配置或程序化生成
            data = endlessLevelBase;
            // 无尽模式的程序化增长？
        }

        if (data != null)
        {
            targetFits = data.targetFits;
            ring.SetRotationSpeed(data.rotationSpeed);
            ring.isClockwise = data.isClockwise;
            ring.SetGaps(data.gapList);
        }
        else
        {
            // 如果没有数据，使用默认逻辑
            targetFits = 5 + (level - 1) * 2;
            ring.SetRotationSpeed(100f + level * 5f);
        }

        patch.ResetPatch();
        if (UIManager.Instance != null) UIManager.Instance.UpdateGameUI();
    }

    public void CheckFit(PatchController patchController)
    {
        if (currentState != GameState.Playing) return;

        // Buff: 完美锁定
        if (BuffManager.Instance.perfectLockRemaining > 0)
        {
            BuffManager.Instance.perfectLockRemaining--;
            HandleFit(10, true);
            return;
        }

        // 贴片从底部发射（角度为 270 度）
        float targetWorldAngle = 270f;
        
        bool matchedAnyGap = false;
        float minAngleDiff = 360f;

        for (int i = 0; i < ring.gaps.Count; i++)
        {
            float gapAngle = ring.GetGapWorldAngle(i);
            float diff = Mathf.Abs(Mathf.DeltaAngle(gapAngle, targetWorldAngle));
            
            if (diff < minAngleDiff)
            {
                minAngleDiff = diff;
            }

            if (ring.gaps[i].isTarget && diff < goodFitThreshold)
            {
                matchedAnyGap = true;
            }
        }

        if (minAngleDiff <= perfectFitThreshold)
        {
            HandleFit(10, true);
        }
        else if (minAngleDiff <= goodFitThreshold)
        {
            HandleFit(5, false);
        }
        else if (BuffManager.Instance.isFlawImmunityActive && minAngleDiff <= failFitThreshold)
        {
            BuffManager.Instance.isFlawImmunityActive = false;
            HandleFit(5, false);
        }
        else
        {
            HandleFail();
        }
    }

    private void HandleFit(int points, bool isPerfect)
    {
        // Buff: 分数翻倍
        if (BuffManager.Instance.doubleScoreRemaining > 0)
        {
            BuffManager.Instance.doubleScoreRemaining--;
            points *= 2;
        }

        score += points;
        if (isPerfect)
        {
            combo++;
            successfulFits++;
            GameLogger.LogSuccess("完美贴合！连击: " + combo, "GAMEPLAY");

            // 更新任务进度
            RetentionManager.Instance.UpdateTaskProgress("Perfect Fit", 1);
            RetentionManager.Instance.UpdateTaskProgress("Combo", combo);

            // 根据连击触发 Buff
            if (combo == 5)
            {
                BuffManager.Instance.ActivateBuff(BuffType.PerfectLock);
            }
            else if (combo == 3)
            {
                BuffManager.Instance.ActivateBuff(BuffType.DoubleScore);
            }
            
            // 播放音效
            AudioManager.Instance.PlaySound("Perfect");
        }
        else
        {
            combo = 0;
            successfulFits++;
            GameLogger.Log("良好贴合！", "GAMEPLAY");
            
            // 播放音效
            AudioManager.Instance.PlaySound("Good");
        }

        UIManager.Instance.UpdateGameUI();

        if (successfulFits >= targetFits)
        {
            WinLevel();
        }
        else
        {
            patch.ResetPatch();
        }
    }

    private void HandleFail()
    {
        combo = 0;
        currentState = GameState.GameOver;
        GameLogger.LogWarning("游戏结束！", "GAME_OVER");
        UIManager.Instance.ShowResultPanel(false, score);
    }

    private void WinLevel()
    {
        currentState = GameState.LevelComplete;
        GameLogger.LogSuccess("关卡完成！", "LEVEL_WIN");

        // 更新任务进度
        RetentionManager.Instance.UpdateTaskProgress("Complete", 1);

        UIManager.Instance.ShowResultPanel(true, score);
    }
}
