using UnityEngine;
using System;

public enum BuffType
{
    PerfectLock,
    DoubleScore,
    SlowDown,
    FlawImmunity
}

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance { get; private set; }

    [Header("Buff States")]
    public int perfectLockRemaining = 0;
    public int doubleScoreRemaining = 0;
    public float slowDownTimer = 0f;
    public bool isFlawImmunityActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (slowDownTimer > 0)
        {
            slowDownTimer -= Time.deltaTime;
            if (slowDownTimer <= 0)
            {
                GameManager.Instance.ring.SetRotationSpeed(GameManager.Instance.ring.rotationSpeed * 2f);
            }
        }
    }

    public void ActivateBuff(BuffType type)
    {
        switch (type)
        {
            case BuffType.PerfectLock:
                perfectLockRemaining = 3;
                break;
            case BuffType.DoubleScore:
                doubleScoreRemaining = 5;
                break;
            case BuffType.SlowDown:
                slowDownTimer = 10f;
                GameManager.Instance.ring.SetRotationSpeed(GameManager.Instance.ring.rotationSpeed * 0.5f);
                break;
            case BuffType.FlawImmunity:
                isFlawImmunityActive = true;
                break;
        }
    }

    public void ResetBuffs()
    {
        perfectLockRemaining = 0;
        doubleScoreRemaining = 0;
        slowDownTimer = 0f;
        isFlawImmunityActive = false;
    }
}
