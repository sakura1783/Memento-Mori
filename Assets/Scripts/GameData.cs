using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : AbstractSingleton<GameData>
{
    [System.Serializable]
    public class CharaData
    {
        public CharaName name;
        public int level;

        public int combatPower;
        public int attackPower;
        public int defencePower;
        public int hp;
        public float criticalRate;
    }

    public List<CharaData> ownedCharaDataList = new();

    public int clearMapNo = 0;  // クリアしたマップの番号。この値+1が次のマップ番号
    public int clearStageNo = 0;


    // TODO ownedCharaDataListへの追加処理(ガチャで新しいキャラを手に入れた際)
}
