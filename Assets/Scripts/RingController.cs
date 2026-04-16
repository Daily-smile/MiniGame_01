using UnityEngine;
using System.Collections.Generic;

public class RingController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public bool isClockwise = true;

    [Header("Gap Settings")]
    public List<GapInfo> gaps = new List<GapInfo>();

    [System.Serializable]
    public class GapInfo
    {
        public float centerAngle; // In degrees, relative to ring's forward
        public float widthAngle;  // Width of the gap in degrees
        public bool isTarget;     // Is this the correct gap for the current patch?
    }

    void Update()
    {
        float direction = isClockwise ? -1f : 1f;
        transform.Rotate(Vector3.forward, direction * rotationSpeed * Time.deltaTime);
    }

    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    public void SetGaps(List<GapInfo> newGaps)
    {
        gaps = newGaps;
    }

    // Get current angle of a gap in world space (relative to Vector3.up/down)
    public float GetGapWorldAngle(int index)
    {
        if (index < 0 || index >= gaps.Count) return 0f;
        
        // Transform the local angle to world space based on current rotation
        float currentRotation = transform.eulerAngles.z;
        float worldAngle = (currentRotation + gaps[index].centerAngle) % 360f;
        if (worldAngle < 0) worldAngle += 360f;
        return worldAngle;
    }
}
