using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level", menuName = "MiniGame/LevelData")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public float rotationSpeed = 100f;
    public bool isClockwise = true;
    public int targetFits = 5;
    public List<RingController.GapInfo> gapList = new List<RingController.GapInfo>();
}
