using System.Collections.Generic;

public class GameData : AbstractSingleton<GameData>
{
    /// <summary>
    /// 所持しているキャラのデータ。
    /// バトルでは、nameやlevelなどバトル中に変更されない値のみを参照し、ステータスは都度計算する。
    /// バトル中に変更されるデータは参照しないように注意する(参照元(ここ)のデータも一緒に変更されてしまうので)。
    /// </summary>
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

        // TODO ランクなど
    }

    public List<OwnedCharaData> ownedCharaDataList = new();

    // public List<TeamAssemblyPop.TeamMemberInfo> playerTeamInfo;
    // public List<TeamAssemblyPop.TeamMemberInfo> opponentTeamInfo;  // TODO いらない？

    public int clearMapNo = 0;  // クリアしたマップの番号。この値+1が次のマップ番号
    public int clearStageNo = 0;

    // TODO ownedCharaDataListへの追加処理(ガチャで新しいキャラを手に入れた際)
}
