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

[CreateAssetMenu(fileName = "CharaInitialDataSO", menuName = "Create CharaInitialDataSO")]
public class CharaInitialDataSO : ScriptableObject
{
    public List<CharaInitialData> charaInitialDataList = new();


    [System.Serializable]
    public class CharaInitialData
    {
        public string name;
        public CharaName englishName;
        public Profession profession;
        public Attribute attribute;

        public int initialCombatPower;
        public int initialAttackPower;
        public int initialDefencePower;
        public int initialHp;
        public int initialCriticalRate;

        // TODO 各ステータスの増加率、他のステータス追加


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="datas"></param>
        public CharaInitialData(string[] datas)
        {
            name = datas[0];
            englishName = (CharaName)System.Enum.Parse(typeof(CharaName), datas[1]);
            profession = (Profession)System.Enum.Parse(typeof(Profession), datas[2]);
            attribute = (Attribute)System.Enum.Parse(typeof(Attribute), datas[3]);
            initialCombatPower = int.Parse(datas[4]);
            initialAttackPower = int.Parse(datas[5]);
            initialDefencePower = int.Parse(datas[6]);
            initialHp = int.Parse(datas[7]);
            initialCriticalRate = int.Parse(datas[8]);
        }
    }
}
