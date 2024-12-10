using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TeamAssemblyPop : MonoBehaviour
{
    /// <summary>
    /// 編成されたキャラの名前とレベル
    /// </summary>
    [System.Serializable]
    public class TeamMemberInfo
    {
        public CharaName name;
        public int level;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        public TeamMemberInfo(CharaName name, int level)
        {
            this.name = name;
            this.level = level;

            // TODO ここでplayerTeamInfoに追加？(現在はCharaButton.cs内で行っている)、ただしopponentTeamInfoへの追加処理でここを使わないのであれば。確認してから決める
        }
    }

    public List<TeamMemberInfo> playerTeamInfo;  // プレイヤーチームに編成されたキャラの、最低限の情報群。この情報を使ってバトルの最初で各キャラのステータスを計算する
    public List<TeamMemberInfo> opponentTeamInfo;

    [SerializeField] private Button btnFight;

    [SerializeField] private Transform charactersTran;  // 所持キャラ一覧

    [SerializeField] private Transform[] playerTeamCharaTran = new Transform[5];
    [SerializeField] private Transform[] opponentTeamCharaTran = new Transform[5];

    [SerializeField] private Text txtPlayerTeamCombat;
    [SerializeField] private Text txtOpponentTeamCombat;

    [SerializeField] private CharaButton charaButtonPrefab;


    /// <summary>
    /// ポップアップが開かれる時毎回行う処理
    /// </summary>
    public void Setup()
    {
        // キャラ一覧にキャラボタンを生成
        foreach (var data in GameData.instance.ownedCharaDataList)
        {
            var charaButton = Instantiate(charaButtonPrefab, charactersTran);
            charaButton.Setup(data, this);
        }

        btnFight.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2f))
            .Subscribe(_ => 
            {
                AssembleOpponentTeam();

                // TODO 編成したそれぞれのチームの情報を(BattleManagerか)GameDataへ渡す←GameDataかな

                // TODO ポップアップを閉じて、バトルへ
            });
    }

    /// <summary>
    /// 敵チームのキャラを編成
    /// </summary>
    private void AssembleOpponentTeam()
    {
        // 現在のマップを取得
        var stageData = DataBaseManager.instance.mapDataSO.mapDataList[GameData.instance.clearMapNo + 1].stageDataSO;

        // 現在のステージの敵のリストを取得
        foreach (var enemyData in stageData.stageDataList[GameData.instance.clearStageNo + 1].enemyDataList)
        {
            // 敵チームを編成
            var enemy = new TeamMemberInfo(enemyData.name, enemyData.level);
            opponentTeamInfo.Add(enemy);
        }
    }
}
