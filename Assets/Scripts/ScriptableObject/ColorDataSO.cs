using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorDataSO", menuName = "Create ColorDataSO")]
public class ColorDataSO : ScriptableObject
{
    public List<ColorData> colorDataList = new();


    [System.Serializable]
    public class ColorData
    {
        public ColorType colorType;
        public string hexadecimalColor;  // 16進数
    }
}
