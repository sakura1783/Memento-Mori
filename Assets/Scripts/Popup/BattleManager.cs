using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BattleManager : PopupBase
{
    public List<CharaController> playerTeam = new();  // TODO CharaStatusPannelクラスからCharaControllerクラスに変更！=> SkillManager等の処理がうまくいく気がする！
    public List<CharaController> opponentTeam = new();

    [SerializeField] private CharaStatusPannel charaStatusPennel;

    [SerializeField] private Transform playerTran;
    [SerializeField] private Transform opponentTran;

    [SerializeField] private TeamAssemblyPop teamAssemblyPop;


    public override void ShowPopup()
    {
        base.ShowPopup();
        PrepareBattle();
    }

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
        // 無限ループになるので一旦コメントアウト
        // do
        // {
        //     ExecuteTurn();

        // } while (!IsBattleOver());
    }

    /// <summary>
    /// バトルのターン内で行う処理
    /// </summary>
    private void ExecuteTurn()
    {
        int count = 0;  // do-while文が何回回ったか

        // TODO 味方1番手→敵1番手→味方2番手...の順に攻撃  // TODO 素早さの順に攻撃

        do
        {
            // 味方の攻撃
            // playerTeam[count].//スキルを使用
            // CharaControllerに

            // 敵の攻撃

            // 各攻撃後、IsBattleOverでバトルを終了するか判定する

            // 次のターンへ(全てのキャラが一回攻撃し終えたらターンを進める)

            count++;

        } while (count < playerTeam.Count && count < opponentTeam.Count);  // 全員が1回行動するまで繰り返す
    }

    /// <summary>
    /// バトルを終了させるかどうか判断する。trueで終了
    /// </summary>
    /// <returns></returns>
    private bool IsBattleOver()
    {
        // どちらのチームが敗北したかを判定
        bool isPlayerDefeated = playerTeam.All(chara => chara.Hp.Value <= 0);  // All(条件)で、要素全てがその条件を満たしているかを判定する
        bool isOpponentDefeated = opponentTeam.All(chara => chara.Hp.Value <= 0);

        if (isPlayerDefeated)
        {
            // プレイヤーが負けた際の処理、または負けたことが分かるようにする
            // 勝敗(勝ったか、負けたか)を何かしらの形で返す
            return true;
        }
        else if (isOpponentDefeated)
        {
            //

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
