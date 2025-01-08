using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffSpriteDataSO", menuName = "Create BuffSpriteDataSO")]
public class BuffSpriteDataSO : ScriptableObject
{
    public List<BuffSpriteData> buffSpriteDataList = new();


    [System.Serializable]
    public class BuffSpriteData
    {
        public BuffType buffType;
        public Sprite sprite;
    }
}
