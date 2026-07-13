using UnityEngine;
using UniRx;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;

/// <summary>
/// HP表示の更新タイミング
/// </summary>
public enum HpDisplayUpdateTiming
{
    Immediate,
    Delayed,
}

/// <summary>
/// バトル時、キャラクター制御クラス
/// (このクラスはバトルで使われるキャラクターの数だけインスタンスが作られるので、singletonやstaticにはしない)
/// </summary>
public class CharaController
{
    private CharacterBase chara;  // キャラの(クラス)インスタンスを生成して代入
    public CharacterBase Chara { get; private set; }

    private readonly CharaName name;
    public CharaName Name => name;

    private readonly Attribute attribute;  // 属性はずっと変わらない情報なのでreadonly
    public Attribute Attribute => attribute;

    private CalculateManager.VariableStatus status = new();
    public CalculateManager.VariableStatus Status => status;

    private ReactiveProperty<int> displayedHp = new();  // 内部処理のHPと、ゲーム画面に表示されるHPを分けて考える
    public IReadOnlyReactiveProperty<int> DisplayedHp => displayedHp;

    public int active1RemainingCoolTime;
    public int active2RemainingCoolTime = 1;  // スキル1を最初に優先して発動するため、初期値は1に設定  // TODO これいらなくね？

    private PassiveSkillState passive1State;
    private PassiveSkillState passive2State;

    public ReactiveProperty<bool> ReceivedCriticalDamage = new(false);  // 競合が起きるので、この値は各キャラクラスからは変更しない

    private CharaStatusPannel charaStatusPannel;
    public CharaStatusPannel CharaStatusPannel
    {
        get => charaStatusPannel;
        set => charaStatusPannel = value;
    }

    // TODO Listに変更?
    private Func<int, int> incomingDamageModifier = damage => damage;  // damage => damageで、デフォルトで受け取った値をそのまま返すように制御。(incomingDamagemodifierに値を設定しないキャラでは、中身はnullでエラーが発生する)

    private CompositeDisposable disposables = new();


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="statusData"></param>
    public CharaController(CalculateManager.VariableStatus statusData, CharaName charaName)
    {
        // 計算後の各ステータスの値を受け取り、キャラに反映
        status = statusData;
        displayedHp.Value = status.Hp.Value;
        name = charaName;  // TODO 削除

        // 属性をスクリプタブルオブジェクトから取得
        attribute = DataBaseManager.instance.charaInitialDataSO.charaInitialDataList.FirstOrDefault(data => data.englishName == charaName).attribute;

        // キャラクターとスキルを紐付け
        CreateSkillEffect(charaName);

        // 戦闘不能になった時、バフを全て削除
        status.Hp
            .Where(value => value <= 0)
            .Take(1)
            .Subscribe(_ => status.Buffs.Clear());

        // パッシブの状態を管理するクラスの作成
        passive1State = new(chara.Passive1Config);
        passive2State = new(chara.Passive2Config);
    }

    /// <summary>
    /// 該当キャラクターのクラスのインスタンスを生成し、変数に代入して、キャラとスキルを紐付ける
    /// </summary>
    /// <param name="charaName"></param>
    private void CreateSkillEffect(CharaName charaName)
    {
        // 引数で指定された型のクラスを取得
        Type type = Type.GetType(charaName.ToString());

        // クラスが見つからないか、CharacterBaseを継承していない場合
        if (type == null || !typeof(CharacterBase).IsAssignableFrom(type))  // typeofで、指定したクラスの型情報を取得。IsAssignableFromで、指定した型が先ほど取得した型情報に代入可能か(=継承しているか)を判定。typeof(基底クラス).IsAssignableFrom(派生クラス)。
        {
            Debug.Log("キャラクターのクラスを取得できません");
            return;
        }

        // type型のインスタンスを生成し、変数に代入 (CharacterBaseを継承しているクラス(該当のキャラの名前のクラス)が代入される)
        chara = (CharacterBase)Activator.CreateInstance(type);  // Activator.CreateInstance()は、new()と同じ処理。インスタンスするクラスのコンストラクタが引数を持つ場合、第二引数以降に記述する
    }

    /// <summary>
    /// HPの更新
    /// </summary>
    /// <param name="amount"></param>
    public int UpdateHp(int amount, HpDisplayUpdateTiming displayUpdateTiming = HpDisplayUpdateTiming.Immediate)
    {
        // 「ダメージ無効」状態の場合、ダメージを受けない
        if (amount < 0 && status.Buffs.Any(buff => buff.type == BuffType.ダメージ無効))
            return status.Hp.Value;
        
        // HPの実データを更新
        status.Hp.Value = Mathf.Clamp(status.Hp.Value + amount, 0, status.MaxHp.Value);

        if (displayUpdateTiming == HpDisplayUpdateTiming.Immediate)
            SetDisplayedHp(status.Hp.Value);  // HP表示の更新

        return status.Hp.Value;
    }

    /// <summary>
    /// 表示用HPを更新
    /// </summary>
    /// <param name="hp"></param>
    public async void SetDisplayedHp(int hp, float delay = 0f)
    {
        if (delay > 0f)
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

        displayedHp.Value = Mathf.Clamp(status.Hp.Value, 0, status.MaxHp.Value);
    }

    /// <summary>
    /// 被ダメージを補正
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public int ModifyIncomingDamage(int damage)
    {
        return incomingDamageModifier(damage);
    }

    /// <summary>
    /// 被ダメージ補正処理をセット
    /// </summary>
    /// <param name="modifier"></param>
    public void SetInComingDamageModifier(Func<int, int> modifier)
    {
        incomingDamageModifier = modifier;
    }

    /// <summary>
    /// アクティブスキルを発動
    /// </summary>
    public void ExecuteActiveSkill(BattleManager battleManager)
    {
        // 「気絶」または「睡眠」状態の場合、行動不能
        if (status.Buffs.Any(debuff => debuff.type == BuffType.気絶 || debuff.type == BuffType.睡眠))
        {
            return;
        }

        // 「沈黙」状態の場合、スキルを使用できない
        bool canUseSkill = !status.Buffs.Any(debuff => debuff.type == BuffType.沈黙);

        // スキル使用(スキル1、スキル2...の順番、全てのスキルがクールタイム中の場合、通常攻撃。)
        if (canUseSkill && active1RemainingCoolTime <= 0)
        {
            chara.ActiveSkill1(this);

            // クールタイムを追加
            active1RemainingCoolTime += chara.Active1CoolTime;

            // キャラ画像の表示、エフェクト再生
            battleManager.SetSkillUserImage(true, name);
            BattleAnimationManager.instance.AddAnimation(this, AnimationType.ActiveSkill);
        }
        else if (canUseSkill && active2RemainingCoolTime <= 0)
        {
            chara.ActiveSkill2(this);
            active2RemainingCoolTime += chara.Active2CoolTime;

            battleManager.SetSkillUserImage(true, name);
            BattleAnimationManager.instance.AddAnimation(this, AnimationType.ActiveSkill);
        }
        else
        {
            chara.BasicAttack(this);
            battleManager.SetSkillUserImage(false);
        }
    }

    /// <summary>
    /// パッシブスキルを発動
    /// </summary>
    /// <param name="config"></param>
    /// <param name="state"></param>
    /// <param name="currentTurnCount"></param>
    public void ExecutePassiveSkill(PassiveActivationTiming activationTiming, BattleManager battleManager)
    {
        TryExecutePassiveSkill(chara.Passive1Config, passive1State, activationTiming, battleManager.TurnCount, () => chara.MeetsPassive1ActivationCondition(this), () => chara.PassiveSkill1(this));
        TryExecutePassiveSkill(chara.Passive2Config, passive2State, activationTiming, battleManager.TurnCount, () => chara.MeetsPassive2ActivationCondition(this), () => chara.PassiveSkill2(this));
    }
    
    /// <summary>
    /// パッシブスキルが発動可能か判定し、可能な場合は実行する
    /// </summary>
    /// <param name="config"></param>
    /// <param name="state"></param>
    /// <param name="activationTiming"></param>
    /// <param name="currentTurn"></param>
    /// <param name="condition"></param>
    /// <param name="passive"></param>
    private void TryExecutePassiveSkill(PassiveSkillConfig config, PassiveSkillState state, PassiveActivationTiming activationTiming, int currentTurn, Func<bool> condition, Action passive)
    {
        // 共通の発動条件判定
        if (!CanExecutePassiveSkill(config, state, activationTiming, currentTurn))
            return;
        // キャラ固有の発動条件判定
        if (!condition())
            return;

        passive();
        // Debug.Log(config == chara.Passive1Config ? $"パッシブスキル1が発動しました：{name}" : $"パッシブスキル2が発動しました：{name}");  // TODO 確認終わったら消す

        state.remainingDuration = config.duration;
        state.remainingCountForReactivation = config.requiredCountForReactivation;

        state.remainingActivationCount--;
        if (state.remainingActivationCount <= 0)
        {
            state.isDisabled = true;
            // Debug.Log($"パッシブスキルを無効化：{name}");  // TODO 確認終わったら消す
        }
    }

    /// <summary>
    /// パッシブスキルが発動可能かどうか判定
    /// </summary>
    /// <param name="config"></param>
    /// <param name="state"></param>
    /// <param name="activationTiming"></param>
    /// <param name="currentTurn"></param>
    /// <returns></returns>
    private bool CanExecutePassiveSkill(PassiveSkillConfig config, PassiveSkillState state, PassiveActivationTiming activationTiming, int currentTurn)
    {
        if (config.activationTiming != activationTiming)
            return false;
        if (state.isDisabled)
            return false;
        if (activationTiming == PassiveActivationTiming.BattleStart)
            return true;
        if (currentTurn < config.startTurn)
            return false;
        if (state.remainingDuration > 0)  // パッシブが発動中
            return false;
        if (state.remainingCountForReactivation > 0)
            return false;

        return true;
    }

    /// <summary>
    /// ターン終了時の処理
    /// </summary>
    public void OnTurnEnded()
    {
        active1RemainingCoolTime--;
        active2RemainingCoolTime--;

        passive1State.remainingDuration--;
        passive2State.remainingDuration--;

        // パッシブスキル再発動に必要なターン数を減少
        ReduceRequiredCountForPassiveReactivation(chara.Passive1Config, passive1State, PassiveReactivationBasis.Turn);
        ReduceRequiredCountForPassiveReactivation(chara.Passive2Config, passive2State, PassiveReactivationBasis.Turn);

        // バフのクールタイムを減少
        foreach (var buff in status.Buffs.Where(x => !x.isIrremovable).ToList())  // ToList()で、foreachがコピーリストを参照するようにする。(下の処理でRemoveBuff()が動き元Listの要素が削除されエラーが出るのを防ぐ)
            buff.Duration.Value = ReduceBuffCoolTime(buff.Duration.Value);
    }

    /// <summary>
    /// 行動終了時の処理
    /// </summary>
    public void OnActionEnded()
    {   
        // パッシブスキル再発動に必要な行動回数を減少
        ReduceRequiredCountForPassiveReactivation(chara.Passive1Config, passive1State, PassiveReactivationBasis.Action);
        ReduceRequiredCountForPassiveReactivation(chara.Passive2Config, passive2State, PassiveReactivationBasis.Action);
    }

    /// <summary>
    /// バフのクールタイムを1減らす
    /// EffectValueの初期値を-1にしている関係で、値==0のみでバフが削除されるようになっているため、バフはこのメソッドを通してクールタイムを減らす
    /// </summary>
    /// <param name="reductionValue"></param>
    /// <returns></returns>
    private int ReduceBuffCoolTime(int reductionValue)
    {
        return Mathf.Max(reductionValue - 1, 0);
    }

    /// <summary>
    /// パッシブ再発動に必要なターン数or行動回数を減少
    /// </summary>
    /// <param name="config"></param>
    /// <param name="state"></param>
    /// <param name="reactivationBasis"></param>
    private void ReduceRequiredCountForPassiveReactivation(PassiveSkillConfig config, PassiveSkillState state, PassiveReactivationBasis reactivationBasis)
    {
        if (config.reactivationBasis != reactivationBasis)
            return;

        state.remainingCountForReactivation--;
    }

    /// <summary>
    /// ReactivePropertyの監視処理を停止(CharaController破棄時に使う)
    /// </summary>
    public void Dispose()
    {
        disposables.Dispose();
    }
}
