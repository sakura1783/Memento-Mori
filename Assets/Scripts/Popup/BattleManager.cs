using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    public List<CharaController> playerTeam = new();  // TODO CharaStatusPannelクラスからCharaControllerクラスに変更！=> SkillManager等の処理がうまくいく気がする！
    public List<CharaController> opponentTeam = new();

    [SerializeField] private CharaStatusPannel charaStatusPennel;

    [SerializeField] private Transform playerTran;
    [SerializeField] private Transform opponentTran;

    [SerializeField] private TeamAssemblyPop teamAssemblyPop;


    /// <summary>
    /// バトルの準備
    /// </summary>
    private void PrepareBattle()
    {
        // 各チーム各キャラのステータスを計算し、リストに追加
        foreach (var data in teamAssemblyPop.playerTeamInfo)
        {
            // CharaControllerの作成(キャラの制御)
            var chara = new CharaController(CalculateManager.instance.CalculateCharaStatus(data.name, data.level));
            playerTeam.Add(chara);  // チームのリストにキャラを追加

            // CharaPannelの生成(キャラの状態の可視化)
            var charaPannel = Instantiate(charaStatusPennel, playerTran);
            charaPannel.Setup(chara, data);
        }
        foreach (var data in teamAssemblyPop.opponentTeamInfo)
        {
            var chara = new CharaController(CalculateManager.instance.CalculateCharaStatus(data.name, data.level));
            opponentTeam.Add(chara);

            var charaPannel = Instantiate(charaStatusPennel, opponentTran);
            charaPannel.Setup(chara, data);
        }

        Battle();
    }

    /// <summary>
    /// バトルの本処理
    /// </summary>
    private void Battle()
    {
        // TODO 味方1番手→敵1番手→味方2番手の順に攻撃

        for (int i = 0; i < playerTeam.Count; i++)
        {
            // TODO 味方の攻撃

            // TODO 敵の攻撃

            // TODO 各攻撃後、IsBattleOverでバトルを終了するか判定する

            // TODO 次のターンへ(全てのキャラが一回攻撃し終えたらターンを進める)
        }
    }

    /// <summary>
    /// バトルを終了させるかどうか判断する
    /// </summary>
    /// <returns></returns>
    private bool IsBattleOver()
    {
        // どちらのチームが敗北したかを判定
        bool isPlayerDefeated = playerTeam.All(chara => chara.Hp.Value <= 0);  // All(条件)で、要素全てがその条件を満たしているかを判定する
        bool isOpponentDefeated = opponentTeam.All(chara => chara.Hp.Value <= 0);

        if (isPlayerDefeated)
        {
            // TODO プレイヤーが勝った際の処理、または勝ったことが分かるようにする
            // TODO 勝敗(勝ったか、負けたか)を何かしらの形で返す
            return true;
        }
        else if (isOpponentDefeated)
        {
            // TODO

            return true;
        }
        else
        {
            return false;
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
