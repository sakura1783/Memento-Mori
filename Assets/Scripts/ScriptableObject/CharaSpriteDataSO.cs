using System.Collections.Generic;
using UnityEngine;

public enum CharaSpriteType
{
    Full,
    Face,
    Shoulder,

    // TODO 背景透過しないものを作ったらその値も追加
}

[CreateAssetMenu(fileName = "CharaSpriteDataSO", menuName = "Create CharaSpriteDataSO")]
public class CharaSpriteDataSO : ScriptableObject
{
    public List<CharaSpriteData> charaSpriteDataList = new();

    [System.Serializable]
    public class CharaSpriteData
    {
        public CharaName charaName;
        
        public Sprite full;
        public Sprite face;
        public Sprite shoulder;
    }
}
