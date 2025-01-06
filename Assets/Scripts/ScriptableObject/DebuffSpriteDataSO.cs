using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebuffSpriteDataSO", menuName = "Create DebuffSpriteDataSO")]
public class DebuffSpriteDataSO : ScriptableObject
{
    public List<DebuffSpriteData> debuffSpriteDataList = new();


    [System.Serializable]
    public class DebuffSpriteData
    {
        public DebuffType debuffType;
        public Sprite sprite;
    }
}
