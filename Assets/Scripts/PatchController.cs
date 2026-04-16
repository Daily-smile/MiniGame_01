using UnityEngine;

public class PatchController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float targetRadius = 2f;
    public float startRadius = 5f;

    [Header("State")]
    private bool isFiring = false;
    private Vector3 targetPosition = Vector3.zero;

    public void Fire()
    {
        if (isFiring) return;
        isFiring = true;
        // Start position is at startRadius from center
        transform.position = Vector3.down * startRadius;
    }

    void Update()
    {
        if (!isFiring) return;

        // Move towards center
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // Check if reached ring's radius
        float currentDistance = Vector3.Distance(transform.position, targetPosition);
        if (currentDistance <= targetRadius)
        {
            OnReachRing();
        }
    }

    private void OnReachRing()
    {
        isFiring = false;
        // Logic for hit/miss will be called here or from GameManager
        // Let's call GameManager's CheckFit method
        GameManager.Instance.CheckFit(this);
    }

    public void ResetPatch()
    {
        isFiring = false;
        transform.position = Vector3.down * startRadius;
    }
}
