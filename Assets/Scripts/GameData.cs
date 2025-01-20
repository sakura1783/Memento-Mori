using System.Collections.Generic;

public class GameData : AbstractSingleton<GameData>
{
    // /// <summary>
    // /// 所持しているキャラのデータ。
    // /// バトルでは、nameやlevelなどバトル中に変更されない値のみを参照し、ステータスは都度計算する。
    // /// バトル中に変更されるデータは参照しないように注意する(参照元(ここ)のデータも一緒に変更されてしまうので)。
    // /// </summary>
    // [System.Serializable]
    // public class OwnedCharaData  // TODO このクラスと下の変数ともに必要かどうか考える
    // {
    //     public CharaName name;
    //     public int level;

    //     public int combatPower;
    //     public int attackPower;
    //     public int defencePower;
    //     public int hp;
    //     public float criticalRate;

    //     // ランクなど
    // }

    // public List<OwnedCharaData> ownedCharaDataList = new();

    /// <summary>
    /// バトルで変動しないキャラのデータ
    /// この情報から、CharaButtonゲームオブジェクトの見た目やホーム画面・バトル画面でのキャラクターのステータス計算などを行う
    /// </summary>
    [System.Serializable]
    public class CharaConstData
    {
        public CharaName name;
        public int level;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        public CharaConstData(CharaName name, int level)
        {
            this.name = name;
            this.level = level;
        }
    }

    /// <summary>
    /// 開催中のガチャ詳細
    /// </summary>
    public class CurrentGachaDetail  // TODO このクラス、ここに書くのが適切か？
    {
        public GachaType gachaType;
        public CharaName pickupChara;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gachaType"></param>
        /// <param name="pickupChara"></param>
        public CurrentGachaDetail(GachaType gachaType, CharaName pickupChara = CharaName.Rosevillea)
        {
            this.gachaType = gachaType;
            this.pickupChara = pickupChara;
        }
    }

    public List<CharaConstData> ownedCharaDataList = new();

    public int clearMapNo = 0;  // クリアしたマップの番号。この値+1が次のマップ番号
    public int clearStageNo = 0;

    public List<CurrentGachaDetail> currentGachaList = new();  // 開催中のガチャの情報。これを使ってゲーム実行時にGachaPop内にオブジェクトを生成


    // TODO ownedCharaDataListへの追加処理(ガチャで新しいキャラを手に入れた際)
}
