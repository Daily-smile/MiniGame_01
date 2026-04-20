using UnityEngine;
using System.Collections.Generic;

public class RingController : MonoBehaviour
{
    [Header("旋转设置")]
    public float rotationSpeed = 100f;
    public bool isClockwise = true;

    [Header("缺口设置")]
    public List<GapInfo> gaps = new List<GapInfo>();

    [System.Serializable]
    public class GapInfo
    {
        public float centerAngle; // 角度（度），相对于圆环的前方
        public float widthAngle;  // 缺口的宽度（度）
        public bool isTarget;     // 这是否是当前贴片对应的正确缺口？
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

    // 获取缺口在世界空间中的当前角度（相对于 Vector3.up/down）
    public float GetGapWorldAngle(int index)
    {
        if (index < 0 || index >= gaps.Count) return 0f;
        
        // 根据当前旋转将局部角度转换为世界空间角度
        float currentRotation = transform.eulerAngles.z;
        float worldAngle = (currentRotation + gaps[index].centerAngle) % 360f;
        if (worldAngle < 0) worldAngle += 360f;
        return worldAngle;
    }
}
