using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 水彩ぼかし画像の種類
/// </summary>
public enum WatercolorPaintType
{
    Yellow,
    Blue,
    Green,
    Orange,
    Pink,
    DarkPink,
    LightPink,
    Purple,
    DarkPurple,
    Sumire,  // 菫色
}

[CreateAssetMenu(fileName = "WatercolorPaintDataSO", menuName = "Create WatercolorPaintDataSO")]
public class WatercolorPaintDataSO : MonoBehaviour
{
    public List<WatercolorPaintData> watercolorPaintDataList = new();


    [System.Serializable]
    public class WatercolorPaintData
    {
        public WatercolorPaintType watercolorPaintType;
        public Sprite sprite;
    }
}
