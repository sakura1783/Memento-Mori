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
        }
    }

    public List<TeamMemberInfo> playerTeamInfo;  // プレイヤーチームに編成されたキャラの、最低限の情報群。この情報を使ってバトルの最初で各キャラのステータスを計算する
    public List<TeamMemberInfo> opponentTeamInfo;

    [SerializeField] private Button btnFight;

    [SerializeField] private Transform charactersTran;

    [SerializeField] private CharaButton charaButtonPrefab;


    public void Setup()
    {
        // キャラボタンを生成
        foreach (var data in GameData.instance.ownedCharaDataList)
        {
            var charaButton = Instantiate(charaButtonPrefab, charactersTran);
            charaButton.Setup(data, this);
        }

        btnFight.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2f))
            .Subscribe(_ => AssembleOpponentTeam());
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
