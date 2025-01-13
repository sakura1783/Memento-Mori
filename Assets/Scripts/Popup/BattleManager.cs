using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public enum BattleState
{
    Continue,
    Win,
    Lose,
}

public class BattleManager : PopupBase
{
    public List<CharaController> playerTeam = new();  // CharaStatusPannelクラスからCharaControllerクラスに変更！=> SkillManager等の処理がうまくいく気がする！
    public List<CharaController> opponentTeam = new();

    [SerializeField] private TeamAssemblyPop teamAssemblyPop;

    [SerializeField] private CharaStatusPannel charaStatusPennel;

    [SerializeField] private Transform playerTran;
    [SerializeField] private Transform opponentTran;

    private int turnCount;
    public int TurnCount => turnCount;

    private BattleState battleState;

    private CharaController previousActChara;  // 直前に(前回)行動したキャラ
    public CharaController PreviousActChara => previousActChara;


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
        // 前回のバトルで変更された情報を初期化  // TODO Initializeメソッド作るか、OnBattleEndメソッドにまとめる？
        turnCount = 0;
        battleState = BattleState.Continue;

        // 各チーム各キャラのステータスを計算し、リストに追加  // TODO リファクタリング
        foreach (var data in teamAssemblyPop.playerTeamInfo)
        {
            // CharaControllerの作成(キャラの制御)
            CharaController chara = new CharaController(CalculateManager.CalculateCharaStatus(data.name, data.level), data.name);
            playerTeam.Add(chara);  // チームのリストにキャラを追加

            // CharaPannelの生成(キャラの状態の可視化)
            var charaPannel = Instantiate(charaStatusPennel, playerTran);
            charaPannel.Setup(chara, data);

            // キャラが戦闘不能になったら、リストから削除  // TODO if(戦闘不能なら)を各処理に追加するか、無視してそのまま処理を実行するか？(CharaControllerは破棄されないため)
            chara.Status.Hp
                .Where(value => value <= 0)
                .Subscribe(_ => playerTeam.Remove(chara))
                .AddTo(this);
        }
        foreach (var data in teamAssemblyPop.opponentTeamInfo)
        {
            var chara = new CharaController(CalculateManager.CalculateCharaStatus(data.name, data.level), data.name);
            opponentTeam.Add(chara);

            var charaPannel = Instantiate(charaStatusPennel, opponentTran);
            charaPannel.Setup(chara, data);

            chara.Status.Hp
                .Where(value => value <= 0)
                .Subscribe(_ => opponentTeam.Remove(chara))
                .AddTo(this);
        }

        Battle();
    }

    /// <summary>
    /// バトル〜終了まで
    /// </summary>
    private void Battle()
    {
        // 勝敗がつくまでターンをループ
        do
        {
            ExecuteTurn();

            // クールタイムを減らす
            // playerTeam.ForEach(chara => chara.ReduceCoolTimeByTurn());
            // opponentTeam.ForEach(chara => chara.ReduceCoolTimeByTurn());
            foreach (var chara in playerTeam.Concat(opponentTeam))  // 上記の処理を簡略化
            {
                chara.ReduceCoolTimeByTurn();
            }

            turnCount++;

        }while (battleState == BattleState.Continue);

        // バトル後の処理
        OnBattleEnd();
    }

    /// <summary>
    /// バトルのターン内で行う処理
    /// </summary>
    /// <returns>バトルを終わるかどうか。trueでバトル続行(ターンを繰り返す)、trueでバトル終了(このメソッドだけでなく、Battle()からも抜ける)</returns>
    private void ExecuteTurn()
    {
        foreach (var chara in playerTeam.Concat(opponentTeam))  // Concat()でリスト2つを結合し、処理を簡素化
        {
            // 「毒」状態の場合、現在HP*?%のダメージを受ける
            var poisonDebuff = chara.Status.Buffs.FirstOrDefault(buff => buff.type == BuffType.毒);
            if (poisonDebuff != null) chara.UpdateHp(-CalculateManager.CalculateSkillEffectValue(chara.Status.Hp.Value, poisonDebuff.effectRate));
 
            // 「再生」状態の場合、HPを最大HP*?%回復
            var regenerationBuff = chara.Status.Buffs.FirstOrDefault(buff => buff.type == BuffType.再生);
            if (regenerationBuff != null) chara.UpdateHp(-CalculateManager.CalculateSkillEffectValue(chara.Status.MaxHp.Value, regenerationBuff.effectRate));
        }
        
        int count = 0;  // do-while文が何回回ったか
        // 味方1番手→敵1番手→味方2番手...の順に行動  // TODO 素早さの順に攻撃
        do
        {
            // 味方の行動
            //if (playerTeam[count] != null)
            if (playerTeam.Count > count)
            {
                playerTeam[count].ExecuteActiveSkill();
                previousActChara = playerTeam[count];

                foreach (var chara in playerTeam.Concat(opponentTeam)) chara.ReceivedCriticalDamage = false;

                // 行動後、バトル終了かどうかを判定。終了の場合trueを返し、Battle()内の処理によって、Battle()内からも抜け出す
                if (IsBattleOver()) return;  // <= return IsBattleOver()では、似ているようで違う処理になってしまうのでダメ。
            }

            // 敵の行動
            //if (opponentTeam[count] != null)
            if (opponentTeam.Count > count)
            {
                opponentTeam[count].ExecuteActiveSkill();
                previousActChara = opponentTeam[count];

                foreach (var chara in playerTeam.Concat(opponentTeam)) chara.ReceivedCriticalDamage = false;

                if (IsBattleOver()) return;
            }

            count++;

        } while (count < playerTeam.Count && count < opponentTeam.Count);  // 全員が1回行動するまで繰り返す

        // 次のターンへ(全てのキャラが一回攻撃し終えたらターンを進める)
        return;
    }

    /// <summary>
    /// バトルを終了させるかどうか判断する。trueで終了
    /// </summary>
    /// <returns></returns>
    private bool IsBattleOver()
    {
        if (playerTeam.All(chara => chara.Status.Hp.Value <= 0))  // All(条件)で、要素全てがその条件を満たしているかを判定する
        {
            battleState = BattleState.Lose;
            return true;
        }
        else if (opponentTeam.All(chara => chara.Status.Hp.Value <= 0))
        {
            battleState = BattleState.Win;
            return true;
        }
        else
        {
            battleState = BattleState.Continue;
            return false;
        }
    }

    /// <summary>
    /// バトルが終わった後の処理
    /// </summary>
    private void OnBattleEnd()
    {
        Debug.Log($"{battleState}");

        PopupManager.instance.GetPopup<ResultPop>().ShowPopup(battleState);
        
        // TODO 生成したゲームオブジェクト等を全て破棄して、最初の状態に戻す

        // 一旦コメントアウト
        // playerTeam.Clear();
        // opponentTeam.Clear();
    }
}
