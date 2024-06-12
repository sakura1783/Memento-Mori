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
        // 各チーム各キャラのステータスを計算
        foreach (var data in teamAssemblyPop.playerTeamInfo)
        {
            playerTeam.Add(CalculateManager.instance.CalculateCharaStatus(data.name, data.level));
        }
        foreach (var data in teamAssemblyPop.opponentTeamInfo)
        {
            opponentTeam.Add(CalculateManager.instance.CalculateCharaStatus(data.name, data.level));
        }

        // TODO CharaStatusPannelの生成。リストで管理

        Battle();
    }

    // TODO 味方1番手→敵1番手→味方2番手の順に攻撃。全てのキャラが1回攻撃し終わったら2ターン目に突入

    /// <summary>
    /// バトルの本処理
    /// </summary>
    private void Battle()
    {
        // TODO while (敵もしくは味方全員のHPが0になるまでループ)

        for (int i = 0; i < playerTeam.Count; i++)
        {
            // TODO 味方の攻撃

            // TODO 敵の攻撃
        }
    }

    /// <summary>
    /// バトルが終わった後の処理
    /// </summary>
    private void OnBattleEnd()
    {
        playerTeam.Clear();
        opponentTeam.Clear();
    }
}
