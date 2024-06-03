using System.Collections.Generic;
using UnityEngine;

public enum CharaName
{
    Rosevillea,
    Nina,
    Elliot,
    Arilosha,
    Setsuna,
}

/// <summary>
/// 職業
/// </summary>
public enum Profession
{
    ウォーリア,
    ガーディアン,
    ソーサラー,
}

/// <summary>
/// 属性
/// </summary>
public enum Attribute
{
    藍,
    紅,
    翠,
    黄,
    天,
    冥,
}


[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Create CharacterDataSO")]
public class CharacterDataSO : ScriptableObject
{
    public List<CharacterData> characterDataList = new();


    [System.Serializable]
    public class CharacterData
    {
        public string name;
        public CharaName englishName;
        public Profession profession;
        public Attribute attribute;

        public int defaultCombatPower;
        public int defaultAttackPower;
        public int defaultDefencePower;
        public int defaultHp;
        public float defaultCriticalRate;

        // TODO 各ステータスの増加率、他のステータス追加


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="datas"></param>
        public CharacterData(string[] datas)
        {
            name = datas[0];
            englishName = (CharaName)System.Enum.Parse(typeof(CharaName), datas[1]);
            profession = (Profession)System.Enum.Parse(typeof(Profession), datas[2]);
            attribute = (Attribute)System.Enum.Parse(typeof(Attribute), datas[3]);
            defaultCombatPower = int.Parse(datas[4]);
            defaultAttackPower = int.Parse(datas[5]);
            defaultDefencePower = int.Parse(datas[6]);
            defaultHp = int.Parse(datas[7]);
            defaultCriticalRate = float.Parse(datas[8]);
        }
    }
}
