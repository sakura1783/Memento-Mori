using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TeamAssemblyPop : MonoBehaviour
{
    [SerializeField] private Button btnFight;

    [SerializeField] private Transform charactersTran;

    [SerializeField] private CharaButton charaButtonPrefab;

    [SerializeField] private BattleManager battleManager;


    public void Setup()
    {
        // キャラボタンを生成
        foreach (var data in GameData.instance.ownedCharaDataList)
        {
            var charaButton = Instantiate(charaButtonPrefab, charactersTran);
            charaButton.Setup(data, battleManager);
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
            var enemy = new BattleManager.TeamCharaData(enemyData.name, enemyData.level);
            battleManager.opponentTeam.Add(enemy);
        }
    }
}
