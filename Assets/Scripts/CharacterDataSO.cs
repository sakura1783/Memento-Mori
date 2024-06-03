using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Create CharacterDataSO")]
public class CharacterDataSO : ScriptableObject
{
    public List<CharacterData> characterDataSO = new();


    [System.Serializable]
    public class CharacterData
    {

    }
}
