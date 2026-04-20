using UnityEngine;

public class PatchController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 10f;
    public float targetRadius = 2f;
    public float startRadius = 5f;

    [Header("状态")]
    private bool isFiring = false;
    private Vector3 targetPosition = Vector3.zero;

    public void Fire()
    {
        if (isFiring) return;
        isFiring = true;
        // 初始位置在距离中心 startRadius 处
        transform.position = Vector3.down * startRadius;
    }

    void Update()
    {
        if (!isFiring) return;

        // 向中心移动
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // 检查是否到达圆环半径
        float currentDistance = Vector3.Distance(transform.position, targetPosition);
        if (currentDistance <= targetRadius)
        {
            OnReachRing();
        }
    }

    private void OnReachRing()
    {
        isFiring = false;
        // 命中/失误逻辑将在这里或从 GameManager 调用
        // 调用 GameManager 的 CheckFit 方法
        GameManager.Instance.CheckFit(this);
    }

    public void ResetPatch()
    {
        isFiring = false;
        transform.position = Vector3.down * startRadius;
    }
}
