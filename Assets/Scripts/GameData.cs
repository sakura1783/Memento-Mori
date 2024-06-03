using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : AbstractSingleton<GameData>
{
    [System.Serializable]
    public class OwnedCharaData
    {
        public CharaName name;
        public int level;

        public int combatPower;
        public int attackPower;
        public int defencePower;
        public int hp;
        public float criticalRate;
    }

    public List<OwnedCharaData> ownedCharaDataList = new();
}
