using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public List<GameData.CharaData> playerTeam = new();
    public List<GameData.CharaData> opponentTeam = new();

    [SerializeField] private TeamAssemblyPop teamAssemblyPop;


    /// <summary>
    /// バトルの準備
    /// </summary>
    private void PrepareBattle()
    {
        // TODO バトル後に移動
        playerTeam.Clear();
        opponentTeam.Clear();

        // 各チーム各キャラのステータスを計算
        foreach (var data in teamAssemblyPop.playerTeamInfo)
        {
            playerTeam.Add(CalculateManager.instance.CalculateCharaStatus(data.name, data.level));
        }
        foreach (var data in teamAssemblyPop.opponentTeamInfo)
        {
            opponentTeam.Add(CalculateManager.instance.CalculateCharaStatus(data.name, data.level));
        }

        // TODO PrepareNextTurn
    }

    // TODO 味方1番手→敵1番手→味方2番手の順に攻撃。全てのキャラが1回攻撃し終わったら2ターン目に突入
}
