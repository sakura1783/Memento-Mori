using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public List<CharaStatusPannel> playerTeam = new();
    public List<CharaStatusPannel> opponentTeam = new();

    [SerializeField] private CharaStatusPannel charaStatusPennel;

    [SerializeField] private Transform playerTran;
    [SerializeField] private Transform opponentTran;

    [SerializeField] private TeamAssemblyPop teamAssemblyPop;


    /// <summary>
    /// バトルの準備
    /// </summary>
    private void PrepareBattle()
    {
        // 各チーム各キャラのステータスを計算し、各チームのリストに追加
        foreach (var data in teamAssemblyPop.playerTeamInfo)
        {
            // CharaControllerの作成(キャラの制御)
            var chara = new CharaController(CalculateManager.instance.CalculateCharaStatus(data.name, data.level));

            // CharaPannelの生成(キャラの状態の可視化)
            var charaPannel = Instantiate(charaStatusPennel, playerTran);
            charaPannel.Setup(chara, data);

            playerTeam.Add(charaPannel);
        }
        foreach (var data in teamAssemblyPop.opponentTeamInfo)
        {
            var chara = new CharaController(CalculateManager.instance.CalculateCharaStatus(data.name, data.level));

            var charaPannel = Instantiate(charaStatusPennel, opponentTran);
            charaPannel.Setup(chara, data);

            opponentTeam.Add(charaPannel);
        }

        Battle();
    }

    /// <summary>
    /// バトルの本処理
    /// </summary>
    private void Battle()
    {
        // TODO 味方1番手→敵1番手→味方2番手の順に攻撃。全てのキャラが1回攻撃し終わったら2ターン目に突入

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
