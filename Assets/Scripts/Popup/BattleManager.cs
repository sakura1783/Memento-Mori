using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    private readonly List<CharaController> playerTeam = new();  // 内部からだけ、要素の変更を許可(外部からは参照のみ許可)
    public IReadOnlyList<CharaController> PlayerTeam => playerTeam;

    private readonly List<CharaController> opponentTeam = new();
    public IReadOnlyList<CharaController> OpponentTeam => opponentTeam;

    [SerializeField] private TeamAssemblyPop teamAssemblyPop;

    [SerializeField] private CharaStatusPannel allyCharaPanel;
    [SerializeField] private CharaStatusPannel opponentCharaPanel;

    [SerializeField] private RectTransform playerTran;
    [SerializeField] private RectTransform opponentTran;

    [SerializeField] private UnityEngine.UI.Image imgSkillUser;

    [SerializeField] private CanvasGroup skillUserImageGroup;
    public CanvasGroup SkillUserImageGroup => skillUserImageGroup;

    private int turnCount;
    public int TurnCount => turnCount;

    private BattleState battleState;

    private CharaController previousActChara;  // 直前に(前回)行動したキャラ
    public CharaController PreviousActChara => previousActChara;

    private List<GameObject> generatedObjs = new();

    private CompositeDisposable battleDisposables = new();


    public override void ShowPopup()
    {
        // 前回使用したオブジェクトと情報を削除
        generatedObjs.ForEach(obj => Destroy(obj));
        generatedObjs.Clear();

        playerTeam.Clear();
        opponentTeam.Clear();

        base.ShowPopup();
        PrepareBattle();  // TODO タイミング検討。Battle().Forget()の処理だけ、Show()の後？
    }

    /// <summary>
    /// バトルの準備
    /// </summary>
    private void PrepareBattle()
    {
        // 前回のバトルで変更された情報を初期化  // TODO Initializeメソッド作るか、OnBattleEndメソッドにまとめる？
        turnCount = 0;
        battleState = BattleState.Continue;

        CreateTeamCharacters(teamAssemblyPop.playerTeamInfo, playerTeam, playerTran);
        CreateTeamCharacters(teamAssemblyPop.opponentTeamInfo, opponentTeam, opponentTran);

        Battle().Forget();  // Forget()で、この非同期処理は待たずに開始するだけでOKと明示
    }

    /// <summary>
    /// チームの各キャラを生成
    /// </summary>
    /// <param name="teamCharaDatas"></param>
    /// <param name="team"></param>
    /// <param name="generateTran"></param>
    private void CreateTeamCharacters(List<GameData.CharaConstData> teamCharaDatas, List<CharaController> team, RectTransform generateTran)
    {
        foreach (var data in teamCharaDatas)
        {
            // CharaControllerの作成(キャラの制御)
            var chara = new CharaController(CalculateManager.CalculateCharaStatus(data.name, data.level), data.name);
            team.Add(chara);

            // CharaPannelの生成(キャラの状態の可視化)
            var panelPrefab = team == playerTeam ? allyCharaPanel : opponentCharaPanel;
            var charaPannel = Instantiate(panelPrefab, generateTran);
            charaPannel.Setup(chara, data);
            generatedObjs.Add(charaPannel.gameObject);

            // キャラが戦闘不能になったら、リストから削除
            chara.Status.Hp
                .Where(value => value <= 0)
                .Subscribe(_ => team.Remove(chara))
                .AddTo(battleDisposables);
        }
    }

    /// <summary>
    /// バトル〜終了まで
    /// </summary>
    private async UniTaskVoid Battle()  // UniTaskVoidで、待たない非同期処理だと明示
    {
        // バトル開始時パッシブを発動
        playerTeam.Concat(opponentTeam).ToList()
            .ForEach(chara => chara.ExecutePassiveSkill(PassiveActivationTiming.BattleStart, this));

        // 勝敗がつくまでターンをループ
        do
        {
            await ExecuteTurn();

            foreach (var chara in playerTeam.Concat(opponentTeam))
                chara.OnTurnEnded();

            turnCount++;
        }while (battleState == BattleState.Continue);

        OnBattleEnd();
    }

    /// <summary>
    /// バトルのターン内で行う処理
    /// </summary>
    /// <returns>バトルを終わるかどうか。trueでバトル続行(ターンを繰り返す)、trueでバトル終了(このメソッドだけでなく、Battle()からも抜ける)</returns>
    private async UniTask ExecuteTurn()
    {
        playerTeam.Concat(opponentTeam).ToList()
            .ForEach(chara => chara.ExecutePassiveSkill(PassiveActivationTiming.TurnStart, this));

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
        // 味方1番手→敵1番手→味方2番手...の順に行動  // TODO 素早さの順に攻撃、リファクタリング
        do
        {
            // 味方の行動
            if (playerTeam.Count > count)
            {
                previousActChara = playerTeam[count];

                playerTeam[count].ExecuteActiveSkill(this);
                playerTeam[count].OnActionEnded();

                // 1キャラの行動終了ごとに、receivedCriticalDamageをfalseにリセット(次のキャラの行動に影響しないようにする)。競合が起きるため、各キャラクラスのスキルメソッド内ではいじらない。
                foreach (var chara in playerTeam.Concat(opponentTeam))
                    chara.ReceivedCriticalDamage.Value = false;

                await BattleAnimationManager.instance.WaitAllAnimations();

                // 行動後、バトル終了かどうかを判定。終了の場合trueを返し、Battle()内の処理によって、Battle()内からも抜け出す
                if (IsBattleOver()) return;
            }

            // 敵の行動
            if (opponentTeam.Count > count)
            {
                previousActChara = opponentTeam[count];

                opponentTeam[count].ExecuteActiveSkill(this);
                opponentTeam[count].OnActionEnded();

                foreach (var chara in playerTeam.Concat(opponentTeam)) chara.ReceivedCriticalDamage.Value = false;

                await BattleAnimationManager.instance.WaitAllAnimations();

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

        battleDisposables.Clear();  // Disposeしてしまうと、battleDisposable自体がなくなってしまう

        HidePopup();
        PopupManager.instance.GetPopup<ResultPop>().ShowPopup(battleState);
    }

    /// <summary>
    /// スキル使用者の画像の表示/非表示を切り替える
    /// </summary>
    /// <param name="isVisible"></param>
    /// <param name="charaName"></param>
    public void SetSkillUserImage(bool isVisible, CharaName charaName = CharaName.None)
    {
        if (isVisible)
        {
            imgSkillUser.sprite = SpriteManager.instance.GetCharaSprite(charaName, CharaSpriteType.Full);
            skillUserImageGroup.DOFade(1, 0.2f).SetEase(Ease.Linear);
        }
        else
            skillUserImageGroup.DOFade(0, 0.2f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// キャラの所属チームと、チーム内での位置を取得
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public (List<CharaController>, int) GetCharaTeamAndIndex(CharaController target)
    {
        var team = playerTeam.Contains(target) ? playerTeam : opponentTeam;
        var charaIndex = team.FindIndex(chara => chara == target);

        return (team, charaIndex);
    }
}
