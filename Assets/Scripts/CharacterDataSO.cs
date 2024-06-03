using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Create CharacterDataSO")]
public class CharacterDataSO : ScriptableObject
{
    public List<CharacterData> characterDataList = new();


    [System.Serializable]
    public class CharacterData
    {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="datas"></param>
        public CharacterData(string[] datas)
        {

        }
    }
}
