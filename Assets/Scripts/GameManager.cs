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

    [Header("References")]
    public RingController ring;
    public PatchController patch;

    [Header("Game Settings")]
    public float perfectFitThreshold = 2f; // degrees
    public float goodFitThreshold = 10f;    // degrees
    public float failFitThreshold = 20f;   // degrees

    [Header("Stats")]
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

    [Header("Level Configuration")]
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
            // Use base for endless or just generate procedurally
            data = endlessLevelBase;
            // Procedural increase for endless?
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
            // Default logic if no data exists
            targetFits = 5 + (level - 1) * 2;
            ring.SetRotationSpeed(100f + level * 5f);
        }

        patch.ResetPatch();
        if (UIManager.Instance != null) UIManager.Instance.UpdateGameUI();
    }

    public void CheckFit(PatchController patchController)
    {
        if (currentState != GameState.Playing) return;

        // Buff: Perfect Lock
        if (BuffManager.Instance.perfectLockRemaining > 0)
        {
            BuffManager.Instance.perfectLockRemaining--;
            HandleFit(10, true);
            return;
        }

        // The patch is fired from the bottom (angle 270 degrees)
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
        // Buff: Double Score
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
            Debug.Log("Perfect Fit! Combo: " + combo);

            // Update Task
            RetentionManager.Instance.UpdateTaskProgress("Perfect Fit", 1);
            RetentionManager.Instance.UpdateTaskProgress("Combo", combo);

            // Trigger buffs based on combo
            if (combo == 5)
            {
                BuffManager.Instance.ActivateBuff(BuffType.PerfectLock);
            }
            else if (combo == 3)
            {
                BuffManager.Instance.ActivateBuff(BuffType.DoubleScore);
            }
            
            // Play Sound
            AudioManager.Instance.PlaySound("Perfect");
        }
        else
        {
            combo = 0;
            successfulFits++;
            Debug.Log("Good Fit!");
            
            // Play Sound
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
        Debug.Log("Game Over!");
        UIManager.Instance.ShowResultPanel(false, score);
    }

    private void WinLevel()
    {
        currentState = GameState.LevelComplete;
        Debug.Log("Level Complete!");

        // Update Task
        RetentionManager.Instance.UpdateTaskProgress("Complete", 1);

        UIManager.Instance.ShowResultPanel(true, score);
    }
}
